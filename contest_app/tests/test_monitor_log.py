"""Tests for APP-1.8.4: monitor + log files + child-window launch buttons.

`_log_queue_item`/button handlers are called directly, same rationale as
the other main_window test modules -- exercises the same code a real
`_drain_tick`/click would run without needing a live queue or Tk event
loop. Log files always live under `make_main_window`'s `tmp_path` fixture
-- never the real per-user `config.log_dir()` (see conftest.py).
"""
from rats import main_window
from rats.serial_protocol import Line, Message
from rats.serial_transport import Disconnected

from gui_helpers import skip_if_no_display


def _read(path):
    return path.read_text(encoding="utf-8")


def test_log_files_created_with_expected_naming_pattern(demo_db, make_main_window, tmp_path):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window._message_log_path.parent == tmp_path / "logs"
        assert window._message_log_path.name.startswith("RATS_message_log_")
        assert window._verbatim_log_path.name.startswith("RATS_verbatim_log_")
        assert window._message_log_path.exists()
        assert window._verbatim_log_path.exists()
    finally:
        window.destroy()


def test_parsed_line_logged_and_shown_when_monitor_on(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        line = Line(raw="<2,1500>", message=Message(2, 1500))
        window._log_queue_item(line)

        assert "<2,1500>" in _read(window._verbatim_log_path)
        assert "Type=2 Value=1500" in _read(window._message_log_path)
        assert "Type=2 Value=1500" in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_watchdog_message_logged_but_not_shown_on_screen(demo_db, make_main_window):
    """Code 0 (WatchDog) is skipped on-screen to avoid spam, but still
    reaches both log files -- "every line, per Q-6"."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        line = Line(raw="<0,0>", message=Message(0, 0))
        window._log_queue_item(line)

        assert "Type=0 Value=0" in _read(window._message_log_path)
        assert "Type=0" not in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_unparsed_line_logged_and_shown(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        line = Line(raw="garbage", message=None)
        window._log_queue_item(line)

        assert "garbage" in _read(window._verbatim_log_path)
        assert "Unparsed: garbage" in _read(window._message_log_path)
        assert "Unparsed: garbage" in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_disconnected_item_logged_and_shown(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._log_queue_item(Disconnected("port unplugged"))

        assert "Disconnected: port unplugged" in _read(window._message_log_path)
        assert "Disconnected: port unplugged" in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_logging_is_complete_regardless_of_monitor_toggle(demo_db, make_main_window):
    """The whole point of decoupling logging from the Monitor toggle:
    turning Monitor off must not lose anything from the log files."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_monitor_toggle_clicked()  # off
        assert window._monitor_on is False

        window._log_queue_item(Line(raw="<3,4000>", message=Message(3, 4000)))

        assert "Type=3 Value=4000" in _read(window._message_log_path)
        assert "Type=3" not in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_verbose_adds_raw_suffix_on_screen_only(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_verbose_toggle_clicked()  # on
        window._log_queue_item(Line(raw="<3,4000>", message=Message(3, 4000)))

        assert "(raw:" in window.monitor_text.get("1.0", "end")
        assert "(raw:" not in _read(window._message_log_path)
    finally:
        window.destroy()


def test_monitor_toggle_button_caption(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window._monitor_on is True
        assert window.monitor_toggle_button["text"] == "NoMonitor"

        window._on_monitor_toggle_clicked()
        assert window._monitor_on is False
        assert window.monitor_toggle_button["text"] == "Monitor"
    finally:
        window.destroy()


def test_verbose_toggle_button_caption(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window._verbose_on is False
        assert window.verbose_toggle_button["text"] == "Verbose"

        window._on_verbose_toggle_clicked()
        assert window._verbose_on is True
        assert window.verbose_toggle_button["text"] == "Concise"
    finally:
        window.destroy()


def test_clear_button_empties_monitor_text_only(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._log_queue_item(Line(raw="<2,1500>", message=Message(2, 1500)))
        assert window.monitor_text.get("1.0", "end").strip() != ""

        window._on_monitor_clear_clicked()

        assert window.monitor_text.get("1.0", "end").strip() == ""
        # the log file is untouched by Clear
        assert "Type=2 Value=1500" in _read(window._message_log_path)
    finally:
        window.destroy()


def test_tx_send_is_logged_via_on_tx_hook(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()  # leave practice mode
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        competition_id = window.competition_tree.get_children()[0]
        window.competition_tree.selection_set(competition_id)
        window._on_competition_selected()
        entry_id = window.entry_tree.get_children()[0]

        # Fake a connected transport so start_new_entry() actually sends.
        sent = []
        window.core.transport = type("_T", (), {"write": lambda self, data: sent.append(data)})()

        window.entry_tree.selection_set(entry_id)
        window._on_entry_selected()

        assert sent == [b"<98,0>\n"]
        assert "Tx" in _read(window._message_log_path)
        assert "<98,0>" in _read(window._message_log_path)
        assert "<98,0>" not in window.monitor_text.get("1.0", "end")
    finally:
        window.destroy()


def test_destroy_closes_log_files(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    message_log = window._message_log
    verbatim_log = window._verbatim_log
    window.destroy()

    assert message_log.closed
    assert verbatim_log.closed


def test_display_buttons_share_one_window_flag(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window.app_state.windows.single_channel_window_open is False

        window._on_display_clicked()  # any of the 3 -- same handler
        assert window.app_state.windows.single_channel_window_open is True

        window._on_display_clicked()
        assert window.app_state.windows.single_channel_window_open is False
    finally:
        window.destroy()


def test_calibrate_run_order_results_toggle_their_own_flags(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_calibrate_clicked()
        assert window.app_state.windows.calibration_window_open is True

        window._on_run_order_clicked()
        assert window.app_state.windows.run_order_window_open is True

        window._on_results_clicked()
        assert window.app_state.windows.single_channel_results_window_open is True

        # confirm they're independent of the shared display flag
        assert window.app_state.windows.single_channel_window_open is False
    finally:
        window.destroy()


def test_name_contestants_backfills_real_db_rows(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        mouse_name, contestant_name, contestant_class = demo_db.execute(
            "SELECT Mouse.Mouse_Name, Contestant.Contestant_Name, Contestant.Class "
            "FROM Mouse JOIN Contestant ON Contestant.Contestant_ID = Mouse.Contestant_ID "
            "LIMIT 1"
        ).fetchone()
        demo_db.execute(
            "INSERT INTO Best_Score_Time "
            "(Entry_ID, Mouse_Name, Contestant_Name, Contestant_Class, "
            "Score_Time_mS, Run_Time_mS, Competition_ID) VALUES (?, ?, '_', '_', ?, ?, ?)",
            (1, mouse_name, 1000, 1000, 1),
        )
        demo_db.commit()

        window._on_name_contestants_clicked()

        row = demo_db.execute(
            "SELECT Contestant_Name, Contestant_Class FROM Best_Score_Time WHERE Mouse_Name = ?",
            (mouse_name,),
        ).fetchone()
        assert row == (contestant_name, contestant_class)
    finally:
        window.destroy()


def test_name_contestants_shows_error_dialog_on_db_failure(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        import sqlite3

        def _raise(conn):
            raise sqlite3.Error("boom")

        monkeypatch.setattr(main_window.db, "backfill_contestant_names", _raise)
        errors = []
        monkeypatch.setattr(
            main_window.messagebox, "showerror", lambda title, msg: errors.append((title, msg))
        )

        window._on_name_contestants_clicked()

        assert errors == [("Name Contestants", "boom")]
    finally:
        window.destroy()
