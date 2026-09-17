using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace RATS;

[DesignerGenerated]
public class single_channel_display : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Display_Timer")]
	private Timer _Display_Timer;

	private OleDbConnection MyConn;

	private OleDbDataAdapter da;

	private DataSet ds;

	private DataTableCollection tables;

	private BindingSource source1;

	private const int run_refresh_delay = 20;

	private const int score_refresh_delay = 50;

	[field: AccessedThroughProperty("Best_Score_Times_lbl")]
	internal virtual Label Best_Score_Times_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Run_Times_list_lbl")]
	internal virtual Label Run_Times_list_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Best_Scores_DataGridView")]
	internal virtual DataGridView Best_Scores_DataGridView
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Entry_Runs_DataGridView")]
	internal virtual DataGridView Entry_Runs_DataGridView
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Competition_Class")]
	internal virtual Label Competition_Class
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

	[field: AccessedThroughProperty("Best_Score_lbl")]
	internal virtual Label Best_Score_lbl
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

	[field: AccessedThroughProperty("Score_Time")]
	internal virtual Label Score_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Touches")]
	internal virtual Label Touches
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Selected_Robot")]
	internal virtual Label Selected_Robot
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Selected_Competition")]
	internal virtual Label Selected_Competition
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Event_Date_lbl")]
	internal virtual Label Event_Date_lbl
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

	[field: AccessedThroughProperty("Run_Time_lbl")]
	internal virtual Label Run_Time_lbl
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

	[field: AccessedThroughProperty("Maze_Time_lbl")]
	internal virtual Label Maze_Time_lbl
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

	[field: AccessedThroughProperty("Split_Time")]
	internal virtual Label Split_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Timer Display_Timer
	{
		[CompilerGenerated]
		get
		{
			return _Display_Timer;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Display_Timer_Tick;
			Timer val = _Display_Timer;
			if (val != null)
			{
				val.Tick -= eventHandler;
			}
			_Display_Timer = value;
			val = _Display_Timer;
			if (val != null)
			{
				val.Tick += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Label1")]
	internal virtual Label Label1
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

	[field: AccessedThroughProperty("Time_Left_Lbl")]
	internal virtual Label Time_Left_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("no_of_runs_Lbl")]
	internal virtual Label no_of_runs_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("run_of_Lbl")]
	internal virtual Label run_of_Lbl
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

	[field: AccessedThroughProperty("available_runs")]
	internal virtual Label available_runs
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public single_channel_display()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		((Form)this)._002Ector();
		((Form)this).Load += single_channel_display_Load;
		((Form)this).FormClosing += new FormClosingEventHandler(single_channel_display_Close);
		source1 = new BindingSource();
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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
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
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Expected O, but got Unknown
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Expected O, but got Unknown
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Expected O, but got Unknown
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Expected O, but got Unknown
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Expected O, but got Unknown
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Expected O, but got Unknown
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Expected O, but got Unknown
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Expected O, but got Unknown
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Expected O, but got Unknown
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Expected O, but got Unknown
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Expected O, but got Unknown
		//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5c: Expected O, but got Unknown
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0beb: Expected O, but got Unknown
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Expected O, but got Unknown
		//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d11: Expected O, but got Unknown
		//IL_0dae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db8: Expected O, but got Unknown
		//IL_0e4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e55: Expected O, but got Unknown
		//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee4: Expected O, but got Unknown
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f70: Expected O, but got Unknown
		//IL_0ff2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffc: Expected O, but got Unknown
		//IL_108f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1099: Expected O, but got Unknown
		//IL_112c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1136: Expected O, but got Unknown
		components = new Container();
		Best_Score_Times_lbl = new Label();
		Run_Times_list_lbl = new Label();
		Best_Scores_DataGridView = new DataGridView();
		Entry_Runs_DataGridView = new DataGridView();
		Competition_Class = new Label();
		Best_Score = new Label();
		Best_Score_lbl = new Label();
		Score_Time_lbl = new Label();
		Score_Time = new Label();
		Touches = new Label();
		Selected_Robot = new Label();
		Selected_Competition = new Label();
		Event_Date_lbl = new Label();
		Run_Time = new Label();
		Run_Time_lbl = new Label();
		Maze_Time = new Label();
		Maze_Time_lbl = new Label();
		Split_Time_lbl = new Label();
		Split_Time = new Label();
		Display_Timer = new Timer(components);
		Label1 = new Label();
		time_left = new Label();
		Time_Left_Lbl = new Label();
		no_of_runs_Lbl = new Label();
		run_of_Lbl = new Label();
		run_number = new Label();
		available_runs = new Label();
		((ISupportInitialize)Best_Scores_DataGridView).BeginInit();
		((ISupportInitialize)Entry_Runs_DataGridView).BeginInit();
		((Control)this).SuspendLayout();
		Best_Score_Times_lbl.AutoSize = true;
		((Control)Best_Score_Times_lbl).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score_Times_lbl).Location = new Point(13, 219);
		((Control)Best_Score_Times_lbl).Name = "Best_Score_Times_lbl";
		((Control)Best_Score_Times_lbl).Size = new Size(263, 20);
		((Control)Best_Score_Times_lbl).TabIndex = 113;
		Best_Score_Times_lbl.Text = "Best Score Times for all Robots";
		Run_Times_list_lbl.AutoSize = true;
		((Control)Run_Times_list_lbl).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Run_Times_list_lbl).Location = new Point(23, 48);
		((Control)Run_Times_list_lbl).Name = "Run_Times_list_lbl";
		((Control)Run_Times_list_lbl).Size = new Size(209, 20);
		((Control)Run_Times_list_lbl).TabIndex = 112;
		Run_Times_list_lbl.Text = "Run Times for this Robot";
		Run_Times_list_lbl.UseMnemonic = false;
		Best_Scores_DataGridView.AllowUserToAddRows = false;
		Best_Scores_DataGridView.AllowUserToDeleteRows = false;
		Best_Scores_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)10;
		Best_Scores_DataGridView.BackgroundColor = SystemColors.Control;
		Best_Scores_DataGridView.BorderStyle = (BorderStyle)2;
		Best_Scores_DataGridView.ColumnHeadersBorderStyle = (DataGridViewHeaderBorderStyle)1;
		Best_Scores_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		((Control)Best_Scores_DataGridView).Location = new Point(12, 242);
		Best_Scores_DataGridView.MultiSelect = false;
		((Control)Best_Scores_DataGridView).Name = "Best_Scores_DataGridView";
		Best_Scores_DataGridView.ReadOnly = true;
		Best_Scores_DataGridView.RowHeadersVisible = false;
		Best_Scores_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Best_Scores_DataGridView).Size = new Size(285, 309);
		((Control)Best_Scores_DataGridView).TabIndex = 111;
		Entry_Runs_DataGridView.AllowUserToAddRows = false;
		Entry_Runs_DataGridView.AllowUserToDeleteRows = false;
		Entry_Runs_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)2;
		Entry_Runs_DataGridView.BackgroundColor = SystemColors.Control;
		Entry_Runs_DataGridView.BorderStyle = (BorderStyle)2;
		Entry_Runs_DataGridView.ColumnHeadersBorderStyle = (DataGridViewHeaderBorderStyle)1;
		Entry_Runs_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		((Control)Entry_Runs_DataGridView).Location = new Point(12, 71);
		Entry_Runs_DataGridView.MultiSelect = false;
		((Control)Entry_Runs_DataGridView).Name = "Entry_Runs_DataGridView";
		Entry_Runs_DataGridView.ReadOnly = true;
		Entry_Runs_DataGridView.RowHeadersVisible = false;
		Entry_Runs_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Entry_Runs_DataGridView).Size = new Size(285, 134);
		((Control)Entry_Runs_DataGridView).TabIndex = 110;
		((Control)Competition_Class).BackColor = SystemColors.Control;
		((Control)Competition_Class).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Competition_Class).ForeColor = SystemColors.MenuHighlight;
		((Control)Competition_Class).Location = new Point(470, 0);
		((Control)Competition_Class).Name = "Competition_Class";
		((Control)Competition_Class).Size = new Size(120, 45);
		((Control)Competition_Class).TabIndex = 109;
		Competition_Class.Text = "Class";
		Competition_Class.TextAlign = (ContentAlignment)64;
		((Control)Best_Score).BackColor = SystemColors.Window;
		((Control)Best_Score).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score).Location = new Point(414, 301);
		((Control)Best_Score).Name = "Best_Score";
		((Control)Best_Score).Size = new Size(145, 51);
		((Control)Best_Score).TabIndex = 108;
		Best_Score.Text = "calc";
		Best_Score.TextAlign = (ContentAlignment)16;
		((Control)Best_Score_lbl).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score_lbl).Location = new Point(302, 301);
		((Control)Best_Score_lbl).Name = "Best_Score_lbl";
		((Control)Best_Score_lbl).Size = new Size(105, 51);
		((Control)Best_Score_lbl).TabIndex = 107;
		Best_Score_lbl.Text = "Best";
		Best_Score_lbl.TextAlign = (ContentAlignment)16;
		((Control)Score_Time_lbl).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Score_Time_lbl).Location = new Point(303, 240);
		((Control)Score_Time_lbl).Name = "Score_Time_lbl";
		((Control)Score_Time_lbl).Size = new Size(105, 51);
		((Control)Score_Time_lbl).TabIndex = 106;
		Score_Time_lbl.Text = "Score";
		Score_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Score_Time).BackColor = SystemColors.Window;
		((Control)Score_Time).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Score_Time).Location = new Point(414, 240);
		((Control)Score_Time).Name = "Score_Time";
		((Control)Score_Time).Size = new Size(145, 51);
		((Control)Score_Time).TabIndex = 105;
		Score_Time.Text = "calc";
		Score_Time.TextAlign = (ContentAlignment)16;
		((Control)Touches).BackColor = SystemColors.Window;
		((Control)Touches).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Touches).Location = new Point(415, 383);
		((Control)Touches).Name = "Touches";
		((Control)Touches).Size = new Size(145, 29);
		((Control)Touches).TabIndex = 104;
		Touches.Text = "no";
		Touches.TextAlign = (ContentAlignment)16;
		((Control)Selected_Robot).BackColor = SystemColors.Control;
		((Control)Selected_Robot).Font = new Font("Microsoft Sans Serif", 27.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Robot).ForeColor = Color.Red;
		((Control)Selected_Robot).Location = new Point(303, 48);
		((Control)Selected_Robot).Name = "Selected_Robot";
		((Control)Selected_Robot).Size = new Size(287, 46);
		((Control)Selected_Robot).TabIndex = 102;
		Selected_Robot.Text = "Robot / Practice Mode";
		Selected_Robot.TextAlign = (ContentAlignment)64;
		((Control)Selected_Competition).BackColor = SystemColors.Control;
		((Control)Selected_Competition).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Competition).ForeColor = SystemColors.MenuHighlight;
		((Control)Selected_Competition).Location = new Point(5, -2);
		((Control)Selected_Competition).Name = "Selected_Competition";
		((Control)Selected_Competition).Size = new Size(470, 47);
		((Control)Selected_Competition).TabIndex = 101;
		Selected_Competition.Text = "Selected Contest";
		Selected_Competition.TextAlign = (ContentAlignment)64;
		((Control)Event_Date_lbl).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Event_Date_lbl).ForeColor = SystemColors.MenuHighlight;
		Event_Date_lbl.ImageAlign = (ContentAlignment)64;
		((Control)Event_Date_lbl).Location = new Point(-81, 71);
		((Control)Event_Date_lbl).Name = "Event_Date_lbl";
		((Control)Event_Date_lbl).Size = new Size(177, 23);
		((Control)Event_Date_lbl).TabIndex = 99;
		Event_Date_lbl.Text = "Date";
		((Control)Run_Time).BackColor = SystemColors.Window;
		((Control)Run_Time).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Run_Time).Location = new Point(414, 179);
		((Control)Run_Time).Name = "Run_Time";
		((Control)Run_Time).Size = new Size(145, 51);
		((Control)Run_Time).TabIndex = 98;
		Run_Time.Text = "secs";
		Run_Time.TextAlign = (ContentAlignment)16;
		((Control)Run_Time_lbl).BackColor = SystemColors.Control;
		((Control)Run_Time_lbl).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Run_Time_lbl).Location = new Point(303, 179);
		((Control)Run_Time_lbl).Name = "Run_Time_lbl";
		((Control)Run_Time_lbl).Size = new Size(105, 51);
		((Control)Run_Time_lbl).TabIndex = 97;
		Run_Time_lbl.Text = "Run";
		Run_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Maze_Time).BackColor = SystemColors.Window;
		((Control)Maze_Time).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Maze_Time).Location = new Point(415, 421);
		((Control)Maze_Time).Name = "Maze_Time";
		((Control)Maze_Time).Size = new Size(145, 29);
		((Control)Maze_Time).TabIndex = 96;
		Maze_Time.Text = "secs";
		Maze_Time.TextAlign = (ContentAlignment)16;
		((Control)Maze_Time_lbl).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Maze_Time_lbl).Location = new Point(304, 421);
		((Control)Maze_Time_lbl).Name = "Maze_Time_lbl";
		((Control)Maze_Time_lbl).Size = new Size(93, 29);
		((Control)Maze_Time_lbl).TabIndex = 95;
		Maze_Time_lbl.Text = "Entry";
		Maze_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Split_Time_lbl).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Split_Time_lbl).Location = new Point(303, 118);
		((Control)Split_Time_lbl).Name = "Split_Time_lbl";
		((Control)Split_Time_lbl).Size = new Size(105, 51);
		((Control)Split_Time_lbl).TabIndex = 94;
		Split_Time_lbl.Text = "Split";
		Split_Time_lbl.TextAlign = (ContentAlignment)16;
		((Control)Split_Time).BackColor = SystemColors.Window;
		((Control)Split_Time).Font = new Font("Microsoft Sans Serif", 24f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Split_Time).Location = new Point(414, 118);
		((Control)Split_Time).Name = "Split_Time";
		((Control)Split_Time).Size = new Size(145, 51);
		((Control)Split_Time).TabIndex = 93;
		Split_Time.Text = "secs";
		Split_Time.TextAlign = (ContentAlignment)16;
		Split_Time.UseMnemonic = false;
		Display_Timer.Interval = 20;
		((Control)Label1).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Label1).Location = new Point(303, 383);
		((Control)Label1).Name = "Label1";
		((Control)Label1).Size = new Size(93, 29);
		((Control)Label1).TabIndex = 116;
		Label1.Text = "Touch";
		Label1.TextAlign = (ContentAlignment)16;
		((Control)time_left).BackColor = SystemColors.Window;
		((Control)time_left).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)time_left).Location = new Point(415, 459);
		((Control)time_left).Name = "time_left";
		((Control)time_left).Size = new Size(145, 29);
		((Control)time_left).TabIndex = 117;
		time_left.Text = "secs";
		time_left.TextAlign = (ContentAlignment)16;
		((Control)Time_Left_Lbl).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Time_Left_Lbl).Location = new Point(303, 459);
		((Control)Time_Left_Lbl).Name = "Time_Left_Lbl";
		((Control)Time_Left_Lbl).Size = new Size(93, 29);
		((Control)Time_Left_Lbl).TabIndex = 118;
		Time_Left_Lbl.Text = "Left";
		Time_Left_Lbl.TextAlign = (ContentAlignment)16;
		((Control)no_of_runs_Lbl).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)no_of_runs_Lbl).Location = new Point(304, 518);
		((Control)no_of_runs_Lbl).Name = "no_of_runs_Lbl";
		((Control)no_of_runs_Lbl).Size = new Size(93, 29);
		((Control)no_of_runs_Lbl).TabIndex = 119;
		no_of_runs_Lbl.Text = "Run #";
		no_of_runs_Lbl.TextAlign = (ContentAlignment)16;
		((Control)run_of_Lbl).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)run_of_Lbl).Location = new Point(471, 518);
		((Control)run_of_Lbl).Name = "run_of_Lbl";
		((Control)run_of_Lbl).Size = new Size(45, 29);
		((Control)run_of_Lbl).TabIndex = 120;
		run_of_Lbl.Text = "of";
		run_of_Lbl.TextAlign = (ContentAlignment)16;
		((Control)run_number).BackColor = SystemColors.Window;
		((Control)run_number).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)run_number).Location = new Point(415, 518);
		((Control)run_number).Name = "run_number";
		((Control)run_number).Size = new Size(50, 29);
		((Control)run_number).TabIndex = 121;
		run_number.Text = "no";
		run_number.TextAlign = (ContentAlignment)16;
		((Control)available_runs).BackColor = SystemColors.Window;
		((Control)available_runs).Font = new Font("Microsoft Sans Serif", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)available_runs).Location = new Point(509, 518);
		((Control)available_runs).Name = "available_runs";
		((Control)available_runs).Size = new Size(50, 29);
		((Control)available_runs).TabIndex = 122;
		available_runs.Text = "no";
		available_runs.TextAlign = (ContentAlignment)16;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(596, 556);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)available_runs);
		((Control)this).Controls.Add((Control)(object)run_number);
		((Control)this).Controls.Add((Control)(object)run_of_Lbl);
		((Control)this).Controls.Add((Control)(object)no_of_runs_Lbl);
		((Control)this).Controls.Add((Control)(object)Time_Left_Lbl);
		((Control)this).Controls.Add((Control)(object)time_left);
		((Control)this).Controls.Add((Control)(object)Label1);
		((Control)this).Controls.Add((Control)(object)Best_Score_Times_lbl);
		((Control)this).Controls.Add((Control)(object)Run_Times_list_lbl);
		((Control)this).Controls.Add((Control)(object)Best_Scores_DataGridView);
		((Control)this).Controls.Add((Control)(object)Entry_Runs_DataGridView);
		((Control)this).Controls.Add((Control)(object)Competition_Class);
		((Control)this).Controls.Add((Control)(object)Best_Score);
		((Control)this).Controls.Add((Control)(object)Best_Score_lbl);
		((Control)this).Controls.Add((Control)(object)Score_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Score_Time);
		((Control)this).Controls.Add((Control)(object)Touches);
		((Control)this).Controls.Add((Control)(object)Selected_Robot);
		((Control)this).Controls.Add((Control)(object)Selected_Competition);
		((Control)this).Controls.Add((Control)(object)Event_Date_lbl);
		((Control)this).Controls.Add((Control)(object)Run_Time);
		((Control)this).Controls.Add((Control)(object)Run_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Maze_Time);
		((Control)this).Controls.Add((Control)(object)Maze_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Split_Time_lbl);
		((Control)this).Controls.Add((Control)(object)Split_Time);
		((Control)this).Name = "single_channel_display";
		((Form)this).Text = "RATS v3.8.0 single channel display";
		((ISupportInitialize)Best_Scores_DataGridView).EndInit();
		((ISupportInitialize)Entry_Runs_DataGridView).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	private void single_channel_display_Load(object sender, EventArgs e)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		Public_Variables.single_channel_window_open = true;
		Display_Timer.Enabled = true;
		if (Public_Variables.database_available)
		{
			MyConn = new OleDbConnection();
			MyConn.ConnectionString = Public_Variables.connString;
			MyConn.Open();
		}
		Public_Variables.refresh_best_score_times_delay = 50;
		Public_Variables.refresh_run_times_delay = 20;
	}

	private void single_channel_display_Close(object sender, FormClosingEventArgs e)
	{
		Public_Variables.single_channel_window_open = false;
		Display_Timer.Enabled = false;
	}

	private void Display_Timer_Tick(object sender, EventArgs e)
	{
		if (Public_Variables.request_single_channel_window_close)
		{
			Public_Variables.request_single_channel_window_close = false;
			((Form)this).Close();
			return;
		}
		checked
		{
			Public_Variables.refresh_best_score_times_delay++;
			if (Public_Variables.refresh_best_score_times_delay > 50)
			{
				refresh_best_scores();
				Public_Variables.refresh_best_score_times = false;
				Public_Variables.refresh_best_score_times_delay = 0;
			}
			Split_Time.Text = Strings.Format((double)Public_Variables.split_time_ms / 1000.0, "F2");
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
			Touches.Text = Strings.Format(Public_Variables.no_of_touches);
			Maze_Time.Text = Strings.Format((double)Public_Variables.maze_time_ms / 1000.0, "F2");
			if (Public_Variables.practice_mode)
			{
				Selected_Competition.Text = "";
				Competition_Class.Text = "";
				time_left.Text = "";
				run_number.Text = "";
				available_runs.Text = "";
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
						((Control)Time_Left_Lbl).ForeColor = Color.Orange;
					}
					else
					{
						((Control)time_left).ForeColor = Color.Red;
						((Control)Time_Left_Lbl).ForeColor = Color.Red;
					}
				}
				else
				{
					((Control)time_left).ForeColor = Color.Black;
					((Control)Time_Left_Lbl).ForeColor = Color.Black;
				}
				run_number.Text = Strings.Format(Public_Variables.no_of_runs_used);
				if (Public_Variables.no_of_runs_used > Public_Variables.no_of_runs_allowed)
				{
					((Control)run_number).ForeColor = Color.Red;
					((Control)no_of_runs_Lbl).ForeColor = Color.Red;
				}
				else
				{
					((Control)run_number).ForeColor = Color.Black;
					((Control)no_of_runs_Lbl).ForeColor = Color.Black;
				}
				available_runs.Text = Strings.Format(Public_Variables.no_of_runs_allowed);
				if ((((Control)run_number).ForeColor == Color.Red) | (((Control)time_left).ForeColor == Color.Red))
				{
					Split_Time.Text = "End";
					((Control)Split_Time).ForeColor = Color.Red;
				}
				else
				{
					((Control)Split_Time).ForeColor = Color.Black;
				}
				Selected_Competition.Text = Public_Variables.public_competition_name;
				Competition_Class.Text = Public_Variables.public_competition_class;
				Selected_Robot.Text = Public_Variables.robot;
			}
			if (Public_Variables.hide_robot_runtimes)
			{
				Public_Variables.hide_robot_runtimes = false;
				((Control)Entry_Runs_DataGridView).Visible = false;
			}
			if (Public_Variables.hide_best_score_times)
			{
				Public_Variables.hide_best_score_times = false;
				((Control)Best_Scores_DataGridView).Visible = false;
			}
			if (Public_Variables.refresh_robot_run_times)
			{
				Public_Variables.refresh_run_times_delay++;
				if (Public_Variables.refresh_run_times_delay > 20)
				{
					refresh_entry_runs();
					Public_Variables.refresh_robot_run_times = false;
					Public_Variables.refresh_run_times_delay = 0;
				}
			}
		}
	}

	private void refresh_best_scores()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		if (!Public_Variables.practice_mode & Public_Variables.database_available)
		{
			((Control)Best_Scores_DataGridView).Visible = true;
			ds = new DataSet();
			tables = ds.Tables;
			string text = "SELECT Mouse_Name, Run_Time_mS/1000, Score_Time_mS/1000, Contestant_Name FROM [Best_Score_Time] WHERE (Competition_ID = " + Public_Variables.competition_id + ") ORDER BY Score_Time_mS";
			da = new OleDbDataAdapter(text, MyConn);
			((DbDataAdapter)(object)da).Fill(ds, "Best_Score_Time");
			DataView dataSource = new DataView(tables[0]);
			source1.DataSource = dataSource;
			Best_Scores_DataGridView.DataSource = dataSource;
			Best_Scores_DataGridView.Columns[0].HeaderText = "Robot";
			Best_Scores_DataGridView.Columns[1].HeaderText = "Run";
			Best_Scores_DataGridView.Columns[2].HeaderText = "Score";
			Best_Scores_DataGridView.Columns[3].HeaderText = "Contestant";
			((Control)Best_Scores_DataGridView).Refresh();
			ds.Dispose();
		}
	}

	private void refresh_entry_runs()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		if (!Public_Variables.practice_mode & Public_Variables.database_available)
		{
			((Control)Entry_Runs_DataGridView).Visible = true;
			ds = new DataSet();
			tables = ds.Tables;
			string text = "SELECT Time_of_Run, Run_Time_mSecs/1000, Touches, Score_Time_mSecs/1000 FROM [Entry_Run] WHERE (Entry_ID = " + Public_Variables.entry_id + ") ORDER BY Time_of_Run DESC";
			da = new OleDbDataAdapter(text, MyConn);
			((DbDataAdapter)(object)da).Fill(ds, "Entry_Run");
			DataView dataSource = new DataView(tables[0]);
			Entry_Runs_DataGridView.DataSource = dataSource;
			Entry_Runs_DataGridView.Columns[0].HeaderText = "Time of Run";
			Entry_Runs_DataGridView.Columns[1].HeaderText = "Run";
			Entry_Runs_DataGridView.Columns[3].HeaderText = "Score";
			Entry_Runs_DataGridView.Columns[0].DefaultCellStyle.Format = "T";
			((Control)Entry_Runs_DataGridView).Refresh();
			ds.Dispose();
		}
	}
}
