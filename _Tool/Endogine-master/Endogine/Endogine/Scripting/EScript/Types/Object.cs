using System;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for Object.
	/// </summary>
	public class Object
	{
		protected object m_value;

		public Object()
		{
		}

		public static Object CreateType(object oUnboxed)
		{
			if (oUnboxed == null)
				return new Object();

			System.Type type = oUnboxed.GetType();
			if (type == typeof(float))
				return new Float((float)oUnboxed);
			if (type == typeof(double))
				return new Float((float)(double)oUnboxed);
			if (type == typeof(int))
				return new Int((int)oUnboxed);
			if (type == typeof(string))
				return new String((string)oUnboxed);
			if (type == typeof(bool))
				return new Int((bool)oUnboxed==true?1:0); //TODO: bool type

			//functions and variables can return/contain any type of object, such as Sprite, Sound etc
			Object o = new Object();
			o.m_value = oUnboxed;
			return o;
		}

		public static Object CreateTypeFromString(string sTokens)
		{
			if (sTokens==null)
				return null;

			char ch = sTokens.Substring(0,1).ToCharArray()[0];
			if ((ch >= 48 && ch <= 57) || ch == 43 || ch == 45 || ch == 46)
			{
				System.Globalization.NumberFormatInfo numFmt = new System.Globalization.CultureInfo("en-US").NumberFormat;
				if (sTokens.IndexOf(".") >= 0)
					return new Float(Convert.ToSingle(sTokens, numFmt)); //NumberFormatInfo
				else
					return new Int(Convert.ToInt32(sTokens));
			}
			else if (ch == '"')
				return new String(sTokens.Substring(1,sTokens.Length-2));
			else if (sTokens.EndsWith("()"))
				return new Method(sTokens);
			else
				return new Variable(sTokens);
		}

		public virtual Object PerformOperation(Executer exec, Operator op, Object otherTerm)
		{
			return null;
		}

		/// <summary>
		/// If it's a method or variable, evaluate and return the Object.
		/// Always an EScript-Object-based value (not dotnet-object)
		/// </summary>
		/// <returns></returns>
		public virtual Object Evaluate(Executer exec)
		{
			//return Object.CreateType(this.GetUnboxed(exec));
			return this; //if not overridden, it evaluated to itself
		}

		/// <summary>
		/// Get the dotnet value
		/// </summary>
		/// <returns></returns>
		public virtual object GetUnboxed(Executer exec)
		{
			//Methods and Variables have no value, they must be evaluated into basic types first:
			if (this.IsNull)
				return null;

			if (this.m_value == null)
			{
				Object o = this.Evaluate(exec);
				return o.GetUnboxed(exec);
			}
			return this.m_value;
		}

		/// <summary>
		/// Is the object truly null - ie not that it only hasn't been evaluated yet.
		/// </summary>
		public virtual bool IsNull
		{
			get
			{
				return this.m_value == null;
			}
		}

//		public override string ToString()
//		{
//			return base.ToString ()+"::";
//		}

//		public virtual Object Copy()
//		{
//			Object o = new Object();
//			o.m_value = this.m_value;
//			return o;
//		}
	}
}
