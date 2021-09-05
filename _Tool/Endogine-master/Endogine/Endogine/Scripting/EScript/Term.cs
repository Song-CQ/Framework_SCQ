using System;
using System.Collections;
using System.Reflection;
using Endogine.Scripting.EScript.Types;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Summary description for Term.
	/// </summary>
	public class Term
	{
		public Expression Expression; //if it's not a simple value, but an expression
		//private bool m_bGotExpression;
		private Types.Object m_value;
		private string m_sTerm;

		public Term(Expression expr)
		{
			this.Expression = expr;
		}

		public Term(string sTerm)
		{
			this.m_sTerm = sTerm;
			this.Value = Types.Object.CreateTypeFromString(this.m_sTerm);
		}

		public Term()
		{
		}

		public Types.Object Value
		{
			get {return this.m_value;}
			set {this.m_value = value;}
		}
//		public void Restore()
//		{
//			if (this.Expression != null)
//				this.Expression.Restore();
//			if (this.m_sTerm != null)
//				this.Value = Types.Object.CreateTypeFromString(this.m_sTerm);
//		}

		public bool CanBeMethod()
		{
			if (this.Value == null)
				return false;
			return this.Value.GetType() == typeof(Types.Variable);
		}
		public void ConvertToMethod()
		{
			this.m_sTerm+="()"; //mark as a function for next round of execution (after Restore)
			this.Value = ((Variable)this.Value).ToMethod();
		}

		public Term PerformOperation(Executer exec, Operator op, Term otherTerm)
		{
			//these should be in Types.Object really...
			Types.Object oThis = null;
			
			//TODO: if it's already been evaluated this time, no need to do it again!
			if (this.Expression!=null)
				this.Value = this.Expression.Evaluate(exec);

			oThis = this.Value;

			Types.Object oOther = null;
			if (otherTerm!=null)
			{
				//TODO: if it's already been evaluated this time, no need to do it again!
				if (otherTerm.Expression!=null)
					otherTerm.Value = otherTerm.Expression.Evaluate(exec);
			
				oOther = otherTerm.Value;
			}
			
			//Here's the bad one: can't well set Value - what about next execution?? E.g. if it was a Variable??
			//this.Value = oThis.PerformOperation(exec, op, oOther);
			//return this.Value;
			Term tNew = new Term();
			tNew.Value = oThis.PerformOperation(exec, op, oOther);
			return tNew;
		}

		public override string ToString()
		{
			if (this.Expression!=null)
				return this.Expression.Print();
			return this.Value.ToString();
		}

//		public Term Copy()
//		{
//			Term t = new Term();
//			if (this.Expression!=null)
//				t.Expression = this.Expression.Copy();
//			t.Value = this.Value.Copy();
//			return t;
//		}
	}
}
