using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Devices;
using RATS.My;

namespace RATS;

[DesignerGenerated]
public class Form1 : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("comPort_ComboBox")]
	private ComboBox _comPort_ComboBox;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("clear_BTN")]
	private Button _clear_BTN;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("connect_BTN")]
	private Button _connect_BTN;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Timer1")]
	private Timer _Timer1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Competition_DataGridView")]
	private DataGridView _Competition_DataGridView;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("New_Mouse_Button")]
	private Button _New_Mouse_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Competition_Class_ComboBox")]
	private ComboBox _Competition_Class_ComboBox;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Mouse_DataGridView")]
	private DataGridView _Mouse_DataGridView;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Selected_Robot")]
	private Label _Selected_Robot;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Touch_Button")]
	private Button _Touch_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Monitor_Button")]
	private Button _Monitor_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Verbose_Button")]
	private Button _Verbose_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("DNF_Button")]
	private Button _DNF_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Practice_Mode_Button")]
	private Button _Practice_Mode_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Calibrate_Button")]
	private Button _Calibrate_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Clear_Button")]
	private Button _Clear_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Display_1_channel_button")]
	private Button _Display_1_channel_button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Results_1ch_button")]
	private Button _Results_1ch_button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Run_order_Button")]
	private Button _Run_order_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Name_contestants_Button")]
	private Button _Name_contestants_Button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Display_1ch_v2_button")]
	private Button _Display_1ch_v2_button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Display_1ch_v2wide_button")]
	private Button _Display_1ch_v2wide_button;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("ExtraRunButton")]
	private Button _ExtraRunButton;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("WatchDogButton")]
	private Button _WatchDogButton;

	private OleDbConnection MyConn;

	private OleDbDataAdapter da;

	private DataSet ds;

	private DataTableCollection tables;

	private BindingSource source1;

	private bool messageFileAvailable;

	private StreamWriter message;

	private StreamWriter verbatim;

	private string message_filename;

	private string comPORT;

	private string receivedData;

	private string inputBuffer;

	private int commandCount;

	private bool split_time_running;

	private bool maze_time_running;

	private int previous_millis_value;

	private int monitor_input;

	private int verbose_monitor;

	private int entry_time_limit_s;

	private int last_run_time_inserted;

	private int last_entry_id_inserted;

	private int last_score_time_inserted;

	private string scoring_model_short_name;

	private int touches_enabled;

	private int touches_cumulative;

	private int touch_time_ms;

	private int touch_time_divider;

	private int entry_time_divider;

	private int touches_per_run;

	private string newValue;

	public OleDbDataReader dr;

	public OleDbDataAdapter dw;

	internal virtual ComboBox comPort_ComboBox
	{
		[CompilerGenerated]
		get
		{
			return _comPort_ComboBox;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = comPort_ComboBox_SelectedIndexChanged;
			ComboBox val = _comPort_ComboBox;
			if (val != null)
			{
				val.SelectedIndexChanged -= eventHandler;
			}
			_comPort_ComboBox = value;
			val = _comPort_ComboBox;
			if (val != null)
			{
				val.SelectedIndexChanged += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("SerialPort1")]
	internal virtual SerialPort SerialPort1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("RichTextBox1")]
	internal virtual RichTextBox RichTextBox1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button clear_BTN
	{
		[CompilerGenerated]
		get
		{
			return _clear_BTN;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = clear_BTN_Click;
			Button val = _clear_BTN;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_clear_BTN = value;
			val = _clear_BTN;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button connect_BTN
	{
		[CompilerGenerated]
		get
		{
			return _connect_BTN;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = connect_BTN_Click;
			Button val = _connect_BTN;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_connect_BTN = value;
			val = _connect_BTN;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Timer Timer1
	{
		[CompilerGenerated]
		get
		{
			return _Timer1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Timer1_Tick;
			Timer val = _Timer1;
			if (val != null)
			{
				val.Tick -= eventHandler;
			}
			_Timer1 = value;
			val = _Timer1;
			if (val != null)
			{
				val.Tick += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Timer_LBL")]
	internal virtual Label Timer_LBL
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Split_Time")]
	internal virtual Label Split_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Split_Time_lbl")]
	internal virtual Label Split_Time_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Maze_Time_lbl")]
	internal virtual Label Maze_Time_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Maze_Time")]
	internal virtual Label Maze_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual DataGridView Competition_DataGridView
	{
		[CompilerGenerated]
		get
		{
			return _Competition_DataGridView;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			EventHandler eventHandler = Competition_DataGridview_SelectionChanged;
			DataGridViewCellEventHandler val = new DataGridViewCellEventHandler(Competition_DataGridView_CellContentClick);
			DataGridView val2 = _Competition_DataGridView;
			if (val2 != null)
			{
				val2.SelectionChanged -= eventHandler;
				val2.CellContentClick -= val;
			}
			_Competition_DataGridView = value;
			val2 = _Competition_DataGridView;
			if (val2 != null)
			{
				val2.SelectionChanged += eventHandler;
				val2.CellContentClick += val;
			}
		}
	}

	[field: AccessedThroughProperty("Run_Time_lbl")]
	internal virtual Label Run_Time_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Run_Time")]
	internal virtual Label Run_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button New_Mouse_Button
	{
		[CompilerGenerated]
		get
		{
			return _New_Mouse_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = New_Mouse_Button_Click;
			Button val = _New_Mouse_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_New_Mouse_Button = value;
			val = _New_Mouse_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Event_Date_lbl")]
	internal virtual Label Event_Date_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Event_Name_lbl")]
	internal virtual Label Event_Name_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual ComboBox Competition_Class_ComboBox
	{
		[CompilerGenerated]
		get
		{
			return _Competition_Class_ComboBox;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Competition_Class_ComboBox_SelectedIndexChanged;
			ComboBox val = _Competition_Class_ComboBox;
			if (val != null)
			{
				val.SelectedIndexChanged -= eventHandler;
			}
			_Competition_Class_ComboBox = value;
			val = _Competition_Class_ComboBox;
			if (val != null)
			{
				val.SelectedIndexChanged += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Selected_Competition")]
	internal virtual Label Selected_Competition
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual DataGridView Mouse_DataGridView
	{
		[CompilerGenerated]
		get
		{
			return _Mouse_DataGridView;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Mouse_DataGridview_SelectionChanged;
			DataGridView val = _Mouse_DataGridView;
			if (val != null)
			{
				val.SelectionChanged -= eventHandler;
			}
			_Mouse_DataGridView = value;
			val = _Mouse_DataGridView;
			if (val != null)
			{
				val.SelectionChanged += eventHandler;
			}
		}
	}

	internal virtual Label Selected_Robot
	{
		[CompilerGenerated]
		get
		{
			return _Selected_Robot;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Selected_Robot_Click;
			Label val = _Selected_Robot;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Selected_Robot = value;
			val = _Selected_Robot;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Touch_Button
	{
		[CompilerGenerated]
		get
		{
			return _Touch_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Touch_Button_Click;
			Button val = _Touch_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Touch_Button = value;
			val = _Touch_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Touches_Label")]
	internal virtual Label Touches_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Score_Time")]
	internal virtual Label Score_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Score_Time_lbl")]
	internal virtual Label Score_Time_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Best_Score_lbl")]
	internal virtual Label Best_Score_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Best_Score")]
	internal virtual Label Best_Score
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Time")]
	internal virtual Label Touch_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Divider")]
	internal virtual Label Touch_Divider
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Maze_Divider")]
	internal virtual Label Maze_Divider
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Cumulative")]
	internal virtual Label Touch_Cumulative
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Monitor_Button
	{
		[CompilerGenerated]
		get
		{
			return _Monitor_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Monitor_Button_Click;
			Button val = _Monitor_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Monitor_Button = value;
			val = _Monitor_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Verbose_Button
	{
		[CompilerGenerated]
		get
		{
			return _Verbose_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Verbose_Button_Click;
			Button val = _Verbose_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Verbose_Button = value;
			val = _Verbose_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Competition_Class_lbl")]
	internal virtual Label Competition_Class_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Time_Label_lbl")]
	internal virtual Label Touch_Time_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Divider_Label_lbl")]
	internal virtual Label Touch_Divider_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Maze_Div_Label_lbl")]
	internal virtual Label Maze_Div_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Cum_Label_lbl")]
	internal virtual Label Touch_Cum_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Enabled_Label_lbl")]
	internal virtual Label Touch_Enabled_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touch_Enabled_Label")]
	internal virtual Label Touch_Enabled_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Timer_State_Label_lbl")]
	internal virtual Label Timer_State_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Timer_State_lbl")]
	internal virtual Label Timer_State_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("State_Text_Label")]
	internal virtual Label State_Text_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button DNF_Button
	{
		[CompilerGenerated]
		get
		{
			return _DNF_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = DNF_Button_Click;
			Button val = _DNF_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_DNF_Button = value;
			val = _DNF_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Practice_Mode_Button
	{
		[CompilerGenerated]
		get
		{
			return _Practice_Mode_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Practice_Mode_Button_Click;
			Button val = _Practice_Mode_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Practice_Mode_Button = value;
			val = _Practice_Mode_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Calibrate_Button
	{
		[CompilerGenerated]
		get
		{
			return _Calibrate_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Calibrate_Button_Click;
			Button val = _Calibrate_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Calibrate_Button = value;
			val = _Calibrate_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("time_remaining_lbl")]
	internal virtual Label time_remaining_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("time_left")]
	internal virtual Label time_left
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Clear_Button
	{
		[CompilerGenerated]
		get
		{
			return _Clear_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Clear_Button_Click;
			Button val = _Clear_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Clear_Button = value;
			val = _Clear_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Display_1_channel_button
	{
		[CompilerGenerated]
		get
		{
			return _Display_1_channel_button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Display_1_Channel_Button_Click;
			Button val = _Display_1_channel_button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Display_1_channel_button = value;
			val = _Display_1_channel_button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Results_1ch_button
	{
		[CompilerGenerated]
		get
		{
			return _Results_1ch_button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Results_1ch_btn_Click;
			Button val = _Results_1ch_button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Results_1ch_button = value;
			val = _Results_1ch_button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Display_2ch_button")]
	internal virtual Button Display_2ch_button
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Results_2ch_button")]
	internal virtual Button Results_2ch_button
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("touches_lbl")]
	internal virtual Label touches_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Run_order_Button
	{
		[CompilerGenerated]
		get
		{
			return _Run_order_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Run_order_Button_Click;
			Button val = _Run_order_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Run_order_Button = value;
			val = _Run_order_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button Name_contestants_Button
	{
		[CompilerGenerated]
		get
		{
			return _Name_contestants_Button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Name_contestants_Button_Click;
			Button val = _Name_contestants_Button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Name_contestants_Button = value;
			val = _Name_contestants_Button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("run_number_Lbl")]
	internal virtual Label run_number_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("run_number")]
	internal virtual Label run_number
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("allowed_runs_Lbl")]
	internal virtual Label allowed_runs_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("allowed_runs")]
	internal virtual Label allowed_runs
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Display_1ch_v2_button
	{
		[CompilerGenerated]
		get
		{
			return _Display_1ch_v2_button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Display_1ch_v2_button_Click;
			Button val = _Display_1ch_v2_button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Display_1ch_v2_button = value;
			val = _Display_1ch_v2_button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("GroupBox1")]
	internal virtual GroupBox GroupBox1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("GroupBox2")]
	internal virtual GroupBox GroupBox2
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("GroupBox3")]
	internal virtual GroupBox GroupBox3
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Display_1ch_v2wide_button
	{
		[CompilerGenerated]
		get
		{
			return _Display_1ch_v2wide_button;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Display_1ch_v2wide_button_Click;
			Button val = _Display_1ch_v2wide_button;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_Display_1ch_v2wide_button = value;
			val = _Display_1ch_v2wide_button;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Current_Contestant_Label")]
	internal virtual Label Current_Contestant_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Rank_Label")]
	internal virtual Label Rank_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Rank_Label_lbl")]
	internal virtual Label Rank_Label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button ExtraRunButton
	{
		[CompilerGenerated]
		get
		{
			return _ExtraRunButton;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = ExtraRunButton_Click;
			Button val = _ExtraRunButton;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_ExtraRunButton = value;
			val = _ExtraRunButton;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	internal virtual Button WatchDogButton
	{
		[CompilerGenerated]
		get
		{
			return _WatchDogButton;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = WatchDogButton_Click;
			Button val = _WatchDogButton;
			if (val != null)
			{
				((Control)val).Click -= eventHandler;
			}
			_WatchDogButton = value;
			val = _WatchDogButton;
			if (val != null)
			{
				((Control)val).Click += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("watchdog_state_Label")]
	internal virtual Label watchdog_state_Label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public Form1()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		((Form)this)._002Ector();
		((Form)this).Load += Form1_Load;
		((Form)this).FormClosing += new FormClosingEventHandler(Form1_Close);
		source1 = new BindingSource();
		messageFileAvailable = false;
		receivedData = "";
		inputBuffer = "";
		commandCount = 0;
		split_time_running = false;
		maze_time_running = false;
		previous_millis_value = 0;
		monitor_input = 1;
		verbose_monitor = 0;
		last_run_time_inserted = 0;
		last_entry_id_inserted = 0;
		last_score_time_inserted = 0;
		scoring_model_short_name = "_Any";
		touches_enabled = 0;
		touches_cumulative = 0;
		touch_time_ms = 0;
		touch_time_divider = 0;
		entry_time_divider = 0;
		touches_per_run = 0;
		newValue = "";
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
		}
		finally
		{
			((Form)this).Dispose(disposing);
		}
	}

	[DebuggerStepThrough]
	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Expected O, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected O, but got Unknown
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Expected O, but got Unknown
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Expected O, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Expected O, but got Unknown
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Expected O, but got Unknown
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Expected O, but got Unknown
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Expected O, but got Unknown
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Expected O, but got Unknown
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Expected O, but got Unknown
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Expected O, but got Unknown
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Expected O, but got Unknown
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Expected O, but got Unknown
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Expected O, but got Unknown
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Expected O, but got Unknown
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Expected O, but got Unknown
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Expected O, but got Unknown
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Expected O, but got Unknown
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Expected O, but got Unknown
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Expected O, but got Unknown
		//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Expected O, but got Unknown
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Expected O, but got Unknown
		//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Expected O, but got Unknown
		//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc2: Expected O, but got Unknown
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Expected O, but got Unknown
		//IL_0fea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Expected O, but got Unknown
		//IL_107f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1089: Expected O, but got Unknown
		//IL_111c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1126: Expected O, but got Unknown
		//IL_11a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b2: Expected O, but got Unknown
		//IL_1242: Unknown result type (might be due to invalid IL or missing references)
		//IL_124c: Expected O, but got Unknown
		//IL_12ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Expected O, but got Unknown
		//IL_163e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1648: Expected O, but got Unknown
		//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1acd: Expected O, but got Unknown
		//IL_1b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b51: Expected O, but got Unknown
		//IL_1bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdf: Expected O, but got Unknown
		//IL_1c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6a: Expected O, but got Unknown
		//IL_1d08: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d12: Expected O, but got Unknown
		//IL_1d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da9: Expected O, but got Unknown
		//IL_1e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2e: Expected O, but got Unknown
		//IL_1ea8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb2: Expected O, but got Unknown
		//IL_1f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f47: Expected O, but got Unknown
		//IL_1fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd8: Expected O, but got Unknown
		//IL_2056: Unknown result type (might be due to invalid IL or missing references)
		//IL_2060: Expected O, but got Unknown
		//IL_20f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_20fa: Expected O, but got Unknown
		//IL_2182: Unknown result type (might be due to invalid IL or missing references)
		//IL_218c: Expected O, but got Unknown
		//IL_2213: Unknown result type (might be due to invalid IL or missing references)
		//IL_221d: Expected O, but got Unknown
		//IL_22be: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c8: Expected O, but got Unknown
		//IL_234a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2354: Expected O, but got Unknown
		//IL_23f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ff: Expected O, but got Unknown
		//IL_2481: Unknown result type (might be due to invalid IL or missing references)
		//IL_248b: Expected O, but got Unknown
		//IL_2578: Unknown result type (might be due to invalid IL or missing references)
		//IL_2582: Expected O, but got Unknown
		//IL_2617: Unknown result type (might be due to invalid IL or missing references)
		//IL_2621: Expected O, but got Unknown
		//IL_270f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2719: Expected O, but got Unknown
		//IL_27ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_27b8: Expected O, but got Unknown
		//IL_2878: Unknown result type (might be due to invalid IL or missing references)
		//IL_2882: Expected O, but got Unknown
		//IL_2928: Unknown result type (might be due to invalid IL or missing references)
		//IL_2932: Expected O, but got Unknown
		//IL_29d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_29e0: Expected O, but got Unknown
		//IL_2a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a6c: Expected O, but got Unknown
		//IL_2afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b06: Expected O, but got Unknown
		//IL_2ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bb0: Expected O, but got Unknown
		components = new Container();
		comPort_ComboBox = new ComboBox();
		SerialPort1 = new SerialPort(components);
		RichTextBox1 = new RichTextBox();
		clear_BTN = new Button();
		connect_BTN = new Button();
		Timer1 = new Timer(components);
		Timer_LBL = new Label();
		Split_Time = new Label();
		Split_Time_lbl = new Label();
		Maze_Time_lbl = new Label();
		Maze_Time = new Label();
		Competition_DataGridView = new DataGridView();
		Run_Time_lbl = new Label();
		Run_Time = new Label();
		New_Mouse_Button = new Button();
		Event_Date_lbl = new Label();
		Event_Name_lbl = new Label();
		Competition_Class_ComboBox = new ComboBox();
		Selected_Competition = new Label();
		Mouse_DataGridView = new DataGridView();
		Selected_Robot = new Label();
		Touch_Button = new Button();
		Touches_Label = new Label();
		Score_Time = new Label();
		Score_Time_lbl = new Label();
		Best_Score_lbl = new Label();
		Best_Score = new Label();
		Touch_Time = new Label();
		Touch_Divider = new Label();
		Maze_Divider = new Label();
		Touch_Cumulative = new Label();
		Monitor_Button = new Button();
		Verbose_Button = new Button();
		Competition_Class_lbl = new Label();
		Touch_Time_Label_lbl = new Label();
		Touch_Divider_Label_lbl = new Label();
		Maze_Div_Label_lbl = new Label();
		Touch_Cum_Label_lbl = new Label();
		Touch_Enabled_Label_lbl = new Label();
		Touch_Enabled_Label = new Label();
		Timer_State_Label_lbl = new Label();
		Timer_State_lbl = new Label();
		State_Text_Label = new Label();
		DNF_Button = new Button();
		Practice_Mode_Button = new Button();
		Calibrate_Button = new Button();
		time_remaining_lbl = new Label();
		time_left = new Label();
		Clear_Button = new Button();
		Display_1_channel_button = new Button();
		Results_1ch_button = new Button();
		Display_2ch_button = new Button();
		Results_2ch_button = new Button();
		touches_lbl = new Label();
		Run_order_Button = new Button();
		Name_contestants_Button = new Button();
		run_number_Lbl = new Label();
		run_number = new Label();
		allowed_runs_Lbl = new Label();
		allowed_runs = new Label();
		Display_1ch_v2_button = new Button();
		GroupBox1 = new GroupBox();
		ExtraRunButton = new Button();
		GroupBox2 = new GroupBox();
		Display_1ch_v2wide_button = new Button();
		GroupBox3 = new GroupBox();
		Current_Contestant_Label = new Label();
		Rank_Label = new Label();
		Rank_Label_lbl = new Label();
		WatchDogButton = new Button();
		watchdog_state_Label = new Label();
		((ISupportInitialize)Competition_DataGridView).BeginInit();
		((ISupportInitialize)Mouse_DataGridView).BeginInit();
		((Control)GroupBox1).SuspendLayout();
		((Control)GroupBox2).SuspendLayout();
		((Control)GroupBox3).SuspendLayout();
		((Control)this).SuspendLayout();
		comPort_ComboBox.DropDownStyle = (ComboBoxStyle)2;
		((Control)comPort_ComboBox).Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((ListControl)comPort_ComboBox).FormattingEnabled = true;
		((Control)comPort_ComboBox).Location = new Point(9, 148);
		((Control)comPort_ComboBox).Name = "comPort_ComboBox";
		((Control)comPort_ComboBox).Size = new Size(121, 21);
		((Control)comPort_ComboBox).TabIndex = 26;
		((TextBoxBase)RichTextBox1).BackColor = SystemColors.Control;
		RichTextBox1.Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((TextBoxBase)RichTextBox1).HideSelection = false;
		((Control)RichTextBox1).Location = new Point(9, 202);
		((Control)RichTextBox1).Name = "RichTextBox1";
		((Control)RichTextBox1).Size = new Size(254, 279);
		((Control)RichTextBox1).TabIndex = 30;
		RichTextBox1.Text = "";
		((Control)clear_BTN).Location = new Point(142, 487);
		((Control)clear_BTN).Name = "clear_BTN";
		((Control)clear_BTN).Size = new Size(56, 21);
		((Control)clear_BTN).TabIndex = 28;
		((ButtonBase)clear_BTN).Text = "Clear";
		((ButtonBase)clear_BTN).UseVisualStyleBackColor = true;
		((Control)connect_BTN).Font = new Font("Microsoft Sans Serif", 8f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)connect_BTN).Location = new Point(136, 148);
		((Control)connect_BTN).Name = "connect_BTN";
		((Control)connect_BTN).Size = new Size(57, 21);
		((Control)connect_BTN).TabIndex = 27;
		((ButtonBase)connect_BTN).Text = "Connect";
		((ButtonBase)connect_BTN).UseVisualStyleBackColor = true;
		Timer1.Interval = 10;
		Timer_LBL.AutoSize = true;
		((Control)Timer_LBL).Font = new Font("Microsoft Sans Serif", 8f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Timer_LBL).Location = new Point(199, 152);
		((Control)Timer_LBL).Name = "Timer_LBL";
		((Control)Timer_LBL).Size = new Size(59, 13);
		((Control)Timer_LBL).TabIndex = 31;
		Timer_LBL.Text = "Timer: OFF";
		((Control)Split_Time).BackColor = SystemColors.Window;
		((Control)Split_Time).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Split_Time).ForeColor = Color.Black;
		((Control)Split_Time).Location = new Point(726, 66);
		((Control)Split_Time).Name = "Split_Time";
		((Control)Split_Time).Size = new Size(88, 18);
		((Control)Split_Time).TabIndex = 41;
		Split_Time.Text = "secs";
		Split_Time.UseMnemonic = false;
		((Control)Split_Time_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Split_Time_lbl).ForeColor = Color.Black;
		((Control)Split_Time_lbl).Location = new Point(725, 46);
		((Control)Split_Time_lbl).Name = "Split_Time_lbl";
		((Control)Split_Time_lbl).Size = new Size(88, 18);
		((Control)Split_Time_lbl).TabIndex = 42;
		Split_Time_lbl.Text = "SplitTime";
		Split_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Maze_Time_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Maze_Time_lbl).ForeColor = Color.Black;
		((Control)Maze_Time_lbl).Location = new Point(5, 46);
		((Control)Maze_Time_lbl).Name = "Maze_Time_lbl";
		((Control)Maze_Time_lbl).Size = new Size(70, 18);
		((Control)Maze_Time_lbl).TabIndex = 43;
		Maze_Time_lbl.Text = "Course";
		Maze_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Maze_Time).BackColor = SystemColors.Window;
		((Control)Maze_Time).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Maze_Time).ForeColor = Color.Black;
		((Control)Maze_Time).Location = new Point(6, 66);
		((Control)Maze_Time).Name = "Maze_Time";
		((Control)Maze_Time).Size = new Size(70, 18);
		((Control)Maze_Time).TabIndex = 44;
		Maze_Time.Text = "secs";
		Competition_DataGridView.AllowUserToAddRows = false;
		Competition_DataGridView.AllowUserToDeleteRows = false;
		Competition_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)8;
		Competition_DataGridView.BackgroundColor = SystemColors.Control;
		Competition_DataGridView.BorderStyle = (BorderStyle)2;
		Competition_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		Competition_DataGridView.ColumnHeadersVisible = false;
		((Control)Competition_DataGridView).Location = new Point(273, 175);
		Competition_DataGridView.MultiSelect = false;
		((Control)Competition_DataGridView).Name = "Competition_DataGridView";
		Competition_DataGridView.ReadOnly = true;
		Competition_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Competition_DataGridView).Size = new Size(203, 93);
		((Control)Competition_DataGridView).TabIndex = 46;
		((Control)Run_Time_lbl).BackColor = SystemColors.Control;
		((Control)Run_Time_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Run_Time_lbl).ForeColor = Color.Black;
		((Control)Run_Time_lbl).Location = new Point(537, 46);
		((Control)Run_Time_lbl).Name = "Run_Time_lbl";
		((Control)Run_Time_lbl).Size = new Size(88, 18);
		((Control)Run_Time_lbl).TabIndex = 47;
		Run_Time_lbl.Text = "Last Run";
		Run_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Run_Time).BackColor = SystemColors.Window;
		((Control)Run_Time).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Run_Time).ForeColor = Color.Black;
		((Control)Run_Time).Location = new Point(538, 66);
		((Control)Run_Time).Name = "Run_Time";
		((Control)Run_Time).Size = new Size(88, 18);
		((Control)Run_Time).TabIndex = 48;
		Run_Time.Text = "secs";
		((Control)New_Mouse_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)New_Mouse_Button).Location = new Point(238, 19);
		((Control)New_Mouse_Button).Name = "New_Mouse_Button";
		((Control)New_Mouse_Button).Size = new Size(110, 61);
		((Control)New_Mouse_Button).TabIndex = 50;
		((ButtonBase)New_Mouse_Button).Text = "New Robot";
		((ButtonBase)New_Mouse_Button).UseVisualStyleBackColor = true;
		((Control)Event_Date_lbl).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Event_Date_lbl).ForeColor = SystemColors.MenuHighlight;
		Event_Date_lbl.ImageAlign = (ContentAlignment)64;
		((Control)Event_Date_lbl).Location = new Point(1, 2);
		((Control)Event_Date_lbl).Name = "Event_Date_lbl";
		((Control)Event_Date_lbl).Size = new Size(123, 22);
		((Control)Event_Date_lbl).TabIndex = 51;
		Event_Date_lbl.Text = "Date";
		Event_Date_lbl.TextAlign = (ContentAlignment)16;
		((Control)Event_Name_lbl).BackColor = SystemColors.Control;
		((Control)Event_Name_lbl).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Event_Name_lbl).ForeColor = SystemColors.MenuHighlight;
		((Control)Event_Name_lbl).Location = new Point(143, 2);
		((Control)Event_Name_lbl).Name = "Event_Name_lbl";
		((Control)Event_Name_lbl).Size = new Size(293, 22);
		((Control)Event_Name_lbl).TabIndex = 52;
		Event_Name_lbl.Text = "Event";
		Event_Name_lbl.TextAlign = (ContentAlignment)16;
		Competition_Class_ComboBox.BackColor = SystemColors.Control;
		((Control)Competition_Class_ComboBox).Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((ListControl)Competition_Class_ComboBox).FormattingEnabled = true;
		Competition_Class_ComboBox.Items.AddRange(new object[2] { "Final", "Heats" });
		((Control)Competition_Class_ComboBox).Location = new Point(273, 152);
		((Control)Competition_Class_ComboBox).Name = "Competition_Class_ComboBox";
		((Control)Competition_Class_ComboBox).Size = new Size(203, 21);
		((Control)Competition_Class_ComboBox).TabIndex = 54;
		Competition_Class_ComboBox.Text = "Select Class";
		((Control)Selected_Competition).BackColor = SystemColors.Control;
		((Control)Selected_Competition).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Competition).ForeColor = SystemColors.MenuHighlight;
		((Control)Selected_Competition).Location = new Point(447, 2);
		((Control)Selected_Competition).Name = "Selected_Competition";
		((Control)Selected_Competition).Size = new Size(344, 22);
		((Control)Selected_Competition).TabIndex = 56;
		Selected_Competition.Text = "Selected Contest";
		Selected_Competition.TextAlign = (ContentAlignment)64;
		Mouse_DataGridView.AllowUserToAddRows = false;
		Mouse_DataGridView.AllowUserToDeleteRows = false;
		Mouse_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)8;
		Mouse_DataGridView.BackgroundColor = SystemColors.Control;
		Mouse_DataGridView.BorderStyle = (BorderStyle)2;
		Mouse_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		Mouse_DataGridView.ColumnHeadersVisible = false;
		Mouse_DataGridView.GridColor = SystemColors.AppWorkspace;
		((Control)Mouse_DataGridView).Location = new Point(273, 278);
		Mouse_DataGridView.MultiSelect = false;
		((Control)Mouse_DataGridView).Name = "Mouse_DataGridView";
		Mouse_DataGridView.ReadOnly = true;
		Mouse_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Mouse_DataGridView).Size = new Size(203, 230);
		((Control)Mouse_DataGridView).TabIndex = 57;
		((Control)Selected_Robot).BackColor = SystemColors.Control;
		((Control)Selected_Robot).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Robot).ForeColor = Color.Red;
		((Control)Selected_Robot).Location = new Point(1, 24);
		((Control)Selected_Robot).Name = "Selected_Robot";
		((Control)Selected_Robot).Size = new Size(290, 22);
		((Control)Selected_Robot).TabIndex = 58;
		Selected_Robot.Text = "Robot / Practice Mode";
		Selected_Robot.TextAlign = (ContentAlignment)16;
		((Control)Touch_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Touch_Button).Location = new Point(6, 19);
		((Control)Touch_Button).Name = "Touch_Button";
		((Control)Touch_Button).Size = new Size(110, 24);
		((Control)Touch_Button).TabIndex = 59;
		((ButtonBase)Touch_Button).Text = "Add Touch";
		((ButtonBase)Touch_Button).UseVisualStyleBackColor = true;
		((Control)Touches_Label).BackColor = SystemColors.Window;
		((Control)Touches_Label).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Touches_Label).ForeColor = Color.Black;
		((Control)Touches_Label).Location = new Point(270, 66);
		((Control)Touches_Label).Name = "Touches_Label";
		((Control)Touches_Label).Size = new Size(72, 18);
		((Control)Touches_Label).TabIndex = 60;
		Touches_Label.Text = "no";
		((Control)Score_Time).BackColor = SystemColors.Window;
		((Control)Score_Time).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Score_Time).ForeColor = Color.Black;
		((Control)Score_Time).Location = new Point(632, 66);
		((Control)Score_Time).Name = "Score_Time";
		((Control)Score_Time).Size = new Size(88, 18);
		((Control)Score_Time).TabIndex = 61;
		Score_Time.Text = "calc";
		((Control)Score_Time_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Score_Time_lbl).ForeColor = Color.Black;
		((Control)Score_Time_lbl).Location = new Point(631, 46);
		((Control)Score_Time_lbl).Name = "Score_Time_lbl";
		((Control)Score_Time_lbl).Size = new Size(88, 18);
		((Control)Score_Time_lbl).TabIndex = 62;
		Score_Time_lbl.Text = "LastScore";
		Score_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Best_Score_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score_lbl).ForeColor = Color.Black;
		((Control)Best_Score_lbl).Location = new Point(348, 46);
		((Control)Best_Score_lbl).Name = "Best_Score_lbl";
		((Control)Best_Score_lbl).Size = new Size(88, 18);
		((Control)Best_Score_lbl).TabIndex = 63;
		Best_Score_lbl.Text = "BestScore";
		Best_Score_lbl.TextAlign = (ContentAlignment)16;
		((Control)Best_Score).BackColor = SystemColors.Window;
		((Control)Best_Score).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score).ForeColor = Color.Black;
		((Control)Best_Score).Location = new Point(349, 66);
		((Control)Best_Score).Name = "Best_Score";
		((Control)Best_Score).Size = new Size(88, 18);
		((Control)Best_Score).TabIndex = 64;
		Best_Score.Text = "calc";
		Touch_Time.AutoSize = true;
		((Control)Touch_Time).Location = new Point(538, 116);
		((Control)Touch_Time).Name = "Touch_Time";
		((Control)Touch_Time).Size = new Size(64, 13);
		((Control)Touch_Time).TabIndex = 65;
		Touch_Time.Text = "Touch Time";
		Touch_Time.TextAlign = (ContentAlignment)16;
		Touch_Divider.AutoSize = true;
		((Control)Touch_Divider).Location = new Point(356, 116);
		((Control)Touch_Divider).Name = "Touch_Divider";
		((Control)Touch_Divider).Size = new Size(74, 13);
		((Control)Touch_Divider).TabIndex = 66;
		Touch_Divider.Text = "Touch Divider";
		Touch_Divider.TextAlign = (ContentAlignment)16;
		Maze_Divider.AutoSize = true;
		((Control)Maze_Divider).Location = new Point(632, 116);
		((Control)Maze_Divider).Name = "Maze_Divider";
		((Control)Maze_Divider).Size = new Size(69, 13);
		((Control)Maze_Divider).TabIndex = 67;
		Maze_Divider.Text = "Maze Divider";
		Maze_Divider.TextAlign = (ContentAlignment)16;
		Touch_Cumulative.AutoSize = true;
		((Control)Touch_Cumulative).Location = new Point(444, 116);
		((Control)Touch_Cumulative).Name = "Touch_Cumulative";
		((Control)Touch_Cumulative).Size = new Size(62, 13);
		((Control)Touch_Cumulative).TabIndex = 68;
		Touch_Cumulative.Text = "Touch Cum";
		Touch_Cumulative.TextAlign = (ContentAlignment)16;
		((Control)Monitor_Button).Location = new Point(12, 487);
		((Control)Monitor_Button).Name = "Monitor_Button";
		((Control)Monitor_Button).Size = new Size(56, 21);
		((Control)Monitor_Button).TabIndex = 69;
		((ButtonBase)Monitor_Button).Text = "NoMonitor";
		((ButtonBase)Monitor_Button).UseVisualStyleBackColor = true;
		((Control)Verbose_Button).Location = new Point(77, 487);
		((Control)Verbose_Button).Name = "Verbose_Button";
		((Control)Verbose_Button).Size = new Size(56, 21);
		((Control)Verbose_Button).TabIndex = 70;
		((ButtonBase)Verbose_Button).Text = "Verbose";
		((ButtonBase)Verbose_Button).UseVisualStyleBackColor = true;
		((Control)Competition_Class_lbl).BackColor = SystemColors.Control;
		((Control)Competition_Class_lbl).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Competition_Class_lbl).ForeColor = SystemColors.MenuHighlight;
		((Control)Competition_Class_lbl).Location = new Point(795, 2);
		((Control)Competition_Class_lbl).Name = "Competition_Class_lbl";
		((Control)Competition_Class_lbl).Size = new Size(89, 22);
		((Control)Competition_Class_lbl).TabIndex = 71;
		Competition_Class_lbl.Text = "Class";
		Competition_Class_lbl.TextAlign = (ContentAlignment)16;
		Touch_Time_Label_lbl.AutoSize = true;
		((Control)Touch_Time_Label_lbl).Location = new Point(538, 103);
		((Control)Touch_Time_Label_lbl).Name = "Touch_Time_Label_lbl";
		((Control)Touch_Time_Label_lbl).Size = new Size(64, 13);
		((Control)Touch_Time_Label_lbl).TabIndex = 76;
		Touch_Time_Label_lbl.Text = "Touch Time";
		Touch_Time_Label_lbl.TextAlign = (ContentAlignment)16;
		Touch_Divider_Label_lbl.AutoSize = true;
		((Control)Touch_Divider_Label_lbl).Location = new Point(356, 104);
		((Control)Touch_Divider_Label_lbl).Name = "Touch_Divider_Label_lbl";
		((Control)Touch_Divider_Label_lbl).Size = new Size(74, 13);
		((Control)Touch_Divider_Label_lbl).TabIndex = 77;
		Touch_Divider_Label_lbl.Text = "Touch Divider";
		Touch_Divider_Label_lbl.TextAlign = (ContentAlignment)16;
		Maze_Div_Label_lbl.AutoSize = true;
		((Control)Maze_Div_Label_lbl).Location = new Point(632, 103);
		((Control)Maze_Div_Label_lbl).Name = "Maze_Div_Label_lbl";
		((Control)Maze_Div_Label_lbl).Size = new Size(76, 13);
		((Control)Maze_Div_Label_lbl).TabIndex = 78;
		Maze_Div_Label_lbl.Text = "Course Divider";
		Maze_Div_Label_lbl.TextAlign = (ContentAlignment)16;
		Touch_Cum_Label_lbl.AutoSize = true;
		((Control)Touch_Cum_Label_lbl).Location = new Point(444, 103);
		((Control)Touch_Cum_Label_lbl).Name = "Touch_Cum_Label_lbl";
		((Control)Touch_Cum_Label_lbl).Size = new Size(79, 13);
		((Control)Touch_Cum_Label_lbl).TabIndex = 79;
		Touch_Cum_Label_lbl.Text = "Touch Cumtive";
		Touch_Cum_Label_lbl.TextAlign = (ContentAlignment)16;
		Touch_Enabled_Label_lbl.AutoSize = true;
		((Control)Touch_Enabled_Label_lbl).Location = new Point(262, 104);
		((Control)Touch_Enabled_Label_lbl).Name = "Touch_Enabled_Label_lbl";
		((Control)Touch_Enabled_Label_lbl).Size = new Size(80, 13);
		((Control)Touch_Enabled_Label_lbl).TabIndex = 82;
		Touch_Enabled_Label_lbl.Text = "Touch Enabled";
		Touch_Enabled_Label_lbl.TextAlign = (ContentAlignment)16;
		Touch_Enabled_Label.AutoSize = true;
		((Control)Touch_Enabled_Label).Location = new Point(262, 116);
		((Control)Touch_Enabled_Label).Name = "Touch_Enabled_Label";
		((Control)Touch_Enabled_Label).Size = new Size(80, 13);
		((Control)Touch_Enabled_Label).TabIndex = 81;
		Touch_Enabled_Label.Text = "Touch Enabled";
		Touch_Enabled_Label.TextAlign = (ContentAlignment)16;
		Timer_State_Label_lbl.AutoSize = true;
		((Control)Timer_State_Label_lbl).Location = new Point(6, 104);
		((Control)Timer_State_Label_lbl).Name = "Timer_State_Label_lbl";
		((Control)Timer_State_Label_lbl).Size = new Size(97, 13);
		((Control)Timer_State_Label_lbl).TabIndex = 83;
		Timer_State_Label_lbl.Text = "Timing Gates State";
		((Control)Timer_State_lbl).Location = new Point(109, 104);
		((Control)Timer_State_lbl).Name = "Timer_State_lbl";
		((Control)Timer_State_lbl).Size = new Size(39, 13);
		((Control)Timer_State_lbl).TabIndex = 84;
		Timer_State_lbl.Text = "State";
		((Control)State_Text_Label).Location = new Point(6, 118);
		((Control)State_Text_Label).Name = "State_Text_Label";
		((Control)State_Text_Label).Size = new Size(142, 14);
		((Control)State_Text_Label).TabIndex = 85;
		State_Text_Label.Text = "Timing Gates State";
		((Control)DNF_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)DNF_Button).Location = new Point(6, 56);
		((Control)DNF_Button).Name = "DNF_Button";
		((Control)DNF_Button).Size = new Size(110, 24);
		((Control)DNF_Button).TabIndex = 86;
		((ButtonBase)DNF_Button).Text = "DNF";
		((ButtonBase)DNF_Button).UseVisualStyleBackColor = true;
		((Control)Practice_Mode_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Practice_Mode_Button).Location = new Point(497, 148);
		((Control)Practice_Mode_Button).Name = "Practice_Mode_Button";
		((Control)Practice_Mode_Button).Size = new Size(342, 24);
		((Control)Practice_Mode_Button).TabIndex = 88;
		((ButtonBase)Practice_Mode_Button).Text = "Switch Mode      Practice <=> Contest";
		((ButtonBase)Practice_Mode_Button).UseVisualStyleBackColor = true;
		((Control)Calibrate_Button).Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Calibrate_Button).Location = new Point(207, 487);
		((Control)Calibrate_Button).Name = "Calibrate_Button";
		((Control)Calibrate_Button).Size = new Size(56, 21);
		((Control)Calibrate_Button).TabIndex = 89;
		((ButtonBase)Calibrate_Button).Text = "Calibrate";
		((ButtonBase)Calibrate_Button).UseVisualStyleBackColor = true;
		((Control)time_remaining_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)time_remaining_lbl).ForeColor = Color.Black;
		((Control)time_remaining_lbl).Location = new Point(82, 46);
		((Control)time_remaining_lbl).Name = "time_remaining_lbl";
		((Control)time_remaining_lbl).Size = new Size(70, 18);
		((Control)time_remaining_lbl).TabIndex = 91;
		time_remaining_lbl.Text = "To Go";
		time_remaining_lbl.TextAlign = (ContentAlignment)16;
		((Control)time_left).BackColor = SystemColors.Window;
		((Control)time_left).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)time_left).ForeColor = Color.Black;
		((Control)time_left).Location = new Point(83, 66);
		((Control)time_left).Name = "time_left";
		((Control)time_left).Size = new Size(70, 18);
		((Control)time_left).TabIndex = 92;
		time_left.Text = "secs";
		time_left.TextAlign = (ContentAlignment)16;
		((Control)Clear_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Clear_Button).Location = new Point(122, 56);
		((Control)Clear_Button).Name = "Clear_Button";
		((Control)Clear_Button).Size = new Size(110, 24);
		((Control)Clear_Button).TabIndex = 93;
		((ButtonBase)Clear_Button).Text = "Clear Display";
		((ButtonBase)Clear_Button).UseVisualStyleBackColor = true;
		((Control)Display_1_channel_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Display_1_channel_button).Location = new Point(6, 55);
		((Control)Display_1_channel_button).Name = "Display_1_channel_button";
		((Control)Display_1_channel_button).Size = new Size(110, 24);
		((Control)Display_1_channel_button).TabIndex = 94;
		((ButtonBase)Display_1_channel_button).Text = "612 x 595";
		((ButtonBase)Display_1_channel_button).UseVisualStyleBackColor = true;
		((Control)Results_1ch_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Results_1ch_button).Location = new Point(238, 23);
		((Control)Results_1ch_button).Name = "Results_1ch_button";
		((Control)Results_1ch_button).Size = new Size(110, 24);
		((Control)Results_1ch_button).TabIndex = 95;
		((ButtonBase)Results_1ch_button).Text = "Results";
		((ButtonBase)Results_1ch_button).UseVisualStyleBackColor = true;
		((Control)Display_2ch_button).Enabled = false;
		((Control)Display_2ch_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Display_2ch_button).Location = new Point(6, 23);
		((Control)Display_2ch_button).Name = "Display_2ch_button";
		((Control)Display_2ch_button).Size = new Size(110, 24);
		((Control)Display_2ch_button).TabIndex = 96;
		((ButtonBase)Display_2ch_button).Text = "Display";
		((ButtonBase)Display_2ch_button).UseVisualStyleBackColor = true;
		((Control)Results_2ch_button).Enabled = false;
		((Control)Results_2ch_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Results_2ch_button).Location = new Point(238, 23);
		((Control)Results_2ch_button).Name = "Results_2ch_button";
		((Control)Results_2ch_button).Size = new Size(110, 24);
		((Control)Results_2ch_button).TabIndex = 97;
		((ButtonBase)Results_2ch_button).Text = "Results";
		((ButtonBase)Results_2ch_button).UseVisualStyleBackColor = true;
		((Control)touches_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)touches_lbl).ForeColor = Color.Black;
		((Control)touches_lbl).Location = new Point(269, 46);
		((Control)touches_lbl).Name = "touches_lbl";
		((Control)touches_lbl).Size = new Size(72, 18);
		((Control)touches_lbl).TabIndex = 98;
		touches_lbl.Text = "Touches";
		touches_lbl.TextAlign = (ContentAlignment)16;
		((Control)Run_order_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Run_order_Button).Location = new Point(122, 55);
		((Control)Run_order_Button).Name = "Run_order_Button";
		((Control)Run_order_Button).Size = new Size(110, 24);
		((Control)Run_order_Button).TabIndex = 99;
		((ButtonBase)Run_order_Button).Text = "Run Order";
		((ButtonBase)Run_order_Button).UseVisualStyleBackColor = true;
		((Control)Name_contestants_Button).Enabled = false;
		((Control)Name_contestants_Button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Name_contestants_Button).Location = new Point(6, 69);
		((Control)Name_contestants_Button).Name = "Name_contestants_Button";
		((Control)Name_contestants_Button).Size = new Size(110, 24);
		((Control)Name_contestants_Button).TabIndex = 100;
		((ButtonBase)Name_contestants_Button).Text = "Add Names";
		((ButtonBase)Name_contestants_Button).UseVisualStyleBackColor = true;
		((Control)Name_contestants_Button).Visible = false;
		((Control)run_number_Lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)run_number_Lbl).ForeColor = Color.Black;
		((Control)run_number_Lbl).Location = new Point(159, 46);
		((Control)run_number_Lbl).Name = "run_number_Lbl";
		((Control)run_number_Lbl).Size = new Size(70, 18);
		((Control)run_number_Lbl).TabIndex = 102;
		run_number_Lbl.Text = "Run No";
		run_number_Lbl.TextAlign = (ContentAlignment)16;
		((Control)run_number).BackColor = SystemColors.Window;
		((Control)run_number).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)run_number).ForeColor = Color.Black;
		((Control)run_number).Location = new Point(160, 66);
		((Control)run_number).Name = "run_number";
		((Control)run_number).Size = new Size(70, 18);
		((Control)run_number).TabIndex = 101;
		run_number.Text = "no";
		((Control)allowed_runs_Lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)allowed_runs_Lbl).ForeColor = Color.Black;
		((Control)allowed_runs_Lbl).Location = new Point(236, 46);
		((Control)allowed_runs_Lbl).Name = "allowed_runs_Lbl";
		((Control)allowed_runs_Lbl).Size = new Size(26, 18);
		((Control)allowed_runs_Lbl).TabIndex = 103;
		allowed_runs_Lbl.Text = "of";
		allowed_runs_Lbl.TextAlign = (ContentAlignment)16;
		((Control)allowed_runs).BackColor = SystemColors.Window;
		((Control)allowed_runs).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)allowed_runs).ForeColor = Color.Black;
		((Control)allowed_runs).Location = new Point(237, 66);
		((Control)allowed_runs).Name = "allowed_runs";
		((Control)allowed_runs).Size = new Size(26, 18);
		((Control)allowed_runs).TabIndex = 104;
		allowed_runs.Text = "no";
		((Control)Display_1ch_v2_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Display_1ch_v2_button).Location = new Point(6, 23);
		((Control)Display_1ch_v2_button).Name = "Display_1ch_v2_button";
		((Control)Display_1ch_v2_button).Size = new Size(110, 24);
		((Control)Display_1ch_v2_button).TabIndex = 105;
		((ButtonBase)Display_1ch_v2_button).Text = "800x600";
		((ButtonBase)Display_1ch_v2_button).UseVisualStyleBackColor = true;
		((Control)GroupBox1).Controls.Add((Control)(object)ExtraRunButton);
		((Control)GroupBox1).Controls.Add((Control)(object)New_Mouse_Button);
		((Control)GroupBox1).Controls.Add((Control)(object)DNF_Button);
		((Control)GroupBox1).Controls.Add((Control)(object)Touch_Button);
		((Control)GroupBox1).Controls.Add((Control)(object)Clear_Button);
		((Control)GroupBox1).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)GroupBox1).ForeColor = Color.Blue;
		((Control)GroupBox1).Location = new Point(491, 175);
		((Control)GroupBox1).Name = "GroupBox1";
		((Control)GroupBox1).Size = new Size(381, 93);
		((Control)GroupBox1).TabIndex = 106;
		GroupBox1.TabStop = false;
		GroupBox1.Text = "Entry Controls";
		((Control)ExtraRunButton).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)ExtraRunButton).Location = new Point(122, 19);
		((Control)ExtraRunButton).Name = "ExtraRunButton";
		((Control)ExtraRunButton).Size = new Size(110, 24);
		((Control)ExtraRunButton).TabIndex = 94;
		((ButtonBase)ExtraRunButton).Text = "Extra Run";
		((ButtonBase)ExtraRunButton).UseVisualStyleBackColor = true;
		((Control)GroupBox2).Controls.Add((Control)(object)Display_1ch_v2wide_button);
		((Control)GroupBox2).Controls.Add((Control)(object)Display_1ch_v2_button);
		((Control)GroupBox2).Controls.Add((Control)(object)Display_1_channel_button);
		((Control)GroupBox2).Controls.Add((Control)(object)Run_order_Button);
		((Control)GroupBox2).Controls.Add((Control)(object)Results_1ch_button);
		((Control)GroupBox2).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)GroupBox2).ForeColor = Color.DarkGreen;
		((Control)GroupBox2).Location = new Point(491, 278);
		((Control)GroupBox2).Name = "GroupBox2";
		((Control)GroupBox2).Size = new Size(381, 93);
		((Control)GroupBox2).TabIndex = 107;
		GroupBox2.TabStop = false;
		GroupBox2.Text = "1 Channel Timer";
		((Control)Display_1ch_v2wide_button).Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Display_1ch_v2wide_button).Location = new Point(122, 23);
		((Control)Display_1ch_v2wide_button).Name = "Display_1ch_v2wide_button";
		((Control)Display_1ch_v2wide_button).Size = new Size(110, 24);
		((Control)Display_1ch_v2wide_button).TabIndex = 106;
		((ButtonBase)Display_1ch_v2wide_button).Text = "1280x720";
		((ButtonBase)Display_1ch_v2wide_button).UseVisualStyleBackColor = true;
		((Control)GroupBox3).Controls.Add((Control)(object)Display_2ch_button);
		((Control)GroupBox3).Controls.Add((Control)(object)Results_2ch_button);
		((Control)GroupBox3).Controls.Add((Control)(object)Name_contestants_Button);
		((Control)GroupBox3).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)GroupBox3).ForeColor = Color.MediumVioletRed;
		((Control)GroupBox3).Location = new Point(491, 388);
		((Control)GroupBox3).Name = "GroupBox3";
		((Control)GroupBox3).Size = new Size(381, 93);
		((Control)GroupBox3).TabIndex = 108;
		GroupBox3.TabStop = false;
		GroupBox3.Text = "2 Channel Timer";
		((Control)Current_Contestant_Label).BackColor = SystemColors.Control;
		((Control)Current_Contestant_Label).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Current_Contestant_Label).ForeColor = Color.Red;
		((Control)Current_Contestant_Label).Location = new Point(645, 24);
		((Control)Current_Contestant_Label).Name = "Current_Contestant_Label";
		((Control)Current_Contestant_Label).Size = new Size(227, 22);
		((Control)Current_Contestant_Label).TabIndex = 109;
		Current_Contestant_Label.Text = "Current Contestant";
		Current_Contestant_Label.TextAlign = (ContentAlignment)16;
		((Control)Rank_Label).BackColor = SystemColors.Window;
		((Control)Rank_Label).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Rank_Label).ForeColor = Color.Black;
		((Control)Rank_Label).Location = new Point(443, 66);
		((Control)Rank_Label).Name = "Rank_Label";
		((Control)Rank_Label).Size = new Size(48, 18);
		((Control)Rank_Label).TabIndex = 111;
		Rank_Label.Text = "calc";
		((Control)Rank_Label_lbl).Font = new Font("Arial Black", 10f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Rank_Label_lbl).ForeColor = Color.Black;
		((Control)Rank_Label_lbl).Location = new Point(442, 46);
		((Control)Rank_Label_lbl).Name = "Rank_Label_lbl";
		((Control)Rank_Label_lbl).Size = new Size(48, 18);
		((Control)Rank_Label_lbl).TabIndex = 110;
		Rank_Label_lbl.Text = "Rank";
		Rank_Label_lbl.TextAlign = (ContentAlignment)16;
		((Control)WatchDogButton).Font = new Font("Microsoft Sans Serif", 8f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)WatchDogButton).Location = new Point(12, 175);
		((Control)WatchDogButton).Name = "WatchDogButton";
		((Control)WatchDogButton).Size = new Size(118, 24);
		((Control)WatchDogButton).TabIndex = 112;
		((ButtonBase)WatchDogButton).Text = "WatchDog is On";
		((ButtonBase)WatchDogButton).UseVisualStyleBackColor = true;
		((Control)watchdog_state_Label).BackColor = SystemColors.Menu;
		((Control)watchdog_state_Label).Cursor = Cursors.Default;
		((Control)watchdog_state_Label).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)watchdog_state_Label).Location = new Point(148, 170);
		((Control)watchdog_state_Label).Name = "watchdog_state_Label";
		((Control)watchdog_state_Label).Size = new Size(110, 29);
		((Control)watchdog_state_Label).TabIndex = 113;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(884, 522);
		((Control)this).Controls.Add((Control)(object)watchdog_state_Label);
		((Control)this).Controls.Add((Control)(object)WatchDogButton);
		((Control)this).Controls.Add((Control)(object)Practice_Mode_Button);
		((Control)this).Controls.Add((Control)(object)Rank_Label);
		((Control)this).Controls.Add((Control)(object)Rank_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Current_Contestant_Label);
		((Control)this).Controls.Add((Control)(object)GroupBox3);
		((Control)this).Controls.Add((Control)(object)GroupBox2);
		((Control)this).Controls.Add((Control)(object)GroupBox1);
		((Control)this).Controls.Add((Control)(object)allowed_runs);
		((Control)this).Controls.Add((Control)(object)allowed_runs_Lbl);
		((Control)this).Controls.Add((Control)(object)run_number_Lbl);
		((Control)this).Controls.Add((Control)(object)run_number);
		((Control)this).Controls.Add((Control)(object)touches_lbl);
		((Control)this).Controls.Add((Control)(object)time_left);
		((Control)this).Controls.Add((Control)(object)time_remaining_lbl);
		((Control)this).Controls.Add((Control)(object)Calibrate_Button);
		((Control)this).Controls.Add((Control)(object)State_Text_Label);
		((Control)this).Controls.Add((Control)(object)Timer_State_lbl);
		((Control)this).Controls.Add((Control)(object)Timer_State_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Touch_Enabled_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Touch_Enabled_Label);
		((Control)this).Controls.Add((Control)(object)Touch_Cum_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Maze_Div_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Touch_Divider_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Touch_Time_Label_lbl);
		((Control)this).Controls.Add((Control)(object)Competition_Class_lbl);
		((Control)this).Controls.Add((Control)(object)Verbose_Button);
		((Control)this).Controls.Add((Control)(object)Monitor_Button);
		((Control)this).Controls.Add((Control)(object)Touch_Cumulative);
		((Control)this).Controls.Add((Control)(object)Maze_Divider);
		((Control)this).Controls.Add((Control)(object)Touch_Divider);
		((Control)this).Controls.Add((Control)(object)Touch_Time);
		((Control)this).Controls.Add((Control)(object)Best_Score);
		((Control)this).Controls.Add((Control)(object)Best_Score_lbl);
		((Control)this).Controls.Add((Control)(object)Score_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Score_Time);
		((Control)this).Controls.Add((Control)(object)Touches_Label);
		((Control)this).Controls.Add((Control)(object)Selected_Robot);
		((Control)this).Controls.Add((Control)(object)Mouse_DataGridView);
		((Control)this).Controls.Add((Control)(object)Selected_Competition);
		((Control)this).Controls.Add((Control)(object)Competition_Class_ComboBox);
		((Control)this).Controls.Add((Control)(object)Event_Name_lbl);
		((Control)this).Controls.Add((Control)(object)Event_Date_lbl);
		((Control)this).Controls.Add((Control)(object)Run_Time);
		((Control)this).Controls.Add((Control)(object)Run_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Competition_DataGridView);
		((Control)this).Controls.Add((Control)(object)Maze_Time);
		((Control)this).Controls.Add((Control)(object)Maze_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Split_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Split_Time);
		((Control)this).Controls.Add((Control)(object)comPort_ComboBox);
		((Control)this).Controls.Add((Control)(object)RichTextBox1);
		((Control)this).Controls.Add((Control)(object)clear_BTN);
		((Control)this).Controls.Add((Control)(object)connect_BTN);
		((Control)this).Controls.Add((Control)(object)Timer_LBL);
		((Control)this).Name = "Form1";
		((Form)this).Text = "Registration And Timing System (RATS) Version 4.0.1a database version 3.3";
		((ISupportInitialize)Competition_DataGridView).EndInit();
		((ISupportInitialize)Mouse_DataGridView).EndInit();
		((Control)GroupBox1).ResumeLayout(false);
		((Control)GroupBox2).ResumeLayout(false);
		((Control)GroupBox3).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		Timer1.Enabled = false;
		Public_Variables.robot = "Practice Mode";
		Selected_Robot.Text = Public_Variables.robot;
		comPORT = "";
		foreach (string serialPortName in ((Computer)MyProject.Computer).Ports.SerialPortNames)
		{
			comPort_ComboBox.Items.Add((object)serialPortName);
		}
		MyConn = new OleDbConnection();
		MyConn.ConnectionString = Public_Variables.connString;
		try
		{
			MyConn.Open();
			string text = "SELECT  Effective_Date, Current_Event, Current_Event_ID FROM Context WHERE Context.ID = 1";
			OleDbCommand val = new OleDbCommand(text, MyConn);
			dr = val.ExecuteReader();
			dr.Read();
			Public_Variables.robotics_event_id = Conversions.ToInteger(dr["Current_Event_ID"]);
			Public_Variables.robotics_event = Conversions.ToString(dr["Current_Event"]);
			Public_Variables.robotics_event_date = Conversions.ToDate(dr["Effective_Date"]);
			((Component)(object)val).Dispose();
			Event_Date_lbl.Text = Public_Variables.robotics_event_date.ToString();
			Event_Name_lbl.Text = Public_Variables.robotics_event;
			Public_Variables.database_available = true;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Public_Variables.database_available = false;
			Public_Variables.robotics_event = "Stand Alone Mode";
			Event_Name_lbl.Text = Public_Variables.robotics_event;
			Public_Variables.robotics_event_date = DateAndTime.Today;
			Event_Date_lbl.Text = Public_Variables.robotics_event_date.ToString();
			ProjectData.ClearProjectError();
		}
		if (!Public_Variables.database_available)
		{
			((Control)Practice_Mode_Button).Enabled = false;
			((Control)Run_order_Button).Enabled = false;
			((Control)Display_1_channel_button).Enabled = false;
			((Control)Display_2ch_button).Enabled = false;
			((Control)Name_contestants_Button).Enabled = false;
			((Control)Results_1ch_button).Enabled = false;
			((Control)Results_2ch_button).Enabled = false;
		}
		message_filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\RATS_message_log" + Strings.Format(DateAndTime.Now.Year, "d4") + Strings.Format(DateAndTime.Now.Month, "d2") + Strings.Format(DateAndTime.Now.Day, "d2") + Strings.Format(DateAndTime.Now.Hour, "d2") + Strings.Format(DateAndTime.Now.Minute, "d2") + Strings.Format(DateAndTime.Now.Second, "d2") + ".txt";
		message = ((ServerComputer)MyProject.Computer).FileSystem.OpenTextFileWriter(message_filename, true);
		message_filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\RATS_verbatim_log" + Strings.Format(DateAndTime.Now.Year, "d4") + Strings.Format(DateAndTime.Now.Month, "d2") + Strings.Format(DateAndTime.Now.Day, "d2") + Strings.Format(DateAndTime.Now.Hour, "d2") + Strings.Format(DateAndTime.Now.Minute, "d2") + Strings.Format(DateAndTime.Now.Second, "d2") + ".txt";
		verbatim = ((ServerComputer)MyProject.Computer).FileSystem.OpenTextFileWriter(message_filename, true);
		messageFileAvailable = true;
	}

	private void Form1_Close(object sender, FormClosingEventArgs e)
	{
		if (messageFileAvailable)
		{
			messageFileAvailable = false;
			message.Close();
			verbatim.Close();
		}
	}

	private void comPort_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Operators.ConditionalCompareObjectNotEqual(comPort_ComboBox.SelectedItem, "", TextCompare: false))
		{
			comPORT = Conversions.ToString(comPort_ComboBox.SelectedItem);
		}
	}

	private void Competition_Class_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		if (Conversions.ToBoolean(Operators.AndObject(Operators.CompareObjectNotEqual(Competition_Class_ComboBox.SelectedItem, "", TextCompare: false), !Public_Variables.practice_mode)))
		{
			ds = new DataSet();
			tables = ds.Tables;
			string text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("SELECT Competition_ID, Competition_Name, Scoring_Model_Short_Name, Entry_Time_Limit_Secs, Number_of_Runs_Allowed, Grace_Period_Secs FROM [Competition] WHERE ((Event_ID = " + Public_Variables.robotics_event_id + ") AND (Competition_Class = '", Competition_Class_ComboBox.SelectedItem), "')) ORDER BY Challenge"));
			da = new OleDbDataAdapter(text, MyConn);
			((DbDataAdapter)(object)da).Fill(ds, "Competition");
			DataView dataSource = new DataView(tables[0]);
			source1.DataSource = dataSource;
			Competition_DataGridView.DataSource = dataSource;
			Competition_Class_lbl.Text = Conversions.ToString(Competition_Class_ComboBox.SelectedItem);
			Public_Variables.public_competition_class = Conversions.ToString(Competition_Class_ComboBox.SelectedItem);
		}
		else
		{
			Competition_Class_lbl.Text = "";
			Public_Variables.public_competition_class = "";
		}
	}

	private void Competition_DataGridview_SelectionChanged(object sender, EventArgs e)
	{
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		if (!Public_Variables.practice_mode)
		{
			if (((BaseCollection)Competition_DataGridView.SelectedRows).Count > 0)
			{
				Selected_Competition.Text = Conversions.ToString(Competition_DataGridView.SelectedRows[0].Cells[1].Value);
				Public_Variables.public_competition_name = Conversions.ToString(Competition_DataGridView.SelectedRows[0].Cells[1].Value);
				Public_Variables.previous_competition_id = Public_Variables.competition_id;
				Public_Variables.competition_id = Conversions.ToInteger(Competition_DataGridView.SelectedRows[0].Cells[0].Value);
				scoring_model_short_name = Conversions.ToString(Competition_DataGridView.SelectedRows[0].Cells[2].Value);
				entry_time_limit_s = Conversions.ToInteger(Competition_DataGridView.SelectedRows[0].Cells[3].Value);
				Public_Variables.no_of_runs_allowed = Conversions.ToInteger(Competition_DataGridView.SelectedRows[0].Cells[4].Value);
				Public_Variables.grace_period_s = Conversions.ToInteger(Competition_DataGridView.SelectedRows[0].Cells[5].Value);
				Public_Variables.grace_period_ms = checked(Public_Variables.grace_period_s * 1000);
			}
			Public_Variables.time_left_s = entry_time_limit_s;
			Public_Variables.no_of_runs_used = 0;
			string text = "SELECT  Touches_Enabled, Touches_Cumulative, Touch_Time_mS, Entry_Time_Divider, Touch_Time_Divider, Touches_Per_Run FROM Scoring_Model WHERE (Scoring_Model_Short_Name = '" + scoring_model_short_name + "')";
			OleDbCommand val = new OleDbCommand(text, MyConn);
			dr = val.ExecuteReader();
			dr.Read();
			touches_enabled = Conversions.ToInteger(dr["Touches_Enabled"]);
			touches_cumulative = Conversions.ToInteger(dr["Touches_Cumulative"]);
			touch_time_ms = Conversions.ToInteger(dr["Touch_Time_mS"]);
			entry_time_divider = Conversions.ToInteger(dr["Entry_Time_Divider"]);
			touch_time_divider = Conversions.ToInteger(dr["Touch_Time_Divider"]);
			touches_per_run = Conversions.ToInteger(dr["Touches_Per_Run"]);
			((Component)(object)val).Dispose();
			Display_pending_entries();
		}
		else
		{
			Selected_Competition.Text = "";
			Public_Variables.public_competition_name = "";
			Public_Variables.previous_competition_id = 0;
			Public_Variables.competition_id = 0;
			scoring_model_short_name = "";
			entry_time_limit_s = 0;
			Public_Variables.no_of_runs_allowed = 0;
			touches_enabled = 0;
			touches_cumulative = 0;
			touch_time_ms = 0;
			entry_time_divider = 0;
			touch_time_divider = 0;
			touches_per_run = 0;
		}
		run_number.Text = Public_Variables.no_of_runs_used.ToString();
		allowed_runs.Text = Public_Variables.no_of_runs_allowed.ToString();
		time_left.Text = Public_Variables.time_left_s.ToString();
		Touch_Time.Text = touch_time_ms.ToString();
		Touch_Divider.Text = touch_time_divider.ToString();
		Maze_Divider.Text = entry_time_divider.ToString();
		if (touches_cumulative == -1)
		{
			Touch_Cumulative.Text = "yes";
		}
		else
		{
			Touch_Cumulative.Text = "no";
		}
		if (touches_enabled == -1)
		{
			Touch_Enabled_Label.Text = "yes";
		}
		else
		{
			Touch_Enabled_Label.Text = "no";
		}
		Public_Variables.refresh_best_score_times = true;
	}

	private void Mouse_DataGridview_SelectionChanged(object sender, EventArgs e)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		if (Public_Variables.practice_mode || ((BaseCollection)Competition_DataGridView.SelectedRows).Count != 1)
		{
			return;
		}
		if (SerialPort1.IsOpen)
		{
			try
			{
				Public_Variables.robot = Conversions.ToString(Mouse_DataGridView.SelectedRows[0].Cells[1].Value);
				Selected_Robot.Text = Public_Variables.robot;
				Public_Variables.entry_id = Conversions.ToInteger(Mouse_DataGridView.SelectedRows[0].Cells[0].Value);
				string text = "SELECT Contestant.Contestant_Name, Contestant.Class\r\n                                            FROM Contestant INNER JOIN Mouse ON Contestant.Contestant_ID = Mouse.Contestant_ID\r\n                                            WHERE (Mouse.Mouse_Name = '" + Public_Variables.robot + "');";
				OleDbCommand val = new OleDbCommand(text, MyConn);
				dr = val.ExecuteReader();
				dr.Read();
				Public_Variables.contestant = Conversions.ToString(dr["Contestant_Name"]);
				Public_Variables.contestant_class = Conversions.ToString(dr["Class"]);
				((Component)(object)val).Dispose();
				Current_Contestant_Label.Text = Public_Variables.contestant;
				SerialPort1.WriteLine("<98,0>");
				message.WriteLine("Tx " + DateTime.Now.ToString() + " <98,0>");
				clear_timer();
				if (Public_Variables.time_left_ms < 0)
				{
					if (Math.Abs(Public_Variables.time_left_ms) < Public_Variables.grace_period_ms)
					{
						((Control)time_left).ForeColor = Color.Orange;
						((Control)time_remaining_lbl).ForeColor = Color.Orange;
					}
					else
					{
						((Control)time_left).ForeColor = Color.Red;
						((Control)time_remaining_lbl).ForeColor = Color.Red;
					}
				}
				else
				{
					((Control)time_left).ForeColor = Color.Black;
					((Control)time_remaining_lbl).ForeColor = Color.Black;
				}
				commandCount = 0;
				return;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Public_Variables.robot = "";
				Selected_Robot.Text = Public_Variables.robot;
				Public_Variables.contestant = "_";
				Current_Contestant_Label.Text = Public_Variables.contestant;
				Public_Variables.contestant_class = "_";
				Public_Variables.robot_rank = 0;
				Rank_Label.Text = "0";
				ProjectData.ClearProjectError();
				return;
			}
		}
		Interaction.MsgBox("Connect a COM port first");
	}

	private void connect_BTN_Click(object sender, EventArgs e)
	{
		if (Operators.CompareString(((ButtonBase)connect_BTN).Text, "Connect", TextCompare: false) == 0)
		{
			if (Operators.CompareString(comPORT, "", TextCompare: false) != 0)
			{
				SerialPort1.Close();
				SerialPort1.PortName = comPORT;
				SerialPort1.BaudRate = 9600;
				SerialPort1.DataBits = 8;
				SerialPort1.Parity = (Parity)0;
				SerialPort1.StopBits = (StopBits)1;
				SerialPort1.Handshake = (Handshake)0;
				SerialPort1.Encoding = Encoding.Default;
				SerialPort1.ReadTimeout = 10000;
				try
				{
					SerialPort1.Open();
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Interaction.MsgBox(ex2.Message);
					ProjectData.ClearProjectError();
				}
				if (SerialPort1.IsOpen)
				{
					Public_Variables.serial_port_opened = true;
					((ButtonBase)connect_BTN).Text = "Dis-connect";
					((Control)comPort_ComboBox).Enabled = false;
					Timer1.Enabled = true;
					Timer_LBL.Text = "Timer: ON";
				}
			}
			else
			{
				Interaction.MsgBox("Select a COM port first");
			}
		}
		else
		{
			Timer1.Enabled = false;
			SerialPort1.Close();
			Public_Variables.serial_port_opened = false;
			((ButtonBase)connect_BTN).Text = "Connect";
			((Control)comPort_ComboBox).Enabled = true;
			Timer_LBL.Text = "Timer: OFF";
			comPort_ComboBox.Text = string.Empty;
		}
	}

	private void Practice_Mode_Button_Click(object sender, EventArgs e)
	{
		if (Public_Variables.practice_mode)
		{
			Public_Variables.practice_mode = false;
			Public_Variables.robot = "";
			Selected_Robot.Text = Public_Variables.robot;
		}
		else
		{
			Public_Variables.practice_mode = true;
			Public_Variables.robot = "Practice Mode";
			Selected_Robot.Text = Public_Variables.robot;
			Public_Variables.contestant = "_";
			Current_Contestant_Label.Text = Public_Variables.contestant;
			Public_Variables.hide_best_score_times = true;
		}
		if (SerialPort1.IsOpen)
		{
			SerialPort1.WriteLine("<98,0>");
			message.WriteLine("Tx " + DateTime.Now.ToString() + " <98,0>");
			clear_timer();
		}
		else
		{
			Interaction.MsgBox("Connect a COM port first");
		}
	}

	private void clear_BTN_Click(object sender, EventArgs e)
	{
		RichTextBox1.Text = "";
		commandCount = 0;
	}

	private void Monitor_Button_Click(object sender, EventArgs e)
	{
		if (monitor_input == 0)
		{
			monitor_input = 1;
			((ButtonBase)Monitor_Button).Text = "NoMonitor";
		}
		else
		{
			monitor_input = 0;
			((ButtonBase)Monitor_Button).Text = "Monitor";
		}
	}

	private void Verbose_Button_Click(object sender, EventArgs e)
	{
		if (verbose_monitor == 0)
		{
			verbose_monitor = 1;
			((ButtonBase)Verbose_Button).Text = "Concise";
		}
		else
		{
			verbose_monitor = 0;
			((ButtonBase)Verbose_Button).Text = "Verbose";
		}
	}

	private void Touch_Button_Click(object sender, EventArgs e)
	{
		checked
		{
			Public_Variables.no_of_touches++;
			Touches_Label.Text = Public_Variables.no_of_touches.ToString();
		}
	}

	private void clear_timer()
	{
		Public_Variables.split_time_ms = 0;
		Split_Time.Text = Strings.Format((double)Public_Variables.split_time_ms / 1000.0, "F2");
		Public_Variables.maze_time_ms = 0;
		Maze_Time.Text = Strings.Format((double)Public_Variables.maze_time_ms / 1000.0, "F2");
		Public_Variables.run_time_ms = 0;
		Run_Time.Text = Strings.Format((double)Public_Variables.run_time_ms / 1000.0, "F3");
		Public_Variables.no_of_touches = 0;
		Touches_Label.Text = Strings.Format(Public_Variables.no_of_touches);
		Public_Variables.score_time_ms = 0;
		Score_Time.Text = Strings.Format((double)Public_Variables.score_time_ms / 1000.0, "F3");
		Public_Variables.fastest_score_time_this_robot = -1;
		Best_Score.Text = "";
		Public_Variables.time_left_ms = checked(entry_time_limit_s * 1000);
		time_left.Text = Strings.Format((double)Public_Variables.time_left_ms / 1000.0, "F2");
		((Control)time_left).ForeColor = Color.Black;
		((Control)time_remaining_lbl).ForeColor = Color.Black;
		Public_Variables.no_of_runs_used = 0;
		run_number.Text = Public_Variables.no_of_runs_used.ToString();
		((Control)run_number).ForeColor = Color.Black;
		((Control)run_number_Lbl).ForeColor = Color.Black;
		Rank_Label.Text = "0";
		Public_Variables.robot_rank = 0;
		split_time_running = false;
		maze_time_running = false;
		Public_Variables.hide_robot_runtimes = true;
	}

	private void DNF_Button_Click(object sender, EventArgs e)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		if (Public_Variables.practice_mode)
		{
			return;
		}
		DialogResult val = MessageBox.Show((IWin32Window)(object)this, "Do you want to terminate this Robot's entry?", "Did Not Finish Requested", (MessageBoxButtons)4, (MessageBoxIcon)32);
		if ((int)val == 6)
		{
			string text = "UPDATE Entry SET [Entry_Used] = TRUE, [Outcome] =  \"Retired\" WHERE ([Entry_ID] = " + Public_Variables.entry_id + ")";
			OleDbCommand val2 = new OleDbCommand(text, MyConn);
			try
			{
				val2.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Interaction.MsgBox(ex2.Message);
				ProjectData.ClearProjectError();
			}
			((Component)(object)val2).Dispose();
			clear_timer();
			Display_pending_entries();
		}
	}

	private void Clear_Button_Click(object sender, EventArgs e)
	{
		clear_timer();
	}

	private void New_Mouse_Button_Click(object sender, EventArgs e)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		DialogResult val = default(DialogResult);
		if (SerialPort1.IsOpen)
		{
			val = MessageBox.Show((IWin32Window)(object)this, "Do you want to end this Robot's entry?", "New Mouse Requested", (MessageBoxButtons)4, (MessageBoxIcon)32);
			if ((int)val == 6)
			{
				SerialPort1.WriteLine("<98,0>");
				message.WriteLine("Tx " + DateTime.Now.ToString() + " <98,0>");
				clear_timer();
			}
		}
		else
		{
			Interaction.MsgBox("Connect a COM port first");
		}
		if (!Public_Variables.practice_mode & ((int)val == 6))
		{
			try
			{
				int index = ((DataGridViewBand)Competition_DataGridView.CurrentRow).Index;
				int num = checked((index <= 0) ? (index + 1) : (index - 1));
				Competition_DataGridView.CurrentCell = Competition_DataGridView.Rows[num].Cells[0];
				Competition_DataGridView.CurrentCell = Competition_DataGridView.Rows[index].Cells[0];
				Public_Variables.fastest_score_time_this_robot = -1;
				Public_Variables.refresh_best_score_times = true;
				Public_Variables.hide_robot_runtimes = true;
				Public_Variables.robot = "";
				Public_Variables.contestant = "_";
				Public_Variables.robot_rank = 0;
				Rank_Label.Text = "0";
				Current_Contestant_Label.Text = Public_Variables.contestant;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Public_Variables.robot = "";
				Selected_Robot.Text = Public_Variables.robot;
				Public_Variables.contestant = "_";
				Current_Contestant_Label.Text = Public_Variables.contestant;
				Public_Variables.robot_rank = 0;
				Rank_Label.Text = "0";
				ProjectData.ClearProjectError();
			}
			Display_pending_entries();
		}
	}

	private void Timer1_Tick(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		Timer1.Enabled = false;
		Timer_LBL.Text = "Timer: OFF";
		num = DateAndTime.Now.Millisecond;
		checked
		{
			num2 = num - previous_millis_value;
			if (num2 < 0)
			{
				num2 += 1000;
			}
			previous_millis_value = num;
			if (split_time_running)
			{
				Public_Variables.split_time_ms += num2;
			}
			if (maze_time_running)
			{
				Public_Variables.maze_time_ms += num2;
				Public_Variables.time_left_ms -= num2;
				if (Public_Variables.time_left_ms < 0)
				{
					if (Math.Abs(Public_Variables.time_left_ms) < Public_Variables.grace_period_ms)
					{
						((Control)time_left).ForeColor = Color.Orange;
					}
					else
					{
						((Control)time_left).ForeColor = Color.Red;
					}
				}
				else
				{
					((Control)time_left).ForeColor = Color.Black;
				}
			}
			if (Public_Variables.timing_gates_state == 5)
			{
				Public_Variables.split_time_ms = Public_Variables.run_time_ms;
			}
			if (Public_Variables.watchdog_active)
			{
				Public_Variables.watchdog_ms_since_reset += num2;
				if (Public_Variables.watchdog_ms_since_reset > 2000)
				{
					Public_Variables.watchdog_alarm = true;
				}
				else
				{
					Public_Variables.watchdog_alarm = false;
				}
			}
			receivedData = ReceiveSerialData();
			inputBuffer += receivedData;
			if (inputBuffer.Contains(">"))
			{
				parseData();
			}
			if (Public_Variables.request_calibration_mode)
			{
				((ButtonBase)Calibrate_Button).Text = "StopCalibrate";
				SerialPort1.WriteLine("<99,CALIBRATION>");
				message.WriteLine("Tx " + DateTime.Now.ToString() + " <99,CALIBRATION>");
				Public_Variables.request_calibration_mode = false;
			}
			if (Public_Variables.request_timer_mode)
			{
				((ButtonBase)Calibrate_Button).Text = "Calibrate";
				SerialPort1.WriteLine("<99,TIMER>");
				message.WriteLine("Tx " + DateTime.Now.ToString() + " <99,TIMER>");
				Public_Variables.request_timer_mode = false;
			}
			Public_Variables.robot = Selected_Robot.Text;
			Split_Time.Text = Strings.Format((double)Public_Variables.split_time_ms / 1000.0, "F2");
			if (!Public_Variables.practice_mode)
			{
				if (((Control)run_number).ForeColor == Color.Black)
				{
					((Control)Split_Time).ForeColor = ((Control)time_left).ForeColor;
				}
				else
				{
					((Control)Split_Time).ForeColor = Color.Red;
				}
				if (((Control)Split_Time).ForeColor == Color.Red)
				{
					Split_Time.Text = "End";
				}
			}
			Run_Time.Text = Strings.Format((double)Public_Variables.run_time_ms / 1000.0, "F3");
			Score_Time.Text = Strings.Format((double)Public_Variables.score_time_ms / 1000.0, "F3");
			if (Public_Variables.fastest_score_time_this_robot > 0)
			{
				Best_Score.Text = Strings.Format((double)Public_Variables.fastest_score_time_this_robot / 1000.0, "F3");
			}
			else
			{
				Best_Score.Text = "0";
			}
			Touches_Label.Text = Strings.Format(Public_Variables.no_of_touches);
			Maze_Time.Text = Strings.Format((double)Public_Variables.maze_time_ms / 1000.0, "F2");
			if (Public_Variables.practice_mode)
			{
				Selected_Competition.Text = "";
				Competition_Class_lbl.Text = "";
				time_left.Text = "";
				Selected_Robot.Text = Public_Variables.robot;
			}
			else
			{
				time_left.Text = Strings.Format((double)Public_Variables.time_left_ms / 1000.0, "F2");
				if (Public_Variables.time_left_ms < 0)
				{
					if (Math.Abs(Public_Variables.time_left_ms) < Public_Variables.grace_period_ms)
					{
						((Control)time_left).ForeColor = Color.Orange;
						((Control)time_remaining_lbl).ForeColor = Color.Orange;
					}
					else
					{
						((Control)time_left).ForeColor = Color.Red;
						((Control)time_remaining_lbl).ForeColor = Color.Red;
					}
				}
				else
				{
					((Control)time_left).ForeColor = Color.Black;
					((Control)time_remaining_lbl).ForeColor = Color.Black;
				}
				Selected_Competition.Text = Public_Variables.public_competition_name;
				Competition_Class_lbl.Text = Public_Variables.public_competition_class;
				Selected_Robot.Text = Public_Variables.robot;
			}
			if (Public_Variables.watchdog_alarm)
			{
				if (Public_Variables.serial_port_opened)
				{
					if (Operators.ConditionalCompareObjectGreater(Public_Variables.watchdog_alarm_repeat_counter, 200, TextCompare: false))
					{
						((Computer)MyProject.Computer).Audio.Play("C:\\Windows\\Media\\notify.wav", (AudioPlayMode)1);
						watchdog_state_Label.Text = "Error";
						((Control)watchdog_state_Label).ForeColor = Color.Red;
						Public_Variables.watchdog_alarm_repeat_counter = 0;
					}
					else
					{
						Public_Variables.watchdog_alarm_repeat_counter = Operators.AddObject(Public_Variables.watchdog_alarm_repeat_counter, 1);
					}
				}
			}
			else
			{
				watchdog_state_Label.Text = "";
			}
			Timer1.Enabled = true;
			Timer_LBL.Text = "Timer: ON";
		}
	}

	public string ReceiveSerialData()
	{
		string result;
		try
		{
			string text = SerialPort1.ReadExisting();
			if (text == null)
			{
				result = "";
			}
			else
			{
				verbatim.Write(text);
				result = text;
			}
		}
		catch (TimeoutException ex)
		{
			ProjectData.SetProjectError(ex);
			TimeoutException ex2 = ex;
			result = "Error: Serial Port read timed out.";
			ProjectData.ClearProjectError();
		}
		return result;
	}

	private void parseData()
	{
		string text = "";
		string text2 = "";
		bool flag = false;
		checked
		{
			while (!flag)
			{
				int num = inputBuffer.IndexOf("<") + 1;
				int num2 = inputBuffer.IndexOf(">") + 1;
				int num3 = inputBuffer.IndexOf(",") + 1;
				if ((monitor_input == 1) & (verbose_monitor == 1))
				{
					((TextBoxBase)RichTextBox1).AppendText("Open = " + Conversions.ToString(num) + "Separator = " + Conversions.ToString(num3) + "Close = " + Conversions.ToString(num2) + "\r\n");
				}
				if (num2 > 0)
				{
					if ((num < num3) & (num3 < num2))
					{
						text = Strings.Mid(inputBuffer, num + 1, num3 - num - 1);
						newValue = Strings.Mid(inputBuffer, num3 + 1, num2 - num3 - 1);
						if (monitor_input == 1 && Operators.CompareString(text, "0", TextCompare: false) != 0)
						{
							text2 = "Message Type = " + text + "Value = " + newValue;
							((TextBoxBase)RichTextBox1).AppendText(text2 + "\r\n");
							if (messageFileAvailable)
							{
								message.WriteLine("Rx " + DateTime.Now.ToString() + " " + text2);
							}
						}
					}
					else
					{
						((TextBoxBase)RichTextBox1).AppendText("Command Discarded \r\n");
					}
					inputBuffer = Strings.Mid(inputBuffer, num2 + 1);
				}
				else
				{
					flag = true;
				}
				switch (text)
				{
				case "0":
					Public_Variables.watchdog_ms_since_reset = 0;
					break;
				case "1":
					Public_Variables.maze_time_ms = (int)Math.Round(Conversion.Val(newValue) * 100.0);
					maze_time_ms_message();
					break;
				case "2":
					Public_Variables.split_time_ms = (int)Math.Round(Conversion.Val(newValue) * 10.0);
					split_time_ms_message();
					break;
				case "3":
					Public_Variables.run_time_ms = (int)Math.Round(Conversion.Val(newValue) * 10.0);
					run_time_ms_message();
					break;
				case "4":
					timer_state_message();
					break;
				case "5":
					Public_Variables.run_time_ms = (int)Math.Round(Conversion.Val(newValue));
					run_time_ms_message();
					break;
				case "6":
					if (Conversion.Val(newValue) == 0.0)
					{
						Public_Variables.run_time_ms = Public_Variables.split_time_ms;
						run_time_ms_message();
					}
					break;
				case "12":
					Public_Variables.split_time_ms = (int)Math.Round(Conversion.Val(newValue));
					split_time_ms_message();
					break;
				case "13":
					Public_Variables.run_time_ms = (int)Math.Round(Conversion.Val(newValue));
					run_time_ms_message();
					break;
				case "30":
					Public_Variables.maze_time_ms = (int)Math.Round(Conversion.Val(newValue));
					maze_time_ms_message();
					break;
				case "71":
					start_gate_state_message();
					break;
				case "72":
					finish_gate_state_message();
					break;
				case "73":
					start_cell_state_message();
					break;
				case "81":
					start_gate_level_message();
					break;
				case "82":
					start_gate_pot_message();
					break;
				case "83":
					finish_gate_level_message();
					break;
				case "84":
					finish_gate_pot_message();
					break;
				case "85":
					start_cell_level_message();
					break;
				case "86":
					start_cell_pot_message();
					break;
				}
			}
		}
	}

	private void maze_time_ms_message()
	{
		Maze_Time.Text = Strings.Format((double)Public_Variables.maze_time_ms / 1000.0, "F2");
		Public_Variables.time_left_ms = checked(entry_time_limit_s * 1000 - Public_Variables.maze_time_ms);
		time_left.Text = Strings.Format((double)Public_Variables.time_left_ms / 1000.0, "F2");
		if (Public_Variables.time_left_ms < 0)
		{
			if (Math.Abs(Public_Variables.time_left_ms) < Public_Variables.grace_period_ms)
			{
				((Control)time_left).ForeColor = Color.Orange;
				((Control)time_remaining_lbl).ForeColor = Color.Orange;
			}
			else
			{
				((Control)time_left).ForeColor = Color.Red;
				((Control)time_remaining_lbl).ForeColor = Color.Red;
			}
		}
		else
		{
			((Control)time_left).ForeColor = Color.Black;
			((Control)time_remaining_lbl).ForeColor = Color.Black;
		}
		maze_time_running = true;
	}

	private void split_time_ms_message()
	{
		Split_Time.Text = Strings.Format((double)Public_Variables.split_time_ms / 1000.0, "F2");
		split_time_running = true;
	}

	private void run_time_ms_message()
	{
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Expected O, but got Unknown
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Expected O, but got Unknown
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Expected O, but got Unknown
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Expected O, but got Unknown
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Expected O, but got Unknown
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Expected O, but got Unknown
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Expected O, but got Unknown
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Expected O, but got Unknown
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		checked
		{
			if (Public_Variables.run_time_ms > 2000)
			{
				Run_Time.Text = Strings.Format((double)Public_Variables.run_time_ms / 1000.0);
				Public_Variables.score_time_ms = 0;
				if (entry_time_divider > 0)
				{
					Public_Variables.score_time_ms = (int)Math.Round((double)Public_Variables.score_time_ms + (double)Public_Variables.maze_time_ms / (double)entry_time_divider);
				}
				if ((touches_enabled == -1) & (Public_Variables.no_of_touches > 0))
				{
					if (touch_time_divider > 0)
					{
						Public_Variables.score_time_ms = (int)Math.Round((double)Public_Variables.score_time_ms + (double)Public_Variables.run_time_ms / (double)touch_time_divider);
					}
					if (touch_time_ms > 0)
					{
						if (touches_cumulative == -1)
						{
							Public_Variables.score_time_ms += touch_time_ms * Public_Variables.no_of_touches;
						}
						else
						{
							Public_Variables.score_time_ms += touch_time_ms;
						}
					}
				}
				Public_Variables.score_time_ms += Public_Variables.run_time_ms;
				Score_Time.Text = Strings.Format((double)Public_Variables.score_time_ms / 1000.0, "F3");
				if ((((last_run_time_inserted == Public_Variables.run_time_ms) & (last_entry_id_inserted == Public_Variables.entry_id)) | (Public_Variables.no_of_runs_used > Public_Variables.no_of_runs_allowed) | (Public_Variables.time_left_ms + Public_Variables.grace_period_ms <= 0)) || !(!Public_Variables.practice_mode & (Public_Variables.entry_id > 0) & (Public_Variables.time_left_ms + Public_Variables.run_time_ms > 0)))
				{
					return;
				}
				message.WriteLine("Msg" + DateTime.Now.ToString() + " Inserting run time for " + Public_Variables.robot);
				string text = "INSERT into Entry_Run ([Entry_ID], [Run_Time_mSecs], [Course_Time_mSecs], [Touches], [Score_Time_mSecs]) values (?, ?, ?, ?, ?)";
				OleDbCommand val = new OleDbCommand(text, MyConn);
				val.Parameters.Add(new OleDbParameter("Entry_ID", (object)Public_Variables.entry_id));
				val.Parameters.Add(new OleDbParameter("Run_Time_mSecs", (object)Public_Variables.run_time_ms));
				val.Parameters.Add(new OleDbParameter("Course_Time_mSecs", (object)Public_Variables.maze_time_ms));
				val.Parameters.Add(new OleDbParameter("Touches", (object)Public_Variables.no_of_touches));
				val.Parameters.Add(new OleDbParameter("Score_Time_mSecs", (object)Public_Variables.score_time_ms));
				try
				{
					val.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Interaction.MsgBox(ex2.Message);
					ProjectData.ClearProjectError();
				}
				((Component)(object)val).Dispose();
				string text2 = "UPDATE Entry SET [Entry_Used] = TRUE, [Outcome] =  \"Successful\" WHERE ([Entry_ID] = " + Public_Variables.entry_id + ")";
				OleDbCommand val2 = new OleDbCommand(text2, MyConn);
				try
				{
					val2.ExecuteNonQuery();
				}
				catch (Exception ex3)
				{
					ProjectData.SetProjectError(ex3);
					Exception ex4 = ex3;
					Interaction.MsgBox(ex4.Message);
					ProjectData.ClearProjectError();
				}
				((Component)(object)val2).Dispose();
				last_run_time_inserted = Public_Variables.run_time_ms;
				last_entry_id_inserted = Public_Variables.entry_id;
				last_score_time_inserted = Public_Variables.score_time_ms;
				if (Public_Variables.fastest_score_time_this_robot < 0)
				{
					Public_Variables.fastest_score_time_this_robot = Public_Variables.score_time_ms;
					Best_Score.Text = Strings.Format((double)Public_Variables.score_time_ms / 1000.0, "F3");
					text = "INSERT into Best_Score_Time ([Entry_ID], [Mouse_Name], [Contestant_Name], [Contestant_Class], [Score_Time_mS], [Run_Time_mS], [Competition_ID]) values (?, ?, ?, ?, ?, ?, ?)";
					OleDbCommand val3 = new OleDbCommand(text, MyConn);
					val3.Parameters.Add(new OleDbParameter("Entry_ID", (object)Public_Variables.entry_id));
					val3.Parameters.Add(new OleDbParameter("Mouse_Name", (object)Public_Variables.robot));
					val3.Parameters.Add(new OleDbParameter("Contestant_Name", (object)Public_Variables.contestant));
					val3.Parameters.Add(new OleDbParameter("Contestant_Class", (object)Public_Variables.contestant_class));
					val3.Parameters.Add(new OleDbParameter("Score_Time_mS", (object)Public_Variables.score_time_ms));
					val3.Parameters.Add(new OleDbParameter("Run_Time_mS", (object)Public_Variables.run_time_ms));
					val3.Parameters.Add(new OleDbParameter("Competition_ID", (object)Public_Variables.competition_id));
					try
					{
						val3.ExecuteNonQuery();
					}
					catch (Exception ex5)
					{
						ProjectData.SetProjectError(ex5);
						Exception ex6 = ex5;
						Interaction.MsgBox(ex6.Message);
						ProjectData.ClearProjectError();
					}
					((Component)(object)val3).Dispose();
				}
				else if (Public_Variables.score_time_ms < Public_Variables.fastest_score_time_this_robot)
				{
					Public_Variables.fastest_score_time_this_robot = Public_Variables.score_time_ms;
					Best_Score.Text = Strings.Format((double)Public_Variables.score_time_ms / 1000.0, "F3");
					text = "UPDATE Best_Score_Time SET [Mouse_Name] = '" + Public_Variables.robot + "', [Contestant_Name] = '" + Public_Variables.contestant + "', [Contestant_Class] = '" + Public_Variables.contestant_class + "', [Score_Time_mS] = " + Public_Variables.score_time_ms + ", [Run_Time_mS] = " + Public_Variables.run_time_ms + ", [Competition_ID] = " + Public_Variables.competition_id + " WHERE ([Entry_ID] = " + Public_Variables.entry_id + ")";
					OleDbCommand val4 = new OleDbCommand(text, MyConn);
					try
					{
						val4.ExecuteNonQuery();
					}
					catch (Exception ex7)
					{
						ProjectData.SetProjectError(ex7);
						Exception ex8 = ex7;
						Interaction.MsgBox(ex8.Message);
						ProjectData.ClearProjectError();
					}
					((Component)(object)val4).Dispose();
				}
				Public_Variables.refresh_robot_run_times = true;
				Public_Variables.refresh_best_score_times = true;
				string text3 = "SELECT Rank FROM(SELECT Mouse_Name, (SELECT COUNT(T1.Score_Time_mS) FROM (SELECT Best_Score_Time.Mouse_Name, Best_Score_Time.Score_Time_mS, Best_Score_Time.Competition_ID FROM Best_Score_Time WHERE (Best_Score_Time.Competition_ID = " + Public_Variables.competition_id + ")) As T1 WHERE T1.Score_Time_mS <= T2.Score_Time_mS) As Rank FROM (SELECT Best_Score_Time.Mouse_Name, Best_Score_Time.Score_Time_mS, Best_Score_Time.Competition_ID FROM Best_Score_Time WHERE (Best_Score_Time.Competition_ID = " + Public_Variables.competition_id + "))  As T2 ORDER BY Score_Time_mS)WHERE(Mouse_Name = '" + Public_Variables.robot + "');";
				OleDbCommand val5 = new OleDbCommand(text3, MyConn);
				try
				{
					dr = val5.ExecuteReader();
					dr.Read();
					Public_Variables.robot_rank = Conversions.ToInteger(dr["Rank"]);
					((Component)(object)val5).Dispose();
					Rank_Label.Text = Public_Variables.robot_rank.ToString();
					return;
				}
				catch (Exception ex9)
				{
					ProjectData.SetProjectError(ex9);
					Exception ex10 = ex9;
					Interaction.MsgBox(ex10.Message);
					ProjectData.ClearProjectError();
					return;
				}
			}
			message.WriteLine("Msg" + DateTime.Now.ToString() + " Discarding <2s run time for " + Public_Variables.robot);
		}
	}

	private void timer_state_message()
	{
		int timing_gates_state = Public_Variables.timing_gates_state;
		checked
		{
			Public_Variables.timing_gates_state = (int)Math.Round(Conversion.Val(newValue));
			Timer_State_lbl.Text = Public_Variables.timing_gates_state.ToString();
			switch (Public_Variables.timing_gates_state)
			{
			case 0:
				State_Text_Label.Text = "Gates callibrating";
				split_time_running = false;
				break;
			case 1:
				State_Text_Label.Text = "Waiting start present";
				split_time_running = false;
				break;
			case 2:
				State_Text_Label.Text = "Robot in start cell";
				split_time_running = false;
				Public_Variables.split_time_ms = 0;
				if (touches_per_run == -1)
				{
					Public_Variables.no_of_touches = 0;
				}
				((Computer)MyProject.Computer).Audio.Play("C:\\Windows\\Media\\chimes.wav", (AudioPlayMode)1);
				break;
			case 3:
				State_Text_Label.Text = "Run started";
				break;
			case 4:
				State_Text_Label.Text = "Run in progress";
				((Computer)MyProject.Computer).Audio.Play("C:\\Windows\\Media\\chord.wav", (AudioPlayMode)1);
				if ((timing_gates_state != Public_Variables.timing_gates_state) & !Public_Variables.practice_mode)
				{
					Public_Variables.no_of_runs_used++;
					run_number.Text = Public_Variables.no_of_runs_used.ToString();
					if (Public_Variables.no_of_runs_used > Public_Variables.no_of_runs_allowed)
					{
						((Control)run_number).ForeColor = Color.Red;
					}
				}
				break;
			case 5:
				State_Text_Label.Text = "Run complete";
				((Computer)MyProject.Computer).Audio.Play("C:\\Windows\\Media\\tada.wav", (AudioPlayMode)1);
				break;
			default:
				State_Text_Label.Text = "Undefined state";
				break;
			}
		}
	}

	private void start_gate_state_message()
	{
		if ((Operators.CompareString(newValue, "ON", TextCompare: false) == 0) | (Operators.CompareString(newValue, "1", TextCompare: false) == 0))
		{
			Public_Variables.STrigger = true;
		}
		else
		{
			Public_Variables.STrigger = false;
		}
	}

	private void finish_gate_state_message()
	{
		if ((Operators.CompareString(newValue, "ON", TextCompare: false) == 0) | (Operators.CompareString(newValue, "1", TextCompare: false) == 0))
		{
			Public_Variables.FTrigger = true;
		}
		else
		{
			Public_Variables.FTrigger = false;
		}
	}

	private void start_cell_state_message()
	{
		if ((Operators.CompareString(newValue, "ON", TextCompare: false) == 0) | (Operators.CompareString(newValue, "1", TextCompare: false) == 0))
		{
			Public_Variables.CTrigger = true;
		}
		else
		{
			Public_Variables.CTrigger = false;
		}
	}

	private void start_gate_level_message()
	{
		Public_Variables.SGLevel = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void start_gate_pot_message()
	{
		Public_Variables.SGPot = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void finish_gate_level_message()
	{
		Public_Variables.FGLevel = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void finish_gate_pot_message()
	{
		Public_Variables.FGPot = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void start_cell_level_message()
	{
		Public_Variables.SCLevel = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void start_cell_pot_message()
	{
		Public_Variables.SCPot = checked((int)Math.Round(Conversion.Val(newValue)));
	}

	private void Calibrate_Button_Click(object sender, EventArgs e)
	{
		Calibration_Form calibration_Form = new Calibration_Form();
		if (SerialPort1.IsOpen)
		{
			if (!Public_Variables.calibration_window_open)
			{
				((Control)calibration_Form).Show();
			}
			else
			{
				Public_Variables.request_calibration_close = true;
			}
		}
		else
		{
			Interaction.MsgBox("Connect a COM port first");
		}
	}

	private void Run_order_Button_Click(object sender, EventArgs e)
	{
		run_order run_order2 = new run_order();
		if (!Public_Variables.run_order_window_open)
		{
			((Control)run_order2).Show();
		}
		else
		{
			Public_Variables.request_run_order_window_close = true;
		}
	}

	private void Display_1_Channel_Button_Click(object sender, EventArgs e)
	{
		single_channel_display single_channel_display2 = new single_channel_display();
		if (!Public_Variables.single_channel_window_open)
		{
			((Control)single_channel_display2).Show();
		}
		else
		{
			Public_Variables.request_single_channel_window_close = true;
		}
	}

	private void Results_1ch_btn_Click(object sender, EventArgs e)
	{
		single_channel_results single_channel_results2 = new single_channel_results();
		if (!Public_Variables.single_channel_results_window_open)
		{
			((Control)single_channel_results2).Show();
		}
		else
		{
			Public_Variables.request_single_channel_results_window_close = true;
		}
	}

	private void Display_pending_entries()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		ds = new DataSet();
		tables = ds.Tables;
		string text = "SELECT Entry_ID, Mouse_Name FROM [Entry] WHERE ((Competition_ID = " + Public_Variables.competition_id + ") AND (Entry_Used = FALSE)) ORDER BY Sequence_Number";
		da = new OleDbDataAdapter(text, MyConn);
		((DbDataAdapter)(object)da).Fill(ds, "Entry");
		DataView dataSource = new DataView(tables[0]);
		source1.DataSource = dataSource;
		Mouse_DataGridView.DataSource = dataSource;
	}

	private void Name_contestants_Button_Click(object sender, EventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		string text = "UPDATE Best_Score_Time, Contestant INNER JOIN Mouse ON Contestant.Contestant_ID = Mouse.Contestant_ID SET Best_Score_Time.Contestant_Name = [Contestant].[Contestant_Name], Best_Score_Time.Contestant_Class = [Contestant].[Class]WHERE (((Best_Score_Time.Mouse_Name)=[Mouse].[Mouse_Name]) AND (Best_Score_Time.Contestant_Name = \"_\"));";
		OleDbCommand val = new OleDbCommand(text, MyConn);
		try
		{
			val.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Interaction.MsgBox(ex2.Message);
			ProjectData.ClearProjectError();
		}
		((Component)(object)val).Dispose();
	}

	private void Display_1ch_v2_button_Click(object sender, EventArgs e)
	{
		single_ch_display_v2 single_ch_display_v3 = new single_ch_display_v2();
		if (!Public_Variables.single_channel_window_open)
		{
			((Control)single_ch_display_v3).Show();
		}
		else
		{
			Public_Variables.request_single_channel_window_close = true;
		}
	}

	private void Display_1ch_v2wide_button_Click(object sender, EventArgs e)
	{
		single_ch_display_16_9 single_ch_display_16_10 = new single_ch_display_16_9();
		if (!Public_Variables.single_channel_window_open)
		{
			((Control)single_ch_display_16_10).Show();
		}
		else
		{
			Public_Variables.request_single_channel_window_close = true;
		}
	}

	private void Selected_Robot_Click(object sender, EventArgs e)
	{
	}

	private void Competition_DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
	}

	private void ExtraRunButton_Click(object sender, EventArgs e)
	{
		checked
		{
			if (Public_Variables.no_of_runs_used > 0)
			{
				Public_Variables.no_of_runs_used--;
				run_number.Text = Public_Variables.no_of_runs_used.ToString();
			}
		}
	}

	private void WatchDogButton_Click(object sender, EventArgs e)
	{
		if (Public_Variables.watchdog_active)
		{
			Public_Variables.watchdog_active = false;
			Public_Variables.watchdog_alarm = false;
			watchdog_state_Label.Text = "";
			Public_Variables.watchdog_ms_since_reset = 0;
			((ButtonBase)WatchDogButton).Text = "WatchDog is Off";
		}
		else
		{
			Public_Variables.watchdog_active = true;
			Public_Variables.watchdog_alarm = false;
			watchdog_state_Label.Text = "";
			Public_Variables.watchdog_ms_since_reset = 0;
			((ButtonBase)WatchDogButton).Text = "WatchDog is On";
		}
	}
}
