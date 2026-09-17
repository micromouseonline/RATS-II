"""Scripted fake serial transport for tests, no hardware required.

APP-1.2 (TEST-1's deliverable). Implements just the slice of pyserial's
`Serial` surface the reader thread (APP-1.6) is designed to use --
`read()`, `in_waiting`, `close()` -- so it can be swapped in for a real
`serial.Serial` instance unchanged. See plans/arch-1-concurrency.md.

Bytes are handed out one scripted chunk per read(), matching how the real
reader loop re-scans its buffer after every read regardless of how many
bytes came back -- this is what lets tests construct cases like a frame
split across two reads, multiple frames in one read, or leading garbage,
just by choosing how the script is chunked.
"""
import queue


class FakeSerialTransport:
    def __init__(self, chunks=()):
        self._queue: "queue.Queue[bytes]" = queue.Queue()
        for chunk in chunks:
            self._queue.put(chunk)
        self._closed = False

    def feed(self, chunk: bytes) -> None:
        """Queue another chunk of bytes as if it just arrived on the wire."""
        self._queue.put(chunk)

    def read(self, size: int = 1) -> bytes:
        """Block for the next scripted chunk.

        `size` is accepted for interface compatibility with pyserial but
        ignored -- each fed chunk is delivered whole.
        """
        if self._closed:
            return b""
        return self._queue.get()

    @property
    def in_waiting(self) -> int:
        return self._queue.qsize()

    def close(self) -> None:
        self._closed = True
        self._queue.put(b"")  # unblock a reader thread blocked in read()
