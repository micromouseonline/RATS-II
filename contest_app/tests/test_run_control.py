"""Tests for APP-1.8.3: run-control buttons + live display wiring.

Button handlers are invoked directly (`window._on_touch_clicked()` etc.)
rather than via `.invoke()`/simulated clicks -- same rationale as
test_event_entry_selection.py: this exercises the same code a real click
would run, without depending on Tk's event queue draining in a headless
test run. `messagebox.askyesno` is monkeypatched for the two confirm-dialog
buttons (DNF, New Mouse) rather than driven interactively.
"""
from rats import main_window

from gui_helpers import skip_if_no_display


def test_entry_pane_starts_disabled_in_practice_mode(demo_db, make_main_window):
    """`practice_mode` defaults `true` (`AppState.entry`), so the entry
    pane starts disabled until Practice Mode is toggled off -- a real
    behavior change from `.2` alone, where nothing gated it yet."""
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window.app_state.entry.practice_mode is True
        assert str(window.competition_class_combo["state"]) == "disabled"
        assert "disabled" in window.competition_tree.state()
        assert "disabled" in window.entry_tree.state()
    finally:
        window.destroy()


def test_practice_mode_toggle_enables_entry_pane(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()

        assert window.app_state.entry.practice_mode is False
        assert str(window.competition_class_combo["state"]) == "readonly"
        assert "disabled" not in window.competition_tree.state()
        assert "disabled" not in window.entry_tree.state()

        window._on_practice_mode_clicked()
        assert window.app_state.entry.practice_mode is True
        assert str(window.competition_class_combo["state"]) == "disabled"
    finally:
        window.destroy()


def test_touch_button_increments_touch_count_and_display(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_touch_clicked()
        window._on_touch_clicked()

        assert window.app_state.run.no_of_touches == 2
        assert window.touches_var.get() == "2"
    finally:
        window.destroy()


def test_clear_button_resets_run_state_and_display(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.app_state.run.no_of_touches = 5
        window.app_state.run.score_time_ms = 1234

        window._on_clear_clicked()

        assert window.app_state.run.no_of_touches == 0
        assert window.touches_var.get() == "0"
        assert window.score_time_var.get() == "0.000"
    finally:
        window.destroy()


def test_extra_run_button_decrements_runs_used(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.app_state.run.no_of_runs_used = 2
        window._on_extra_run_clicked()
        assert window.app_state.run.no_of_runs_used == 1
        assert window.run_number_var.get() == "1"
    finally:
        window.destroy()


def test_watchdog_button_toggles_state_and_caption(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window.app_state.watchdog.watchdog_active is True
        assert window.watchdog_button["text"] == "WatchDog is On"

        window._on_watchdog_clicked()
        assert window.app_state.watchdog.watchdog_active is False
        assert window.watchdog_button["text"] == "WatchDog is Off"

        window._on_watchdog_clicked()
        assert window.app_state.watchdog.watchdog_active is True
        assert window.watchdog_button["text"] == "WatchDog is On"
    finally:
        window.destroy()


def test_dnf_does_nothing_in_practice_mode_without_a_dialog(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        assert window.app_state.entry.practice_mode is True
        asked = []
        monkeypatch.setattr(main_window.messagebox, "askyesno", lambda *a, **k: asked.append(1) or True)

        window._on_dnf_clicked()

        assert asked == []
    finally:
        window.destroy()


def test_dnf_confirmed_marks_entry_retired_and_refreshes_tree(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()  # leave practice mode
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        competition_id = int(window.competition_tree.get_children()[0])
        window.competition_tree.selection_set(str(competition_id))
        window._on_competition_selected()
        entry_id = int(window.entry_tree.get_children()[0])
        window.entry_tree.selection_set(str(entry_id))
        window._on_entry_selected()

        monkeypatch.setattr(main_window.messagebox, "askyesno", lambda *a, **k: True)
        window._on_dnf_clicked()

        assert str(entry_id) not in window.entry_tree.get_children()
    finally:
        window.destroy()


def test_dnf_declined_leaves_entry_untouched(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        competition_id = int(window.competition_tree.get_children()[0])
        window.competition_tree.selection_set(str(competition_id))
        window._on_competition_selected()
        entry_id = int(window.entry_tree.get_children()[0])
        window.entry_tree.selection_set(str(entry_id))
        window._on_entry_selected()

        monkeypatch.setattr(main_window.messagebox, "askyesno", lambda *a, **k: False)
        window._on_dnf_clicked()

        assert str(entry_id) in window.entry_tree.get_children()
    finally:
        window.destroy()


def test_new_mouse_confirmed_clears_selection_labels(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        competition_id = int(window.competition_tree.get_children()[0])
        window.competition_tree.selection_set(str(competition_id))
        window._on_competition_selected()
        entry_id = int(window.entry_tree.get_children()[0])
        window.entry_tree.selection_set(str(entry_id))
        window._on_entry_selected()
        assert window.selected_robot_var.get() != "Robot: --"

        monkeypatch.setattr(main_window.messagebox, "askyesno", lambda *a, **k: True)
        window._on_new_mouse_clicked()

        assert window.selected_robot_var.get() == "Robot: --"
        # core.new_mouse() sets contestant to legacy's "_" placeholder, not
        # blank -- displayed literally, matching .2's convention for it.
        assert window.current_contestant_var.get() == "Contestant: _"
    finally:
        window.destroy()


def test_new_mouse_declined_leaves_selection_alone(demo_db, make_main_window, monkeypatch):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window._on_practice_mode_clicked()
        window.competition_class_var.set("Final")
        window._on_competition_class_selected()
        competition_id = int(window.competition_tree.get_children()[0])
        window.competition_tree.selection_set(str(competition_id))
        window._on_competition_selected()
        entry_id = int(window.entry_tree.get_children()[0])
        window.entry_tree.selection_set(str(entry_id))
        window._on_entry_selected()
        robot_before = window.selected_robot_var.get()

        monkeypatch.setattr(main_window.messagebox, "askyesno", lambda *a, **k: False)
        window._on_new_mouse_clicked()

        assert window.selected_robot_var.get() == robot_before
    finally:
        window.destroy()


def test_refresh_live_display_formats_values_from_state(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        run = window.app_state.run
        run.split_time_ms = 1230
        run.maze_time_ms = 4560
        run.run_time_ms = 7891
        run.score_time_ms = 1000
        run.fastest_score_time_this_robot = 2500
        run.robot_rank = 3
        run.time_left_ms = 59990
        run.no_of_runs_used = 2
        run.no_of_runs_allowed = 5
        run.no_of_touches = 4
        run.timing_gates_state = 4

        window._refresh_live_display()

        assert window.split_time_var.get() == "1.23"
        assert window.maze_time_var.get() == "4.56"
        assert window.run_time_var.get() == "7.891"
        assert window.score_time_var.get() == "1.000"
        assert window.best_score_var.get() == "2.500"
        assert window.rank_var.get() == "3"
        assert window.time_left_var.get() == "59.99"
        assert window.run_number_var.get() == "2"
        assert window.allowed_runs_var.get() == "5"
        assert window.touches_var.get() == "4"
        assert window.timer_state_var.get() == "4 - Run in progress"
        assert window.watchdog_state_var.get() == ""
    finally:
        window.destroy()


def test_refresh_live_display_shows_error_on_watchdog_alarm(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.app_state.watchdog.watchdog_alarm = True
        window._refresh_live_display()
        assert window.watchdog_state_var.get() == "Error"
    finally:
        window.destroy()


def test_tick_interpolation_reflects_in_live_display(demo_db, make_main_window):
    skip_if_no_display()
    window = make_main_window(demo_db)
    try:
        window.app_state.run.split_time_running = True
        window.core.tick(500)
        window._refresh_live_display()
        assert window.split_time_var.get() == "0.50"
    finally:
        window.destroy()
