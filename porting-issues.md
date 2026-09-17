# Porting Issues — RATS (VB/WinForms) → Python 3 / Tkinter / SQLite

Findings from reading the decompiled source, not speculation. See `CLAUDE.md` for the general project overview.

## Significant barriers

### 1. No database schema exists anywhere — **in progress, tooling available**
The connection string points to `R:\RATS_Competitions_be.accdb` — a network path that isn't in this repo. There's no `.accdb` file, no exported schema, no sample data checked in here. Table/column names had to be reverse-engineered from ~20 scattered SQL strings in `legacy/rats_exe/RATS/Form1.cs` (`Context`, `Competition`, `Entry`, `Entry_Run`, `Best_Score_Time`, `Contestant`, `Mouse`, `Scoring_Model`).

The real `.accdb` files and a working copy of the RATS exe are now available in a Windows environment. `tools/access_export/` has scripts to pull ground truth from them via ODBC + ADOX (no full MS Access install needed — just the ACE driver the exe already depends on): `schema_dump.py` (tables/columns/indexes/relationships), `query_sql_dump.py` (SQL text of saved Access queries — may hold logic not visible in the VB source), and `export_data_to_sqlite.py` (full data dump into a SQLite file for dev/test seed data). See `tools/access_export/README.md` for how to run them. Report layouts are explicitly out of scope for this tooling (not extractable without Access itself).

### 2. Access SQL dialect won't translate 1:1 to SQLite — **resolved, `DB-5`**
Found a genuine blocker at `legacy/rats_exe/RATS/Form1.cs:3360`:
```sql
UPDATE Best_Score_Time, Contestant INNER JOIN Mouse ON Contestant.Contestant_ID = Mouse.Contestant_ID
SET Best_Score_Time.Contestant_Name = [Contestant].[Contestant_Name], ...
```
Multi-table `UPDATE ... JOIN` is Access-only syntax — rewritten as a
correlated subquery, but naively doing so introduces a real bug (silently
NULLs a row Access's `INNER JOIN` would have left untouched) — needs an
`EXISTS` guard to match Access's filtering semantics exactly. There's also
a deeply nested self-join ranking query (`legacy/rats_exe/RATS/Form1.cs:3139`)
computing leaderboard rank via triple-nested `SELECT COUNT(...)` — rewritten
as a direct correlated-`COUNT` translation, verified to preserve an unusual
tie-breaking quirk (tied scores share the *worse* rank, not the better one).
`TRUE`/`FALSE` literals turned out not to need a `1`/`0` rewrite — SQLite
3.23+ accepts them natively, and the `sqlite3` module here reports 3.45.1.
Full rewrites, rationale, and verification: `plans/db-5-sql-rewrites.md` and
`contest_app/tests/check_db5_rewrites.py`.

### 3. Windows-only runtime dependencies
- DB access requires the `Microsoft.ACE.OLEDB.12.0` provider — Windows/Access Database Engine only. You can't even run the original `.exe` for comparison on Linux without Wine + that driver installed (which is flaky under Wine).
- Sound alerts are hardcoded: `My.Computer.Audio.Play("C:\Windows\Media\notify.wav", ...)`, `chimes.wav`, `chord.wav`, `tada.wav` — these files don't exist outside Windows and need replacing.
- Fonts used in the scoreboard displays (`Arial Black`, specific point sizes for `Courier New`) are Windows defaults not guaranteed present on Linux/Mac.

### 4. Global mutable state with a polling architecture
`Public_Variables` is a giant static module (~90 fields) that every window reads/writes directly, coordinated only by `*_window_open` / `request_*_close` boolean flags checked on `Timer` ticks (10–20ms intervals across 6 different timers). This is inherently single-threaded-safe in WinForms because everything runs on the UI thread. If the Python port uses a background thread for serial I/O (likely, to avoid blocking Tkinter), you now have real concurrency the original never had to deal with — this needs a deliberate design, not a mechanical port.

### 5. Undocumented serial protocol — **mitigated, vendor spec available**
Framing is simple (`<code,payload>`, 9600 8N1), but the actual protocol is a large, ad-hoc `switch` statement on numeric codes (`0`–`5`, up through at least `85`, `86`, plus control codes `98`, `99`) scattered through `legacy/rats_exe/RATS/Form1.cs`. A vendor spec now lives at `legacy/rats_exe/protocol/preferredMessageSequences.pdf` — message format, 1/2/3-gate state machines, precedence rules, watchdog behavior, and an "Annex A" table of every message type (legacy-compatibility codes included). Still worth cross-checking Annex A against the `switch` cases for anything the app handles that isn't in the doc, but this is no longer a from-scratch reverse-engineering job. Tracked as `SER-1` in `todo.md`.

### 6. Decompiled-code trust gap — **de-risked, no upfront verification pass planned**
This is ILSpy output, not original source — constructs like `Operators.ConditionalCompareObjectNotEqual` and `checked` arithmetic blocks suggest VB idioms that may not decompile with perfect semantic fidelity. Subtle behavior (rounding in `maze_time_ms` conversion, grace-period color thresholds) is the kind of thing that could be subtly wrong. Originally the plan was to run the reference `.exe` systematically against real/sample data as an oracle — but it's a mature app that only really runs meaningfully against live race events, not something to batch-verify standalone. Revised approach (`REF-1` in `todo.md`): the user has run this app at real competitions several times and can answer most behavioral questions from experience as they come up during the port, plus can run a targeted check on the live system for anything specific. No upfront verification pass — questions get raised and answered as they surface during `APP-1` and later steps, since the full list of ambiguous-behavior questions isn't knowable in advance anyway.

The specific risk flagged here originally — the `object`-typed `watchdog_alarm_repeat_counter` — is now **fully resolved** by `APP-1.0`'s full read-through: it's a simple modulo-repeat alarm (increments every 10ms tick, fires `notify.wav` + sets an error label once it passes 200, i.e. ~2 seconds, then resets), no hidden complexity. Safe to reimplement directly as a plain integer counter.

## Prerequisite tasks (roughly in order)

1. ~~Recover the real database schema.~~ **Done** — extracted directly on Linux via `mdbtools` (no Windows/ODBC/ACE driver needed after all): see `legacy/database/exports/schema.sql` and the regenerated `legacy/database/rats_ER_diagram.pdf`. The `tools/access_export/schema_dump.py` script is now redundant. The saved-query-SQL piece (`tools/access_export/query_sql_dump.py`, needs ADOX/COM on Windows) is also done now — `legacy/database/queries.json` is `{}`: **zero saved queries in the `.accdb`**, so the multi-table `UPDATE...JOIN` in barrier 2 is genuinely inline VB code, not a saved query. Tracked as `DB-4` in `todo.md`.
2. ~~Get or fabricate sample data.~~ **Done** — `live_data/rats.db` (built by `legacy/database/build_sqlite.py` from the `mdbtools` CSV exports) is real dev/test seed data from the live `.accdb`. Contains real contestant names/emails — sanitize before sharing anywhere outside this environment. `tools/access_export/export_data_to_sqlite.py` is now redundant.
3. ~~Document the serial protocol as its own spec.~~ **Done** — a vendor spec (`legacy/rats_exe/protocol/preferredMessageSequences.pdf`, transcribed into `legacy/rats_exe/protocol/serial_protocol.md`) turned out to already exist, cross-checked against the decompiled `switch` in `parseData()`. Tracked as `SER-1` in `todo.md`.
4. ~~Rewrite the Access-only SQL~~ **Done** — both queries rewritten and verified against real data. See barrier 2 above, `plans/db-5-sql-rewrites.md`, `contest_app/tests/check_db5_rewrites.py`. Tracked as `DB-5` in `todo.md`.
5. **Pick the concurrency model up front**: background thread + thread-safe queue feeding Tkinter's `after()`, vs. pure polling. Design the `Public_Variables` replacement (a proper state object with locking or message-passing) around that decision before porting any single window. Tracked as `ARCH-1` in `todo.md`.
6. **Use the reference exe as an oracle — on demand, not a batch pass.** See barrier 6 above — running the original `.exe` systematically isn't practical against a live-events-only app. Behavioral questions get raised to the user (who has hands-on experience with it) as they surface during later steps. Tracked as `REF-1` in `todo.md`.
7. **Replace Windows-only touches**: swap `My.Computer.Audio.Play` + hardcoded `.wav` paths for a cross-platform sound library, pick fonts that exist on the target OS.
8. **Decide the UI strategy for the 4 near-duplicate display windows** (`single_channel_display`, `_v2`, `_16_9`, `_results`) — likely one parametrized Tkinter class with layout options rather than four separate ports.
9. **Stand up minimal test scaffolding** (mock serial device, in-memory SQLite) early — there's currently zero test coverage, and timing/scoring logic is exactly the kind of thing that fails silently in a rewrite.
