using System;
using System.Xml;
using System.Collections;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for TreeViewHelper.
	/// </summary>
	public class TreeViewHelper
	{
		public TreeViewHelper()
		{
		}

		public static Hashtable GetExpandedNodeTags(TreeView treeView1)
		{
			Hashtable htExpandedNodes = new Hashtable();
			if (treeView1.Nodes.Count > 0)
			{
				//remember which nodes were expanded, so the view looks the same after an update!
				RecurseGetExpandedNodeTags(treeView1.Nodes, ref htExpandedNodes);
			}
			return htExpandedNodes;
		}

		public static void ExpandNodes(TreeView treeView1, Hashtable htExpandedNodes)
		{
			RecurseExpandNodes(treeView1.Nodes, htExpandedNodes);
		}

		private static void RecurseGetExpandedNodeTags(TreeNodeCollection a_nodes, ref Hashtable a_ht)
		{
			foreach (TreeNode node in a_nodes)
			{
				if (node.IsExpanded)
				{
					Hashtable htSub =  new Hashtable();
					a_ht.Add(node.Tag, htSub);
					RecurseGetExpandedNodeTags(node.Nodes, ref htSub);
				}
			}
		}

		private static void RecurseExpandNodes(TreeNodeCollection a_nodes, Hashtable a_ht)
		{
//			if (m_htMarkers.ContainsKey(a_node.Tag))
//			{
//				a_node.Text = "!! "+a_node.Text;
//			}
			foreach (TreeNode node in a_nodes)
			{
				if (a_ht.ContainsKey(node.Tag))
				{
					node.Expand();
					Hashtable htSub = (Hashtable)a_ht[node.Tag];
					RecurseExpandNodes(node.Nodes, htSub);
				}
			}
		}

		public static void RecurseTreeFromXml(XmlNodeList a_xmlNodes, TreeNodeCollection a_treeNodes)
		{
			foreach (XmlNode node in a_xmlNodes)
			{
				if (node.NodeType == XmlNodeType.Text)
					continue;
				TreeNode treeNode = new TreeNode(node.Name);
				treeNode.Tag = node;
				a_treeNodes.Add(treeNode);

				if (node.ChildNodes.Count > 0)
					RecurseTreeFromXml(node.ChildNodes, treeNode.Nodes);
			}
		}


		public static TreeNode GetTreeNodeFromTag(TreeView treeView1, object tag)
		{
			return RecursiveGetTreeNodeFromTag(treeView1.Nodes, tag);
		}
		private static TreeNode RecursiveGetTreeNodeFromTag(TreeNodeCollection a_nodes, object tag)
		{
			foreach (TreeNode node in a_nodes)
			{
				if (node.Tag == tag)
					return node;
				TreeNode foundNode = RecursiveGetTreeNodeFromTag(node.Nodes, tag);
				if (foundNode != null)
					return foundNode;
			}
			return (TreeNode)null;
		}


		private static void RecurseGetVisible(TreeNodeCollection a_nodes, ref ArrayList a_flatNodeList, bool a_bAddEvenIfNotExpanded)
		{
			foreach (TreeNode node in a_nodes)
			{
				if (a_bAddEvenIfNotExpanded || node.IsExpanded)
					a_flatNodeList.Add(node);
				if (node.IsExpanded)
				{
					RecurseGetVisible(node.Nodes, ref a_flatNodeList, true);
				}
			}
		}
		public static ArrayList GetFlatVisibleTreeNodes(TreeView treeView1)
		{
			ArrayList flatTreeNodes = new ArrayList();
			RecurseGetVisible(treeView1.Nodes, ref flatTreeNodes, true);
			return flatTreeNodes;
		}

	}
}
