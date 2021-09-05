using System;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for Star.
	/// </summary>
	public class Star : Sprite
	{
		public Star()
		{
		}

		public override void EnterFrame()
		{
			base.EnterFrame();

			ERectangleF rct = new ERectangleF(0,0,640,480);
			EPointF loc = this.ConvParentLocToRootLoc(Loc);
			rct.WrapPointInside(loc);
			Loc = this.ConvRootLocToParentLoc(loc);
		}
	}
}
