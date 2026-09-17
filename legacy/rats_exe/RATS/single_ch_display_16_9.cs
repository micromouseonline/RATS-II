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
public class single_ch_display_16_9 : Form
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

	[field: AccessedThroughProperty("Run_order_DataGridView")]
	internal virtual DataGridView Run_order_DataGridView
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Running_order_GroupBox")]
	internal virtual GroupBox Running_order_GroupBox
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Rank_label_lbl")]
	internal virtual Label Rank_label_lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Rank_label")]
	internal virtual Label Rank_label
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Contestant_Label")]
	internal virtual Label Contestant_Label
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

	[field: AccessedThroughProperty("available_runs")]
	internal virtual Label available_runs
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

	[field: AccessedThroughProperty("no_of_runs_Lbl")]
	internal virtual Label no_of_runs_Lbl
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

	[field: AccessedThroughProperty("time_left")]
	internal virtual Label time_left
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label1")]
	internal virtual Label Label1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

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

	[field: AccessedThroughProperty("Best_Score")]
	internal virtual Label Best_Score
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

	[field: AccessedThroughProperty("run_number")]
	internal virtual Label run_number
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

	[field: AccessedThroughProperty("Best_Score_lbl")]
	internal virtual Label Best_Score_lbl
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

	[field: AccessedThroughProperty("Split_Time")]
	internal virtual Label Split_Time
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public single_ch_display_16_9()
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
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
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Expected O, but got Unknown
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Expected O, but got Unknown
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Expected O, but got Unknown
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Expected O, but got Unknown
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Expected O, but got Unknown
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Expected O, but got Unknown
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Expected O, but got Unknown
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Expected O, but got Unknown
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Expected O, but got Unknown
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Expected O, but got Unknown
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Expected O, but got Unknown
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8a: Expected O, but got Unknown
		//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b97: Expected O, but got Unknown
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Expected O, but got Unknown
		//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d50: Expected O, but got Unknown
		//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc4: Expected O, but got Unknown
		//IL_0eab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb5: Expected O, but got Unknown
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Expected O, but got Unknown
		//IL_101e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1028: Expected O, but got Unknown
		//IL_10bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c5: Expected O, but got Unknown
		//IL_116f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1179: Expected O, but got Unknown
		//IL_121d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1227: Expected O, but got Unknown
		//IL_12cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d5: Expected O, but got Unknown
		//IL_137b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1385: Expected O, but got Unknown
		components = new Container();
		DataGridViewCellStyle val = new DataGridViewCellStyle();
		DataGridViewCellStyle val2 = new DataGridViewCellStyle();
		DataGridViewCellStyle val3 = new DataGridViewCellStyle();
		DataGridViewCellStyle val4 = new DataGridViewCellStyle();
		Run_order_DataGridView = new DataGridView();
		Running_order_GroupBox = new GroupBox();
		Rank_label_lbl = new Label();
		Rank_label = new Label();
		Contestant_Label = new Label();
		Event_Name_lbl = new Label();
		available_runs = new Label();
		run_of_Lbl = new Label();
		no_of_runs_Lbl = new Label();
		Time_Left_Lbl = new Label();
		time_left = new Label();
		Label1 = new Label();
		Best_Score_Times_lbl = new Label();
		Run_Times_list_lbl = new Label();
		Best_Scores_DataGridView = new DataGridView();
		Entry_Runs_DataGridView = new DataGridView();
		Best_Score = new Label();
		Display_Timer = new Timer(components);
		run_number = new Label();
		Competition_Class = new Label();
		Best_Score_lbl = new Label();
		Touches = new Label();
		Selected_Robot = new Label();
		Selected_Competition = new Label();
		Split_Time = new Label();
		((ISupportInitialize)Run_order_DataGridView).BeginInit();
		((Control)Running_order_GroupBox).SuspendLayout();
		((ISupportInitialize)Best_Scores_DataGridView).BeginInit();
		((ISupportInitialize)Entry_Runs_DataGridView).BeginInit();
		((Control)this).SuspendLayout();
		Run_order_DataGridView.AllowUserToAddRows = false;
		Run_order_DataGridView.AllowUserToDeleteRows = false;
		Run_order_DataGridView.AllowUserToResizeRows = false;
		Run_order_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)16;
		Run_order_DataGridView.BackgroundColor = SystemColors.Control;
		Run_order_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		Run_order_DataGridView.ColumnHeadersVisible = false;
		((Control)Run_order_DataGridView).Location = new Point(12, 38);
		((Control)Run_order_DataGridView).Name = "Run_order_DataGridView";
		Run_order_DataGridView.RowHeadersVisible = false;
		Run_order_DataGridView.RowHeadersWidth = 80;
		Run_order_DataGridView.RowHeadersWidthSizeMode = (DataGridViewRowHeadersWidthSizeMode)1;
		((Control)Run_order_DataGridView).Size = new Size(166, 613);
		((Control)Run_order_DataGridView).TabIndex = 1;
		((Control)Running_order_GroupBox).BackColor = SystemColors.Control;
		((Control)Running_order_GroupBox).Controls.Add((Control)(object)Run_order_DataGridView);
		((Control)Running_order_GroupBox).Font = new Font("Microsoft Sans Serif", 11.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Running_order_GroupBox).ForeColor = SystemColors.ControlText;
		((Control)Running_order_GroupBox).Location = new Point(1068, 12);
		((Control)Running_order_GroupBox).Name = "Running_order_GroupBox";
		((Control)Running_order_GroupBox).Size = new Size(184, 657);
		((Control)Running_order_GroupBox).TabIndex = 183;
		Running_order_GroupBox.TabStop = false;
		Running_order_GroupBox.Text = "Running Order of Next Entries";
		((Control)Rank_label_lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Rank_label_lbl).ForeColor = SystemColors.ControlText;
		((Control)Rank_label_lbl).Location = new Point(958, 277);
		((Control)Rank_label_lbl).Name = "Rank_label_lbl";
		((Control)Rank_label_lbl).Size = new Size(72, 22);
		((Control)Rank_label_lbl).TabIndex = 182;
		Rank_label_lbl.Text = "Rank";
		Rank_label_lbl.TextAlign = (ContentAlignment)16;
		((Control)Rank_label).BackColor = Color.DarkSlateBlue;
		((Control)Rank_label).Font = new Font("Courier New", 48f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Rank_label).ForeColor = Color.Yellow;
		((Control)Rank_label).Location = new Point(954, 302);
		((Control)Rank_label).Name = "Rank_label";
		((Control)Rank_label).Size = new Size(108, 60);
		((Control)Rank_label).TabIndex = 181;
		Rank_label.Text = "00";
		Rank_label.TextAlign = (ContentAlignment)16;
		((Control)Contestant_Label).BackColor = SystemColors.Control;
		((Control)Contestant_Label).Font = new Font("Arial Black", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Contestant_Label).ForeColor = Color.Black;
		((Control)Contestant_Label).Location = new Point(411, 43);
		((Control)Contestant_Label).Name = "Contestant_Label";
		((Control)Contestant_Label).Size = new Size(380, 32);
		((Control)Contestant_Label).TabIndex = 180;
		Contestant_Label.Text = "Contestant";
		Contestant_Label.TextAlign = (ContentAlignment)16;
		((Control)Event_Name_lbl).BackColor = SystemColors.Control;
		((Control)Event_Name_lbl).Font = new Font("Arial Black", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Event_Name_lbl).ForeColor = SystemColors.MenuHighlight;
		((Control)Event_Name_lbl).Location = new Point(12, 2);
		((Control)Event_Name_lbl).Name = "Event_Name_lbl";
		((Control)Event_Name_lbl).Size = new Size(399, 43);
		((Control)Event_Name_lbl).TabIndex = 179;
		Event_Name_lbl.Text = "Event";
		Event_Name_lbl.TextAlign = (ContentAlignment)16;
		((Control)available_runs).BackColor = Color.DarkSlateBlue;
		((Control)available_runs).Font = new Font("Courier New", 27.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)available_runs).ForeColor = Color.Yellow;
		((Control)available_runs).Location = new Point(270, 326);
		((Control)available_runs).Name = "available_runs";
		((Control)available_runs).Size = new Size(64, 36);
		((Control)available_runs).TabIndex = 178;
		available_runs.Text = "00";
		available_runs.TextAlign = (ContentAlignment)16;
		((Control)run_of_Lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)run_of_Lbl).ForeColor = SystemColors.ControlText;
		((Control)run_of_Lbl).Location = new Point(272, 301);
		((Control)run_of_Lbl).Name = "run_of_Lbl";
		((Control)run_of_Lbl).Size = new Size(45, 22);
		((Control)run_of_Lbl).TabIndex = 176;
		run_of_Lbl.Text = "Of";
		run_of_Lbl.TextAlign = (ContentAlignment)16;
		((Control)no_of_runs_Lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)no_of_runs_Lbl).ForeColor = SystemColors.ControlText;
		((Control)no_of_runs_Lbl).Location = new Point(164, 277);
		((Control)no_of_runs_Lbl).Name = "no_of_runs_Lbl";
		((Control)no_of_runs_Lbl).Size = new Size(72, 22);
		((Control)no_of_runs_Lbl).TabIndex = 175;
		no_of_runs_Lbl.Text = "Run No";
		no_of_runs_Lbl.TextAlign = (ContentAlignment)16;
		((Control)Time_Left_Lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Time_Left_Lbl).ForeColor = SystemColors.ControlText;
		((Control)Time_Left_Lbl).Location = new Point(457, 279);
		((Control)Time_Left_Lbl).Name = "Time_Left_Lbl";
		((Control)Time_Left_Lbl).Size = new Size(122, 22);
		((Control)Time_Left_Lbl).TabIndex = 174;
		Time_Left_Lbl.Text = "Time Left";
		Time_Left_Lbl.TextAlign = (ContentAlignment)64;
		((Control)time_left).BackColor = Color.DarkSlateBlue;
		((Control)time_left).Font = new Font("Courier New", 48f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)time_left).ForeColor = Color.LimeGreen;
		((Control)time_left).Location = new Point(356, 302);
		((Control)time_left).Name = "time_left";
		((Control)time_left).Size = new Size(223, 60);
		((Control)time_left).TabIndex = 173;
		time_left.Text = "00:00";
		time_left.TextAlign = (ContentAlignment)16;
		((Control)Label1).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Label1).ForeColor = SystemColors.ControlText;
		((Control)Label1).Location = new Point(18, 277);
		((Control)Label1).Name = "Label1";
		((Control)Label1).Size = new Size(102, 22);
		((Control)Label1).TabIndex = 172;
		Label1.Text = "Touches";
		Label1.TextAlign = (ContentAlignment)16;
		Best_Score_Times_lbl.AutoSize = true;
		((Control)Best_Score_Times_lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score_Times_lbl).ForeColor = SystemColors.ControlText;
		((Control)Best_Score_Times_lbl).Location = new Point(607, 374);
		((Control)Best_Score_Times_lbl).Name = "Best_Score_Times_lbl";
		((Control)Best_Score_Times_lbl).Size = new Size(342, 27);
		((Control)Best_Score_Times_lbl).TabIndex = 171;
		Best_Score_Times_lbl.Text = "Best Score Times for all Robots";
		Run_Times_list_lbl.AutoSize = true;
		((Control)Run_Times_list_lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Run_Times_list_lbl).ForeColor = SystemColors.ControlText;
		((Control)Run_Times_list_lbl).Location = new Point(18, 374);
		((Control)Run_Times_list_lbl).Name = "Run_Times_list_lbl";
		((Control)Run_Times_list_lbl).Size = new Size(123, 27);
		((Control)Run_Times_list_lbl).TabIndex = 170;
		Run_Times_list_lbl.Text = "Run Times";
		Run_Times_list_lbl.TextAlign = (ContentAlignment)16;
		Run_Times_list_lbl.UseMnemonic = false;
		Best_Scores_DataGridView.AllowUserToAddRows = false;
		Best_Scores_DataGridView.AllowUserToDeleteRows = false;
		Best_Scores_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)10;
		Best_Scores_DataGridView.BackgroundColor = SystemColors.Control;
		Best_Scores_DataGridView.BorderStyle = (BorderStyle)2;
		Best_Scores_DataGridView.ColumnHeadersBorderStyle = (DataGridViewHeaderBorderStyle)1;
		val.Alignment = (DataGridViewContentAlignment)16;
		val.BackColor = SystemColors.Control;
		val.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		val.ForeColor = SystemColors.WindowText;
		val.SelectionBackColor = SystemColors.Highlight;
		val.SelectionForeColor = SystemColors.HighlightText;
		val.WrapMode = (DataGridViewTriState)1;
		Best_Scores_DataGridView.ColumnHeadersDefaultCellStyle = val;
		Best_Scores_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		val2.Alignment = (DataGridViewContentAlignment)16;
		val2.BackColor = SystemColors.Window;
		val2.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		val2.ForeColor = SystemColors.ControlText;
		val2.SelectionBackColor = SystemColors.Highlight;
		val2.SelectionForeColor = SystemColors.HighlightText;
		val2.WrapMode = (DataGridViewTriState)2;
		Best_Scores_DataGridView.DefaultCellStyle = val2;
		((Control)Best_Scores_DataGridView).Location = new Point(437, 404);
		Best_Scores_DataGridView.MultiSelect = false;
		((Control)Best_Scores_DataGridView).Name = "Best_Scores_DataGridView";
		Best_Scores_DataGridView.ReadOnly = true;
		Best_Scores_DataGridView.RowHeadersVisible = false;
		Best_Scores_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Best_Scores_DataGridView).Size = new Size(625, 265);
		((Control)Best_Scores_DataGridView).TabIndex = 169;
		Entry_Runs_DataGridView.AllowUserToAddRows = false;
		Entry_Runs_DataGridView.AllowUserToDeleteRows = false;
		Entry_Runs_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)2;
		Entry_Runs_DataGridView.BackgroundColor = SystemColors.Control;
		Entry_Runs_DataGridView.BorderStyle = (BorderStyle)2;
		Entry_Runs_DataGridView.ColumnHeadersBorderStyle = (DataGridViewHeaderBorderStyle)1;
		val3.Alignment = (DataGridViewContentAlignment)16;
		val3.BackColor = SystemColors.Control;
		val3.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		val3.ForeColor = SystemColors.WindowText;
		val3.SelectionBackColor = SystemColors.Highlight;
		val3.SelectionForeColor = SystemColors.HighlightText;
		val3.WrapMode = (DataGridViewTriState)1;
		Entry_Runs_DataGridView.ColumnHeadersDefaultCellStyle = val3;
		Entry_Runs_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		val4.Alignment = (DataGridViewContentAlignment)16;
		val4.BackColor = SystemColors.Window;
		val4.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		val4.ForeColor = SystemColors.ControlText;
		val4.SelectionBackColor = SystemColors.Highlight;
		val4.SelectionForeColor = SystemColors.HighlightText;
		val4.WrapMode = (DataGridViewTriState)2;
		Entry_Runs_DataGridView.DefaultCellStyle = val4;
		((Control)Entry_Runs_DataGridView).Location = new Point(15, 404);
		Entry_Runs_DataGridView.MultiSelect = false;
		((Control)Entry_Runs_DataGridView).Name = "Entry_Runs_DataGridView";
		Entry_Runs_DataGridView.ReadOnly = true;
		Entry_Runs_DataGridView.RowHeadersVisible = false;
		Entry_Runs_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Entry_Runs_DataGridView).Size = new Size(384, 265);
		((Control)Entry_Runs_DataGridView).TabIndex = 168;
		((Control)Best_Score).BackColor = Color.DarkSlateBlue;
		((Control)Best_Score).Font = new Font("Courier New", 48f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score).ForeColor = Color.Yellow;
		((Control)Best_Score).Location = new Point(599, 302);
		((Control)Best_Score).Name = "Best_Score";
		((Control)Best_Score).Size = new Size(340, 60);
		((Control)Best_Score).TabIndex = 166;
		Best_Score.Text = "0:00.000";
		Best_Score.TextAlign = (ContentAlignment)32;
		Display_Timer.Interval = 20;
		((Control)run_number).BackColor = Color.DarkSlateBlue;
		((Control)run_number).Font = new Font("Courier New", 48f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)run_number).ForeColor = Color.Yellow;
		((Control)run_number).Location = new Point(156, 302);
		((Control)run_number).Name = "run_number";
		((Control)run_number).Size = new Size(108, 60);
		((Control)run_number).TabIndex = 177;
		run_number.Text = "00";
		run_number.TextAlign = (ContentAlignment)16;
		((Control)Competition_Class).BackColor = SystemColors.Control;
		((Control)Competition_Class).Font = new Font("Arial Black", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Competition_Class).ForeColor = SystemColors.MenuHighlight;
		((Control)Competition_Class).Location = new Point(933, 38);
		((Control)Competition_Class).Name = "Competition_Class";
		((Control)Competition_Class).Size = new Size(127, 43);
		((Control)Competition_Class).TabIndex = 167;
		Competition_Class.Text = "Class";
		Competition_Class.TextAlign = (ContentAlignment)64;
		((Control)Best_Score_lbl).Font = new Font("Arial Black", 14f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Best_Score_lbl).ForeColor = SystemColors.ControlText;
		((Control)Best_Score_lbl).Location = new Point(607, 277);
		((Control)Best_Score_lbl).Name = "Best_Score_lbl";
		((Control)Best_Score_lbl).Size = new Size(145, 22);
		((Control)Best_Score_lbl).TabIndex = 165;
		Best_Score_lbl.Text = "Best Score";
		Best_Score_lbl.TextAlign = (ContentAlignment)16;
		((Control)Touches).BackColor = Color.DarkSlateBlue;
		((Control)Touches).Font = new Font("Courier New", 48f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Touches).ForeColor = Color.Yellow;
		((Control)Touches).Location = new Point(12, 302);
		((Control)Touches).Name = "Touches";
		((Control)Touches).Size = new Size(108, 60);
		((Control)Touches).TabIndex = 162;
		Touches.Text = "00";
		Touches.TextAlign = (ContentAlignment)16;
		((Control)Selected_Robot).BackColor = SystemColors.Control;
		((Control)Selected_Robot).Font = new Font("Arial Black", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Robot).ForeColor = Color.Black;
		((Control)Selected_Robot).Location = new Point(12, 40);
		((Control)Selected_Robot).Name = "Selected_Robot";
		((Control)Selected_Robot).Size = new Size(393, 41);
		((Control)Selected_Robot).TabIndex = 161;
		Selected_Robot.Text = "Robot/Practice Mode";
		Selected_Robot.TextAlign = (ContentAlignment)16;
		((Control)Selected_Competition).BackColor = SystemColors.Control;
		((Control)Selected_Competition).Font = new Font("Arial Black", 20.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Competition).ForeColor = SystemColors.MenuHighlight;
		((Control)Selected_Competition).Location = new Point(644, 2);
		((Control)Selected_Competition).Name = "Selected_Competition";
		((Control)Selected_Competition).Size = new Size(418, 43);
		((Control)Selected_Competition).TabIndex = 160;
		Selected_Competition.Text = "Selected Contest";
		Selected_Competition.TextAlign = (ContentAlignment)64;
		((Control)Split_Time).BackColor = Color.DarkSlateBlue;
		((Control)Split_Time).Font = new Font("Courier New", 148f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Split_Time).ForeColor = Color.Yellow;
		((Control)Split_Time).Location = new Point(12, 81);
		((Control)Split_Time).Name = "Split_Time";
		((Control)Split_Time).Size = new Size(1050, 183);
		((Control)Split_Time).TabIndex = 154;
		Split_Time.Text = "0:00.000";
		Split_Time.TextAlign = (ContentAlignment)2;
		Split_Time.UseMnemonic = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(1264, 681);
		((Control)this).Controls.Add((Control)(object)Running_order_GroupBox);
		((Control)this).Controls.Add((Control)(object)Rank_label_lbl);
		((Control)this).Controls.Add((Control)(object)Rank_label);
		((Control)this).Controls.Add((Control)(object)Contestant_Label);
		((Control)this).Controls.Add((Control)(object)Event_Name_lbl);
		((Control)this).Controls.Add((Control)(object)available_runs);
		((Control)this).Controls.Add((Control)(object)run_of_Lbl);
		((Control)this).Controls.Add((Control)(object)no_of_runs_Lbl);
		((Control)this).Controls.Add((Control)(object)Time_Left_Lbl);
		((Control)this).Controls.Add((Control)(object)time_left);
		((Control)this).Controls.Add((Control)(object)Label1);
		((Control)this).Controls.Add((Control)(object)Best_Score_Times_lbl);
		((Control)this).Controls.Add((Control)(object)Run_Times_list_lbl);
		((Control)this).Controls.Add((Control)(object)Best_Scores_DataGridView);
		((Control)this).Controls.Add((Control)(object)Entry_Runs_DataGridView);
		((Control)this).Controls.Add((Control)(object)Best_Score);
		((Control)this).Controls.Add((Control)(object)run_number);
		((Control)this).Controls.Add((Control)(object)Competition_Class);
		((Control)this).Controls.Add((Control)(object)Best_Score_lbl);
		((Control)this).Controls.Add((Control)(object)Touches);
		((Control)this).Controls.Add((Control)(object)Selected_Robot);
		((Control)this).Controls.Add((Control)(object)Selected_Competition);
		((Control)this).Controls.Add((Control)(object)Split_Time);
		((Control)this).Name = "single_ch_display_16_9";
		((Form)this).Text = "single_ch_display_16_9";
		((ISupportInitialize)Run_order_DataGridView).EndInit();
		((Control)Running_order_GroupBox).ResumeLayout(false);
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
		Event_Name_lbl.Text = Public_Variables.robotics_event;
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
				refresh_running_order();
				Public_Variables.refresh_best_score_times = false;
				Public_Variables.refresh_best_score_times_delay = 0;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)Public_Variables.split_time_ms / 1000.0);
			Split_Time.Text = timeSpan.ToString("m\\:ss\\.fff");
			if (Public_Variables.fastest_score_time_this_robot > 0)
			{
				TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)Public_Variables.fastest_score_time_this_robot / 1000.0);
				Best_Score.Text = timeSpan2.ToString("m\\:ss\\.fff");
				Public_Variables.hide_robot_runtimes = false;
			}
			else
			{
				Best_Score.Text = "00:00.000";
				Public_Variables.hide_robot_runtimes = true;
			}
			Touches.Text = Strings.Format(Public_Variables.no_of_touches);
			if (Public_Variables.practice_mode)
			{
				Selected_Competition.Text = "";
				Competition_Class.Text = "";
				time_left.Text = "";
				run_number.Text = "";
				available_runs.Text = "";
				Selected_Robot.Text = "Practice Mode";
				Contestant_Label.Text = "";
				Rank_label.Text = "";
				((Control)Run_order_DataGridView).Visible = false;
				((Control)Best_Scores_DataGridView).Visible = false;
				((Control)Entry_Runs_DataGridView).Visible = false;
			}
			else
			{
				TimeSpan timeSpan3 = TimeSpan.FromSeconds((double)Public_Variables.time_left_ms / 1000.0);
				time_left.Text = timeSpan3.ToString("m\\:ss");
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
					((Control)time_left).ForeColor = Color.LimeGreen;
				}
				run_number.Text = Strings.Format(Public_Variables.no_of_runs_used);
				if (Public_Variables.no_of_runs_used > Public_Variables.no_of_runs_allowed)
				{
					((Control)run_number).ForeColor = Color.Red;
				}
				else
				{
					((Control)run_number).ForeColor = Color.Yellow;
				}
				available_runs.Text = Strings.Format(Public_Variables.no_of_runs_allowed);
				if ((((Control)run_number).ForeColor == Color.Red) | (((Control)time_left).ForeColor == Color.Red))
				{
					Split_Time.Text = "End";
					((Control)Split_Time).ForeColor = Color.Red;
				}
				else
				{
					((Control)Split_Time).ForeColor = Color.Yellow;
				}
				Selected_Competition.Text = Public_Variables.public_competition_name;
				Competition_Class.Text = Public_Variables.public_competition_class;
				Selected_Robot.Text = Public_Variables.robot;
				Contestant_Label.Text = Public_Variables.contestant;
				Rank_label.Text = Public_Variables.robot_rank.ToString();
				((Control)Run_order_DataGridView).Visible = true;
				((Control)Best_Scores_DataGridView).Visible = true;
				((Control)Entry_Runs_DataGridView).Visible = true;
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

	private void refresh_running_order()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (!Public_Variables.practice_mode & Public_Variables.database_available)
		{
			string text = "SELECT Mouse_Name AS Robot FROM [Entry] WHERE ((Competition_ID = " + Public_Variables.competition_id + ") AND (Entry_Used = FALSE)) ORDER BY Sequence_Number";
			BindingSource val = new BindingSource();
			DataSet dataSet = new DataSet();
			DataTableCollection dataTableCollection = dataSet.Tables;
			OleDbDataAdapter val2 = new OleDbDataAdapter(text, MyConn);
			((DbDataAdapter)(object)val2).Fill(dataSet);
			DataView dataSource = (DataView)(val.DataSource = new DataView(dataTableCollection[0]));
			Run_order_DataGridView.DataSource = dataSource;
		}
	}
}
