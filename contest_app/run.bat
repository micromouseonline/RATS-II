@echo off
REM Sets up contest_app\.venv on first run (or after a dependency change),
REM then launches the timing app.
cd /d "%~dp0"

if not exist .venv (
    python -m venv .venv
)
.venv\Scripts\pip install -q -e .
.venv\Scripts\python -m rats.main_window
