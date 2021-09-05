using System;
using System.Drawing;
using System.Collections;
using Endogine;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for PathCalc.
	/// </summary>
	public class PathCalc
	{
		private PlayArea m_playArea;

		public PathCalc(PlayArea a_playArea)
		{
			m_playArea = a_playArea;
		}

		public void DrawPath(ArrayList aLocs)
		{
			int n = Math.Min(10,aLocs.Count);

			EPointF gfxLoc = null;
			for (int i = 0; i < n; i++)
			{
				EPointF pnt = (EPointF)aLocs[i];
				gfxLoc = m_playArea.Grid.GetGfxLocFromGridLoc(pnt);
				//Sprite sp = (Sprite)aSprites[i];
				//sp.Loc = gfxLoc;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a_loc">start loc</param>
		/// <param name="a_vel">find first collision/bounce while moving this amount</param>
		/// <param name="pntGridStick"></param>
		/// <returns>true if it hit something to stick to (another ball, or ceiling)</returns>
		public bool GetFirstStickOrBounce(ref EPointF a_loc, ref EPointF a_vel,
			out EPoint pntGridStick, out EPointF pntBounce, bool bMoveBallAfterCollision)
		{
			EPointF pntCollision = new EPointF(0,0);
			EPointF pntCircleAtCollision;
			EPointF pntNormal = new EPointF(0,0);
			bool bCollided = false;

			pntGridStick = null; //new EPoint(0,0);
			pntBounce = null; //new EPointF(0,0);

			float fFirstTime = 1000;
			ArrayList aBalls = m_playArea.Grid.GetAllBalls();
			foreach (Ball ball in aBalls)
			{
				float fTime;
				EPointF pntThisCollision;

				if (Endogine.Collision.Collision.CalcFirstCircleCircleCollisions(
					a_loc, ball.Loc,
					a_vel, new EPointF(0,0), 
					m_playArea.Grid.BallDiameter/2-1, m_playArea.Grid.BallDiameter/2-1, //it's more like the original if we subtract 1
					out pntThisCollision, out fTime))
				{
					if (fTime < fFirstTime)
					{
						pntCollision = pntThisCollision;
						fFirstTime = fTime;
					}
				}
			}
			if (fFirstTime < 1000)
			{
				EPointF pntfGrid = m_playArea.Grid.GetGridLocFromGfxLoc(pntCollision);
				pntGridStick = m_playArea.Grid.RoundToClosestGridLoc(pntfGrid);
				a_loc = m_playArea.Grid.GetGfxLocFromGridLoc(pntGridStick);
				a_vel = new EPointF(0,0);
				return true;
			}

			//check wall bounces and ceiling stick
			for (int nLineNum = 0; nLineNum < m_playArea.m_aCollisionLines.Count; nLineNum++)
			{
				ERectangleF rctLine = (ERectangleF)m_playArea.m_aCollisionLines[nLineNum];
				if (Endogine.Collision.Collision.CalcCircleLineCollision(a_loc, m_playArea.Grid.BallDiameter/2, a_vel, rctLine, out pntCollision, out pntCircleAtCollision))
				{
					if (nLineNum == 0)
					{
						//hit ceiling: make it stick!
						EPointF pntfGrid = m_playArea.Grid.GetGridLocFromGfxLoc(pntCollision);
						EndogineHub.Put(pntCollision.ToString() + "  " + pntfGrid.ToString());
						pntGridStick = m_playArea.Grid.RoundToClosestGridLoc(pntfGrid);
						a_loc = m_playArea.Grid.GetGfxLocFromGridLoc(pntGridStick);
						a_vel.X = 0;
						a_vel.Y = 0;
						return true;
					}

					//TODO: check all lines, and use the one that is collided with first.
					//Then a new test should be done from the bounce point

					pntBounce = pntCollision;
					//bounce against a wall: which direction will it have afterwards
					double dNormal = Endogine.Collision.Collision.GetNormalAngle(rctLine);
					//the line's direction, it shouldn't matter if it's up or down, or left or right - same bounce regardless
					if (dNormal >= Math.PI)
						dNormal-=Math.PI;
					if (dNormal >= Math.PI/2)
						dNormal-=Math.PI/2;

					double dVelAngle = Math.Atan2(a_vel.X, -a_vel.Y);

					double dAfterBounceAngle = dNormal-(dVelAngle-dNormal);
					//just so it's easier to debug:
					if (dAfterBounceAngle > Math.PI)
						dAfterBounceAngle-=Math.PI;

					//how far can it move before hitting wall
					EPointF pntBefore = new EPointF(pntCircleAtCollision.X-a_loc.X, pntCircleAtCollision.Y-a_loc.Y);
					double dPartOfMove = Endogine.Collision.PointLine.PointIsWhereOnLine(pntBefore, new ERectangleF(0,0,a_vel.X,a_vel.Y));
					if (dPartOfMove > 0.99)
						dPartOfMove = 0.99;
					else if (dPartOfMove < 0 && dPartOfMove > -0.001)
						dPartOfMove = 0;

					//the rest of the movement will be in bounce direction
					a_vel = EPointF.FromLengthAndAngle(a_vel.Length, (float)dAfterBounceAngle);
					//EPointF velNew = EPointF.FromLengthAndAngle(a_vel.Length, (float)dAfterBounceAngle);
					//a_vel.X = velNew.X;
					//a_vel.Y = velNew.Y;

					//TODO: pntCircleAtCollision is not correctly calculated.
					a_loc = pntCircleAtCollision;

					if (bMoveBallAfterCollision)
					{
						a_loc.X+= a_vel.X*(float)(1.0-dPartOfMove);
						a_loc.Y+= a_vel.Y*(float)(1.0-dPartOfMove);
					}

					bCollided = true;
					break;
				}
			}
			if (!bCollided)
			{
				a_loc.X+=a_vel.X;
				a_loc.Y+=a_vel.Y;
			}
			return false;
		}

	}
}
