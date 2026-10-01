#!/usr/bin/env bash
# Sets up contest_app/.venv on first run (or after a dependency change),
# then runs the test suite. Works on Linux and macOS.
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")"

if [ ! -d .venv ]; then
    python3 -m venv .venv
fi
.venv/bin/pip install -q -e '.[dev]'
.venv/bin/pytest "$@"
