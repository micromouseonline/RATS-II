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


def test_run_state_defaults_added_in_app_1_7():
    state = AppState()
    assert state.run.entry_time_limit_s == 0
    assert state.run.split_time_running is False
    assert state.run.maze_time_running is False
    assert state.run.last_run_time_inserted == 0
    assert state.run.last_entry_id_inserted == 0
    assert state.run.last_score_time_inserted == 0


def test_scoring_config_defaults():
    state = AppState()
    assert state.scoring.touches_enabled == 0
    assert state.scoring.touches_cumulative == 0
    assert state.scoring.touch_time_ms == 0
    assert state.scoring.entry_time_divider == 0
    assert state.scoring.touch_time_divider == 0
    assert state.scoring.touches_per_run == 0


def test_clear_timer_resets_run_state_to_a_fresh_entry_baseline():
    state = AppState()
    state.run.entry_time_limit_s = 600
    state.run.split_time_ms = 1111
    state.run.maze_time_ms = 2222
    state.run.run_time_ms = 3333
    state.run.no_of_touches = 4
    state.run.score_time_ms = 5555
    state.run.fastest_score_time_this_robot = 6666
    state.run.no_of_runs_used = 3
    state.run.robot_rank = 2
    state.run.split_time_running = True
    state.run.maze_time_running = True
    state.display_refresh.hide_robot_runtimes = False

    state.clear_timer()

    assert state.run.split_time_ms == 0
    assert state.run.maze_time_ms == 0
    assert state.run.run_time_ms == 0
    assert state.run.no_of_touches == 0
    assert state.run.score_time_ms == 0
    assert state.run.fastest_score_time_this_robot == -1
    assert state.run.time_left_ms == 600000  # entry_time_limit_s * 1000
    assert state.run.no_of_runs_used == 0
    assert state.run.robot_rank == 0
    assert state.run.split_time_running is False
    assert state.run.maze_time_running is False
    assert state.display_refresh.hide_robot_runtimes is True

    # entry_time_limit_s and last_*_inserted dedup fields are NOT reset --
    # they survive across runs of the same entry (Form1.cs's clear_timer()
    # never touches them either).
    assert state.run.entry_time_limit_s == 600
