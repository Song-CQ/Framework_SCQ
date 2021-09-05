using System;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for MeterBase.
	/// </summary>
	public class MeterBase : Sprite
	{
		protected float m_fMin = 0;
		protected float m_fMax = 1;
		protected float m_fVal;

        object _autoFetchObject;
        System.Reflection.PropertyInfo _autoFetchPropInfo;

		public MeterBase()
		{
		}

		public virtual float Value
		{
			get {return m_fVal;}
			set
			{
				m_fVal = value;
			}
		}

        public float GetFraction()
        {
            return (this.m_fVal - this.m_fMin) / (this.m_fMax - this.m_fMin);
        }

        public float MaxValue
        {
            get { return this.m_fMax; }
            set { this.m_fMax = value; }
        }
        public float MinValue
        {
            get { return this.m_fMin; }
            set { this.m_fMin = value; }
        }

        public void SetAutoFetch(object o, string property)
        {
            this._autoFetchObject = o;
            if (o != null)
                this._autoFetchPropInfo = Endogine.Serialization.Access.GetPropertyInfoNoCase(o, property);
            else
                this._autoFetchPropInfo = null;
        }

        public override void EnterFrame()
        {
            if (this._autoFetchObject != null)
            {
                object o = this._autoFetchPropInfo.GetValue(this._autoFetchObject, null);
                if (o is float)
                    this.Value = (float)o;
                else if (o is int)
                    this.Value = (float)(int)o;
                else if (o is double)
                    this.Value = (float)(double)o;
            }
        }
	}
}
