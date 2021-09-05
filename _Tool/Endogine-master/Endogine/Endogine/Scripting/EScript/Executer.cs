using System;
using Endogine.Scripting.EScript.Nodes;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Summary description for Executer.
	/// </summary>
	public class Executer
	{
		private System.Collections.Stack m_callStack;
		private System.Collections.ArrayList m_valueStack;
		private BaseNode m_currentNode;

		private bool m_bPaused;

		public Executer(MethodNode nodeToExecute)
		{
			this.Rewind();
			this.m_callStack.Push(nodeToExecute);
		}

		public object Run()
		{
			//EH.Put("Start!");
			while (this.Step() && !this.m_bPaused)
			{}
			return null;
		}
		public bool Paused
		{
			get {return this.m_bPaused;}
			set {this.m_bPaused = value;}
		}
		public void Rewind()
		{
			//EH.Put("Rewind called");
			MethodNode nodeToExecute = null;
			if (this.m_callStack != null)
			{
				//find first item in stack (i.e., the first called method)
				while (this.m_callStack.Count > 1)
					this.m_callStack.Pop();
				nodeToExecute = (MethodNode)this.m_callStack.Pop();
			}

			this.m_callStack = new System.Collections.Stack();
			this.m_valueStack = new System.Collections.ArrayList();
			this.m_currentNode = null;
			if (nodeToExecute != null)
				this.m_callStack.Push(nodeToExecute);
		}

		public bool Step() //returns false when finished
		{
			bool bDebug = false;
			BaseNode owningNode = (BaseNode)this.m_callStack.Peek();
			BaseNode nextNode = owningNode.GetNextNode(this, this.m_currentNode, null);

			if (nextNode == null)
			{
				if (bDebug) EH.Put("No more nodes on that depth");
				//that node is finished. Go up one level or quit!
				if (this.m_callStack.Count <= 1) //there has to be two nodes above the current!
				{
					//fire event! Quit!
					return false;
				}
				//step up one level, and find next node there:
				this.m_currentNode = (BaseNode)this.m_callStack.Pop();
				this.Step();
			}
			else
			{
				if (nextNode.ParentNode != owningNode) //when going down one level (always just one)
				{
					this.m_callStack.Push(nextNode.ParentNode);
					this.m_currentNode = nextNode;
					if (bDebug) EH.Put("Entering childnode: "+nextNode.Name);
				}
				else //new node is at same level
				{
					this.m_currentNode = nextNode;
					if (bDebug) EH.Put("Nextsibling : "+nextNode.Name); //nextNode.GetType().ToString()
				}
			}
//			if (this.m_currentNode.GetType() == typeof(IfNode))
//				this.m_currentNode = this.m_currentNode;

			//only ExpressionNodes can be executed, the others are only asked for next node.
			if (this.m_currentNode.GetType() != typeof(ExpressionNode))
			{
				this.m_callStack.Push(this.m_currentNode);
				this.m_currentNode = null;
				return this.Step();
			}
			else
			{
				this.m_currentNode.Execute(this); //object oVal = 
				return true;
			}
		}
	}
}
