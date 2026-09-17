# APP-1.8: main window layout plan

Prep notes written at the end of the planning session before `APP-1.8`
implementation starts (in a new session) — captures what the user asked
for so it isn't lost between sessions: the placeholder-widget strategy,
responsive layout from the start, and the full widget inventory grouped
by sub-stage.

## Three standing requirements for `APP-1.8` (and `.9`–`.12`)

1. **Build the full main-window layout up front, with placeholders for
   everything, not sub-stage by sub-stage.** The user expects to want to
   alter the UI layout as work progresses, and wants to see/react to the
   whole picture early rather than discovering the final shape only once
   `.4` lands. Concretely: `APP-1.8.1` should lay out **every** region and
   widget below (whether or not that widget's sub-stage has landed yet),
   with not-yet-wired widgets rendered disabled/inert (e.g. a `state="disabled"`
   `Label`/`Button`, or a grid with a "not yet implemented" placeholder
   row) rather than simply absent. Each later sub-stage then *activates*
   its widgets (wires them to `AppCore`/the queue) rather than adding new
   layout regions from scratch. This also means layout feedback from the
   user can happen as early as `.1`, before any real functionality exists
   behind most of it.
2. **Every stage with a manual-smoke-test success criterion ships a
   numbered test checklist with expected results**, for the user to
   actually execute against their own hardware (confirmed available: the
   user has manually-operated timing-gate hardware for testing the serial
   interface). This applies to every remaining `APP-1.8.*` sub-stage and
   to `APP-1.9`–`.12` (calibration, run order, displays, results), all of
   which share the same "manual smoke test against real hardware"
   pattern — not something to reintroduce each time, a standing
   convention from here on. The checklist itself isn't written yet (it
   depends on the actual implementation details of each sub-stage) — this
   doc just records the commitment so it isn't dropped.
3. **Responsive layout from the very first pass, not retrofitted later.**
   The `.1` placeholder skeleton itself needs to use Tkinter's resizing
   machinery properly from the start — `grid`/`pack` with weighted
   rows/columns (or a `PanedWindow` where regions should be
   independently resizable), no widget given a fixed pixel size that
   would just clip or leave dead space at other window sizes. Matches
   `Q-5`/`UI-1`'s decision for the display windows (resizable, not a
   fixed-pixel copy of the legacy designer) — the same standard applies
   here to the main window, and it's much cheaper to build resizing in
   from the first placeholder pass than to bolt it on after `.2`–`.4`
   have already assumed a fixed layout.

## Full widget inventory (`Form1.cs`, decompiled `InitializeComponent`)

Grouped by which `APP-1.8` sub-stage activates it. Legacy pixel positions
aren't reproduced here on purpose — `Q-5`/`UI-1` already committed this
port to resizable Tkinter layouts, not a fixed-pixel copy, and the user
expects to iterate on layout anyway.

**`APP-1.8.1` — connection:**
- `comPort_ComboBox` (COM port dropdown)
- `connect_BTN` (Connect/Disconnect toggle)
- **new, not in legacy**: baud-rate choice dropdown, alongside the port
  dropdown (per the user's `APP-1.6`-era note)
- (File → Open Database… / last-path config file has no legacy widget
  equivalent — legacy hardcoded `connString`)

**`APP-1.8.2` — event/competition/entry selection:**
- `Event_Name_lbl`, `Event_Date_lbl`
- `Competition_Class_ComboBox`, `Competition_Class_lbl`
- `Competition_DataGridView`, `Selected_Competition` (label)
- `Mouse_DataGridView`, `Selected_Robot` (label)
- `Current_Contestant_Label`

**`APP-1.8.3` — run control + live display:**
- Buttons: `Touch_Button`, `DNF_Button`, `Clear_Button`,
  `New_Mouse_Button`, `Practice_Mode_Button`, `ExtraRunButton`,
  `WatchDogButton`
- Live values: `Split_Time`/`Split_Time_lbl`, `Maze_Time`/`Maze_Time_lbl`,
  `Run_Time`/`Run_Time_lbl`, `Score_Time`/`Score_Time_lbl`,
  `Best_Score`/`Best_Score_lbl`, `Rank_Label`/`Rank_Label_lbl`,
  `time_left`/`time_remaining_lbl`, `run_number`/`run_number_Lbl`,
  `allowed_runs`/`allowed_runs_Lbl`, `Touches_Label`/`touches_lbl`,
  `watchdog_state_Label`
- Run/gate state: `Timer_State_lbl`/`Timer_State_Label_lbl`,
  `State_Text_Label`
- Scoring-config readout (populated on competition select, `APP-1.4`'s
  `select_scoring_model`): `Touch_Time`, `Touch_Divider`, `Maze_Divider`,
  `Touch_Cumulative`, `Touch_Enabled_Label` + their `_lbl` label pairs

**`APP-1.8.4` — monitor, logs, child-window launch buttons:**
- `RichTextBox1` (raw Tx/Rx monitor), `clear_BTN` (clears it),
  `Monitor_Button`, `Verbose_Button`
- `Name_contestants_Button` (the `DB-5.1` backfill maintenance action —
  one-off DB fixup, not really "run control")
- `Calibrate_Button` → `APP-1.9`
- `Run_order_Button` → `APP-1.10`
- `Display_1_channel_button`, `Display_1ch_v2_button`,
  `Display_1ch_v2wide_button` → `APP-1.11` (parametrized single-channel
  display, `UI-1`)
- `Results_1ch_button` → `APP-1.11` (`single_channel_results`)

**Not carried into the port at all**: `Display_2ch_button`,
`Results_2ch_button`. Checked, not assumed: both are permanently
`Enabled = false` in the legacy `InitializeComponent` with no click
handler anywhere in `Form1.cs` — genuinely vestigial 2-channel-mode
buttons, consistent with `SER-1`/`Q-3`'s finding that this build has no
dual-channel support at all. Not even worth a disabled placeholder.

**Legacy-only, no port equivalent**: `Timer1` (the 10ms polling timer,
superseded by `ARCH-1`'s reader thread + `APP-1.8.1`'s `after()` loop) and
`Timer_LBL` (its on/off indicator label — nothing in the new design turns
polling on/off the same way, so this label has no clear home; revisit if
it turns out to mean something else once `.1`/`.3` are actually built).

## GroupBox layout containers

`GroupBox1`/`GroupBox2`/`GroupBox3` group related controls in the legacy
designer (e.g. `GroupBox3` holds the display/results launch buttons,
confirmed from `Controls.Add` call sites). Not translating these 1:1 into
Tkinter `LabelFrame`s by name — group boundaries should follow whatever
layout the placeholder pass in `.1` actually produces, informed by the
regions above, not a literal copy of 3 legacy boxes.
