"""Tests for the main window (APP-1.8.1).

Port/baud enumeration and the window construction are tested here without
any real hardware. The full Connect/Disconnect flow against real hardware
is a manual smoke test (see plans/app-1-8-main-window-layout.md and the
checklist shipped with this stage) -- not exercised here.
"""
from rats import main_window
from rats.config import AppConfig
from rats.main_window import (
    BAUD_RATES,
    DEFAULT_BAUD,
    DEFAULT_WINDOW_SIZE,
    RESTORE_WINDOW_SIZE,
    list_available_ports,
)

from gui_helpers import skip_if_no_display


class _FakePortInfo:
    def __init__(self, device):
        self.device = device


def test_list_available_ports_wraps_comports(monkeypatch):
    fake_ports = [_FakePortInfo("COM3"), _FakePortInfo("/dev/ttyUSB0")]
    monkeypatch.setattr(
        main_window.serial.tools.list_ports, "comports", lambda: fake_ports
    )

    assert list_available_ports() == ["COM3", "/dev/ttyUSB0"]


def test_list_available_ports_empty(monkeypatch):
    monkeypatch.setattr(main_window.serial.tools.list_ports, "comports", lambda: [])
    assert list_available_ports() == []


def test_baud_rates_include_legacy_and_new_speed():
    assert 9600 in BAUD_RATES
    assert 115200 in BAUD_RATES
    assert DEFAULT_BAUD == 9600


def test_main_window_constructs_without_error(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.update_idletasks()

        assert str(window.port_combo) != ""
        assert window.connect_button["text"] == "Connect"
        assert str(window.monitor_text) != ""
        assert str(window.competition_tree) != ""
        assert str(window.entry_tree) != ""
    finally:
        window.destroy()


def test_main_window_child_windows_not_built_yet(demo_db, make_main_window):
    """`APP-1.9`-`.12` haven't built the calibration/run-order/display/
    results windows yet, but their launch buttons are active as of `.4`
    (they just toggle `AppState.windows` flags) -- see test_monitor_log.py.
    Every other pane's widgets are active too as of `.2`/`.3`, covered in
    test_event_entry_selection.py / test_run_control.py. Nothing in
    MainWindow itself should still be `disabled` at this point."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert str(window.calibrate_button["state"]) == "normal"
        assert str(window.monitor_toggle_button["state"]) == "normal"
    finally:
        window.destroy()


def test_default_window_size_matches_legacy_client_size(demo_db, make_main_window):
    """Legacy Form1's designed ClientSize, `Form1.cs:2092`."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.update_idletasks()
        assert (window.winfo_width(), window.winfo_height()) == DEFAULT_WINDOW_SIZE
    finally:
        window.destroy()


def test_panes_start_at_equal_thirds(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.update_idletasks()
        total_width = window._paned.winfo_width()
        third = total_width // 3

        assert window._paned.sashpos(0) == third
        assert window._paned.sashpos(1) == 2 * third
    finally:
        window.destroy()


def test_title_shows_current_window_size(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.update_idletasks()
        width, height = DEFAULT_WINDOW_SIZE
        assert f"{width}x{height}" in window.title()
    finally:
        window.destroy()


def test_restoring_saved_window_size_is_currently_disabled(demo_db, make_main_window, monkeypatch):
    """RESTORE_WINDOW_SIZE gates reading the saved size back on startup --
    the size is still saved (see the next test), just not applied yet."""
    skip_if_no_display()
    assert RESTORE_WINDOW_SIZE is False

    monkeypatch.setattr(
        main_window.config_module,
        "load_config",
        lambda: AppConfig(last_window_width=1234, last_window_height=999),
    )

    window = make_main_window(demo_db)
    try:
        window.update_idletasks()
        assert (window.winfo_width(), window.winfo_height()) == DEFAULT_WINDOW_SIZE
    finally:
        window.destroy()


def test_window_size_is_saved_on_close(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    saved = {}
    monkeypatch.setattr(
        main_window.config_module,
        "save_config",
        lambda cfg: saved.update(width=cfg.last_window_width, height=cfg.last_window_height),
    )

    window = make_main_window(demo_db)
    window.update_idletasks()
    window.destroy()

    width, height = DEFAULT_WINDOW_SIZE
    assert saved == {"width": width, "height": height}
