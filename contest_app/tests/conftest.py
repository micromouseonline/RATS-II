import pytest

from dbfixture import demo_db_connection
from rats.main_window import MainWindow


@pytest.fixture
def demo_db():
    """A fresh in-memory sqlite3 connection seeded from sample_data/demo.db."""
    conn = demo_db_connection()
    yield conn
    conn.close()


@pytest.fixture
def make_main_window(tmp_path):
    """Builds a `MainWindow` with a real (fake-transport) connect path but
    an isolated `tmp_path` log directory -- `APP-1.8.4`'s log files must
    never land in the real per-user `config.log_dir()` just from running
    the test suite (same concern as never touching `sample_data/demo.db`
    on disk, see the `dbfixture` module docstring)."""

    def _make(conn, **kwargs) -> MainWindow:
        kwargs.setdefault("open_serial_port_fn", lambda port, baud: None)
        kwargs.setdefault("log_dir", tmp_path / "logs")
        return MainWindow(conn=conn, **kwargs)

    return _make
