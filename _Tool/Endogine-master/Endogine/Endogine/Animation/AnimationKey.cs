using System;

namespace Endogine.Animation
{
	/// <summary>
	/// Summary description for AnimationKey.
	/// </summary>
	public class AnimationKey
	{
		private EPointF m_pntKey;

		//TODO: how to shape time/value curve
		private EPointF m_pntAnchorBefore;
		private EPointF m_pntAnchorAfter;

		public AnimationKey(float fTime, float fValue)
		{
			m_pntKey = new EPointF(fTime, fValue);
			m_pntAnchorBefore = new EPointF();
			m_pntAnchorAfter = new EPointF();
		}

		public float Time
		{
			get {return m_pntKey.X;}
			set {m_pntKey.X = value;}
		}
		public float Value
		{
			get {return m_pntKey.Y;}
			set {m_pntKey.Y = value;}
		}

		public EPointF AnchorBefore
		{
			get {return m_pntAnchorBefore;}
			set {m_pntAnchorBefore = value;}
		}
		public EPointF AnchorAfter
		{
			get {return m_pntAnchorAfter;}
			set {m_pntAnchorAfter = value;}
		}
	}
}
