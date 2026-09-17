"""Headless app core (APP-1.7): wires the state object (`APP-1.3`), DB
layer (`APP-1.4`), and serial transport (`APP-1.6`) together exactly as
`parseData()`'s switch and the `*_message()` handlers do -- draining the
queue, mutating state, triggering the same DB writes on run completion.
**Still no Tkinter at all** -- `APP-1.8` is thin Tkinter wiring around
this.

Scope is wider than just the message-driven path (found reading
`Form1.cs` in full during `APP-1.0`): touch counting, DNF, `clear_timer()`
as one state-object method, and the shared "start new entry" primitive
(`NewMouse` + `clear_timer()`) that 3 separate legacy call sites duplicate.

**Deliberately out of scope here** (periodic-timer concerns, not
message-driven -- `Timer1_Tick`, `Form1.cs:2648`): the local time
interpolation that ticks `split_time_ms`/`maze_time_ms` between real gate
messages, and the watchdog alarm's repeat-counter/sound. Both belong to
`APP-1.8`'s `after()`-driven main-loop wiring, not this headless core.

**Sound and outbound-write side effects** are surfaced through optional
callbacks (`on_sound`, `transport`) rather than baked in, so this module
never imports an audio library or requires a real serial connection to
test -- `APP-1.12` is where sound actually gets wired to playback.

See `plans/arch-1-concurrency.md` and `Q-7` in `todo.md` (user-confirmed:
`Touches_Enabled`/etc. are plain true/false -- see
`rats.state.ScoringConfig`'s docstring).
"""
from __future__ import annotations

import sqlite3
from typing import Callable, Optional

from rats import db
from rats.serial_protocol import Line, Message
from rats.serial_transport import Disconnected, QueueItem, Transport, send_new_mouse
from rats.state import AppState

# TimerState values (SER-1 / Calibration_Form state table).
_STATE_ROBOT_AT_START = 2
_STATE_RUNNING = 4
_STATE_RUN_COMPLETE = 5

# The exact three sounds Form1.cs's timer_state_message() plays, per state.
_SOUND_BY_STATE = {
    _STATE_ROBOT_AT_START: "chimes.wav",
    _STATE_RUNNING: "chord.wav",
    _STATE_RUN_COMPLETE: "tada.wav",
}

# A run shorter than this is discarded outright (run_time_ms_message()'s
# "Discarding <2s run time" branch, Form1.cs:3018).
_MIN_RUN_TIME_MS = 2000


class AppCore:
    """One instance per running app, alongside one `AppState`. Owns no
    thread and no queue itself -- `drain_queue()` is called by whatever
    owns the `after()`-driven main loop (`APP-1.8`) or, in tests, directly."""

    def __init__(
        self,
        state: AppState,
        conn: sqlite3.Connection,
        transport: Optional[Transport] = None,
        on_sound: Optional[Callable[[str], None]] = None,
    ):
        self.state = state
        self.conn = conn
        self.transport = transport
        self.on_sound = on_sound

    # -- Queue draining ---------------------------------------------------

    def drain_queue(self, items: list) -> None:
        """Process a batch of `QueueItem`s already pulled off the reader
        thread's queue, in order -- what `APP-1.8`'s `after()` callback
        does each tick. Every item's raw text should already have been
        logged by the caller (`Q-6`) before it gets here; this only
        handles dispatch."""
        for item in items:
            self.handle_queue_item(item)

    def handle_queue_item(self, item: QueueItem) -> None:
        if isinstance(item, Line):
            if item.message is not None:
                self.handle_message(item.message)
        elif isinstance(item, Disconnected):
            self.state.connection.serial_port_opened = False

    def handle_message(self, message: Message) -> None:
        """`parseData()`'s `switch (text)`, Form1.cs:2878 onward."""
        code, value = message.code, message.value
        if code == 0:  # WatchDog
            self.state.watchdog.watchdog_ms_since_reset = 0
        elif code == 1:  # CourseTime
            self._on_maze_time(value)
        elif code == 2:  # SplitTime
            self._on_split_time(value)
        elif code == 3:  # RunTime
            self._on_run_time(value)
        elif code == 4:  # TimerState
            self._on_timer_state(value)
        elif code == 5:  # RunTimeMs
            self._on_run_time(value)
        elif code == 6:  # SplitToRun
            if value == 0:
                self.state.run.run_time_ms = self.state.run.split_time_ms
                self._on_run_time(self.state.run.run_time_ms)
        elif code == 12:  # C1SplitTime
            self._on_split_time(value)
        elif code == 13:  # C1RunTime
            self._on_run_time(value)
        elif code == 30:  # CourseTimeMs
            self._on_maze_time(value)
        elif code == 71:  # STrigger
            self.state.gate_diagnostics.STrigger = value
        elif code == 72:  # FTrigger
            self.state.gate_diagnostics.FTrigger = value
        elif code == 73:  # CTrigger
            self.state.gate_diagnostics.CTrigger = value
        elif code == 81:  # SGLevel
            self.state.gate_diagnostics.SGLevel = value
        elif code == 82:  # SGPot
            self.state.gate_diagnostics.SGPot = value
        elif code == 83:  # FGLevel
            self.state.gate_diagnostics.FGLevel = value
        elif code == 84:  # FGPot
            self.state.gate_diagnostics.FGPot = value
        elif code == 85:  # SCLevel
            self.state.gate_diagnostics.SCLevel = value
        elif code == 86:  # SCPot
            self.state.gate_diagnostics.SCPot = value

    # -- Message handlers (maze_time_ms_message/split_time_ms_message/
    #    run_time_ms_message/timer_state_message, Form1.cs ~2952 onward) --

    def _on_maze_time(self, value: int) -> None:
        self.state.run.maze_time_ms = value
        self.state.run.time_left_ms = (
            self.state.run.entry_time_limit_s * 1000 - self.state.run.maze_time_ms
        )
        self.state.run.maze_time_running = True

    def _on_split_time(self, value: int) -> None:
        self.state.run.split_time_ms = value
        self.state.run.split_time_running = True

    def _on_timer_state(self, new_state: int) -> None:
        old_state = self.state.run.timing_gates_state
        self.state.run.timing_gates_state = new_state

        if new_state in (0, 1):
            self.state.run.split_time_running = False
        elif new_state == _STATE_ROBOT_AT_START:
            self.state.run.split_time_running = False
            self.state.run.split_time_ms = 0
            if self.state.scoring.touches_per_run:
                self.state.run.no_of_touches = 0
        elif new_state == _STATE_RUNNING:
            if old_state != new_state and not self.state.entry.practice_mode:
                self.state.run.no_of_runs_used += 1

        sound = _SOUND_BY_STATE.get(new_state)
        if sound is not None and self.on_sound is not None:
            self.on_sound(sound)

    def _on_run_time(self, run_time_ms: int) -> None:
        self.state.run.run_time_ms = run_time_ms
        if run_time_ms <= _MIN_RUN_TIME_MS:
            return  # "Discarding <2s run time", Form1.cs:3159

        run = self.state.run
        scoring = self.state.scoring
        score_time_ms = 0
        if scoring.entry_time_divider > 0:
            score_time_ms += round(run.maze_time_ms / scoring.entry_time_divider)
        if scoring.touches_enabled and run.no_of_touches > 0:
            if scoring.touch_time_divider > 0:
                score_time_ms += round(run_time_ms / scoring.touch_time_divider)
            if scoring.touch_time_ms > 0:
                if scoring.touches_cumulative:
                    score_time_ms += scoring.touch_time_ms * run.no_of_touches
                else:
                    score_time_ms += scoring.touch_time_ms
        score_time_ms += run_time_ms
        run.score_time_ms = score_time_ms

        entry = self.state.entry
        is_duplicate = (
            run.last_run_time_inserted == run_time_ms and run.last_entry_id_inserted == entry.entry_id
        )
        should_discard = (
            is_duplicate
            or run.no_of_runs_used > run.no_of_runs_allowed
            or run.time_left_ms + run.grace_period_ms <= 0
            or not (not entry.practice_mode and entry.entry_id > 0 and run.time_left_ms + run_time_ms > 0)
        )
        if should_discard:
            return

        db.insert_entry_run(
            self.conn, entry.entry_id, run_time_ms, run.maze_time_ms, run.no_of_touches, score_time_ms
        )
        db.mark_entry_successful(self.conn, entry.entry_id)
        run.last_run_time_inserted = run_time_ms
        run.last_entry_id_inserted = entry.entry_id
        run.last_score_time_inserted = score_time_ms

        competition_id = self.state.event.competition_id
        if run.fastest_score_time_this_robot < 0:
            run.fastest_score_time_this_robot = score_time_ms
            db.insert_best_score_time(
                self.conn,
                entry_id=entry.entry_id,
                mouse_name=entry.robot,
                contestant_name=entry.contestant,
                contestant_class=entry.contestant_class,
                score_time_ms=score_time_ms,
                run_time_ms=run_time_ms,
                competition_id=competition_id,
            )
        elif score_time_ms < run.fastest_score_time_this_robot:
            run.fastest_score_time_this_robot = score_time_ms
            db.update_best_score_time(
                self.conn,
                entry_id=entry.entry_id,
                mouse_name=entry.robot,
                contestant_name=entry.contestant,
                contestant_class=entry.contestant_class,
                score_time_ms=score_time_ms,
                run_time_ms=run_time_ms,
                competition_id=competition_id,
            )

        self.state.display_refresh.refresh_robot_run_times = True
        self.state.display_refresh.refresh_best_score_times = True
        rank = db.rank_of(self.conn, competition_id, entry.robot)
        run.robot_rank = rank if rank is not None else 0

    # -- Competition / entry selection (Form1.cs event handlers) ----------

    def select_competition(self, competition: db.CompetitionSummary) -> None:
        """`Competition_DataGridview_SelectionChanged`, `Form1.cs:2261`.
        Practice-mode gating (the legacy handler no-ops when practice mode
        is on) is the caller's responsibility -- the competition grid is
        disabled during practice mode (`APP-1.8`)."""
        self.state.event.previous_competition_id = self.state.event.competition_id
        self.state.event.competition_id = competition.competition_id
        self.state.event.competition_name = competition.competition_name
        self.state.run.entry_time_limit_s = competition.entry_time_limit_s
        self.state.run.no_of_runs_allowed = competition.no_of_runs_allowed
        self.state.run.grace_period_s = competition.grace_period_s
        self.state.run.grace_period_ms = competition.grace_period_s * 1000
        self.state.run.time_left_s = competition.entry_time_limit_s
        self.state.run.no_of_runs_used = 0

        model = db.select_scoring_model(self.conn, competition.scoring_model_short_name)
        if model is not None:
            self.state.scoring.touches_enabled = model.touches_enabled
            self.state.scoring.touches_cumulative = model.touches_cumulative
            self.state.scoring.touch_time_ms = model.touch_time_ms
            self.state.scoring.entry_time_divider = model.entry_time_divider
            self.state.scoring.touch_time_divider = model.touch_time_divider
            self.state.scoring.touches_per_run = model.touches_per_run
        self.state.display_refresh.refresh_best_score_times = True

    def select_entry(self, entry_id: int, mouse_name: str) -> None:
        """`Mouse_DataGridview_SelectionChanged`, `Form1.cs:2335`. A
        DB-lookup miss falls back to the legacy `"_"` placeholder
        convention rather than raising."""
        self.state.entry.entry_id = entry_id
        self.state.entry.robot = mouse_name
        contestant = db.select_contestant_for_mouse(self.conn, mouse_name)
        if contestant is not None:
            self.state.entry.contestant, self.state.entry.contestant_class = contestant
        else:
            self.state.entry.contestant = "_"
            self.state.entry.contestant_class = "_"
            self.state.run.robot_rank = 0
        self.start_new_entry()

    def toggle_practice_mode(self) -> None:
        """`Practice_Mode_Button_Click`, `Form1.cs:2452`."""
        if self.state.entry.practice_mode:
            self.state.entry.practice_mode = False
            self.state.entry.robot = ""
        else:
            self.state.entry.practice_mode = True
            self.state.entry.robot = "Practice Mode"
            self.state.entry.contestant = "_"
            self.state.display_refresh.hide_best_score_times = True
        self.start_new_entry()

    def new_mouse(self) -> None:
        """`New_Mouse_Button_Click`, `Form1.cs:2592`, after the confirm
        dialog (a UI-layer concern, not core). Legacy's grid-refresh hack
        (jump `CurrentCell` to force `SelectionChanged` to re-fire) has no
        Tkinter equivalent (`behavior_inventory.md`) -- callers should just
        re-render the pending-entries list directly."""
        self.start_new_entry()
        if not self.state.entry.practice_mode:
            self.state.run.fastest_score_time_this_robot = -1
            self.state.display_refresh.refresh_best_score_times = True
            self.state.display_refresh.hide_robot_runtimes = True
            self.state.entry.robot = ""
            self.state.entry.contestant = "_"
            self.state.run.robot_rank = 0

    def start_new_entry(self) -> None:
        """The shared `NewMouse` (`<98,0>`) + `clear_timer()` primitive --
        found sent identically from `select_entry()`/
        `toggle_practice_mode()`/`new_mouse()` (3 separate call sites in
        the legacy app, `APP-1.0`), collapsed to one function here."""
        if self.transport is not None:
            send_new_mouse(self.transport)
        self.state.clear_timer()

    def clear(self) -> None:
        """`Clear_Button_Click`, `Form1.cs:2587` -- `clear_timer()` only,
        no `NewMouse` (unlike `start_new_entry()`'s 3 call sites)."""
        self.state.clear_timer()

    def touch(self) -> None:
        """`Touch_Button_Click`, `Form1.cs:2513`."""
        self.state.run.no_of_touches += 1

    def dnf(self) -> None:
        """`DNF_Button_Click`, `Form1.cs:2553`, after the confirm dialog (a
        UI-layer concern, not core). No-op in practice mode, matching the
        legacy early return. No `Entry_Run`/`Best_Score_Time` row is
        written -- purely an `Entry` state change."""
        if self.state.entry.practice_mode:
            return
        db.mark_entry_retired(self.conn, self.state.entry.entry_id)
        self.state.clear_timer()
