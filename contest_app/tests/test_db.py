"""Unit tests for the DB access layer (APP-1.4). One test class per
function, against the in-memory demo_db fixture. rank_of()/
backfill_contestant_names() coverage here supersedes the old standalone
check_db5_rewrites.py (DB-5.6) -- same assertions, now pytest-native and
exercising rats.db's functions directly instead of duplicated SQL."""
from rats import db


def test_get_context(demo_db):
    context = db.get_context(demo_db)
    assert context is not None
    assert context.current_event == "UKMARS Spring 2026"
    assert context.current_event_id == 47


def test_select_competitions_orders_by_challenge(demo_db):
    competitions = db.select_competitions(demo_db, event_id=1, competition_class="Heats")
    assert len(competitions) > 0
    ids = {c.competition_id for c in competitions}
    assert 1 in ids  # UKNCWF, Heats
    by_id = {c.competition_id: c for c in competitions}
    assert by_id[1].scoring_model_short_name == "UKNCWF"
    assert by_id[1].entry_time_limit_s == 600


def test_select_competitions_no_match_returns_empty_list(demo_db):
    assert db.select_competitions(demo_db, event_id=999999, competition_class="Heats") == []


def test_select_scoring_model(demo_db):
    model = db.select_scoring_model(demo_db, "UKMaze")
    assert model == db.ScoringModel(
        touches_enabled=1,
        touches_cumulative=0,
        touch_time_ms=3000,
        entry_time_divider=30,
        touch_time_divider=10,
        touches_per_run=0,
    )


def test_select_scoring_model_no_match_returns_none(demo_db):
    assert db.select_scoring_model(demo_db, "NoSuchModel") is None


def test_select_pending_entries_ordered_by_sequence_number(demo_db):
    entries = db.select_pending_entries(demo_db, competition_id=80)
    assert [e.entry_id for e in entries] == [322, 337, 335, 343]
    assert entries[0].mouse_name == "Any Robot"


def test_select_pending_entries_no_match_returns_empty_list(demo_db):
    assert db.select_pending_entries(demo_db, competition_id=999999) == []


def test_select_contestant_for_mouse(demo_db):
    assert db.select_contestant_for_mouse(demo_db, "Freda") == ("Ian Butterworth", "Senior")


def test_select_contestant_for_mouse_no_match_returns_none(demo_db):
    assert db.select_contestant_for_mouse(demo_db, "No Such Robot") is None


def test_mark_entry_retired(demo_db):
    db.mark_entry_retired(demo_db, entry_id=35)
    row = demo_db.execute(
        "SELECT Entry_Used, Outcome FROM Entry WHERE Entry_ID = 35"
    ).fetchone()
    assert row == (1, "Retired")


def test_mark_entry_successful(demo_db):
    db.mark_entry_successful(demo_db, entry_id=36)
    row = demo_db.execute(
        "SELECT Entry_Used, Outcome FROM Entry WHERE Entry_ID = 36"
    ).fetchone()
    assert row == (1, "Successful")


def test_insert_entry_run(demo_db):
    before = demo_db.execute("SELECT COUNT(*) FROM Entry_Run").fetchone()[0]
    new_id = db.insert_entry_run(
        demo_db, entry_id=37, run_time_ms=12345, course_time_ms=11000, touches=2, score_time_ms=13000
    )
    after = demo_db.execute("SELECT COUNT(*) FROM Entry_Run").fetchone()[0]
    assert after == before + 1
    row = demo_db.execute(
        "SELECT Entry_ID, Run_Time_mSecs, Course_Time_mSecs, Touches, Score_Time_mSecs "
        "FROM Entry_Run WHERE Entry_Run_ID = ?",
        (new_id,),
    ).fetchone()
    assert row == (37, 12345, 11000, 2, 13000)


def test_insert_best_score_time(demo_db):
    # Entry 37 has no existing Best_Score_Time row in the demo data.
    assert demo_db.execute(
        "SELECT 1 FROM Best_Score_Time WHERE Entry_ID = 37"
    ).fetchone() is None
    new_id = db.insert_best_score_time(
        demo_db,
        entry_id=37,
        mouse_name="Freda",
        contestant_name="Ian Butterworth",
        contestant_class="Senior",
        score_time_ms=9000,
        run_time_ms=8500,
        competition_id=1,
    )
    row = demo_db.execute(
        "SELECT Entry_ID, Mouse_Name, Score_Time_mS, Competition_ID "
        "FROM Best_Score_Time WHERE ID = ?",
        (new_id,),
    ).fetchone()
    assert row == (37, "Freda", 9000, 1)


def test_update_best_score_time(demo_db):
    # Entry 111 already has Best_Score_Time.ID = 83 (Score_Time_mS 44370).
    db.update_best_score_time(
        demo_db,
        entry_id=111,
        mouse_name="Neon",
        contestant_name="Someone",
        contestant_class="Senior",
        score_time_ms=40000,
        run_time_ms=39000,
        competition_id=18,
    )
    row = demo_db.execute(
        "SELECT Score_Time_mS, Run_Time_mS FROM Best_Score_Time WHERE ID = 83"
    ).fetchone()
    assert row == (40000, 39000)
    # No new row was created -- same count as before.
    assert demo_db.execute(
        "SELECT COUNT(*) FROM Best_Score_Time WHERE Entry_ID = 111"
    ).fetchone()[0] == 1


class TestBackfillContestantNames:
    """DB-5.1, migrated from check_db5_rewrites.py."""

    def test_leaves_a_genuinely_unmatched_placeholder_row_alone(self, demo_db):
        # Best_Score_Time.ID = 781: Contestant_Name = Mouse_Name = "_",
        # with no matching Mouse row -- a dummy row, not a real backfill
        # candidate. Access's INNER JOIN excludes it from the update target
        # set entirely; the EXISTS guard reproduces that.
        db.backfill_contestant_names(demo_db)
        row = demo_db.execute(
            "SELECT Mouse_Name, Contestant_Name, Contestant_Class "
            "FROM Best_Score_Time WHERE ID = 781"
        ).fetchone()
        assert row == ("_", "_", "_")

    def test_fills_in_a_real_match(self, demo_db):
        mouse_name, contestant_id = demo_db.execute(
            "SELECT Mouse_Name, Contestant_ID FROM Mouse WHERE Contestant_ID IS NOT NULL LIMIT 1"
        ).fetchone()
        expected_name, expected_class = demo_db.execute(
            "SELECT Contestant_Name, Class FROM Contestant WHERE Contestant_ID = ?",
            (contestant_id,),
        ).fetchone()
        demo_db.execute(
            "INSERT INTO Best_Score_Time (ID, Entry_ID, Mouse_Name, Contestant_Name, "
            "Contestant_Class, Score_Time_mS, Run_Time_mS, Competition_ID) "
            "VALUES (999001, 0, ?, '_', '_', 1000, 1000, 0)",
            (mouse_name,),
        )
        db.backfill_contestant_names(demo_db)
        row = demo_db.execute(
            "SELECT Contestant_Name, Contestant_Class FROM Best_Score_Time WHERE ID = 999001"
        ).fetchone()
        assert row == (expected_name, expected_class)


class TestRankOf:
    """DB-5.2, migrated from check_db5_rewrites.py."""

    def test_matches_independent_python_computation_for_every_competition(self, demo_db):
        competitions = [
            r[0] for r in demo_db.execute("SELECT DISTINCT Competition_ID FROM Best_Score_Time")
        ]
        for comp_id in competitions:
            rows = demo_db.execute(
                "SELECT Mouse_Name, Score_Time_mS FROM Best_Score_Time WHERE Competition_ID = ?",
                (comp_id,),
            ).fetchall()
            times = [t for _, t in rows]
            for mouse_name, score_time in rows:
                # Mirror SQL three-valued logic: NULL Score_Time_mS makes
                # "<=" unknown (never true) on either side.
                expected = sum(
                    1 for t in times
                    if t is not None and score_time is not None and t <= score_time
                )
                actual = db.rank_of(demo_db, comp_id, mouse_name)
                assert actual == expected, f"competition {comp_id}, mouse {mouse_name!r}"

    def test_real_tie_case_competition_39(self, demo_db):
        """Jehu II 8030 (fastest), Jehu III & Orange tied at 9260, Asymouse
        9350. Both tied mice get the WORSE shared rank (3), not standard
        RANK() (which would give both rank 2)."""
        expected = {"Jehu II": 1, "Jehu III": 3, "Orange": 3, "Asymouse": 4}
        for mouse_name, expected_rank in expected.items():
            assert db.rank_of(demo_db, 39, mouse_name) == expected_rank, mouse_name

    def test_no_best_score_time_row_returns_none(self, demo_db):
        assert db.rank_of(demo_db, competition_id=1, mouse_name="No Such Robot") is None
