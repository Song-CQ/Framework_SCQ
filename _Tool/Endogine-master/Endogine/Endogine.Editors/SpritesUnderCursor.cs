using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for SpritesUnderCursor.
	/// </summary>
	public class SpritesUnderCursor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbMouseActive;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SpritesUnderCursor()
		{
			InitializeComponent();

			EH.Instance.EnterFrameEvent+=new EnterFrame(Instance_EnterFrameEvent);
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
			this.label1 = new System.Windows.Forms.Label();
			this.cbMouseActive = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 176);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			// 
			// cbMouseActive
			// 
			this.cbMouseActive.Location = new System.Drawing.Point(16, 176);
			this.cbMouseActive.Name = "cbMouseActive";
			this.cbMouseActive.Size = new System.Drawing.Size(128, 24);
			this.cbMouseActive.TabIndex = 3;
			this.cbMouseActive.Text = "Only MouseActive";
			// 
			// SpritesUnderCursor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(240, 197);
			this.Controls.Add(this.cbMouseActive);
			this.Controls.Add(this.label1);
			this.Name = "SpritesUnderCursor";
			this.Text = "SpritesUnderCursor";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SpritesUnderCursor_Paint);
			this.ResumeLayout(false);

		}
		#endregion

		private void SpritesUnderCursor_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		}

		private void RefreshData()
		{
			string s = "";
			System.Collections.ArrayList sprites = EH.Instance.Stage.RootSprite.GetSpritesUnderLoc(EH.Instance.MouseLoc.ToEPointF(), -1);
			foreach (Endogine.Sprite sp in sprites)
			{
				if (this.cbMouseActive.Checked)
					if (!sp.MouseActive)
						continue;
				s+=sp.GetSceneGraphName() + "\r\n";
			}
			
			if (this.label1.Text != s) //lbSprites
			{
				this.label1.Text = s;
				this.Invalidate();
			}
			//EH.Instance.
		}

		private void Instance_EnterFrameEvent()
		{
			this.RefreshData();
		}
	}
}
