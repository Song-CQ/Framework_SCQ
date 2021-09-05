using System;
using System.Collections;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Summary description for Functions.
	/// </summary>
	public class Functions
	{
		public static Functions Instance;
		private static Hashtable m_htUserValues;
		private static int m_someValue;
		public static Nodes.ClassNode ThisNode;
		public static Nodes.BaseNode CurrentNode;

		public Functions()
		{
			Instance = this;
			m_htUserValues = new Hashtable();
		}

		public static object GetUserValue(string sName)
		{
			return m_htUserValues[sName];
		}
		public static void SetUserValue(string sName, object oVal)
		{
			m_htUserValues[sName] = oVal;
		}
		public static bool UserValueExists(string sName)
		{
			return m_htUserValues.ContainsKey(sName);
		}


		public static int SomeValue
		{
			get {return m_someValue;}
			set {m_someValue = value;}
		}

		public static System.Reflection.PropertyInfo GetPropertyInfo(object o, string sProperty)
		{
			return null;
		}

		public static double Random(double dMin, double dMax)
		{
			Random rnd = new Random();
			return rnd.NextDouble()*(dMax-dMin)+dMin;
		}

		public static void Put(object oVal)
		{
			//TODO: how to "automatically" call the unboxed value's ToString() function?? I don't want "Int32", I want "9"!
			//I can't specify each and every type that might be used here...
			string s = "";
			if (oVal.GetType() == typeof(int))
				s = ((int)oVal).ToString();
			else if (oVal.GetType() == typeof(float))
				s = ((float)oVal).ToString();
			else if (oVal.GetType() == typeof(double))
				s = ((double)oVal).ToString();
			else if (oVal.GetType() == typeof(string))
				s = ((string)oVal).ToString();

			EH.Put(s);
		}
		public static int foo(int val1, int val2)
		{
			return val1+val2;
		}
		public static int bar(int val1, int val2)
		{
			return val1*val2;
		}
	}
}
