"""Trivial checks proving out the shared test fixtures (APP-1.2)."""
from fake_serial import FakeSerialTransport


def test_demo_db_fixture_starts_with_real_data(demo_db):
    count = demo_db.execute("SELECT COUNT(*) FROM Mouse").fetchone()[0]
    assert count > 0


def test_demo_db_fixture_mutations_do_not_leak_across_tests(demo_db):
    # If a previous test's mutation somehow leaked (e.g. the fixture
    # accidentally shared one connection or wrote through to demo.db on
    # disk), this table would already be empty here.
    count = demo_db.execute("SELECT COUNT(*) FROM Mouse").fetchone()[0]
    assert count > 0
    demo_db.execute("DELETE FROM Mouse")
    assert demo_db.execute("SELECT COUNT(*) FROM Mouse").fetchone()[0] == 0


def test_fake_serial_transport_delivers_scripted_chunks_in_order():
    transport = FakeSerialTransport([b"<98,0>", b"<12,150>"])
    assert transport.read() == b"<98,0>"
    assert transport.read() == b"<12,150>"


def test_fake_serial_transport_feed_after_construction():
    transport = FakeSerialTransport()
    transport.feed(b"<99,TIMER>")
    assert transport.read() == b"<99,TIMER>"
