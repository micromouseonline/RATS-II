# RATSdb — the registration database front-end

`RATS.exe` (in `legacy/rats_exe/`) is the live *timing* app — it's only used during
an actual run, connects directly to the backend `.accdb`, and has no idea
this front-end exists (confirmed: zero references anywhere in the
decompiled source).

**RATSdb** (`RATS_DB_Front_end.accdb`) is a separate, actively-maintained
MS Access 2016 application — confirmed via
`RATS_User_Guide_for_v3p9p1_db3p3.pdf` (doc version 2.0, May 2024 — newer
than the timing exe). Organizers use it for everything *around* an event:

- Creating a new Robotics Event ("Context" form, stays open for the session
  — this is what the backend's `Context` table tracks)
- Registering robots/contestants and creating competition entries
- Sequencing entries
- Manual score entry/correction
- Reports and exports: Scores Report, Unsplit Scores, All Runs, Export
  Entries/Results/Markdown

It talks to the *same* backend (`live_data/RATS_Competitions_be.accdb`,
presumably via Access linked tables) — same schema we already extracted for
`DB-1`–`DB-5`, no new schema info here (its own Appendix A ERD in the user
guide is the same stale 2018 diagram we already superseded).

## What's here

- `RATS_DB_Front_end.accdb` — the RATSdb application file itself (forms,
  reports, macros — not extractable by `mdbtools`, same limitation noted in
  `tools/access_export/README.md`)
- `RATS_User_Guide_for_v3p9p1_db3p3.pdf` — official user guide, documents
  the actual workflows visually since the forms themselves aren't
  extractable

## What's here now (as of `REG-1`)

- `queries.json` — raw ADOX extraction, all 54 saved queries' SQL text
- `queries.md` — categorized writeup with the cross-cutting findings:
  `Rank_Query` matches `DB-5.2` exactly; the `DB-5.1` mystery placeholder
  row's origin (a 3-step manual-timing-entry workflow, one step of which is
  character-for-character identical to `RATS.exe`'s own fix-up query); an
  "unnamed placeholder" admin cleanup toolkit; 3 more Access-only
  `UPDATE...JOIN` queries needing the `DB-5`-style rewrite treatment; and
  the form-control references (`[Forms]![Context_Form]!Event_ID` etc.) that
  will need to become explicit session/state values in the port.

## Scope

Per user decision (2026-09-17): full replacement of the RATS system is the
real goal, not just the timing app. RATSdb's registration/reporting
functionality is in scope, but as a **second phase, after `APP-1`** (the
timing app port). Tracked under the `REG` group in `../../todo.md`.

## Porting methodology is different from the timing app

For `RATS.exe` (`APP-*`), we have the full decompiled C# source, so the
Python rewrite can be verified against exact original behavior — that's
literally what `DB-5`'s tests do (bit-for-bit parity with known original SQL
logic, including reproducing an intentional-looking tie-ranking quirk
rather than "fixing" it).

**RATSdb has no equivalent ground truth available to us.** Its forms,
reports, macros, and VBA modules are stored in Access's proprietary binary
format — none of our tooling can read them (`mdbtools` handles only tables
reliably and queries unreliably; `query_sql_dump.py`/ADOX gets query SQL
text but has no access to VBA or form designs). Getting the real
implementation would require opening the file in the actual Microsoft
Access application on Windows and reading the VBA source directly (Alt+F11)
or automating its VBE object model — out of scope for now.

So `REG-*` is necessarily a **functionally-equivalent rebuild, not a
mechanical port**: built from what we *can* see — the official user guide's
documented workflows (`RATS_User_Guide_for_v3p9p1_db3p3.pdf`) and the actual
query logic (`REG-1`'s extraction) — rather than from exact original
implementation details. "Correct" for `REG-*` means "matches documented
behavior and produces the same results as the queries," not "byte-identical
to hidden VBA we've never seen."
