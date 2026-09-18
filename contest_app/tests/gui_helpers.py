"""Shared helper for GUI tests (APP-1.8.1): skip cleanly when no display is
available, instead of erroring. First GUI test support in this suite --
every module up to this stage explicitly avoided importing `tkinter`
(`test_state.py`'s subprocess test even proves `rats.state` doesn't pull it
in as a side effect); `APP-1.8` is the first stage that needs a real Tk
root to test against.

Tests under this module construct `MainWindow` directly (it subclasses
`tk.Tk` and is its own root) -- `skip_if_no_display()` only probes whether
Tk can talk to a display at all, using a disposable root, before that.
"""
import tkinter as tk

import pytest


def skip_if_no_display() -> None:
    """`pytest.skip()` if no display is available (e.g. a headless CI box,
    per `PLAT-1`), otherwise return normally."""
    try:
        probe = tk.Tk()
    except tk.TclError as exc:
        pytest.skip(f"no display available for Tk: {exc}")
    else:
        probe.destroy()
