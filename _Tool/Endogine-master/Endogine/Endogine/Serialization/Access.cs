using System;
using System.Reflection;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for Access.
	/// </summary>
	public class Access
	{
		public Access()
		{
		}

		//TODO: goddamn, why can't I GetType() return PropertyInfo in runtime,  but
		//an unusable RuntimePropertyInfo instead?!?!?
		public static bool FindAsPropOrField(object obj, string sName, ref PropertyInfo propInfo, ref FieldInfo fieldInfo)
		{
			propInfo = GetPropertyInfoNoCase(obj, sName);
			if (propInfo != null)
				return true;
			fieldInfo = GetFieldInfoNoCase(obj, sName); 
			if (fieldInfo != null)
				return true;
			return false;
		}
//		public static object FindAsPropOrField(object obj, string sName)
//		{
//			PropertyInfo propInfo = GetPropertyInfoNoCase(obj, sName);
//			if (propInfo != null)
//				return propInfo;
//			FieldInfo fieldInfo = GetFieldInfoNoCase(obj, sName);
//			if (fieldInfo != null)
//				return fieldInfo;
//			return null;
//		}
		public static object MethodExistsInObject(object obj, string sName, System.Type[] aParams)
		{
			System.Type type = obj.GetType();
			if (aParams == null)
				aParams = new System.Type[]{};
			MethodInfo methodInfo = type.GetMethod(sName, aParams); //TODO: types of params
			return methodInfo!=null;
		}

		public static bool PropertyExistsInObject(object obj, string sProp)
		{
			System.Type type = obj.GetType();
			PropertyInfo propInfo = type.GetProperty(sProp);
			return (propInfo != null);
		}

		public static object GetProperty(object obj, string sProp)
		{
			System.Type type = obj.GetType();
			PropertyInfo propInfo = type.GetProperty(sProp);
			return propInfo.GetValue(obj, null);
		}
		public static object GetPropertyNoCase(object obj, string sProp)
		{
			PropertyInfo propInfo = GetPropertyInfoNoCase(obj, sProp);
			return propInfo.GetValue(obj, null);
		}

		public static System.Reflection.PropertyInfo GetPropertyInfoNoCase(object obj, string sProp)
		{
			sProp = sProp.ToLower();
			System.Type type = obj.GetType();
			System.Reflection.PropertyInfo[] propInfos = type.GetProperties();
			foreach (System.Reflection.PropertyInfo propInfo in propInfos)
			{
				if (propInfo.Name.ToLower() == sProp)
					return propInfo;
			}
			return null;
		}
		public static System.Reflection.FieldInfo GetFieldInfoNoCase(object obj, string sField)
		{
			sField = sField.ToLower();
			System.Type type = obj.GetType();
			System.Reflection.FieldInfo[] fieldInfos = type.GetFields();
			foreach (System.Reflection.FieldInfo fieldInfo in fieldInfos)
			{
				if (fieldInfo.Name.ToLower() == sField)
					return fieldInfo;
			}
			return null;
		}

		public static void SetProperty(object obj, string sProp, string sVal)
		{
			System.Type type = obj.GetType();
			PropertyInfo propInfo = type.GetProperty(sProp);
			object val = GetStringAsType(propInfo.PropertyType, sVal);
			propInfo.SetValue(obj, val, null);
		}
		public static void SetProperty(object obj, string sProp, object oVal)
		{
			System.Type type = obj.GetType();
			PropertyInfo propInfo = type.GetProperty(sProp);
			if (propInfo.PropertyType != oVal.GetType())
			{
				if (propInfo.PropertyType == typeof(double))
					oVal = (double)oVal;
				else if (propInfo.PropertyType == typeof(float))
					oVal = (float)oVal;
				else if (propInfo.PropertyType == typeof(int))
					oVal = (int)oVal;
			}
			propInfo.SetValue(obj, oVal, null);
		}
		public static void SetField(object o, string sField, object oVal)
		{
			FieldInfo fieldInfo = GetFieldInfoNoCase(o, sField); 
			if (fieldInfo != null)
				fieldInfo.SetValue(o, oVal);
		}

		public static object CallMethod(object obj, string sMethod, object[] aArgs)
		{
			System.Type type = obj.GetType();
			MethodInfo[] methodInfos = type.GetMethods();
			foreach (MethodInfo methodInfo in methodInfos)
			{
				if (methodInfo.Name == sMethod)
				{
					if (methodInfo.GetParameters().Length != aArgs.Length)
						continue;

					//make it simple for now, just accept the method regardless of parameter types
					ParameterInfo[] parameterInfos = methodInfo.GetParameters();
					for (int i = 0; i < aArgs.Length; i++)
					{
						ParameterInfo parameterInfo = parameterInfos[i];
						System.Type paramType = parameterInfo.ParameterType;
						if (paramType != aArgs[i].GetType())
						{
							if (paramType == typeof(int))
								aArgs[i] = Convert.ToInt32(aArgs[i]); //(int)(double)aArgs[i];
							else if (paramType == typeof(float))
								aArgs[i] = Convert.ToSingle(aArgs[i]); //(float)(double)
						}
					}

					//TODO: does this always return a RuntimePropertyInfo?? .net bug?
					return methodInfo.Invoke(obj, aArgs);
//					PropertyInfo propInfo = (PropertyInfo)methodInfo.Invoke(obj, aArgs);
//					return propInfo.GetValue(obj, null);
				}
			}
			return null;
		}
		public static object CallMethod(object obj, string sMethod, string[] aArgs)
		{
			object[] aParams = null;
			System.Type[] aTypes = null;
			if (aArgs!=null)
			{
				aParams = new object[aArgs.Length];
				aTypes = new Type[aArgs.Length];
				for (int i = 0; i < aParams.Length; i++)
				{
					aParams[i] = GuessGetStringAsObject(aArgs[i]);
					aTypes[i] = aParams[i].GetType();
				}
			}
			System.Type type = obj.GetType();

			MethodInfo methodInfo = null;
			if (aTypes!=null)
				methodInfo = type.GetMethod(sMethod, aTypes);
			else
				methodInfo = type.GetMethod(sMethod);

//			ParameterInfo[] aPI = methodInfo.GetParameters();
//			for (int i = 0; i < aPI.Length; i++)
//				aParams[i] = GetStringAsType(aPI[i].ParameterType, aArgs[i]);
			return methodInfo.Invoke(obj, aParams);
		}

		public static object GuessGetStringAsObject(string sVal)
		{
			object o = null;
			try
			{
				o = Convert.ToInt32(sVal);
			}
			catch
			{
				try
				{
					o = Convert.ToSingle(sVal);
				}
				catch
				{
					return sVal;
				}
			}
			return o;
		}

		public static object GetStringAsType(System.Type type, string sVal)
		{
			if (type == typeof(int))
				return (int)Convert.ToSingle(sVal);
			else if (type == typeof(float))
				return Convert.ToSingle(sVal);
			else if (type == typeof(double))
				return Convert.ToDouble(sVal);
			return sVal;
		}
	}
}
