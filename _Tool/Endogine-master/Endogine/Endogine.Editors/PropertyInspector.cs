using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine
{
	/// <summary>
	/// TODO: use PropertyGrid and PropertyInfo
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dndotnet/html/usingpropgrid.asp
	/// http://www.thecodeproject.com/csharp/configeditor.asp
	/// http://www.codeproject.com/csharp/Fast_Dynamic_Properties.asp
	/// http://www.codeproject.com/cs/miscctrl/CustomPropGrid.asp
	/// http://www.codeproject.com/csharp/SimpleSerializer.asp
	/// http://www.codeproject.com/csharp/DzCollectionEditor.asp
	/// Summary description for PropertyInspector.
	/// </summary>
	public class PropertyInspector : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Button btnBehaviors;
		private System.Windows.Forms.Button btnLocScaleRot;
		private System.Windows.Forms.CheckBox cbAutoswitch;
		private Sprite m_sp = null;

		public PropertyInspector()
		{
			InitializeComponent();
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.btnBehaviors = new System.Windows.Forms.Button();
			this.btnLocScaleRot = new System.Windows.Forms.Button();
			this.cbAutoswitch = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(240, 272);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// btnBehaviors
			// 
			this.btnBehaviors.Location = new System.Drawing.Point(8, 280);
			this.btnBehaviors.Name = "btnBehaviors";
			this.btnBehaviors.Size = new System.Drawing.Size(104, 23);
			this.btnBehaviors.TabIndex = 2;
			this.btnBehaviors.Text = "Behaviors... (0)";
			this.btnBehaviors.Click += new System.EventHandler(this.btnBehaviors_Click);
			// 
			// btnLocScaleRot
			// 
			this.btnLocScaleRot.Location = new System.Drawing.Point(120, 280);
			this.btnLocScaleRot.Name = "btnLocScaleRot";
			this.btnLocScaleRot.Size = new System.Drawing.Size(88, 23);
			this.btnLocScaleRot.TabIndex = 3;
			this.btnLocScaleRot.Text = "Loc/Scale/Rot";
			this.btnLocScaleRot.Click += new System.EventHandler(this.btnLocScaleRot_Click);
			// 
			// cbAutoswitch
			// 
			this.cbAutoswitch.Checked = true;
			this.cbAutoswitch.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutoswitch.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.cbAutoswitch.Location = new System.Drawing.Point(8, 304);
			this.cbAutoswitch.Name = "cbAutoswitch";
			this.cbAutoswitch.Size = new System.Drawing.Size(168, 16);
			this.cbAutoswitch.TabIndex = 4;
			this.cbAutoswitch.Text = "Auto-switch to selected sprite";
			// 
			// PropertyInspector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(240, 325);
			this.Controls.Add(this.cbAutoswitch);
			this.Controls.Add(this.btnLocScaleRot);
			this.Controls.Add(this.btnBehaviors);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "PropertyInspector";
			this.Text = "PropertyInspector";
			this.Resize += new System.EventHandler(this.PropertyInspector_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		public void ShowProperties(Sprite a_sp)
		{
			if (a_sp == null)
				return;
			m_sp = a_sp;

			this.Text = this.m_sp.GetSceneGraphName() + " Properties";
			//this.Text = m_sp.Name + " Properties";

			this.propertyGrid1.SelectedObject = a_sp;

//			System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
			int nNumBh = this.m_sp.GetNumBehaviors();
			this.btnBehaviors.Text = "Behaviors ("+nNumBh+") ...";
			//this.btnBehaviors.Top = this.propertyGrid1.Bottom+10;
			//this.btnBehaviors.Click+=new EventHandler(btn_Click);
			//this.btnBehaviors.Width = 100;
			//this.Controls.Add(this.btnBehaviors);
		}

		private void btnBehaviors_Click(object sender, System.EventArgs e)
		{
			BehaviorInspector bhi = new BehaviorInspector();
			bhi.SetSprite(m_sp);
			bhi.MdiParent = this.MdiParent;
			bhi.Show();
		}

		private void btnLocScaleRot_Click(object sender, System.EventArgs e)
		{
			Endogine.Editors.LocScaleRotEdit ctrl = new Endogine.Editors.LocScaleRotEdit();
			ctrl.EditSprite = this.m_sp;
			ctrl.MdiParent = this.MdiParent;
			ctrl.Show();
		}

		private void PropertyInspector_Resize(object sender, System.EventArgs e)
		{
			this.propertyGrid1.Width = this.ClientRectangle.Right-this.propertyGrid1.Left;
		}

		public bool AutoswitchToSprite
		{
			get {return this.cbAutoswitch.Checked;}
		}
	}
}
