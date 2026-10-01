"""Pure widget layout for the main window -- no `AppCore`/DB/serial
dependency at all.

Split out of `main_window.py` so the entire visual layout can be built and
clicked in isolation, without a database, hardware, or any of the app's
real behavior wired up -- useful for iterating on layout/spacing (e.g.
`APP-1.8.5`'s layout review) without needing to relaunch the full app each
time.

`MainWindowLayout` builds every widget from `APP-1.8.1`-`.4`, arranged per
the user's own layout drawing (`APP-1.8.5`, `tmp/rats-II-layout.drawio.png`),
and wires each button/selection to a same-named handler method -- but every handler here
is a harmless `pass` stub, so the class is fully constructible and
interactive on its own (see `main()` below). `rats.main_window.MainWindow`
subclasses this and overrides every stub with its real implementation;
Python's normal method resolution means the real app gets real behavior
and this module never needs to know that subclass exists.

Run directly (`python -m rats.main_window_layout`) to preview the layout.
"""
from __future__ import annotations

import tkinter as tk
from datetime import datetime
from pathlib import Path
from tkinter import ttk
from typing import Optional

import serial.tools.list_ports
import sv_ttk

from rats import config as config_module
from rats.config import AppConfig

# UI-2: sv_ttk's light theme, chosen over stock ttk after the user compared
# both sv_ttk variants live against this window -- "an improvement", light
# preferred "for now".
THEME = "light"

WINDOW_TITLE = "RATS Contest Timing"
# The user's layout drawing's size (`APP-1.8.5`,
# `tmp/rats-II-layout.drawio.png`) -- replaces legacy Form1's 884x522
# `ClientSize`, which the redesigned layout no longer follows.
DEFAULT_WINDOW_SIZE = (960, 577)

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

# Shown until a real Context row is loaded (no DB open, or one without a
# Context table row) -- reuses legacy's "Stand Alone Mode" name (Q-4) as
# just a placeholder label now, not a functional degraded mode.
NO_EVENT_TEXT = "STAND ALONE"

# Colours from the user's layout drawing (drawio's stock palette). The two
# lavender bands are the top info bar and the bottom status bar; everything
# between sits on the cream body. sv_ttk's button/switch sprites have
# transparent corners, so they blend onto either (checked before building).
_BAND_COLOR = "#e1d5e7"
_BODY_COLOR = "#f9f7ed"
_NAME_COLOR = "#cc0000"
_TILE_CAPTION_COLOR = "#eeeeee"
_TILE_VALUE_COLOR = "#ffffff"
_CONTEST_SELECT_COLOR = "#b0e3e6"
_ENTRY_SELECT_COLOR = "#d5e8d4"
_ALARM_COLOR = "#cc0000"

_INFO_BAR_FONT = ("TkDefaultFont", 14, "bold")
_NAME_ROW_FONT = ("TkDefaultFont", 14, "bold")
_TILE_CAPTION_FONT = ("TkDefaultFont", 12, "bold")
_TILE_VALUE_FONT = ("TkDefaultFont", 13)
_RUN_BUTTON_FONT = ("TkDefaultFont", 12, "bold")
_DISPLAY_BUTTON_FONT = ("TkDefaultFont", 10, "bold")
_STATUS_BAR_FONT = ("TkDefaultFont", 12)

# The drawing's three body columns (log / lists / buttons) are roughly
# 290 : 257 : 353 px of its 960 -- kept as grid weights in a `uniform`
# group, so the proportions hold at any window size.
_BODY_COLUMN_WEIGHTS = (29, 26, 35)
# Tighter than sv_ttk's default rows, so the drawing's seven entries fit
# the ENTRY list at the default window size without scrolling.
_LIST_ROW_HEIGHT = 20
_BODY_SIDE_PAD = 16
_BODY_COLUMN_GAP = 15

# The live-value tiles, one tuple per body column: (caption, StringVar
# attribute, relative width). Captions are the drawing's; the attributes
# are the same `APP-1.8.3` live-display variables as before ("Course Time"
# is `maze_time`, "Last Run"/"Last Score" are `run_time`/`score_time`) --
# except "Run", which now shows used/allowed in one tile ("3/5").
_TILE_SPECS = (
    (("Course Time", "maze_time_var", 7), ("Remaining", "time_left_var", 6), ("Run", "run_count_var", 3)),
    (("Touch", "touches_var", 4), ("Best Score", "best_score_var", 7), ("Rank", "rank_var", 3)),
    (("Last Run", "run_time_var", 1), ("Last Score", "score_time_var", 1), ("Split Time", "split_time_var", 1)),
)

# Not on the main window in the drawing -- shown by Tools -> Scoring Model...
_SCORING_SPECS = (
    ("Touch time", "touch_time_cfg_var"),
    ("Touch divider", "touch_divider_cfg_var"),
    ("Maze divider", "maze_divider_cfg_var"),
    ("Cumulative", "touch_cumulative_cfg_var"),
    ("Enabled", "touch_enabled_cfg_var"),
)

GATE_MODE_TIMER = "timer"
GATE_MODE_CALIBRATE = "calibrate"


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
        self._configure_styles()

        self._cfg = cfg if cfg is not None else config_module.load_config()
        self._db_path: Optional[Path] = None
        self._update_title()

        self._scoring_dialog: Optional[tk.Toplevel] = None
        self.baud_var = tk.IntVar(value=self._cfg.last_baud or DEFAULT_BAUD)
        # Legacy's startup default (`monitor_input = 1`, `Form1`'s constructor).
        self.monitor_var = tk.BooleanVar(value=True)
        for _, attr in _SCORING_SPECS:
            setattr(self, attr, tk.StringVar(value="--"))

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

    def _update_title(self) -> None:
        db_name = self._db_path.name if self._db_path is not None else "NONE"
        self.title(f"{WINDOW_TITLE} - {db_name}")

    # -- Styles -----------------------------------------------------------

    def _configure_styles(self) -> None:
        """Everything the drawing needs beyond stock sv_ttk: the cream body
        (root style background, inherited by every ttk widget), the two
        lavender bands, bold run-control buttons on sv_ttk's own
        `Accent.TButton` (user decision: stock accent blue, no custom
        button images), and a selection colour per list."""
        style = ttk.Style(self)
        style.configure(".", background=_BODY_COLOR)
        self.configure(background=_BODY_COLOR)

        style.configure("Band.TFrame", background=_BAND_COLOR)
        style.configure("Band.TCheckbutton", background=_BAND_COLOR, font=_STATUS_BAR_FONT)
        style.configure("Band.TRadiobutton", background=_BAND_COLOR, font=_STATUS_BAR_FONT)

        style.configure("Run.Accent.TButton", font=_RUN_BUTTON_FONT)
        style.configure("Display.Accent.TButton", font=_DISPLAY_BUTTON_FONT)

        for name, color in (
            ("Contest.Treeview", _CONTEST_SELECT_COLOR),
            ("Entry.Treeview", _ENTRY_SELECT_COLOR),
        ):
            style.configure(name, rowheight=_LIST_ROW_HEIGHT)
            style.map(name, background=[("selected", color)], foreground=[("selected", "#191919")])

    # -- Layout -----------------------------------------------------------

    def _build_menu(self) -> None:
        """File / Connection / Tools. The last two hold the controls the
        drawing leaves off the main window (user decision: menus + status
        bar rather than dropping them): baud rate, log clear, the on-screen
        monitor toggle, Name Contestants and the scoring-model readout."""
        menubar = tk.Menu(self)

        file_menu = tk.Menu(menubar, tearoff=False)
        file_menu.add_command(label="Open Database…", command=self._on_open_database)
        file_menu.add_separator()
        file_menu.add_command(label="Exit", command=self.destroy)
        menubar.add_cascade(label="File", menu=file_menu)

        self._connection_menu = tk.Menu(menubar, tearoff=False)
        baud_menu = tk.Menu(self._connection_menu, tearoff=False)
        for rate in BAUD_RATES:
            baud_menu.add_radiobutton(label=str(rate), variable=self.baud_var, value=rate)
        self._connection_menu.add_cascade(label="Baud", menu=baud_menu)
        menubar.add_cascade(label="Connection", menu=self._connection_menu)

        tools_menu = tk.Menu(menubar, tearoff=False)
        tools_menu.add_command(label="Clear Log", command=self._on_monitor_clear_clicked)
        tools_menu.add_checkbutton(
            label="Show serial traffic in log",
            variable=self.monitor_var,
            command=self._on_monitor_toggle_clicked,
        )
        tools_menu.add_separator()
        tools_menu.add_command(label="Name Contestants…", command=self._on_name_contestants_clicked)
        tools_menu.add_command(label="Scoring Model…", command=self._show_scoring_dialog)
        menubar.add_cascade(label="Tools", menu=tools_menu)

        self.config(menu=menubar)

    def _build_layout(self) -> None:
        self.columnconfigure(0, weight=1)
        self.rowconfigure(2, weight=1)

        self._build_info_bar()
        self._build_name_row()
        self._build_body()
        self._build_status_bar()

    def _build_info_bar(self) -> None:
        """Top band: Date (left, value only), Contest Name (centre, name
        only), Mode (right, `PRACTICE` or the selected competition's name)
        -- a scoreboard-style banner, not a form, so no field captions."""
        bar = ttk.Frame(self, style="Band.TFrame", padding=(_BODY_SIDE_PAD, 4))
        bar.grid(row=0, column=0, sticky="ew")
        for column in range(3):
            bar.columnconfigure(column, weight=1)

        # With no database open (this class's permanent state), the date
        # shown is today's date -- user decision, `main_window.py`'s
        # `_load_event_context` does the same for the real subclass.
        self.event_date_var = tk.StringVar(value=datetime.now().strftime("%d/%m/%Y"))
        self.event_name_var = tk.StringVar(value=NO_EVENT_TEXT)
        self.mode_var = tk.StringVar(value="PRACTICE")
        for column, (var, sticky) in enumerate(
            ((self.event_date_var, "w"), (self.event_name_var, ""), (self.mode_var, "e"))
        ):
            ttk.Label(bar, textvariable=var, background=_BAND_COLOR, font=_INFO_BAR_FONT).grid(
                row=0, column=column, sticky=sticky
            )

    def _build_name_row(self) -> None:
        """Selected Robot (left) and Contestant (right), below the info bar
        -- values only, per the drawing (no "Robot:"/"Contestant:" captions)."""
        row = ttk.Frame(self, padding=(_BODY_SIDE_PAD, 6, _BODY_SIDE_PAD, 2))
        row.grid(row=1, column=0, sticky="ew")
        row.columnconfigure(0, weight=1)
        row.columnconfigure(1, weight=1)

        self.selected_robot_var = tk.StringVar(value="--")
        ttk.Label(
            row, textvariable=self.selected_robot_var, font=_NAME_ROW_FONT, foreground=_NAME_COLOR
        ).grid(row=0, column=0, sticky="w")

        self.current_contestant_var = tk.StringVar(value="--")
        ttk.Label(
            row, textvariable=self.current_contestant_var, font=_NAME_ROW_FONT, foreground=_NAME_COLOR
        ).grid(row=0, column=1, sticky="e")

    def _build_body(self) -> None:
        """The three fixed columns between the bands (log / lists /
        buttons), each with the same three rows: live-value tiles, a
        controls row, then the column's main area (the only row that
        stretches). A plain grid, not a `PanedWindow` -- the drawing has no
        sashes."""
        body = ttk.Frame(self, padding=(_BODY_SIDE_PAD, 4, _BODY_SIDE_PAD, 8))
        body.grid(row=2, column=0, sticky="nsew")
        self._body = body
        body.rowconfigure(2, weight=1)

        last_column = len(_BODY_COLUMN_WEIGHTS) - 1
        for column, weight in enumerate(_BODY_COLUMN_WEIGHTS):
            body.columnconfigure(column, weight=weight, uniform="body")
            padx = (0, 0 if column == last_column else _BODY_COLUMN_GAP)

            tiles = ttk.Frame(body)
            tiles.grid(row=0, column=column, sticky="ew", padx=padx)
            self._build_tiles(tiles, _TILE_SPECS[column])

            controls = ttk.Frame(body)
            controls.grid(row=1, column=column, sticky="ew", padx=padx, pady=(10, 0))
            main = ttk.Frame(body)
            main.grid(row=2, column=column, sticky="nsew", padx=padx, pady=(10, 0))
            build_controls, build_main = (
                (self._build_connection_controls, self._build_log_area),
                (self._build_class_controls, self._build_selection_area),
                (self._build_mode_controls, self._build_run_area),
            )[column]
            build_controls(controls)
            build_main(main)

    def _build_tiles(self, frame: ttk.Frame, specs: tuple) -> None:
        """One group of live-value tiles: a grey caption over a white value."""
        last_column = len(specs) - 1
        for column, (caption, attr, weight) in enumerate(specs):
            frame.columnconfigure(column, weight=weight)
            var = tk.StringVar(value="--")
            setattr(self, attr, var)
            padx = (0, 0 if column == last_column else 8)
            ttk.Label(
                frame,
                text=caption,
                font=_TILE_CAPTION_FONT,
                background=_TILE_CAPTION_COLOR,
                anchor="center",
                padding=(4, 2),
            ).grid(row=0, column=column, sticky="ew", padx=padx)
            ttk.Label(
                frame,
                textvariable=var,
                font=_TILE_VALUE_FONT,
                background=_TILE_VALUE_COLOR,
                anchor="center",
                padding=(4, 2),
            ).grid(row=1, column=column, sticky="ew", padx=padx)

    def _build_connection_controls(self, frame: ttk.Frame) -> None:
        """COM port + the connect switch (`APP-1.8.1`). The switch is
        sv_ttk's `Switch.TCheckbutton`; its ON/OFF caption follows
        `connect_var` by itself, so `MainWindow` only ever sets the
        variable. Baud lives in the Connection menu."""
        frame.columnconfigure(0, weight=1)

        self.port_var = tk.StringVar()
        self.port_combo = ttk.Combobox(frame, textvariable=self.port_var, state="readonly", width=8)
        self.port_combo.grid(row=0, column=0, sticky="ew", padx=(0, 12))

        self.connect_var = tk.BooleanVar(value=False)
        self.connect_switch = ttk.Checkbutton(
            frame,
            text="OFF",
            width=4,
            style="Switch.TCheckbutton",
            variable=self.connect_var,
            command=self._on_connect_clicked,
        )
        self.connect_switch.grid(row=0, column=1)
        self.connect_var.trace_add(
            "write",
            lambda *_: self.connect_switch.configure(text="ON" if self.connect_var.get() else "OFF"),
        )

    def _build_class_controls(self, frame: ttk.Frame) -> None:
        frame.columnconfigure(0, weight=1)
        self.competition_class_var = tk.StringVar()
        self.competition_class_combo = ttk.Combobox(
            frame,
            textvariable=self.competition_class_var,
            state="readonly",
            values=COMPETITION_CLASSES,
            width=8,
        )
        self.competition_class_combo.grid(row=0, column=0, sticky="ew")
        self.competition_class_combo.bind(
            "<<ComboboxSelected>>", self._on_competition_class_selected
        )

    def _build_mode_controls(self, frame: ttk.Frame) -> None:
        """The Practice <=> Contest toggle, captioned just `MODE` per the
        drawing -- the current mode itself reads out in the info bar's
        right-hand cell, directly above."""
        frame.columnconfigure(0, weight=1)
        self.practice_mode_button = ttk.Button(
            frame, text="MODE", style="Run.Accent.TButton", command=self._on_practice_mode_clicked
        )
        self.practice_mode_button.grid(row=0, column=0, sticky="ew")

    def _build_log_area(self, frame: ttk.Frame) -> None:
        """Left column: the raw Rx monitor (`APP-1.8.4`), titled "System
        Log". Its Monitor/Clear controls are in the Tools menu and Verbose
        is in the status bar."""
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(0, weight=1)

        box = tk.Frame(frame, background=_TILE_VALUE_COLOR, highlightthickness=1)
        box.configure(highlightbackground="#808080", highlightcolor="#808080")
        box.grid(row=0, column=0, sticky="nsew")
        box.columnconfigure(0, weight=1)
        box.rowconfigure(2, weight=1)

        ttk.Label(
            box,
            text="System Log",
            font=("TkDefaultFont", 9, "bold"),
            background=_TILE_VALUE_COLOR,
            anchor="center",
            padding=(0, 2),
        ).grid(row=0, column=0, sticky="ew")
        ttk.Separator(box, orient=tk.HORIZONTAL).grid(row=1, column=0, sticky="ew")

        # width/height are only the *requested* size -- kept tiny so this
        # widget never dictates the column's width; the grid stretches it.
        self.monitor_text = tk.Text(
            box,
            width=10,
            height=4,
            state="disabled",
            wrap="none",
            relief="flat",
            borderwidth=0,
            highlightthickness=0,
            font=("TkFixedFont", 9),
        )
        self.monitor_text.grid(row=2, column=0, sticky="nsew")

    def _build_selection_area(self, frame: ttk.Frame) -> None:
        """Middle column: competition and entry lists (`APP-1.8.2`).

        Practice-mode gating (`core.select_competition`'s docstring: "the
        competition grid is disabled during practice mode") is wired in
        `APP-1.8.3` (`_set_entry_pane_enabled`, called from the MODE button
        and once at startup) rather than here -- `practice_mode` defaults
        `true`, so these actually start disabled until the user leaves
        practice mode, matching legacy."""
        frame.columnconfigure(0, weight=1)
        frame.rowconfigure(0, weight=1)
        frame.rowconfigure(1, weight=1)

        self.competition_tree = self._build_list(
            frame, row=0, column_id="name", heading="CONTEST", style="Contest.Treeview"
        )
        self.competition_tree.bind("<<TreeviewSelect>>", self._on_competition_selected)

        self.entry_tree = self._build_list(
            frame, row=1, column_id="mouse", heading="ENTRY", style="Entry.Treeview"
        )
        self.entry_tree.bind("<<TreeviewSelect>>", self._on_entry_selected)

    def _build_list(
        self, frame: ttk.Frame, *, row: int, column_id: str, heading: str, style: str
    ) -> ttk.Treeview:
        tree = ttk.Treeview(
            frame, columns=(column_id,), show="headings", height=4, selectmode="browse", style=style
        )
        tree.heading(column_id, text=heading, anchor="w")
        tree.column(column_id, width=80, anchor="w")
        tree.grid(row=row, column=0, sticky="nsew", pady=(0 if row == 0 else 8, 0))
        return tree

    def _build_run_area(self, frame: ttk.Frame) -> None:
        """Right column: run-control buttons (`APP-1.8.3`) above the
        child-window launch buttons (`APP-1.8.4`)."""
        frame.columnconfigure(0, weight=1, uniform="run")
        frame.columnconfigure(1, weight=1, uniform="run")

        # Captions are the drawing's -- attribute names stay tied to the
        # legacy C# handler names for cross-reference with
        # behavior_inventory.md. (row, column, columnspan) on a 2-wide grid.
        button_specs = [
            ("Add Touch", "touch_button", self._on_touch_clicked, 0, 0, 1),
            ("Extra Run", "extra_run_button", self._on_extra_run_clicked, 0, 1, 1),
            ("Retire (DNF)", "dnf_button", self._on_dnf_clicked, 1, 0, 1),
            ("Clear Display", "clear_button", self._on_clear_clicked, 1, 1, 1),
            ("New Robot", "new_mouse_button", self._on_new_mouse_clicked, 2, 0, 2),
        ]
        for label, attr, handler, row, column, span in button_specs:
            frame.rowconfigure(row, weight=1, uniform="run_rows")
            btn = ttk.Button(frame, text=label, style="Run.Accent.TButton", command=handler)
            padx = (0, 0) if span == 2 else ((0, 15) if column == 0 else (15, 0))
            btn.grid(
                row=row, column=column, columnspan=span, sticky="nsew", padx=padx, pady=(0, 12)
            )
            setattr(self, attr, btn)

        launch_frame = ttk.LabelFrame(frame, text="DISPLAY", padding=(8, 2, 8, 8))
        launch_frame.grid(row=3, column=0, columnspan=2, sticky="ew")
        # The 3 single-channel display variants are captioned by resolution
        # in the legacy app (legacy/legacy-rats-screen.png), not "v2"/"wide"
        # as the C# class names (single_ch_display_v2/_16_9) suggest --
        # 612x595 is the original single_channel_display, 1280x720 the
        # 16:9 variant (UI-1's parametrized class covers all 3, APP-1.11).
        # All 3 display buttons share one command/flag per UI-1 -- they're
        # interchangeable skins of one window slot, not independent windows.
        # No target windows exist until APP-1.9-.12 -- these just toggle the
        # AppState.windows open flags for now. The sixth slot is an inert
        # placeholder (user decision), reserved for a future window.
        launch_specs = [
            ("800 x 600", "display_1ch_v2_button", self._on_display_clicked),
            ("612 x 595", "display_1ch_button", self._on_display_clicked),
            ("Results", "results_1ch_button", self._on_results_clicked),
            ("1280 x 720", "display_1ch_wide_button", self._on_display_clicked),
            ("Run Order", "run_order_button", self._on_run_order_clicked),
            ("—", "spare_display_button", None),
        ]
        for i, (label, attr, handler) in enumerate(launch_specs):
            row, col = divmod(i, 3)
            launch_frame.columnconfigure(col, weight=1, uniform="launch")
            btn = ttk.Button(launch_frame, text=label, style="Display.Accent.TButton")
            if handler is None:
                btn.state(("disabled",))
            else:
                btn.configure(command=handler)
            btn.grid(row=row, column=col, sticky="ew", padx=3, pady=3)
            setattr(self, attr, btn)

    def _build_status_bar(self) -> None:
        """Bottom band, in the drawing's order: Timer, Verbose, Watchdog,
        Calibrate. Timer/Calibrate are one mutually exclusive pair -- the
        gate controller's mode (user decision) -- so they're radio buttons
        on `gate_mode_var`; Verbose and Watchdog are independent toggles.
        The timer-state and watchdog-alarm readouts sit at the right."""
        bar = ttk.Frame(self, style="Band.TFrame", padding=(_BODY_SIDE_PAD, 2))
        bar.grid(row=3, column=0, sticky="ew")

        self.gate_mode_var = tk.StringVar(value=GATE_MODE_TIMER)
        self.verbose_var = tk.BooleanVar(value=False)
        # `AppState`'s default (`WatchdogState.watchdog_active = True`).
        self.watchdog_var = tk.BooleanVar(value=True)

        self.timer_mode_radio = ttk.Radiobutton(
            bar,
            text="Timer",
            style="Band.TRadiobutton",
            variable=self.gate_mode_var,
            value=GATE_MODE_TIMER,
            command=self._on_gate_mode_selected,
        )
        self.verbose_check = ttk.Checkbutton(
            bar,
            text="Verbose",
            style="Band.TCheckbutton",
            variable=self.verbose_var,
            command=self._on_verbose_toggle_clicked,
        )
        self.watchdog_check = ttk.Checkbutton(
            bar,
            text="Watchdog",
            style="Band.TCheckbutton",
            variable=self.watchdog_var,
            command=self._on_watchdog_clicked,
        )
        self.calibrate_mode_radio = ttk.Radiobutton(
            bar,
            text="Calibrate",
            style="Band.TRadiobutton",
            variable=self.gate_mode_var,
            value=GATE_MODE_CALIBRATE,
            command=self._on_gate_mode_selected,
        )
        for widget in (
            self.timer_mode_radio,
            self.verbose_check,
            self.watchdog_check,
            self.calibrate_mode_radio,
        ):
            widget.pack(side="left", padx=(0, 16))

        self.timer_state_var = tk.StringVar(value="--")
        ttk.Label(bar, textvariable=self.timer_state_var, background=_BAND_COLOR).pack(side="right")
        self.watchdog_state_var = tk.StringVar(value="")
        ttk.Label(
            bar,
            textvariable=self.watchdog_state_var,
            background=_BAND_COLOR,
            foreground=_ALARM_COLOR,
            font=("TkDefaultFont", 10, "bold"),
        ).pack(side="right", padx=(0, 12))

    def _show_scoring_dialog(self) -> None:
        """Tools -> Scoring Model...: the current competition's scoring
        parameters, read-only. Shares the same `*_cfg_var`s the live
        display refresh already fills, so it stays current while open."""
        if self._scoring_dialog is not None and self._scoring_dialog.winfo_exists():
            self._scoring_dialog.lift()
            return
        dialog = tk.Toplevel(self)
        dialog.title("Scoring Model")
        dialog.transient(self)
        dialog.resizable(False, False)
        self._scoring_dialog = dialog

        frame = ttk.Frame(dialog, padding=12)
        frame.pack(fill="both", expand=True)
        for row, (label, attr) in enumerate(_SCORING_SPECS):
            ttk.Label(frame, text=f"{label}:").grid(row=row, column=0, sticky="w", padx=(0, 16), pady=2)
            ttk.Label(frame, textvariable=getattr(self, attr)).grid(row=row, column=1, sticky="e", pady=2)
        ttk.Button(frame, text="Close", command=dialog.destroy).grid(
            row=len(_SCORING_SPECS), column=0, columnspan=2, pady=(12, 0)
        )

    def _set_entry_pane_enabled(self, enabled: bool) -> None:
        """Practice-mode gating (`core.select_competition`'s docstring:
        "the competition grid is disabled during practice mode") -- called
        from the Practice Mode toggle and once at startup, since
        `practice_mode` defaults `true`."""
        self.competition_class_combo.configure(state="readonly" if enabled else "disabled")
        tree_state = ("!disabled",) if enabled else ("disabled",)
        self.competition_tree.state(tree_state)
        self.entry_tree.state(tree_state)

    def _set_connection_controls_enabled(self, enabled: bool) -> None:
        """Port and baud can't change while connected."""
        self.port_combo.configure(state="readonly" if enabled else "disabled")
        self._connection_menu.entryconfigure("Baud", state="normal" if enabled else "disabled")

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

    def _on_gate_mode_selected(self) -> None:
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
