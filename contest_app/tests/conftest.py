import pytest

from dbfixture import demo_db_connection


@pytest.fixture
def demo_db():
    """A fresh in-memory sqlite3 connection seeded from sample_data/demo.db."""
    conn = demo_db_connection()
    yield conn
    conn.close()
