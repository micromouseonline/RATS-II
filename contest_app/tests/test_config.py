"""Unit tests for the per-user config file (APP-1.8.1)."""
from rats.config import AppConfig, load_config, save_config


def test_load_missing_file_returns_defaults(tmp_path):
    cfg = load_config(tmp_path / "does_not_exist" / "config.json")
    assert cfg == AppConfig()


def test_load_corrupt_json_returns_defaults(tmp_path):
    path = tmp_path / "config.json"
    path.write_text("{not valid json", encoding="utf-8")
    assert load_config(path) == AppConfig()


def test_load_non_object_json_returns_defaults(tmp_path):
    path = tmp_path / "config.json"
    path.write_text("[1, 2, 3]", encoding="utf-8")
    assert load_config(path) == AppConfig()


def test_save_then_load_round_trips(tmp_path):
    path = tmp_path / "nested" / "config.json"
    cfg = AppConfig(last_db_path="/some/event.db", last_port="COM3", last_baud=115200)

    save_config(cfg, path)
    loaded = load_config(path)

    assert loaded == cfg


def test_save_creates_parent_directory(tmp_path):
    path = tmp_path / "a" / "b" / "c" / "config.json"
    save_config(AppConfig(), path)
    assert path.exists()


def test_load_partial_file_fills_in_defaults(tmp_path):
    path = tmp_path / "config.json"
    path.write_text('{"last_port": "COM5"}', encoding="utf-8")

    cfg = load_config(path)

    assert cfg.last_port == "COM5"
    assert cfg.last_db_path is None
    assert cfg.last_baud == 9600
