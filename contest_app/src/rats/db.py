"""DB access layer (APP-1.4): one function per DB operation `Form1.cs`
performs.

Every function takes an explicit `sqlite3.Connection`, never a hardcoded
path -- per `REPO-3`, callers own connecting to whichever `.db` file is
open (`sample_data/demo.db` by default, or a real event DB chosen via
`APP-1.8`'s "File -> Open Database..."). This replaces
`Public_Variables.connString` entirely; there is no module-level
connection here.

All writes are parameterized (the legacy app built several of these with
raw string concatenation, including straight into an executed `UPDATE` --
fine as the thing being ported, not something to carry forward).

`rank_of()` and `backfill_contestant_names()` are the `DB-5` rewrites
(see `plans/db-5-sql-rewrites.md`) -- this module is now their one home;
`DB-5.6`'s coverage moved to `contest_app/tests/test_db.py`, replacing the
old standalone `check_db5_rewrites.py`.
"""
from __future__ import annotations

import sqlite3
from dataclasses import dataclass
from typing import Optional


@dataclass
class Context:
    """The `Context` table's singleton row -- which event is active."""

    effective_date: Optional[str]
    current_event: str
    current_event_id: int
    current_competition_name: Optional[str]
    database_version: Optional[str]


def get_context(conn: sqlite3.Connection) -> Optional[Context]:
    row = conn.execute(
        "SELECT Effective_Date, Current_Event, Current_Event_ID, "
        "Current_Competition_Name, Database_Version FROM Context"
    ).fetchone()
    return Context(*row) if row is not None else None


@dataclass
class CompetitionSummary:
    competition_id: int
    competition_name: str
    scoring_model_short_name: str
    entry_time_limit_s: int
    no_of_runs_allowed: int
    grace_period_s: int


def select_competitions(
    conn: sqlite3.Connection, event_id: int, competition_class: str
) -> list[CompetitionSummary]:
    """`Competition_Class_ComboBox_SelectedIndexChanged`'s query."""
    rows = conn.execute(
        "SELECT Competition_ID, Competition_Name, Scoring_Model_Short_Name, "
        "Entry_Time_Limit_Secs, Number_of_Runs_Allowed, Grace_Period_Secs "
        "FROM Competition WHERE Event_ID = ? AND Competition_Class = ? "
        "ORDER BY Challenge",
        (event_id, competition_class),
    ).fetchall()
    return [CompetitionSummary(*row) for row in rows]


@dataclass
class ScoringModel:
    touches_enabled: int
    touches_cumulative: int
    touch_time_ms: int
    entry_time_divider: int
    touch_time_divider: int
    touches_per_run: int


def select_scoring_model(
    conn: sqlite3.Connection, scoring_model_short_name: str
) -> Optional[ScoringModel]:
    """`Competition_DataGridview_SelectionChanged`'s `Scoring_Model` lookup
    -- every scoring-formula input `DB-5`/`APP-1.7` depend on except
    `entry_time_limit_s`/`no_of_runs_allowed`/`grace_period_s`, which come
    from the `Competition` row itself (`select_competitions`, above)."""
    row = conn.execute(
        "SELECT Touches_Enabled, Touches_Cumulative, Touch_Time_mS, "
        "Entry_Time_Divider, Touch_Time_Divider, Touches_Per_Run "
        "FROM Scoring_Model WHERE Scoring_Model_Short_Name = ?",
        (scoring_model_short_name,),
    ).fetchone()
    return ScoringModel(*row) if row is not None else None


@dataclass
class PendingEntry:
    entry_id: int
    mouse_name: str


def select_pending_entries(
    conn: sqlite3.Connection, competition_id: int
) -> list[PendingEntry]:
    """`Display_pending_entries()`'s query -- entries not yet used, in
    running order. Identical to `run_order.cs`'s query (minus `Entry_ID`),
    see `APP-1.10`."""
    rows = conn.execute(
        "SELECT Entry_ID, Mouse_Name FROM Entry "
        "WHERE Competition_ID = ? AND Entry_Used = FALSE "
        "ORDER BY Sequence_Number",
        (competition_id,),
    ).fetchall()
    return [PendingEntry(*row) for row in rows]


def select_contestant_for_mouse(
    conn: sqlite3.Connection, mouse_name: str
) -> Optional[tuple[str, str]]:
    """`Mouse_DataGridview_SelectionChanged`'s Mouse -> Contestant lookup.
    Returns `(Contestant_Name, Class)`, or None if no match."""
    row = conn.execute(
        "SELECT Contestant.Contestant_Name, Contestant.Class "
        "FROM Contestant JOIN Mouse ON Contestant.Contestant_ID = Mouse.Contestant_ID "
        "WHERE Mouse.Mouse_Name = ?",
        (mouse_name,),
    ).fetchone()
    return (row[0], row[1]) if row is not None else None


def mark_entry_retired(conn: sqlite3.Connection, entry_id: int) -> None:
    """DNF (`DNF_Button_Click`): Entry_Used = TRUE, Outcome = 'Retired'.
    No `Entry_Run`/`Best_Score_Time` row is written for a DNF."""
    conn.execute(
        "UPDATE Entry SET Entry_Used = TRUE, Outcome = 'Retired' WHERE Entry_ID = ?",
        (entry_id,),
    )
    conn.commit()


def mark_entry_successful(conn: sqlite3.Connection, entry_id: int) -> None:
    """Normal run completion (`run_time_ms_message`)."""
    conn.execute(
        "UPDATE Entry SET Entry_Used = TRUE, Outcome = 'Successful' WHERE Entry_ID = ?",
        (entry_id,),
    )
    conn.commit()


def insert_entry_run(
    conn: sqlite3.Connection,
    entry_id: int,
    run_time_ms: int,
    course_time_ms: int,
    touches: int,
    score_time_ms: int,
) -> int:
    """Logs one completed run. Returns the new `Entry_Run_ID`."""
    cur = conn.execute(
        "INSERT INTO Entry_Run "
        "(Entry_ID, Run_Time_mSecs, Course_Time_mSecs, Touches, Score_Time_mSecs) "
        "VALUES (?, ?, ?, ?, ?)",
        (entry_id, run_time_ms, course_time_ms, touches, score_time_ms),
    )
    conn.commit()
    return cur.lastrowid


def insert_best_score_time(
    conn: sqlite3.Connection,
    *,
    entry_id: int,
    mouse_name: str,
    contestant_name: str,
    contestant_class: str,
    score_time_ms: int,
    run_time_ms: int,
    competition_id: int,
) -> int:
    """First personal-best for this entry in this competition. Returns the
    new `Best_Score_Time.ID`."""
    cur = conn.execute(
        "INSERT INTO Best_Score_Time "
        "(Entry_ID, Mouse_Name, Contestant_Name, Contestant_Class, "
        "Score_Time_mS, Run_Time_mS, Competition_ID) VALUES (?, ?, ?, ?, ?, ?, ?)",
        (
            entry_id,
            mouse_name,
            contestant_name,
            contestant_class,
            score_time_ms,
            run_time_ms,
            competition_id,
        ),
    )
    conn.commit()
    return cur.lastrowid


def update_best_score_time(
    conn: sqlite3.Connection,
    *,
    entry_id: int,
    mouse_name: str,
    contestant_name: str,
    contestant_class: str,
    score_time_ms: int,
    run_time_ms: int,
    competition_id: int,
) -> None:
    """An improved personal-best for an entry that already has a row."""
    conn.execute(
        "UPDATE Best_Score_Time SET Mouse_Name = ?, Contestant_Name = ?, "
        "Contestant_Class = ?, Score_Time_mS = ?, Run_Time_mS = ?, "
        "Competition_ID = ? WHERE Entry_ID = ?",
        (
            mouse_name,
            contestant_name,
            contestant_class,
            score_time_ms,
            run_time_ms,
            competition_id,
            entry_id,
        ),
    )
    conn.commit()


def backfill_contestant_names(conn: sqlite3.Connection) -> None:
    """`DB-5.1` (`Name_contestants_Button_Click`): fills in
    `Contestant_Name`/`Contestant_Class` on `Best_Score_Time` rows still
    carrying the `"_"` placeholder. The `EXISTS` guard matters -- without
    it, genuinely-unmatched placeholder rows get silently NULLed instead of
    left alone (Access's `INNER JOIN` excludes them from the update target
    set entirely; a naive correlated-subquery rewrite does not). See
    `plans/db-5-sql-rewrites.md`."""
    conn.execute(
        """
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
          AND EXISTS (
              SELECT 1
              FROM Mouse
              JOIN Contestant ON Contestant.Contestant_ID = Mouse.Contestant_ID
              WHERE Mouse.Mouse_Name = Best_Score_Time.Mouse_Name
          )
        """
    )
    conn.commit()


def rank_of(
    conn: sqlite3.Connection, competition_id: int, mouse_name: str
) -> Optional[int]:
    """`DB-5.2`: leaderboard rank -- count of rows in this competition with
    `Score_Time_mS <= this mouse's`. Tied rows share the WORSE rank, not
    standard `RANK()` semantics -- this preserves the legacy app's actual
    behavior exactly (see `plans/db-5-sql-rewrites.md`). None if the mouse
    has no `Best_Score_Time` row in this competition."""
    row = conn.execute(
        """
        SELECT
            (SELECT COUNT(*)
             FROM Best_Score_Time AS t1
             WHERE t1.Competition_ID = t2.Competition_ID
               AND t1.Score_Time_mS <= t2.Score_Time_mS) AS Rank
        FROM Best_Score_Time AS t2
        WHERE t2.Competition_ID = ? AND t2.Mouse_Name = ?
        """,
        (competition_id, mouse_name),
    ).fetchone()
    return row[0] if row is not None else None
