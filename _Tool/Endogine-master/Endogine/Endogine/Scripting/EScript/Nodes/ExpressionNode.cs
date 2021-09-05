using System;

namespace Endogine.Scripting.EScript.Nodes
{
	/// <summary>
	/// Summary description for ExpressionNode.
	/// </summary>
	public class ExpressionNode : BaseNode
	{
		private Expression m_expression;
		public ExpressionNode()
		{
		}

		public void SetExpression(string s)
		{
			m_expression = new Expression();
			m_expression.Parse(s);
		}

		public override object Execute(Executer exec)
		{
			//EH.Put(m_expression.Print());
			return m_expression.Evaluate(exec);
		}
	}
}
