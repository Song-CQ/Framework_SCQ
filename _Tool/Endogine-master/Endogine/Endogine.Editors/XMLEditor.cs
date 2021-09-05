using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Data;

using Endogine;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for XMLEditor.
	/// </summary>
	public class XMLEditor : System.Windows.Forms.Form, IXMLEditor
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TreeView treeView1;
		//private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem miAddNode;
		private System.Windows.Forms.MenuItem miAddSubNode;
		private System.Windows.Forms.MenuItem miRemove;
		private System.Windows.Forms.MenuItem miRename;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.MenuItem miFileOpen;
		private System.Windows.Forms.MenuItem miFileSave;
		private XmlDocument m_doc;
		private string m_sCurrentFile;
		private System.Windows.Forms.MenuItem miFileSaveAs;
		private System.Windows.Forms.ColumnHeader columnHeaderAttributes;
		private System.Windows.Forms.ColumnHeader columnHeaderValue;
		private System.Windows.Forms.DataGrid dataGrid1;
		private bool m_bUserEditingLabel = false;
		private Point m_pntLastDataGridCell;

		public XMLEditor()
		{
			InitializeComponent();
			m_pntLastDataGridCell = new Point(-1,-1);
			splitter1.Location = new Point(treeView1.Right, splitter1.Location.Y);
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.miAddNode = new System.Windows.Forms.MenuItem();
			this.miAddSubNode = new System.Windows.Forms.MenuItem();
			this.miRemove = new System.Windows.Forms.MenuItem();
			this.miRename = new System.Windows.Forms.MenuItem();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miFileOpen = new System.Windows.Forms.MenuItem();
			this.miFileSave = new System.Windows.Forms.MenuItem();
			this.miFileSaveAs = new System.Windows.Forms.MenuItem();
			this.columnHeaderAttributes = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderValue = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.treeView1);
			this.panel1.Controls.Add(this.dataGrid1);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(304, 152);
			this.panel1.TabIndex = 0;
			this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(0, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 152);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			this.splitter1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitter1_MouseDown);
			// 
			// treeView1
			// 
			this.treeView1.ContextMenu = this.contextMenu1;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Indent = 15;
			this.treeView1.LabelEdit = true;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Scrollable = false;
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(121, 96);
			this.treeView1.TabIndex = 0;
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
			this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
			this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
			this.treeView1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.miAddNode,
																						 this.miAddSubNode,
																						 this.miRemove,
																						 this.miRename});
			// 
			// miAddNode
			// 
			this.miAddNode.Index = 0;
			this.miAddNode.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miAddNode.Text = "Add at same level";
			this.miAddNode.Click += new System.EventHandler(this.miAddNode_Click);
			// 
			// miAddSubNode
			// 
			this.miAddSubNode.Index = 1;
			this.miAddSubNode.Text = "Add at sublevel";
			this.miAddSubNode.Click += new System.EventHandler(this.miAddSubNode_Click);
			// 
			// miRemove
			// 
			this.miRemove.Index = 2;
			this.miRemove.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.miRemove.Text = "Remove";
			this.miRemove.Click += new System.EventHandler(this.miRemove_Click);
			// 
			// miRename
			// 
			this.miRename.Index = 3;
			this.miRename.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miRename.Text = "Rename";
			this.miRename.Click += new System.EventHandler(this.miRename_Click);
			// 
			// dataGrid1
			// 
			this.dataGrid1.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGrid1.CaptionVisible = false;
			this.dataGrid1.ColumnHeadersVisible = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.FlatMode = true;
			this.dataGrid1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(120, 0);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ParentRowsVisible = false;
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.RowHeadersVisible = false;
			this.dataGrid1.Size = new System.Drawing.Size(160, 96);
			this.dataGrid1.TabIndex = 1;
			this.dataGrid1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGrid1_MouseDown);
			this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			this.dataGrid1.Leave += new System.EventHandler(this.dataGrid1_Leave);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miFileOpen,
																					  this.miFileSave,
																					  this.miFileSaveAs});
			this.menuItem1.Text = "File";
			// 
			// miFileOpen
			// 
			this.miFileOpen.Index = 0;
			this.miFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miFileOpen.Text = "Open";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// miFileSave
			// 
			this.miFileSave.Index = 1;
			this.miFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miFileSave.Text = "Save";
			this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
			// 
			// miFileSaveAs
			// 
			this.miFileSaveAs.Index = 2;
			this.miFileSaveAs.Text = "Save as...";
			this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
			// 
			// columnHeaderAttributes
			// 
			this.columnHeaderAttributes.Text = "Attributes";
			// 
			// columnHeaderValue
			// 
			this.columnHeaderValue.Text = "Value";
			// 
			// XMLEditor
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 273);
			this.Controls.Add(this.panel1);
			this.Menu = this.mainMenu1;
			this.Name = "XMLEditor";
			this.Text = "XMLEditor";
			this.Resize += new System.EventHandler(this.XMLEditor_Resize);
			this.Load += new System.EventHandler(this.XMLEditor_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.XMLEditor_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.XMLEditor_DragEnter);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

//		private void RecurseTreeFromXml(XmlNodeList a_xmlNodes, TreeNodeCollection a_treeNodes)
//		{
//			foreach (XmlNode node in a_xmlNodes)
//			{
//				if (node.NodeType == XmlNodeType.Text)
//					continue;
//				TreeNode treeNode = new TreeNode(node.Name);
//				treeNode.Tag = node;
//				a_treeNodes.Add(treeNode);
//
//				if (node.ChildNodes.Count > 0)
//					RecurseTreeFromXml(node.ChildNodes, treeNode.Nodes);
//			}
//		}

		private void XMLEditor_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				try
				{
					if (files[0].IndexOf(".xml") > 0)
						this.Open(files[0]);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
					return;
				}
			}
		}

		public void Open(string a_sFile)
		{
			m_sCurrentFile = a_sFile;
			XmlTextReader r = new XmlTextReader(a_sFile);
			m_doc = new XmlDocument();
			m_doc.Load(r);
			r.Close();

			UpdateViews();

//			treeView1.Nodes.Clear();
//			TreeNode node;
//			node = new TreeNode("haha");
//			treeView1.Nodes.Add(node);
//
//			
////			treeView1.Nodes.Clear();
//			node = new TreeNode("ug");
//			treeView1.Nodes.Add(node);
//
//			treeView1.Nodes.RemoveAt(0);
			//this.ResizeViews();
			//UpdateViews();
		}

		private void XMLEditor_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// If the data is a file or a bitmap, display the copy cursor.
			if (e.Data.GetDataPresent(DataFormats.FileDrop) ) 
			{
				e.Effect = DragDropEffects.Copy;
				this.Focus();
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}

		}

		private XmlNode GetXmlNodeFromTreeNode(TreeNode a_treeNode)
		{
			return (XmlNode)a_treeNode.Tag;
		}

		private string CreateAttributesString(XmlNode a_node)
		{
			string sAttribs = "";
			foreach (XmlAttribute attrib in a_node.Attributes)
				sAttribs+=attrib.Name+"=\""+attrib.InnerText + "\" ";
			return sAttribs;
		}

		private void UpdateListView()
		{
			ArrayList flatTreeNodes = TreeViewHelper.GetFlatVisibleTreeNodes(treeView1);

			DataTable dt = new DataTable();
			DataColumn col = dt.Columns.Add();
			col.ColumnName = "Attributes";

			col = dt.Columns.Add();
			col.ColumnName = "Text";

			foreach (TreeNode node in flatTreeNodes)
			{
				//EndogineHub.Put(node.Text);
				XmlNode xmlNode = (XmlNode)node.Tag;

				string sAttribs = CreateAttributesString(xmlNode);
				string sText = "";
				foreach (XmlNode subNode in xmlNode.ChildNodes)
				{
					if (subNode.NodeType == XmlNodeType.Text)
						sText = subNode.InnerText;
				}
				DataRow row = dt.NewRow();
				dt.Rows.Add(row);
				row["Attributes"] = sAttribs;
				row["Text"] = sText;
			}

			dataGrid1.DataSource = dt;
		}

		private void ResizeViews()
		{
			int nHeight = panel1.Height;

			ArrayList flatTreeNodes = TreeViewHelper.GetFlatVisibleTreeNodes(treeView1);
			if (flatTreeNodes.Count > 0)
			{
				TreeNode lastNode = (TreeNode)flatTreeNodes[flatTreeNodes.Count-1];
				nHeight = Math.Max(lastNode.Bounds.Bottom + 20, nHeight);
			}
			//expand tree and list views so they're as tall as needed (panel will supply scrollbar)
			treeView1.Height = nHeight;
			dataGrid1.Height = nHeight;
		}

		private void UpdateViews()
		{
			XmlNode selected = null;
			if (treeView1.SelectedNode != null)
				selected = (XmlNode)treeView1.SelectedNode.Tag;
			Hashtable htExpanded = TreeViewHelper.GetExpandedNodeTags(treeView1);

			TreeNode tempNode = null;
			if (treeView1.Nodes.Count > 0)
			{
				//TODO: for some reason, clearing the treeview nodes makes the following node collection half-invisible!!! .NET bug!!!
				//treeView1.Nodes.Clear();
				tempNode = new TreeNode("_temp");
				treeView1.Nodes.Add(tempNode);
				treeView1.Nodes.RemoveAt(0);
			}
			Endogine.Editors.TreeViewHelper.RecurseTreeFromXml(m_doc.ChildNodes, treeView1.Nodes);
			if (tempNode != null)
				treeView1.Nodes.Remove(tempNode);

			TreeViewHelper.ExpandNodes(treeView1, htExpanded);
			UpdateListView();

			if (selected != null)
			{
				TreeNode node = TreeViewHelper.GetTreeNodeFromTag(treeView1, selected);
				if (node != null)
					treeView1.SelectedNode = node;
			}

			ResizeViews();

			//TODO: this doesn't work...
			//this.splitter1.Location = new Point(treeView1.Right, splitter1.Location.Y);
		}

		private void treeView1_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
		}
		private void treeView1_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
		}

		private void treeView1_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateListView();
			ResizeViews();
		}

		private void treeView1_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateListView();
			ResizeViews();
		}

		private void splitter1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		private void treeView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
//			if (e.KeyCode == Keys.Delete)
//			{
//			}
		}

		private void miRemove_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				XmlNode node = this.GetXmlNodeFromTreeNode(treeView1.SelectedNode);
				if (node != null && node.ParentNode != null)
				{
					XmlNode parent = node.ParentNode;
					parent.RemoveChild(node);
					treeView1.SelectedNode = TreeViewHelper.GetTreeNodeFromTag(treeView1, parent);
					UpdateViews();
				}
			}
		}

		private void miRename_Click(object sender, System.EventArgs e)
		{
			m_bUserEditingLabel = true;
			treeView1.SelectedNode.BeginEdit();
		}
		private void treeView1_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (m_bUserEditingLabel == false)
				return;
			m_bUserEditingLabel = false;
			if (e.Label == null)
				return;

			XmlNode node = this.GetXmlNodeFromTreeNode(e.Node);
			XmlNode newNode = Endogine.Serialization.XmlHelper.RenameNode(node, e.Label);

			this.UpdateViews();
			TreeNode selected = TreeViewHelper.GetTreeNodeFromTag(treeView1, newNode);
			treeView1.SelectedNode = selected;
//			this.UpdateViews();
		}


		private XmlNode AddNodeToChildNodes(XmlNode a_node, XmlNode a_nodeToAdd)
		{
			if (a_nodeToAdd == null)
				a_nodeToAdd = m_doc.CreateNode(XmlNodeType.Element, "New", null);

			a_node.AppendChild(a_nodeToAdd);
			UpdateViews();
			return a_nodeToAdd;
		}

		private void miAddSubNode_Click(object sender, System.EventArgs e)
		{
			XmlNode xmlNode = null;
			if (treeView1.SelectedNode != null)
				xmlNode = GetXmlNodeFromTreeNode(treeView1.SelectedNode);
			else
				xmlNode = (XmlNode)m_doc;
			this.AddNodeToChildNodes(xmlNode, (XmlNode)null);
		}

		private void miAddNode_Click(object sender, System.EventArgs e)
		{
			XmlNode xmlNode = null;
			if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
				xmlNode = GetXmlNodeFromTreeNode(treeView1.SelectedNode.Parent);
			else 
				xmlNode = (XmlNode)m_doc;
			this.AddNodeToChildNodes(xmlNode, (XmlNode)null);
		}

		private void miFileOpen_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
				this.Open(dlg.FileName);
		}

		private void miFileSave_Click(object sender, System.EventArgs e)
		{
			if (m_sCurrentFile == null)
				return;

			m_doc.Save(m_sCurrentFile);
		}

		private void XMLEditor_Load(object sender, System.EventArgs e)
		{
			//this.Open("C:\\Documents and Settings\\Jonas\\Mina dokument\\Visual Studio Projects\\EndoTest01\\Test.xml");
		}

		private void miFileSaveAs_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				m_sCurrentFile = dlg.FileName;
				m_doc.Save(m_sCurrentFile);
			}
		}

		private void XMLEditor_Resize(object sender, System.EventArgs e)
		{
			panel1.Width = this.ClientRectangle.Width;
			panel1.Height = this.ClientRectangle.Height;
			ResizeViews();
		}

		private void panel1_Resize(object sender, System.EventArgs e)
		{
			if (dataGrid1 != null)
			{
				dataGrid1.Width = panel1.Width - dataGrid1.Left;
				DataTable dt = (DataTable)dataGrid1.DataSource;
				if (dt != null)
				{
					if (dt.Columns.Count > 0)
					{
						dataGrid1.PreferredColumnWidth = dataGrid1.Width/dt.Columns.Count-2;
					}
				}
			}
			//dataGrid1.Controls
			//for (int i = 0; i < dt.Columns.Count; i++)
			//	 dt.Columns[i].Width = dataGrid1.Width/dt.Columns.Count-2;
		}

		private void dataGrid1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//dataGrid1.ReadOnly = false;
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			ArrayList aVisibleNodes = TreeViewHelper.GetFlatVisibleTreeNodes(treeView1);
			int nNumVisible = aVisibleNodes.Count;
			//TODO: ugly way to stop the extra line in dataview (for adding new rows) from being available:
			int nCurrRow = dataGrid1.CurrentCell.RowNumber;
			if (nCurrRow >= nNumVisible)
			{
				//dataGrid1.Select(nCurrRow-1);
				dataGrid1.CurrentRowIndex = nCurrRow-1;
			}
			else
				dataGrid1.ReadOnly = false;

			if (m_pntLastDataGridCell.X >= 0)
			{

				string sVal = Convert.ToString(((DataTable)dataGrid1.DataSource).Rows[m_pntLastDataGridCell.Y][m_pntLastDataGridCell.X]);
				TreeNode treeNode = (TreeNode)aVisibleNodes[m_pntLastDataGridCell.Y];
				XmlNode xmlNode = (XmlNode)treeNode.Tag;

				if (m_pntLastDataGridCell.X == 0)
				{
					string sForXMLParse = "<temp "+sVal+"/>";
					XmlDocument xmlDoc = new XmlDocument();
					try
					{
						xmlDoc.LoadXml(sForXMLParse);
						xmlNode.Attributes.RemoveAll();

						foreach (XmlAttribute attrib in xmlDoc.ChildNodes[0].Attributes)
						{
							XmlAttribute newAttrib = (XmlAttribute)m_doc.CreateNode(XmlNodeType.Attribute, attrib.Name, null);
							newAttrib.InnerText = attrib.InnerText;
							xmlNode.Attributes.Append(newAttrib);
						}
					}
					catch
					{
						((DataTable)dataGrid1.DataSource).Rows[m_pntLastDataGridCell.Y][m_pntLastDataGridCell.X] = CreateAttributesString(xmlNode);
						//MessageBox.Show("Attributes formatting invalid");
					}
					EndogineHub.Put(sVal);
				}
				else if (m_pntLastDataGridCell.X == 1)
				{
					bool bFound = false;
					foreach (XmlNode childNode in xmlNode.ChildNodes)
					{
						if (childNode.NodeType == XmlNodeType.Text)
						{
							childNode.InnerText = sVal;
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						XmlNode newNode = m_doc.CreateTextNode(sVal);
						xmlNode.AppendChild(newNode);
					}
				}
			}
			m_pntLastDataGridCell.Y = dataGrid1.CurrentRowIndex;
			m_pntLastDataGridCell.X = dataGrid1.CurrentCell.ColumnNumber;
		}

		private void dataGrid1_Leave(object sender, System.EventArgs e)
		{
			//so that the extra row at bottom doesn't show
			dataGrid1.ReadOnly = true;
		}
	}
}
