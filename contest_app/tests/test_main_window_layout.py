"""Tests for `MainWindowLayout` (main_window_layout.py) -- the pure-widget
base class `MainWindow` (`main_window.py`) subclasses.

No `demo_db`/`conn`/`make_main_window` fixture anywhere in this file: that's
the whole point of the split -- this class has zero DB/AppCore/serial
dependency, so it constructs and can be clicked with nothing but Tkinter
itself. Config is injected via `cfg=AppConfig(...)` rather than touching
the real per-user config file, for determinism.
"""
from datetime import datetime

from rats.config import AppConfig
from rats.main_window_layout import (
    BAUD_RATES,
    DEFAULT_BAUD,
    DEFAULT_WINDOW_SIZE,
    GATE_MODE_CALIBRATE,
    GATE_MODE_TIMER,
    NO_EVENT_TEXT,
    RESTORE_WINDOW_SIZE,
    MainWindowLayout,
    list_available_ports,
)

from gui_helpers import skip_if_no_display

# Every stub handler a real click/selection can reach -- see
# main_window_layout.py's "Stub handlers" section. Exercised generically
# below so adding a new handler without a matching stub fails loudly.
_STUB_HANDLERS = [
    "_on_open_database",
    "_on_connect_clicked",
    "_on_competition_class_selected",
    "_on_competition_selected",
    "_on_entry_selected",
    "_on_touch_clicked",
    "_on_dnf_clicked",
    "_on_clear_clicked",
    "_on_new_mouse_clicked",
    "_on_practice_mode_clicked",
    "_on_extra_run_clicked",
    "_on_watchdog_clicked",
    "_on_monitor_toggle_clicked",
    "_on_verbose_toggle_clicked",
    "_on_gate_mode_selected",
    "_on_run_order_clicked",
    "_on_display_clicked",
    "_on_results_clicked",
    "_on_name_contestants_clicked",
]


class _FakePortInfo:
    def __init__(self, device):
        self.device = device


def test_list_available_ports_wraps_comports(monkeypatch):
    import rats.main_window_layout as layout_module

    fake_ports = [_FakePortInfo("COM3"), _FakePortInfo("/dev/ttyUSB0")]
    monkeypatch.setattr(layout_module.serial.tools.list_ports, "comports", lambda: fake_ports)

    assert list_available_ports() == ["COM3", "/dev/ttyUSB0"]


def test_list_available_ports_empty(monkeypatch):
    import rats.main_window_layout as layout_module

    monkeypatch.setattr(layout_module.serial.tools.list_ports, "comports", lambda: [])
    assert list_available_ports() == []


def test_baud_rates_include_legacy_and_new_speed():
    assert 9600 in BAUD_RATES
    assert 115200 in BAUD_RATES
    assert DEFAULT_BAUD == 9600


def test_constructs_without_error():
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window.update_idletasks()
        assert str(window.port_combo) != ""
        assert window.connect_switch["text"] == "OFF"
        assert str(window.monitor_text) != ""
        assert str(window.competition_tree) != ""
        assert str(window.entry_tree) != ""
        assert window.title() == "RATS Contest Timing - NONE"
    finally:
        window.destroy()


def test_every_stub_handler_is_callable_and_harmless():
    """A no-op stub for every widget hookup -- if a new handler gets wired
    into a `_build_*` method without a matching stub here, constructing
    `MainWindowLayout` alone already fails before this test even runs."""
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        for name in _STUB_HANDLERS:
            handler = getattr(window, name)
            handler()  # every stub takes no required args (event=None where relevant)
    finally:
        window.destroy()


def test_default_window_size_matches_layout_drawing():
    """The user's `APP-1.8.5` layout drawing's size, not legacy Form1's."""
    assert DEFAULT_WINDOW_SIZE == (960, 577)
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window.update_idletasks()
        assert (window.winfo_width(), window.winfo_height()) == DEFAULT_WINDOW_SIZE
    finally:
        window.destroy()


def test_body_columns_keep_the_drawings_proportions():
    """The log / lists / buttons columns are a plain grid (no sashes) whose
    widths stay in the drawing's 29:26:35 ratio whatever the content."""
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window.update_idletasks()
        widths = [window._body.grid_bbox(column, 2)[2] for column in range(3)]
        assert widths[2] > widths[0] > widths[1] > 0

        # Each column's main widget really is in that column, left to right.
        xs = [
            widget.winfo_rootx()
            for widget in (window.monitor_text, window.entry_tree, window.touch_button)
        ]
        assert xs == sorted(xs)
    finally:
        window.destroy()


def test_connect_switch_caption_follows_its_variable():
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window.connect_var.set(True)
        assert window.connect_switch["text"] == "ON"
        window.connect_var.set(False)
        assert window.connect_switch["text"] == "OFF"
    finally:
        window.destroy()


def test_status_bar_toggles_start_at_their_defaults():
    """Timer/Calibrate share one variable (a mutually exclusive pair);
    Watchdog starts on and Verbose off, matching `AppState`/legacy."""
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        assert window.gate_mode_var.get() == GATE_MODE_TIMER
        window.calibrate_mode_radio.invoke()
        assert window.gate_mode_var.get() == GATE_MODE_CALIBRATE
        window.timer_mode_radio.invoke()
        assert window.gate_mode_var.get() == GATE_MODE_TIMER

        assert window.watchdog_var.get() is True
        assert window.verbose_var.get() is False
        assert window.monitor_var.get() is True
    finally:
        window.destroy()


def test_spare_display_button_is_inert():
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        assert window.spare_display_button.instate(("disabled",))
    finally:
        window.destroy()


def test_connection_controls_lock_and_unlock_together():
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window._set_connection_controls_enabled(False)
        assert str(window.port_combo["state"]) == "disabled"
        assert window._connection_menu.entrycget("Baud", "state") == "disabled"
        window._set_connection_controls_enabled(True)
        assert str(window.port_combo["state"]) == "readonly"
        assert window._connection_menu.entrycget("Baud", "state") == "normal"
    finally:
        window.destroy()


def test_scoring_model_dialog_opens_once():
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        window._show_scoring_dialog()
        first = window._scoring_dialog
        window._show_scoring_dialog()
        assert window._scoring_dialog is first
        assert first.winfo_exists()
    finally:
        window.destroy()


def test_title_shows_none_when_no_database_open():
    """No DB is open at the pure-layout level (`self._db_path` defaults to
    `None`) -- `MainWindow` sets a real path and updates the title once it
    resolves/opens one."""
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        assert window.title() == "RATS Contest Timing - NONE"
    finally:
        window.destroy()


def test_info_bar_shows_placeholders_before_any_event_is_loaded():
    """Top info bar (Date/Contest Name/Mode) starts at its placeholder
    values -- `MainWindow` overwrites `event_name_var` once it loads a real
    `Context` row (and `event_date_var` with the real effective date), and
    `mode_var` once it knows the real `practice_mode` state. With no
    database open, the date shown is today's date (user decision), not a
    dash."""
    skip_if_no_display()
    window = MainWindowLayout(cfg=AppConfig())
    try:
        assert window.event_date_var.get() == datetime.now().strftime("%d/%m/%Y")
        assert window.event_name_var.get() == NO_EVENT_TEXT
        assert window.mode_var.get() == "PRACTICE"
    finally:
        window.destroy()


def test_restoring_saved_window_size_is_currently_disabled():
    """RESTORE_WINDOW_SIZE gates reading the saved size back on startup --
    MainWindow still saves it on close, just doesn't apply it yet."""
    skip_if_no_display()
    assert RESTORE_WINDOW_SIZE is False

    window = MainWindowLayout(cfg=AppConfig(last_window_width=1234, last_window_height=999))
    try:
        window.update_idletasks()
        assert (window.winfo_width(), window.winfo_height()) == DEFAULT_WINDOW_SIZE
    finally:
        window.destroy()


def test_port_and_baud_preselected_from_injected_config(monkeypatch):
    import rats.main_window_layout as layout_module

    fake_ports = [_FakePortInfo("COM3"), _FakePortInfo("COM7")]
    monkeypatch.setattr(layout_module.serial.tools.list_ports, "comports", lambda: fake_ports)
    skip_if_no_display()

    window = MainWindowLayout(cfg=AppConfig(last_port="COM7", last_baud=115200))
    try:
        assert window.port_var.get() == "COM7"
        assert window.baud_var.get() == 115200
    finally:
        window.destroy()
