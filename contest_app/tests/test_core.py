"""Integration/unit tests for the headless app core (APP-1.7).

TestFullRunSequence / TestRunWithTouches / TestDnf are the stage's
required success criteria: a scripted state 2->3->4->5 message sequence
through the core against a scratch DB, a run-with-touches case exercising
DB-5's score formula, and a DNF case. Everything else here covers the
widened scope (touch counting, clear_timer, the shared start-new-entry
primitive, competition/entry selection) at unit-test grain.
"""
from fake_serial import FakeSerialTransport
from rats import db
from rats.core import AppCore
from rats.serial_protocol import Line, Message
from rats.serial_transport import Disconnected
from rats.state import AppState

# Real sample_data/demo.db fixtures used throughout: Competition 2 uses
# the 'UKMaze' Scoring_Model (Touches_Enabled=1, Touches_Cumulative=0,
# Touch_Time_mS=3000, Entry_Time_Divider=30, Touch_Time_Divider=10,
# Touches_Per_Run=0); it has two real unused entries, 36 ("Erratic") and
# 56 ("Freda"), and (confirmed) zero existing Best_Score_Time rows.
MAZE_COMPETITION = db.CompetitionSummary(
    competition_id=2,
    competition_name="Test Maze Heat",
    scoring_model_short_name="UKMaze",
    entry_time_limit_s=600,
    no_of_runs_allowed=5,
    grace_period_s=30,
)


def make_core(demo_db, transport=None):
    core = AppCore(AppState(), demo_db, transport=transport)
    core.select_competition(MAZE_COMPETITION)
    return core


class TestFullRunSequence:
    """Required success criterion: state 2->3->4->5 with
    C1SplitTime/C1RunTime, asserted against Entry_Run, Best_Score_Time,
    and rank -- plus the wire protocol's "send C1RunTime 3x" dedup
    convention, since the sequence naturally covers it."""

    def test_full_run_writes_entry_run_and_best_score_time_and_rank(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = False

        core.handle_message(Message(4, 2))  # TimerState: robot at start
        core.handle_message(Message(30, 0))  # CourseTimeMs: maze timer reset
        core.handle_message(Message(4, 3))  # TimerState: started
        core.handle_message(Message(12, 0))  # C1SplitTime: split timer reset
        core.handle_message(Message(4, 4))  # TimerState: running
        core.handle_message(Message(30, 9260))  # CourseTimeMs update
        core.handle_message(Message(4, 5))  # TimerState: run complete
        core.handle_message(Message(13, 9260))  # C1RunTime
        core.handle_message(Message(13, 9260))  # repeat (by convention, x3)
        core.handle_message(Message(13, 9260))  # repeat

        expected_score = round(9260 / 30) + 9260  # entry_time_divider only, no touches

        runs = demo_db.execute(
            "SELECT Entry_ID, Run_Time_mSecs, Course_Time_mSecs, Touches, Score_Time_mSecs "
            "FROM Entry_Run WHERE Entry_ID = 36"
        ).fetchall()
        assert runs == [(36, 9260, 9260, 0, expected_score)], "dedup should discard the 2 repeats"

        entry = demo_db.execute("SELECT Entry_Used, Outcome FROM Entry WHERE Entry_ID = 36").fetchone()
        assert entry == (1, "Successful")

        best = demo_db.execute(
            "SELECT Mouse_Name, Contestant_Name, Contestant_Class, Score_Time_mS, Run_Time_mS, Competition_ID "
            "FROM Best_Score_Time WHERE Entry_ID = 36"
        ).fetchall()
        assert best == [("Erratic", "David Hannaford", "Senior", expected_score, 9260, 2)]

        assert core.state.run.score_time_ms == expected_score
        assert core.state.run.robot_rank == 1


class TestRunWithTouches:
    """Required success criterion: DB-5's score formula with
    no_of_touches > 0, using UKMaze's real touch configuration."""

    def test_run_with_touches_applies_the_touch_terms(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(56, "Freda")
        core.state.entry.practice_mode = False

        core.handle_message(Message(4, 2))
        core.handle_message(Message(30, 0))
        core.handle_message(Message(4, 3))
        core.handle_message(Message(12, 0))
        core.handle_message(Message(4, 4))
        core.touch()
        core.touch()
        core.handle_message(Message(30, 8000))  # maze_time_ms = 8000
        core.handle_message(Message(4, 5))
        core.handle_message(Message(13, 9260))  # run_time_ms = 9260

        # entry_time_divider term + touch_time_divider term + flat
        # touch_time_ms (touches_cumulative is falsy -> added once, not
        # per-touch) + run_time_ms itself.
        expected_score = round(8000 / 30) + round(9260 / 10) + 3000 + 9260

        run = demo_db.execute(
            "SELECT Run_Time_mSecs, Course_Time_mSecs, Touches, Score_Time_mSecs "
            "FROM Entry_Run WHERE Entry_ID = 56"
        ).fetchone()
        assert run == (9260, 8000, 2, expected_score)

        best_score = demo_db.execute(
            "SELECT Score_Time_mS, Run_Time_mS FROM Best_Score_Time WHERE Entry_ID = 56"
        ).fetchone()
        assert best_score == (expected_score, 9260)
        assert core.state.run.score_time_ms == expected_score


class TestDnf:
    """Required success criterion: Outcome = 'Retired', no scoring row."""

    def test_dnf_retires_the_entry_with_no_scoring_row(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = False

        core.dnf()

        entry = demo_db.execute("SELECT Entry_Used, Outcome FROM Entry WHERE Entry_ID = 36").fetchone()
        assert entry == (1, "Retired")
        assert demo_db.execute("SELECT COUNT(*) FROM Entry_Run WHERE Entry_ID = 36").fetchone()[0] == 0
        assert demo_db.execute("SELECT COUNT(*) FROM Best_Score_Time WHERE Entry_ID = 36").fetchone()[0] == 0

    def test_dnf_is_a_no_op_in_practice_mode(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = True

        core.dnf()

        entry = demo_db.execute("SELECT Entry_Used, Outcome FROM Entry WHERE Entry_ID = 36").fetchone()
        assert entry == (0, None)


# --- Discard guards on _on_run_time ------------------------------------


class TestRunTimeDiscardGuards:
    def test_run_shorter_than_2_seconds_is_discarded(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = False

        core.handle_message(Message(13, 1500))

        assert core.state.run.run_time_ms == 1500  # still recorded, per legacy
        assert demo_db.execute("SELECT COUNT(*) FROM Entry_Run WHERE Entry_ID = 36").fetchone()[0] == 0

    def test_run_discarded_when_over_runs_allowed(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = False
        core.state.run.no_of_runs_used = 99

        core.handle_message(Message(13, 9260))

        assert demo_db.execute("SELECT COUNT(*) FROM Entry_Run WHERE Entry_ID = 36").fetchone()[0] == 0

    def test_run_discarded_in_practice_mode(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = True

        core.handle_message(Message(13, 9260))

        assert demo_db.execute("SELECT COUNT(*) FROM Entry_Run WHERE Entry_ID = 36").fetchone()[0] == 0

    def test_run_discarded_when_time_expired_past_grace(self, demo_db):
        core = make_core(demo_db)
        core.select_entry(36, "Erratic")
        core.state.entry.practice_mode = False
        core.state.run.time_left_ms = -999999
        core.state.run.grace_period_ms = 100

        core.handle_message(Message(13, 9260))

        assert demo_db.execute("SELECT COUNT(*) FROM Entry_Run WHERE Entry_ID = 36").fetchone()[0] == 0


# --- clear_timer / touch / start_new_entry / entry selection -----------


def test_touch_increments_no_of_touches(demo_db):
    core = make_core(demo_db)
    core.touch()
    core.touch()
    assert core.state.run.no_of_touches == 2


def test_clear_resets_state_without_sending_new_mouse(demo_db):
    transport = FakeSerialTransport()
    core = make_core(demo_db, transport=transport)
    core.state.run.score_time_ms = 1234
    core.state.run.no_of_touches = 3

    core.clear()

    assert core.state.run.score_time_ms == 0
    assert core.state.run.no_of_touches == 0
    assert transport.written == []


def test_start_new_entry_sends_new_mouse_and_clears_timer():
    transport = FakeSerialTransport()
    core = AppCore(AppState(), None, transport=transport)
    core.state.run.score_time_ms = 999

    core.start_new_entry()

    assert transport.written == [b"<98,0>\n"]
    assert core.state.run.score_time_ms == 0


def test_select_entry_looks_up_contestant_and_starts_new_entry(demo_db):
    transport = FakeSerialTransport()
    core = make_core(demo_db, transport=transport)

    core.select_entry(36, "Erratic")

    assert core.state.entry.entry_id == 36
    assert core.state.entry.robot == "Erratic"
    assert core.state.entry.contestant == "David Hannaford"
    assert core.state.entry.contestant_class == "Senior"
    assert transport.written == [b"<98,0>\n"]  # start_new_entry fired


def test_select_entry_falls_back_to_placeholder_for_unmatched_mouse(demo_db):
    core = make_core(demo_db)
    core.select_entry(999, "No Such Robot")
    assert core.state.entry.contestant == "_"
    assert core.state.entry.contestant_class == "_"
    assert core.state.run.robot_rank == 0


def test_toggle_practice_mode_entering(demo_db):
    core = make_core(demo_db)
    core.state.entry.practice_mode = False

    core.toggle_practice_mode()

    assert core.state.entry.practice_mode is True
    assert core.state.entry.robot == "Practice Mode"
    assert core.state.entry.contestant == "_"
    assert core.state.display_refresh.hide_best_score_times is True


def test_toggle_practice_mode_leaving(demo_db):
    core = make_core(demo_db)
    core.state.entry.practice_mode = True

    core.toggle_practice_mode()

    assert core.state.entry.practice_mode is False
    assert core.state.entry.robot == ""


def test_new_mouse_blanks_selection_outside_practice_mode(demo_db):
    core = make_core(demo_db)
    core.select_entry(36, "Erratic")
    core.state.entry.practice_mode = False
    core.state.run.robot_rank = 7

    core.new_mouse()

    assert core.state.entry.robot == ""
    assert core.state.entry.contestant == "_"
    assert core.state.run.robot_rank == 0
    assert core.state.display_refresh.hide_robot_runtimes is True


def test_new_mouse_leaves_selection_alone_in_practice_mode(demo_db):
    core = make_core(demo_db)
    core.state.entry.practice_mode = True
    core.state.entry.robot = "Practice Mode"

    core.new_mouse()

    assert core.state.entry.robot == "Practice Mode"


# --- Competition selection ----------------------------------------------


def test_select_competition_loads_scoring_model_and_run_limits(demo_db):
    core = AppCore(AppState(), demo_db)
    core.select_competition(MAZE_COMPETITION)

    assert core.state.event.competition_id == 2
    assert core.state.run.entry_time_limit_s == 600
    assert core.state.run.no_of_runs_allowed == 5
    assert core.state.run.grace_period_s == 30
    assert core.state.run.grace_period_ms == 30000
    assert core.state.scoring.touches_enabled == 1
    assert core.state.scoring.entry_time_divider == 30
    assert core.state.scoring.touch_time_ms == 3000


# --- Queue item dispatch (handle_queue_item) ----------------------------


def test_handle_queue_item_dispatches_a_parsed_line(demo_db):
    core = make_core(demo_db)
    core.state.watchdog.watchdog_ms_since_reset = 5000

    core.handle_queue_item(Line("<0,0>", Message(0, None)))

    assert core.state.watchdog.watchdog_ms_since_reset == 0


def test_handle_queue_item_ignores_an_unparsed_line(demo_db):
    core = make_core(demo_db)
    core.state.watchdog.watchdog_ms_since_reset = 5000

    core.handle_queue_item(Line("garbage", None))

    assert core.state.watchdog.watchdog_ms_since_reset == 5000  # unchanged


def test_handle_queue_item_marks_disconnected(demo_db):
    core = make_core(demo_db)
    core.state.connection.serial_port_opened = True

    core.handle_queue_item(Disconnected("device disappeared"))

    assert core.state.connection.serial_port_opened is False


def test_drain_queue_processes_a_batch_in_order(demo_db):
    core = make_core(demo_db)
    core.select_entry(36, "Erratic")
    core.state.entry.practice_mode = False

    core.drain_queue([
        Line("<4,2>", Message(4, 2)),
        Line("<4,3>", Message(4, 3)),
        Line("<4,4>", Message(4, 4)),
    ])

    assert core.state.run.timing_gates_state == 4
    assert core.state.run.no_of_runs_used == 1


# --- Sound signal (state-transition sounds, real playback deferred) ----


def test_timer_state_transitions_fire_the_right_sound_callback(demo_db):
    sounds = []
    core = AppCore(AppState(), demo_db, on_sound=sounds.append)
    core.select_competition(MAZE_COMPETITION)

    core.handle_message(Message(4, 2))
    core.handle_message(Message(4, 3))
    core.handle_message(Message(4, 4))
    core.handle_message(Message(4, 5))

    assert sounds == ["chimes.wav", "chord.wav", "tada.wav"]


def test_extra_run_decrements_runs_used(demo_db):
    core = make_core(demo_db)
    core.state.run.no_of_runs_used = 3
    core.extra_run()
    assert core.state.run.no_of_runs_used == 2


def test_extra_run_does_not_go_negative(demo_db):
    core = make_core(demo_db)
    core.state.run.no_of_runs_used = 0
    core.extra_run()
    assert core.state.run.no_of_runs_used == 0


def test_toggle_watchdog_turns_off_and_resets_alarm(demo_db):
    core = make_core(demo_db)
    core.state.watchdog.watchdog_alarm = True
    core.state.watchdog.watchdog_ms_since_reset = 5000

    core.toggle_watchdog()

    assert core.state.watchdog.watchdog_active is False
    assert core.state.watchdog.watchdog_alarm is False
    assert core.state.watchdog.watchdog_ms_since_reset == 0


def test_toggle_watchdog_turns_back_on(demo_db):
    core = make_core(demo_db)
    core.toggle_watchdog()  # off
    core.toggle_watchdog()  # on again
    assert core.state.watchdog.watchdog_active is True


def test_tick_advances_split_and_maze_time_while_running(demo_db):
    core = make_core(demo_db)
    core.state.run.split_time_running = True
    core.state.run.maze_time_running = True
    core.state.run.time_left_ms = 10000

    core.tick(250)

    assert core.state.run.split_time_ms == 250
    assert core.state.run.maze_time_ms == 250
    assert core.state.run.time_left_ms == 9750


def test_tick_does_not_advance_when_not_running(demo_db):
    core = make_core(demo_db)
    core.tick(250)
    assert core.state.run.split_time_ms == 0
    assert core.state.run.maze_time_ms == 0


def test_tick_freezes_split_time_to_run_time_once_run_complete(demo_db):
    core = make_core(demo_db)
    core.state.run.timing_gates_state = 5  # RUN_COMPLETE
    core.state.run.run_time_ms = 4321
    core.state.run.split_time_ms = 999

    core.tick(100)

    assert core.state.run.split_time_ms == 4321


def test_tick_trips_watchdog_alarm_after_threshold(demo_db):
    core = make_core(demo_db)

    core.tick(1000)
    assert core.state.watchdog.watchdog_alarm is False

    core.tick(1001)
    assert core.state.watchdog.watchdog_alarm is True


def test_tick_does_not_touch_watchdog_when_inactive(demo_db):
    core = make_core(demo_db)
    core.state.watchdog.watchdog_active = False
    core.state.watchdog.watchdog_ms_since_reset = 500

    core.tick(5000)

    assert core.state.watchdog.watchdog_ms_since_reset == 500
    assert core.state.watchdog.watchdog_alarm is False


def test_tick_watchdog_alarm_sounds_immediately_when_tripped(demo_db):
    """`AppState.watchdog`'s default `watchdog_alarm_repeat_counter` (201)
    means the very first tripped tick sounds immediately."""
    sounds = []
    core = AppCore(AppState(), demo_db, on_sound=sounds.append)
    core.select_competition(MAZE_COMPETITION)
    core.state.connection.serial_port_opened = True

    core.tick(2001)

    assert sounds == ["notify.wav"]
    assert core.state.watchdog.watchdog_alarm_repeat_counter == 0


def test_tick_watchdog_alarm_repeats_after_threshold_ticks(demo_db):
    sounds = []
    core = AppCore(AppState(), demo_db, on_sound=sounds.append)
    core.select_competition(MAZE_COMPETITION)
    core.state.connection.serial_port_opened = True
    core.state.watchdog.watchdog_ms_since_reset = 2001
    core.state.watchdog.watchdog_alarm = True
    core.state.watchdog.watchdog_alarm_repeat_counter = 200

    core.tick(0)
    assert sounds == []
    assert core.state.watchdog.watchdog_alarm_repeat_counter == 201

    core.tick(0)
    assert sounds == ["notify.wav"]
    assert core.state.watchdog.watchdog_alarm_repeat_counter == 0


def test_tick_watchdog_alarm_silent_without_serial_port_open(demo_db):
    sounds = []
    core = AppCore(AppState(), demo_db, on_sound=sounds.append)
    core.select_competition(MAZE_COMPETITION)
    core.state.connection.serial_port_opened = False

    core.tick(2001)

    assert core.state.watchdog.watchdog_alarm is True
    assert sounds == []
    assert core.state.watchdog.watchdog_alarm_repeat_counter == 201
