using System;
using System.Drawing;
using System.Collections;

namespace Endogine.Collision
{
	/// <summary>
	/// Summary description for Collision.
	/// </summary>
	public class Collision
	{
		public Collision()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static bool CalcFirstCircleCircleCollisions(EPointF loc1, EPointF loc2, EPointF vel1, EPointF vel2, float r1, float r2, out EPointF collLoc, out float fTime)
		{
			collLoc = new EPointF(0,0);
			fTime = -1;
			ArrayList aLocs;
			ArrayList aTimes;
			if (CalcCircleCircleCollisions(loc1, loc2, vel1, vel2, r1, r2, out aLocs, out aTimes))
			{
				fTime = (float)aTimes[0];
				collLoc = (EPointF)aLocs[0];
				if (fTime > 1)
					return false;
				if (fTime < 0)
				{
					fTime = (float)aTimes[1];
					collLoc = (EPointF)aLocs[1];
					if (fTime < 0 || fTime > 1)
						return false;
				}
//				else
//				{
//					CalcCircleCircleCollisions(loc1, loc2, vel1, vel2, r1, r2, out aLocs, out aTimes);
//				}
				CalcCircleCircleCollisions(loc1, loc2, vel1, vel2, r1, r2, out aLocs, out aTimes);
				return true;
			}
			return false;
		}
		
		public static bool CalcCircleCircleCollisions(EPointF loc1, EPointF loc2, EPointF vel1, EPointF vel2, float r1, float r2, out ArrayList aLocs, out ArrayList aTimes)
		{
			aLocs = new ArrayList();
			aTimes = new ArrayList();

			EPointF loc = loc2-loc1; //new EPointF(loc2.X-loc1.X, loc2.Y-loc1.Y);
			EPointF vel = vel2-vel1; //new EPointF(vel2.X-vel1.X, vel2.Y-vel1.Y);

			if (vel.X == 0 && vel.Y == 0)
				return false;
  
			//(velx*velx + vely*vely)X^2 + 2*(locx*velx + locy*vely)*X + locx*locx + locy*locy - (r1 + r2)(r1 + r2) = 0
  
			float r = r1 + r2;
  
			EPointF locPwr2 = new EPointF(loc.X*loc.X, loc.Y*loc.Y);
			EPointF velPwr2 = new EPointF(vel.X*vel.X, vel.Y*vel.Y);

			float a = 2f*(loc.X*vel.X + loc.Y*vel.Y);
			float b = locPwr2.X + locPwr2.Y - r*r;

			float divide = velPwr2.X + velPwr2.Y;

			a/=divide;
			b/=divide;

			float roots = a*a/4 - b;
			if (roots < 0)
				return false;

			roots = (float)Math.Sqrt(roots);
			float mid = -a/2;

			EPointF pntHit;
			float fTime;

			fTime = mid-roots;
			for (int n = 0; n < 2; n++)
			{
				pntHit = loc1+vel1*fTime; // -??
				aLocs.Add(pntHit);
				aTimes.Add(fTime);

				fTime = mid+roots;
			}
			return true;
		}

		public static bool CalcCircleLineCollision(EPointF circleLoc, float radius, EPointF circleMovement, ERectangleF lineRect, out EPointF pntFirstCollision, out EPointF circleLocAtCollision)
		{
			//first calc two lines parallel with line, at radius distance from it
			double dAngleLine = Math.Atan2((double)lineRect.Width, -(double)lineRect.Height);
			double dAnglePerp = dAngleLine+Math.PI/2;
			EPointF pntOffset = new EPointF(radius*(float)Math.Sin(dAnglePerp), -radius*(float)Math.Cos(dAnglePerp));
			ERectangleF lineRect1 = lineRect;
			ERectangleF circleMovementLine = new ERectangleF(circleLoc, new EPointF(circleMovement.X, circleMovement.Y));

			float fTimeFirstCollision = 1000;
			pntFirstCollision = new EPointF(0,0);
			circleLocAtCollision = new EPointF(0,0);
			for (int i = -1; i < 2; i+=2)
			{
				ERectangleF lineRectParallel = lineRect.Copy();
				lineRectParallel.Offset(pntOffset.X*i, pntOffset.Y*i);
				EPointF pntCollision;
				if (CalcLineLineCollision(circleMovementLine, lineRectParallel, out pntCollision))
				{
					EPointF pntDiff = new EPointF(pntCollision.X-circleMovementLine.X, pntCollision.Y-circleMovementLine.Y); //circleMovementLine.Left, pntCollision.Y-circleMovementLine.Top
					float fTimeCollision = Endogine.Collision.PointLine.PointIsWhereOnLine(pntDiff, circleMovementLine);
					if (fTimeCollision < fTimeFirstCollision)
					{
						fTimeFirstCollision = fTimeCollision;
						pntFirstCollision = new EPointF(pntCollision.X-pntOffset.X*i, pntCollision.Y-pntOffset.Y*i);
						circleLocAtCollision = pntCollision;
					}
				}
			}
			//also calc circle/circle collision (for line end points)
			ArrayList aLocs, aTimes;
			EPointF pntLinePointToTest = lineRect.Location;
			for (int i = 0; i < 2; i++)
			{
				if (CalcCircleCircleCollisions(circleLoc, pntLinePointToTest, circleMovement, new EPointF(0,0), radius, 0, out aLocs, out aTimes))
				{
					EPointF pntCollision = (EPointF)aLocs[0];
					float fTimeCollision = (float)aTimes[0];
					if (fTimeCollision < 0)
					{
						fTimeCollision = (float)aTimes[1];
						if (fTimeCollision < 0 || fTimeCollision > 1)
							continue;
						pntCollision = (EPointF)aLocs[1];
					}
					if (fTimeCollision < fTimeFirstCollision)
					{
						fTimeFirstCollision = fTimeCollision;
						pntFirstCollision = pntLinePointToTest;
						circleLocAtCollision = new EPointF(circleLoc.X+circleMovement.X*fTimeCollision, circleLoc.Y+circleMovement.Y*fTimeCollision);
					}
				}
				pntLinePointToTest = new EPointF(lineRect.X+lineRect.Width, lineRect.Y+lineRect.Height); //lineRect.Left+lineRect.Width, lineRect.Top
			}
			if (fTimeFirstCollision <= 1)
				return true;
			return false;
		}

		public static float CalcMovingLineLineCollision(ERectangleF lineRect1, ERectangleF lineRect2, EPointF vel1, EPointF vel2, out EPointF pntCollision)
		{
			//TODO: will work like this:
			//Check line/line collision from each of the line's endpoints to endpoints+vel against the other line.
			//Do this for both lines, as a collision maybe missed or misinterpreted
			pntCollision = new EPointF();
			return 0f;
		}
		public static bool CalcLineLineCollision(ERectangleF lineRect1, ERectangleF lineRect2, out EPointF pntCollision)
		{
			pntCollision = new EPointF(0,0);

			bool bLine1GotA = true;
			bool bLine2GotA = true;

			float line1A = 0;
			float line2A = 0;
			float line1B = 0;
			float line2B = 0;

			if (lineRect1.Width != 0)
			{
				line1A = lineRect1.Height/lineRect1.Width;
				line1B = lineRect1.Y-lineRect1.X*line1A; //lineRect1.Top-lineRect1.Left
			}
			else
				bLine1GotA = false;

			if (lineRect2.Width != 0)
			{
				line2A = lineRect2.Height/lineRect2.Width;
				line2B = lineRect2.Y-lineRect2.X*line2A; //lineRect2.Top-lineRect2.Left
			}
			else
				bLine2GotA = false;

			if (line1A==line2A)
			{
				if (bLine1GotA && bLine2GotA)
					return false;
			}

			if (line1B == line2B)
			{
				pntCollision.X = 0;
				pntCollision.Y = line1B;
			}
			if (bLine1GotA && bLine2GotA)
			{
				pntCollision.X = (line2B-line1B)/(line1A-line2A);
				pntCollision.Y = line1A*pntCollision.X + line1B;
			}
			else if (bLine1GotA)
			{
				pntCollision.X = lineRect2.X; //.Left;
				pntCollision.Y = line1A*pntCollision.X + line1B;//(pntCollision.Y-line1B)/line1A;
			}
			else if (bLine2GotA)
			{
				pntCollision.X = lineRect1.X; //.Left;
				pntCollision.Y = line2A*pntCollision.X + line2B;
				//pntCollision.Y = lineRect1.Left;
				//pntCollision.X = (pntCollision.Y-line2B)/line2A;
			}

			//is point on both lines' segments?
			float fRoundError = 0.001f;
			NormalizeRect(ref lineRect1);
			NormalizeRect(ref lineRect2);
			if (pntCollision.X - lineRect1.Left < -fRoundError || pntCollision.X - lineRect2.Left < -fRoundError)
				return false;
			if (pntCollision.X - lineRect1.Right > fRoundError || pntCollision.X - lineRect2.Right > fRoundError)
				return false;
			if (pntCollision.Y - lineRect1.Top < -fRoundError|| pntCollision.Y - lineRect2.Top < -fRoundError)
				return false;
			if (pntCollision.Y - lineRect1.Bottom > fRoundError|| pntCollision.Y - lineRect2.Bottom > fRoundError)
				return false;

			return true;
		}

		public static bool CalcLineLineCollision(EPointF a_ptA, EPointF a_ptB, EPointF a_ptC, EPointF a_ptD)
		{
			float f =((a_ptB.X-a_ptA.X)*(a_ptD.Y-a_ptC.Y)- 
				(a_ptB.Y-a_ptA.Y)*(a_ptD.X-a_ptC.X));

			if (f == 0)
				return false;

			float d=((a_ptA.Y-a_ptC.Y)*(a_ptD.X-a_ptC.X)-
				(a_ptA.X-a_ptC.X)*(a_ptD.Y-a_ptC.Y));

			if (f>0)
			{
				if (d<0 || d>f)
					return false;
			}
			else
			{
				if(d>0 || d<f)
					return false;
			}

			float e=((a_ptA.Y-a_ptC.Y)*(a_ptB.X-a_ptA.X)-
				(a_ptA.X-a_ptC.X)*(a_ptB.Y-a_ptA.Y));

			if(f>0)
			{
				if(e<0 || e>f)
					return false;
			}
			else
			{
				if(e>0 || e<f)
					return false;
			}

			return true;
		}


		public static double GetNormalAngle(ERectangleF rct)
		{
			double dAngleLine = Math.Atan2((double)rct.Width, -(double)rct.Height);
			dAngleLine+=Math.PI/2;
			if (dAngleLine > Math.PI)
				dAngleLine-=Math.PI;
			return dAngleLine;
		}

		private static void NormalizeRect(ref ERectangleF rct)
		{
			if (rct.Width < 0)
			{
				rct.Offset(rct.Width,0);
				rct.Width = -rct.Width;
			}
			if (rct.Height < 0)
			{
				rct.Offset(0,rct.Height);
				rct.Height = -rct.Height;
			}
		}

		public static float GetOrientationAngle(float fAngle)
		{
			if (fAngle < 0)
				fAngle+=(float)Math.PI*2;
			if (fAngle > Math.PI*2)
				fAngle-=(float)Math.PI*2;
			if (fAngle >= Math.PI)
				fAngle-=(float)Math.PI;
			return fAngle;
		}

		public static double GetReflectedAngle(double a_dAngleRay, double a_dAngleMirror)
		{
			double dMirrorNormal = a_dAngleMirror + Math.PI/2;

			//the line's direction, it shouldn't matter if it's up or down, or left or right - same bounce regardless
			if (dMirrorNormal >= Math.PI)
				dMirrorNormal-=Math.PI;
//			if (dMirrorNormal >= Math.PI/2)
//				dMirrorNormal-=Math.PI/2;

			//double dVelAngle = Math.Atan2(a_vel.X, -a_vel.Y);

			double dAfterBounceAngle = dMirrorNormal-(a_dAngleRay-dMirrorNormal);
			//just so it's easier to debug:
			if (dAfterBounceAngle > Math.PI*2)
				dAfterBounceAngle-=Math.PI*2;
			return dAfterBounceAngle;
		}
	}
}
