using System;
using System.Collections;
using System.Reflection;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Facilitates scripting, by treating Properties, Fields, and UserValues the same - 
	/// and case insensitive (EScript Parameters as well?)
	/// </summary>
	public class MemberValueWrapper
	{
		private FieldInfo m_field;
		private PropertyInfo m_prop;
		private string m_userValue;
		private Nodes.ClassNode m_classNode;
		private object m_otherType;

		private object m_belongsToObj;

		public bool IsExec;

		//TODO: a list for searching after members in objects if not found in default...private static ArrayList m_

		public MemberValueWrapper(string sName, object obj)
		{
			if (sName == "this")
				this.m_classNode = Functions.ThisNode;
			else if (sName == "_exec")
				this.IsExec = true;
			else
			{
				m_belongsToObj = obj;
				if (m_belongsToObj == null)
					m_belongsToObj = Functions.Instance;

				if (this.FindAsPropOrField(sName) == false)
				{
					m_userValue = sName;
				}
				//throw new Exception("No such field or property: "+sName);
			}
		}

		private bool FindAsPropOrField(string sName)
		{
			return Serialization.Access.FindAsPropOrField(this.m_belongsToObj, sName, ref this.m_prop, ref this.m_field);
		}

		public object Value
		{
			//TODO: stack with scope / _exec for the thread
			get
			{
				if (this.m_prop != null)
					return this.m_prop.GetValue(this.m_belongsToObj, null);
				else if (this.m_field != null)
					return this.m_field.GetValue(this.m_belongsToObj);
				else if (this.m_userValue != null)
					return Functions.GetUserValue(this.m_userValue);
				else if (this.m_classNode != null)
					return this.m_classNode;
				else
					return m_otherType;
			}
			set
			{
				if (this.m_prop != null)
				{
					//TODO: automatic casting to proper type
					if (value.GetType() == typeof(float))
					{
						if (this.m_prop.PropertyType == typeof(int))
							value = (int)(float)value;
					}
					this.m_prop.SetValue(this.m_belongsToObj, value, null);
				}
				else if (this.m_field != null)
					this.m_field.SetValue(this.m_belongsToObj, value);
				else if (this.m_userValue != null)
					Functions.SetUserValue(this.m_userValue, value);
				else if (this.m_classNode != null)
					throw new Exception("'this' can't be set!");
				else
					this.m_otherType = value;
			}
		}
	}
}
