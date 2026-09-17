"""Unit tests for the AppState / Public_Variables replacement (APP-1.3)."""
import subprocess
import sys

from rats.state import AppState


def test_construction_needs_no_gui_db_serial_import():
    """Proves the decoupling: importing/constructing rats.state must not
    pull in tkinter, sqlite3, or serial as a side effect. Runs in a fresh
    subprocess -- by the time this test module runs, other test modules in
    the same pytest session have already imported sqlite3 themselves, so
    checking sys.modules in-process would prove nothing about rats.state."""
    result = subprocess.run(
        [
            sys.executable,
            "-c",
            "import sys\n"
            "from rats.state import AppState\n"
            "AppState()\n"
            "leaked = [m for m in ('tkinter', 'sqlite3', 'serial') if m in sys.modules]\n"
            "print(','.join(leaked))\n",
        ],
        capture_output=True,
        text=True,
        check=True,
    )
    assert result.stdout.strip() == "", f"unexpected imports: {result.stdout.strip()}"


def test_defaults_match_legacy_public_variables():
    state = AppState()

    assert state.connection.database_available is False
    assert state.connection.serial_port_opened is False

    assert state.event.robotics_event_id == 0
    assert state.event.robotics_event_date is None
    assert state.event.competition_id == 0

    assert state.entry.practice_mode is True

    assert state.run.time_left_ms == 600000
    assert state.run.time_left_s == 600
    assert state.run.grace_period_s == 30
    assert state.run.grace_period_ms == 30000
    assert state.run.no_of_runs_allowed == 5
    assert state.run.fastest_score_time_this_robot == -1

    assert state.watchdog.watchdog_active is True
    assert state.watchdog.watchdog_alarm_repeat_counter == 201


def test_each_appstate_instance_is_independent():
    a = AppState()
    b = AppState()
    a.run.score_time_ms = 1234
    assert b.run.score_time_ms == 0
