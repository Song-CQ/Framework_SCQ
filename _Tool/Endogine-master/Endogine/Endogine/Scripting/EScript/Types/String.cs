using System;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for String.
	/// </summary>
	public class String : Literal
	{
		public String(string s)
		{
			this.m_value = s;
		}

		public override Object PerformOperation(Executer exec, Operator op, Object otherTerm)
		{
			if (op == null)
				return this;

			this.CheckOperation(op);

			string sReturn = "";
			string sThis = (string)this.GetUnboxed(exec);
			string sOther = otherTerm.GetUnboxed(exec).ToString();
			switch (op.InternalTokens)
			{
				case "+":
					sReturn = sThis + sOther;
					break;
				case "-":
					sReturn = sThis.Replace(sOther, "");
					break;
				case "*":
					for (int i = Convert.ToInt32(sOther)-1; i>=0; i--)
						sReturn+=sThis;
					break;
			}

			//this.m_value = sReturn;
			//return this;
			return Types.Object.CreateType(sReturn);
		}
	}
}
