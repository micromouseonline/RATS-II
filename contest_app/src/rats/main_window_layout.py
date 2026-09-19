"""Pure widget layout for the main window -- no `AppCore`/DB/serial
dependency at all.

Split out of `main_window.py` so the entire visual layout can be built and
clicked in isolation, without a database, hardware, or any of the app's
real behavior wired up -- useful for iterating on layout/spacing (e.g.
`APP-1.8.5`'s review against `legacy/legacy-rats-screen.png`) without
needing to relaunch the full app each time.

`MainWindowLayout` builds every widget from `APP-1.8.1`-`.4` and wires each
button/selection to a same-named handler method -- but every handler here
is a harmless `pass` stub, so the class is fully constructible and
interactive on its own (see `main()` below). `rats.main_window.MainWindow`
subclasses this and overrides every stub with its real implementation;
Python's normal method resolution means the real app gets real behavior
and this module never needs to know that subclass exists.

Run directly (`python -m rats.main_window_layout`) to preview the layout.
"""
from __future__ import annotations

import tkinter as tk
from pathlib import Path
from tkinter import ttk
from typing import Optional

import serial.tools.list_ports
import sv_ttk

from rats import config as config_module
from rats.config import AppConfig

# UI-2: sv_ttk's light theme, chosen over stock ttk after the user compared
# both sv_ttk variants live against this window -- "an improvement", light
# preferred "for now". A known cost, not yet fixed: sv_ttk's wider button
# padding clips several run-control captions in their current pane width
# ("Clear Display" -> "Cle", etc.) -- left for APP-1.8.5's layout pass.
THEME = "light"

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

BAUD_RATES = [9600, 19200, 38400, 57600, 115200]
DEFAULT_BAUD = config_module.DEFAULT_BAUD

# Legacy hardcodes exactly these two choices (`Form1.cs:1669`,
# `Competition_Class_ComboBox.Items.AddRange(new object[2] { "Final", "Heats" })`)
# rather than reading distinct classes from the DB -- real data also has a
# "Playoff" class, unreachable via this dropdown in the legacy app too.
COMPETITION_CLASSES = ["Final", "Heats"]


def list_available_ports() -> list[str]:
    """`comPort_ComboBox`'s population (`Form1_Load`), via `pyserial`
    instead of legacy VB's `Ports.SerialPortNames`."""
    return [p.device for p in serial.tools.list_ports.comports()]


class MainWindowLayout(tk.Tk):
    """Builds the *entire* main-window layout up front, per the standing
    convention set for `APP-1.8`-`.12` (`plans/app-1-8-main-window-layout.md`).
    No `AppCore`/DB/serial dependency -- see module docstring."""

    def __init__(self, *, cfg: Optional[AppConfig] = None):
        super().__init__()
        sv_ttk.set_theme(THEME)
        self.title(WINDOW_TITLE)
        self.bind("<Configure>", self._on_root_configure)

        self._cfg = cfg if cfg is not None else config_module.load_config()
        self._db_path: Optional[Path] = None

        self._build_menu()
        self._build_layout()
        self._refresh_ports()
        self._apply_startup_geometry()

    # -- Window geometry ------------------------------------------------

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
        left_sash_pos = total_width * 260 // 800
        right_sash_pos = total_width * 480 // 800
        self._paned.sashpos(0, left_sash_pos)
        self._paned.sashpos(1, right_sash_pos)

    def _on_root_configure(self, event: "tk.Event") -> None:
        if event.widget is not self:
            return
        self.title(f"{WINDOW_TITLE} ({self.winfo_width()}x{self.winfo_height()})")

    # -- Layout -----------------------------------------------------------

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
        competition grid is disabled during practice mode") is wired in
        `APP-1.8.3` (`_set_entry_pane_enabled`, called from the Practice
        Mode toggle and once at startup) rather than here -- `practice_mode`
        defaults `true`, so this pane actually starts disabled until the
        user leaves practice mode, matching legacy."""
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
        """Pane 2: run control buttons + live display (`APP-1.8.3`)."""
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
            ("Add Touch", "touch_button", self._on_touch_clicked),
            ("DNF", "dnf_button", self._on_dnf_clicked),
            ("Clear Display", "clear_button", self._on_clear_clicked),
            ("New Robot", "new_mouse_button", self._on_new_mouse_clicked),
            ("Practice <=> Contest", "practice_mode_button", self._on_practice_mode_clicked),
            ("Extra Run", "extra_run_button", self._on_extra_run_clicked),
            ("WatchDog", "watchdog_button", self._on_watchdog_clicked),
        ]
        for col, (label, attr, handler) in enumerate(button_specs):
            button_row.columnconfigure(col, weight=1)
            btn = ttk.Button(button_row, text=label, command=handler)
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
        buttons (`APP-1.8.4`)."""
        frame = ttk.Frame(paned, padding=4)
        paned.add(frame, weight=1)
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(1, weight=1)

        toggle_row = ttk.Frame(frame)
        toggle_row.grid(row=0, column=0, sticky="ew")
        self.monitor_toggle_button = ttk.Button(
            toggle_row, text="Monitor", command=self._on_monitor_toggle_clicked
        )
        self.monitor_toggle_button.pack(side="left")
        self.verbose_toggle_button = ttk.Button(
            toggle_row, text="Verbose", command=self._on_verbose_toggle_clicked
        )
        self.verbose_toggle_button.pack(side="left", padx=(4, 0))
        self.monitor_clear_button = ttk.Button(
            toggle_row, text="Clear", command=self._on_monitor_clear_clicked
        )
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
        # All 3 display buttons share one command/flag per UI-1 -- they're
        # interchangeable skins of one window slot, not independent windows.
        # No target windows exist until APP-1.9-.12 -- these just toggle the
        # AppState.windows open flags for now.
        launch_specs = [
            ("Calibrate", "calibrate_button", self._on_calibrate_clicked),
            ("Run Order", "run_order_button", self._on_run_order_clicked),
            ("612 x 595", "display_1ch_button", self._on_display_clicked),
            ("800 x 600", "display_1ch_v2_button", self._on_display_clicked),
            ("1280 x 720", "display_1ch_wide_button", self._on_display_clicked),
            ("Results (1ch)", "results_1ch_button", self._on_results_clicked),
            ("Name Contestants", "name_contestants_button", self._on_name_contestants_clicked),
        ]
        for i, (label, attr, handler) in enumerate(launch_specs):
            row, col = divmod(i, 4)
            launch_frame.columnconfigure(col, weight=1)
            btn = ttk.Button(launch_frame, text=label, command=handler)
            btn.grid(row=row, column=col, sticky="ew", padx=2, pady=2)
            setattr(self, attr, btn)

    def _set_entry_pane_enabled(self, enabled: bool) -> None:
        """Practice-mode gating (`core.select_competition`'s docstring:
        "the competition grid is disabled during practice mode") -- called
        from the Practice Mode toggle and once at startup, since
        `practice_mode` defaults `true`."""
        self.competition_class_combo.configure(state="readonly" if enabled else "disabled")
        tree_state = ("!disabled",) if enabled else ("disabled",)
        self.competition_tree.state(tree_state)
        self.entry_tree.state(tree_state)

    def _append_monitor_line(self, text: str) -> None:
        self.monitor_text.configure(state="normal")
        self.monitor_text.insert("end", text + "\n")
        self.monitor_text.see("end")
        self.monitor_text.configure(state="disabled")

    def _on_monitor_clear_clicked(self) -> None:
        """`clear_BTN_Click`, `Form1.cs:2481` -- `commandCount` isn't
        ported (confirmed dead code: declared, reset twice, never read or
        incremented anywhere in the legacy app)."""
        self.monitor_text.configure(state="normal")
        self.monitor_text.delete("1.0", "end")
        self.monitor_text.configure(state="disabled")

    # -- Connection (port list only -- no real connect/disconnect here) ----

    def _refresh_ports(self) -> None:
        ports = list_available_ports()
        self.port_combo["values"] = ports
        preferred = self._cfg.last_port
        if preferred and preferred in ports:
            self.port_var.set(preferred)
        elif ports:
            self.port_var.set(ports[0])

    # -- Stub handlers ------------------------------------------------------
    #
    # Every widget above is wired to one of these. They do nothing here --
    # `rats.main_window.MainWindow` overrides each with real behavior. This
    # is what makes this class fully constructible and clickable on its own
    # (see the module docstring and `main()` below).

    def _on_open_database(self) -> None:
        pass

    def _on_connect_clicked(self) -> None:
        pass

    def _on_competition_class_selected(self, event: object = None) -> None:
        pass

    def _on_competition_selected(self, event: object = None) -> None:
        pass

    def _on_entry_selected(self, event: object = None) -> None:
        pass

    def _on_touch_clicked(self) -> None:
        pass

    def _on_dnf_clicked(self) -> None:
        pass

    def _on_clear_clicked(self) -> None:
        pass

    def _on_new_mouse_clicked(self) -> None:
        pass

    def _on_practice_mode_clicked(self) -> None:
        pass

    def _on_extra_run_clicked(self) -> None:
        pass

    def _on_watchdog_clicked(self) -> None:
        pass

    def _on_monitor_toggle_clicked(self) -> None:
        pass

    def _on_verbose_toggle_clicked(self) -> None:
        pass

    def _on_calibrate_clicked(self) -> None:
        pass

    def _on_run_order_clicked(self) -> None:
        pass

    def _on_display_clicked(self) -> None:
        pass

    def _on_results_clicked(self) -> None:
        pass

    def _on_name_contestants_clicked(self) -> None:
        pass


def main() -> None:
    MainWindowLayout().mainloop()


if __name__ == "__main__":
    main()
