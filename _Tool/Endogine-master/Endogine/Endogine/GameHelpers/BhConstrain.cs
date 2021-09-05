using System;
using System.Collections;
using Endogine;

namespace Endogine.GameHelpers
{
	/// <summary>
	/// Summary description for BhConstrain.
	/// </summary>
	public class BhConstrain : Behavior
	{
		public delegate void ConstrainDelegate(object sender, ConstrainArea area);
        //public event ConstrainDelegate Enter;
        //public event ConstrainDelegate Leave;
        //public event ConstrainDelegate StillInside;

		public struct ConstrainArea
		{
			public ERectangleF Constraint;
			public ERectangleF Snap;
			public bool ConstrainRectInsteadOfPoint;
			public object Tag;
		}

		private ArrayList m_areas;
		private bool m_bAutoConstrain = true;
		private ConstrainArea m_currentConstrainArea;

		public BhConstrain()
		{
			this.m_areas = new ArrayList();
		}

		public void AddArea(ConstrainArea area)
		{
			this.m_areas.Add(area);
		}
		public void AddArea(ERectangleF rctConstraint, ERectangleF rctSnap, bool bConstrainRectInsteadOfPoint, object tag)
		{
			ConstrainArea area = new ConstrainArea();
			area.Constraint = rctConstraint;
			area.Snap = rctSnap;
			area.ConstrainRectInsteadOfPoint = bConstrainRectInsteadOfPoint;
			area.Tag = tag;
			this.AddArea(area);
		}

		protected override void EnterFrame()
		{
			if (this.m_bAutoConstrain)
				this.Constrain(this.m_sp);

			base.EnterFrame ();
		}


		public bool GetIsConstrained(Sprite sp, ConstrainArea area)
		{
			if (area.ConstrainRectInsteadOfPoint)
			{
				//when the surrounding rect must be inside the rect
				ERectangleF rctIntersect = area.Snap.Copy();
				rctIntersect.Intersect(sp.Rect);
				if (rctIntersect.Equals(sp.Rect))
				{
					//the surrounding rect is totally inside the snap rect

					if (!(sp.Rect.Width > area.Constraint.Width || sp.Rect.Height > area.Constraint.Height))
						return true; //it's small enough to fit inside constraint
				}
			}
			else
			{
				//when the loc must be inside the rect
				if (area.Snap==null)
					return true; //no snap area defined; it's always snapped
				return area.Snap.Contains(sp.Loc);
			}
			return false;
		}

		public void Constrain(Sprite sp)
		{
			//on constrain me, a_oOptionalMouse

			ConstrainArea wasConstrainedTo = this.m_currentConstrainArea;
			//TODO: do I have to convert area to a class instead of a struct?
//			this.m_currentConstrainArea = null;

			for (int nAreaNum = 0; nAreaNum < this.m_areas.Count; nAreaNum++)
			{
				ConstrainArea area = (ConstrainArea)this.m_areas[nAreaNum];
				if (area.Constraint!=null) //if (ilk(m_rctConstraintArea) = #Rect) then
				{
					if (this.GetIsConstrained(sp, area))
					{
						this.m_currentConstrainArea = area;

						EPointF pntMove = new EPointF();
						EPointF pntLoc = sp.Loc;
						ERectangleF rct = sp.Rect;

						if (area.ConstrainRectInsteadOfPoint)
						{
							//when the surrounding rect must be inside the rect
							//move it so it's inside the constraintArea
							if (rct.Left < area.Constraint.Left)
								pntMove.X = area.Constraint.Left-rct.Left;
							else if (rct.Right > area.Constraint.Right)
								pntMove.X = area.Constraint.Right-rct.Right;

							if (rct.Top < area.Constraint.Top)
								pntMove.Y = area.Constraint.Top-rct.Top;
							else if (rct.Bottom < area.Constraint.Bottom)
								pntMove.Y = area.Constraint.Bottom-rct.Bottom;
						}
						else
						{
							//move it so the loc is inside the constraintArea
							if (pntLoc.X < area.Constraint.Left)
								pntMove.X = area.Constraint.Left-pntLoc.X;
							else if (pntLoc.X > area.Constraint.Right)
								pntMove.X = area.Constraint.Right-pntLoc.X;

							if (pntLoc.Y < area.Constraint.Top)
								pntMove.Y = area.Constraint.Top-pntLoc.Y;
							else if (pntLoc.Y > area.Constraint.Bottom)
								pntMove.Y = area.Constraint.Bottom-pntLoc.Y;
						}
						//if (objectP(a_oOptionalMouse)) then a_oOptionalMouse.m_pntCurrentMouseLoc = a_oOptionalMouse.m_pntCurrentMouseLoc+pntMove        
						sp.Loc = pntLoc + pntMove;
						break;
					}
				}
			}

			
//			if (wasConstrainedTo != null && wasConstrainedTo != this.m_currentConstrainArea)
//			{
//				if (this.Leave!=null)
//					this.Leave(this, wasConstrainedTo);
//			}
//			if (this.m_currentConstrainArea != null && wasConstrainedTo != this.m_currentConstrainArea)
//			{
//				if (this.Enter!=null)
//					this.Enter(this, this.m_currentConstrainArea);
//			}
//			else if (this.m_currentConstrainArea != null && wasConstrainedTo == this.m_currentConstrainArea)
//			{
//				if (this.StillInside!=null)
//					this.StillInside(this, this.m_currentConstrainArea);
//			}
		}
	}
}
