using System;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for Number.
	/// </summary>
	public class Number : Literal
	{
		public Number()
		{
		}

		public override Object PerformOperation(Executer exec, Operator op, Object otherTerm)
		{
			if (op == null)
				return this;

			this.CheckOperation(op);

			double dReturn = 0;
			double dThis = Convert.ToDouble(this.GetUnboxed(exec));
			if (op.IsBinary)
			{
				double dOther =  Convert.ToDouble(otherTerm.GetUnboxed(exec));
				switch (op.InternalTokens)
				{
					case "+":
						dReturn = dThis + dOther;
						break;
					case "-":
						dReturn = dThis - dOther;
						break;
					case "*":
						dReturn = dThis * dOther;
						break;
					case "/":
						dReturn = dThis / dOther;
						break;
					case "==":
						return Types.Object.CreateType(dThis == dOther);
					case ">=":
						return Types.Object.CreateType(dThis >= dOther);
					case "<=":
						return Types.Object.CreateType(dThis <= dOther);
					case ">":
						return Types.Object.CreateType(dThis > dOther);
					case "<":
						return Types.Object.CreateType(dThis < dOther);
					case "!=":
						return Types.Object.CreateType(dThis != dOther);
				}
			}
			else //unary
			{
				switch (op.InternalTokens)
				{
					case "pre-":
						dReturn=-dThis;
						break;
				}
			}
			System.Type type = this.GetType();
			//Nope - never change the actual value, instead return a new object with the value!
//			if (type == typeof(Int))
//				this.m_value = (int)dReturn;
//			else if (type == typeof(Float))
//				this.m_value = (float)dReturn;
//			return this;

			if (type == typeof(Int))
				return Types.Object.CreateType((int)dReturn);
			else if (type == typeof(Float))
				return Types.Object.CreateType((float)dReturn);
			
			throw new Exception("Unknown number type");
		}
	}
}
