using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for SceneGraphViewer.
	/// </summary>
    public partial class SceneGraphViewer : System.Windows.Forms.Form, ISceneGraphViewer
	{
        Endogine.Editors.DragDropHelper dragDropHelper;
        Sprite m_spRoot; //this window's root (doesn't have to be the stage's root)
        PropertyInspector m_pi;
        List<Sprite> _markedSprites;
        Dictionary<Sprite, TreeNode> _spriteToNode;
        

		public SceneGraphViewer()
		{
			InitializeComponent();

            this._markedSprites = new List<Sprite>();

			MenuItem mi, miSub;
			
			mi = new MenuItem("Refresh");
			miView.MenuItems.Add(mi);
			mi.Shortcut = System.Windows.Forms.Shortcut.F5;
			mi = new MenuItem("&Filter...");
			miView.MenuItems.Add(mi);
			mi = new MenuItem("S&ort...");
			miView.MenuItems.Add(mi);
			mi = new MenuItem("&Group...");
			miView.MenuItems.Add(mi);
			mi = new MenuItem("-");
			miView.MenuItems.Add(mi);

			mi = new MenuItem("&Load preset");
			miView.MenuItems.Add(mi);
			miSub = new MenuItem("From file...");
			mi.MenuItems.Add(miSub);
			miSub = new MenuItem("Offscreen");
			mi.MenuItems.Add(miSub);
			miSub = new MenuItem("Invisible");
			mi.MenuItems.Add(miSub);
			miSub = new MenuItem("Scaled");
			mi.MenuItems.Add(miSub);
			miSub = new MenuItem("Rotated");
			mi.MenuItems.Add(miSub);

			mi = new MenuItem("&Save preset...");
			miView.MenuItems.Add(mi);

			foreach (MenuItem miViewItems in miView.MenuItems)
				miViewItems.Click+=new EventHandler(miViewItems_Click);

			EndogineHub endo = EndogineHub.Instance;
			m_spRoot = endo.Stage.RootSprite;

			RefreshView();

			dragDropHelper = new DragDropHelper(this.treeView1);
			dragDropHelper.DragDrop+=new DragEventHandler(dragDropHelper_DragDrop);
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			RefreshView();
		}

		public void SetRootSprite(Sprite sp)
		{
			m_spRoot = sp;
			RefreshView();
		}

		public void RefreshView()
		{
			Sprite oldSelectedSprite = this.SelectedSprite;

            List<Sprite> expandedSprites = new List<Sprite>();
			if (treeView1.Nodes.Count > 0)
			{
				//remember which nodes were expanded, so the view looks the same after an update!
                this.RecurseGetExpandedNodes(treeView1.Nodes[0], expandedSprites);
			}

            this.FillTreeStartingFrom(m_spRoot);

            if (expandedSprites.Count > 0)
                this.RecurseExpandNodes(treeView1.Nodes[0], expandedSprites);

			if (oldSelectedSprite != null)
				this.SelectedSprite = oldSelectedSprite;
		}

        private void RecurseGetExpandedNodes(TreeNode a_node, List<Sprite> expandedSprites)
        {
            if (a_node.IsExpanded)
                expandedSprites.Add(this.SpriteFromNode(a_node));

            foreach (TreeNode child in a_node.Nodes)
            {
                if (child.IsExpanded)
                    this.RecurseGetExpandedNodes(child, expandedSprites);
            }
        }

        private void RecurseExpandNodes(TreeNode a_node, List<Sprite> expandedSprites)
		{
            //if (this._markedNodes.Contains(a_node))
            //    a_node.Text = "!! "+a_node.Text;

            if (expandedSprites.Contains(this.SpriteFromNode(a_node)))
			{
				a_node.Expand();
				foreach (TreeNode child in a_node.Nodes)
                    this.RecurseExpandNodes(child, expandedSprites);
			}
		}

        private void FillTreeStartingFrom(Sprite sp)
        {
            this._spriteToNode = new Dictionary<Sprite, TreeNode>();
            this.treeView1.Nodes.Clear();
			this.RecurseAddNodes((TreeNode)null, sp, -1);
        }

		private void RecurseAddNodes(TreeNode a_node, Sprite a_sp, int a_nNumRecurseLevels)
		{

			//show name, member name, and class name
			string sText = a_sp.GetSceneGraphName();

			TreeNode newNode = new TreeNode(sText);
			newNode.Tag = a_sp.GetHashCode();

			if (a_node == null)
				this.treeView1.Nodes.Add(newNode);
			else
				a_node.Nodes.Add(newNode); //treeView1.SelectedNode.Nodes.Add(newNode);

            this._spriteToNode.Add(a_sp, newNode);

			if (a_nNumRecurseLevels > 0)
				a_nNumRecurseLevels--;
			for (int i = 0; i < a_sp.ChildCount; i++)
			{
				Sprite sp = a_sp.GetChildByIndex(i);
				if (a_nNumRecurseLevels == -1 || a_nNumRecurseLevels > 0)
					RecurseAddNodes(newNode, sp, a_nNumRecurseLevels);
			}
		}

        /// <summary>
        /// Finds other Forms in the MDI that want to switch to show this sprite's properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void treeView1_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
            //TODO: use events instead, this is ugly...

			Sprite sp = this.SpriteFromNode(e.Node);
			//TODO: when not in Mdi mode
			if (this.MdiParent != null)
			{
				foreach (Form form in this.MdiParent.MdiChildren)
				{
					System.Type type = form.GetType();
					if (type == typeof(LocScaleRotEdit))
					{
						if (((LocScaleRotEdit)form).AutoswitchToSprite)
							((LocScaleRotEdit)form).EditSprite = sp;
					}
					else if (type == typeof(PropertyInspector))
					{
						if (((PropertyInspector)form).AutoswitchToSprite)
							((PropertyInspector)form).ShowProperties(sp);
					}
				}
			}
		}


		private void miViewItems_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			switch (mi.Text)
			{
				case "Refresh":
					RefreshView();
					break;
				case "&Filter...":
					//TODO: make dataTable with all sprites and all their props, use SQL SELECTs
					break;
				case "S&ort...":
					break;
				case "&Group...":
					break;
				case "&Load view...":
					break;
				case "&Save view...":
					break;
			}
		}

		private TreeNode RecurseGetNode(int a_nSpriteHash, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (a_nSpriteHash == (int)node.Tag)
					return node;
				if (node.Nodes.Count > 0)
				{
					TreeNode foundNode = RecurseGetNode(a_nSpriteHash, node.Nodes);
					if (foundNode != null)
						return foundNode;
				}
			}
			return (TreeNode)null;
		}
		private TreeNode NodeFromSprite(Sprite a_sp)
		{
            if (a_sp == null)
                return null;
            if (this._spriteToNode == null)
                this.RefreshView();
            if (!this._spriteToNode.ContainsKey(a_sp))
                this.RefreshView();

            return this._spriteToNode[a_sp];
            //int nHash = a_sp.GetHashCode();
            ////TreeNodeCollection nodes = treeView1.Nodes;
            //return RecurseGetNode(nHash, treeView1.Nodes);
		}
		private Sprite SpriteFromNode(TreeNode a_node)
		{
            if (a_node == null)
                return null;
            if (this._spriteToNode == null)
                this.RefreshView();

            foreach (KeyValuePair<Sprite, TreeNode> kv in this._spriteToNode)
        	{
                if (kv.Value == a_node)
                    return kv.Key;
        	}
            return null;

			if (a_node == null)
				return (Sprite)null;
			//find out which sprite the node refers to.
			//start at root sprite, then walk the hierarchy to find sprite
			TreeNode parent = a_node;
			ArrayList aHierarchy = new ArrayList();
			for (;;)
			{
				aHierarchy.Insert(0, parent);
				if (parent.Parent == null)
					break;
				parent = parent.Parent;
			}
			aHierarchy.RemoveAt(0); //we know that the first in hierarchy is the root sprite.

			Sprite sp = m_spRoot;
			bool bFoundIt = true;
			foreach (TreeNode node in aHierarchy)
			{
				int hash = (int)node.Tag;
				Sprite spChild = sp.GetChildByHashCode(hash);
				if (spChild == null)
				{
					//TODO: messagebox doesn't appear for some reason...
					//MessageBox.Show("Sprite has been deleted or moved");
					bFoundIt = false;
					break;
				}
				else
					sp = spChild;
			}
			if (bFoundIt)
				return sp;
			return (Sprite)null;
		}

		public void MarkSprite(Sprite sp)
		{
            TreeNode node = this.NodeFromSprite(sp);
			this.MarkNode(node);
		}
		public void MarkNode(TreeNode a_node)
		{
			if (a_node == null)
				return;

			Sprite sp = this.SpriteFromNode(a_node);
			if (sp != null)
			{
                //check if there's already a marker on the sprite:
                if (this._markedSprites.Contains(sp))
				{
                    ////yes, so remove it:
                    this._markedSprites.Remove(sp);
                    //TODO: it shouldn't be a behavior, but a plain child sprite. Easier to manage...
                    BhSpriteTransformer bhXform = (BhSpriteTransformer)sp.GetBehaviorByIndex(0);
                    bhXform.Dispose();
                    a_node.ForeColor = Color.Black;
				}
				else
				{
                    //no; add a transformer:
					BhSpriteTransformer bhXform = new BhSpriteTransformer();
					sp.AddBehavior(bhXform);
					bhXform.Init();
                    a_node.ForeColor = Color.Red;
                    this._markedSprites.Add(sp);
				}
			}
		}

        public Sprite SelectedSprite
		{
			get
			{
				return this.SpriteFromNode(treeView1.SelectedNode);
			}
			set
			{
				TreeNode node = NodeFromSprite(value);
				if (node != null)
					treeView1.SelectedNode = node;
			}
		}

		private void miMark_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode == null)
				return;
			MarkNode(treeView1.SelectedNode);
		}

		private void miDelete_Click(object sender, System.EventArgs e)
		{
			Rectangle rct = this.treeView1.SelectedNode.Bounds;
			this.SpriteFromNode(treeView1.SelectedNode).Dispose();
			this.RefreshView();

			TreeNode node = this.treeView1.GetNodeAt(rct.X+1, rct.Y+1);
			if (node == null)
				node = this.treeView1.GetNodeAt(rct.X+1, rct.Y-4);
			if (node != null)
				this.treeView1.SelectedNode = node;
		}

		private void miExport_Click(object sender, System.EventArgs e)
		{
			this.Export(null, false);
		}


		private void miImport_Click(object sender, System.EventArgs e)
		{
			this.Import(EH.Instance.Stage.RootSprite, true);
		}

		private void SceneGraphViewer_Activated(object sender, System.EventArgs e)
		{
			RefreshView();
		}

		private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
			}
		}


		private void treeView1_DoubleClick(object sender, System.EventArgs e)
		{
		}

		private void miProperties_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode == null)
				return;

			Sprite sp = SpriteFromNode(treeView1.SelectedNode);
			if (sp!=null)
			{
				if (m_pi == null || m_pi.IsDisposed)
				{
					m_pi = new PropertyInspector();
					m_pi.MdiParent = this.MdiParent;
				}
				m_pi.Show();
				m_pi.ShowProperties(sp);
			}
		}

		private void miOpenNodeInNew_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode == null)
				return;

			Sprite sp = SpriteFromNode(treeView1.SelectedNode);

			SceneGraphViewer scgrvw = new SceneGraphViewer();
			scgrvw.MdiParent = this.MdiParent;
			scgrvw.SetRootSprite(sp);
			scgrvw.Show();
		}

		private void miCenterCamera_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode == null)
				return;

			Sprite sp = SpriteFromNode(treeView1.SelectedNode);
			EPointF pnt = sp.ConvParentLocToRootLoc(sp.Loc);
			EndogineHub endo = EndogineHub.Instance;
			EPointF pntSize = new EPointF(endo.Stage.RenderControl.Width, endo.Stage.RenderControl.Height);
			endo.Stage.Camera.Loc = pnt+endo.Stage.Camera.Loc - pntSize*0.5f;
		}

		private void miLocScaleRot_Click(object sender, System.EventArgs e)
		{
			LocScaleRotEdit ctrl = new LocScaleRotEdit();
			Sprite sp = SpriteFromNode(treeView1.SelectedNode);
			ctrl.EditSprite = sp;
			ctrl.MdiParent = this.MdiParent;
			ctrl.Show();
		}


		private void Export(Sprite a_sp, bool a_bOnlyChildren)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Endogine/Director (*.sgr)|*.sgr|All files|*.*";
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				//TODO: plugin system for serialization!!
				Endogine.Serialization.EndogineXML.Save(dlg.FileName, a_sp, a_bOnlyChildren);
			}
		}
		private void Import(Sprite a_sp, bool a_bReplace)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Endogine/Director (*.sgr)|*.sgr|Photoshop (*.psd)|*.psd|Flash (*.swf)|*.swf|All files|*.*";
			//dlgOpenFile.ShowReadOnly = true;
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				System.IO.FileInfo finfo = new System.IO.FileInfo(dlg.FileName);
				if (finfo.Extension == ".psd")
				{
					Endogine.Serialization.Photoshop.Document psd = new Endogine.Serialization.Photoshop.Document(dlg.FileName);
				}
				else
					Endogine.Serialization.EndogineXML.Load(dlg.FileName, a_sp);
				//TODO: why doesn't it work to show this messagebox?
				//	if (MessageBox.Show(this, "Merge with current scene (otherwise replace)?", "Merge or replace", MessageBoxButtons.YesNo,
				//		MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)

			}
		}

		private void miExportOnlyChildren_Click(object sender, System.EventArgs e)
		{
			this.Export(this.SelectedSprite, true);
		}

		private void miExportThisAndChildren_Click(object sender, System.EventArgs e)
		{
			this.Export(this.SelectedSprite, false);
		}

		private void miImportReplace_Click(object sender, System.EventArgs e)
		{
			this.Import(this.SelectedSprite, true);
		}
		private void miImportMerge_Click(object sender, System.EventArgs e)
		{
		
		}

		private void miImportMovie_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Endogine/Director (*.sgr)|*.sgr|All files|*.*";
			//dlgOpenFile.ShowReadOnly = true;
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				Endogine.Serialization.EndogineXML.LoadMovie(dlg.FileName, this.SelectedSprite);
			}
		}

		private void SceneGraphViewer_Resize(object sender, System.EventArgs e)
		{
			this.treeView1.Width = this.ClientRectangle.Right-this.treeView1.Left;
			this.treeView1.Height = this.ClientRectangle.Bottom-this.treeView1.Top;
		}

		private void dragDropHelper_DragDrop(object sender, DragEventArgs e)
		{
			this.RefreshView();
		}

        private void miCopy_Click(object sender, EventArgs e)
        {
            Sprite sp = this.SelectedSprite.Copy();
        }
	}
}
