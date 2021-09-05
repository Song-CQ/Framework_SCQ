using System;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for FSConstant.
	/// </summary>
	public class Literal : Object
	{
		public Literal()
		{
		}

		protected void CheckOperation(Operator op)
		{
			if (op == null)
				return;

			if (op.IsSettingOperator)
				throw new Exception("Can't set a constant expression, e.g. 5=3");

			if (op.InternalTokens == ".")
			{
				//TODO: allow 1.ToString() etc? If so, perform call on "this"
				throw new Exception("Can't use . operator on constants - yet");
			}
		}

		public override string ToString()
		{
			return this.m_value.ToString();
		}
	}
}
