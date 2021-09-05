using System;
using System.Drawing;
using System.Collections;

namespace Endogine.DirtyRects
{
	/// <summary>
	/// Summary description for DirtyRectOptimizer.
	/// </summary>
	public class DirtyRectOptimizerMacGuru : DirtyRectOptimizer
	{
		private int m_nMaxNumRects = 10;

		public DirtyRectOptimizerMacGuru()
		{
		}
		
		public override void ReduceRects(ref ArrayList a_aRects)
		{

			//TODO: if more than N rects, sort them by area (or locX?)

			//TODO: check the area affected - if it's small, just do a join!

			int nCnt = a_aRects.Count;
			if (nCnt > 100) //more than 100 tests will take too much time - just do a join!
			{
				Rectangle rctUnion = RectsUnion(a_aRects);
				a_aRects.Clear();
				a_aRects.Add(rctUnion);
				return;
			}
  
			Rectangle rct = new Rectangle(0,0,0,0);
			bool bGotRect = false;
			int nCheckThisPos = nCnt-1;
			for(;;)
			{
				if (bGotRect == false)
				{
					rct = (Rectangle)a_aRects[nCheckThisPos];
					bGotRect = true;
					a_aRects.RemoveAt(nCheckThisPos);
				}

				//if (m_bMakeCallback) then makeCallback(m_plCallbackInfo, me, #Draw, [#AllRects:a_aRects, #CheckThis:rct])

				int nJoinedRectAtPos = ReduceRectsSub(ref a_aRects, rct);

				//    if (m_bMakeCallback) then makeCallback(m_plCallbackInfo, me, #Draw, [#AllRects:a_aRects])

				//Added step: if there's been a join, the current dirtyRect list must be rechecked
				//because maybe the new rect should join with another
				if (nJoinedRectAtPos >= 0)
				{
					if (a_aRects.Count == 1)
						return;
					rct = (Rectangle)a_aRects[nJoinedRectAtPos];
					a_aRects.RemoveAt(nJoinedRectAtPos);
					nCheckThisPos = a_aRects.Count;
				}
				else
				{
					nCheckThisPos--;
					bGotRect = false;
					if (nCheckThisPos <= 0)
						return;
				}
			}
		}

		private int ReduceRectsSub(ref ArrayList a_aRects, Rectangle a_rctNew)
		{
			int nJoinedRectAtPos = -1;
			for (int i = a_aRects.Count-1; i >= 0; i--)
			{
				Rectangle rctOld = (Rectangle)a_aRects[i];
				//    if (m_bMakeCallback) then makeCallback(m_plCallbackInfo, me, #Draw, [#AllRects:a_aRects, #CheckThis:a_rctNew, #CheckAgainst:rctOld])

				//1. Check if the new rect touches any rects in the dirtyrect list
				//Rectangle rctIntersect = rctOld;
				//rctIntersect.Intersect(a_rctNew);

				if (rctOld.IntersectsWith(a_rctNew))
				{
					//2. If it does, join them and replace the rect in the dirtyrect list
					ExpandRectToInclude(ref rctOld, a_rctNew);
					nJoinedRectAtPos = i;
					break;
				}
			}
			if (nJoinedRectAtPos == -1)
			{
				//no join performed above
				if (a_aRects.Count < m_nMaxNumRects)
				{
					//3. If not, and there is room in the dirtyrect list add it to the end of the list
					a_aRects.Add(a_rctNew);
				}
				else
				{
					//4. If there is no room (dirtyrects = maxrects) then loop through the dirtyrect list,
					//to find the two rects that when joined use up the least amount of area.
					int nMinArea = 100000000;
					Rectangle rctMin = new Rectangle(0,0,0,0);
					nJoinedRectAtPos = -1;

					Rectangle rctJoined = new Rectangle(0,0,0,0);
					for (int i = a_aRects.Count; i >= 0; i--)
					{
						rctJoined = (Rectangle)a_aRects[i];
						ExpandRectToInclude(ref rctJoined, a_rctNew);
						//        if (m_bMakeCallback) then makeCallback(m_plCallbackInfo, me, #Draw, [#AllRects:a_aRects, #CheckThis:a_rctNew, #CheckAgainst:a_aRects.getAt(m), #Union:rctJoined])
						int nArea = rctJoined.Width*rctJoined.Height;
						if (nArea < nMinArea)
						{
							nMinArea = nArea;
							nJoinedRectAtPos = i;
							rctMin = rctJoined;
						}
					}
					//Delete those two rects and replaced with a single union rect.
					a_aRects[nJoinedRectAtPos] = rctMin;
				}
			}
			return nJoinedRectAtPos;
		}

	}
}
