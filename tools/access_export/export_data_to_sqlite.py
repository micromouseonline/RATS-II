"""Export every user table's data from an Access .accdb into a new SQLite file.

Run on Windows with the Access Database Engine (ACE) driver installed:

    python export_data_to_sqlite.py "R:\\RATS_Competitions_be.accdb" rats_dev.sqlite

Type mapping (Access ODBC type_name -> SQLite):
    COUNTER (AutoNumber)          -> INTEGER PRIMARY KEY
    BYTE, INTEGER, SMALLINT, LONG -> INTEGER
    BIT (Yes/No)                  -> INTEGER (0/1)
    SINGLE, DOUBLE, CURRENCY,
      DECIMAL, NUMERIC            -> REAL
    DATETIME                      -> TEXT, ISO 8601 (YYYY-MM-DD HH:MM:SS)
    VARCHAR, LONGCHAR, CHAR       -> TEXT
    LONGBINARY, VARBINARY,
      OLEOBJECT                   -> BLOB (copied as-is; verify size before
                                      relying on this for real attachments)
    anything else                 -> TEXT (fallback, printed as a warning)
"""
import datetime
import sqlite3
import sys

import pyodbc

TYPE_MAP = {
    "COUNTER": "INTEGER",
    "BYTE": "INTEGER",
    "INTEGER": "INTEGER",
    "SMALLINT": "INTEGER",
    "LONG": "INTEGER",
    "BIT": "INTEGER",
    "SINGLE": "REAL",
    "DOUBLE": "REAL",
    "CURRENCY": "REAL",
    "DECIMAL": "REAL",
    "NUMERIC": "REAL",
    "DATETIME": "TEXT",
    "VARCHAR": "TEXT",
    "LONGCHAR": "TEXT",
    "CHAR": "TEXT",
    "LONGBINARY": "BLOB",
    "VARBINARY": "BLOB",
    "OLEOBJECT": "BLOB",
}

SYSTEM_TABLE_PREFIXES = ("MSys",)


def connect_access(db_path):
    conn_str = (
        r"DRIVER={Microsoft Access Driver (*.mdb, *.accdb)};"
        rf"DBQ={db_path};"
    )
    return pyodbc.connect(conn_str, autocommit=True)


def sqlite_type_for(access_type):
    mapped = TYPE_MAP.get(access_type.upper())
    if mapped is None:
        print(f"  warning: unmapped Access type {access_type!r}, defaulting to TEXT")
        return "TEXT"
    return mapped


def convert_value(value):
    if isinstance(value, (datetime.date, datetime.datetime)):
        return value.isoformat(sep=" ")
    if isinstance(value, bool):
        return int(value)
    return value


def export_table(access_cur, sqlite_conn, table_name):
    columns = list(access_cur.columns(table=table_name))
    columns.sort(key=lambda c: c.ordinal_position)

    pk_cols = {row.column_name for row in access_cur.primaryKeys(table=table_name)}
    single_pk = pk_cols.pop() if len(pk_cols) == 1 else None

    col_defs = []
    for col in columns:
        sqlite_type = sqlite_type_for(col.type_name)
        if single_pk and col.column_name == single_pk and sqlite_type == "INTEGER":
            col_defs.append(f'"{col.column_name}" INTEGER PRIMARY KEY')
        else:
            col_defs.append(f'"{col.column_name}" {sqlite_type}')

    create_sql = f'CREATE TABLE "{table_name}" ({", ".join(col_defs)});'
    sqlite_conn.execute(create_sql)

    col_names = [c.column_name for c in columns]
    quoted_col_names = ", ".join(f'"{n}"' for n in col_names)
    placeholders = ", ".join("?" for _ in col_names)
    insert_sql = f'INSERT INTO "{table_name}" ({quoted_col_names}) VALUES ({placeholders});'

    access_cur.execute(f'SELECT * FROM "{table_name}"')
    rows = access_cur.fetchall()
    converted = [tuple(convert_value(v) for v in row) for row in rows]
    if converted:
        sqlite_conn.executemany(insert_sql, converted)

    print(f"  {table_name}: {len(converted)} rows")


def main():
    if len(sys.argv) != 3:
        print("usage: export_data_to_sqlite.py <path-to.accdb> <output.sqlite>")
        sys.exit(1)

    access_path, sqlite_path = sys.argv[1], sys.argv[2]

    access_conn = connect_access(access_path)
    access_cur = access_conn.cursor()

    sqlite_conn = sqlite3.connect(sqlite_path)

    table_names = [
        row.table_name
        for row in access_cur.tables(tableType="TABLE")
        if not row.table_name.startswith(SYSTEM_TABLE_PREFIXES)
    ]

    print(f"Exporting {len(table_names)} tables to {sqlite_path}")
    for name in table_names:
        export_table(access_cur, sqlite_conn, name)

    sqlite_conn.commit()
    sqlite_conn.close()
    print("Done.")


if __name__ == "__main__":
    main()
