# Access DB extraction tools

Run these **on the Windows machine** that has the Access Database Engine
(ACE) driver installed — the same driver the RATS exe uses via
`Microsoft.ACE.OLEDB.12.0`. Full MS Access is not required.

## Setup

```
pip install pyodbc pywin32
```

Confirm the ODBC driver is present:
```
python -c "import pyodbc; print([d for d in pyodbc.drivers() if 'Access' in d])"
```
You should see `Microsoft Access Driver (*.mdb, *.accdb)`. If it's missing,
install the "Access Database Engine 2016 Redistributable" matching your
Python's bitness (32-bit Python needs the 32-bit engine, etc).

## Scripts

Run in this order, pointing each at the real `.accdb` path:

1. **`schema_dump.py <path-to.accdb>`**
   Dumps tables, columns (name/type/size/nullable), primary keys, indexes,
   and declared relationships (foreign keys) to `schema.json` and a
   human-readable `schema.md` in the current directory.

2. **`query_sql_dump.py <path-to.accdb>`**
   Uses ADOX (via COM/`pywin32`) to pull the actual SQL text of every saved
   Access query (select queries show up as ADOX Views, action queries as
   Procedures) into `queries.json`. This is the only step needing COM —
   plain ODBC has no catalog call for "give me this view's SQL".

3. **`export_data_to_sqlite.py <path-to.accdb> <output.sqlite>`**
   Reads every user table's rows via ODBC and writes them into a fresh
   SQLite database, applying the Access→SQLite type mapping documented in
   the script header. This becomes the dev/test seed data for the Python
   port.

Copy the three output files (`schema.json`, `schema.md`, `queries.json`,
and the `.sqlite` file) back into the Linux dev environment — they don't
need to be committed as-is (the `.sqlite` in particular may contain real
competitor data), but `schema.md` and `queries.json` should be reviewed and
folded into proper schema documentation for the port.

## What's NOT covered here

Access **report** layouts aren't extractable this way — reading a report's
definition requires the Access application object model (or at minimum the
Access runtime), which isn't installed. If report layouts turn out to
matter for the Tkinter UI, that needs either installing the free Access
Runtime, or just visually inspecting the reports on a machine that has full
Access.
