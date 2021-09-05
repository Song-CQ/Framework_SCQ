using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for ResourceBrowser.
	/// </summary>
	public class ResourceBrowser : System.Windows.Forms.Form, IResourceBrowser
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.ComponentModel.Container components = null;
		private Endogine.Editors.TreeGrid treeGrid1;

		private DataTable dataTable;

		public ResourceBrowser()
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.treeGrid1 = new Endogine.Editors.TreeGrid();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem5});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem3,
																					  this.menuItem4});
			this.menuItem1.Text = "Edit";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Add...";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Delete";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "Find...";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.Text = "View";
			// 
			// treeGrid1
			// 
			this.treeGrid1.Location = new System.Drawing.Point(0, 0);
			this.treeGrid1.Name = "treeGrid1";
			this.treeGrid1.Size = new System.Drawing.Size(296, 224);
			this.treeGrid1.TabIndex = 1;
			this.treeGrid1.XmlDocument = null;
			this.treeGrid1.XmlNodeMouseStartDrag += new Endogine.Editors.XmlNodeEvent(this.treeGrid1_XmlNodeMouseStartDrag);
			this.treeGrid1.XmlNodeMouseDown += new Endogine.Editors.XmlNodeEvent(this.treeGrid1_XmlNodeMouseDown);
			// 
			// ResourceBrowser
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 233);
			this.Controls.Add(this.treeGrid1);
			this.Menu = this.mainMenu1;
			this.Name = "ResourceBrowser";
			this.Text = "ResourceBrowser";
			this.Resize += new System.EventHandler(this.ResourceBrowser_Resize);
			this.Load += new System.EventHandler(this.ResourceBrowser_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
		
		}

		public void CreateTree()
		{
			FillDataTable();

			string sGrouping = "TypeMajor";
			string sFilter = "MediaType = 'Graphic'";
			string sSorting = "Name";

			XmlDocument doc = new XmlDocument();
			XmlNode rootNode = doc.CreateElement("root");
			doc.AppendChild(rootNode);

			if (sGrouping != null && sGrouping != "")
			{
				//TODO: how to query a dataTable with SQL, like GROUP BY????
				string sQuery = "SELECT DISTINCT("+sGrouping+") AS Group FROM Table";
				//emulating the result of query:
				DataTable dtGroups = new DataTable();
				dtGroups.Columns.Add("Group");
				DataRow row;
				row = dtGroups.NewRow();
				dtGroups.Rows.Add(row);
				row["Group"] = "File";
				row = dtGroups.NewRow();
				dtGroups.Rows.Add(row);
				row["Group"] = "Member";

				if (sFilter.Length > 0)
					sFilter+= " AND ";
				foreach (DataRow rowGroup in dtGroups.Rows)
				{
					string sGroupVal = rowGroup["Group"].ToString();

					XmlNode catNode = doc.CreateElement(sGroupVal);
					rootNode.AppendChild(catNode);

					if (rowGroup["Group"].GetType() == typeof(string))
						sGroupVal = "'"+sGroupVal+"'";
					string sMergedFilter = sFilter + sGrouping+" = "+sGroupVal;
					DataView dvGroup = new DataView(dataTable, sMergedFilter, sSorting,
						DataViewRowState.CurrentRows);

					foreach (DataRowView rowX in dvGroup)
					{
						string sXmlAdjustedName = rowX["Name"].ToString();
						sXmlAdjustedName = System.Text.RegularExpressions.Regex.Replace(
							sXmlAdjustedName, @"[\s]", "");

						int nIndex = sXmlAdjustedName.IndexOf(".");
						if (nIndex >= 0)
							sXmlAdjustedName = sXmlAdjustedName.Substring(0,nIndex);
						XmlNode node = doc.CreateElement(sXmlAdjustedName);
						catNode.AppendChild(node);
						foreach (DataColumn col in dataTable.Columns)
						{
							XmlAttribute attrib = doc.CreateAttribute(col.ColumnName);
							attrib.InnerText = rowX[col.ColumnName].ToString();
							node.Attributes.Append(attrib);
						}
					}

				}
			}

			treeGrid1.XmlDocument = doc;
		}

		private void FillDataTable()
		{
			dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("MediaType", typeof(string)); //Graphic, sound, video, multi
			dataTable.Columns.Add("TypeMajor", typeof(string)); //Member, File, Unit etc
			dataTable.Columns.Add("TypeMinor", typeof(System.Type)); //Member type, unit type etc
			dataTable.Columns.Add("Bytes", typeof(int)); //Memory needed

			dataTable.Columns.Add("Width", typeof(float)); //for graphics only
			dataTable.Columns.Add("Height", typeof(float)); //for graphics only

			dataTable.Columns.Add("NumFrames", typeof(float)); //for video and animating bitmaps

			dataTable.Columns.Add("Length", typeof(float)); //sound&video length in seconds

			//existing members:
			int nNumMembers = EH.Instance.CastLib.GetMemberCount();
			for (int i = 0; i<nNumMembers; i++)
			{
				Endogine.ResourceManagement.MemberBase mb = EH.Instance.CastLib.GetByIndex(i);

				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				row["Name"] = mb.Name;
				row["MediaType"] = "Graphic";
				row["TypeMajor"] = "Member";
				row["Width"] = ((MemberSpriteBitmap)mb).Size.X;
				row["Height"] = ((MemberSpriteBitmap)mb).Size.Y;
			}

			//files in media directory:
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(EH.Instance.CastLib.DirectoryPath);
			System.IO.FileInfo[] aFiles = dirInfo.GetFiles();
			for (int i = 0; i<aFiles.Length; i++)
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				row["Name"] = aFiles[i].Name;
				row["MediaType"] = "Graphic";
				row["TypeMajor"] = "File";
//				row["Width"] = ((MemberSpriteBitmap)mb).Size.X;
//				row["Height"] = ((MemberSpriteBitmap)mb).Size.Y;
			}


		}

		private void ResourceBrowser_Load(object sender, System.EventArgs e)
		{
			CreateTree();
		}


		private void treeGrid1_XmlNodeMouseDown(object sender, System.Xml.XmlNode node)
		{
		}

		private void ResourceBrowser_Resize(object sender, System.EventArgs e)
		{
			this.treeGrid1.Width = this.ClientRectangle.Width-this.treeGrid1.Left;
			this.treeGrid1.Height = this.ClientRectangle.Height-this.treeGrid1.Top;
		}

		private void treeGrid1_XmlNodeMouseStartDrag(object sender, System.Xml.XmlNode node)
		{
			if (node.Attributes.Count > 0)
			{
				string sType = node.Attributes.GetNamedItem("TypeMajor").InnerText;
				treeGrid1.DoDragDrop(sType+";"+node.Name, DragDropEffects.Copy);
			}
		}
	}
}
