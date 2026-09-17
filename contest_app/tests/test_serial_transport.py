"""Unit tests for the serial transport / reader thread (APP-1.6)."""
import queue
import time

import serial

from fake_serial import FakeSerialTransport
from rats.serial_protocol import Line, Message
from rats.serial_transport import (
    Disconnected,
    SerialReader,
    configure_serial_port,
    send_new_mouse,
    send_set_mode,
)


def drain(q: "queue.Queue", count: int, timeout: float = 2.0) -> list:
    """Collect exactly `count` items from q, waiting up to `timeout`
    total -- avoids a flaky test racing the background thread."""
    items = []
    deadline = time.monotonic() + timeout
    while len(items) < count:
        remaining = deadline - time.monotonic()
        assert remaining > 0, f"timed out waiting for {count} items, got {len(items)}"
        items.append(q.get(timeout=remaining))
    return items


def test_messages_arrive_on_the_queue_in_order():
    transport = FakeSerialTransport([b"<0,0>\n<4,4>\n<13,9260>\n"])
    q: "queue.Queue" = queue.Queue()
    reader = SerialReader(transport, q)
    reader.start()
    try:
        items = drain(q, 3)
        assert items == [
            Line("<0,0>", Message(0, None)),
            Line("<4,4>", Message(4, 4)),
            Line("<13,9260>", Message(13, 9260)),
        ]
    finally:
        reader.stop()


def test_a_line_split_across_two_reads_still_parses():
    transport = FakeSerialTransport()
    q: "queue.Queue" = queue.Queue()
    reader = SerialReader(transport, q)
    reader.start()
    try:
        transport.feed(b"<13,926")
        transport.feed(b"0>\n")
        items = drain(q, 1)
        assert items == [Line("<13,9260>", Message(13, 9260))]
    finally:
        reader.stop()


def test_garbage_lines_still_reach_the_queue_for_logging():
    transport = FakeSerialTransport([b"junk line\n<13,9260>\n"])
    q: "queue.Queue" = queue.Queue()
    reader = SerialReader(transport, q)
    reader.start()
    try:
        items = drain(q, 2)
        assert items == [
            Line("junk line", None),
            Line("<13,9260>", Message(13, 9260)),
        ]
    finally:
        reader.stop()


def test_thread_stops_cleanly_within_timeout():
    transport = FakeSerialTransport()
    q: "queue.Queue" = queue.Queue()
    reader = SerialReader(transport, q)
    reader.start()
    stopped = reader.stop(timeout=2.0)
    assert stopped is True


def test_transport_error_surfaces_as_disconnected_not_a_crash():
    class BrokenTransport:
        def read(self, size=1):
            raise OSError("device disappeared")

        def close(self):
            pass

    q: "queue.Queue" = queue.Queue()
    reader = SerialReader(BrokenTransport(), q)
    reader.start()
    items = drain(q, 1)
    assert items == [Disconnected("device disappeared")]
    # the thread already exited on its own after the error -- stop() should
    # still report it as stopped.
    assert reader.stop(timeout=2.0) is True


def test_send_new_mouse_writes_the_exact_wire_bytes():
    transport = FakeSerialTransport()
    send_new_mouse(transport)
    assert transport.written == [b"<98,0>\n"]


def test_send_set_mode_writes_the_exact_wire_bytes():
    transport = FakeSerialTransport()
    send_set_mode(transport, "CALIBRATION")
    send_set_mode(transport, "TIMER")
    assert transport.written == [b"<99,CALIBRATION>\n", b"<99,TIMER>\n"]


# --- Real pyserial configuration (no hardware needed: unopened Serial()) --


def test_configure_serial_port_sets_every_legacy_attribute():
    ser = serial.Serial()
    configure_serial_port(ser, port_name="/dev/ttyFAKE", baud_rate=115200)
    assert ser.port == "/dev/ttyFAKE"
    assert ser.baudrate == 115200
    assert ser.bytesize == serial.EIGHTBITS
    assert ser.parity == serial.PARITY_NONE
    assert ser.stopbits == serial.STOPBITS_ONE
    assert ser.xonxoff is False
    assert ser.rtscts is False
    assert ser.dsrdtr is False
    assert ser.timeout == 0.2


def test_configure_serial_port_baud_rate_is_not_hardcoded():
    # The legacy app hardcoded 9600 with no UI to change it -- confirm our
    # port actually takes whatever baud rate is passed, since APP-1.8's
    # port selector will offer a choice (noted during this stage).
    ser = serial.Serial()
    configure_serial_port(ser, port_name="/dev/ttyFAKE", baud_rate=9600)
    assert ser.baudrate == 9600
    configure_serial_port(ser, port_name="/dev/ttyFAKE", baud_rate=115200)
    assert ser.baudrate == 115200
