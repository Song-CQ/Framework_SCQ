using System;

namespace Endogine.Scripting.EScript.Types
{
	/// <summary>
	/// Summary description for Member.
	/// </summary>
	public class Member : Object
	{
		public string Name;
		protected object m_belongsToObject;

		public Member()
		{
		}

//		protected object FindObjectForProperty(string sProp)
//		{
//			object obj = Functions.Instance;
//			if (Serialization.Access.PropertyExistsInObject(obj, sProp))
//				return obj;
//
//			if (Functions.UserValueExists(sProp))
//			{
//			}
//		}

		public object BelongsToObject
		{
			get 
			{
//				if (this.m_belongsToObject == null)
//					this.m_belongsToObject = Functions.Instance;
				return this.m_belongsToObject;
			}
			set {this.m_belongsToObject = value;}
		}

		public override bool IsNull
		{
			get
			{
				if (this.Name == null)
					return base.IsNull;
				return false;
			}
		}
	}
}
