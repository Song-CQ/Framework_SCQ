using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Endogine;

namespace Tests
{
	/// <summary>
	/// http://www.amanith.org/blog/index.php
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private EndogineHub m_endogine;
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
        private MenuItem miEditorPicRef;
        private MenuItem miGridSettings;
		private MainBase m_formStage = null;

		public Form1()
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
				if (m_endogine!=null)
					m_endogine.Dispose();

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miOnlineSpriteEdit = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miEditorXML = new System.Windows.Forms.MenuItem();
            this.miEditorResources = new System.Windows.Forms.MenuItem();
            this.miEditorScene = new System.Windows.Forms.MenuItem();
            this.miEditorCamControl = new System.Windows.Forms.MenuItem();
            this.miEditorConsole = new System.Windows.Forms.MenuItem();
            this.miEditorPicRef = new System.Windows.Forms.MenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.miGridSettings = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
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
            this.miEditorConsole,
            this.miEditorPicRef,
            this.miGridSettings});
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
            // miEditorPicRef
            // 
            this.miEditorPicRef.Index = 5;
            this.miEditorPicRef.Text = "PicRef tool";
            this.miEditorPicRef.Click += new System.EventHandler(this.miEditorPicRef_Click);
            // 
            // miGridSettings
            // 
            this.miGridSettings.Index = 6;
            this.miGridSettings.Text = "Grid settings";
            this.miGridSettings.Click += new System.EventHandler(this.miGridSettings_Click);
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
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		//[MTAThread]
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

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
			m_endogine = new EndogineHub(Application.ExecutablePath);

			string[] aAvailableStrategies = StageBase.GetAvailableRenderers(null);
//			string[] aAvailableStrategies = new string[]{Enum.GetName(typeof(EndogineHub.RenderStrategy), 0), Enum.GetName(typeof(EndogineHub.RenderStrategy), 1)};
            if (Endogine.AppSettings.Instance.GetNodeText("MDI") != "false")
                this.IsMdiContainer = true;

            if (Endogine.AppSettings.Instance.GetNodeText("SetupDialog") != "false")
            {
                SetupWindow wndSetup = new SetupWindow(aAvailableStrategies, this);
                wndSetup.ShowDialog();
            }

			m_formStage = new Main();
			
			Form formMdiParent = (Form)null;
			if (this.IsMdiContainer)
			{
				this.Width = 800;
				this.Height = 600;
				this.WindowState = FormWindowState.Maximized;
				formMdiParent = this;
				m_formStage.MdiParent = this;
			}
			else
			{
				this.Visible = false; //TODO: this doesn't work
				this.Text = "Should be invisible!";
			}
			//TODO: anyhow, it's strange to use a Form to start from, the project should probably be a console application.

			m_formStage.Show();

			m_endogine.Init(m_formStage, formMdiParent, this);
			m_formStage.EndogineInitDone();
		}

		private void miOnlineSpriteEdit_Click(object sender, System.EventArgs e)
		{
			((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
			m_endogine.OnScreenEdit = ((MenuItem)sender).Checked;
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
            EH.Instance.OpenEditor("SceneGraphViewer");
        }

		private void miEditorCamControl_Click(object sender, System.EventArgs e)
		{
            EH.Instance.OpenEditor("CamControl");
        }

		private void miEditorConsole_Click(object sender, System.EventArgs e)
		{
            EH.Instance.OpenEditor("MessageWindow");
        }

        private void miEditorPicRef_Click(object sender, EventArgs e)
        {
            EH.Instance.OpenEditor("PicRefTool");
        }

        private void miGridSettings_Click(object sender, EventArgs e)
        {
            EH.Instance.OpenEditor("GridSettings");
        }
	}
}
