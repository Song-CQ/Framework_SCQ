using System;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for WrappingSprite.
	/// </summary>
	public class WrappingSprite : Endogine.GameHelpers.GameSprite
	{
		private ERectangleF m_rctWrap;
		public WrappingSprite()
		{
		}

		public ERectangleF WrapRect
		{
			set {m_rctWrap = value;}
		}

		public override void EnterFrame()
		{
			base.EnterFrame();
			EPointF loc = this.ConvParentLocToRootLoc(Loc);
			m_rctWrap.WrapPointInside(loc);
			Loc = this.ConvRootLocToParentLoc(loc);
		}
	}
}
