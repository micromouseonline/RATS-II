using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace RATS;

[DesignerGenerated]
public class run_order : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Display_Timer")]
	private Timer _Display_Timer;

	private OleDbConnection MyConn;

	private const int run_order_delay = 20;

	[field: AccessedThroughProperty("Run_order_DataGridView")]
	internal virtual DataGridView Run_order_DataGridView
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Title_Label")]
	internal virtual Label Title_Label
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

	public run_order()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		((Form)this)._002Ector();
		((Form)this).Load += run_order_Load;
		((Form)this).FormClosing += new FormClosingEventHandler(run_order_Close);
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
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		components = new Container();
		DataGridViewCellStyle val = new DataGridViewCellStyle();
		DataGridViewCellStyle val2 = new DataGridViewCellStyle();
		Run_order_DataGridView = new DataGridView();
		Title_Label = new Label();
		Display_Timer = new Timer(components);
		((ISupportInitialize)Run_order_DataGridView).BeginInit();
		((Control)this).SuspendLayout();
		Run_order_DataGridView.AllowUserToAddRows = false;
		Run_order_DataGridView.AllowUserToDeleteRows = false;
		Run_order_DataGridView.AllowUserToResizeRows = false;
		Run_order_DataGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)16;
		Run_order_DataGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		Run_order_DataGridView.ColumnHeadersVisible = false;
		val.Alignment = (DataGridViewContentAlignment)16;
		val.BackColor = SystemColors.Window;
		val.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		val.ForeColor = SystemColors.ControlText;
		val.SelectionBackColor = SystemColors.Highlight;
		val.SelectionForeColor = SystemColors.HighlightText;
		val.WrapMode = (DataGridViewTriState)2;
		Run_order_DataGridView.DefaultCellStyle = val;
		((Control)Run_order_DataGridView).Location = new Point(3, 61);
		((Control)Run_order_DataGridView).Name = "Run_order_DataGridView";
		val2.Alignment = (DataGridViewContentAlignment)16;
		val2.BackColor = SystemColors.Control;
		val2.Font = new Font("Microsoft Sans Serif", 9.75f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		val2.ForeColor = SystemColors.WindowText;
		val2.SelectionBackColor = SystemColors.Highlight;
		val2.SelectionForeColor = SystemColors.HighlightText;
		val2.WrapMode = (DataGridViewTriState)1;
		Run_order_DataGridView.RowHeadersDefaultCellStyle = val2;
		Run_order_DataGridView.RowHeadersVisible = false;
		Run_order_DataGridView.RowHeadersWidth = 80;
		Run_order_DataGridView.RowHeadersWidthSizeMode = (DataGridViewRowHeadersWidthSizeMode)1;
		((Control)Run_order_DataGridView).Size = new Size(164, 484);
		((Control)Run_order_DataGridView).TabIndex = 0;
		Title_Label.AutoSize = true;
		((Control)Title_Label).Font = new Font("Microsoft Sans Serif", 22f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Title_Label).Location = new Point(9, 8);
		((Control)Title_Label).Name = "Title_Label";
		((Control)Title_Label).Size = new Size(154, 36);
		((Control)Title_Label).TabIndex = 1;
		Title_Label.Text = "Run Order";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(170, 557);
		((Form)this).ControlBox = false;
		((Control)this).Controls.Add((Control)(object)Title_Label);
		((Control)this).Controls.Add((Control)(object)Run_order_DataGridView);
		((Control)this).Name = "run_order";
		((Form)this).Text = "Running_order";
		((ISupportInitialize)Run_order_DataGridView).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	private void run_order_Load(object sender, EventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		Public_Variables.run_order_window_open = true;
		Display_Timer.Enabled = true;
		MyConn = new OleDbConnection();
		MyConn.ConnectionString = Public_Variables.connString;
		MyConn.Open();
		Public_Variables.refresh_run_order_delay = 20;
	}

	private void run_order_Close(object sender, FormClosingEventArgs e)
	{
		Public_Variables.run_order_window_open = false;
		Display_Timer.Enabled = false;
	}

	private void Display_Timer_Tick(object sender, EventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		string text = "SELECT Mouse_Name AS Robot FROM [Entry] WHERE ((Competition_ID = " + Public_Variables.competition_id + ") AND (Entry_Used = FALSE)) ORDER BY Sequence_Number";
		BindingSource val = new BindingSource();
		checked
		{
			if (Public_Variables.request_run_order_window_close)
			{
				Public_Variables.request_run_order_window_close = false;
				((Form)this).Close();
			}
			else if (Public_Variables.database_available)
			{
				Public_Variables.refresh_run_order_delay++;
				if (Public_Variables.refresh_run_order_delay > 20)
				{
					DataSet dataSet = new DataSet();
					DataTableCollection tables = dataSet.Tables;
					OleDbDataAdapter val2 = new OleDbDataAdapter(text, MyConn);
					((DbDataAdapter)(object)val2).Fill(dataSet);
					DataView dataSource = (DataView)(val.DataSource = new DataView(tables[0]));
					Run_order_DataGridView.DataSource = dataSource;
					Public_Variables.refresh_run_order_delay = 0;
				}
			}
		}
	}
}
