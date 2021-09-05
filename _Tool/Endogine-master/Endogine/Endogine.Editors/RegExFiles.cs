using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Endogine.Editors
{
    public partial class RegExFiles : UserControl
    {
        public RegExFiles()
        {
            InitializeComponent();
        }

        public string[] FileNames
        {
            get { return this.GetFilenames(); }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "*.*";
            dlg.DereferenceLinks = true;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //this.tbRegEx.Lines = dlg.FileNames;
                string s = Endogine.Files.FileFinder.StringsToHumanReadable(dlg.FileNames);

                this.tbRegEx.Text += s + "\r\n";
            }
        }


        private string[] GetFilenames()
        {
            string[] strings = Endogine.Files.FileFinder.HumanReadableToStrings(this.tbRegEx.Text);
            return Endogine.Files.FileFinder.GenerateFilenamesFromRegexList(strings);
        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedIndex == 1)
            {
                //TODO: check which lines are marked in this.tbRegEx.
                //The corresponding files should be marked in tbFiles!
                string[] allFiles = this.GetFilenames();
                string text = "";
                foreach (string file in allFiles)
                    text += file + "\r\n";
                this.tbFiles.Text = text;
            }
            else if (tc.SelectedIndex == 2)
            {
                string[] allFiles = this.GetFilenames();
                this.treeView1.Nodes.Clear();
                if (allFiles.Length > 0)
                {
                    //this.treeView1.Nodes.Add("root");
                    foreach (string file in allFiles)
                        this.GetOrCreateNode(this.treeView1.Nodes, null, file);

                    //find first node in hierarchy that has more than one child
                    //make that the first node in the tree. User can right-click and step up if needed.
                    TreeNode node = this.treeView1.Nodes[0];
                    while (true)
                    {
                        if (node.Nodes.Count > 1 || node.Nodes.Count == 0)
                        {
                            TreeNode parent = node.Parent;
                            //TODO: should I dispose of the nodes that are removed here..?
                            if (parent != null)
                            {
                                this.treeView1.Nodes.Clear();
                                this.treeView1.Nodes.Add(parent);
                            }
                            else
                            {
                                this.treeView1.Nodes[0].Nodes.Clear();
                                this.treeView1.Nodes[0].Nodes.Add(node);
                            }
                            break;
                        }
                        node = node.Nodes[0];
                    }

                    //For each 
                    this.treeView1.Nodes[0].ExpandAll();
                }
            }
        }

        private TreeNode GetOrCreateNode(TreeNodeCollection addToNodes, TriStateTreeNode startNode, string XPath)
        {
            string relativePath = null;
            string[] nodeNames = null;
            char delimiter = '\\';
            if (startNode == null)
            {
                relativePath = XPath;
                int i = relativePath.IndexOf(delimiter);
                string name = relativePath.Substring(0, i);
                startNode = new TriStateTreeNode(name);
                startNode.Name = name;
                startNode.Tag = name;
                addToNodes.Add(startNode);
                relativePath = relativePath.Remove(0, i+1);
                nodeNames = relativePath.Split(delimiter);
            }
            else
            {
                relativePath = XPath.Remove(0, ((string)startNode.Tag).Length);
                nodeNames = relativePath.Split(delimiter);
            }

            TriStateTreeNode node = startNode;
            foreach(string name in nodeNames)
            {
                TriStateTreeNode newNode = (TriStateTreeNode)node.Nodes[name];
                if (newNode == null)
                {
                    bool isLeaf = (name == nodeNames[nodeNames.Length - 1]);

                    if (!isLeaf)
                    {
                        //TreeNode showTheRestOfDirectoryNode = node.Nodes.Add("Other...");
                        //showTheRestOfDirectoryNode.Name = "N/A";
                        //this.treeView1.SetTreeNodeState(showTheRestOfDirectoryNode, TriStateTreeView.CheckBoxState.Unchecked);
                    }

                    newNode = new TriStateTreeNode(name);
                    node.Nodes.Add(newNode);
                    //newNode = node.Nodes.Add(name);
                    newNode.Name = name;
                    newNode.Tag = XPath;
                    this.treeView1.SetTreeNodeState(newNode, TriStateTreeView.CheckBoxState.Unchecked);

                    if (!isLeaf)
                        newNode.Expand();
                    //node.ImageIndex = 0;
                }
                node = newNode;
            }
            this.treeView1.ToggleTreeNodeState(node);

            node.ForeColor = System.Drawing.Color.Blue;
            return node;
        }

        private void FillNodeWithFolderContents(TriStateTreeNode folderNode)
        {
            ////Remove "root\"
            //if (folderRealPath.StartsWith("root"))
            //    folderRealPath = folderRealPath.Remove(0, 5);
            ////string filePath = newNode.FullPath.Remove(0, (startNode.Text + startNode.TreeView.PathSeparator).Length);
            //if (folderRealPath.EndsWith(":"))
            //    folderRealPath += "\\";

            DirectoryInfo di = new DirectoryInfo((string)folderNode.Tag);
            if (!di.Exists)
                return;
            FileSystemInfo[] fsis = di.GetFileSystemInfos();

            //sort alphabetically
            SortedList<string, TreeNode> sorted = new SortedList<string, TreeNode>();
            Dictionary<string, TriStateTreeView.CheckBoxState> states = new Dictionary<string, TriStateTreeView.CheckBoxState>();
            foreach (FileSystemInfo fsi in fsis)
            {
                TriStateTreeNode node = (TriStateTreeNode)folderNode.Nodes[fsi.Name];
                if (node == null)
                {
                    //TODO: is fsi a folder or a file? How to find out?
                    node = new TriStateTreeNode(fsi.Name);
                    folderNode.Nodes.Add(node);
                    //node = folderNode.Nodes.Add(fsi.Name);
                    node.Name = fsi.Name;
                    node.Tag = fsi.FullName;
                    this.treeView1.SetTreeNodeState(node, TriStateTreeView.CheckBoxState.Unchecked);
                }
                sorted.Add(fsi.Name, node);
                states.Add(fsi.Name, this.treeView1.GetTreeNodeState(node));
            }
            folderNode.Nodes.Clear();

            sorted.GetEnumerator();
            for (int i = 0; i < sorted.Count; i++)
            {
                TriStateTreeNode node = (TriStateTreeNode)sorted.Values[i];
                TriStateTreeView.CheckBoxState state = states[node.Name];
                folderNode.Nodes.Add(node);
                this.treeView1.SetTreeNodeState(node, state);
            }
        }


        private void RegExFiles_Resize(object sender, EventArgs e)
        {
            this.tabControl1.Width = ((Control)sender).Width - this.tabControl1.Left;
            this.tabControl1.Height = ((Control)sender).Height - this.tabControl1.Top;
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            Control ctrl = (Control)this.tabPage1;
            this.tbFiles.Width = ctrl.Width - this.tbFiles.Left;
            this.tbFiles.Height = ctrl.Height - this.tbFiles.Top;

            this.tbRegEx.Width = ctrl.Width - this.tbRegEx.Left;
            this.tbRegEx.Height = ctrl.Height - this.tbRegEx.Top;

            this.treeView1.Width = ctrl.Width - this.treeView1.Left;
            this.treeView1.Height = ctrl.Height - this.treeView1.Top;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.treeView1.SelectedNode = e.Node;
                bool showAddParent = true;
                if (e.Node.Parent == null)
                    showAddParent = false;
                this.contextMenuStrip1.Items["addParentFolderToolStripMenuItem"].Enabled = showAddParent;
                this.contextMenuStrip1.Items["addParentsToRootToolStripMenuItem"].Enabled = showAddParent; //showContentsToolStripMenuItem

                this.contextMenuStrip1.Show((Control)sender, e.X, e.Y);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name == "N/A")
            {
                //remove "Other..."
                string s = e.Node.FullPath;
                s = s.Substring(0,s.LastIndexOf("\\"));
                TriStateTreeNode parent = (TriStateTreeNode)e.Node.Parent;
                parent.Nodes.Remove(e.Node);
                this.FillNodeWithFolderContents(parent);
            }
        }
    }
}
