import pytest

from dbfixture import demo_db_connection
from rats import config as config_module
from rats.main_window import MainWindow


@pytest.fixture
def demo_db():
    """A fresh in-memory sqlite3 connection seeded from sample_data/demo.db."""
    conn = demo_db_connection()
    yield conn
    conn.close()


@pytest.fixture
def make_main_window(tmp_path, monkeypatch):
    """Builds a `MainWindow` with a real (fake-transport) connect path but
    an isolated `tmp_path` log directory -- `APP-1.8.4`'s log files must
    never land in the real per-user `config.log_dir()` just from running
    the test suite (same concern as never touching `sample_data/demo.db`
    on disk, see the `dbfixture` module docstring).

    Also redirects `config.config_path()` into `tmp_path` -- found while
    testing the `APP-1.8`-layout-split refactor: `_on_open_database()` and
    `destroy()` (called by every test's `finally: window.destroy()`) both
    call `config_module.save_config()` for real, which was silently
    overwriting the developer's actual `~/.config/rats-contest-app/config.json`
    on every test run (confirmed: it ended up pointing `last_db_path` at a
    deleted pytest tmp file). Same root-cause category as the `demo.db` and
    log-file incidents -- a test hitting a real default path instead of an
    injected one."""
    fake_config_path = tmp_path / "config.json"
    monkeypatch.setattr(config_module, "config_path", lambda: fake_config_path)

    def _make(conn, **kwargs) -> MainWindow:
        kwargs.setdefault("open_serial_port_fn", lambda port, baud: None)
        kwargs.setdefault("log_dir", tmp_path / "logs")
        return MainWindow(conn=conn, **kwargs)

    return _make
