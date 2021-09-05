using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Endogine
{
	/// <summary>
	/// Summary description for BehaviorInspector.
	/// </summary>
	public class BehaviorInspector : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblDescrHead;
		private System.Windows.Forms.Label lblDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderIndex;

		private Sprite m_sp;

		public BehaviorInspector()
		{
			InitializeComponent();
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
			this.lblDescrHead = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderIndex = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// lblDescrHead
			// 
			this.lblDescrHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDescrHead.Location = new System.Drawing.Point(0, 80);
			this.lblDescrHead.Name = "lblDescrHead";
			this.lblDescrHead.Size = new System.Drawing.Size(100, 16);
			this.lblDescrHead.TabIndex = 1;
			this.lblDescrHead.Text = "Description";
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(0, 96);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(256, 32);
			this.lblDescription.TabIndex = 2;
			this.lblDescription.Text = "TODO: How to get user-defined metadata about a class? Attributes of constructor.." +
				"?";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(192, 0);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(56, 23);
			this.btnAdd.TabIndex = 3;
			this.btnAdd.Text = "Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(192, 24);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(56, 23);
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 128);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(248, 208);
			this.propertyGrid1.TabIndex = 6;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeaderName,
																						this.columnHeaderIndex});
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(184, 80);
			this.listView1.TabIndex = 7;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 140;
			// 
			// columnHeaderIndex
			// 
			this.columnHeaderIndex.Text = "Index";
			this.columnHeaderIndex.Width = 40;
			// 
			// BehaviorInspector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 333);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblDescrHead);
			this.Name = "BehaviorInspector";
			this.Text = "BehaviorInspector";
			this.Resize += new System.EventHandler(this.BehaviorInspector_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		public void SetSprite(Sprite a_sp)
		{
			m_sp = a_sp;
			RefreshView();
		}

		public void RefreshView()
		{
//			listBox1.Items.Clear();
			this.listView1.Items.Clear();
			this.Text = this.m_sp.GetSceneGraphName() + " Behaviors";

//			DataTable dt = new DataTable();
//			dt.Columns.Add("Name", typeof(string));
//			dt.Columns.Add("Index", typeof(int));
			int nNumBhs = m_sp.GetNumBehaviors();
			for (int i = 0; i < nNumBhs; i++)
			{
				Behavior bh = (Behavior)m_sp.GetBehaviorByIndex(i);
//				DataRow row = dt.NewRow();
//				row["Name"] = bh.ToString();
//				row["Index"] = i;
//				dt.Rows.Add(row);
//				listBox1.Items.Add();
				ListViewItem item = new ListViewItem(new string[]{bh.ToString(), i.ToString()});
				this.listView1.Items.Add(item);
			}

//			this.listView1.DataSource = dt;
//			this.listBox1.DisplayMember = "Name";
		}
		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedItems.Count == 0)
//			if (listBox1.SelectedItem == null)
				return;
			ListViewItem item = this.listView1.SelectedItems[0];
			int index = Convert.ToInt32(item.SubItems[1].Text);
			//int index = listBox1.SelectedIndex;
			Behavior bh = (Behavior)m_sp.GetBehaviorByIndex(index);
			bh.Dispose();
			RefreshView();
		}

		private void GetAllFiles(string a_sPath, string a_sPattern, ref ArrayList a_aFileNames)
		{
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(a_sPath);
			System.IO.FileInfo[] aFiles = dirInfo.GetFiles( a_sPattern);
			foreach (System.IO.FileInfo file in aFiles)
				a_aFileNames.Add(file.Name);
			System.IO.DirectoryInfo[] aDirs = dirInfo.GetDirectories();
			foreach (System.IO.DirectoryInfo dir in aDirs)
				GetAllFiles(dir.FullName, a_sPattern, ref a_aFileNames);
		}
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			//TODO: How to dynamically create an object from a string? Use scripting engine? Or a factory?
			//TODO: How to find all behavior classes? Search in a separate folder? Or register somehow?
			Behavior bh = null;
			//TODO: only works in IDE mode. How to do it in normal mode?
			System.IO.FileInfo finfo = new System.IO.FileInfo(EndogineHub.Instance.CastLib.DirectoryPath);
			ArrayList aFiles = new ArrayList();
			GetAllFiles(finfo.Directory.FullName, "Bh*.cs", ref aFiles);

			if (false)
			{

			}
			else
			{
				System.Runtime.Remoting.ObjectHandle obj = System.Activator.CreateInstance("Tests", "DivStuff.BhSwing");
				object o = obj.Unwrap();
				bh = (Behavior)o;
			}
			//Behavior bh =  (Behavior)obj.Unwrap();//typeof("ThisMovie.BhSwing")
			if (bh != null)
			{
				m_sp.AddBehavior(bh);
				RefreshView();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listView1.SelectedItems.Count == 0)
				this.propertyGrid1.SelectedObject = null;
			else
			{
				ListViewItem item = this.listView1.SelectedItems[0];
				int index = Convert.ToInt32(item.SubItems[1].Text);
				Behavior bh = this.m_sp.GetBehaviorByIndex(index);
				//bh.GetType().
				this.propertyGrid1.SelectedObject = bh;
			}
		}

		private void BehaviorInspector_Resize(object sender, System.EventArgs e)
		{
			this.propertyGrid1.Height = this.ClientRectangle.Height-this.propertyGrid1.Top;
			this.propertyGrid1.Width = this.ClientRectangle.Width-this.propertyGrid1.Left;
		}
	}
}
