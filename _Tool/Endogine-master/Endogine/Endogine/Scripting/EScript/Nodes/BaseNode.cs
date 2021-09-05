using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for BaseNode.
	/// </summary>
	public class BaseNode : Endogine.Node
	{
		public BaseNode()
		{
		}

		public virtual object Execute(Executer exec)
		{
			return null;
		}

		public ClassNode ClassNode
		{
			get 
			{
				BaseNode node = (BaseNode)this.RootNode;
				return (ClassNode)node.ChildNodes.GetByIndex(0);
			}
		}

		public MethodNode ParentMethod
		{
			get
			{
				BaseNode node = (BaseNode)this.ParentNode;
				if (node != null)
				{
					if (node.GetType() == typeof(MethodNode))
						return (MethodNode)node;
					return node.ParentMethod;
				}
				return null;
			}
		}

		public virtual BaseNode GetNextNode(Executer exec, BaseNode alreadyUsedChildNode, object nodesResult)
		{
			if (alreadyUsedChildNode != null)
				return (BaseNode)alreadyUsedChildNode.NextSibling;
			if (this.HasChildNodes)
				return (BaseNode)this.FirstChild;
			return null;
		}
	}
}