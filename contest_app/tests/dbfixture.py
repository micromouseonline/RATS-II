"""Shared test fixture: an in-memory SQLite copy of sample_data/demo.db.

APP-1.2 (TEST-1's deliverable). Building the copy in memory via sqlite3's
backup API -- rather than copying the file to a temp path on disk, which is
what check_db5_rewrites.py did before this existed -- means no filesystem
writes at all and a fresh, independent copy per call, so mutations in one
test can never leak into another.
"""
import sqlite3
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
DEMO_DB = REPO_ROOT / "sample_data" / "demo.db"


def demo_db_connection() -> sqlite3.Connection:
    """Return a fresh in-memory sqlite3 connection seeded from demo.db.

    The source file is opened read-only and only ever read from -- this
    never touches sample_data/demo.db on disk. Caller owns the returned
    connection and should close() it when done.
    """
    source = sqlite3.connect(f"file:{DEMO_DB}?mode=ro", uri=True)
    target = sqlite3.connect(":memory:")
    source.backup(target)
    source.close()
    target.execute("PRAGMA foreign_keys = OFF")
    return target
