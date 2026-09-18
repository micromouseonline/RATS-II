"""Tests for APP-1.8.2: event/competition/entry selection wiring.

Handlers are called directly (`window._on_competition_class_selected()`
etc.) rather than via simulated Tk events -- Treeview/Combobox selection
state is already set synchronously by `selection_set()`/the StringVar, so
this exercises the same code the real `<<TreeviewSelect>>`/
`<<ComboboxSelected>>` bindings call, without depending on Tk's event
queue actually draining in a headless test run.
"""
import shutil

from rats import db, main_window
from rats.main_window import MainWindow

from dbfixture import DEMO_DB
from gui_helpers import skip_if_no_display


def _make_window(conn):
    return MainWindow(conn=conn, open_serial_port_fn=lambda port, baud: None)


def _first_final_competition_with_entries(conn, event_id):
    for competition in db.select_competitions(conn, event_id, "Final"):
        if db.select_pending_entries(conn, competition.competition_id):
            return competition
    raise AssertionError("sample_data/demo.db should have a Final competition with pending entries")


def test_event_context_loaded_on_startup(demo_db):
    skip_if_no_display()
    context = db.get_context(demo_db)
    window = _make_window(demo_db)
    try:
        assert window.app_state.event.robotics_event_id == context.current_event_id
        assert window.event_name_var.get() == f"Event: {context.current_event}"
    finally:
        window.destroy()


def test_selecting_class_populates_competition_tree(demo_db):
    skip_if_no_display()
    window = _make_window(demo_db)
    try:
        event_id = window.app_state.event.robotics_event_id
        expected = db.select_competitions(demo_db, event_id, "Final")
        assert expected

        window.competition_class_var.set("Final")
        window._on_competition_class_selected()

        tree_ids = set(window.competition_tree.get_children())
        assert tree_ids == {str(c.competition_id) for c in expected}
    finally:
        window.destroy()


def test_selecting_competition_updates_core_state_and_entry_tree(demo_db):
    skip_if_no_display()
    window = _make_window(demo_db)
    try:
        event_id = window.app_state.event.robotics_event_id
        competition = _first_final_competition_with_entries(demo_db, event_id)

        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        window.competition_tree.selection_set(str(competition.competition_id))
        window._on_competition_selected()

        assert window.app_state.event.competition_id == competition.competition_id
        assert window.selected_competition_var.get() == f"Selected: {competition.competition_name}"

        expected_entries = db.select_pending_entries(demo_db, competition.competition_id)
        tree_ids = set(window.entry_tree.get_children())
        assert tree_ids == {str(e.entry_id) for e in expected_entries}
    finally:
        window.destroy()


def test_selecting_entry_updates_core_state_and_labels(demo_db):
    skip_if_no_display()
    window = _make_window(demo_db)
    try:
        event_id = window.app_state.event.robotics_event_id
        competition = _first_final_competition_with_entries(demo_db, event_id)
        entry = db.select_pending_entries(demo_db, competition.competition_id)[0]

        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        window.competition_tree.selection_set(str(competition.competition_id))
        window._on_competition_selected()
        window.entry_tree.selection_set(str(entry.entry_id))
        window._on_entry_selected()

        assert window.app_state.entry.entry_id == entry.entry_id
        assert window.app_state.entry.robot == entry.mouse_name
        assert window.selected_robot_var.get() == f"Robot: {entry.mouse_name}"
        assert window.current_contestant_var.get() == f"Contestant: {window.app_state.entry.contestant}"
    finally:
        window.destroy()


def test_changing_class_resets_downstream_selection(demo_db):
    skip_if_no_display()
    window = _make_window(demo_db)
    try:
        event_id = window.app_state.event.robotics_event_id
        competition = _first_final_competition_with_entries(demo_db, event_id)
        entry = db.select_pending_entries(demo_db, competition.competition_id)[0]

        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        window.competition_tree.selection_set(str(competition.competition_id))
        window._on_competition_selected()
        window.entry_tree.selection_set(str(entry.entry_id))
        window._on_entry_selected()

        window.competition_class_var.set("Heats")
        window._on_competition_class_selected()

        assert window.selected_competition_var.get() == "Selected: --"
        assert window.selected_robot_var.get() == "Robot: --"
        assert window.current_contestant_var.get() == "Contestant: --"
        assert window.entry_tree.get_children() == ()
    finally:
        window.destroy()


def test_open_database_resets_and_reloads_event_context(demo_db, monkeypatch, tmp_path):
    skip_if_no_display()
    window = _make_window(demo_db)
    try:
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        assert window.competition_tree.get_children()  # sanity: something got populated

        other_db_path = tmp_path / "other.db"
        shutil.copy(DEMO_DB, other_db_path)
        monkeypatch.setattr(
            main_window.filedialog, "askopenfilename", lambda **kwargs: str(other_db_path)
        )

        window._on_open_database()

        assert window.competition_class_var.get() == ""
        assert window.competition_tree.get_children() == ()
        assert window.selected_competition_var.get() == "Selected: --"
        assert window.event_name_var.get() != "Event: --"
    finally:
        window.destroy()
