using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Collision
{
    public class PointLine
    {

        public static EPointF GetClosestPointOnLine(EPointF pLineStartingAtOrigo, EPointF pCheck)
        {
            EPointF pHit;
            if (pLineStartingAtOrigo.X == 0)
            {
                pHit = new EPointF(pCheck.X, 0);
            }
            else
            {
                if (pLineStartingAtOrigo.Y == 0)
                {
                    pHit = new EPointF(0, pCheck.Y);
                }
                else
                {
                    //equation: y = ax + b
                    //we know a for pDiff line:
                    float a1 = (float)pLineStartingAtOrigo.Y / pLineStartingAtOrigo.X;
                    //and we want a for other line to be perpendicular to a1
                    float a2 = -1f / a1;

                    //b for line 1 is always 0. For line 2, we have to calculate.
                    //We know that it goes through pMouse:
                    //pMouse.Y = a2*pMouse.X + b2
                    float b2 = pCheck.Y - a2 * pCheck.X;

                    //a1x + b1 = a2x + b2  ->  a1x - a2x = b2 - b1  -> x = (b2-b1)/(a1-a2)      (b2 == 0)
                    float x = b2 / (a1 - a2);
                    float y = a1 * x;
                    pHit = new EPointF(x, y);
                }
            }
            //TODO: test if outside line end points
            return pHit;
        }
        public static EPointF GetClosestPointOnLine(EPointF pLine1, EPointF pLine2, EPointF pCheck)
        {
            EPointF pDiff = pLine2 - pLine1;
            pCheck = pCheck - pLine1;
            EPointF p = GetClosestPointOnLine(pDiff, pCheck);
            return p + pLine1;
        }

        public static float PointIsWhereOnLine(EPointF pnt, ERectangleF rct)
        {
            //NormalizeRect(ref rct);
            if (rct.Width != 0)
                return pnt.X / rct.Width;
            if (rct.Height != 0)
                return pnt.Y / rct.Height;
            return -1000;
        }

        public static float GetDistanceFromLine(EPointF pLine1, EPointF pLine2, EPointF pCheck)
        {
            EPointF p = GetClosestPointOnLine(pLine1, pLine2, pCheck);
            EPointF pDiff = pCheck - p;
            return pDiff.Length;// (float)Math.Sqrt(pDiff.X * pDiff.X + pDiff.Y * pDiff.Y);
        }

        public static bool PointInTriangle(EPointF point, EPointF p1, EPointF p2, EPointF p3)
        {
            return (PointsOnSameSideOfLine(point, p1, p2, p3) && PointsOnSameSideOfLine(point, p2, p1, p3) && PointsOnSameSideOfLine(point, p3, p1, p2));
        }
        public static bool PointsOnSameSideOfLine(EPointF p1, EPointF p2, EPointF lineEnd1, EPointF lineEnd2)
        {
            Vector3 v1 = new Vector3(p1.X, p1.Y, 0);
            Vector3 v2 = new Vector3(p2.X, p2.Y, 0);

            Vector3 vLine1 = new Vector3(lineEnd1.X, lineEnd1.Y, 0);
            Vector3 vLine2 = new Vector3(lineEnd2.X, lineEnd2.Y, 0);

            Vector3 cp1 = Vector3.Cross(vLine2 - vLine1, v1 - vLine1);
            Vector3 cp2 = Vector3.Cross(vLine2 - vLine1, v2 - vLine1);

            return (cp1.Dot(cp2) >= 0);
        }
        
    }
}
