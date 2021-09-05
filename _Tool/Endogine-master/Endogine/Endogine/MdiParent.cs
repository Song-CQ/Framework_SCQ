using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Endogine
{
	/// <summary>
	/// http://www.amanith.org/blog/index.php
	/// Summary description for Form1.
	/// </summary>
	public class MdiParent : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.MenuItem miOnlineSpriteEdit;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem miEditorXML;
		private System.Windows.Forms.MenuItem miEditorResources;
		private System.Windows.Forms.MenuItem miEditorScene;
		private System.Windows.Forms.MenuItem miEditorConsole;
		private System.Windows.Forms.MenuItem miEditorCamControl;
		private MainBase m_formStage = null;

        public MdiParent()
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
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miOnlineSpriteEdit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miEditorXML = new System.Windows.Forms.MenuItem();
			this.miEditorResources = new System.Windows.Forms.MenuItem();
			this.miEditorScene = new System.Windows.Forms.MenuItem();
			this.miEditorCamControl = new System.Windows.Forms.MenuItem();
			this.miEditorConsole = new System.Windows.Forms.MenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miOnlineSpriteEdit});
			this.menuItem1.Text = "Edit";
			// 
			// miOnlineSpriteEdit
			// 
			this.miOnlineSpriteEdit.Index = 0;
			this.miOnlineSpriteEdit.Text = "Onscreen sprite edit";
			this.miOnlineSpriteEdit.Click += new System.EventHandler(this.miOnlineSpriteEdit_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miEditorXML,
																					  this.miEditorResources,
																					  this.miEditorScene,
																					  this.miEditorCamControl,
																					  this.miEditorConsole});
			this.menuItem2.Text = "Editors";
			// 
			// miEditorXML
			// 
			this.miEditorXML.Index = 0;
			this.miEditorXML.Text = "Simple XML";
			this.miEditorXML.Click += new System.EventHandler(this.miEditorXML_Click);
			// 
			// miEditorResources
			// 
			this.miEditorResources.Index = 1;
			this.miEditorResources.Shortcut = System.Windows.Forms.Shortcut.Ctrl3;
			this.miEditorResources.Text = "Resources";
			this.miEditorResources.Click += new System.EventHandler(this.miEditorResources_Click);
			// 
			// miEditorScene
			// 
			this.miEditorScene.Index = 2;
			this.miEditorScene.Shortcut = System.Windows.Forms.Shortcut.Ctrl4;
			this.miEditorScene.Text = "Scene";
			this.miEditorScene.Click += new System.EventHandler(this.miEditorScene_Click);
			// 
			// miEditorCamControl
			// 
			this.miEditorCamControl.Index = 3;
			this.miEditorCamControl.Text = "Cam control";
			this.miEditorCamControl.Click += new System.EventHandler(this.miEditorCamControl_Click);
			// 
			// miEditorConsole
			// 
			this.miEditorConsole.Index = 4;
			this.miEditorConsole.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.miEditorConsole.Text = "Console";
			this.miEditorConsole.Click += new System.EventHandler(this.miEditorConsole_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(176, 149);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Endogine";
			this.Load += new System.EventHandler(this.Form1_Load);

		}
		#endregion

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e) 
		{
			if (e.KeyCode == Keys.Escape) 
			{
				this.Close();
				return;
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			//m_formStage = new Main();
			
			Form formMdiParent = (Form)null;
			if (IsMdiContainer)
			{
				this.Width = 640;
				this.Height = 480;
				this.WindowState = FormWindowState.Maximized;
				formMdiParent = this;
				m_formStage.MdiParent = this;
			}
			else
			{
				this.Visible = false; // this doesn't work
				this.Text = "Should be invisible!";
			}
			//TODO: anyhow, it's strange to use a Form to start from, the project should probably be a console application.

			m_formStage.Show();

			//m_endogine.Init(m_formStage, formMdiParent, this);
			//m_formStage.EndogineInitDone();
		}

		private void miOnlineSpriteEdit_Click(object sender, System.EventArgs e)
		{
			((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
			EH.Instance.OnScreenEdit = ((MenuItem)sender).Checked;
		}

		private void miEditorXML_Click(object sender, System.EventArgs e)
		{
			EH.Instance.OpenEditor("XMLEditor");
		}

		private void miEditorResources_Click(object sender, System.EventArgs e)
		{
			EH.Instance.OpenEditor("ResourceBrowser");
		}

		private void miEditorScene_Click(object sender, System.EventArgs e)
		{
		
		}

		private void miEditorCamControl_Click(object sender, System.EventArgs e)
		{
		
		}

		private void miEditorConsole_Click(object sender, System.EventArgs e)
		{
		
		}

	}
}
