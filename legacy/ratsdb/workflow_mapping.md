# RATSdb workflow → query mapping (`REG-2`)

Source: `RATS_User_Guide_for_v3p9p1_db3p3.pdf` section 2 ("Registration
Database - RATSdb"), pages 9–24 — the full remainder of the guide's RATSdb
coverage beyond what was already read (pages 1, 4–8). **Pages 25–36 are
"Timing Supervisor – RATSts" content — that's `RATS.exe`/`APP-1` territory,
not RATSdb**, and is out of scope for this doc (flagged for whoever's doing
the `APP-1` behavior inventory: it has good screenshots of Practice/Contest
Mode, DNF, Add Touch, and the results display, pages 25–34). Page 29 of the
PDF is blank.

Cross-referenced against `queries.md`'s categorized query catalog.
Confidence levels are my assessment from name + SQL shape + guide context,
not confirmed against actual VBA (which we can't read — see
`README.md`).

## Workflows with a confident query match

| Guide section | What it does | Backing quer(ies) | Confidence |
|---|---|---|---|
| 2.9/2.10 List Contests (report) | Lists all competitions for the current event: Comp ID, Event, Challenge, Class, Date, Scoring Model | `Competitions_for_current_Event` | **High** — column list in the report screenshot matches the query's SELECT list exactly |
| 2.9/2.11 List Entries (report) | Lists entries for current event: Entry, Mouse Name, Comp, Used, Challenge, Class | `Entries_for_current_Event` | **High** — same pattern, though the report doesn't surface `Registration_Source`/`Sequence_Number` that the query also selects (probably just not columns shown in this report layout) |
| 2.3 Entry Form (combo data) | Entry form's own record source, joining Entry+Competition+Contestant+Mouse | `Entry Form Query` | **High** — literally named for this |
| 2.3 Entry Form challenge lookup | Populates the Challenge field when a competition is picked | `Get_entry_form_challenge` | **High** — named for this exact purpose, and its SQL references `Entry_form!Competition_ID_Combo` |
| 2.3/2.4 Robot Name dropdown | Populates the robot-name picker | `Get_mice_names` | **High** — trivial `SELECT Mouse_Name FROM Mouse ORDER BY Mouse_Name` |
| 2.13 All Runs (report) | Every individual run (not just best), for current event | `Run_Times_for_current_Event` | **High** — exact semantic match; query joins `Entry_Run`+`Entry`+`Mouse`+`Contestant`+`Competition` |
| 2.12 Manual Input → "Make Best Time" | Creates a manual best-score row, marks entry used, backfills mouse/contestant | `Insert_Manual_Best_Time` + `Manual_Set_Entry_Used` (+ later `Populate_Manual_Best_Score_Mouse_and_Competition` + `Populate_Best_Score_Contestants`) | **High** — SQL literally references `Forms![Manual_Timing_Entry_Run]`, the exact form shown in this section |

## Workflows with a plausible but unconfirmed match

| Guide section | What it does | Candidate quer(ies) | Notes |
|---|---|---|---|
| 2.13 Scores Report | Printable results, presumably split by Junior/Senior class | `event_results` | Its `ORDER BY Challenge, Contestant_Class, Competition_Class DESC` (Heats before Finals) is exactly the curated ordering a printable report would want. `Best_Score_Times_for_current_Event` is a weaker candidate — same data, no curated ordering, more likely backs an internal subform than a top-level report |
| 2.13 Unsplit Scores | Same as Scores Report but *not* separated by class | `Robot_Score_Times_for_current_Event` | Its SQL (`Entry LEFT JOIN Best_Score_Time`) has no class/competition breakdown at all — matches "unsplit" well, but not confirmed |

## Export queries — resolved (follow-up from earlier draft of this doc)

- **Export Entries → `Extended_Entries_Extract_for_current_Event`, confirmed.**
  Its plain-named sibling, `Entries_Extract_for_current_Event`, extracted as
  literally `SELECT;` — empty, not a real query (either genuinely blanked
  out in the source, or an Access query type ADOX's `Command.CommandText`
  can't render). `Extended_Entries_Extract_for_current_Event` has full,
  sensible SQL with export-friendly column aliases (`Robot`, `Contest`,
  `Team_Information`) — that's the real one.
- **Export Results → `Used_Entries_Contest_Name_Scores_Current_Event`,
  confirmed.** These three aren't competing candidates — they're a 3-layer
  query pipeline, each built on the previous (standard Access technique):
  `Used_Entries_with_contest_for_current_Event` (base: `Entry` ⋈
  `Competition`, filtered `Outcome <> "Not Run"`) →
  `Used_Entries_Contest_Scores_Current_Event` (adds `Best_Score_Time` via
  `LEFT JOIN`) → `Used_Entries_Contest_Name_Scores_Current_Event` (adds
  `Mouse`/`Contestant` name+class, ordered by `Competition_ID`, `Outcome
  DESC`, `Score_Time_mS`). The last, outermost one is the actual export
  source; the other two are internal building blocks, not separate export
  paths.

## Workflow with NO query match found — likely VBA-only

**Export Markdown** — "Markdown format file suitable for pasting into WordPress... created on the R: drive root folder." None of the 54 query names reference markdown or text formatting. This strongly suggests the *data* comes from one of the same sources as Export Results, but the actual markdown templating (headers, table syntax, etc.) is VBA string-building code we can't extract — confirms `README.md`'s point that `REG-3` will need to design this output format itself, not port it. **One useful clue**: `RATS.exe`'s own results display (`single_channel_results.cs`, shown on user-guide page 34, outside this doc's scope but visible in the same PDF) shows a plain `Robot | Run Time | Score Time | Contestant` table — plausible starting point for what the markdown table's columns should be, since it's fed by the same `Best_Score_Time` data, but this is inference, not confirmation.

## Query with NO matching documented workflow

`Get_Contestant_From_Mouse` (hardcoded test value `"Asymouse"` in its SQL) — already flagged in `queries.md` as a likely dev/debug leftover, confirmed here: no corresponding button/screen anywhere in the guide.

## Two distinct manual-entry paths found — only one is documented with screenshots

Section 2.12's "Manual Input" form has **two buttons**, and they map very differently:
- **"Make Best Time"** → the documented `Insert_Manual_Best_Time`/`Manual_Set_Entry_Used`/`Populate_*` chain (see above) — completes the entry, records a `Best_Score_Time` row.
- **"Save Entry Time"** → records a slower run "for completeness" *without* completing the entry (per the guide: "leave the entry open"). **No matching query found in the catalog for this.** Most likely explanation: the form is directly bound to `Entry_Run`, so "Save Entry Time" is just Access's native bound-form record-save behavior (no custom query object needed at all) rather than a named query. Worth confirming, but if true, this means `REG-3`'s design just needs a plain "insert one `Entry_Run` row" DB function — straightforward regardless of the missing query.

Separately, `Add_Sequence_No_To_Entries` (an `UPDATE...JOIN` against the `ETL_seeding_upload` staging table, `queries.md`) does **not** match section 2.8's documented sequencing workflow, which is a direct inline-grid edit of `Sequence_Number` per entry (`Sequence Competitions` form, no bulk-upload step shown). This suggests **two ways to set sequence numbers exist**: manual (grid edit, shown in the guide, no query needed) and bulk (`ETL_seeding_upload` import + this query, not documented in the guide at all — possibly a legacy/power-user path).

## Other findings worth carrying into `REG-3`

- **`Entry.Registration_Source` values confirmed**: the "Where Registered" dropdown on the Entry Form (section 2.3, page 9) offers `OnLineForm` (default), `Venue`, `UKMARS`, `Other` — this is the actual enum for that column, not previously known.
- **`Competition` setup fields, all confirmed from the New Contest form** (section 2.6): `Scoring_Model` picked from a dropdown (examples shown: `UKM19`, `UKWF19` — checked against `live_data/exports/Scoring_Model.csv`, both present (rows 7-8), no drift), `Entry_Time_Limit_Secs`, `Grace_Period_Secs`, `Runs_Allowed` (labelled "Runs Allowed" in the UI). `Contest_Name` defaults to the selected `Challenge` value and is then user-editable.
- **Most create/edit workflows (New Robot, New Contestant, New Contest, New Challenge) likely need no saved query at all** — they're straightforward bound-form inserts into `Mouse`/`Contestant`/`Competition`/`Challenge`. Nothing in the catalog obviously backs them beyond the dropdown-population queries already listed above. This *simplifies* `REG-3`'s job for these screens — they're plain CRUD, not complex business logic.
- **Difficulty picture for `REG-3`**: registration/setup CRUD looks straightforward. The harder parts are (1) the manual-timing-entry state machine (two save paths, one of which isn't backed by any extractable query), (2) picking the right report query among 3 similarly-named candidates for Export Results, and (3) reconstructing the Markdown export's template with no source to copy from at all.
