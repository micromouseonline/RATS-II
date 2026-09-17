using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace RATS;

[DesignerGenerated]
public class Calibration_Form : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Timer2")]
	private Timer _Timer2;

	[field: AccessedThroughProperty("Start_Cell_Pot")]
	internal virtual Label Start_Cell_Pot
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Timer Timer2
	{
		[CompilerGenerated]
		get
		{
			return _Timer2;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = Timer2_Tick;
			Timer val = _Timer2;
			if (val != null)
			{
				val.Tick -= eventHandler;
			}
			_Timer2 = value;
			val = _Timer2;
			if (val != null)
			{
				val.Tick += eventHandler;
			}
		}
	}

	[field: AccessedThroughProperty("Tick_Tock")]
	internal virtual Label Tick_Tock
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Cell_Header_Lbl")]
	internal virtual Label Start_Cell_Header_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Gate_Header_Lbl")]
	internal virtual Label Start_Gate_Header_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Finish_Gate_Header_Lbl")]
	internal virtual Label Finish_Gate_Header_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Pot_Value_Lbl")]
	internal virtual Label Pot_Value_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Brightness_Value_Lbl")]
	internal virtual Label Brightness_Value_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Trigger_Lbl")]
	internal virtual Label Trigger_Lbl
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Cell_Bright")]
	internal virtual Label Start_Cell_Bright
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Cell_Trigger")]
	internal virtual Label Start_Cell_Trigger
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Trigger")]
	internal virtual Label Start_Trigger
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Bright")]
	internal virtual Label Start_Bright
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Start_Pot")]
	internal virtual Label Start_Pot
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Finish_Trigger")]
	internal virtual Label Finish_Trigger
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Finish_Bright")]
	internal virtual Label Finish_Bright
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Finish_Pot")]
	internal virtual Label Finish_Pot
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public Calibration_Form()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		((Form)this)._002Ector();
		((Form)this).Load += Calibration_Form_Load;
		((Form)this).FormClosing += new FormClosingEventHandler(Calibration_Form_Close);
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
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Expected O, but got Unknown
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Expected O, but got Unknown
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Expected O, but got Unknown
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Expected O, but got Unknown
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Expected O, but got Unknown
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Expected O, but got Unknown
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Expected O, but got Unknown
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Expected O, but got Unknown
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Expected O, but got Unknown
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Expected O, but got Unknown
		components = new Container();
		Start_Cell_Pot = new Label();
		Timer2 = new Timer(components);
		Tick_Tock = new Label();
		Start_Cell_Header_Lbl = new Label();
		Start_Gate_Header_Lbl = new Label();
		Finish_Gate_Header_Lbl = new Label();
		Pot_Value_Lbl = new Label();
		Brightness_Value_Lbl = new Label();
		Trigger_Lbl = new Label();
		Start_Cell_Bright = new Label();
		Start_Cell_Trigger = new Label();
		Start_Trigger = new Label();
		Start_Bright = new Label();
		Start_Pot = new Label();
		Finish_Trigger = new Label();
		Finish_Bright = new Label();
		Finish_Pot = new Label();
		((Control)this).SuspendLayout();
		Start_Cell_Pot.AutoSize = true;
		((Control)Start_Cell_Pot).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Cell_Pot).Location = new Point(170, 70);
		((Control)Start_Cell_Pot).Name = "Start_Cell_Pot";
		((Control)Start_Cell_Pot).Size = new Size(78, 22);
		((Control)Start_Cell_Pot).TabIndex = 0;
		Start_Cell_Pot.Text = "Cell_Pot";
		Tick_Tock.AutoSize = true;
		((Control)Tick_Tock).Location = new Point(29, 259);
		((Control)Tick_Tock).Name = "Tick_Tock";
		((Control)Tick_Tock).Size = new Size(0, 13);
		((Control)Tick_Tock).TabIndex = 1;
		Start_Cell_Header_Lbl.AutoSize = true;
		((Control)Start_Cell_Header_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Cell_Header_Lbl).Location = new Point(28, 70);
		((Control)Start_Cell_Header_Lbl).Name = "Start_Cell_Header_Lbl";
		((Control)Start_Cell_Header_Lbl).Size = new Size(83, 24);
		((Control)Start_Cell_Header_Lbl).TabIndex = 2;
		Start_Cell_Header_Lbl.Text = "Start Cell";
		Start_Gate_Header_Lbl.AutoSize = true;
		((Control)Start_Gate_Header_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Gate_Header_Lbl).Location = new Point(28, 129);
		((Control)Start_Gate_Header_Lbl).Name = "Start_Gate_Header_Lbl";
		((Control)Start_Gate_Header_Lbl).Size = new Size(90, 24);
		((Control)Start_Gate_Header_Lbl).TabIndex = 3;
		Start_Gate_Header_Lbl.Text = "Start Gate";
		Finish_Gate_Header_Lbl.AutoSize = true;
		((Control)Finish_Gate_Header_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Finish_Gate_Header_Lbl).Location = new Point(28, 191);
		((Control)Finish_Gate_Header_Lbl).Name = "Finish_Gate_Header_Lbl";
		((Control)Finish_Gate_Header_Lbl).Size = new Size(105, 24);
		((Control)Finish_Gate_Header_Lbl).TabIndex = 4;
		Finish_Gate_Header_Lbl.Text = "Finish Gate";
		Pot_Value_Lbl.AutoSize = true;
		((Control)Pot_Value_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Pot_Value_Lbl).Location = new Point(170, 20);
		((Control)Pot_Value_Lbl).Name = "Pot_Value_Lbl";
		((Control)Pot_Value_Lbl).Size = new Size(91, 24);
		((Control)Pot_Value_Lbl).TabIndex = 5;
		Pot_Value_Lbl.Text = "Pot Value";
		Brightness_Value_Lbl.AutoSize = true;
		((Control)Brightness_Value_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Brightness_Value_Lbl).Location = new Point(307, 20);
		((Control)Brightness_Value_Lbl).Name = "Brightness_Value_Lbl";
		((Control)Brightness_Value_Lbl).Size = new Size(152, 24);
		((Control)Brightness_Value_Lbl).TabIndex = 6;
		Brightness_Value_Lbl.Text = "Brightness Value";
		Trigger_Lbl.AutoSize = true;
		((Control)Trigger_Lbl).Font = new Font("Microsoft Sans Serif", 14f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Trigger_Lbl).Location = new Point(512, 20);
		((Control)Trigger_Lbl).Name = "Trigger_Lbl";
		((Control)Trigger_Lbl).Size = new Size(71, 24);
		((Control)Trigger_Lbl).TabIndex = 7;
		Trigger_Lbl.Text = "Trigger";
		Start_Cell_Bright.AutoSize = true;
		((Control)Start_Cell_Bright).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Cell_Bright).Location = new Point(307, 70);
		((Control)Start_Cell_Bright).Name = "Start_Cell_Bright";
		((Control)Start_Cell_Bright).Size = new Size(98, 22);
		((Control)Start_Cell_Bright).TabIndex = 8;
		Start_Cell_Bright.Text = "Cell_Bright";
		Start_Cell_Trigger.AutoSize = true;
		((Control)Start_Cell_Trigger).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Cell_Trigger).Location = new Point(512, 70);
		((Control)Start_Cell_Trigger).Name = "Start_Cell_Trigger";
		((Control)Start_Cell_Trigger).Size = new Size(109, 22);
		((Control)Start_Cell_Trigger).TabIndex = 9;
		Start_Cell_Trigger.Text = "Cell_Trigger";
		Start_Trigger.AutoSize = true;
		((Control)Start_Trigger).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Trigger).Location = new Point(512, 129);
		((Control)Start_Trigger).Name = "Start_Trigger";
		((Control)Start_Trigger).Size = new Size(116, 22);
		((Control)Start_Trigger).TabIndex = 12;
		Start_Trigger.Text = "Start_Trigger";
		Start_Bright.AutoSize = true;
		((Control)Start_Bright).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Bright).Location = new Point(307, 129);
		((Control)Start_Bright).Name = "Start_Bright";
		((Control)Start_Bright).Size = new Size(105, 22);
		((Control)Start_Bright).TabIndex = 11;
		Start_Bright.Text = "Start_Bright";
		Start_Pot.AutoSize = true;
		((Control)Start_Pot).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Start_Pot).Location = new Point(170, 129);
		((Control)Start_Pot).Name = "Start_Pot";
		((Control)Start_Pot).Size = new Size(85, 22);
		((Control)Start_Pot).TabIndex = 10;
		Start_Pot.Text = "Start_Pot";
		Finish_Trigger.AutoSize = true;
		((Control)Finish_Trigger).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Finish_Trigger).Location = new Point(512, 191);
		((Control)Finish_Trigger).Name = "Finish_Trigger";
		((Control)Finish_Trigger).Size = new Size(126, 22);
		((Control)Finish_Trigger).TabIndex = 15;
		Finish_Trigger.Text = "Finish_Trigger";
		Finish_Bright.AutoSize = true;
		((Control)Finish_Bright).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Finish_Bright).Location = new Point(307, 191);
		((Control)Finish_Bright).Name = "Finish_Bright";
		((Control)Finish_Bright).Size = new Size(115, 22);
		((Control)Finish_Bright).TabIndex = 14;
		Finish_Bright.Text = "Finish_Bright";
		Finish_Pot.AutoSize = true;
		((Control)Finish_Pot).Font = new Font("Microsoft Sans Serif", 13f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)Finish_Pot).Location = new Point(170, 191);
		((Control)Finish_Pot).Name = "Finish_Pot";
		((Control)Finish_Pot).Size = new Size(95, 22);
		((Control)Finish_Pot).TabIndex = 13;
		Finish_Pot.Text = "Finish_Pot";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(670, 311);
		((Control)this).Controls.Add((Control)(object)Finish_Trigger);
		((Control)this).Controls.Add((Control)(object)Finish_Bright);
		((Control)this).Controls.Add((Control)(object)Finish_Pot);
		((Control)this).Controls.Add((Control)(object)Start_Trigger);
		((Control)this).Controls.Add((Control)(object)Start_Bright);
		((Control)this).Controls.Add((Control)(object)Start_Pot);
		((Control)this).Controls.Add((Control)(object)Start_Cell_Trigger);
		((Control)this).Controls.Add((Control)(object)Start_Cell_Bright);
		((Control)this).Controls.Add((Control)(object)Trigger_Lbl);
		((Control)this).Controls.Add((Control)(object)Brightness_Value_Lbl);
		((Control)this).Controls.Add((Control)(object)Pot_Value_Lbl);
		((Control)this).Controls.Add((Control)(object)Finish_Gate_Header_Lbl);
		((Control)this).Controls.Add((Control)(object)Start_Gate_Header_Lbl);
		((Control)this).Controls.Add((Control)(object)Start_Cell_Header_Lbl);
		((Control)this).Controls.Add((Control)(object)Tick_Tock);
		((Control)this).Controls.Add((Control)(object)Start_Cell_Pot);
		((Control)this).Name = "Calibration_Form";
		((Form)this).Text = "Calibration";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	private void Calibration_Form_Load(object sender, EventArgs e)
	{
		Public_Variables.calibration_window_open = true;
		Public_Variables.request_calibration_mode = true;
		Timer2.Enabled = true;
	}

	private void Calibration_Form_Close(object sender, FormClosingEventArgs e)
	{
		Public_Variables.calibration_window_open = false;
		Public_Variables.request_timer_mode = true;
		Timer2.Enabled = false;
	}

	private void Timer2_Tick(object sender, EventArgs e)
	{
		if (Public_Variables.request_calibration_close)
		{
			Public_Variables.request_calibration_close = false;
			((Form)this).Close();
			return;
		}
		Start_Cell_Pot.Text = Strings.Format(Public_Variables.SCPot);
		Start_Cell_Bright.Text = Strings.Format(Public_Variables.SCLevel);
		Start_Cell_Trigger.Text = Strings.Format(Public_Variables.CTrigger);
		Start_Pot.Text = Strings.Format(Public_Variables.SGPot);
		Start_Bright.Text = Strings.Format(Public_Variables.SGLevel);
		Start_Trigger.Text = Strings.Format(Public_Variables.STrigger);
		Finish_Pot.Text = Strings.Format(Public_Variables.FGPot);
		Finish_Bright.Text = Strings.Format(Public_Variables.FGLevel);
		Finish_Trigger.Text = Strings.Format(Public_Variables.FTrigger);
	}
}
