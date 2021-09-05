using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// An IfNode is either an 'if', an 'else if' or an 'else' node
	/// 'if' is the default. If a sub-if node is added to that node,
	/// it's either an 'else if' or an 'else' node (depending on if there's an IfStatement or not)
	/// </summary>
	public class IfNode : ChunkNode
	{
		private Expression m_ifExpression;
		private IfNode m_nextIfNode;

		public IfNode()
		{
//No, changed my mind...			//First child node is the If statement, second is an If  (for "else if") or Chunk node (for "else")
//			this.ChildNodes.Add("If", null);
//			this.ChildNodes.Add("possibleElseIf", null);
		}

		public void SetIfStatement(string s)
		{
			this.m_ifExpression = new Expression();
			this.m_ifExpression.Parse(s);

//			ExpressionNode exprNode = new ExpressionNode();
//			exprNode.SetExpression(s);
//			this.ChildNodes.SetByIndex(0, exprNode);
		}


		public void SetNextIfNode(IfNode node) //next 'else if' or 'else' node
		{
			m_nextIfNode = node;
		}

		public override BaseNode GetNextNode(Executer exec, BaseNode alreadyUsedChildNode, object nodesResult)
		{
			//Hmm, could I execute the if-statement directly here? Must I really wait 
			//for executer to execute it and then proceed based on the result??
			
			//I'll go for direct if-clause execution for now:
			if (alreadyUsedChildNode == null)
			{
//				if (this.m_ifExpression==null || Convert.ToInt32(this.m_ifExpression.Evaluate(exec)) != 0)
				if (this.m_ifExpression==null ||
					Convert.ToInt32(this.m_ifExpression.Evaluate(exec).GetUnboxed(exec)) != 0)
					return (BaseNode)this.FirstChild;
				if (this.m_nextIfNode!=null)
					return this.m_nextIfNode;
				return null;
			}
			return base.GetNextNode(exec, alreadyUsedChildNode, nodesResult);

//			int nIndex = this.ChildNodes.IndexOfValue(alreadyUsedChildNode);
//			if (nIndex == 0) //it was the "if" clause
//			{
//				if ((int)((Types.Object)nodesResult).GetUnboxed() != 0)
//					return (BaseNode)this.ChildNodes.GetByIndex(2); //this is where the statements begin if it's true
//				if (this.ChildNodes.Count > 1)
//					return (BaseNode)this.ChildNodes.GetByIndex(1); 
//			}
//			return null;
		}
	}
}
