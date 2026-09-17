# RATS

Two separate legacy Windows applications sharing one MS Access backend,
both being ported to Python 3 / Tkinter / SQLite:

1. **`RATS.exe`** (`legacy/rats_exe/`) — decompiled VB.NET WinForms app (via
   ILSpy, hence C# with `Microsoft.VisualBasic.*` calls and the `RATS.My`
   namespace). The live *timing* app for robotics competitions (likely
   Micromouse-style maze runs) — talks to a timing-gate controller over
   serial and logs results to the database. Only used during an actual run.
2. **RATSdb** (`legacy/ratsdb/`) — a separate, actively-maintained MS Access
   2016 application organizers use for everything *around* an event:
   setting up events, registering robots/contestants, creating/sequencing
   entries, manual score correction, and results reporting/export.
   `RATS.exe` never touches this file (confirmed: zero references in its
   decompiled source); it's a fully independent app that happens to
   read/write the same backend tables.

Full replacement of both is the goal — RATSdb as a second phase, after the
timing app port (`APP-1` in `todo.md`) is underway. See `todo.md` for the
live plan (`DB-*`/`SER-*`/`ARCH-*`/`UI-*`/`PLAT-*`/`APP-*` groups for the
timing app, `REG-*` for RATSdb, `REPO-*` for repo/release prep).

**Why**: a major driver for this rewrite is escaping the Windows + full
Microsoft Access + VBA dependency entirely. RATSdb requires a *full* Access
install (not just the free ACE driver) on whatever machine manages events —
the goal is a portable, open-source, multi-platform (Linux/Mac/Windows)
replacement for both apps, not just a language port.

The two Python apps being built mirror the two legacy apps by function, not
just by source file:
- **`contest_app/`** (`APP-*` in `todo.md`) — contest-time only: collect
  race timing from the gates, display it live, store results. Replaces
  `RATS.exe`.
- **`management_app/`** (`REG-*` in `todo.md`, not started) — everything
  else: user/contestant/mouse management, event and entry setup,
  sequencing, manual corrections, results reporting. Replaces RATSdb.

## Layout

- **`legacy/`** — everything about the old system, read-only reference
  material (nothing here builds standalone). Organized as one unit
  specifically so it can be deleted wholesale once the port is complete and
  trusted — nothing in the new apps should depend on it long-term.
  - `legacy/database/` — the Access backend's schema (shared by both old
    apps, not changing): `exports/schema.sql` (mdbtools dump, structure
    only, no real data), `build_sqlite.py` (rebuilds `live_data/rats.db`
    from `live_data/exports/`), `rats_ER_diagram.pdf` (regenerated from the
    current schema via Graphviz), `rats_ER_diagram-stale.pdf` (original 2018
    ERD — missing several tables/columns added since, kept for reference).
  - `legacy/rats_exe/` — the decompiled C# WinForms app + everything
    documenting it.
    - `RATS/Form1.cs` — main window: robot/entry selection, serial
      connection, run control (touch, DNF, practice mode, calibrate),
      competition grid.
    - `RATS/Calibration_Form.cs` — sensor gate calibration.
    - `RATS/run_order.cs` — running-order display, polls DB on a timer.
    - `RATS/single_channel_display.cs`, `single_ch_display_v2.cs`,
      `single_ch_display_16_9.cs` — alternate scoreboard/display windows
      (different layouts/aspect ratios).
    - `RATS/single_channel_results.cs` — results display window.
    - `RATS/Public_Variables.cs` — global mutable state (VB "module"): the
      closest thing to a shared app-state/session object.
    - `RATS.My/*` — VB `My` namespace scaffolding, not real app logic.
    - `RATS_4p0p1a.exe` — original binary this was decompiled from.
    - `RATS.csproj`, `Properties/`, `RATS.My.Resources/` — net45 WinForms
      project file + scaffolding, references ILSpy-extracted DLLs outside
      this repo.
    - `protocol/` — vendor spec for the serial protocol
      (`preferredMessageSequences.pdf`) plus `serial_protocol.md`, a
      diffable markdown transcription of it (`SER-1`).
    - `behavior_inventory.md` — full read-through of every window's actual
      behavior, done before `APP-1` implementation started (`APP-1.0`).
  - `legacy/ratsdb/` — the RATSdb registration/reporting Access app:
    `RATS_DB_Front_end.accdb`, its official user guide, and its ~54 saved
    queries' SQL text + analysis (`queries.json`/`queries.md`,
    `workflow_mapping.md`, `REG-1`/`REG-2`). See `legacy/ratsdb/README.md`.
  - `legacy/ReadMe.txt` — the original distribution's overview (both apps,
    install instructions). Independently confirms Stand Alone Mode
    (`Q-4`) and the network-drive (`R:`) setup behind the hardcoded
    connection string — not being carried into the port (`REPO-3` uses
    explicit file selection instead, no mapped drives). Kept for reference
    while writing a new README.
  - `legacy/RATS_user_guide_summary.md` — an independent workflow summary
    (not written by us) — cross-checks `legacy/rats_exe/behavior_inventory.md`
    and `legacy/ratsdb/workflow_mapping.md`.
  - `legacy/contest-registration-processing.py` — an existing, currently-used
    pandas tool that turns Google Forms registration exports into an
    event-day checklist. Not part of either old app, but its Google-Forms
    extraction functionality is intended to become part of
    `management_app` eventually (`REG-*`).
- **`contest_app/`** — the new timing-app Python port.
  - `src/rats/` — package skeleton (just starting — see `todo.md`'s `APP-1`
    breakdown).
  - `tests/` — its test suite, including `check_db5_rewrites.py` (verifies
    the `DB-5` SQL rewrites against real data in `live_data/`).
- **`management_app/`** — the new RATSdb-replacement Python port. Empty
  skeleton (`src/`, `tests/`) — not started, phase 2 (`REG-*`).
- **`live_data/`** — **git-ignored, real competition data, never
  committed** (`REPO-1`): `RATS_Competitions_be.accdb` (live-data copy),
  `exports/*.csv` (mdbtools dump of it), `rats.db` (the resulting SQLite
  DB, built by `legacy/database/build_sqlite.py`). Has real PII (contestant
  emails, class) — see `todo.md`'s `REPO-*`/`Q-2` for the sanitization
  policy.
- **`sample_data/`** — safe demo data, committed to git (`REPO-2`, done): a
  full sanitized copy of `live_data/rats.db`, built by `build_demo_db.py`.
  `Contestant_Email_Address` is replaced with a fabricated-but-plausible
  address per contestant (real name + a random classic Microsoft demo
  domain, e.g. `ian.butterworth@fabrikam.com`) rather than blanked, so forms
  have realistic non-empty data to test against; `Class` is kept real (used
  as a filter/key in several places) — everything else, including specific
  rows existing tests depend on, is untouched. Both apps default to opening
  this DB.
- **`tools/access_export/`** — Windows-side helper scripts (need ADOX/COM
  via `pywin32`) for pulling saved Access query SQL text; not runnable on
  Linux. See `porting-issues.md`.
- **`plans/`** — forward-looking design docs for the *new* port (not legacy
  analysis, which lives under `legacy/` instead), linked from `todo.md`.
- `todo.md` — top-level working plan and record of work. `porting-issues.md`
  — detailed barrier analysis.

## Key behavior

- **Database**: MS Access via `Provider=Microsoft.ACE.OLEDB.12.0`, path hardcoded in `Public_Variables.connString`. The real backend (`RATS_Competitions_be.accdb`) has 15 tables: `Competition`, `Entry`, `Entry_Run`, `Best_Score_Time`, `Mouse`, `Contestant` (has a `Class` column, not a separate `Class` table), `Context`, `Scoring_Model`, `Challenge`, `Competition_Structure`, `Robotics_Event`, plus 4 empty `ETL_*` staging tables (`ETL_Contestant`, `ETL_Entry`, `ETL_Mouse`, `ETL_seeding_upload`) used for bulk import. See `legacy/database/` for the extracted schema and ERD.
- **Serial protocol**: `SerialPort1`, 9600 8N1, newline-terminated ASCII commands of the form `<code,payload>`, e.g. `<98,0>` (reset/query?), `<99,CALIBRATION>` / `<99,TIMER>` (mode switch). Inbound frames parsed by locating `<...>` in the buffer (`legacy/rats_exe/RATS/Form1.cs` ~line 2845). All traffic logged to a `message` log with Tx/Rx timestamps.
- **Timers**: most forms poll the DB or hardware state on a `Timer` tick rather than using events/push updates.
- **Multi-window**: main form spawns child forms (run order, calibration, single-channel displays); each tracks its own `*_window_open` / `request_*_close` flag in `Public_Variables` for cross-form coordination — effectively a poor-man's pub/sub.

## Porting to Python 3 / Tkinter / SQLite

Design decisions are made — see `todo.md` for the full record, `plans/` for
detailed writeups. Summary:
- `Public_Variables` → an explicit state-object instance, mutated only from
  the main thread (see `ARCH-1`).
- Access/OLEDB → `sqlite3`. Schema already extracted into `legacy/database/`;
  `live_data/rats.db` (git-ignored) is a working SQLite copy of the live
  data for local dev — the app itself will take an explicit DB file path,
  not a hardcoded one (`REPO-3`), defaulting to a bundled
  `sample_data/demo.db`. The two Access-only queries are already rewritten
  and tested (`DB-5`, `plans/db-5-sql-rewrites.md`).
- `SerialPort` → `pyserial`, run on a **background reader thread feeding a
  `queue.Queue`** (not polling like the original) — decided in `ARCH-1`
  because of the planned 9600→115200 baud increase; full design in
  `plans/arch-1-concurrency.md`. Preserve the `<code,payload>` framing and
  Tx/Rx logging behavior.
- The 3 `single_ch_display_*` variants collapse into **one parametrized
  class**; `single_channel_results` stays separate — decided in `UI-1`,
  confirmed from the code (they already share one exclusivity flag) and by
  the user.
- Fonts/sounds: bundle `.ttf`/`.wav` with the app; fonts registered
  per-process via `ctypes` (no install needed) — decided in `PLAT-1`,
  `plans/plat-1-platform-compat.md`.
- No existing tests or build tooling beyond `legacy/rats_exe/RATS.csproj` —
  see `legacy/` above. `APP-1` in `todo.md` breaks the actual build into
  atomic, testable stages (`APP-1.1`–`.12`).
