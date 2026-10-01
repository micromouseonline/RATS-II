# RATS-II

An open-source, cross-platform rewrite of RATS (Registration And Timing
System) — software for running robotics competitions such as maze-solving,
line-following, and wall-following challenges (the data this project was
built against shows UK robotics events, e.g. `UKMARS Spring 2024`, and
scoring models like `UKMaze`/`UKNCWF`/`UKM19`).

## What RATS does

The original RATS is actually two cooperating applications sharing one
database:

- **A timing app** — connects to timing-gate hardware over serial during a
  contest, times runs, calculates scores, and displays live results to
  competitors and the audience.
- **A registration/management app** — everything around a contest: setting
  up events, registering robots and contestants, sequencing entries,
  correcting scores, and producing results reports and exports.

## Why rewrite it

The original system depends entirely on Windows: the timing app is a
WinForms executable, and the management app requires a **full** Microsoft
Access installation (not just the free runtime) on whatever machine runs
event registration. The two talk to each other only through a database file
reached via a mapped network drive. None of this is necessary to solve the
actual problem, and it makes the software hard to run, maintain, or extend
on anything but a specific Windows setup.

RATS-II rewrites both applications in Python — portable across Linux,
macOS, and Windows, built on open-source tools throughout (SQLite instead
of Access, Tkinter instead of WinForms), with no mapped drives, no VBA, and
no Windows-only dependencies. The goal is a system any club running these
competitions can install and run anywhere, and that's actually maintainable
by people who aren't Access/VBA specialists.

## What's in this repository

- **`legacy/`** — the original RATS system: decompiled source, the Access
  database, user documentation, and our own analysis of how it actually
  behaves. Reference material only; nothing here is built or run as part
  of the new system.
- **`contest_app/`** — the new timing application (in progress).
- **`management_app/`** — the new registration/management application (not
  started yet).
- **`live_data/`** — real competition data, git-ignored and never shared.
- **`sample_data/`** — a demo database derived from actual contest data, with
  email addresses anonymized (everything else — names, robots, results —
  is real, since it's already public via published contest results). Safe
  to commit and share; both apps default to opening it.

## Running the contest timing app

`contest_app` requires Python 3.10+ and Tkinter (bundled with the official
Python installer on Windows/macOS; on Linux it's usually a separate
package — see below). There's no release build yet, so it's run from
source into a local virtual environment (`contest_app/.venv`, created
automatically, git-ignored).

The quickest way is the bundled script, which creates/updates the venv and
launches the app in one step. From the repository root:

**Linux / macOS**
```sh
./contest_app/run.sh
```
If Tkinter isn't already installed, install it first (one-off, needs
sudo): Debian/Ubuntu `sudo apt install python3-tk`, Fedora
`sudo dnf install python3-tkinter`, Arch `sudo pacman -S tk`, macOS
(Homebrew Python) `brew install python-tk`.

**Windows**
```bat
contest_app\run.bat
```
(Double-clicking `run.bat` in File Explorer also works.) Tkinter is
included with the standard python.org installer, so no extra install step
is needed.

Equivalent manual steps, if you'd rather manage the virtual environment
yourself (same on every platform; use `.venv\Scripts\` instead of
`.venv/bin/` on Windows):
```sh
cd contest_app
python3 -m venv .venv
.venv/bin/pip install -e .
.venv/bin/python -m rats.main_window
```

The app opens with no database loaded — use its **File > Open Database...**
to pick one. `sample_data/demo.db` (at the repository root) is a safe demo
database to start with; `live_data/rats.db`, if you have it, is real
competition data and is git-ignored.

## Running the tests

Same idea, via the matching test script:

**Linux / macOS**
```sh
./contest_app/run_tests.sh
```

**Windows**
```bat
contest_app\run_tests.bat
```

Both forward extra arguments to `pytest`, e.g. `./contest_app/run_tests.sh -k serial`.

Manual equivalent:
```sh
cd contest_app
python3 -m venv .venv
.venv/bin/pip install -e '.[dev]'
.venv/bin/pytest
```

`management_app` has no code yet (phase 2, not started), so there's
nothing to run or test there.

## Status

Early stage. The legacy system has been fully analyzed and documented, and
the design decisions for the timing app are made — see `todo.md` for the
live plan and progress, and `CLAUDE.md` for a detailed technical map of the
repository. There is no working application yet.

## Acknowledgments

The original RATS applications this project rewrites were the work of Ian
Butterworth, for UKMARS.

## License

MIT — see [LICENSE](LICENSE).
