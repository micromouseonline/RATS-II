"""Small per-user config file (`APP-1.8.1`): remembers the last-opened DB
path, serial port, and baud rate across sessions.

Lives outside the repo in a per-user config directory (not project-local),
so it survives moving/reinstalling the checkout and needs no `.gitignore`
entry. A missing or corrupt file is never an error -- `load_config()` just
returns defaults, the same "fall back, don't raise" convention
`rats.db.select_contestant_for_mouse` already uses for a DB-lookup miss.
"""
from __future__ import annotations

import json
import os
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Optional

APP_DIR_NAME = "rats-contest-app"
CONFIG_FILE_NAME = "config.json"

DEFAULT_BAUD = 9600


def config_dir() -> Path:
    """Per-user config directory: `%APPDATA%\\rats-contest-app` on Windows,
    `~/.config/rats-contest-app` elsewhere (no `platformdirs` dependency --
    this is the one env var/fallback branch that actually differs)."""
    if os.name == "nt":
        base = os.environ.get("APPDATA")
        if base:
            return Path(base) / APP_DIR_NAME
    return Path.home() / ".config" / APP_DIR_NAME


def config_path() -> Path:
    return config_dir() / CONFIG_FILE_NAME


@dataclass
class AppConfig:
    last_db_path: Optional[str] = None
    last_port: Optional[str] = None
    last_baud: int = DEFAULT_BAUD
    # Saved on every close, but not yet read back on startup --
    # `main_window.RESTORE_WINDOW_SIZE` gates that half of the feature off
    # for now (see its docstring). Kept here so the data is already being
    # collected once that's switched on.
    last_window_width: Optional[int] = None
    last_window_height: Optional[int] = None


def load_config(path: Optional[Path] = None) -> AppConfig:
    """Missing file, unreadable file, or malformed JSON all fall back to
    defaults rather than raising -- a bad config file should never stop the
    app from starting."""
    target = path if path is not None else config_path()
    try:
        raw = target.read_text(encoding="utf-8")
        data = json.loads(raw)
    except (OSError, ValueError):
        return AppConfig()
    if not isinstance(data, dict):
        return AppConfig()
    defaults = AppConfig()
    return AppConfig(
        last_db_path=data.get("last_db_path", defaults.last_db_path),
        last_port=data.get("last_port", defaults.last_port),
        last_baud=data.get("last_baud", defaults.last_baud),
        last_window_width=data.get("last_window_width", defaults.last_window_width),
        last_window_height=data.get("last_window_height", defaults.last_window_height),
    )


def save_config(cfg: AppConfig, path: Optional[Path] = None) -> None:
    target = path if path is not None else config_path()
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_text(json.dumps(asdict(cfg), indent=2), encoding="utf-8")
