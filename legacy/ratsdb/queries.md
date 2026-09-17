# RATSdb saved queries — extracted and categorized

Extracted via `tools/access_export/query_sql_dump.py` (ADOX, Windows,
`REG-1`) — raw output in `queries.json` (54 queries, all classified by ADOX
as "procedure (action query)" since most reference form controls or take
parameters, not because they're all `INSERT`/`UPDATE`/`DELETE`). This doc
categorizes them and calls out the cross-cutting findings; not every query
gets individual prose below — see `queries.json` for anything not detailed
here.

## Cross-cutting findings

### `Rank_Query` matches `DB-5.2` exactly

```sql
SELECT Rank
FROM (SELECT Mouse_Name, (SELECT COUNT(T1.Score_Time_mS) FROM (SELECT ... FROM Best_Score_Time
      WHERE (Best_Score_Time.Competition_ID = [Comp_ID])) AS T1
      WHERE T1.Score_Time_mS <= T2.Score_Time_mS) AS Rank
      FROM (SELECT ... FROM Best_Score_Time WHERE (Best_Score_Time.Competition_ID = [Comp_ID])) AS T2
      ORDER BY Score_Time_mS) AS [%$##@_Alias]
WHERE (Mouse_Name = [Mouse]);
```

Same `<=`-count formula, same worse-of-tie semantics, as the query inline in
`legacy/rats_exe/RATS/Form1.cs:3139` that `DB-5.2` already rewrote and tested. Only
difference is parameterization (`[Comp_ID]`/`[Mouse]` as Access query
parameters here, vs. inline string substitution in `RATS.exe`). **Resolves
`REG-1.1`**: one canonical ranking algorithm across the whole RATS system,
already correctly captured in `plans/db-5-sql-rewrites.md`.

### Solved the `DB-5.1` mystery row — it's a real, documented workflow

`DB-5.1` found one real placeholder row in the live data
(`Best_Score_Time.ID = 781`, `Mouse_Name`/`Contestant_Name`/
`Contestant_Class` all `'_'`) and had to guess at its origin. RATSdb's
queries show exactly how such rows are created — a 3-step manual-timing
workflow, presumably driven by a `Manual_Timing_Entry_Run` form for
retimed/corrected runs:

1. **`Insert_Manual_Best_Time`** — creates the row from the form's fields,
   `Mouse_Name`/`Contestant_Name`/`Contestant_Class` hardcoded to `'_'`
   placeholders:
   ```sql
   INSERT INTO Best_Score_Time (Entry_ID, Run_Time_mS, Score_Time_mS, Mouse_Name, Contestant_Name, Contestant_Class)
   VALUES (Forms![Manual_Timing_Entry_Run].Entry_ID, Forms![Manual_Timing_Entry_Run].Run_Time_mSecs,
           Forms![Manual_Timing_Entry_Run].Score_Time_mSecs, "_", "_", "_");
   ```
   (Paired with `Manual_Set_Entry_Used`, which marks the `Entry` as used.)
2. **`Populate_Manual_Best_Score_Mouse_and_Competition`** — backfills the
   real `Mouse_Name`/`Competition_ID` from the linked `Entry`:
   ```sql
   UPDATE Best_Score_Time INNER JOIN Entry ON Best_Score_Time.[Entry_ID] = Entry.[Entry_ID]
   SET Best_Score_Time.Competition_ID = [Entry].[Competition_ID], Best_Score_Time.Mouse_Name = Entry.[Mouse_Name]
   WHERE (Best_Score_Time.Mouse_Name = "_");
   ```
3. **`Populate_Best_Score_Contestants`** — backfills `Contestant_Name`/
   `Contestant_Class` via `Mouse` → `Contestant`. **This is character-for-
   character identical** to `RATS.exe`'s `Name_contestants_Button_Click`
   query (`legacy/rats_exe/RATS/Form1.cs:3360`, the exact subject of `DB-5.1`) — the
   two apps share this fix-up logic verbatim.

Row `781` has `Entry_ID = 0` and `Competition_ID = 0` — step 1 ran with an
unpopulated/blank `Entry_ID` on the form (an aborted or test attempt), so
step 2's join against `Entry` never matched anything, leaving it stuck at
`'_'` forever. This **validates `DB-5.1`'s `EXISTS`-guard fix** as the
correct behavior (leave unmatched rows alone) — RATSdb's own admin tools
(next section) are the intended way to handle rows like this, not silent
backfill-to-`NULL`.

### An "unnamed" placeholder cleanup toolkit exists

RATSdb has a dedicated set of admin queries for finding and removing
exactly this kind of stuck placeholder row (`Contestant_Class = "_"`):
`Show_Unnamed_Best_Scores`, `Show_Unnamed_Entry_Runs`,
`Show_Unnamed_Used_Entries` (review), `Delete_Unnamed_Best_Scores`,
`Delete_Unnamed_Entry_Runs` (cleanup), and `Clear_Unnamed_Entries` (resets
the linked `Entry.Entry_Used` back to `False` so it can be re-attempted).
Row `781` in the live data was simply never cleaned up via this toolkit.

### More Access-only multi-table `UPDATE...JOIN` instances (relevant for `REG-3`)

Beyond `Populate_Best_Score_Contestants` (identical to `DB-5.1`'s subject),
three more queries use the same Access-only syntax that will need the same
SQLite rewrite treatment (correlated subquery + `EXISTS`/join-match guard,
per the `DB-5.1` pattern) when `REG-3` gets to implementation:
- `Populate_Manual_Best_Score_Mouse_and_Competition` (above)
- `Add_Sequence_No_To_Entries` — backfills `Entry.Sequence_Number` from the
  `ETL_seeding_upload` staging table
- `Clear_Unnamed_Entries` (above)

### Form-control references need translating to explicit parameters

Many "for_current_Event" queries don't take a normal parameter — they read
directly off a live, open Access form, e.g.:
```sql
WHERE (Competition.Event_ID=[Forms]![Context_Form]![Event_ID])
```
This is standard Access behavior (`Update_Saved_Context` persists the
`Context_Form`'s fields into the `Context` table's singleton row, `ID = 1`,
confirming the table's purpose) but has no SQL equivalent — porting these
means replacing the form reference with an explicit value from whatever
session/state object the management app ends up using (e.g. "current event
ID" tracked explicitly, not read off a live form).

### `TRUE`/`FALSE` literals appear here too

Same as `DB-5.3`'s finding for `RATS.exe` — several queries use `TRUE`/
`FALSE`/`False` (`Manual_Set_Entry_Used`, `Unused_Entries_for_current_Event`,
`Clear_Unnamed_Entries`). Already confirmed these need no rewrite on modern
SQLite (3.23+).

## By category

**Event/context setup & navigation**: `Todays_Event` (matches
`Robotics_Event.Event_Date` against `Context.Effective_Date`),
`Todays_Competitions`, `Todays_Heats`, `Todays_Finals`,
`Competitions_for_current_Event`, `Update_Saved_Context`,
`~sq_fContext_Form` (record source: all `Robotics_Event` rows, for the
event picker).

**Registration / entry management**: `Entry Display Query`,
`Entry Form Query`, `Entries_for_current_Event`,
`Unused_Entries_for_current_Event` (`Entry_Used = FALSE`), `Get_mice_names`,
`Get_entry_form_challenge`, `Add_Sequence_No_To_Entries`, `Mouse Query`
(base `Mouse` ⋈ `Contestant`, reused by several other queries).

**Bulk import / ETL seeding**: `Append_ETL_Best_Score_Time`,
`Append_ETL_Entry_Run` (straight staging-table → live-table copies),
`Clear_ETL_seeding_upload`.

**Manual timing entry workflow**: see "Cross-cutting findings" above —
`Insert_Manual_Best_Time`, `Manual_Set_Entry_Used`,
`Populate_Manual_Best_Score_Mouse_and_Competition`,
`Populate_Best_Score_Contestants`, plus `Get_Contestant_From_Mouse` (has a
hardcoded test value, `"Asymouse"` — looks like an ad hoc dev/debug query,
not part of the regular workflow).

**Orphaned-row cleanup toolkit**: `Show_Unnamed_Best_Scores`,
`Show_Unnamed_Entry_Runs`, `Show_Unnamed_Used_Entries`,
`Delete_Unnamed_Best_Scores`, `Delete_Unnamed_Entry_Runs`,
`Clear_Unnamed_Entries`.

**One-off data-fix / migration queries — not ongoing app behavior**:
`Fix_Populate_Competition_Name` (backfills `Competition_Name` from
`Challenge`), `Fix_Populate_Competition_Structure` (defaults
`Competition_Structure_Name` to `'Single Phase'`), `Patch_Successful_Entry_Outcome`
(operates on `Check_Retirements`, itself a query joining `Entry` for the
current event against `Best_Score_Time`). These look like one-time
migration scripts from when those columns were added — not something the
new app needs to replicate as live behavior, just noted for completeness.

**Reporting / results / export**: `event_results` (full results across all
competitions for the current event — ordered Challenge, Contestant_Class,
Competition_Class **DESC** so Heats list before Finals, then Score_Time_mS),
`Best_Score_Times_for_current_Event`, `Best_Scores_for_selected_Competition`,
`Robot_Score_Times_for_current_Event`, `Run_Times_for_current_Event`,
`Run_Times_for_all_Events`, `Get_Scores_for_a_Contest`,
`Get_Entries_for_a_Contest`, `Used_Entries_Contest_Name_Scores_Current_Event`,
`Used_Entries_Contest_Scores_Current_Event`,
`Used_Entries_with_contest_for_current_Event`,
`Extended_Entries_Extract_for_current_Event`,
`Entries_Extract_for_current_Event`, `Rank_Query`. (`Copy Of
Best_Score_Times_for_current_Event` looks like a stale duplicate left over
from editing — an Access user's "make a copy before I change this" habit,
not a real distinct feature.)

**Auto-generated subform record sources** (not designed queries, just Access
binding plain table/query data to an embedded subform control — lowest
priority to document further): `~sq_cEntry_form~sq_cEntry Display Query
subform`, `~sq_cNew Contest~sq_cTodays_Competitions subform`,
`~sq_cSequence_Competitions~sq_cEntry Subform`, `~sq_fContestant` (`SELECT
DISTINCTROW * FROM Contestant`), `~sq_fScoring_Model`.
