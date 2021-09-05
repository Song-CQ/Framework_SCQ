using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.CodeDom.Compiler;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for MessageWindow.
	/// </summary>
    public class MessageWindow : System.Windows.Forms.Form, IMessageWindow
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.ComboBox cbbLanguage;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MessageWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.richTextBox1.Text+="\n";

//			ArrayList langs = Endogine.Scripting.ScriptingProvider.GetAvailableLanguages();
//			foreach (string lang in langs)
//				this.cbbLanguage.Items.Add(lang);
//			this.cbbLanguage.SelectedIndex = 0;
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

		public void Put(string s)
		{
			int nOverflow = richTextBox1.Text.Length -6*1024;
			if (nOverflow > 0)
				richTextBox1.Text = richTextBox1.Text.Remove(0, nOverflow);

			richTextBox1.Text+=s+"\n";
			this.Invalidate();
			//TODO: how to invalidate although the form isn't in focus?? this.Focus();
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.cbbLanguage = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(0, 24);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBox1.Size = new System.Drawing.Size(296, 248);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "Welcome to Endogine";
			this.richTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyDown);
			this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			// 
			// cbbLanguage
			// 
			this.cbbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbLanguage.Location = new System.Drawing.Point(0, 0);
			this.cbbLanguage.Name = "cbbLanguage";
			this.cbbLanguage.Size = new System.Drawing.Size(72, 21);
			this.cbbLanguage.TabIndex = 2;
			this.cbbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbbLanguage_SelectedIndexChanged);
			// 
			// MessageWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.cbbLanguage);
			this.Controls.Add(this.richTextBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Location = new System.Drawing.Point(0, 300);
			this.Name = "MessageWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "MessageWindow";
			this.Resize += new System.EventHandler(this.MessageWindow_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		private void richTextBox1_TextChanged(object sender, System.EventArgs e)
		{
		}

		private void MessageWindow_Resize(object sender, System.EventArgs e)
		{
			richTextBox1.Width = ClientRectangle.Width;
			richTextBox1.Height = ClientRectangle.Height - richTextBox1.Top;
		}

		private void richTextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Return)
			{
				int nStart = this.richTextBox1.Text.LastIndexOf("\n", this.richTextBox1.SelectionStart);
				if (nStart == this.richTextBox1.SelectionStart && nStart > 0)
					nStart =  this.richTextBox1.Text.LastIndexOf("\n", nStart-1);
				nStart++;
				int nLength = this.richTextBox1.SelectionStart-nStart;
				if (nLength > 0)
				{
					string codeString = this.richTextBox1.Text.Substring(nStart, nLength);
					//codeString = this.richTextBox1.Lines[this.richTextBox1.Lines.Length-1];
					this.ExecuteCode(codeString);
				}
			}
		}

		public object ExecuteCode(string code)
		{
			string languageSuffix = (string)this.cbbLanguage.SelectedItem;

//			Endogine.Scripting.ScripterBase scripter = Endogine.Scripting.ScriptingProvider.GetScripter(languageSuffix);
//			if (scripter == null)
//			{
//				scripter = Endogine.Scripting.ScriptingProvider.CreateScripter(languageSuffix);
//				if (scripter!=null)
//				{
//					//TODO: EndogineHub (or someone) must be told about additional assemblies
//					string[] assemblies = new string[]{"System","Endogine","YE"};
//					scripter.AddReferencedAssemblies(assemblies);
//				}
//			}
//			if (scripter==null)
//				return null;
//
//			return scripter.Execute(code);
			return null;
		}

		private void cbbLanguage_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
