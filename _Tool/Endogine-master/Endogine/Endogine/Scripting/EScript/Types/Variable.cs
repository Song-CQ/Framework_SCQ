using System;
using System.Reflection;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// A Variable is first a string, then it evaluates to Valuewrapper, and finally to a .net predefined value
	/// </summary>
	public class Variable : Member
	{
		public MemberValueWrapper ValueWrapper;

		public Variable(string s)
		{
			this.Name = s;
		}

		public MemberValueWrapper EvaluateToWrapper(Executer exec)
		{
			if (this.ValueWrapper == null)
				this.ValueWrapper = new MemberValueWrapper(this.Name, this.BelongsToObject);
			if (this.ValueWrapper.IsExec)
				this.ValueWrapper.Value = exec;
			return this.ValueWrapper;
		}

		public override Object Evaluate(Executer exec)
		{
			//EH.Put("var:"+this.Name);
			//this.ValueWrapper = null;
			object oNewVal = this.EvaluateToWrapper(exec).Value;
			return Object.CreateType(oNewVal);
		}

		public override Object PerformOperation(Executer exec, Operator op, Object otherTerm)
		{
			if (op == null)
				return this;

			if (op.InternalTokens == ".")
			{
				object oUnboxed = null;
				oUnboxed = this.Evaluate(exec).GetUnboxed(exec);

				if (otherTerm.GetType() == typeof(Method))
				{
					Method func = (Method)otherTerm;
					func.BelongsToObject = oUnboxed;
					return func.Evaluate(exec);
				}
				if (otherTerm.GetType() == typeof(Variable))
				{
					Variable var = (Variable)otherTerm;
					var.BelongsToObject = oUnboxed;
					var.EvaluateToWrapper(exec);
					return var;
				}
				return null;
			}

			if (op.IsSettingOperator)
			{
				this.EvaluateToWrapper(exec);

				if (!op.IsBinary)
				{
					//TODO: ++ and -- operators
					return this;
				}

				Object oNewVal = otherTerm.Evaluate(exec);
				if (op.InternalTokens!="=")
				{
					//for other than "=", we need to know the current value
					string sSubOp = op.InternalTokens.Substring(0,1);
					Operator subOp = Parser.GetOperator(sSubOp);
					this.PerformOperation(exec, op, oNewVal);
					oNewVal = this;
				}
				this.ValueWrapper.Value = oNewVal.GetUnboxed(exec);

				return oNewVal;
			}

			Object oEvaluated = this.Evaluate(exec);
			return oEvaluated.PerformOperation(exec, op, otherTerm);
		}

		/// <summary>
		/// Used while parsing only; first Methods will be considered Variables,
		/// but when "(" is encountered it will be transformed to a Method.
		/// </summary>
		/// <returns></returns>
		public Method ToMethod()
		{
			return new Method(this.Name);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override bool IsNull
		{
			get
			{
				if (this.ValueWrapper == null && this.Name == null)
					return base.IsNull;
				return false;
			}
		}
	}
}