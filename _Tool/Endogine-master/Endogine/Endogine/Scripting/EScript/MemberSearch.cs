using System;
using System.Collections;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Utility to find a field/property or method in one of the specified objects
	/// </summary>
	public class MemberSearch
	{
		public static ArrayList SearchObjects = new ArrayList();

		public MemberSearch()
		{
		}


		public static object FindVariableObject(string sName) //object[] aParams
		{
			foreach (object o in SearchObjects)
			{
				System.Reflection.PropertyInfo propInfo = null;
				System.Reflection.FieldInfo fieldInfo = null;
				object oPropOrFieldInfo = Serialization.Access.FindAsPropOrField(o, sName, ref propInfo, ref fieldInfo);
				if (oPropOrFieldInfo != null)
					return o;
			}
			return null;
		}

		public static object FindMethodObject(string sName, System.Type[] aParamTypes)
		{
			//System.Type[] aParamTypes = null;
			foreach (object o in SearchObjects)
			{
				object oMethodInfo = Serialization.Access.MethodExistsInObject(o, sName, aParamTypes);
				if (oMethodInfo != null)
					return o;
			}
			return null;
		}
	}
}
