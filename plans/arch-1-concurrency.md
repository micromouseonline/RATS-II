# ARCH-1: Concurrency model decision

**Decision: background thread + thread-safe queue** (not pure `after()`
polling). Rationale below; this doc is the design to build against once
`APP-1` starts.

## Why

The legacy app (`legacy/rats_exe/RATS/Form1.cs:2648`, `Timer1_Tick`) polls at a fixed
10ms interval on the UI thread: disables the timer, calls
`SerialPort1.ReadExisting()` (non-blocking snapshot), parses, re-enables the
timer. No background thread, no `DataReceived` event — confirmed by reading
the source, not assumed. It self-serializes (a slow tick just delays the
next one) and has worked in production for years at 9600 baud.

That model's weak point: a busy UI thread (e.g. the synchronous DB writes
inline in `run_time_ms_message()`, `Form1.cs:3056` onward) delays draining
the serial port. At 9600 baud the OS receive buffer absorbs a few slow
ticks fine. The port is moving to **115200 baud** (12x the byte rate), which
shrinks that safety margin by the same factor — the same stall duration now
represents 12x the buffered bytes, with real risk of dropped bytes rather
than just added latency.

A dedicated reader thread blocking on `serial.read()` is immune to this —
it keeps draining the port regardless of what Tkinter's main loop is doing.

Secondary factor, not the deciding one: this shape also gives a
transport-agnostic boundary for free. Bluetooth Classic (RFCOMM behaves like
a serial port — same blocking-read shape) or a future websocket source
(naturally `asyncio`, but its event loop can push onto the same
`queue.Queue`) become additional producers feeding the same queue, with no
change needed on the consumer/UI side. Not building this now, just not
closing the door on it for free.

## Design

```
[reader thread]                    [main / Tk thread]
serial.read() (blocking)  --\
parse bytes -> Message      |--->  queue.Queue()  --->  after(N, drain_queue)
                             /                              |
                                                              v
                                                    mutate state object,
                                                    update widgets
```

**Hard rule: the reader thread only parses and enqueues.** It never touches
the shared state object or any Tk widget directly. All state mutation and
all UI updates happen on the main thread, inside the `after()`-driven
`drain_queue()` callback, draining everything currently on the queue each
tick. This is the one invariant that makes the rest of the design safe
without needing locks on the state object itself — enforce it in code
review, not just convention.

- **Reader thread**: owns the `pyserial` (or future transport) connection.
  Loop: blocking read → accumulate into `<...>` frames (same framing logic
  as legacy `parseData()`'s buffer scan) → on a complete frame, construct a
  small `Message(type: int, value: str, received_at: float)` object → put
  on the queue. Catches and logs its own exceptions; a transport error
  should surface as a special message on the queue (e.g. `Disconnected`),
  not an unhandled thread crash.
- **Queue**: stdlib `queue.Queue()`, thread-safe by construction (GIL
  covers it, no extra locking needed).
- **Main thread**: `after(N, drain_queue)` — likely a short interval like
  10–20ms, similar cadence to the legacy `Timer1`, but now just draining a
  queue instead of doing the actual I/O. Drains *all* currently-queued
  messages per tick (not just one), so bursts don't accumulate display lag.
  This is also where the legacy app's local time interpolation
  (`split_time_ms`/`maze_time_ms` ticking between real gate messages, per
  the vendor spec's precedence rule) belongs — it's a main-thread-only
  concern, unrelated to the reader thread.
- **State object** (the `Public_Variables` replacement): a plain object/
  dataclass instance, mutated only from the main thread inside
  `drain_queue()` and window event handlers. No locking needed as long as
  the hard rule above holds.
- **Outbound messages** (`NewMouse`, `SetMode`): still go straight to
  `serial.write()`. These are rare (a handful of call sites in the legacy
  app) and don't need to route through the queue — only inbound parsing
  needs the thread boundary, since that's what has to keep up with a
  potentially-busy UI thread.
- **Lifecycle**: reader thread starts on connect, must be cleanly stoppable
  on disconnect/app close (e.g. a sentinel value or a `threading.Event` the
  read loop checks between reads — exact mechanism to work out during
  `APP-1`, since `pyserial`'s blocking read doesn't have a built-in
  cooperative-cancel hook; a read timeout + flag check is the usual pattern).

## Testability (feeds `TEST-1`)

Keep frame-parsing (`bytes/str -> Message`) as a pure function, separate
from the thread/queue plumbing around it. `TEST-1`'s mock serial device can
then either feed bytes through the real reader thread for an integration
test, or call the parser directly for fast unit tests — same split the
legacy `parseData()` already has to make (the buffer-scanning frame logic
vs. the per-code side effects), just done cleanly instead of tangled
together with UI updates like the original.

## Open for `APP-1` to decide

- Exact `after()` interval for `drain_queue` (legacy used 10ms; may not need
  to be that tight once real I/O isn't happening on the main thread).
- Read timeout value for the reader thread's cooperative-cancel check.
- Whether `Message` is a dataclass, a `NamedTuple`, or something else —
  not architecturally significant, just needs picking.
