"""Tests for the main window (APP-1.8.1).

Pure-layout construction (widget presence, window geometry, port/baud
enumeration, stub handlers) is tested against `MainWindowLayout` directly
in test_main_window_layout.py -- it has no DB/AppCore/serial dependency at
all. This file covers `MainWindow`-specific (business-logic) behavior: the
connect switch is exercised against a fake transport; the full
Connect/Disconnect flow against real hardware is a manual smoke test (see
the checklist shipped with each stage).
"""
import shutil

from rats import main_window
from rats.main_window import DEFAULT_WINDOW_SIZE, WINDOW_TITLE, _format_event_date

from dbfixture import DEMO_DB
from fake_serial import FakeSerialTransport
from gui_helpers import skip_if_no_display


def test_format_event_date_strips_time_and_uses_british_order():
    assert _format_event_date("2026-04-18 00:00:00") == "18/04/2026"


def test_format_event_date_handles_date_only_string():
    assert _format_event_date("2026-04-18") == "18/04/2026"


def test_format_event_date_handles_missing_value():
    assert _format_event_date(None) == "--"
    assert _format_event_date("") == "--"


def test_format_event_date_falls_back_to_raw_on_unrecognized_format():
    assert _format_event_date("not a date") == "not a date"


def test_main_window_constructs_without_error(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.update_idletasks()

        assert str(window.port_combo) != ""
        assert window.connect_switch["text"] == "OFF"
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
        assert not window.calibrate_mode_radio.instate(("disabled",))
        assert not window.run_order_button.instate(("disabled",))
    finally:
        window.destroy()


def test_title_shows_none_when_db_path_unknown(demo_db, make_main_window):
    """`make_main_window` injects `conn=demo_db` with no `db_path` -- an
    in-memory connection has no filename, so `NONE` is the correct title,
    same as `MainWindowLayout`'s own default."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window.title() == f"{WINDOW_TITLE} - NONE"
    finally:
        window.destroy()


def test_title_shows_db_filename_when_known(demo_db, make_main_window, tmp_path):
    skip_if_no_display()
    window = make_main_window(demo_db, db_path=tmp_path / "myevent.db")
    try:
        assert window.title() == f"{WINDOW_TITLE} - myevent.db"
    finally:
        window.destroy()


def test_title_updates_when_opening_a_new_database(demo_db, make_main_window, monkeypatch, tmp_path):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        other_path = tmp_path / "other.db"
        shutil.copy(DEMO_DB, other_path)
        monkeypatch.setattr(
            main_window.filedialog, "askopenfilename", lambda **kwargs: str(other_path)
        )

        window._on_open_database()

        assert window.title() == f"{WINDOW_TITLE} - other.db"
    finally:
        window.destroy()


def test_starts_with_no_database_open(make_main_window):
    """User decision: a remembered/bundled default DB path can point at a
    file that's moved or been deleted, so the app always starts with
    nothing open -- File -> Open Database... is required every launch."""
    skip_if_no_display()
    window = make_main_window(None)
    try:
        assert window.conn is None
        assert window.title() == f"{WINDOW_TITLE} - NONE"
        assert window.event_name_var.get() == main_window.NO_EVENT_TEXT
        assert not window._entry_pane_should_be_enabled()
        assert str(window.competition_class_combo["state"]) == "disabled"
    finally:
        window.destroy()


def test_practice_mode_cannot_be_left_without_a_database(make_main_window, monkeypatch):
    skip_if_no_display()
    errors = []
    monkeypatch.setattr(
        main_window.messagebox, "showerror", lambda title, msg: errors.append((title, msg))
    )
    window = make_main_window(None)
    try:
        window._on_practice_mode_clicked()

        assert window.app_state.entry.practice_mode is True
        assert window.mode_var.get() == "PRACTICE"
        assert len(errors) == 1
        assert str(window.competition_class_combo["state"]) == "disabled"
    finally:
        window.destroy()


def test_opening_a_database_lets_practice_mode_be_left(make_main_window, monkeypatch, tmp_path):
    skip_if_no_display()
    window = make_main_window(None)
    try:
        other_path = tmp_path / "other.db"
        shutil.copy(DEMO_DB, other_path)
        monkeypatch.setattr(
            main_window.filedialog, "askopenfilename", lambda **kwargs: str(other_path)
        )
        window._on_open_database()
        assert window.conn is not None

        window._on_practice_mode_clicked()

        assert window.app_state.entry.practice_mode is False
        assert str(window.competition_class_combo["state"]) == "readonly"
    finally:
        window.destroy()


def test_name_contestants_requires_a_database(make_main_window, monkeypatch):
    skip_if_no_display()
    errors = []
    monkeypatch.setattr(
        main_window.messagebox, "showerror", lambda title, msg: errors.append((title, msg))
    )
    window = make_main_window(None)
    try:
        window._on_name_contestants_clicked()
        assert errors == [("Name Contestants", "Open a database first.")]
    finally:
        window.destroy()


def test_opening_database_never_persists_its_path(make_main_window, monkeypatch, tmp_path):
    """Regression guard for the user decision to stop remembering
    `last_db_path` -- `_on_open_database` shouldn't call `save_config` at
    all, since nothing about the DB path is saved anymore."""
    skip_if_no_display()
    save_calls = []
    monkeypatch.setattr(main_window.config_module, "save_config", save_calls.append)
    window = make_main_window(None)
    try:
        other_path = tmp_path / "other.db"
        shutil.copy(DEMO_DB, other_path)
        monkeypatch.setattr(
            main_window.filedialog, "askopenfilename", lambda **kwargs: str(other_path)
        )
        window._on_open_database()
        assert save_calls == []
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


def test_connect_switch_turns_on_and_locks_port_and_baud(demo_db, make_main_window):
    """Driven through `invoke()`, a real click: the switch flips its own
    variable first, then `_on_connect_clicked` runs."""
    skip_if_no_display()
    window = make_main_window(demo_db, open_serial_port_fn=lambda port, baud: FakeSerialTransport())
    try:
        window.port_var.set("FAKE0")

        window.connect_switch.invoke()
        assert window.app_state.connection.serial_port_opened is True
        assert window.connect_var.get() is True
        assert window.connect_switch["text"] == "ON"
        assert str(window.port_combo["state"]) == "disabled"
        assert window._connection_menu.entrycget("Baud", "state") == "disabled"

        window.connect_switch.invoke()
        assert window.app_state.connection.serial_port_opened is False
        assert window.connect_var.get() is False
        assert window.connect_switch["text"] == "OFF"
        assert str(window.port_combo["state"]) == "readonly"
        assert window._connection_menu.entrycget("Baud", "state") == "normal"
    finally:
        window.destroy()


def test_connect_switch_returns_to_off_when_the_port_fails_to_open(
    demo_db, make_main_window, monkeypatch
):
    skip_if_no_display()
    errors = []
    monkeypatch.setattr(
        main_window.messagebox, "showerror", lambda title, msg: errors.append((title, msg))
    )

    def failing_open(port, baud):
        raise OSError("no such port")

    window = make_main_window(demo_db, open_serial_port_fn=failing_open)
    try:
        window.port_var.set("FAKE0")
        window.connect_switch.invoke()

        assert len(errors) == 1
        assert window.connect_var.get() is False
        assert window.connect_switch["text"] == "OFF"
        assert str(window.port_combo["state"]) == "readonly"
    finally:
        window.destroy()


def test_connect_switch_returns_to_off_with_no_port_selected(make_main_window, monkeypatch):
    skip_if_no_display()
    errors = []
    monkeypatch.setattr(
        main_window.messagebox, "showerror", lambda title, msg: errors.append((title, msg))
    )
    window = make_main_window(None)
    try:
        window.port_var.set("")
        window.connect_switch.invoke()

        assert len(errors) == 1
        assert window.connect_var.get() is False
    finally:
        window.destroy()
