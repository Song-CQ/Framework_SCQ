using System;
using System.Collections.Generic;

namespace Endogine.Animation
{
	/// <summary>
	/// Animates an object property
	/// </summary>

	public class Animator
	{
		public enum Modes
		{
			Once,
			Loop,
			PingPong
		}
		private Modes m_mode = Modes.Once;

		private bool m_bActive = true;

		private float m_fTime;
		private float m_fStep;
		private SortedList<float,AnimationKey> _slKeys;

		private System.Reflection.PropertyInfo m_pi = null;
		private object m_obj;

		private int m_nFrameInterval = 0;
		private int m_nTimeInterval = 0;

		private System.Timers.Timer m_timer;

		public Animator(object a_obj, string a_sProp)
		{
			m_obj = a_obj;
			System.Type type = a_obj.GetType();
			m_pi = type.GetProperty(a_sProp);

            _slKeys = new SortedList<float, AnimationKey>();

			if (m_pi == null)
				throw new Exception("Property "+a_sProp+" not found for animating "+a_obj.ToString());

			m_fTime = 0.0f;
			m_fStep = 1.0f;
			
			this.FrameInterval = 1;
		}

		public Modes Mode
		{
			get {return m_mode;}
			set {m_mode = value;}
		}

		public bool Active
		{
			get {return this.m_bActive;}
			set {this.m_bActive = value;}
		}

		public int FrameInterval
		{
			get {return this.m_nFrameInterval;}
			set
			{
				if (this.m_timer!=null)
				{
					this.m_timer.Dispose();
					this.m_timer = null;
					this.m_nTimeInterval = 0;
				}
				if (this.m_nFrameInterval == 0)
					EH.Instance.EnterFrameEvent+=new Endogine.EnterFrame(m_endogine_EnterFrameEvent);
				this.m_nFrameInterval = value;
			}
		}

		public int TimeInterval
		{
			get {return this.m_nTimeInterval;}
			set
			{
				if (this.m_nFrameInterval > 0)
				{
					EH.Instance.EnterFrameEvent-=new Endogine.EnterFrame(m_endogine_EnterFrameEvent);
					this.m_nFrameInterval = 0;
				}
				this.m_nTimeInterval = value;
				this.m_timer = new System.Timers.Timer(value);
				this.m_timer.Elapsed+=new System.Timers.ElapsedEventHandler(m_timer_Elapsed);
				this.m_timer.Start();
			}
		}

		public void Dispose()
		{
			//TODO: not sure how delegates work... When object is disposed, will the delegates that point to it be cleaned automatically?
            if (this.m_timer!=null)
                this.m_timer.Dispose();
			EH.Instance.EnterFrameEvent-=new Endogine.EnterFrame(m_endogine_EnterFrameEvent);
		}

		public float StepSize
		{
			get {return m_fStep;}
			set
			{
				m_fStep = value;
			}
		}

		public float GetValueAtTime(float a_fTime)
		{
			if (_slKeys.Count > 1)
			{
				//TODO: use interpolation strategies. Right now it's linear.
				//http://www.tinaja.com/cubic01.asp
				AnimationKey key1 = this.GetKeyNearTime(a_fTime, 0, true);
				AnimationKey key2 = this.GetKeyNearTime(a_fTime, 1, true);

				if (key1.Time == key2.Time) //TODO: shouldn't really happen? key2 should be null instead?
					return key1.Value;

				float fWhereInbetween = (a_fTime-key1.Time)/(key2.Time-key1.Time);
				float fVal = (key2.Value-key1.Value)*fWhereInbetween + key1.Value;
				return fVal;
			}
			else
				return a_fTime;
		}

		public void Step()
		{
			if (!m_bActive)
				return;

            if (this.m_fStep == 0)
                return;

			this.m_fTime+=this.m_fStep;
			if (this._slKeys.Count >= 1)
			{
                //TODO: optimize - should cache last key's time.
				AnimationKey finalKey = this._slKeys.Values[this._slKeys.Count-1];
				if (this.m_fTime > finalKey.Time)
				{
					if (m_mode == Modes.Once)
					{
						m_bActive = false;
						return;
					}
					else if (m_mode == Modes.Loop)
						m_fTime = 0;
				}
			}

            //TODO: optimize - should cache next key's time and only proceed if it's been passed.
			float fVal = this.GetValueAtTime(m_fTime);

			if (m_pi != null)
			{
				if (m_pi.PropertyType == typeof(int))
					m_pi.SetValue(m_obj, (int)fVal, null);
				else if (m_pi.PropertyType == typeof(float))
					m_pi.SetValue(m_obj, fVal, null);
				else if (m_pi.PropertyType == typeof(double))
					m_pi.SetValue(m_obj, (double)fVal, null);
			}
		}

		private void m_endogine_EnterFrameEvent()
		{
			this.Step();
		}

		private void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.Step();
		}

		public void AddKey(AnimationKey key)
		{
			this._slKeys.Add(key.Time, key);
		}
		public void RemoveKey(AnimationKey key)
		{
			this._slKeys.Remove(key.Time);
		}
		public void RemoveKeyAtTime(float a_fTime)
		{
			this._slKeys.Remove(a_fTime);
		}
		public void RemoveKeyAtIndex(int a_nIndex)
		{
			this._slKeys.RemoveAt(a_nIndex);
		}


		/// <summary>
		/// Get an animation key near the specified time
		/// </summary>
		/// <param name="a_fTime"></param>
		/// <param name="a_nOffset">0=last key (or, if a_fTime coincides with a key, the current key), 1=next key. Other values are also allowed</param>
		/// <param name="a_bIfOutsideReturnExtreme">If the specified key doesn't exist, should it return the nearest</param>
		/// <returns>The requested key</returns>
		public AnimationKey GetKeyNearTime(float a_fTime, int a_nOffset, bool a_bIfOutsideReturnExtreme)
		{
			int nIndex = this._slKeys.IndexOfKey(a_fTime);
			if (nIndex < 0)
			{
				AnimationKey keyTemp = new AnimationKey(0,0);
				this._slKeys.Add(a_fTime, keyTemp);
				nIndex = this._slKeys.IndexOfKey(a_fTime);
				this._slKeys.RemoveAt(nIndex);
				nIndex--;
			}
			nIndex+=a_nOffset;
			if (nIndex < 0)
			{
				if (a_bIfOutsideReturnExtreme)
					nIndex = 0;
				else
					return null;
			}
			if (nIndex >= this._slKeys.Count)
			{
				if (a_bIfOutsideReturnExtreme)
					nIndex = this._slKeys.Count-1;
				else
					return null;
			}
			return this._slKeys.Values[nIndex];
		}

        public void SetAnimationList(List<float> vals)
        {
            SortedList<float, float> sl = new SortedList<float, float>();
            for (int i = 0; i < vals.Count; i++)
                sl.Add(i, vals[i]);
            this.SetAnimationList(sl);
        }

        public void SetAnimationList(SortedList<float, float> vals)
        {
            SortedList<float, AnimationKey> sl = new SortedList<float, AnimationKey>();
            for (int i = 0; i < vals.Count; i++)
                sl.Add(vals.Keys[i], new AnimationKey(vals.Keys[i], vals.Values[i]));
            this.AnimationList = sl;
        }

		public SortedList<float,AnimationKey> AnimationList
		{
            get { return this._slKeys; }
            set
            {
                //TODO: dispose of keys?
                this._slKeys = value;
                if (this.m_fTime > this._slKeys.Keys[this._slKeys.Count - 1])
                    this.m_fTime = 0;
            }
		}
		public float Position
		{
			set{this.m_fTime = value;}
			get{return this.m_fTime;}
		}

//		public void Load(System.Xml.XmlNode node)
//		{
//
//		}
	}
}
