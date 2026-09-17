"""Dump the SQL text of every saved Access query via ADOX (COM).

Plain ODBC exposes saved queries as views/procedures by name only, not their
SQL. ADOX's Catalog object gives access to that SQL text. Requires the ACE
OLEDB provider (same one RATS uses) and pywin32 -- not full MS Access.

Run on Windows:

    python query_sql_dump.py "R:\\RATS_Competitions_be.accdb"

Writes queries.json into the current directory.
"""
import json
import sys

import win32com.client


def main():
    if len(sys.argv) != 2:
        print("usage: query_sql_dump.py <path-to.accdb>")
        sys.exit(1)

    db_path = sys.argv[1]
    conn_str = (
        "Provider=Microsoft.ACE.OLEDB.12.0;"
        f"Data Source={db_path};"
    )

    catalog = win32com.client.Dispatch("ADOX.Catalog")
    catalog.ActiveConnection = conn_str

    queries = {}

    # Select queries show up as Views.
    for view in catalog.Views:
        try:
            sql = view.Command.CommandText
        except Exception as exc:  # noqa: BLE001 - just record and move on
            sql = f"<could not read: {exc}>"
        queries[view.Name] = {"kind": "view (select query)", "sql": sql}

    # Action queries (append/update/delete/make-table) show up as Procedures.
    for proc in catalog.Procedures:
        try:
            sql = proc.Command.CommandText
        except Exception as exc:  # noqa: BLE001
            sql = f"<could not read: {exc}>"
        queries[proc.Name] = {"kind": "procedure (action query)", "sql": sql}

    with open("queries.json", "w", encoding="utf-8") as f:
        json.dump(queries, f, indent=2)

    print(f"Found {len(queries)} saved queries. Wrote queries.json")


if __name__ == "__main__":
    main()
