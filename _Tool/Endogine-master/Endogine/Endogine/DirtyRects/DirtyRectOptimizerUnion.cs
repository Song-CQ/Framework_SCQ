using System;
using System.Drawing;
using System.Collections;

namespace Endogine.DirtyRects
{
	/// <summary>
	/// Summary description for DirtyRectOptimizer.
	/// </summary>
	public class DirtyRectOptimizerUnion : DirtyRectOptimizer
	{

		public DirtyRectOptimizerUnion()
		{
		}
		
		public override void ReduceRects(ref ArrayList a_aRects)
		{
			Rectangle rctUnion = RectsUnion(a_aRects);
			a_aRects.Clear();
			a_aRects.Add(rctUnion);
		}
	}
}
