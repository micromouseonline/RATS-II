"""Application state: the `Public_Variables` replacement (APP-1.3, ARCH-1).

Plain data only, grouped logically by concern -- no behavior, no GUI/DB/
serial imports. Mirrors `legacy/rats_exe/RATS/Public_Variables.cs` field for
field (same names, so it stays easy to cross-reference against the legacy
source and `plans/`/`todo.md`, which already cite several of these names
directly), with two differences:

- `connString` is dropped. Per `REPO-3`, the hardcoded Access connection
  string is replaced by an explicit SQLite file path, which is the DB
  layer's concern (`APP-1.4`/`APP-1.8`), not app state.
- `public_competition_name`/`public_competition_class` are renamed to
  `competition_name`/`competition_class` -- the `public_` prefix only ever
  reflected VB module-level accessibility, which has no meaning here.

Per `ARCH-1`: exactly one `AppState` instance exists per running app,
mutated only from the main thread (inside `drain_queue()` and window event
handlers) -- never from the serial reader thread. That rule lives in the
code that uses this object, not here.
"""
from __future__ import annotations

from dataclasses import dataclass, field
from datetime import datetime
from typing import Optional


@dataclass
class ConnectionState:
    """Live connection status. The DB file path itself lives in the DB
    layer/window config (`APP-1.4`/`APP-1.8`), not here."""

    database_available: bool = False
    serial_port_opened: bool = False


@dataclass
class EventState:
    """The selected robotics event and competition."""

    robotics_event_id: int = 0
    robotics_event: str = ""
    robotics_event_date: Optional[datetime] = None
    competition_name: str = ""
    competition_class: str = ""
    competition_id: int = 0
    previous_competition_id: int = 0


@dataclass
class EntryState:
    """The selected entry (robot + contestant) about to run."""

    entry_id: int = 0
    robot: str = ""
    contestant: str = ""
    contestant_class: str = ""
    practice_mode: bool = True


@dataclass
class RunState:
    """Live timing/scoring state for the run in progress, plus the
    per-competition run-count/limit fields it's checked against."""

    split_time_ms: int = 0
    run_time_ms: int = 0
    maze_time_ms: int = 0
    timing_gates_state: int = 0
    score_time_ms: int = 0
    fastest_score_time_this_robot: int = -1
    robot_rank: int = 0
    no_of_touches: int = 0
    time_left_ms: int = 600000
    time_left_s: int = 600
    grace_period_s: int = 30
    grace_period_ms: int = 30000
    no_of_runs_used: int = 0
    no_of_runs_allowed: int = 5


@dataclass
class DisplayRefreshState:
    """Poll/refresh bookkeeping for the scoreboard-facing display windows."""

    refresh_robot_run_times: bool = False
    refresh_run_times_delay: int = 0
    refresh_best_score_times: bool = False
    refresh_best_score_times_delay: int = 0
    refresh_run_order_delay: int = 0
    hide_robot_runtimes: bool = False
    hide_best_score_times: bool = False


@dataclass
class WindowState:
    """Cross-window open/close-request flags -- the legacy app's poor-man's
    pub/sub for coordinating the main form with its child windows."""

    single_channel_window_open: bool = False
    request_single_channel_window_close: bool = False
    single_channel_results_window_open: bool = False
    request_single_channel_results_window_close: bool = False
    run_order_window_open: bool = False
    request_run_order_window_close: bool = False
    calibration_window_open: bool = False
    request_calibration_close: bool = False
    request_calibration_mode: bool = False
    request_timer_mode: bool = False


@dataclass
class GateDiagnosticsState:
    """Calibration-form readings for the three sensor gates: Start Gate
    (SG), Finish Gate (FG), and Start Cell (SC) -- each a potentiometer
    reading, a brightness level, and a triggered flag."""

    SGPot: int = 0
    SGLevel: int = 0
    STrigger: bool = False
    FGPot: int = 0
    FGLevel: int = 0
    FTrigger: bool = False
    SCPot: int = 0
    SCLevel: int = 0
    CTrigger: bool = False


@dataclass
class WatchdogState:
    """Serial-link watchdog status."""

    watchdog_active: bool = True
    watchdog_alarm: bool = False
    watchdog_ms_since_reset: int = 0
    watchdog_alarm_repeat_counter: int = 201


@dataclass
class AppState:
    """The full application state -- one instance per running app."""

    connection: ConnectionState = field(default_factory=ConnectionState)
    event: EventState = field(default_factory=EventState)
    entry: EntryState = field(default_factory=EntryState)
    run: RunState = field(default_factory=RunState)
    display_refresh: DisplayRefreshState = field(default_factory=DisplayRefreshState)
    windows: WindowState = field(default_factory=WindowState)
    gate_diagnostics: GateDiagnosticsState = field(default_factory=GateDiagnosticsState)
    watchdog: WatchdogState = field(default_factory=WatchdogState)
