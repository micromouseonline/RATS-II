"""Rudimentary checks for the DB-5 Access -> SQLite SQL rewrites.

Runs the rewritten queries (see ../../plans/db-5-sql-rewrites.md) against a
scratch copy of sample_data/demo.db (a sanitized copy of the real data --
REPO-2) and checks their results match what the original Access queries are
known to do. Uses demo.db rather than the git-ignored live_data/rats.db so
this test actually works on a fresh clone of the repo, not just machines
that have the real data locally.

This is deliberately not a full pytest suite yet (see TEST-1 in
../../todo.md) -- just enough to prove the rewrites are correct before they
get wired into the app. unittest.TestCase is used instead of bare asserts
so pytest can pick this file up unchanged once real test scaffolding
exists.

Run directly:
    python3 contest_app/tests/check_db5_rewrites.py
"""
import shutil
import sqlite3
import tempfile
import unittest
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
DEMO_DB = REPO_ROOT / "sample_data" / "demo.db"

BACKFILL_NAIVE_SQL = """
    UPDATE Best_Score_Time
    SET
        Contestant_Name = (
            SELECT Contestant.Contestant_Name
            FROM Mouse
            JOIN Contestant ON Contestant.Contestant_ID = Mouse.Contestant_ID
            WHERE Mouse.Mouse_Name = Best_Score_Time.Mouse_Name
        ),
        Contestant_Class = (
            SELECT Contestant.Class
            FROM Mouse
            JOIN Contestant ON Contestant.Contestant_ID = Mouse.Contestant_ID
            WHERE Mouse.Mouse_Name = Best_Score_Time.Mouse_Name
        )
    WHERE Contestant_Name = '_'
"""

BACKFILL_GUARDED_SQL = BACKFILL_NAIVE_SQL + """
    AND EXISTS (
        SELECT 1
        FROM Mouse
        JOIN Contestant ON Contestant.Contestant_ID = Mouse.Contestant_ID
        WHERE Mouse.Mouse_Name = Best_Score_Time.Mouse_Name
    )
"""

RANK_SQL = """
    SELECT
        (SELECT COUNT(*)
         FROM Best_Score_Time AS t1
         WHERE t1.Competition_ID = t2.Competition_ID
           AND t1.Score_Time_mS <= t2.Score_Time_mS) AS Rank
    FROM Best_Score_Time AS t2
    WHERE t2.Competition_ID = ?
      AND t2.Mouse_Name = ?
"""


def scratch_copy():
    """Copy demo.db to a temp file and return an open connection to it."""
    tmp = tempfile.NamedTemporaryFile(suffix=".db", delete=False)
    tmp.close()
    shutil.copyfile(DEMO_DB, tmp.name)
    conn = sqlite3.connect(tmp.name)
    conn.execute("PRAGMA foreign_keys = OFF")
    return conn


class BackfillRewriteTests(unittest.TestCase):
    """DB-5.1: the Name_contestants_Button_Click backfill UPDATE."""

    def setUp(self):
        self.conn = scratch_copy()

    def tearDown(self):
        self.conn.close()

    def test_demo_data_has_exactly_one_placeholder_row_with_no_mouse_match(self):
        cur = self.conn.execute(
            "SELECT ID, Mouse_Name FROM Best_Score_Time WHERE Contestant_Name = '_'"
        )
        rows = cur.fetchall()
        self.assertEqual(len(rows), 1, "expected exactly one placeholder row in demo data")
        row_id, mouse_name = rows[0]
        match = self.conn.execute(
            "SELECT 1 FROM Mouse WHERE Mouse_Name = ?", (mouse_name,)
        ).fetchone()
        self.assertIsNone(match, "test assumption broken: dummy row's Mouse_Name now matches a real Mouse")

    def test_naive_rewrite_corrupts_the_unmatched_dummy_row(self):
        """Documents the trap: without the EXISTS guard, the dummy row gets NULLed."""
        self.conn.execute(BACKFILL_NAIVE_SQL)
        row = self.conn.execute(
            "SELECT Contestant_Name, Contestant_Class FROM Best_Score_Time WHERE ID = 781"
        ).fetchone()
        self.assertEqual(row, (None, None), "naive rewrite no longer corrupts the row -- re-check this test's premise")

    def test_guarded_rewrite_leaves_the_unmatched_dummy_row_alone(self):
        self.conn.execute(BACKFILL_GUARDED_SQL)
        row = self.conn.execute(
            "SELECT Mouse_Name, Contestant_Name, Contestant_Class FROM Best_Score_Time WHERE ID = 781"
        ).fetchone()
        self.assertEqual(row, ("_", "_", "_"))

    def test_guarded_rewrite_fills_in_a_real_match(self):
        # Synthesize a placeholder row whose Mouse_Name DOES match a real Mouse.
        mouse_name, contestant_id = self.conn.execute(
            "SELECT Mouse_Name, Contestant_ID FROM Mouse WHERE Contestant_ID IS NOT NULL LIMIT 1"
        ).fetchone()
        expected_name, expected_class = self.conn.execute(
            "SELECT Contestant_Name, Class FROM Contestant WHERE Contestant_ID = ?", (contestant_id,)
        ).fetchone()
        self.conn.execute(
            "INSERT INTO Best_Score_Time (ID, Entry_ID, Mouse_Name, Contestant_Name, Contestant_Class, "
            "Score_Time_mS, Run_Time_mS, Competition_ID) VALUES (999001, 0, ?, '_', '_', 1000, 1000, 0)",
            (mouse_name,),
        )
        self.conn.execute(BACKFILL_GUARDED_SQL)
        row = self.conn.execute(
            "SELECT Contestant_Name, Contestant_Class FROM Best_Score_Time WHERE ID = 999001"
        ).fetchone()
        self.assertEqual(row, (expected_name, expected_class))

    def test_no_duplicate_mouse_names_in_demo_data(self):
        """If this ever fails, the join key is ambiguous and the rewrite's
        'which row wins' behavior needs re-examining (same latent risk the
        original Access query already had)."""
        dupes = self.conn.execute(
            "SELECT Mouse_Name, COUNT(*) c FROM Mouse GROUP BY Mouse_Name HAVING c > 1"
        ).fetchall()
        self.assertEqual(dupes, [])


class RankRewriteTests(unittest.TestCase):
    """DB-5.2: the leaderboard rank query."""

    def setUp(self):
        self.conn = scratch_copy()

    def tearDown(self):
        self.conn.close()

    def rank_of(self, competition_id, mouse_name):
        row = self.conn.execute(RANK_SQL, (competition_id, mouse_name)).fetchone()
        return row[0]

    def test_matches_independent_python_computation_for_every_competition(self):
        competitions = [
            r[0] for r in self.conn.execute("SELECT DISTINCT Competition_ID FROM Best_Score_Time")
        ]
        for comp_id in competitions:
            rows = self.conn.execute(
                "SELECT Mouse_Name, Score_Time_mS FROM Best_Score_Time WHERE Competition_ID = ?",
                (comp_id,),
            ).fetchall()
            times = [t for _, t in rows]
            for mouse_name, score_time in rows:
                # Mirror SQL three-valued logic: a NULL Score_Time_mS makes
                # "<=" unknown (never true), on either side of the
                # comparison -- not a Python None-comparison error.
                expected = sum(
                    1 for t in times
                    if t is not None and score_time is not None and t <= score_time
                )
                actual = self.rank_of(comp_id, mouse_name)
                self.assertEqual(
                    actual, expected,
                    f"competition {comp_id}, mouse {mouse_name!r}: expected rank {expected}, got {actual}",
                )

    def test_real_tie_case_competition_39(self):
        """Jehu II 8030 (fastest), Jehu III & Orange tied at 9260, Asymouse 9350.
        The Access '<=' formula gives BOTH tied mice the WORSE shared rank (3),
        not the standard RANK() behavior (which would give both rank 2)."""
        expected = {
            "Jehu II": 1,
            "Jehu III": 3,
            "Orange": 3,
            "Asymouse": 4,
        }
        for mouse_name, expected_rank in expected.items():
            self.assertEqual(self.rank_of(39, mouse_name), expected_rank, mouse_name)


if __name__ == "__main__":
    unittest.main(verbosity=2)
