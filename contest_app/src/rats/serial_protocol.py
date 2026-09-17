"""Serial frame parser (APP-1.5): pure `<code,value>` protocol logic.

No I/O, no threading -- takes/returns plain strings and dataclasses only.
`APP-1.6`'s reader thread owns turning bytes off the wire into `str` and
feeding it here incrementally; this module never talks to a port or a log
file.

Scope is exactly the 19 inbound + 2 outbound codes `SER-1` found the real
app uses (`legacy/rats_exe/protocol/serial_protocol.md`) -- the C1/C2
dual-channel codes and the 96/97 timer-type handshake are never sent or
received by this build and are out of scope (`Q-3`).

## Line-based framing (per user correction, not the literal decompiled loop)

Messages are newline-terminated (`CLAUDE.md`, `SER-1`'s "followed by
CRLF"). Per the user's recollection of the real app's behavior -- treated
as a hard requirement regardless of whether it was deliberate or an
incidental side effect of the original implementation:

- A line not starting with `<` as its very first character is **totally
  ignored** -- not scanned for an embedded frame elsewhere in the line.
- Everything **after** the closing `>` on a line is ignored -- at most one
  message per line, never multiple frames packed into one line.
- **Every** input line -- whether or not it parses to a `Message` -- must
  remain available for the caller to log as a permanent record (`Line.raw`
  below). Actual file writing is `APP-1.6`/`APP-1.8`'s job, not this pure
  layer's, but nothing here may drop a raw line silently.

This replaces an earlier version of this module that mirrored
`parseData()`'s literal approach -- hunting for `<`/`,`/`>` anywhere in a
continuous character buffer, independent of newlines, which could recover
a valid frame following leading garbage on the same line. That recovery
behavior is what this rewrite deliberately removes.

## Value interpretation

`parse_message()` mirrors each `*_message()` handler's value scaling
(`Form1.cs` ~line 2878 onward): `CourseTime`/`SplitTime`/`RunTime` (legacy
2-gate-compat codes `1`/`2`/`3`) scale by `*100`/`*10`/`*10` into
milliseconds; `RunTimeMs`/`C1SplitTime`/`C1RunTime`/`CourseTimeMs` (`5`,
`12`, `13`, `30`) are already milliseconds, no scaling; `TimerState` (`4`)
and `SplitToRun` (`6`) are bare integers (a state number and a 0-guard
respectively -- interpreting *what* `SplitToRun` should do needs the
current split time, which is app state, not this pure layer's job:
`APP-1.7`); the trigger codes (`71`/`72`/`73`) are booleans, true only for
the exact strings `"ON"`/`"1"` (case-sensitive, matching the legacy
`TextCompare: false` binary comparison); the level/pot codes (`81`-`86`)
are bare integers. `WatchDog` (`0`) carries no meaningful value. A code
outside the in-scope set parses to `None` -- same as legacy's `switch` with
no matching `case`, a silent no-op, not an error.

Numeric parsing uses a small VB `Val()`-alike (`_val()`): a leading numeric
prefix, `0.0` if there isn't one -- never raises, since this is reading a
live embedded device, not validated input.
"""
from __future__ import annotations

import re
from dataclasses import dataclass
from typing import Optional, Union

# Inbound message type codes (SER-1's 19).
WATCH_DOG = 0
COURSE_TIME = 1
SPLIT_TIME = 2
RUN_TIME = 3
TIMER_STATE = 4
RUN_TIME_MS = 5
SPLIT_TO_RUN = 6
C1_SPLIT_TIME = 12
C1_RUN_TIME = 13
COURSE_TIME_MS = 30
S_TRIGGER = 71
F_TRIGGER = 72
C_TRIGGER = 73
SG_LEVEL = 81
SG_POT = 82
FG_LEVEL = 83
FG_POT = 84
SC_LEVEL = 85
SC_POT = 86

# Outbound message type codes (SER-1's 2).
NEW_MOUSE = 98
SET_MODE = 99

_TRIGGER_CODES = frozenset((S_TRIGGER, F_TRIGGER, C_TRIGGER))
_PLAIN_INT_CODES = frozenset(
    (TIMER_STATE, RUN_TIME_MS, SPLIT_TO_RUN, C1_SPLIT_TIME, C1_RUN_TIME,
     COURSE_TIME_MS, SG_LEVEL, SG_POT, FG_LEVEL, FG_POT, SC_LEVEL, SC_POT)
)

_NUMBER_RE = re.compile(r"[+-]?(\d+\.?\d*|\.\d+)")


def _val(s: str) -> float:
    """Loose numeric parse mimicking VB's Val(): a leading numeric prefix
    (after optional leading whitespace), or 0.0 if there isn't one."""
    match = _NUMBER_RE.match(s.strip())
    return float(match.group()) if match else 0.0


@dataclass(frozen=True)
class Message:
    """One parsed inbound frame. `value`'s type/meaning depends on `code`
    -- see the module docstring."""

    code: int
    value: Optional[Union[int, bool]]


@dataclass(frozen=True)
class Line:
    """One raw newline-terminated input line and whatever it parsed to.
    `raw` is populated even when `message` is None (a garbage line, or a
    well-formed frame for an out-of-scope code) -- callers must be able to
    log every line, not just the ones that produced a `Message`."""

    raw: str
    message: Optional[Message]


def split_lines(buffer: str) -> tuple[list[str], str]:
    """Split `buffer` into complete newline-terminated lines.

    A trailing `\\r` (CRLF) is stripped from each line. Whatever remains
    after the last `\\n` -- an in-progress line with no terminator yet --
    is returned as the remainder, so the caller can prepend the next
    batch of bytes to it (this is what lets a line split across two
    separate reads still parse correctly once it's complete).
    """
    lines: list[str] = []
    while True:
        nl = buffer.find("\n")
        if nl == -1:
            break
        line = buffer[:nl]
        if line.endswith("\r"):
            line = line[:-1]
        lines.append(line)
        buffer = buffer[nl + 1:]
    return lines, buffer


def parse_message(code_str: str, value_str: str) -> Optional[Message]:
    """Interpret one `(code, value)` pair. None for a code outside the
    in-scope set (matches legacy's switch having no case for it)."""
    try:
        code = int(code_str)
    except ValueError:
        return None

    if code == WATCH_DOG:
        return Message(code, None)
    if code == COURSE_TIME:
        return Message(code, round(_val(value_str) * 100))
    if code == SPLIT_TIME:
        return Message(code, round(_val(value_str) * 10))
    if code == RUN_TIME:
        return Message(code, round(_val(value_str) * 10))
    if code in _TRIGGER_CODES:
        return Message(code, value_str in ("ON", "1"))
    if code in _PLAIN_INT_CODES:
        return Message(code, round(_val(value_str)))
    return None


def parse_line(line: str) -> Optional[Message]:
    """Parse one already-isolated line. None if the line doesn't start
    with `<`, isn't well-formed (`<code,value>`), or is for an
    out-of-scope code. Only the first `,` and `>` on the line matter --
    anything after the closing `>` is ignored."""
    if not line.startswith("<"):
        return None
    comma_i = line.find(",")
    close_i = line.find(">")
    if comma_i == -1 or close_i == -1 or not (0 < comma_i < close_i):
        return None
    return parse_message(line[1:comma_i], line[comma_i + 1:close_i])


def read_messages(buffer: str) -> tuple[list[Line], str]:
    """`split_lines()` + `parse_line()` in one call -- what `APP-1.6`'s
    reader thread calls each time new bytes arrive. Returns one `Line` per
    complete line found (raw text always populated, for logging every
    line regardless of parse outcome) plus the unconsumed remainder."""
    raw_lines, remainder = split_lines(buffer)
    lines = [Line(raw=raw, message=parse_line(raw)) for raw in raw_lines]
    return lines, remainder


def format_new_mouse() -> str:
    """`<98,0>` -- NewMouse, sent on entry/robot selection."""
    return "<98,0>\n"


def format_set_mode(mode: str) -> str:
    """`<99,CALIBRATION>` or `<99,TIMER>` -- SetMode, calibration toggle."""
    if mode not in ("CALIBRATION", "TIMER"):
        raise ValueError(f"invalid SetMode value: {mode!r}")
    return f"<99,{mode}>\n"
