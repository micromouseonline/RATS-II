# RATS serial protocol

Transcribed from `preferredMessageSequences.pdf` (same directory, vendor
spec, "Preferred message sequences for communicating with RATS
(Registration And Timing System)") so the message set is diffable and easy
to reference from code. The PDF is authoritative if this drifts from it.

## Cross-check against the decompiled app (`SER-1`, done)

`legacy/rats_exe/RATS/Form1.cs` is the only file that touches `SerialPort1` — all
inbound dispatch is the `switch (text)` in `parseData()` (line 2878), all
outbound writes are 5 call sites (lines 2358, 2471, 2606, 2712, 2719).

**Inbound codes the app actually handles:** `0, 1, 2, 3, 4, 5, 6, 12, 13, 30,
71, 72, 73, 81, 82, 83, 84, 85, 86` — 19 of Annex A's codes, matched exactly
(same meanings, same value scaling) to their Annex A entries.

**Outbound codes the app actually sends:** only `98` (`<98,0>`, `NewMouse` —
sent on entry/robot selection, 3 call sites) and `99` (`<99,CALIBRATION>` /
`<99,TIMER>`, `SetMode` — calibration mode toggle). Matches Annex A exactly.

**In Annex A but never referenced anywhere in the app** (not sent, not
handled on receipt):
- **All Channel 1/2 codes**: `11, 14–19, 21–29` (`C1Reaction`, `C1STrigger`,
  `C1FTrigger`, `C1CTrigger`, `C1SGLevel`, `C1FGLevel`, `C1SCLevel`, and the
  whole `C2*` set). This decompiled build has **no dual-channel timer
  support** — it only understands the legacy/single-channel message set
  (`0–6`, `12`, `13`, `30`, `71–73`, `81–86`). `C1SplitTime` (`12`) and
  `C1RunTime` (`13`) *are* handled, but only because they alias the
  single-channel `SplitTime`/`RunTime` semantics — the app doesn't
  distinguish channel 1 from channel 2 at all.
- **`96` (`TimerType`)** — never received/handled. The app never asks the
  timer to identify itself.
- **`97` (`RequestType`)** — never sent. Consistent with `96` being unhandled
  — the handshake for detecting a 1-channel vs. 2-channel timer isn't
  implemented in this build at all.

**Porting implication — resolved (`../todo.md` `Q-3`):** only single-channel
timers are in use, so the Python port is scoped to exactly the 19 inbound +
2 outbound codes above. `C1`/`C2` channel-prefixed codes and the `96`/`97`
timer-type handshake are out of scope — no need to implement them.

## Connection

USB serial: 9600 baud, 8 data bits, no parity, 1 stop bit.

## Message format

All messages: `<message_type,value>` followed by CRLF, where `message_type`
and `value` are both integers.

## Precedence

Values from attached timing gates always take precedence over values RATS
interpolates for running displays. The run time recorded for a robot run is
always the value the gates provide. Likewise, if the gates supply an updated
course time (time used from the allowed entry time), it overrides any
internally calculated value in RATS.

## Recognized timing gate states

Used by the `TimerState` message (type `4`) and the state machines below.

| State No | Name | Mandatory to report | Comments |
|---|---|---|---|
| 0 | Calibrating | | |
| 1 | Waiting for robot in start position | | |
| 2 | Robot in start position | Yes | Reporting this state clears the split time and resets the touch count for a run |
| 3 | Started | | |
| 4 | Running | Yes | Reporting this state increments the run count for the robot and checks it against the number of runs allowed |
| 5 | Finishing | | |

## Messages: RATS PC → timing gates

Only one message flows this direction: **NewMouse**. On receipt, the timing
gates reset themselves to a state ready for a new robot entry.

## Messages: timing gates → RATS (preferred usage)

These five are the ones actually described with usage notes in the spec (the
full type list is Annex A below).

| type | Name | Description | Usage |
|---|---|---|---|
| 30 | **CourseTimeMs** | Length of time in ms the current mouse has been active in the maze | Tells RATS to start timing the entry and apply the entry-time-limit constraint for the contest. **Must be sent with value 0 at the start of a robot entry.** Updates after that aren't required, but if sent they override RATS's own calculated value. |
| 12 | **C1SplitTime** | Channel 1: length of time in ms for the current mouse on its current run | Tells RATS to start reporting the timing of a robot run. Informational only while running — not recorded as a run time. **Must be sent with value 0 when the robot triggers the start gate.** Updates after that aren't required, but override RATS's calculated value if sent. |
| 13 | **C1RunTime** | Channel 1: length of time in ms for a run that has just completed | Tells RATS the definitive run time to store and use for scoring. **Must be sent when the robot triggers the finish gate.** By convention repeated 3 times to mitigate line errors — RATS records only the first and discards the duplicates. |
| 4 | **TimerState** | Timer state reported from the gates | Reports the state number (see table above). **States 2 and 4 must be reported**; others are optional. |
| 0 | **WatchDog** | Resets the watchdog timer in RATS | Gates should send this with value 0 **at least every second**. If more than 2 seconds elapses between WatchDog messages, RATS raises an error to alert judges. RATS can suppress this check for older legacy gates. |

## State machines

Three reference configurations, depending on the number of physical gates.
All three share states `0` (Calibrating), `9` (New Entry Requested), `2`
(Robot at Start), `3` (Started), `4` (Running), `5` (Finishing) — they differ
in which physical gate trigger drives which transition, and whether state
`1` (Waiting Robot) is used.

### 3-gate maze (start gate + finish gate + mouse-in-start-cell gate)

| State | Name | Predecessors | Entry condition | Actions | Successors | Exit condition |
|---|---|---|---|---|---|---|
| 0 | Calibrating | Initial state | Power On | Wait `NewMouse` from RATS | 9 New Entry | `NewMouse` received |
| 1 | Waiting Robot | 9 New Entry, 5 Finishing | Immediate / Finish gate cleared | Send `<TimerState,1>`; wait for robot present at start | 2 Robot at Start | Robot at start triggered |
| 2 | Robot at Start | 1 Waiting Robot, 4 Running | Robot at start triggered | Send `<TimerState,2>`; wait for start gate triggered | 3 Started | Start gate triggered |
| 3 | Started | 2 Robot at Start | Start gate triggered | Send `<TimerState,3>`; send `<C1SplitTime,0>`; wait for start gate cleared | 4 Running | Start gate cleared |
| 4 | Running | 3 Started | Start gate cleared | Send `<TimerState,4>`; wait for finish gate triggered or robot present-in-start triggered | 5 Finishing, 2 Robot at Start | Finish gate triggered / robot at start triggered |
| 5 | Finishing | 4 Running | Finish gate triggered | Send `<TimerState,5>`; send `<C1RunTime,value>` ×3; wait for finish gate cleared | 1 Waiting Robot | Finish gate cleared |
| 9 | New Entry | 0, 1, 2, 3, 4, 5 (any) | `NewMouse` | Initialise gates and gate timer; send `<CourseTimeMs,0>` | 1 Waiting Robot | Immediate, after actions |

### 2-gate line follower (start/finish gate + mouse-in-start-cell gate)

State `1` (Waiting Robot) is unused — entry goes straight to state `2`.

| State | Name | Predecessors | Entry condition | Actions | Successors | Exit condition |
|---|---|---|---|---|---|---|
| 0 | Calibrating | Initial state | Power On | Wait `NewMouse` from RATS | 9 New Entry | `NewMouse` received |
| 1 | Waiting Robot | — | — | State not used | — | — |
| 2 | Robot at Start | 9 New Entry, 5 Finishing | Immediate / Finish gate cleared | Send `<TimerState,2>`; wait for start gate triggered | 3 Started | Start gate triggered |
| 3 | Started | 2 Robot at Start, 4 Running | Start gate triggered | Send `<TimerState,3>`; send `<C1SplitTime,0>`; wait for start gate cleared | 4 Running | Start gate cleared |
| 4 | Running | 3 Started | Start gate cleared | Send `<TimerState,4>`; wait for finish gate triggered or start gate triggered | 5 Finishing, 3 Started | Finish gate triggered / start gate triggered |
| 5 | Finishing | 4 Running | Finish gate triggered | Send `<TimerState,5>`; send `<C1RunTime,value>` ×3; wait for finish gate cleared | 2 Robot at Start | Finish gate cleared |
| 9 | New Entry | 0, 2, 3, 4, 5 (any) | `NewMouse` | Initialise gates and gate timer; send `<CourseTimeMs,0>` | 2 Robot at Start | Immediate, after actions |

### 1-gate line follower (single combined start/finish gate)

Start and finish are the same physical gate here, so "Start/Finish gate
triggered/cleared" drives every transition.

| State | Name | Predecessors | Entry condition | Actions | Successors | Exit condition |
|---|---|---|---|---|---|---|
| 0 | Calibrating | Initial state | Power On | Wait `NewMouse` from RATS | 9 New Entry | `NewMouse` received |
| 1 | Waiting Robot | — | — | State not used | — | — |
| 2 | Robot at Start | 9 New Entry | Immediate | Send `<TimerState,2>`; wait for start gate triggered | 3 Started | Start/Finish gate triggered |
| 3 | Started | 2 Robot at Start | Start/Finish gate triggered | Send `<TimerState,3>`; send `<C1SplitTime,0>`; wait for start gate cleared | 4 Running | Start/Finish gate cleared |
| 4 | Running | 3 Started | Start/Finish gate cleared | Send `<TimerState,4>`; wait for finish gate triggered or start gate triggered | 5 Finishing | Start/Finish gate triggered |
| 5 | Finishing | 4 Running | Finish gate triggered | Send `<TimerState,5>`; send `<C1RunTime,value>` ×3 | 2 Robot at Start | Immediate, after actions |
| 9 | New Entry | 0, 2, 3, 4, 5 (any) | `NewMouse` | Initialise gates and gate timer; send `<CourseTimeMs,0>` | 2 Robot at Start | Immediate, after actions |

## Annex A — all message types

| code | Name | Direction | Tx frequency | Comments |
|---|---|---|---|---|
| 0 | WatchDog | Arduino → PC | Event Driven | Resets the watchdog timer in RATS |
| 1 | CourseTime | Arduino → PC | 1T or 100msec | Length of time in 100ms units the current mouse has been active in the maze |
| 2 | SplitTime | Arduino → PC | 1T or 10msec | *Legacy compat.* Length of time in 10ms units for the current mouse's current run (display only) |
| 3 | RunTime | Arduino → PC | Event Driven | *Legacy compat.* Length of time in 10ms units for a run just completed (definitive, used for score) |
| 4 | TimerState | Arduino → PC | Event Driven | Timer state reported from Arduino (valid values: any integer — see state table above) |
| 5 | RunTimeMs | Arduino → PC | Event Driven | Length of time in 1ms units for a run just completed (definitive, used for score) |
| 6 | SplitToRun | Arduino → PC | Event Driven | Copy the split time to the run time so RATS does all timing from received messages (value must be 0 or no action is taken) |
| 11 | C1Reaction | Arduino → PC | Event Driven | Channel 1: ms between run start and start-gate trigger (must be sent before `C1RunTime`) |
| 12 | C1SplitTime | Arduino → PC | 10msec | Channel 1: ms for current mouse's current run (display only) |
| 13 | C1RunTime | Arduino → PC | Event Driven | Channel 1: ms for a run just completed (definitive, used for score) |
| 14 | C1STrigger | Arduino → PC | Event Driven | Channel 1: new Start Gate trigger value (ON/OFF) |
| 15 | C1FTrigger | Arduino → PC | Event Driven | Channel 1: new Finish Gate trigger value (ON/OFF) |
| 16 | C1CTrigger | Arduino → PC | Event Driven | Channel 1: new Mouse-in-Start-Cell trigger value (ON/OFF) |
| 17 | C1SGLevel | Arduino → PC | 100msec | Channel 1: Start Gate phototransistor intensity |
| 18 | C1FGLevel | Arduino → PC | 100msec | Channel 1: Finish Gate phototransistor intensity |
| 19 | C1SCLevel | Arduino → PC | 100msec | Channel 1: Mouse-in-Start-Cell phototransistor intensity |
| 21 | C2Reaction | Arduino → PC | Event Driven | Channel 2: ms between run start and start-gate trigger (must be sent before `C2RunTime`) |
| 22 | C2SplitTime | Arduino → PC | 10msec | Channel 2: ms for current mouse's current run (display only) |
| 23 | C2RunTime | Arduino → PC | Event Driven | Channel 2: ms for a run just completed (definitive, used for score) |
| 24 | C2STrigger | Arduino → PC | Event Driven | Channel 2: new Start Gate trigger value (ON/OFF) |
| 25 | C2FTrigger | Arduino → PC | Event Driven | Channel 2: new Finish Gate trigger value (ON/OFF) |
| 26 | C2CTrigger | Arduino → PC | Event Driven | Channel 2: new Mouse-in-Start-Cell trigger value (ON/OFF) |
| 27 | C2SGLevel | Arduino → PC | 100msec | Channel 2: Start Gate phototransistor intensity |
| 28 | C2FGLevel | Arduino → PC | 100msec | Channel 2: Finish Gate phototransistor intensity |
| 29 | C2SCLevel | Arduino → PC | 100msec | Channel 2: Mouse-in-Start-Cell phototransistor intensity |
| 30 | CourseTimeMs | Arduino → PC | Event Driven | Higher-resolution alternative to type `1`. ms the current mouse has been active in the maze |
| 71 | STrigger | Arduino → PC | Event Driven | *Legacy compat.* New Start Gate trigger value (ON/OFF) |
| 72 | FTrigger | Arduino → PC | Event Driven | *Legacy compat.* New Finish Gate trigger value (ON/OFF) |
| 73 | CTrigger | Arduino → PC | Event Driven | *Legacy compat.* New Mouse-in-Start-Cell trigger value (ON/OFF) |
| 81 | SGLevel | Arduino → PC | 100msec | *Legacy compat.* Start Gate phototransistor intensity |
| 82 | SGPot | Arduino → PC | 100msec | *Legacy compat.* Start Gate potentiometer value |
| 83 | FGLevel | Arduino → PC | 100msec | *Legacy compat.* Finish Gate phototransistor intensity |
| 84 | FGPot | Arduino → PC | 100msec | *Legacy compat.* Finish Gate potentiometer value |
| 85 | SCLevel | Arduino → PC | 100msec | *Legacy compat.* Mouse-in-Start-Cell phototransistor intensity |
| 86 | SCPot | Arduino → PC | 100msec | *Legacy compat.* Mouse-in-Start-Cell potentiometer value |
| 96 | TimerType | Arduino → PC | Event Driven | Type of timer (valid values: `1CH`, `2CH`) |
| 97 | RequestType | PC → Arduino | Event Driven | Request the timer identify its type (value always 0) |
| 98 | NewMouse | PC → Arduino | Event Driven | A new mouse was selected in the Windows app (value always 0) |
| 99 | SetMode | PC → Arduino | Event Driven | Controls Arduino mode (valid values: `TIMER` normal mode, `CALIBRATION` start returning calibration data) |
