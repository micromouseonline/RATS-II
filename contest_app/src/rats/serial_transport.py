"""Serial transport (APP-1.6): reader thread + `queue.Queue`, per
`plans/arch-1-concurrency.md`.

Wraps `APP-1.5`'s line parser (`rats.serial_protocol`) around a duck-typed
transport -- anything exposing a blocking `read(size) -> bytes` and a
`close()`, whether that's real `pyserial.Serial` or `APP-1.2`'s
`FakeSerialTransport`. The background thread does nothing but read bytes,
split them into `Line`s, and enqueue them -- per `ARCH-1`'s hard rule, it
never touches app state or a Tk widget directly; all of that happens on
the main thread when it drains the queue (which is also where actual log-
file writing belongs, since every `Line.raw` -- parsed or not -- needs to
reach the log per `Q-6`).

Outbound writes (`NewMouse`, `SetMode`) don't go through this thread or the
queue -- they're rare (a handful of call sites in the legacy app) and go
straight to `transport.write()`, per `ARCH-1`.
"""
from __future__ import annotations

import queue
import threading
from dataclasses import dataclass
from typing import Optional, Protocol, Union

import serial

from rats.serial_protocol import Line, format_new_mouse, format_set_mode, read_messages


class Transport(Protocol):
    def read(self, size: int = 1) -> bytes: ...
    def write(self, data: bytes) -> int: ...
    def close(self) -> None: ...


@dataclass(frozen=True)
class Disconnected:
    """Enqueued when the reader thread's `transport.read()` raises -- a
    connection drop surfaces as a queue item, not an unhandled thread
    crash (`ARCH-1`)."""

    error: str


QueueItem = Union[Line, Disconnected]

DEFAULT_READ_CHUNK_SIZE = 4096


class SerialReader:
    """Owns a background thread draining `transport` into `out_queue` as
    `Line`/`Disconnected` items, in order. `start()`/`stop()` control its
    lifecycle; nothing else about it is public."""

    def __init__(
        self,
        transport: Transport,
        out_queue: "queue.Queue[QueueItem]",
        read_chunk_size: int = DEFAULT_READ_CHUNK_SIZE,
    ):
        self._transport = transport
        self._queue = out_queue
        self._read_chunk_size = read_chunk_size
        self._stop_event = threading.Event()
        self._thread: Optional[threading.Thread] = None

    def start(self) -> None:
        if self._thread is not None:
            raise RuntimeError("SerialReader already started")
        self._thread = threading.Thread(target=self._run, name="SerialReader", daemon=True)
        self._thread.start()

    def stop(self, timeout: float = 2.0) -> bool:
        """Request the reader thread stop and wait up to `timeout` seconds
        for it to actually finish. Closing the transport unblocks a
        currently-in-progress blocking `read()` call. Returns whether the
        thread had stopped by the time this returns."""
        self._stop_event.set()
        try:
            self._transport.close()
        except Exception:
            pass
        if self._thread is None:
            return True
        self._thread.join(timeout)
        return not self._thread.is_alive()

    def _run(self) -> None:
        buffer = ""
        while not self._stop_event.is_set():
            try:
                chunk = self._transport.read(self._read_chunk_size)
            except Exception as exc:
                self._queue.put(Disconnected(str(exc)))
                return
            if not chunk:
                continue
            buffer += chunk.decode("ascii", errors="replace")
            lines, buffer = read_messages(buffer)
            for line in lines:
                self._queue.put(line)


def configure_serial_port(
    ser: "serial.Serial", *, port_name: str, baud_rate: int, read_timeout: float = 0.2
) -> None:
    """Set every attribute `connect_BTN_Click` (`Form1.cs:2400`) sets on
    `SerialPort1` -- except `baud_rate` is a parameter (legacy hardcoded
    `9600`; `APP-1.8`'s port selector will offer a UI choice, noted during
    this stage) and `read_timeout` defaults far shorter than legacy's
    `10000`ms. That long timeout suited the old non-blocking polling-Timer
    model; this reader thread *blocks* in `read()`, so it needs a short
    timeout for `SerialReader.stop()`'s cooperative-cancel check to be
    responsive rather than waiting up to 10 seconds."""
    ser.port = port_name
    ser.baudrate = baud_rate
    ser.bytesize = serial.EIGHTBITS
    ser.parity = serial.PARITY_NONE
    ser.stopbits = serial.STOPBITS_ONE
    ser.xonxoff = False
    ser.rtscts = False
    ser.dsrdtr = False
    ser.timeout = read_timeout


def open_serial_port(port_name: str, baud_rate: int, *, read_timeout: float = 0.2) -> "serial.Serial":
    """Construct, configure, and open a real connection. Needs actual (or
    looped-back) hardware -- not unit-tested here; `configure_serial_port`
    above is the testable parameter-wiring piece."""
    ser = serial.Serial()
    configure_serial_port(ser, port_name=port_name, baud_rate=baud_rate, read_timeout=read_timeout)
    ser.open()
    return ser


def send_new_mouse(transport: Transport) -> str:
    """`<98,0>` -- sent on entry/robot selection. Straight to the wire, not
    through the queue (see module docstring). Returns the formatted string
    sent, so callers (`AppCore.start_new_entry`, `APP-1.8.4`) can log it
    without reformatting."""
    msg = format_new_mouse()
    transport.write(msg.encode("ascii"))
    return msg


def send_set_mode(transport: Transport, mode: str) -> str:
    """`<99,CALIBRATION>`/`<99,TIMER>` -- calibration mode toggle. Returns
    the formatted string sent (see `send_new_mouse`)."""
    msg = format_set_mode(mode)
    transport.write(msg.encode("ascii"))
    return msg
