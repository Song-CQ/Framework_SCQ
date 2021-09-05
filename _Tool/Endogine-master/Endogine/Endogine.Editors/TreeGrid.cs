using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace Endogine.Editors
{
	public delegate void XmlNodeEvent(object sender, XmlNode node);
	/// <summary>
	/// Summary description for TreeGrid.
	/// </summary>
	public class TreeGrid : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView treeView1;
		private System.ComponentModel.Container components = null;

		private XmlDocument xmlDoc;
		public event XmlNodeEvent XmlNodeMouseDown = null;
		public event XmlNodeEvent XmlNodeMouseStartDrag = null;

		private XmlNode xmlNodeMouseDownOn = null; //Which node the mouse button was pressed on
		private EPoint mouseDownLoc;
		private System.Windows.Forms.Panel panelTreeView;
		private Endogine.Editors.VersatileDataGrid versatileDataGrid1;
		private Endogine.Editors.CustomSplitter customSplitter1;
		private bool m_bStartedDragging = false;

		public TreeGrid()
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.panelTreeView = new System.Windows.Forms.Panel();
			this.versatileDataGrid1 = new Endogine.Editors.VersatileDataGrid();
			this.customSplitter1 = new Endogine.Editors.CustomSplitter();
			this.panelTreeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Indent = 15;
			this.treeView1.LabelEdit = true;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Scrollable = false;
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(121, 88);
			this.treeView1.TabIndex = 0;
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
			this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
			this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
			this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseMove);
			// 
			// panelTreeView
			// 
			this.panelTreeView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelTreeView.Controls.Add(this.treeView1);
			this.panelTreeView.Location = new System.Drawing.Point(0, 15);
			this.panelTreeView.Name = "panelTreeView";
			this.panelTreeView.Size = new System.Drawing.Size(120, 104);
			this.panelTreeView.TabIndex = 6;
			// 
			// versatileDataGrid1
			// 
			this.versatileDataGrid1.BackColor = System.Drawing.SystemColors.Window;
			this.versatileDataGrid1.Location = new System.Drawing.Point(120, 0);
			this.versatileDataGrid1.Name = "versatileDataGrid1";
			this.versatileDataGrid1.Size = new System.Drawing.Size(144, 128);
			this.versatileDataGrid1.TabIndex = 7;
			this.versatileDataGrid1.ScrolledVertical += new Endogine.Editors.VersatileDataGrid.ScrolledDelegate(this.versatileDataGrid1_ScrolledVertical);
			// 
			// customSplitter1
			// 
			this.customSplitter1.BackColor = System.Drawing.Color.Black;
			this.customSplitter1.Cursor = System.Windows.Forms.Cursors.VSplit;
			this.customSplitter1.Location = new System.Drawing.Point(120, 0);
			this.customSplitter1.Name = "customSplitter1";
			this.customSplitter1.Size = new System.Drawing.Size(2, 136);
			this.customSplitter1.TabIndex = 8;
			this.customSplitter1.SplitterMoved += new Endogine.Editors.CustomSplitter.SplitterEventHandler(this.customSplitter1_SplitterMoved);
			// 
			// TreeGrid
			// 
			this.Controls.Add(this.customSplitter1);
			this.Controls.Add(this.versatileDataGrid1);
			this.Controls.Add(this.panelTreeView);
			this.Name = "TreeGrid";
			this.Size = new System.Drawing.Size(256, 136);
			this.Resize += new System.EventHandler(this.TreeGrid_Resize);
			this.Load += new System.EventHandler(this.TreeGrid_Load);
			this.panelTreeView.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public XmlDocument XmlDocument
		{
			set
			{
				this.xmlDoc = value;
				this.treeView1.Nodes.Clear();
				if (this.xmlDoc != null)
				{
					if (this.xmlDoc.ChildNodes.Count > 0)
					{
						if (this.xmlDoc.ChildNodes[0].ChildNodes.Count > 0)
							TreeViewHelper.RecurseTreeFromXml(this.xmlDoc.ChildNodes[0].ChildNodes, this.treeView1.Nodes);
					}
				}
				this.UpdateListView();
			}
			get {return this.xmlDoc;}
		}

		/// <summary>
		/// The main use for the TreeGrid will be a Multi-Object PropertyGrid.
		/// In this case, send a TreeNodeCollection, with each Node.Tag being an object
		/// </summary>
		public void SetObjectTree(TreeNodeCollection tree)
		{
			this.treeView1.Nodes.Clear();
			foreach (TreeNode node in tree)
				this.treeView1.Nodes.Add(node);
		}

		/// <summary>
		/// Instead of using an XmlDocument as a source, a DataView can be used,
		/// providing that there exists a column called "_TreePath" which contains
		/// a text with hierarchical info:
		/// _treepath	name
		///					root
		/// root			x
		/// root			y
		/// root/x		a
		/// root/y		a
		/// Note that a node called "root" isn't required
		/// </summary>
		public DataView DataView
		{
			set
			{
				//System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)value[0]["Bitmap"];
			}
		}

		private void ResizeViews()
		{
			int nWidth = this.ClientRectangle.Width; //this.ClientRectangle.Width
			this.versatileDataGrid1.Width = nWidth-this.versatileDataGrid1.Left;

			int nHeight = this.ClientRectangle.Height;
			//this.treeView1.Height = nHeight-this.treeView1.Top;
			this.panelTreeView.Height = nHeight-this.treeView1.Top;
			this.versatileDataGrid1.Height = nHeight-this.versatileDataGrid1.Top;
		}

		private void UpdateListView()
		{
			ArrayList flatTreeNodes = TreeViewHelper.GetFlatVisibleTreeNodes(treeView1);

			DataTable dt = new DataTable();
			
			foreach (TreeNode node in flatTreeNodes)
			{
				XmlNode xmlNode = (XmlNode)node.Tag;
				DataRow row = dt.NewRow();

//				if (xmlNode.Attributes.GetNamedItem("TypeMajor")
				foreach (XmlAttribute attrib in xmlNode.Attributes)
				{
					DataColumn col = dt.Columns[attrib.Name];
					if (col == null)
					{
						col = dt.Columns.Add(attrib.Name, typeof(string));
					}
					row[col] = attrib.InnerText;

					bool bDisplayColumnAsBitmap = false;
					if (bDisplayColumnAsBitmap)
					{
						Hashtable htBitmaps = new Hashtable();
						Bitmap bmp = (Bitmap)htBitmaps[xmlNode];
					}
				}
				dt.Rows.Add(row);
			}
			this.versatileDataGrid1.DataSource = dt;
			this.versatileDataGrid1.AutoAdjustColumns(30,120,false);
			this.treeView1.Height = Math.Max(this.versatileDataGrid1.GridTotalHeight+10, this.panelTreeView.Height);
		}

		private void treeView1_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateListView();
			ResizeViews();
		}

		private void treeView1_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateListView();
			ResizeViews();
		}

		private void TreeGrid_Load(object sender, System.EventArgs e)
		{
		
		}

		private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_bStartedDragging = false;
			this.mouseDownLoc = new EPoint(e.X, e.Y);
			XmlNode xmlNode = null;
			try
			{
				TreeNode node = treeView1.GetNodeAt(e.X, e.Y);
				if (node != null)
					xmlNode = (XmlNode)node.Tag;
			}
			catch
			{
				return;
			}

			if (xmlNode==null)
				return;

			this.xmlNodeMouseDownOn = xmlNode;
			if (XmlNodeMouseDown!=null)
				XmlNodeMouseDown(this, xmlNode);
		}

		private void TreeGrid_Resize(object sender, System.EventArgs e)
		{
			ResizeViews();
		}

		private void treeView1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (xmlNodeMouseDownOn!=null)
			{
				EPoint pntDiff = this.mouseDownLoc-new EPoint(e.X, e.Y);
				if (!m_bStartedDragging && pntDiff.ToEPointF().Length > 5)
				{
					m_bStartedDragging = true;
					if (XmlNodeMouseStartDrag!=null)
						XmlNodeMouseStartDrag(this, xmlNodeMouseDownOn);
				}
			}
		}

		private void treeView1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			xmlNodeMouseDownOn = null;
		}

		private void versatileDataGrid1_ScrolledVertical(object sender, int newLoc)
		{
			this.treeView1.Top = -newLoc;
		}

		private void customSplitter1_SplitterMoved(object sender, int newLoc)
		{
			this.panelTreeView.Width = this.customSplitter1.Left-this.panelTreeView.Left;
			this.treeView1.Width = this.panelTreeView.Width;
			this.versatileDataGrid1.Left = this.customSplitter1.Left+this.customSplitter1.Width;
			this.versatileDataGrid1.Width = this.ClientRectangle.Right-this.versatileDataGrid1.Left;
		}
	}
}
