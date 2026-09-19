"""Tests for the main window (APP-1.8.1).

Pure-layout construction (widget presence, window geometry, port/baud
enumeration, stub handlers) is tested against `MainWindowLayout` directly
in test_main_window_layout.py -- it has no DB/AppCore/serial dependency at
all. This file covers `MainWindow`-specific (business-logic) behavior: the
full Connect/Disconnect flow against real hardware is a manual smoke test
(see the checklist shipped with each stage) -- not exercised here.
"""
from rats import main_window
from rats.main_window import DEFAULT_WINDOW_SIZE

from gui_helpers import skip_if_no_display


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
