# APP-1 behavior inventory

Full read-through of all previously-uninvestigated code in the timing app's
window files, closing the gap flagged before `APP-1` implementation starts.
Coverage: `Form1.cs` (3441 lines, now fully read), `Calibration_Form.cs`
(439), `run_order.cs` (219), `single_channel_display.cs` (844),
`single_ch_display_v2.cs` (1048), `single_ch_display_16_9.cs` (910),
`single_channel_results.cs` (290) — all fully read (designer/layout
boilerplate skimmed for concrete values only, not line-by-line).

## `APP-1.7` scope must widen — these are core scoring paths, not edge cases

`APP-1.7`'s current success criterion (a scripted `2→3→4→5` message sequence)
only covers the *automatic*, serial-driven run-completion path. Two
manually-triggered UI paths feed the same scoring state and are currently
untested:

- **`Touch_Button_Click`** (`Form1.cs:2515`) — increments
  `Public_Variables.no_of_touches`. This directly feeds `run_time_ms_message`'s
  score formula (`Scoring_Model.Touches_Enabled`/`Touches_Cumulative`/
  `Touch_Time_mS`, already in `DB-5`'s scope) — a run with touches produces a
  different `Score_Time_mS` than one without, and this is the *only* way
  `no_of_touches` changes. `APP-1.7`'s core needs a "touch" action alongside
  the message-driven ones, and its integration test should cover at least
  one run with touches enabled.
- **`DNF_Button_Click`** (`Form1.cs:2553`) — a whole separate outcome path:
  guarded by `practice_mode` (no-ops if practice mode is on), shows a
  Yes/No confirmation dialog ("Do you want to terminate this Robot's
  entry?"), and on confirmation runs
  `UPDATE Entry SET Entry_Used = TRUE, Outcome = "Retired" WHERE Entry_ID = ?`
  then calls `clear_timer()` (see below) and refreshes the pending-entries
  list. No `Best_Score_Time`/`Entry_Run` row is written for a DNF — it's
  purely an `Entry` state change. Needs its own test case in `APP-1.7`'s
  scope: confirm no scoring row gets written, `Entry.Outcome` becomes
  `"Retired"`, and the entry drops out of the pending list.
- **`clear_timer()`** (`Form1.cs:2524`, called by `DNF_Button_Click`,
  `Clear_Button_Click`, and indirectly via `New_Mouse`/`Practice_Mode`
  handlers) — resets *all* run-state display fields to zero/blank in one
  place: `split_time_ms`, `maze_time_ms`, `run_time_ms`, `no_of_touches`,
  `score_time_ms`, `fastest_score_time_this_robot` (`-1`),
  `time_left_ms` (reset to `entry_time_limit_s * 1000`), `no_of_runs_used`
  (`0`), `robot_rank` (`0`), plus clears both timer-running flags and sets
  `hide_robot_runtimes = true`. This is effectively "start a fresh entry"
  and belongs in the state object's design as a single explicit method, not
  scattered field resets.

**Also affects scoring config, not yet in `APP-1.4`'s scope**:
`Competition_DataGridview_SelectionChanged` (`Form1.cs:2261`) is where
`touches_enabled`, `touches_cumulative`, `touch_time_ms`, `entry_time_divider`,
`touch_time_divider`, `touches_per_run`, `entry_time_limit_s`,
`no_of_runs_allowed`, and `grace_period_ms` actually get loaded — from a
`Scoring_Model` lookup keyed on the selected competition's
`Scoring_Model_Short_Name`, plus columns from the `Competition` row itself.
This is the query that populates every one of the scoring-formula inputs
`DB-5`/`APP-1.7` depend on; it belongs in `APP-1.4`'s DB layer alongside the
`Entry_Run`/`Best_Score_Time` operations already scoped there.

## Serial connection setup (`APP-1.6`) — baud rate is hardcoded here

`connect_BTN_Click` (`Form1.cs:2400`) is where the serial connection is
actually configured: `BaudRate = 9600` (hardcoded — this is the literal line
that needs to become `115200` per the port's stated goal), `DataBits = 8`,
`Parity = None`, `StopBits = One`, `Handshake = None`,
`ReadTimeout = 10000`ms. Toggles button text `Connect`/`Dis-connect`,
disables the port-selection combo box while connected, and only enables
`Timer1` (the polling loop, superseded by `ARCH-1`'s reader thread) once
`SerialPort1.IsOpen` is confirmed true. Also: `NewMouse` (`<98,0>`) is sent
not just from a dedicated button but from **three separate places** —
`Mouse_DataGridview_SelectionChanged` (on selecting an entry),
`Practice_Mode_Button_Click` (on toggling practice mode), and
`New_Mouse_Button_Click` (explicit button, with a confirmation dialog) —
all three do the identical `SerialPort1.WriteLine("<98,0>")` +
`message.WriteLine(...)` + `clear_timer()` sequence. Worth a single shared
"start new entry" function in the port rather than three copies.

## Form1 — additional behavior not yet covered

- **`Form1_Load`**: sets `robot = "Practice Mode"` initially, populates the
  COM-port dropdown from `Ports.SerialPortNames`, opens the main DB
  connection and reads the `Context` singleton row (`Effective_Date`,
  `Current_Event`, `Current_Event_ID`) to know which event is active — if
  that connection/query fails, falls back to **"Stand Alone Mode"**
  (`database_available = false`) and disables `Practice_Mode`, `Run_order`,
  both `Display_*`, `Name_contestants`, and both `Results_*` buttons. This
  offline/stand-alone mode is a real, distinct operating mode not
  previously tracked — worth an explicit decision on whether the Python
  port supports it or requires the DB to always be reachable. Also opens
  two timestamped log files on the Desktop
  (`RATS_message_log<timestamp>.txt`, `RATS_verbatim_log<timestamp>.txt`) —
  matches the "all traffic logged to a message log" behavior in `CLAUDE.md`,
  now with the exact filename pattern and location.
- **`Form1_Close`**: just closes the two log files.
- **`comPort_ComboBox_SelectedIndexChanged`**: stores the selected port name
  in a local field; no other effect until `Connect` is clicked.
- **`Competition_Class_ComboBox_SelectedIndexChanged`**: (guarded by "not
  practice mode") queries `Competition` filtered by event + class
  (`Competition_Class = 'Heats'`/`'Final'` etc.), populates the competition
  grid, ordered by `Challenge`.
- **`Competition_DataGridview_SelectionChanged`**: see scoring-config note
  above; also resets `no_of_runs_used = 0` on every competition selection.
- **`Mouse_DataGridview_SelectionChanged`**: guarded by "not practice mode"
  and exactly one competition selected; requires `SerialPort1.IsOpen`
  (shows `"Connect a COM port first"` otherwise); looks up
  `Contestant_Name`/`Class` via `Mouse`→`Contestant` join, sends `NewMouse`,
  calls `clear_timer()`. On any exception (e.g. DB hiccup), falls back to
  blanking `robot`/`contestant` to `"_"` and zeroing rank — same placeholder
  convention as the `DB-5.1`/`REG-1` "_"-marker rows.
- **`Practice_Mode_Button_Click`**: toggles `practice_mode`; entering
  practice mode sets `contestant = "_"` and `hide_best_score_times = true`;
  requires an open serial port (same "Connect a COM port first" guard).
- **`clear_BTN_Click`**: clears the raw-message monitor `RichTextBox1` and
  resets a message counter — pure UI convenience, no state effect.
- **`Monitor_Button_Click`** / **`Verbose_Button_Click`**: toggle
  `monitor_input`/`verbose_monitor` flags that control how much of the raw
  serial traffic gets echoed into `RichTextBox1` (see `parseData()`'s
  existing conditional logging, already known). Button text toggles
  Monitor/NoMonitor and Verbose/Concise.
- **`New_Mouse_Button_Click`**: confirmation dialog ("Do you want to end
  this Robot's entry?"), then `NewMouse` + `clear_timer()`; **also contains
  a WinForms-specific hack**: it jumps `Competition_DataGridView.CurrentCell`
  to an adjacent row and back, purely to force the grid's
  `SelectionChanged` event to re-fire and refresh the pending-entries list.
  This has no meaning in Tkinter — the port should just call the refresh
  function directly, no equivalent hack needed.
- **`Calibrate_Button_Click`**: opens `Calibration_Form` if not already
  open (else requests it close) — requires an open serial port.
- **`Run_order_Button_Click`**: opens the `run_order` window (same
  open/close toggle pattern as the display windows, per `UI-1`).
- **`ExtraRunButton_Click`**: decrements `no_of_runs_used` (only if `> 0`) —
  an admin override to grant an extra run without touching
  `no_of_runs_allowed`.
- **`WatchDogButton_Click`**: toggles `watchdog_active`; resets
  `watchdog_alarm`/`watchdog_ms_since_reset` either way; button text
  toggles "WatchDog is On"/"WatchDog is Off".
- **Watchdog alarm sound** (`Timer1_Tick`, `Form1.cs:2784`): when
  `watchdog_alarm` is set and the serial port is open, increments
  `watchdog_alarm_repeat_counter` every tick (10ms) and — once it passes
  `200` (i.e. **roughly every 2 seconds** at the original 10ms tick rate) —
  plays `notify.wav`, sets the watchdog label to red `"Error"`, and resets
  the counter to repeat. This is the exact counter flagged as an
  `object`-typed decompile-fidelity risk in `porting-issues.md` barrier 6;
  now fully understood — it's a simple modulo-repeat alarm, no hidden
  complexity, safe to reimplement directly as an integer counter.
- **`Selected_Robot_Click`**, **`Competition_DataGridView_CellContentClick`**:
  both empty handlers (designer-wired, no-ops). Not needed in the port.

## `Calibration_Form.cs` — fully confirmed, no surprises

A static 3×3 grid (Start Cell / Start Gate / Finish Gate rows × Pot Value /
Brightness Value / Trigger columns), driven entirely by `Public_Variables`
fields Form1 already populates from protocol codes `71`–`73`/`81`–`86` (no
calculation of its own). On load: sets `calibration_window_open = true`,
`request_calibration_mode = true` (which `Form1`'s tick loop picks up and
sends `<99,CALIBRATION>`), starts its own `Timer2` (interval not present in
the visible designer code — WinForms default, effectively fast/cosmetic
polling of already-computed fields). On close: reverts to
`request_timer_mode = true` (sends `<99,TIMER>`). Confirms `SER-1`'s
protocol documentation exactly — no new codes, no new logic.

## `run_order.cs` — fully confirmed

Opens its **own** separate `OleDbConnection` (a third connection to the
backend, alongside `Form1`'s and each display window's own). Polls on a
timer, but throttles actual DB refresh to every 20 ticks via a manual
counter (`refresh_run_order_delay`), not the timer interval itself. Query:
```sql
SELECT Mouse_Name AS Robot FROM [Entry]
WHERE (Competition_ID = ? AND Entry_Used = FALSE)
ORDER BY Sequence_Number
```
— identical to `Form1.cs`'s `Display_pending_entries()` query. Single
unlabeled "Robot" column, no grid headers shown, title label "Run Order",
no close/minimize/maximize window chrome (`ControlBox = false` — closed only
via `Form1`'s toggle button setting `request_run_order_window_close`).
**`APP-1.10`'s success criterion should test this exact query**, not a
generic "ordering" claim.

## Display windows — concrete parametrization differences (`UI-1`)

All three layout variants share the same overall structure (`Load`/`Close`/
`Display_Timer_Tick`/`refresh_best_scores`/`refresh_entry_runs`, each with
their own separate DB connection) but differ in real, non-cosmetic ways —
not just aspect ratio:

| | `single_channel_display` (base) | `single_ch_display_v2` | `single_ch_display_16_9` |
|---|---|---|---|
| Window size | 596×556 | 784×561 | 1264×681 |
| Visual theme | Light (`SystemColors.Window` backgrounds, Black/Orange/Red text, `Microsoft Sans Serif`) | Dark "LED tile" look: key stat labels (`available_runs`, `time_left`, `run_number`, `Best_Score`, `Touches`) have `BackColor = DarkSlateBlue` with Yellow/LimeGreen/Red text; `Arial Black` bold labels + huge `Courier New` numerals (e.g. `Best_Score`/`run_number`/`time_left` all 48pt) | Same dark "LED tile" theme as v2, sizes scaled up further (e.g. `Split_Time` is **148pt** `Courier New`) |
| Time display format | Plain decimal seconds, `"F2"`/`"F3"` (e.g. `12.34`) | `TimeSpan` `m:ss.fff` (e.g. `1:23.456`) for `Split_Time`/`Best_Score`; `m:ss` for `time_left` | Same as v2 (confirmed identical logic) |
| `Maze_Time` field | Present, visible, `"F2"` format, 20pt font | Present in designer **but `Visible = false`** — still updated in code (`"000.000"` format) but never shown to the user | **Field doesn't exist at all** — removed from the designer, and the code line updating it is simply absent (not hidden, genuinely gone) |
| Embedded running-order grid | **None** | Present — same query as `run_order.cs`, refreshed alongside `refresh_best_scores()` every 50 ticks | Present, identical to v2 |
| Contestant name / rank display | **None** (`Contestant_Label`/`Rank_label` fields don't exist) | Present: `Contestant_Label`, `Rank_label`, `Rank_label_lbl` | Present, identical to v2 |
| Event name label | **None** | `Event_Name_lbl`, set once on load from `Public_Variables.robotics_event` | Present, identical to v2 |
| `refresh_entry_runs` query | Includes `Time_of_Run`, headed "Time of Run"/"Run"/"Score", **ordered `DESC` by `Time_of_Run`** (most recent first) | Drops `Time_of_Run` entirely — just "Run"/"Touch"/"Score", **no `ORDER BY` at all** (relies on whatever order the DB returns, effectively insertion order) | **Uses the base's version** (with `Time_of_Run`, ordered `DESC`) — not v2's stripped version, despite sharing v2's other additions |
| Practice-mode grid visibility | Grids hidden only via separate `hide_robot_runtimes`/`hide_best_score_times` flags (not handled inline in the practice-mode branch) | Explicitly hides `Run_order_DataGridView`/`Best_Scores_DataGridView`/`Entry_Runs_DataGridView` inline when `practice_mode` is true | Same as v2 |

**Implication for the parametrized class design** (`APP-1.11`): this isn't
just "pick a size/font-scale parameter." The real axes of variation are:
(1) visual theme (light/plain vs. dark/LED-tile), (2) time format (decimal
seconds vs. `m:ss.fff`), (3) which optional fields exist at all
(`Maze_Time`, contestant name, rank, event name, embedded running-order
grid), and (4) the entry-runs query's exact columns/ordering. `16_9` being a
genuine **hybrid** of v2's additions plus base's entry-runs query (not
simply "v2 resized") means a naive two-axis parametrization (theme × size)
would miss this — worth deciding explicitly whether the Python port
preserves this exact hybrid or normalizes it (a real design choice, not
just a port detail).

## `single_channel_results.cs` — structurally different, confirmed

Not a live/ticking display at all: `Load` does a **one-shot** query
(identical to the base display's `refresh_best_scores` query, headed
"Robot"/"Run Time"/"Score Time"/"Contestant") and never refreshes it again —
`Display_Timer_Tick` exists solely to poll for the close-request flag,
nothing else. Contains a `Thread.Sleep(30)` blocking call in `Load` (likely
a workaround for an OLEDB driver race condition on connection-open) — no
equivalent needed in the Python port. `APP-1.11`'s success criterion for
this window should be "populates once on open, matches
`Best_Score_Time` for the selected competition ordered by `Score_Time_mS`,"
not a live-update test.

## Confirmed: no other gaps found in this pass

- Only **one** Access-only multi-table `UPDATE...JOIN` exists anywhere in
  the timing app's source (`Name_contestants_Button_Click`, already fully
  covered by `DB-5.1`) — checked all 7 files.
- No new `.wav`/font references beyond what `PLAT-1` already tracks
  (`notify.wav`, `chimes.wav`, `chord.wav`, `tada.wav`; `Arial Black` +
  `Courier New` + default `Microsoft Sans Serif`).
- No new serial protocol codes — `Calibration_Form.cs`'s message handling
  matches `SER-1`'s documented codes exactly.
