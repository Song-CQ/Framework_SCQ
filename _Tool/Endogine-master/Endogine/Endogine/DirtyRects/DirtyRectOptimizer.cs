using System;
using System.Collections;
using System.Drawing;

namespace Endogine.DirtyRects
{
	/// <summary>
	/// Summary description for DirtyRectOptimizer.
	/// </summary>
	public class DirtyRectOptimizer
	{
		//private m_plCallbackInfo, m_bMakeCallback
		public DirtyRectOptimizer()
		{
		}

		public virtual void ReduceRects(ref ArrayList a_aRects)
		{}

		public Rectangle RectsUnion(ArrayList a_aRects)
		{
			Rectangle rctUnion = new Rectangle(1000000,1000000,-1000000,-1000000);
			foreach (Rectangle rctX in a_aRects)
			{
				//TODO: create my own rectangle class with new expandToInclude!
				ExpandRectToInclude(ref rctUnion, rctX);
			}
			return rctUnion;
		}

		protected void ExpandRectToInclude(ref Rectangle rctToExpand, Rectangle a_rctExpandToThis)
		{
			if (a_rctExpandToThis.X < rctToExpand.X)
			{
				rctToExpand.Width+= rctToExpand.X-a_rctExpandToThis.X;
				rctToExpand.X = a_rctExpandToThis.X;
			}
			if (a_rctExpandToThis.Y < rctToExpand.Y)
			{
				rctToExpand.Height+= rctToExpand.Y-a_rctExpandToThis.Y;
				rctToExpand.Y = a_rctExpandToThis.Y;
			}
			if (a_rctExpandToThis.Right > rctToExpand.Right)
				rctToExpand.Width+= rctToExpand.Right-a_rctExpandToThis.Right;
			if (a_rctExpandToThis.Bottom > rctToExpand.Bottom)
				rctToExpand.Height+= rctToExpand.Bottom-a_rctExpandToThis.Bottom;
		}

	}
}
