using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for CameraControl.
	/// </summary>
    public class LocScaleRotEdit : System.Windows.Forms.Form, ILocScaleRotEdit
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private Endogine.ValueEdit valueEditLoc;
		private Endogine.ValueEdit valueEditScale;
		private System.Windows.Forms.CheckBox cbAutoswitch;

		private Sprite m_sp = null;

		public LocScaleRotEdit()
		{
			InitializeComponent();

			valueEditScale.DefaultValue = new EPointF(1,1);
			valueEditScale.Jog.Factor = new EPointF(0.01f,0.01f);
			valueEditScale.Shuttle.Factor = new EPointF(0.001f,0.001f);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.valueEditLoc = new Endogine.ValueEdit();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.valueEditScale = new Endogine.ValueEdit();
			this.cbAutoswitch = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.valueEditLoc);
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 96);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Loc";
			// 
			// valueEditLoc
			// 
			this.valueEditLoc.CurrentValue = null;
			this.valueEditLoc.DefaultValue = new EPointF();
			this.valueEditLoc.Location = new System.Drawing.Point(8, 16);
			this.valueEditLoc.Name = "valueEditLoc";
			this.valueEditLoc.Size = new System.Drawing.Size(184, 72);
			this.valueEditLoc.StartValue = null;
			this.valueEditLoc.TabIndex = 0;
			this.valueEditLoc.ValueChanged += new Endogine.ValueEdit.ValueChangedDelegate(this.valueEditLoc_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.valueEditScale);
			this.groupBox2.Location = new System.Drawing.Point(0, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 96);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Scale";
			// 
			// valueEditScale
			// 
			this.valueEditScale.CurrentValue = null;
			this.valueEditScale.DefaultValue = new EPointF();
			this.valueEditScale.Location = new System.Drawing.Point(8, 16);
			this.valueEditScale.Name = "valueEditScale";
			this.valueEditScale.Size = new System.Drawing.Size(184, 72);
			this.valueEditScale.StartValue = null;
			this.valueEditScale.TabIndex = 0;
			this.valueEditScale.ValueChanged += new Endogine.ValueEdit.ValueChangedDelegate(this.valueEditScale_ValueChanged);
			// 
			// cbAutoswitch
			// 
			this.cbAutoswitch.Checked = true;
			this.cbAutoswitch.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutoswitch.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.cbAutoswitch.Location = new System.Drawing.Point(16, 192);
			this.cbAutoswitch.Name = "cbAutoswitch";
			this.cbAutoswitch.Size = new System.Drawing.Size(168, 16);
			this.cbAutoswitch.TabIndex = 9;
			this.cbAutoswitch.Text = "Auto-switch to selected sprite";
			this.cbAutoswitch.CheckedChanged += new System.EventHandler(this.cbAutoswitch_CheckedChanged);
			// 
			// LocScaleRotEdit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(202, 210);
			this.Controls.Add(this.cbAutoswitch);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LocScaleRotEdit";
			this.Text = "LocScaleRot Edit";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void valueEditLoc_ValueChanged(object sender, Endogine.EPointF pnt)
		{
			if (m_sp==null)
				return;
			m_sp.Loc = pnt;
		}

		private void valueEditScale_ValueChanged(object sender, Endogine.EPointF pnt)
		{
			if (m_sp==null)
				return;
			m_sp.Scaling= pnt;
		}

		private void cbAutoswitch_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		public Sprite EditSprite
		{
			get {return m_sp;}
			set
			{
				m_sp = value;
				this.valueEditLoc.InitWithValue(m_sp.Loc.Copy());
				this.valueEditScale.InitWithValue(m_sp.Scaling.Copy());

				this.Text = m_sp.Name + " Loc/Scale/Rot Edit";
			}
		}

		public bool AutoswitchToSprite
		{
			get {return this.cbAutoswitch.Checked;}
			set {this.cbAutoswitch.Checked = value;}
		}
	}
}
