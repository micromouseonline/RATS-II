#!/usr/bin/env python3
"""Build rats.db (SQLite) from the mdbtools CSV exports in ../../live_data/exports/.

Source: RATS_Competitions_be.accdb, exported via mdbtools (mdb-schema / mdb-export)
into ../../live_data/exports/*.csv. Re-run after re-exporting if the source
.accdb changes. Real data lives in live_data/ (git-ignored, REPO-1) — never
in legacy/database/ itself.
"""
import csv
import datetime
import sqlite3
from pathlib import Path

HERE = Path(__file__).parent
REPO_ROOT = HERE.parent.parent
LIVE_DATA = REPO_ROOT / "live_data"
EXPORTS = LIVE_DATA / "exports"
DB_PATH = LIVE_DATA / "rats.db"

# (table, primary key column or None, {column: sqlite type}, {date/datetime columns})
# Types follow exports/schema.sql; INTEGER columns that are really booleans
# (Entry_Used, Touches_Enabled, ...) are kept as INTEGER (0/1) to match Access.
TABLES = {
    "Best_Score_Time": (
        "ID",
        {
            "ID": "INTEGER", "Entry_ID": "INTEGER", "Mouse_Name": "TEXT",
            "Contestant_Name": "TEXT", "Contestant_Class": "TEXT",
            "Score_Time_mS": "INTEGER", "Run_Time_mS": "INTEGER",
            "Competition_ID": "INTEGER",
        },
        set(),
    ),
    "Challenge": (
        "Challenge_ID",
        {"Challenge_ID": "INTEGER", "Challenge_Type": "TEXT"},
        set(),
    ),
    "Competition": (
        "Competition_ID",
        {
            "Competition_ID": "INTEGER", "Event_ID": "INTEGER",
            "Competition_Name": "TEXT", "Challenge": "TEXT",
            "Competition_Class": "TEXT", "Competition_Date": "TEXT",
            "Scoring_Model_Short_Name": "TEXT", "Entry_Time_Limit_Secs": "INTEGER",
            "Competition_Structure_Name": "TEXT", "Number_of_Runs_Allowed": "INTEGER",
            "Grace_Period_Secs": "INTEGER",
        },
        {"Competition_Date"},
    ),
    "Competition_Structure": (
        "ID",
        {"ID": "INTEGER", "Competition_Structure_Name": "TEXT"},
        set(),
    ),
    "Context": (
        "ID",
        {
            "ID": "INTEGER", "Effective_Date": "TEXT", "Current_Event": "TEXT",
            "Current_Event_ID": "INTEGER", "Current_Competition_Name": "TEXT",
            "Database_Version": "TEXT",
        },
        {"Effective_Date"},
    ),
    "Entry": (
        "Entry_ID",
        {
            "Entry_ID": "INTEGER", "Mouse_Name": "TEXT", "Competition_ID": "INTEGER",
            "Entry_Used": "INTEGER", "Sequence_Number": "INTEGER",
            "Registration_Source": "TEXT", "Outcome": "TEXT",
        },
        set(),
    ),
    "Entry_Run": (
        "Entry_Run_ID",
        {
            "Entry_Run_ID": "INTEGER", "Entry_ID": "INTEGER",
            "Run_Time_mSecs": "INTEGER", "Course_Time_mSecs": "INTEGER",
            "Touches": "INTEGER", "Score_Time_mSecs": "INTEGER",
            "Date_of_Run": "TEXT", "Time_of_Run": "TEXT", "Manual_Timing": "TEXT",
        },
        {"Date_of_Run", "Time_of_Run"},
    ),
    "ETL_Contestant": (
        None,
        {
            "Contestant_ID": "INTEGER", "Contestant_Name": "TEXT", "Class": "TEXT",
            "Contestant_Email_Address": "TEXT",
        },
        set(),
    ),
    "ETL_Entry": (
        None,
        {
            "Entry_ID": "INTEGER", "Mouse_Name": "TEXT", "Competition_ID": "INTEGER",
            "Entry_Used": "INTEGER", "Sequence_Number": "INTEGER",
        },
        set(),
    ),
    "ETL_Mouse": (
        None,
        {
            "Mouse_ID": "INTEGER", "Mouse_Name": "TEXT", "Contestant_ID": "INTEGER",
            "Further_Information": "TEXT",
        },
        set(),
    ),
    "Mouse": (
        "Mouse_ID",
        {
            "Mouse_ID": "INTEGER", "Mouse_Name": "TEXT", "Contestant_ID": "INTEGER",
            "Further_Information": "TEXT",
        },
        set(),
    ),
    "Robotics_Event": (
        "Event_ID",
        {
            "Event_ID": "INTEGER", "Event_Name": "TEXT", "Event_Date": "TEXT",
            "Database_Version": "TEXT",
        },
        {"Event_Date"},
    ),
    "Scoring_Model": (
        "ID",
        {
            "ID": "INTEGER", "Scoring_Model_Short_Name": "TEXT",
            "Scoring_Model_Name": "TEXT", "Start_Trigger": "TEXT",
            "End_Trigger": "TEXT", "Touches_Enabled": "INTEGER",
            "Touches_Cumulative": "INTEGER", "Touch_Time_mS": "INTEGER",
            "Entry_Time_Divider": "INTEGER", "Touch_Time_Divider": "INTEGER",
            "Touches_Per_Run": "INTEGER",
        },
        set(),
    ),
    "Contestant": (
        "Contestant_ID",
        {
            "Contestant_ID": "INTEGER", "Contestant_Name": "TEXT", "Class": "TEXT",
            "Contestant_Email_Address": "TEXT",
        },
        set(),
    ),
    "ETL_seeding_upload": (
        "ID",
        {
            "ID": "INTEGER", "Entry_ID": "INTEGER", "Mouse_Name": "TEXT",
            "Competition_ID": "INTEGER", "Sequence_Number": "INTEGER",
        },
        set(),
    ),
}


def convert_date(value: str) -> str | None:
    """mdb-export dates look like 'MM/DD/YY HH:MM:SS' -> ISO 'YYYY-MM-DD HH:MM:SS'."""
    if not value:
        return None
    dt = datetime.datetime.strptime(value, "%m/%d/%y %H:%M:%S")
    return dt.strftime("%Y-%m-%d %H:%M:%S")


def build():
    if DB_PATH.exists():
        DB_PATH.unlink()
    conn = sqlite3.connect(DB_PATH)
    cur = conn.cursor()
    cur.execute("PRAGMA foreign_keys = OFF")  # source has no declared FKs to preserve

    for table, (pk, columns, date_cols) in TABLES.items():
        col_defs = []
        for col, sqltype in columns.items():
            if pk and col == pk:
                col_defs.append(f'"{col}" {sqltype} PRIMARY KEY')
            else:
                col_defs.append(f'"{col}" {sqltype}')
        cur.execute(f'CREATE TABLE "{table}" ({", ".join(col_defs)})')

        csv_path = EXPORTS / f"{table}.csv"
        with open(csv_path, newline="", encoding="utf-8") as f:
            reader = csv.DictReader(f)
            rows = []
            for row in reader:
                values = []
                for col in columns:
                    raw = row.get(col, "")
                    if raw == "":
                        values.append(None)
                    elif col in date_cols:
                        values.append(convert_date(raw))
                    else:
                        values.append(raw)
                rows.append(values)

        if rows:
            placeholders = ", ".join("?" for _ in columns)
            col_list = ", ".join(f'"{c}"' for c in columns)
            cur.executemany(
                f'INSERT INTO "{table}" ({col_list}) VALUES ({placeholders})', rows
            )
        conn.commit()
        print(f"{table}: {len(rows)} rows")

    conn.close()
    print(f"\nBuilt {DB_PATH}")


if __name__ == "__main__":
    build()
