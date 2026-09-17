using System;
using Microsoft.VisualBasic.CompilerServices;

namespace RATS;

[StandardModule]
internal sealed class Public_Variables
{
	public static string connString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=R:RATS_Competitions_be.accdb";

	public static int robotics_event_id = 0;

	public static string robotics_event = "";

	public static DateTime robotics_event_date;

	public static string public_competition_name = "";

	public static string public_competition_class = "";

	public static int competition_id = 0;

	public static int previous_competition_id = 0;

	public static int entry_id = 0;

	public static string robot = "";

	public static string contestant = "";

	public static string contestant_class = "";

	public static bool practice_mode = true;

	public static bool database_available = false;

	public static int split_time_ms = 0;

	public static int run_time_ms = 0;

	public static int maze_time_ms = 0;

	public static int timing_gates_state = 0;

	public static int score_time_ms = 0;

	public static int fastest_score_time_this_robot = -1;

	public static int robot_rank = 0;

	public static int no_of_touches = 0;

	public static int time_left_ms = 600000;

	public static int time_left_s = 600;

	public static int grace_period_s = 30;

	public static int grace_period_ms = 30000;

	public static int no_of_runs_used = 0;

	public static int no_of_runs_allowed = 5;

	public static bool single_channel_window_open = false;

	public static bool request_single_channel_window_close = false;

	public static bool refresh_robot_run_times = false;

	public static int refresh_run_times_delay = 0;

	public static bool refresh_best_score_times = false;

	public static int refresh_best_score_times_delay = 0;

	public static bool hide_robot_runtimes = false;

	public static bool hide_best_score_times = false;

	public static bool single_channel_results_window_open = false;

	public static bool request_single_channel_results_window_close = false;

	public static bool run_order_window_open = false;

	public static bool request_run_order_window_close = false;

	public static int refresh_run_order_delay = 0;

	public static bool STrigger = false;

	public static bool FTrigger = false;

	public static bool CTrigger = false;

	public static int SGLevel = 0;

	public static int SGPot = 0;

	public static int FGLevel = 0;

	public static int FGPot = 0;

	public static int SCLevel = 0;

	public static int SCPot = 0;

	public static bool calibration_window_open = false;

	public static bool request_calibration_close = false;

	public static bool request_calibration_mode = false;

	public static bool request_timer_mode = false;

	public static bool watchdog_active = true;

	public static bool watchdog_alarm = false;

	public static int watchdog_ms_since_reset = 0;

	public static object watchdog_alarm_repeat_counter = 201;

	public static bool serial_port_opened = false;
}
