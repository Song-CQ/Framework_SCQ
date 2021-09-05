using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for ClassNode.
	/// </summary>
	public class ClassNode : BaseNode
	{
		protected bool m_bPaused; //TODO: should be on a global level instead?
		//protected System.Collections.Stack m_nodeStack = new System.Collections.Stack();
		protected BaseNode m_currentNode;

		public ClassNode()
		{
		}

		public MethodNode GetMethod(string sName)
		{
			return (MethodNode)this.ChildNodes[sName];
		}

		public bool Paused
		{
			get {return this.m_bPaused;}
			set {m_bPaused = value;}
		}

//		public void X()
//		{
//			object o = m_currentNode.GetNextToExecute();
//			if (o == null) //node finished
//			{
//				BaseNode parentNode = (BaseNode)m_currentNode.ParentNode;
//				if (m_currentNode.GetType() == typeof(ReturnNode))
//					parentNode.ChildResult = m_currentNode.Result;
//				m_currentNode = parentNode;
//			}
//			else if (o.GetType() == typeof(EScript.Expression))
//			{
//				EScript.Expression expr = (EScript.Expression)o;
//				expr.Evaluate();
//			}
//			else //it's a new node (child of current)
//			{
//				BaseNode childNode = (BaseNode)o;
//				m_nodeStack.Push(childNode);
//			}
			


			//BaseNode nodeX = node.GetNextToExecute();
			//nodeX
//		}
		//TODO: should override AddChild, and check that it's always a Method
		//TODO: this goes for other node types as well (check type of the added node)

		//TODO: add functions to access terms, such as GetFunctionCalls("Put"), GetPropertyAccess("Text") etc
		//so user can find occurrences of interest

		//TODO: add events during execution, so objects can hook into the process while it's running
		//e.g. each time a property is get/set, a function is called.

//		protected override object _Execute()
//		{
//			MethodNode node = this.GetMethod("main");
//			if (node == null)
//			{
//				if (this.ChildNodes.Count == 0)
//					throw new Exception("No methods in this class");
//				node = (MethodNode)this.ChildNodes.GetByIndex(0);
//			}
//			return node.Execute();
//		}
	}
}
