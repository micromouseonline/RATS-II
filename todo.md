# TODO — RATS Python/Tkinter/SQLite port

Top-level working plan and record of work for porting **two** legacy apps
sharing one MS Access backend: `RATS.exe` (the live timing app) and RATSdb
(the registration/reporting app, `legacy/ratsdb/README.md`) — full replacement of
both is the goal, RATSdb as a second phase after the timing app. See
`CLAUDE.md` for the project overview and `porting-issues.md` for the
detailed analysis of why each barrier below is a barrier. Detailed plans for
individual steps live in `plans/` (linked below once written).

Items are grouped and given a short ID (`GROUP-N`) for reference elsewhere
(commit messages, plan files, discussion). Groups: `DB` (database),
`SER` (serial protocol), `ARCH` (architecture/state/concurrency),
`REF` (verification against the reference exe), `PLAT` (platform
compatibility), `UI` (windows/display), `TEST` (test scaffolding),
`APP` (timing app build), `REG` (RATSdb registration/reporting app,
phase 2 — not blocking `APP-*`), `REPO` (preparing the repo for public
GitHub release — real data out of version control, demo DB, general
tidiness).

## Done

- [x] **DB-1** — Extract real DB schema + data from `RATS_Competitions_be.accdb`
      via `mdbtools` on Linux (no Windows/ODBC needed) —
      `legacy/database/exports/schema.sql`, `legacy/database/build_sqlite.py`,
      `live_data/exports/*.csv`, `live_data/rats.db`.
- [x] **DB-2** — Regenerate an accurate ERD from the current schema
      (Graphviz) — `legacy/database/rats_ER_diagram.pdf`; old 2018 ERD kept as
      `legacy/database/rats_ER_diagram-stale.pdf` for reference.
- [x] **DB-3** — Document real table/column list in `CLAUDE.md`.
- [x] **SER-1** — Transcribed the vendor spec
      (`legacy/rats_exe/protocol/preferredMessageSequences.pdf`) into
      `legacy/rats_exe/protocol/serial_protocol.md`, then cross-checked it against the
      decompiled `switch` in `parseData()` (`legacy/rats_exe/RATS/Form1.cs:2878`).
      Finding: this build only implements 19 inbound codes (`0–6, 12, 13, 30,
      71–73, 81–86`) and 2 outbound (`98, 99`) — it has **no dual-channel
      (`C1`/`C2`) support and never does the `96`/`97` timer-type handshake**,
      even though those are in the vendor spec. See `Q-3` below.
- [x] **DB-4** — Ran `tools/access_export/query_sql_dump.py` on Windows
      (needed a 32-bit Python to match the 32-bit ACE OLEDB provider — 64-bit
      Python gave "Provider cannot be found"). Result: `legacy/database/queries.json`
      is `{}` — **zero saved queries in the `.accdb`**. All SQL logic lives
      inline in the VB/C# source; nothing hidden to account for.
- [x] **DB-5** — **Rewrote Access-only SQL for SQLite**, verified against
      real data in `live_data/rats.db`. Full write-up:
      `plans/db-5-sql-rewrites.md`. Tests: `contest_app/tests/check_db5_rewrites.py`
      (7 tests, all passing — run with `python3 contest_app/tests/check_db5_rewrites.py`).
  - [x] **DB-5.1** — Backfill `UPDATE...JOIN` (`Form1.cs:3360`) rewritten
        as correlated subqueries **plus an `EXISTS` guard**. Without the
        guard, the rewrite silently NULLs out the one real placeholder row
        in the live data (`Best_Score_Time.ID = 781`) instead of leaving it
        untouched like Access's `INNER JOIN` does — caught by the tests,
        not assumed. Details + both SQL versions in the plan doc.
  - [x] **DB-5.2** — Rank query (`Form1.cs:3139`) rewritten as a direct
        correlated-`COUNT` translation (no window function needed at this
        data scale). Verified bit-for-bit against every competition in the
        live data, including the exact tie semantics (tied rows share the
        *worse* rank, e.g. real Competition 39: ranks come out `1, 3, 3, 4`,
        not `1, 2, 2, 4`) and a real `NULL Score_Time_mS` row (handled
        correctly by SQL's three-valued logic with no special-casing).
  - [x] **DB-5.3** — Confirmed (not assumed): the `sqlite3` module here
        reports SQLite `3.45.1`, well past the `3.23` baseline where
        `TRUE`/`FALSE` literals became native keywords — the other 3
        `TRUE`/`FALSE` spots (`Form1.cs:2568, 3075, 3348`) need no rewrite.
  - [x] **DB-5.4** — Both rewrites, with rationale and the gotchas above,
        recorded in `plans/db-5-sql-rewrites.md`.
  - [x] **DB-5.5** — `contest_app/tests/check_db5_rewrites.py`: runs both rewrites
        against a scratch copy of `live_data/rats.db`, cross-checks the rank
        query against an independent Python computation across every
        competition, and specifically exercises the tie case and the
        unmatched-row case.
  - [x] **DB-5.6** — Kept as `unittest.TestCase`s (not a one-off script) so
        pytest can pick the file up unchanged once `TEST-1` stands up real
        scaffolding.
- [x] **ARCH-1** — **Decided: background thread + thread-safe queue**, not
      polling. Confirmed the legacy app is pure UI-thread polling
      (`Timer1_Tick`, `Form1.cs:2648`, every 10ms, no background thread or
      `DataReceived` event) — deciding factor against repeating that here is
      the move to **115200 baud** (12x the byte rate), which shrinks the
      margin for a busy UI thread to stall the serial read without dropping
      bytes. Secondary bonus: a queue of parsed messages is a
      transport-agnostic boundary, so Bluetooth Classic or a websocket
      source later just become additional producers feeding the same queue
      — not building that now, just not closing the door on it for free.
      Full design (thread/queue shape, the state-object mutation rule,
      testability split, open questions for `APP-1`):
      `plans/arch-1-concurrency.md`.
- [x] **UI-1** — **Decided: one parametrized class for the 3 live-timer
      layout variants** (`single_channel_display`, `single_ch_display_v2`,
      `single_ch_display_16_9`); `single_channel_results` stays a separate
      class. Confirmed from the code, not assumed: all three layout variants
      read/write the *same* shared flag,
      `Public_Variables.single_channel_window_open` (e.g.
      `single_channel_display.cs:672/686`, `single_ch_display_v2.cs:848/863`,
      `single_ch_display_16_9.cs:710/725`), and each button click handler
      toggles against that shared flag rather than opening a second window —
      the legacy app already treats them as one mutually-exclusive slot with
      three interchangeable skins, not three independent displays.
      `single_channel_results` has its own separate flag pair
      (`single_channel_results.cs:248/278`) and is genuinely
      independently-open-able — matches the real usage pattern (one live
      timer display + one results display, on two separate monitors for
      audience view). User confirmed this matches recollection of real
      event usage.
- [x] **PLAT-1** — **Decided: fonts bundled as `.ttf` and registered
      per-process via `ctypes`** (per-OS APIs — `AddFontResourceEx` on
      Windows, `CTFontManagerRegisterFontsForURL` on macOS,
      `FcConfigAppFontAddFile` on Linux — no extra dependency, no
      install/admin needed), to guarantee the scoreboard looks the same
      everywhere rather than hoping a similar system font is present.
      Sounds: `.wav` files bundled at a relative path (decided); which
      cross-platform playback library (`simpleaudio` /  `pygame.mixer` /
      `playsound`) stays open — small, low-risk, deferred to `APP-1`. Full
      writeup: `plans/plat-1-platform-compat.md`.

## Next steps

- [ ] **REF-1** — **Use the reference exe as an oracle — on demand, not a
      scheduled pass.** Running the original `.exe` systematically isn't
      practical: it's a mature app that only really runs meaningfully
      against live race events, not something to batch-verify standalone.
      Revised approach: the user has run this app at real competitions
      several times and can answer most behavioral questions from experience
      as they come up during the port, and can run a targeted check on the
      live system for anything that needs it. We also don't yet know the
      full list of ambiguous-behavior questions the port will raise — no
      point trying to front-load them all now. So: no upfront verification
      pass; flag specific behavior questions (rounding in `maze_time_ms`,
      grace-period color thresholds, etc.) to the user as they surface
      during `APP-1` and later steps, instead of batching them here.
- [ ] **APP-1** — **Design + build the Python app skeleton.** Depends only
      on `ARCH-1`, `UI-1`, and `PLAT-1` (all done) — not blocked on `REF-1`.
      Broken into atomic, independently-testable stages rather than one
      big-bang implementation — see dependency notes and success criteria
      below. Not strictly sequential: `APP-1.2`/`.3`/`.4`/`.5` can happen in
      any order relative to each other once `APP-1.1` is done; `APP-1.6`
      needs `.5`; `APP-1.7` needs `.3`+`.4`+`.6`; everything from `APP-1.8`
      on needs `.7` (first point there's a core to wire a window to).
      Breakdown may get revisited before/during implementation.
  - [x] **APP-1.0** — Full behavior inventory of every previously-unread
        line in the timing app's window files (`Form1.cs`'s remaining
        handlers, `Calibration_Form.cs`, `run_order.cs`, all 4 display
        windows) — closes the gap flagged before committing to code.
        `plans/app-1-behavior-inventory.md`. Widened `.4`/`.6`/`.7`/`.10`/
        `.11` below based on what it found; raised `Q-4`/`Q-5`. Confirmed:
        no new Access-only SQL beyond `DB-5.1`, no new serial codes beyond
        `SER-1`, no new Windows-only deps beyond `PLAT-1`.
  - [x] **APP-1.1** — Project scaffolding: `contest_app/pyproject.toml`
        (src layout, `[tool.setuptools.packages.find] where = ["src"]`,
        `[tool.pytest.ini_options] testpaths = ["tests"]`), dependency
        `pyserial>=3.5`, dev dependency `pytest>=8` (sound library still
        TBD — deferred to `APP-1.12`, not needed yet). Verified (not
        assumed) both from `contest_app/` and from the repo root:
        `pip install -e '.[dev]'` / `pip install -e './contest_app[dev]'`
        both succeed into a venv, and `pytest` collects cleanly with 0
        items either way (exit code 5, "no tests ran" — expected, not an
        error: `check_db5_rewrites.py` is deliberately not
        pytest-discoverable yet, per its own docstring; real tests start at
        `APP-1.2`). `contest_app/tests/README.md` updated with the venv
        setup steps.
  - [x] **APP-1.2** — Test scaffolding (`TEST-1`'s deliverable):
        `contest_app/tests/dbfixture.py`'s `demo_db_connection()` builds an
        **in-memory** SQLite copy of `sample_data/demo.db` per call (via
        `sqlite3`'s backup API — no disk writes, no cross-test leakage);
        `contest_app/tests/fake_serial.py`'s `FakeSerialTransport` scripts
        `read()`/`in_waiting`/`close()`/`feed()` matching the slice of
        pyserial's interface the future reader thread (`APP-1.6`) will use,
        so byte sequences can be fed through without hardware. **Success:**
        `contest_app/tests/test_fixtures.py` (4 tests, incl. an isolation
        check) exercises both fixtures and passes; `check_db5_rewrites.py`
        now calls `demo_db_connection()` instead of its own tempfile-copy
        `scratch_copy()` — its original 7 tests still pass unchanged, both
        standalone and via pytest, confirmed from both `contest_app/` and
        the repo root.
  - [x] **APP-1.3** — State object: the `Public_Variables` replacement —
        `contest_app/src/rats/state.py`'s `AppState`, plain dataclasses
        grouped into `ConnectionState`/`EventState`/`EntryState`/
        `RunState`/`DisplayRefreshState`/`WindowState`/
        `GateDiagnosticsState`/`WatchdogState`, mirroring
        `Public_Variables.cs` field-for-field for traceability (two
        deliberate deviations, documented in the module docstring:
        `connString` dropped — DB path is `APP-1.4`/`.8`'s concern per
        `REPO-3` — and `public_competition_name`/`public_competition_class`
        renamed to drop the meaningless `public_` prefix). Scoring-model
        fields (`touches_enabled` etc.) deliberately left out — those are
        Form1-local in the legacy code, not `Public_Variables`, and
        `APP-1.4` is where that lookup's home gets decided. **Success:**
        `contest_app/tests/test_state.py` asserts defaults match the legacy
        values (`time_left_ms=600000`, `grace_period_s=30`,
        `no_of_runs_allowed=5`, `fastest_score_time_this_robot=-1`,
        `watchdog_alarm_repeat_counter=201`, etc.) and instance
        independence; a subprocess-isolated check confirms constructing
        `AppState` never pulls in `tkinter`/`sqlite3`/`serial` (proves the
        decoupling for real, not by inspection — an in-process check would
        have been contaminated by other test modules' own imports).
  - [x] **APP-1.4** — DB access layer: `contest_app/src/rats/db.py`, one
        function per DB operation `Form1.cs` performs — `get_context`,
        `select_competitions`, `select_scoring_model` (the
        `Competition_DataGridview_SelectionChanged` lookup: `touches_enabled`,
        `touches_cumulative`, `touch_time_ms`, `entry_time_divider`,
        `touch_time_divider`, `touches_per_run` — `entry_time_limit_s`/
        `no_of_runs_allowed`/`grace_period_s` come from `select_competitions`'s
        `Competition` row instead, found in `APP-1.0`), `select_pending_entries`,
        `select_contestant_for_mouse`, `mark_entry_retired`,
        `mark_entry_successful`, `insert_entry_run`, `insert_best_score_time`,
        `update_best_score_time`, plus the `DB-5` rewrites
        `backfill_contestant_names`/`rank_of`. **Takes an explicit
        `sqlite3.Connection`, not a hardcoded connection string** — per
        `REPO-3`, replaces `Public_Variables.connString`; all writes
        parameterized (the legacy code built several via raw string
        concatenation — not carried forward). **Success:**
        `contest_app/tests/test_db.py`, 26 tests, one per function, against
        real values from `sample_data/demo.db`; `DB-5`'s tests ported in
        alongside (fulfills `DB-5.6`) — `check_db5_rewrites.py` kept (still
        referenced from closed `DB-5`/`REPO-4` history, not worth rewriting)
        but now calls `db.backfill_contestant_names()`/`db.rank_of()`
        instead of duplicating their SQL; its own 7 tests still pass.
  - [x] **APP-1.5** — Serial frame parser: `contest_app/src/rats/serial_protocol.py`,
        pure functions, no I/O/threading. Line-based (`\n`-terminated),
        **not** the literal bracket-hunting-across-an-arbitrary-buffer
        approach `parseData()` uses — changed per `Q-6` (user correction):
        a line not starting with `<` is totally ignored, everything after
        the closing `>` on a line is ignored (one message per line max),
        and every raw line survives (via `Line`) for a later logging layer
        to record regardless of parse outcome. `parse_message()` covers all
        19 inbound codes' value scaling (`*100`/`*10`/none per code, per
        `parseData()`'s handlers) plus the boolean trigger codes and a
        lenient VB-`Val()`-alike numeric parse; `format_new_mouse()`/
        `format_set_mode()` cover the 2 outbound codes. **Success:**
        `contest_app/tests/test_serial_protocol.py`, 36 tests — a line
        split across two reads, multiple lines in one read, CRLF handling,
        the `Q-6` garbage-line-not-recovered case, one test per inbound
        code, plus the outbound formatters.
  - [x] **APP-1.6** — Serial transport: `contest_app/src/rats/serial_transport.py`.
        `SerialReader` (background thread + `queue.Queue`, per
        `plans/arch-1-concurrency.md`): drains a duck-typed transport
        (real `pyserial.Serial` or `APP-1.2`'s `FakeSerialTransport`
        unchanged), splits into `Line`s via `APP-1.5`'s parser, enqueues
        every line — not just parsed ones, per `Q-6` — so a later logging
        layer never misses one; `start()`/`stop(timeout)` lifecycle;
        transport errors surface as `Disconnected` queue items, not thread
        crashes. `configure_serial_port`/`open_serial_port` replace
        `connect_BTN_Click` (`Form1.cs:2400`)'s setup — **baud rate is now
        a parameter, not hardcoded** (`APP-1.8`'s port selector will offer
        a choice, per the user's note), read timeout defaults to `0.2`s
        (down from legacy's `10000`ms, needed since this thread actually
        blocks in `read()` — see `plans/arch-1-concurrency.md`, now
        synced). `send_new_mouse`/`send_set_mode` cover the 2 outbound
        writes, straight to the wire per `ARCH-1`, not through the queue.
        **Success:** `contest_app/tests/test_serial_transport.py`, 9
        tests — in-order queue delivery, a line split across two reads,
        garbage lines still reaching the queue for logging, clean shutdown
        within timeout (stable across repeated runs), a transport-error →
        `Disconnected` case, both outbound senders, and
        `configure_serial_port` against a real unopened `pyserial.Serial()`
        confirming the baud rate actually varies.
  - [x] **APP-1.7** — Headless app core: `contest_app/src/rats/core.py`'s
        `AppCore`, wiring `.3`+`.4`+`.6` together exactly as `parseData()`'s
        switch and the `*_message()` handlers do. **Still no Tkinter at
        all.** Covers the widened scope from `APP-1.0`'s findings —
        `touch()`, `dnf()` (guarded by practice mode, no scoring row
        written, per legacy), `AppState.clear_timer()` as one method
        (moved onto the state object per the plan here), and
        `start_new_entry()` as the one shared `NewMouse` (`<98,0>`) +
        `clear_timer()` primitive replacing 3 duplicated legacy call
        sites — plus `select_competition()`/`select_entry()`/
        `toggle_practice_mode()`/`new_mouse()` (needed to drive the
        integration tests) and `drain_queue()`/`handle_queue_item()`
        (`Line`/`Disconnected` dispatch, per `APP-1.6`). Sound and
        outbound-write side effects surface through optional callbacks
        (`on_sound`, `transport`) rather than being baked in — no audio
        import, no real serial connection needed to test; actual playback
        wiring is `APP-1.12`. **Extended `rats/state.py`**: `RunState`
        gained `entry_time_limit_s`/`split_time_running`/
        `maze_time_running`/the 3 `last_*_inserted` dedup fields, and a
        new `ScoringConfig` group holds the `Scoring_Model` lookup —
        fields `APP-1.3` deliberately left undecided pending this stage.
        **Deliberately out of scope** (periodic-`Timer1_Tick`-driven, not
        message-driven — `APP-1.8`'s job): the local time interpolation
        ticking `split_time_ms`/`maze_time_ms` between real gate messages,
        and the watchdog alarm's repeat-counter/sound.
        **`Q-7` found and resolved** (user-confirmed): the decompiled code
        compares `Touches_Enabled`/etc. literally against `-1`, but the
        real backend data stores `1`/`0` — taken literally the touches
        bonus would be permanently dead code. `ScoringConfig` treats these
        as plain truthy flags instead, confirmed correct by the user
        (touches are simply true/false) rather than a settled guess. A
        related fact recorded but not implemented: the user believes a
        touched entry should never out-rank an untouched entry on a tied
        score, but "probably not in the code" either — `db.rank_of()`
        doesn't consider touches; noted for later (`REG-*` or a future
        revisit), not actioned now.
        **Success:** `contest_app/tests/test_core.py`, 23 tests — the
        three required cases (a full state `2→3→4→5` run with
        `C1SplitTime`/`C1RunTime`, including the wire protocol's "send
        `C1RunTime` 3×" case proving the dedup guard; a run-with-touches
        case exercising `DB-5`'s score formula; a DNF case) against real
        `sample_data/demo.db` entries/competitions, all matched against
        `Entry_Run`, `Best_Score_Time`, and rank directly — plus discard-
        guard cases and unit coverage for every other method. First true
        integration test of the whole non-GUI pipeline.
  - [ ] **APP-1.8** — Main window: thin Tkinter wiring around `APP-1.7`'s
        core. Broken into 4 sub-stages (decided with the user — too much
        UI surface for one shot, and the stage's only real success
        criterion is a manual smoke test the user has to run, not
        something worth batching into one big diff to review at once).
        Not strictly sequential beyond `.1` unblocking the rest (`.2`/`.3`
        both need `.1`'s connection/queue-draining loop and DB-open;
        `.4`'s log files ideally land early enough to cover `.2`/`.3`'s
        manual testing too, but functionally only depend on `.1`). Full
        layout/widget-inventory plan, and three standing conventions the
        user set for this stage and `APP-1.9`–`.12`: `.1` builds
        placeholders for the **entire** main-window layout up front (not
        grown sub-stage by sub-stage, so the user can react to/request
        changes to the whole layout early — inert widgets get activated
        by later sub-stages, not added fresh); **responsive layout from
        that first placeholder pass**, not retrofitted later (weighted
        `grid`/`pack`, no fixed-pixel widgets, matching `Q-5`/`UI-1`'s
        resizable-not-fixed-pixel standard already set for the display
        windows); and **every manual-smoke-test stage from here on ships
        a numbered test checklist with expected results** for the user to
        run against their own hardware (confirmed available). Full writeup:
        `plans/app-1-8-main-window-layout.md`.
    - [ ] **APP-1.8.1** — Window shell + connection: COM port dropdown
          (`pyserial.tools.list_ports`), **baud-rate choice added
          alongside it** (legacy hardcoded `9600` in `connect_BTN_Click`,
          no UI for it at all — noted during `APP-1.6`, needed since the
          port moves to 115200 and a user may still have older 9600
          hardware), Connect/Disconnect wired to
          `configure_serial_port`/`SerialReader` (`APP-1.6`). **`Q-4`
          resolved: "Stand Alone Mode" (degraded offline operation when
          the DB is unreachable) is a real requirement**, needed for
          practice sessions without a database — implemented per `REPO-3`
          as **DB file selection**, not a degraded mode: opens
          `sample_data/demo.db` by default, "File → Open Database…" for a
          real file, remembering the last-opened path in a small local
          git-ignored config file. Also starts the `after()`-driven queue-
          draining loop on connect (nothing visible reacts to it yet --
          `.2`/`.3` add that). **Success:** manual smoke test against real
          (or looped-back) hardware for the actual connect; automated
          coverage for the config-file read/write and port/baud
          enumeration glue; "constructs without error" for the rest.
    - [ ] **APP-1.8.2** — Event/competition/entry selection: competition-
          class dropdown + competition grid (`db.select_competitions`),
          entry/mouse grid (`db.select_pending_entries`), wired to
          `core.select_competition()`/`core.select_entry()`. **Success:**
          manual smoke test against `sample_data/demo.db`'s real
          event/competition/entry data.
    - [ ] **APP-1.8.3** — Run-control buttons + live display:
          `Touch`/`DNF`/`Clear`/`New Mouse`/`Practice Mode`/
          `Extra Run`/`WatchDog` wired to their `AppCore` methods, plus
          the live run/score/rank/time-left display updating off the
          drained queue. **Also where `APP-1.7`'s deliberately-deferred
          local time interpolation gets wired up** (`Timer1_Tick`'s
          `split_time_ms`/`maze_time_ms` ticking between real gate
          messages) — it needs an actual periodic timer, which only
          exists once this stage's `after()` loop is running. **Success:**
          manual smoke test against real hardware exercising a full run,
          including touches and a DNF.
    - [ ] **APP-1.8.4** — Monitor + log files + child-window launch
          buttons: the raw Tx/Rx `RichTextBox` monitor (`Monitor`/
          `Verbose` toggle buttons), **the two timestamped log files**
          (user confirmed: replicate this — every line, per `Q-6`, not
          just successfully-parsed ones — but relocated to sit next to
          the open DB file or a project-local `logs/` dir instead of a
          hardcoded `Desktop` path, which isn't meaningful cross-platform;
          exact legacy filename pattern in
          `plans/app-1-behavior-inventory.md`), and the four launch
          buttons for the calibration/run-order/display/results windows
          (`APP-1.9`–`.12` build the windows themselves; these just toggle
          the existing open/close-request flags in `AppState.windows`).
          **Success:** manual check that every line arriving on the queue
          shows up in the log file, parsed or not; "constructs without
          error" for the launch buttons (their target windows don't exist
          until `.9`–`.12`).
  - [ ] **APP-1.9** — Calibration form. `APP-1.0` confirmed this is simple —
        a static 3×3 grid driven entirely by state fields `APP-1.7` already
        populates from protocol codes `71`–`73`/`81`–`86`, no calculation of
        its own. **Success:** manual smoke test against real gate hardware
        in calibration mode.
  - [ ] **APP-1.10** — Run-order display. `APP-1.0` found the exact query:
        `SELECT Mouse_Name AS Robot FROM Entry WHERE Competition_ID = ? AND
        Entry_Used = FALSE ORDER BY Sequence_Number` (identical to
        `Display_pending_entries()`'s). **Success:** unit test this exact
        query against a scratch DB, not a generic "ordering" claim.
  - [ ] **APP-1.11** — Display windows: one parametrized single-channel
        class + separate results display, per `UI-1`. **`Q-5` resolved,
        two parts:**
        - **Aspect ratio / sizing**: the legacy windows were each designed
          for a specific fixed resolution, not a real aspect-ratio system.
          The port should be **fully resizable**, supporting **4:3 and
          16:9** as the initial target aspect ratios (more can be added
          later — don't over-engineer a general N-aspect-ratio system now).
        - **Field content**: **normalize across all layouts** — one
          consistent feature set (contestant name, rank, event name,
          running-order grid, and the entry-runs query with `Time_of_Run`
          ordered `DESC`) regardless of aspect ratio/size. The legacy
          `16_9`-is-a-hybrid quirk (v2's fields + base's query) was
          accidental, not a deliberate design worth preserving — only
          theme/size/layout should vary by aspect ratio, not which data is
          shown.
        `single_channel_results` confirmed structurally different: one-shot
        snapshot on open, never refreshes — its success criterion is
        "populates once, matches `Best_Score_Time` for the selected
        competition ordered by `Score_Time_mS`," not a live-update test.
        **Success:** instantiate at both 4:3 and 16:9, assert no
        construction errors and identical field content at both; manual
        visual check that layout scales sensibly across a range of window
        sizes within each aspect ratio.
  - [ ] **APP-1.12** — `PLAT-1` wiring: bundle real `.ttf`/`.wav` assets,
        hook up `ctypes` font registration and the chosen sound library into
        the windows from `.8`–`.11`. **Success:** manual visual/audio check
        on at least two platforms that fonts render identically and sounds
        play.

## RATSdb (phase 2 — after `APP-*`, not blocking it)

Discovered 2026-09-17: `RATS.exe` isn't the whole system. **RATSdb**
(`legacy/ratsdb/RATS_DB_Front_end.accdb`) is a separate, actively-maintained MS
Access 2016 app (user guide dated May 2024 — newer than the timing exe)
that organizers use for event setup, robot/contestant registration, entry
sequencing, manual score correction, and results reporting/export. `RATS.exe`
never touches it. Full details: `legacy/ratsdb/README.md`. User decision: full
replacement of both apps is the goal, RATSdb as a second phase.

**Porting methodology differs from `APP-*`**: RATSdb's forms/VBA/macros are
locked in Access's proprietary format, unreadable by any tool we have — no
ground truth to verify a mechanical port against, unlike `RATS.exe`'s full
decompiled source. `REG-*` is a **functionally-equivalent rebuild** from the
user guide's documented workflows + the actual query logic, not a
line-for-line port. Full reasoning: `legacy/ratsdb/README.md`.

- [x] **REG-1** — Extracted RATSdb's saved queries (54, not ~50) via
      `tools/access_export/query_sql_dump.py` on Windows, same as `DB-4`.
      `legacy/ratsdb/queries.json` (raw) + `legacy/ratsdb/queries.md` (categorized,
      cross-cutting findings called out).
  - [x] **REG-1.1** — `Rank_Query` matches `DB-5.2`'s rewrite exactly (same
        `<=`-count, worse-of-tie formula, just parameterized differently) —
        one canonical ranking algorithm across the whole system, no
        divergence to reconcile.
  - **Bonus findings** (see `legacy/ratsdb/queries.md` for full detail): solved the
    `DB-5.1` mystery row's origin (a 3-step manual-timing-entry workflow;
    step 3, `Populate_Best_Score_Contestants`, is character-for-character
    identical to `RATS.exe`'s `Name_contestants_Button_Click` query) —
    validates `DB-5.1`'s `EXISTS`-guard fix, since RATSdb has its own
    "unnamed placeholder" cleanup toolkit for exactly this kind of stuck
    row. Also found 3 more Access-only multi-table `UPDATE...JOIN` queries
    needing the same rewrite treatment when `REG-3` gets to implementation.
- [x] **REG-2** — Read the remaining 27 pages of the user guide and mapped
      every RATSdb workflow against the query catalog — `legacy/ratsdb/workflow_mapping.md`.
      Scope correction: pages 25–36 turned out to be "Timing Supervisor —
      RATSts" content (`RATS.exe`/`APP-*` territory, not RATSdb — folded
      into `APP-1`'s behavior inventory instead, see below). Findings:
  - 7 workflows confidently matched to specific queries (List Contests,
    List Entries, Entry Form + its lookups, All Runs, the "Make Best Time"
    manual-entry chain).
  - **Two distinct manual-entry paths exist, only one backed by a saved
    query**: "Make Best Time" (the known `Insert_Manual_Best_Time` chain)
    vs. "Save Entry Time" (records a slower run, leaves the entry open, no
    matching query at all — almost certainly a plain Access bound-form
    save with no custom query object; means `REG-3` just needs a simple
    "insert one `Entry_Run` row" function here, not a mystery to solve).
  - **Export Entries/Results resolved**: `Extended_Entries_Extract_for_current_Event`
    (its plain-named sibling extracted as literally `SELECT;` — empty, not
    real) and `Used_Entries_Contest_Name_Scores_Current_Event` (the
    outermost layer of a 3-query pipeline, not 3 competing candidates).
  - **Export Markdown confirmed VBA-only** — no query references markdown/
    text formatting anywhere; `REG-3` designs this output format fresh,
    can't port it. One inference clue: `RATS.exe`'s own results display
    uses a plain `Robot | Run Time | Score Time | Contestant` table.
  - **Two sequencing paths likely exist**: manual grid-edit (documented,
    no query needed) vs. bulk `ETL_seeding_upload` import
    (`Add_Sequence_No_To_Entries`, not documented in the guide at all).
  - `Entry.Registration_Source` enum confirmed: `OnLineForm`/`Venue`/
    `UKMARS`/`Other`.
  - **Good news**: most registration CRUD (New Robot/Contestant/Contest/
    Challenge) is plain bound-form inserts, no saved query involved —
    simpler than feared. Full detail: `legacy/ratsdb/workflow_mapping.md`.
- [ ] **REG-3** — **Design the RATSdb port** now that `REG-1`/`REG-2` give a
      full picture. Not broken into atomic stages yet — do that the same
      way `APP-1` was, informed by `legacy/ratsdb/queries.md` +
      `legacy/ratsdb/workflow_mapping.md` + `legacy/RATS_user_guide_summary.md`
      (an independent workflow summary, good cross-check). Known hard parts
      going in: the manual-timing-entry state machine (two save paths), and
      the Markdown export template (no source to copy from at all).
      **Scope addition**: `management_app` should absorb the Google-Forms
      registration extraction currently done by the standalone
      `legacy/contest-registration-processing.py` script (turns Google Forms
      registration exports into an event-day checklist) — not part of
      either old app, but intended to become a feature of the new one.

## Repository cleanup for public release (before `APP-1` coding starts)

User decision (2026-09-17): before starting `APP-1` implementation, clean up
the folder/file structure into something coherent enough for a public
GitHub repo. Central to this: real competition data (containing real
emails and other PII) must never enter git history at all — resolves `Q-2`
more cleanly than in-place sanitization would have.

- [x] **REPO-1** — Moved all real-data files into a git-ignored
      `live_data/` folder (`.gitignore` added) — never committed, never
      touches GitHub: `live_data/RATS_Competitions_be.accdb`,
      `live_data/rats.db`, `live_data/exports/*.csv`. Left in place (at the
      time — since relocated again under `legacy/` by the later
      `legacy`/`contest_app`/`management_app` reorg, see below):
      `database/exports/schema.sql` (structure only, no real data, now
      `legacy/database/exports/schema.sql`),
      `legacy/ratsdb/RATS_DB_Front_end.accdb` (confirmed empty tables — no
      real data of its own, kept as the only surviving copy of RATSdb's
      actual application, same reasoning as keeping
      `legacy/rats_exe/RATS_4p0p1a.exe`). Removed
      `database/blank-RATS_Competitions_be-template.accdb` entirely —
      superseded by `REPO-2`'s "cleaned-up real data" demo DB plan,
      redundant with `exports/schema.sql`. Also cleaned up 2 stray
      duplicates of `query_sql_dump.py` (identical to the canonical
      `tools/access_export/query_sql_dump.py`, one at repo root and one in
      the old `database/` folder). Updated
      `legacy/database/build_sqlite.py` and
      `contest_app/tests/check_db5_rewrites.py` to point at the new
      `live_data/` paths — reran both, still working (`build_sqlite.py`
      rebuilds cleanly, all 7 `DB-5` tests still pass).
      The mechanism for pointing the app at real data (an explicit,
      user-chosen file path — see `REPO-3`) works with any file location,
      so the git-ignored folder is just a convenient default, not an
      architectural requirement.
- [x] **REPO-2** — Built `sample_data/demo.db`: a full sanitized copy of
      `live_data/rats.db` (not a synthetic or trimmed-down excerpt — avoids
      fabricating unrealistic values/relationships in a schema this
      complicated). `sample_data/build_demo_db.py`.
      **Policy revised 2026-09-18** — `Class` turned out to need keeping:
      it's used as a filter/key in several places (report splits, RATSdb
      queries) and effectively needs to be `NOT NULL` in practice; nulling
      it broke more than it protected, so `Contestant.Class` and
      `Best_Score_Time.Contestant_Class` are now left untouched, real
      values, matching `live_data` exactly. `Contestant_Email_Address` is
      handled differently too: rather than blanking every real email,
      contestants who had one get a **fabricated-but-plausible** replacement
      built from their real name + a random classic Microsoft demo domain
      (`contoso.com`, `fabrikam.com`, `northwindtraders.com`, etc., 10-domain
      list, fixed random seed for reproducible output) — e.g. `Ian
      Butterworth` → `ian.butterworth@fabrikam.com`. Contestants who never
      had a real email stay `NULL` — nothing invented where there was
      nothing. This keeps email fields realistically non-empty for UI/form
      testing without leaking anyone's real address. Verified: row counts
      identical to `live_data/rats.db` for every table; exactly 103 real
      email rows → 103 fake email rows (1:1, confirmed by count not just
      presence), zero overlap with any real address; `Class` fields
      byte-identical to `live_data` in both tables; every other specific row
      existing analysis/tests depend on untouched (Competition 39 tie case,
      the `Best_Score_Time.ID=781` placeholder row, `Further_Information`
      free text, contestant/robot names).
- [ ] **REPO-3** — **DB file selection, not a hardcoded path.** Replaces
      `Public_Variables.connString`'s hardcoded single path with a real
      mechanism: the app opens `sample_data/demo.db` by default, with a
      "File → Open Database…" action (or startup prompt) to point at the
      real file instead (wherever it lives — the `REPO-1` local folder, a
      USB drive, anywhere). Remember the last-opened path in a small local
      (git-ignored) config file. This is a cleaner implementation of `Q-4`'s
      Stand Alone Mode than the legacy app's "disable half the buttons"
      approach — no DB opened yet just means "still on the demo/practice
      database," not a degraded state. **Changes scope for `APP-1.4`** (DB
      layer must accept an arbitrary path, not a hardcoded connection
      string) **and `APP-1.8`** (main window needs the open/select-database
      UX) — see those entries.
- [x] **REPO-4** — Full top-level reorg, organized by app rather than by
      kind of material (source vs. spec vs. analysis), since that mix was
      the actual source of incoherence (`ratsdb/` bundled everything about
      one app together while `RATS.exe`'s equivalent material was scattered
      across `legacy/`/`protocol/`/`plans/`). Final shape:
      - **`legacy/`** — everything about the old system, one unit,
        deletable wholesale once the port is trusted: `legacy/database/`
        (shared backend schema/ERD — doesn't change), `legacy/rats_exe/`
        (was top-level `legacy/` + `protocol/` +
        `plans/app-1-behavior-inventory.md`, since that's pure legacy
        analysis not forward design), `legacy/ratsdb/` (unchanged contents,
        just relocated, no separate "reference" folder — it was already
        one coherent unit).
      - **`contest_app/`** — the timing-app Python port: `src/rats/` (was
        top-level `src/rats/`), `tests/` (was top-level `tests/`).
      - **`management_app/`** — RATSdb-replacement Python port skeleton
        (`src/`, `tests/`), not started (`REG-*`).
      - **`live_data/`** — renamed from `local_data/` (user: "expresses
        purpose rather than location").
      - **`sample_data/`** — new, parallel to `live_data/` (one
        git-ignored/real, one committed/safe) for `REPO-2`'s demo DB.
      - `plans/` stays for forward-looking design only (`ARCH-1`, `PLAT-1`,
        `DB-5`'s rewrite spec) now that legacy analysis moved out.
      Updated every path reference across `CLAUDE.md`, this file,
      `porting-issues.md`, `plans/*.md`, `legacy/ratsdb/README.md`, and both
      Python scripts (`legacy/database/build_sqlite.py`,
      `contest_app/tests/check_db5_rewrites.py`) — reran both, still
      working. Also recreated `legacy/database/queries.json` (`DB-4`'s
      empty-`{}` result), which had gone missing from disk at some point
      before this reorg — content was already fully known, so recreated
      rather than left as a dangling reference.

## Open questions for the user

- ~~**Q-1**~~ — **Resolved:** the 3 live-timer layout variants collapse into
  one parametrized window; `single_channel_results` stays separate (see
  `UI-1`).
- ~~**Q-2**~~ — **Resolved:** real data never enters the repo at all —
  cleaner than in-place sanitization (see `REPO-1`/`REPO-2`/`REPO-3`). For
  the demo DB (`REPO-2`), only `Contestant_Email_Address` needs handling —
  replaced with a fabricated-but-plausible address per contestant, not
  blanked, so forms have realistic non-empty data to test against. `Class`
  turned out to need keeping real (used as a filter/key in several places);
  everything else (names, robot names, dates, locations,
  `Further_Information`) is explicitly fine per user policy — robot/
  contestant names and event context are already effectively public via
  published contest results.
- ~~**Q-3**~~ — **Resolved:** only single-channel timers are in use, so the
  Python port stays scoped to what the legacy app actually does (19 inbound
  + 2 outbound codes, no `C1`/`C2` or `96`/`97` handshake — see `SER-1`).
- ~~**Q-4**~~ — **Resolved:** yes, Stand Alone Mode (degraded offline
  operation) is a real requirement — needed for practice sessions without a
  database. See `APP-1.8`.
- ~~**Q-5**~~ — **Resolved, two parts:** (1) support 4:3 and 16:9 as the
  initial target aspect ratios, fully resizable within each, more added
  later if needed — the legacy windows were designed for fixed resolutions,
  not real aspect-ratio flexibility; (2) normalize field content across all
  layouts (the `16_9`-is-a-hybrid quirk was accidental, not worth
  preserving). See `APP-1.11`.
- ~~**Q-7**~~ — **Resolved, user confirmation:** touches are simply
  true/false — confirms `ScoringConfig`'s plain-truthy treatment
  (`contest_app/src/rats/state.py`) over a literal `== -1` port of the
  decompiled comparison. User's recollection: an entry that gets touched
  *can* (but "probably" doesn't in practice) carry a score penalty. Found
  during `APP-1.7`: the real backend data stores `Touches_Enabled`/
  `Touches_Cumulative`/`Touches_Per_Run` as `1`/`0` (checked every
  `Scoring_Model` row in `sample_data/demo.db`), never literal `-1` as the
  C# comparison would require — whether that's a long-standing dead-code
  bug in the legacy app or an Access/OLEDB runtime quirk invisible to the
  exported data is now moot, since the port's own truthy semantics are
  confirmed correct independent of that question.
  **New, separate item raised alongside this** (not a resolution of Q-7,
  a fact for the record): the user believes the *intended* rule is that a
  touched entry should never be able to out-rank an untouched entry with
  the same `Score_Time_mS` — but "probably not in the code" (i.e. neither
  the legacy app nor this port currently enforces it; `db.rank_of()`'s
  `<=` tie semantics, `plans/db-5-sql-rewrites.md`, don't consider
  touches at all). Not implementing this now — no ticket, since it was
  raised as background/context, not a change request — but worth
  remembering if ranking/tie-breaking ever comes up again (`REG-*` phase,
  or a future `APP-1.7` revisit).
- ~~**Q-6**~~ — **Resolved, user correction during `APP-1.5`:** the real
  app's line-handling is stricter than the decompiled `parseData()` loop
  taken literally suggests. User's recollection, treated as a firm
  requirement regardless of whether it was originally deliberate or
  incidental: (1) a line whose first character isn't `<` is **totally
  ignored**, not scanned for an embedded frame elsewhere in the line; (2)
  everything after the closing `>` on a line is ignored — at most one
  message per line; (3) **every** input line must remain available to log
  as a permanent record, whether or not it parses to a message. Changed
  `contest_app/src/rats/serial_protocol.py` from continuous
  bracket-hunting across an arbitrary buffer (which could recover a valid
  frame following leading garbage on the same line — no longer allowed) to
  splitting on `\n` first, then parsing each isolated line; added `Line`
  (raw text + parsed `Message` or `None`) so a later logging layer
  (`APP-1.6`/`APP-1.8`) can log every line without this pure-parsing layer
  dropping any of them. See `APP-1.5`.
