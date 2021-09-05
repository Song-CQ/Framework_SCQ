using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine
{
	/// <summary>
	/// Summary description for Setup.
	/// </summary>
	public class SetupWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox cbMDI;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Form form1;
		public SetupWindow(string[] a_aAvailableStrategies, Form form)
		{
			InitializeComponent();

			form1 = form;

			for (int i = 0; i < a_aAvailableStrategies.GetLength(0); i++)
			{
				string s = a_aAvailableStrategies[i];
				RadioButton rb = new RadioButton();
				rb.Text = s;
				rb.Top = i*20 + 15;
				rb.Left = 10;
				rb.CheckedChanged+=new EventHandler(rb_CheckedChanged);
				groupBox1.Controls.Add(rb);
			}
			((RadioButton)groupBox1.Controls[0]).Checked = true;
		}

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
			this.btnGo = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.cbMDI = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(240, 160);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(64, 23);
			this.btnGo.TabIndex = 0;
			this.btnGo.Text = "Go";
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 88);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Rendering mode";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(16, 96);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(152, 64);
			this.checkBox1.TabIndex = 2;
			this.checkBox1.Text = "Fullscreen (only works with D3D so far, and the mouse reading is a bit off)";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// cbMDI
			// 
			this.cbMDI.Checked = true;
			this.cbMDI.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbMDI.Location = new System.Drawing.Point(184, 24);
			this.cbMDI.Name = "cbMDI";
			this.cbMDI.Size = new System.Drawing.Size(120, 104);
			this.cbMDI.TabIndex = 3;
			this.cbMDI.Text = "MDI mode (for development and debugging). NOTE: keyboard input doesn\'t work in MD" +
				"I mode! Bug in .NET.";
			this.cbMDI.CheckedChanged += new System.EventHandler(this.cbMDI_CheckedChanged);
			// 
			// SetupWindow
			// 
			this.AcceptButton = this.btnGo;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(306, 191);
			this.ControlBox = false;
			this.Controls.Add(this.cbMDI);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnGo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SetupWindow";
			this.Text = "Setup";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SetupWindow_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void rb_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = (RadioButton)sender;
			if (rb.Checked)
			{
				EndogineHub endo = EndogineHub.Instance;
				endo.PreSetRenderStrategy(rb.Text);
			}
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			EndogineHub endo = EndogineHub.Instance;
			bool bFull = ((CheckBox)sender).Checked;
			endo.PreSetFullscreen(bFull);
			if (bFull)
			{
				cbMDI.Checked = false;
				cbMDI.Enabled = false;
			}
			else
			{
				cbMDI.Enabled = true;
			}

		}

		private void cbMDI_CheckedChanged(object sender, System.EventArgs e)
		{
			//form1.IsMdiContainer = ((CheckBox)sender).Checked;
		}

		private void SetupWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            if (form1!=null)
    			form1.IsMdiContainer = cbMDI.Checked;
		}
	}
}
