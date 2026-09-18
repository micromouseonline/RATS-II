"""Main window (`APP-1.8.1`/`.2`): thin Tkinter wiring around `AppCore`
(`APP-1.7`).

Builds the *entire* main-window layout up front, per the standing
convention set for `APP-1.8`-`.12` (`plans/app-1-8-main-window-layout.md`):
every region this stage through `APP-1.8.4` needs is laid out now, with
anything not yet wired shown disabled/placeholder rather than absent.
`.1` activated the connection toolbar (COM port/baud, Connect/Disconnect)
and DB file selection; `.2` activates the event/competition/entry
selection pane -- `.3`/`.4` activate the rest.

Layout uses a horizontal `ttk.PanedWindow` under a connection toolbar so
the three main regions resize independently, with weighted `grid`/`pack`
throughout and no fixed-pixel widget sizes (`Q-5`/`UI-1`'s resizable
standard, applied here from the first placeholder pass rather than
retrofitted).
"""
from __future__ import annotations

import queue
import sqlite3
import tkinter as tk
from pathlib import Path
from tkinter import filedialog, messagebox, ttk
from typing import Callable, Optional

import serial.tools.list_ports

from rats import config as config_module
from rats import db
from rats.core import AppCore
from rats.serial_transport import SerialReader, Transport, open_serial_port
from rats.state import AppState

REPO_ROOT = Path(__file__).resolve().parent.parent.parent.parent
DEFAULT_DB_PATH = REPO_ROOT / "sample_data" / "demo.db"

BAUD_RATES = [9600, 19200, 38400, 57600, 115200]
DEFAULT_BAUD = config_module.DEFAULT_BAUD

# Legacy hardcodes exactly these two choices (`Form1.cs:1669`,
# `Competition_Class_ComboBox.Items.AddRange(new object[2] { "Final", "Heats" })`)
# rather than reading distinct classes from the DB -- real data also has a
# "Playoff" class, unreachable via this dropdown in the legacy app too.
COMPETITION_CLASSES = ["Final", "Heats"]

WINDOW_TITLE = "RATS Contest Timing"
# Legacy Form1's designed size (`Form1.cs:2092`, `ClientSize = new Size(884, 522)`).
DEFAULT_WINDOW_SIZE = (884, 522)

# The window size is already saved to the config file on every close
# (`MainWindow.destroy`) so the data is being collected, but reopening at
# that saved size is switched off for now -- flip this once the `.2`-`.4`
# layout has stabilized enough that a remembered size stays meaningful
# rather than clipping a layout that's since grown. Leave the read-back
# code below in place; just this flag needs to change.
RESTORE_WINDOW_SIZE = False

# How often the after()-loop drains the reader thread's queue, and the most
# items it will process in one tick (bounds how long a single Tk callback
# can run if a burst of messages arrives at once).
POLL_INTERVAL_MS = 50
MAX_ITEMS_PER_TICK = 200


def list_available_ports() -> list[str]:
    """`comPort_ComboBox`'s population (`Form1_Load`), via `pyserial`
    instead of legacy VB's `Ports.SerialPortNames`."""
    return [p.device for p in serial.tools.list_ports.comports()]


def _open_db(path: Path) -> sqlite3.Connection:
    """Open `path` as a SQLite DB, failing fast if it isn't one. Querying
    `sqlite_master` (rather than a no-op like `SELECT 1`) forces SQLite to
    actually read the file's header, so a non-database file raises here
    instead of only failing later on first real use."""
    conn = sqlite3.connect(str(path))
    try:
        conn.execute("SELECT name FROM sqlite_master LIMIT 1")
    except sqlite3.Error:
        conn.close()
        raise
    return conn


class MainWindow(tk.Tk):
    """The app's root window. One instance per running app, alongside one
    `AppState`/`AppCore` (`ARCH-1`)."""

    def __init__(
        self,
        *,
        app_state: Optional[AppState] = None,
        conn: Optional[sqlite3.Connection] = None,
        db_path: Optional[Path] = None,
        open_serial_port_fn: Callable[[str, int], Transport] = open_serial_port,
    ):
        super().__init__()
        self.title(WINDOW_TITLE)
        self.bind("<Configure>", self._on_root_configure)

        self._open_serial_port_fn = open_serial_port_fn
        self._cfg = config_module.load_config()

        self.app_state = app_state if app_state is not None else AppState()
        if conn is not None:
            self.conn = conn
            self._db_path = db_path
        else:
            self._db_path = db_path if db_path is not None else self._resolve_startup_db_path()
            self.conn = self._open_startup_db(self._db_path)
        self.core = AppCore(self.app_state, self.conn)

        self._reader: Optional[SerialReader] = None
        self._queue: Optional["queue.Queue"] = None
        self._drain_after_id: Optional[str] = None

        self._competitions_by_id: dict = {}
        self._entries_by_id: dict = {}

        self._build_menu()
        self._build_layout()
        self._refresh_ports()
        self._apply_startup_geometry()
        self._load_event_context()

    # -- Window geometry ----------------------------------------------------

    def _apply_startup_geometry(self) -> None:
        width, height = DEFAULT_WINDOW_SIZE
        if RESTORE_WINDOW_SIZE and self._cfg.last_window_width and self._cfg.last_window_height:
            width, height = self._cfg.last_window_width, self._cfg.last_window_height
        self.geometry(f"{width}x{height}")

        # Split the three panes into equal thirds up front -- PanedWindow's
        # `weight` only governs how *extra* space is redistributed on a
        # later resize, not each pane's initial width, so the sashes need
        # setting explicitly once the window's actual width is known.
        self.update_idletasks()
        total_width = self._paned.winfo_width() or width
        third = total_width // 3
        self._paned.sashpos(0, third)
        self._paned.sashpos(1, 2 * third)

    def _on_root_configure(self, event: "tk.Event") -> None:
        if event.widget is not self:
            return
        self.title(f"{WINDOW_TITLE} ({self.winfo_width()}x{self.winfo_height()})")

    # -- Startup DB resolution ---------------------------------------------

    def _resolve_startup_db_path(self) -> Path:
        candidate = self._cfg.last_db_path
        if candidate and Path(candidate).is_file():
            return Path(candidate)
        return DEFAULT_DB_PATH

    def _open_startup_db(self, path: Path) -> sqlite3.Connection:
        try:
            return _open_db(path)
        except sqlite3.Error:
            return _open_db(DEFAULT_DB_PATH)

    # -- Layout --------------------------------------------------------------

    def _build_menu(self) -> None:
        menubar = tk.Menu(self)
        file_menu = tk.Menu(menubar, tearoff=False)
        file_menu.add_command(label="Open Database…", command=self._on_open_database)
        file_menu.add_separator()
        file_menu.add_command(label="Exit", command=self.destroy)
        menubar.add_cascade(label="File", menu=file_menu)
        self.config(menu=menubar)

    def _build_layout(self) -> None:
        self.columnconfigure(0, weight=1)
        self.rowconfigure(1, weight=1)

        self._build_toolbar()

        paned = ttk.PanedWindow(self, orient=tk.HORIZONTAL)
        paned.grid(row=1, column=0, sticky="nsew")
        self._paned = paned

        self._build_entry_pane(paned)
        self._build_run_pane(paned)
        self._build_monitor_pane(paned)

    def _build_toolbar(self) -> None:
        toolbar = ttk.Frame(self, padding=4)
        toolbar.grid(row=0, column=0, sticky="ew")
        toolbar.columnconfigure(5, weight=1)

        ttk.Label(toolbar, text="Port:").grid(row=0, column=0, padx=(0, 4))
        self.port_var = tk.StringVar()
        self.port_combo = ttk.Combobox(
            toolbar, textvariable=self.port_var, state="readonly", width=14
        )
        self.port_combo.grid(row=0, column=1, padx=(0, 8))

        ttk.Label(toolbar, text="Baud:").grid(row=0, column=2, padx=(0, 4))
        self.baud_var = tk.IntVar(value=self._cfg.last_baud or DEFAULT_BAUD)
        self.baud_combo = ttk.Combobox(
            toolbar,
            textvariable=self.baud_var,
            state="readonly",
            width=8,
            values=BAUD_RATES,
        )
        self.baud_combo.grid(row=0, column=3, padx=(0, 8))

        self.connect_button = ttk.Button(
            toolbar, text="Connect", command=self._on_connect_clicked
        )
        self.connect_button.grid(row=0, column=4, padx=(0, 12))

        db_label = f"DB: {self._db_path.name}" if self._db_path is not None else "DB: (unsaved)"
        self.db_label_var = tk.StringVar(value=db_label)
        ttk.Label(toolbar, textvariable=self.db_label_var, anchor="e").grid(
            row=0, column=5, sticky="e"
        )

    def _build_entry_pane(self, paned: ttk.PanedWindow) -> None:
        """Pane 1: event/competition/entry selection (`APP-1.8.2`).

        Practice-mode gating (`core.select_competition`'s docstring: "the
        competition grid is disabled during practice mode") is deferred to
        `APP-1.8.3`, which is what actually wires the Practice Mode toggle
        and its state transitions -- this pane works regardless of
        `practice_mode` for now, matching `.2`'s own success criterion (a
        manual smoke test against `sample_data/demo.db`, no toggle needed
        to reach it)."""
        frame = ttk.Frame(paned, padding=4)
        paned.add(frame, weight=1)
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(3, weight=1)
        frame.rowconfigure(6, weight=1)

        self.event_name_var = tk.StringVar(value="Event: --")
        ttk.Label(frame, textvariable=self.event_name_var).grid(row=0, column=0, sticky="w")
        self.event_date_var = tk.StringVar(value="Date: --")
        ttk.Label(frame, textvariable=self.event_date_var).grid(row=1, column=0, sticky="w")

        class_row = ttk.Frame(frame)
        class_row.grid(row=2, column=0, sticky="ew", pady=(4, 0))
        class_row.columnconfigure(1, weight=1)
        ttk.Label(class_row, text="Class:").grid(row=0, column=0)
        self.competition_class_var = tk.StringVar()
        self.competition_class_combo = ttk.Combobox(
            class_row,
            textvariable=self.competition_class_var,
            state="readonly",
            values=COMPETITION_CLASSES,
        )
        self.competition_class_combo.grid(row=0, column=1, sticky="ew", padx=(4, 0))
        self.competition_class_combo.bind(
            "<<ComboboxSelected>>", self._on_competition_class_selected
        )

        self.competition_tree = ttk.Treeview(
            frame, columns=("name",), show="headings", height=5, selectmode="browse"
        )
        self.competition_tree.heading("name", text="Competition")
        self.competition_tree.grid(row=3, column=0, sticky="nsew", pady=(4, 0))
        self.competition_tree.bind("<<TreeviewSelect>>", self._on_competition_selected)

        self.selected_competition_var = tk.StringVar(value="Selected: --")
        ttk.Label(frame, textvariable=self.selected_competition_var).grid(
            row=4, column=0, sticky="w", pady=(4, 0)
        )

        self.entry_tree = ttk.Treeview(
            frame, columns=("mouse",), show="headings", height=5, selectmode="browse"
        )
        self.entry_tree.heading("mouse", text="Mouse")
        self.entry_tree.grid(row=6, column=0, sticky="nsew", pady=(4, 0))
        self.entry_tree.bind("<<TreeviewSelect>>", self._on_entry_selected)

        self.selected_robot_var = tk.StringVar(value="Robot: --")
        ttk.Label(frame, textvariable=self.selected_robot_var).grid(
            row=7, column=0, sticky="w", pady=(4, 0)
        )
        self.current_contestant_var = tk.StringVar(value="Contestant: --")
        ttk.Label(frame, textvariable=self.current_contestant_var).grid(
            row=8, column=0, sticky="w"
        )

    def _build_run_pane(self, paned: ttk.PanedWindow) -> None:
        """Pane 2: run control buttons + live display (`APP-1.8.3`, inert
        here)."""
        frame = ttk.Frame(paned, padding=4)
        paned.add(frame, weight=1)
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(1, weight=1)

        button_row = ttk.Frame(frame)
        button_row.grid(row=0, column=0, sticky="ew")
        # Captions match the legacy app's actual on-screen text
        # (legacy/legacy-rats-screen.png), not the C# handler names --
        # attribute names below stay tied to those handler names for
        # cross-reference with behavior_inventory.md.
        button_specs = [
            ("Add Touch", "touch_button"),
            ("DNF", "dnf_button"),
            ("Clear Display", "clear_button"),
            ("New Robot", "new_mouse_button"),
            ("Practice <=> Contest", "practice_mode_button"),
            ("Extra Run", "extra_run_button"),
            ("WatchDog", "watchdog_button"),
        ]
        for col, (label, attr) in enumerate(button_specs):
            button_row.columnconfigure(col, weight=1)
            btn = ttk.Button(button_row, text=label, state="disabled")
            btn.grid(row=0, column=col, sticky="ew", padx=2, pady=2)
            setattr(self, attr, btn)

        values_frame = ttk.LabelFrame(frame, text="Live", padding=4)
        values_frame.grid(row=1, column=0, sticky="nsew", pady=(8, 0))
        values_frame.columnconfigure(1, weight=1)
        values_frame.columnconfigure(3, weight=1)

        value_specs = [
            ("Split", "split_time_var"),
            ("Maze", "maze_time_var"),
            ("Run", "run_time_var"),
            ("Score", "score_time_var"),
            ("Best score", "best_score_var"),
            ("Rank", "rank_var"),
            ("Time left", "time_left_var"),
            ("Run #", "run_number_var"),
            ("Runs allowed", "allowed_runs_var"),
            ("Touches", "touches_var"),
            ("WatchDog state", "watchdog_state_var"),
            ("Timer state", "timer_state_var"),
        ]
        for i, (label, attr) in enumerate(value_specs):
            row, half = divmod(i, 2)
            var = tk.StringVar(value="--")
            setattr(self, attr, var)
            ttk.Label(values_frame, text=f"{label}:").grid(
                row=row, column=half * 2, sticky="w", padx=(0, 4), pady=1
            )
            ttk.Label(values_frame, textvariable=var).grid(
                row=row, column=half * 2 + 1, sticky="w", padx=(0, 12), pady=1
            )

        scoring_frame = ttk.LabelFrame(frame, text="Scoring model", padding=4)
        scoring_frame.grid(row=2, column=0, sticky="ew", pady=(8, 0))
        scoring_specs = [
            ("Touch time", "touch_time_cfg_var"),
            ("Touch divider", "touch_divider_cfg_var"),
            ("Maze divider", "maze_divider_cfg_var"),
            ("Cumulative", "touch_cumulative_cfg_var"),
            ("Enabled", "touch_enabled_cfg_var"),
        ]
        for col, (label, attr) in enumerate(scoring_specs):
            scoring_frame.columnconfigure(col, weight=1)
            var = tk.StringVar(value="--")
            setattr(self, attr, var)
            cell = ttk.Frame(scoring_frame)
            cell.grid(row=0, column=col, sticky="ew", padx=4)
            ttk.Label(cell, text=label).pack(anchor="w")
            ttk.Label(cell, textvariable=var).pack(anchor="w")

    def _build_monitor_pane(self, paned: ttk.PanedWindow) -> None:
        """Pane 3: raw Tx/Rx monitor, log toggles, child-window launch
        buttons (`APP-1.8.4`/`.9`-`.12`, inert here)."""
        frame = ttk.Frame(paned, padding=4)
        paned.add(frame, weight=1)
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(1, weight=1)

        toggle_row = ttk.Frame(frame)
        toggle_row.grid(row=0, column=0, sticky="ew")
        self.monitor_toggle_button = ttk.Button(toggle_row, text="Monitor", state="disabled")
        self.monitor_toggle_button.pack(side="left")
        self.verbose_toggle_button = ttk.Button(toggle_row, text="Verbose", state="disabled")
        self.verbose_toggle_button.pack(side="left", padx=(4, 0))
        self.monitor_clear_button = ttk.Button(toggle_row, text="Clear", state="disabled")
        self.monitor_clear_button.pack(side="left", padx=(4, 0))

        self.monitor_text = tk.Text(frame, height=10, state="disabled", wrap="none")
        self.monitor_text.grid(row=1, column=0, sticky="nsew", pady=(4, 0))

        launch_frame = ttk.LabelFrame(frame, text="Windows", padding=4)
        launch_frame.grid(row=2, column=0, sticky="ew", pady=(8, 0))
        # The 3 single-channel display variants are captioned by resolution
        # in the legacy app (legacy/legacy-rats-screen.png), not "v2"/"wide"
        # as the C# class names (single_ch_display_v2/_16_9) suggest --
        # 612x595 is the original single_channel_display, 1280x720 the
        # 16:9 variant (UI-1's parametrized class covers all 3, APP-1.11).
        launch_specs = [
            ("Calibrate", "calibrate_button"),
            ("Run Order", "run_order_button"),
            ("612 x 595", "display_1ch_button"),
            ("800 x 600", "display_1ch_v2_button"),
            ("1280 x 720", "display_1ch_wide_button"),
            ("Results (1ch)", "results_1ch_button"),
            ("Name Contestants", "name_contestants_button"),
        ]
        for i, (label, attr) in enumerate(launch_specs):
            row, col = divmod(i, 4)
            launch_frame.columnconfigure(col, weight=1)
            btn = ttk.Button(launch_frame, text=label, state="disabled")
            btn.grid(row=row, column=col, sticky="ew", padx=2, pady=2)
            setattr(self, attr, btn)

    # -- Event / competition / entry selection (`APP-1.8.2`) ---------------

    def _load_event_context(self) -> None:
        """`Form1_Load`'s `Context` read (`Form1.cs:2183`), minus the
        "Stand Alone Mode" fallback -- `.1` already guarantees some DB is
        always open, so a missing `Context` row (an unlikely but possible
        malformed DB) just shows the placeholder text rather than a fake
        event name."""
        context = db.get_context(self.conn)
        if context is None:
            self.app_state.event.robotics_event_id = 0
            self.app_state.event.robotics_event = ""
            self.event_name_var.set("Event: --")
            self.event_date_var.set("Date: --")
            return
        self.app_state.event.robotics_event_id = context.current_event_id
        self.app_state.event.robotics_event = context.current_event
        self.event_name_var.set(f"Event: {context.current_event}")
        self.event_date_var.set(f"Date: {context.effective_date or '--'}")

    def _reset_competition_and_entry_state(self) -> None:
        """Clears everything downstream of "which DB is open" -- called
        when a new database is opened (`_on_open_database`), since the old
        DB's competitions/entries no longer apply."""
        self.competition_class_var.set("")
        for item in self.competition_tree.get_children():
            self.competition_tree.delete(item)
        self._competitions_by_id.clear()
        self.selected_competition_var.set("Selected: --")
        self._clear_entry_selection()

    def _clear_entry_selection(self) -> None:
        for item in self.entry_tree.get_children():
            self.entry_tree.delete(item)
        self._entries_by_id.clear()
        self.selected_robot_var.set("Robot: --")
        self.current_contestant_var.set("Contestant: --")

    def _on_competition_class_selected(self, event: object = None) -> None:
        """`Competition_Class_ComboBox_SelectedIndexChanged`, `Form1.cs:2237`."""
        for item in self.competition_tree.get_children():
            self.competition_tree.delete(item)
        self._competitions_by_id.clear()
        self.selected_competition_var.set("Selected: --")
        self._clear_entry_selection()

        selected_class = self.competition_class_var.get()
        if not selected_class:
            return
        competitions = db.select_competitions(
            self.conn, self.app_state.event.robotics_event_id, selected_class
        )
        for competition in competitions:
            iid = str(competition.competition_id)
            self._competitions_by_id[iid] = competition
            self.competition_tree.insert("", "end", iid=iid, values=(competition.competition_name,))

    def _on_competition_selected(self, event: object = None) -> None:
        """`Competition_DataGridview_SelectionChanged`, `Form1.cs:2261`."""
        selection = self.competition_tree.selection()
        if not selection:
            return
        competition = self._competitions_by_id.get(selection[0])
        if competition is None:
            return

        self.core.select_competition(competition)
        self.selected_competition_var.set(f"Selected: {competition.competition_name}")
        self._clear_entry_selection()

        for entry in db.select_pending_entries(self.conn, competition.competition_id):
            iid = str(entry.entry_id)
            self._entries_by_id[iid] = entry
            self.entry_tree.insert("", "end", iid=iid, values=(entry.mouse_name,))

    def _on_entry_selected(self, event: object = None) -> None:
        """`Mouse_DataGridview_SelectionChanged`, `Form1.cs:2335`. Legacy
        requires `SerialPort1.IsOpen` here (it writes `NewMouse` straight to
        the port) -- `core.start_new_entry()` already tolerates
        `transport is None` (`APP-1.7`), so this works with or without a
        connection instead of popping a "Connect a COM port first" dialog."""
        selection = self.entry_tree.selection()
        if not selection:
            return
        entry = self._entries_by_id.get(selection[0])
        if entry is None:
            return

        self.core.select_entry(entry.entry_id, entry.mouse_name)
        self.selected_robot_var.set(f"Robot: {self.app_state.entry.robot}")
        self.current_contestant_var.set(f"Contestant: {self.app_state.entry.contestant}")

    # -- Connection ------------------------------------------------------

    def _refresh_ports(self) -> None:
        ports = list_available_ports()
        self.port_combo["values"] = ports
        preferred = self._cfg.last_port
        if preferred and preferred in ports:
            self.port_var.set(preferred)
        elif ports:
            self.port_var.set(ports[0])

    def _on_connect_clicked(self) -> None:
        if self._reader is not None:
            self._disconnect()
        else:
            self._connect()

    def _connect(self) -> None:
        port_name = self.port_var.get()
        if not port_name:
            messagebox.showerror("Connect", "No serial port selected.")
            return
        try:
            baud = int(self.baud_var.get())
        except (TypeError, ValueError, tk.TclError):
            baud = DEFAULT_BAUD

        try:
            transport = self._open_serial_port_fn(port_name, baud)
        except Exception as exc:  # pyserial raises several distinct types
            messagebox.showerror("Connect", f"Could not open {port_name}: {exc}")
            return

        self._queue = queue.Queue()
        self._reader = SerialReader(transport, self._queue)
        self._reader.start()
        self.core.transport = transport
        self.app_state.connection.serial_port_opened = True

        self.port_combo.configure(state="disabled")
        self.baud_combo.configure(state="disabled")
        self.connect_button.configure(text="Disconnect")

        self._cfg.last_port = port_name
        self._cfg.last_baud = baud
        config_module.save_config(self._cfg)

        self._drain_after_id = self.after(POLL_INTERVAL_MS, self._drain_tick)

    def _disconnect(self) -> None:
        if self._drain_after_id is not None:
            self.after_cancel(self._drain_after_id)
            self._drain_after_id = None
        if self._reader is not None:
            self._reader.stop()
            self._reader = None
        self._queue = None
        self.core.transport = None
        self.app_state.connection.serial_port_opened = False

        self.port_combo.configure(state="readonly")
        self.baud_combo.configure(state="readonly")
        self.connect_button.configure(text="Connect")

    def _drain_tick(self) -> None:
        """The `after()`-driven queue-drain loop. Nothing downstream reacts
        to the drained items yet (`.2`/`.3` add that) -- this only proves
        the loop runs without error and reacts to a mid-session drop."""
        items = []
        if self._queue is not None:
            for _ in range(MAX_ITEMS_PER_TICK):
                try:
                    items.append(self._queue.get_nowait())
                except queue.Empty:
                    break
        if items:
            self.core.drain_queue(items)

        if not self.app_state.connection.serial_port_opened and self._reader is not None:
            self._disconnect()
            return

        self._drain_after_id = self.after(POLL_INTERVAL_MS, self._drain_tick)

    # -- Database ----------------------------------------------------------

    def _on_open_database(self) -> None:
        path_str = filedialog.askopenfilename(
            title="Open Database",
            filetypes=[("SQLite database", "*.db"), ("All files", "*.*")],
        )
        if not path_str:
            return
        path = Path(path_str)
        try:
            new_conn = _open_db(path)
        except sqlite3.Error as exc:
            messagebox.showerror("Open Database", f"Could not open {path.name}: {exc}")
            return

        old_conn = self.conn
        self.conn = new_conn
        self.core.conn = new_conn
        self._db_path = path
        self.db_label_var.set(f"DB: {path.name}")
        old_conn.close()

        self._reset_competition_and_entry_state()
        self._load_event_context()

        self._cfg.last_db_path = str(path)
        config_module.save_config(self._cfg)

    # -- Lifecycle -----------------------------------------------------------

    def destroy(self) -> None:
        if self._reader is not None:
            self._disconnect()
        # Saved unconditionally -- only reading it back on startup is
        # gated by RESTORE_WINDOW_SIZE, so the data is ready once that's on.
        self._cfg.last_window_width = self.winfo_width()
        self._cfg.last_window_height = self.winfo_height()
        config_module.save_config(self._cfg)
        super().destroy()


def main() -> None:
    window = MainWindow()
    window.mainloop()


if __name__ == "__main__":
    main()
