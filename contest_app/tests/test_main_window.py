"""Tests for the main window (APP-1.8.1).

Port/baud enumeration and the window construction are tested here without
any real hardware. The full Connect/Disconnect flow against real hardware
is a manual smoke test (see plans/app-1-8-main-window-layout.md and the
checklist shipped with this stage) -- not exercised here.
"""
from rats import main_window
from rats.main_window import BAUD_RATES, DEFAULT_BAUD, MainWindow, list_available_ports

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


def test_main_window_constructs_without_error(demo_db):
    skip_if_no_display()
    window = MainWindow(conn=demo_db, open_serial_port_fn=lambda port, baud: None)
    try:
        window.update_idletasks()

        assert str(window.port_combo) != ""
        assert window.connect_button["text"] == "Connect"
        assert str(window.monitor_text) != ""
        assert str(window.competition_tree) != ""
        assert str(window.entry_tree) != ""
    finally:
        window.destroy()


def test_main_window_placeholder_widgets_start_disabled(demo_db):
    skip_if_no_display()
    window = MainWindow(conn=demo_db, open_serial_port_fn=lambda port, baud: None)
    try:
        assert str(window.touch_button["state"]) == "disabled"
        assert str(window.calibrate_button["state"]) == "disabled"
        assert "disabled" in window.competition_tree.state()
    finally:
        window.destroy()
