# DB-5: Access → SQLite SQL rewrites

Two Access-only queries from `legacy/rats_exe/RATS/Form1.cs` needed rewriting for
SQLite. Both are transcribed here with the original, the rewrite, and why
the rewrite isn't just a syntax swap. Verified by
`contest_app/tests/check_db5_rewrites.py` against real data in `live_data/rats.db` — see
that file for the executable proof.

## 1. Contestant-name backfill (`Form1.cs:3360`, `Name_contestants_Button_Click`)

### Original (Access, multi-table `UPDATE...JOIN` — not valid SQLite)

```sql
UPDATE Best_Score_Time, Contestant
INNER JOIN Mouse ON Contestant.Contestant_ID = Mouse.Contestant_ID
SET
    Best_Score_Time.Contestant_Name = [Contestant].[Contestant_Name],
    Best_Score_Time.Contestant_Class = [Contestant].[Class]
WHERE
    ((Best_Score_Time.Mouse_Name) = [Mouse].[Mouse_Name])
    AND (Best_Score_Time.Contestant_Name = "_");
```

Fills in `Contestant_Name`/`Contestant_Class` on `Best_Score_Time` rows
still carrying the `"_"` placeholder, by joining `Mouse_Name` → `Mouse` →
`Contestant`.

### Rewrite (SQLite)

```sql
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
  );
```

### The gotcha the tests caught

The live data (`live_data/rats.db`) has exactly one row with
`Contestant_Name = '_'` (`Best_Score_Time.ID = 781`) — and its `Mouse_Name`
is *also* `'_'`, which has no matching row in `Mouse` at all. It's a dummy
row, not a real backfill candidate.

Access's `INNER JOIN` excludes non-matching rows from the update target set
entirely — this dummy row is untouched, stays `'_'`/`'_'`/`'_'`.

A naive SQLite rewrite (correlated subqueries in `SET`, no `EXISTS` guard)
gets this wrong: the `WHERE Contestant_Name = '_'` clause alone still
matches the dummy row, and the scalar subqueries evaluate to `NULL` (no
match), so it silently **NULLs out the row** instead of leaving it alone —
a real behavioral divergence from the original, not just a syntax
difference. The `AND EXISTS (...)` clause reproduces the `INNER JOIN`'s
filtering semantics and fixes this. `contest_app/tests/check_db5_rewrites.py` asserts
both: that the naive version *does* corrupt the dummy row (so the trap
doesn't silently regress) and that the guarded version leaves it alone.

Also verified: `Mouse.Mouse_Name` has no duplicates in the live data, so the
join is unambiguous (a duplicate would make the correlated subquery's
"which row wins" undefined, same latent risk the original Access query
already had).

## 2. Leaderboard rank (`Form1.cs:3139`, inside `run_time_ms_message()`)

### Original (Access, nested self-join subqueries — valid SQL but worth a direct translation)

```sql
SELECT Rank
FROM (
    SELECT
        Mouse_Name,
        (SELECT COUNT(T1.Score_Time_mS)
         FROM (SELECT Best_Score_Time.Mouse_Name, Best_Score_Time.Score_Time_mS, Best_Score_Time.Competition_ID
               FROM Best_Score_Time
               WHERE (Best_Score_Time.Competition_ID = ?)) AS T1
         WHERE T1.Score_Time_mS <= T2.Score_Time_mS) AS Rank
    FROM (SELECT Best_Score_Time.Mouse_Name, Best_Score_Time.Score_Time_mS, Best_Score_Time.Competition_ID
          FROM Best_Score_Time
          WHERE (Best_Score_Time.Competition_ID = ?)) AS T2
    ORDER BY Score_Time_mS
) WHERE (Mouse_Name = ?);
```

For a given mouse in a given competition, `Rank` = count of rows in that
competition with `Score_Time_mS <= this row's Score_Time_mS`.

### Rewrite (SQLite)

```sql
SELECT
    (SELECT COUNT(*)
     FROM Best_Score_Time AS t1
     WHERE t1.Competition_ID = t2.Competition_ID
       AND t1.Score_Time_mS <= t2.Score_Time_mS) AS Rank
FROM Best_Score_Time AS t2
WHERE t2.Competition_ID = ?
  AND t2.Mouse_Name = ?;
```

Direct translation, same semantics, no window function needed at this data
scale (max ~15 rows per competition in the live data).

### The gotcha to know about (not a rewrite bug — inherent to the original formula)

This is **not** standard `RANK()` (which gives tied rows the *best* position
in their tie group, e.g. two rows tied for fastest both get rank 1, next
distinct value gets rank 3). The `<=` comparison here means tied rows all
get the *worst* position in their tie group instead.

Real example, Competition 39 (`Jehu II` 8030ms, `Jehu III` & `Orange` tied
at 9260ms, `Asymouse` 9350ms):

| Mouse | Score_Time_mS | This formula's rank | Standard `RANK()` would give |
|---|---|---|---|
| Jehu II | 8030 | 1 | 1 |
| Jehu III | 9260 | **3** | 2 |
| Orange | 9260 | **3** | 2 |
| Asymouse | 9350 | 4 | 4 |

Rank `2` is never assigned — both tied mice get `3`. This looks like an odd
design choice but it's the actual behavior the legacy app has always shown
users, so the rewrite preserves it exactly rather than "fixing" it into a
more conventional ranking. `contest_app/tests/check_db5_rewrites.py` asserts this exact
table against the live data.

**Also found**: one live row (`Best_Score_Time.ID = 348`, mouse `Dave`,
competition `89`) has a `NULL` `Score_Time_mS`. SQL's three-valued logic
already handles this correctly without any special-casing — `NULL <= x` is
always unknown/false, on either side of the comparison, so a `NULL` row
never counts toward anyone's rank and gets rank `0` itself. The test's
independent Python-side verification has to mirror that explicitly (a bare
`t <= score_time` raises `TypeError` on `None`); the SQL rewrite needs no
change for it.

## `TRUE`/`FALSE` literals (`DB-5.3`)

Three more spots use Access's `TRUE`/`FALSE` literals
(`legacy/rats_exe/RATS/Form1.cs:2568, 3075, 3348`). SQLite has natively accepted
`TRUE`/`FALSE` (as aliases for `1`/`0`) since 3.23.0 (2018). The `sqlite3`
module bundled with the Python running here reports SQLite `3.45.1`, so
**no rewrite needed** for these — confirmed, not assumed.
