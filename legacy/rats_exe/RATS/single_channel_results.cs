using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace RATS;

[DesignerGenerated]
public class single_channel_results : Form
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

	[field: AccessedThroughProperty("Best_Scores_DataGridView")]
	internal virtual DataGridView Best_Scores_DataGridView
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

	public single_channel_results()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		((Form)this)._002Ector();
		((Form)this).Load += single_channel_display_Load;
		((Form)this).FormClosing += new FormClosingEventHandler(single_channel_results_Close);
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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Expected O, but got Unknown
		components = new Container();
		DataGridViewCellStyle val = new DataGridViewCellStyle();
		DataGridViewCellStyle val2 = new DataGridViewCellStyle();
		Best_Scores_DataGridView = new DataGridView();
		Competition_Class = new Label();
		Selected_Competition = new Label();
		Event_Date_lbl = new Label();
		Display_Timer = new Timer(components);
		Label1 = new Label();
		((ISupportInitialize)Best_Scores_DataGridView).BeginInit();
		((Control)this).SuspendLayout();
		Best_Scores_DataGridView.AllowUserToAddRows = false;
		Best_Scores_DataGridView.AllowUserToDeleteRows = false;
		Best_Scores_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)6;
		Best_Scores_DataGridView.BackgroundColor = SystemColors.Control;
		Best_Scores_DataGridView.BorderStyle = (BorderStyle)0;
		Best_Scores_DataGridView.ColumnHeadersBorderStyle = (DataGridViewHeaderBorderStyle)1;
		val.Alignment = (DataGridViewContentAlignment)16;
		val.BackColor = SystemColors.Control;
		val.Font = new Font("Microsoft Sans Serif", 9f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		val.ForeColor = SystemColors.WindowText;
		val.SelectionBackColor = SystemColors.Highlight;
		val.SelectionForeColor = SystemColors.HighlightText;
		val.WrapMode = (DataGridViewTriState)1;
		Best_Scores_DataGridView.ColumnHeadersDefaultCellStyle = val;
		Best_Scores_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		((Control)Best_Scores_DataGridView).Location = new Point(74, 46);
		Best_Scores_DataGridView.MultiSelect = false;
		((Control)Best_Scores_DataGridView).Name = "Best_Scores_DataGridView";
		Best_Scores_DataGridView.ReadOnly = true;
		Best_Scores_DataGridView.RowHeadersVisible = false;
		val2.Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		Best_Scores_DataGridView.RowsDefaultCellStyle = val2;
		Best_Scores_DataGridView.SelectionMode = (DataGridViewSelectionMode)1;
		((Control)Best_Scores_DataGridView).Size = new Size(515, 499);
		((Control)Best_Scores_DataGridView).TabIndex = 111;
		((Control)Competition_Class).BackColor = SystemColors.Control;
		((Control)Competition_Class).Font = new Font("Microsoft Sans Serif", 22f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Competition_Class).ForeColor = SystemColors.MenuHighlight;
		((Control)Competition_Class).Location = new Point(549, 3);
		((Control)Competition_Class).Name = "Competition_Class";
		((Control)Competition_Class).Size = new Size(120, 40);
		((Control)Competition_Class).TabIndex = 109;
		Competition_Class.Text = "Class";
		Competition_Class.TextAlign = (ContentAlignment)64;
		((Control)Selected_Competition).BackColor = SystemColors.Control;
		((Control)Selected_Competition).Font = new Font("Microsoft Sans Serif", 22f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Selected_Competition).ForeColor = SystemColors.MenuHighlight;
		((Control)Selected_Competition).Location = new Point(150, 3);
		((Control)Selected_Competition).Name = "Selected_Competition";
		((Control)Selected_Competition).Size = new Size(393, 40);
		((Control)Selected_Competition).TabIndex = 101;
		Selected_Competition.Text = "Selected Contest_______________";
		Selected_Competition.TextAlign = (ContentAlignment)16;
		((Control)Event_Date_lbl).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Event_Date_lbl).ForeColor = SystemColors.MenuHighlight;
		Event_Date_lbl.ImageAlign = (ContentAlignment)64;
		((Control)Event_Date_lbl).Location = new Point(-81, 71);
		((Control)Event_Date_lbl).Name = "Event_Date_lbl";
		((Control)Event_Date_lbl).Size = new Size(177, 23);
		((Control)Event_Date_lbl).TabIndex = 99;
		Event_Date_lbl.Text = "Date";
		((Control)Label1).BackColor = SystemColors.Control;
		((Control)Label1).Font = new Font("Microsoft Sans Serif", 22f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)Label1).ForeColor = SystemColors.MenuHighlight;
		((Control)Label1).Location = new Point(3, 3);
		((Control)Label1).Name = "Label1";
		((Control)Label1).Size = new Size(141, 40);
		((Control)Label1).TabIndex = 114;
		Label1.Text = "Results";
		Label1.TextAlign = (ContentAlignment)16;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(678, 557);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)Label1);
		((Control)this).Controls.Add((Control)(object)Best_Scores_DataGridView);
		((Control)this).Controls.Add((Control)(object)Competition_Class);
		((Control)this).Controls.Add((Control)(object)Selected_Competition);
		((Control)this).Controls.Add((Control)(object)Event_Date_lbl);
		((Control)this).Name = "single_channel_results";
		((Form)this).Text = "RATS v3.7.2 single channel results display";
		((ISupportInitialize)Best_Scores_DataGridView).EndInit();
		((Control)this).ResumeLayout(false);
	}

	private void single_channel_display_Load(object sender, EventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		Public_Variables.single_channel_results_window_open = true;
		if (Public_Variables.database_available)
		{
			Display_Timer.Enabled = true;
			MyConn = new OleDbConnection();
			MyConn.ConnectionString = Public_Variables.connString;
			MyConn.Open();
			Selected_Competition.Text = Public_Variables.public_competition_name;
			Competition_Class.Text = Public_Variables.public_competition_class;
			Thread.Sleep(30);
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
			Best_Scores_DataGridView.Columns[1].HeaderText = "Run Time";
			Best_Scores_DataGridView.Columns[2].HeaderText = "Score Time";
			Best_Scores_DataGridView.Columns[3].HeaderText = "Contestant";
			((Control)Best_Scores_DataGridView).Refresh();
			ds.Dispose();
		}
	}

	private void single_channel_results_Close(object sender, FormClosingEventArgs e)
	{
		Public_Variables.single_channel_results_window_open = false;
		Display_Timer.Enabled = false;
	}

	private void Display_Timer_Tick(object sender, EventArgs e)
	{
		if (Public_Variables.request_single_channel_results_window_close)
		{
			Public_Variables.request_single_channel_results_window_close = false;
			((Form)this).Close();
		}
	}
}
