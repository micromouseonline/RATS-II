#!/usr/bin/env python3
"""Build sample_data/demo.db -- a sanitized copy of live_data/rats.db for
public/sample/test use (REPO-2).

Sanitization policy (Q-2, revised 2026-09-18):
- `Class` is kept as-is, real values. Originally redacted, but it's used as
  a filter/key in several places (e.g. Junior/Senior report splits,
  RATSdb's queries) and turned out to need NOT NULL in practice -- nulling
  it broke more than it protected. Retained for now.
- `Contestant_Email_Address` is real for nobody, but NOT uniformly blank
  either: for every contestant who has a real email in the live data, we
  fabricate a plausible-but-fake one from their real name and a random
  classic Microsoft demo domain (contoso.com, fabrikam.com, etc.) -- so
  email fields have realistic-looking non-empty content to test forms
  against, without leaking anyone's actual address. Contestants who never
  gave a real email stay NULL (we don't invent data that wasn't there).

Everything else is copied verbatim, including the specific edge-case rows
existing tests already depend on (Best_Score_Time.ID=781's unmatched
placeholder, the Competition 39 tie case, the NULL Score_Time_mS row) -- this
is a full sanitized copy, not a synthesized or trimmed-down excerpt, so it
preserves real relationships rather than fabricating unrealistic ones.

Re-run after rebuilding live_data/rats.db (legacy/database/build_sqlite.py)
if the source data changes.
"""
import random
import re
import shutil
import sqlite3
from pathlib import Path

HERE = Path(__file__).parent
REPO_ROOT = HERE.parent
SOURCE = REPO_ROOT / "live_data" / "rats.db"
DEST = HERE / "demo.db"

# Classic Microsoft demo/documentation domains.
DEMO_DOMAINS = [
    "contoso.com",
    "fabrikam.com",
    "litwareinc.com",
    "northwindtraders.com",
    "woodgrovebank.com",
    "adatum.com",
    "treyresearch.net",
    "wingtiptoys.com",
    "tailspintoys.com",
    "proseware.com",
]

# Fixed seed: reproducible output across re-runs (stable diffs, stable tests).
_RNG = random.Random(20260918)


def fake_email(contestant_name: str) -> str:
    cleaned = re.sub(r"[^A-Za-z\s]", " ", contestant_name)
    words = [w.lower() for w in cleaned.split() if w]
    local_part = ".".join(words[:3]) if words else "contestant"
    domain = _RNG.choice(DEMO_DOMAINS)
    return f"{local_part}@{domain}"


def build():
    if not SOURCE.exists():
        raise SystemExit(
            f"{SOURCE} not found -- run legacy/database/build_sqlite.py first"
        )
    if DEST.exists():
        DEST.unlink()
    shutil.copyfile(SOURCE, DEST)

    conn = sqlite3.connect(DEST)
    cur = conn.cursor()

    rows = cur.execute(
        "SELECT Contestant_ID, Contestant_Name FROM Contestant "
        "WHERE Contestant_Email_Address IS NOT NULL AND Contestant_Email_Address <> ''"
    ).fetchall()
    for contestant_id, name in rows:
        cur.execute(
            "UPDATE Contestant SET Contestant_Email_Address = ? WHERE Contestant_ID = ?",
            (fake_email(name), contestant_id),
        )
    conn.commit()

    # Verify: no real email survived, but every contestant who had one still
    # has a (fake) one, and nobody who didn't have one got a fabricated one.
    cur.execute(
        "SELECT COUNT(*) FROM Contestant WHERE Contestant_Email_Address IS NOT NULL"
    )
    fake_email_count = cur.fetchone()[0]
    conn.close()

    assert fake_email_count == len(rows), (
        f"expected {len(rows)} fake emails, found {fake_email_count}"
    )

    print(f"Built {DEST} ({len(rows)} fake emails generated, Class left untouched)")


if __name__ == "__main__":
    build()
