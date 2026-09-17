"""Unit tests for the serial frame parser (APP-1.5)."""
from rats.serial_protocol import (
    Line,
    Message,
    format_new_mouse,
    format_set_mode,
    parse_line,
    parse_message,
    read_messages,
    split_lines,
)


# --- Line splitting / buffering ---------------------------------------


def test_line_split_across_two_reads():
    lines, remainder = read_messages("<13,926")
    assert lines == []
    assert remainder == "<13,926"

    lines, remainder = read_messages(remainder + "0>\n")
    assert lines == [Line("<13,9260>", Message(13, 9260))]
    assert remainder == ""


def test_multiple_lines_in_one_read():
    lines, remainder = read_messages("<0,0>\n<4,4>\n<13,9260>\n")
    assert [l.message for l in lines] == [Message(0, None), Message(4, 4), Message(13, 9260)]
    assert remainder == ""


def test_crlf_terminated_lines_are_handled():
    lines, remainder = read_messages("<13,9260>\r\n")
    assert lines == [Line("<13,9260>", Message(13, 9260))]
    assert remainder == ""


def test_incomplete_trailing_line_is_kept_as_remainder():
    lines, remainder = read_messages("<13,9260>\n<4,")
    assert [l.message for l in lines] == [Message(13, 9260)]
    assert remainder == "<4,"


# --- The two behaviors from the user's correction ----------------------


def test_a_line_not_starting_with_less_than_is_totally_ignored():
    # Even though a valid-looking frame appears later in the same line,
    # it must NOT be recovered -- the whole line is garbage because its
    # first character isn't '<'.
    lines, remainder = read_messages("noise<13,9260>\n")
    assert lines == [Line("noise<13,9260>", None)]
    assert remainder == ""


def test_garbage_line_followed_by_a_valid_line_parses_the_valid_one():
    lines, remainder = read_messages("junk\n<13,9260>\n")
    assert lines == [Line("junk", None), Line("<13,9260>", Message(13, 9260))]
    assert remainder == ""


def test_characters_after_the_closing_bracket_are_ignored():
    lines, remainder = read_messages("<13,9260>trailing junk\n")
    assert lines == [Line("<13,9260>trailing junk", Message(13, 9260))]
    assert remainder == ""


def test_every_raw_line_is_preserved_for_logging_even_when_unparseable():
    # A malformed line (starts with '<' but never closes) must still show
    # up in the returned Line list with its raw text, for the caller to
    # log -- even though message is None.
    lines, remainder = read_messages("<not a real frame\n<13,9260>\n")
    assert lines == [
        Line("<not a real frame", None),
        Line("<13,9260>", Message(13, 9260)),
    ]
    assert remainder == ""


def test_out_of_scope_code_line_is_logged_but_produces_no_message():
    # 11 = C1Reaction, real Annex A code but out of scope (Q-3).
    lines, remainder = read_messages("<11,42>\n")
    assert lines == [Line("<11,42>", None)]
    assert remainder == ""


def test_blank_line_is_ignored_but_still_present_for_logging():
    lines, remainder = read_messages("\n<13,9260>\n")
    assert lines == [Line("", None), Line("<13,9260>", Message(13, 9260))]


def test_unparseable_code_is_ignored():
    assert parse_message("abc", "0") is None


def test_lenient_value_parsing_defaults_to_zero_for_garbage():
    assert parse_message("4", "not-a-number") == Message(4, 0)


def test_parse_line_directly():
    assert parse_line("<13,9260>") == Message(13, 9260)
    assert parse_line("garbage") is None
    assert parse_line("<13,9260") is None  # no closing '>'


def test_split_lines_directly():
    lines, remainder = split_lines("<0,0>\n<4,4>\npartial")
    assert lines == ["<0,0>", "<4,4>"]
    assert remainder == "partial"


# --- Per-code value interpretation (SER-1's 19 inbound codes) --------


def test_watch_dog_code_0():
    assert parse_message("0", "0") == Message(0, None)


def test_course_time_code_1_scales_by_100():
    assert parse_message("1", "12.3") == Message(1, 1230)


def test_split_time_code_2_scales_by_10():
    assert parse_message("2", "45.6") == Message(2, 456)


def test_run_time_code_3_scales_by_10():
    assert parse_message("3", "78.9") == Message(3, 789)


def test_timer_state_code_4_no_scaling():
    assert parse_message("4", "4") == Message(4, 4)


def test_run_time_ms_code_5_no_scaling():
    assert parse_message("5", "5000") == Message(5, 5000)


def test_split_to_run_code_6_no_scaling():
    assert parse_message("6", "0") == Message(6, 0)


def test_c1_split_time_code_12_no_scaling():
    assert parse_message("12", "1500") == Message(12, 1500)


def test_c1_run_time_code_13_no_scaling():
    assert parse_message("13", "9260") == Message(13, 9260)


def test_course_time_ms_code_30_no_scaling():
    assert parse_message("30", "30000") == Message(30, 30000)


def test_s_trigger_code_71_true_only_for_on_or_1():
    assert parse_message("71", "ON") == Message(71, True)
    assert parse_message("71", "1") == Message(71, True)
    assert parse_message("71", "OFF") == Message(71, False)
    assert parse_message("71", "0") == Message(71, False)


def test_f_trigger_code_72_true_only_for_on_or_1():
    assert parse_message("72", "ON") == Message(72, True)
    assert parse_message("72", "0") == Message(72, False)


def test_c_trigger_code_73_true_only_for_on_or_1():
    assert parse_message("73", "1") == Message(73, True)
    assert parse_message("73", "OFF") == Message(73, False)


def test_sg_level_code_81():
    assert parse_message("81", "512") == Message(81, 512)


def test_sg_pot_code_82():
    assert parse_message("82", "200") == Message(82, 200)


def test_fg_level_code_83():
    assert parse_message("83", "600") == Message(83, 600)


def test_fg_pot_code_84():
    assert parse_message("84", "210") == Message(84, 210)


def test_sc_level_code_85():
    assert parse_message("85", "700") == Message(85, 700)


def test_sc_pot_code_86():
    assert parse_message("86", "220") == Message(86, 220)


# --- Outbound framing --------------------------------------------------


def test_format_new_mouse():
    assert format_new_mouse() == "<98,0>\n"


def test_format_set_mode():
    assert format_set_mode("CALIBRATION") == "<99,CALIBRATION>\n"
    assert format_set_mode("TIMER") == "<99,TIMER>\n"


def test_format_set_mode_rejects_unknown_mode():
    import pytest

    with pytest.raises(ValueError):
        format_set_mode("NOT_A_MODE")
