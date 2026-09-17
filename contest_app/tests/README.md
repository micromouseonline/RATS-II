# Tests

pytest suite for the `contest_app/src/rats` package.

Setup (from `contest_app/`):

```
python3 -m venv .venv
.venv/bin/pip install -e '.[dev]'
.venv/bin/pytest
```

No real pytest tests yet (`APP-1.1` — project scaffolding only); `pytest`
currently collects 0 items, which is expected. `check_db5_rewrites.py` is a
standalone `unittest` script, not yet pytest-discoverable (see its
docstring) — it gets folded into the real suite in `APP-1.2`/`APP-1.4`, see
`../../todo.md`.
