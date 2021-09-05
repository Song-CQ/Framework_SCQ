using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine
{
    public class Quad
    {
        EPointF[] _points;

        public Quad(ERectangleF rct)
        {
            this._points = new EPointF[4];
            this._points[0] = rct.TopLeft;
            this._points[1] = new EPointF(rct.Right, rct.Top);
            this._points[2] = rct.BottomRight;
            this._points[3] = new EPointF(rct.Left, rct.Bottom);
        }

        public Quad(EPointF[] points)
        {
            this._points = points;
        }

        public Quad(EPointF p1, EPointF p2, EPointF p3, EPointF p4)
        {
            this._points = new EPointF[4];
            this._points[0] = p1;
            this._points[1] = p2;
            this._points[2] = p3;
            this._points[3] = p4;
        }

        public ERectangleF GetBoundingRect()
        {
            return null;
        }

        /// <summary>
        /// Transforms a point in a rectangle to where it would be in the quad
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public EPointF MapFromRect(EPointF pnt, ERectangleF rect)
        {
            EPointF fract = pnt / rect.Size;
            EPointF pntTopOfQuadAtX = (this._points[1] - this._points[0])*fract.X + this._points[0];
            EPointF pntBottomOfQuadAtX = (this._points[2] - this._points[3]) * fract.X + this._points[3];
            EPointF inQuad = (pntBottomOfQuadAtX - pntTopOfQuadAtX) * fract.Y + pntTopOfQuadAtX;
            return inQuad;
            //pntFract = point(float(a_pnt[1])/a_rct.width, float(a_pnt[2])/a_rct.height)
            //pntTopOfQuadAtX = (a_aQuad[2]-a_aQuad[1])*pntFract[1]+a_aQuad[1]
            //pntBottomOfQuadAtX = (a_aQuad[3]-a_aQuad[4])*pntFract[1]+a_aQuad[4]
            //return (pntBottomOfQuadAtX-pntTopOfQuadAtX)*pntFract[2] + pntTopOfQuadAtX
        }

        /// <summary>
        /// Transforms a point in the quad to where it would be if the quad was stretched out to a rectangle
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public EPointF MapToRect(EPointF pnt, ERectangleF rect)
        {
            //--Courtesy of NoiseCrime

            float x = pnt.X;
            float y = pnt.Y;

            float af = this._points[0].X - this._points[1].X - this._points[3].X + this._points[2].X;
            float bf = this._points[1].X - this._points[0].X;
            float cf = this._points[3].X - this._points[0].X;
            float df = this._points[0].X;

            float ef = this._points[0].Y - this._points[1].Y - this._points[3].Y + this._points[2].Y;
            float ff = this._points[1].Y - this._points[0].Y;
            float gf = this._points[3].Y - this._points[0].Y;
            float hf = this._points[0].Y;

            float A = af * ff - bf * ef;
            float B = ef * x - af * y + af * hf - df * ef + cf * ff - bf * gf;
            float C = gf * x - cf * y + cf * hf - df * gf;
            float D = af * gf - cf * ef;

            float u, v;
            float tmpBResult = 0;
            if (Math.Abs(A) > 0.00001f)
            {
                tmpBResult = (B * B) - (4f * A * C);

                if (tmpBResult < 0)
                    return null;

                tmpBResult = (float)Math.Sqrt(tmpBResult);

                u = (-B - tmpBResult) / (2f * A);

                float u2 = 0;
                //  If our u is outside our range.
                if ((u < 0) || (u > 1))
                    u2 = (-B + tmpBResult) / (2f * A);

                // If u2 is in the correct range, use u2
                if ((u2 >= 0) && (u2 <= 1))
                    u = u2;
                else //pick whichever is closest to range (0...1)
                    if (Math.Abs(u2 - 0.5f) < Math.Abs(u - 0.5f))
                        u = u2;
            }
            else
            {
                if ((int)Math.Round(B) != 0)
                    u = -C / B;
                else
                    u = 0; //return null; //!!@ "Error Quad mapping U = 0")
            }

            if (Math.Abs(D) > 0.00001f)
            {
                float temp = af * u + cf;
                v = -1f;

                if ((temp > 0.00001f) || (temp < -0.00001f))
                    v = (x - bf * u - df) / temp;
                else
                    v = 0; //return null; //    v = 0 -- !!@ "Error Quad mapping V = 0

                if ((v < 0) || (v > 1))
                {
                    float E = ef * x - af * y + af * hf - df * ef - cf * ff + bf * gf;
                    float F = ff * x - bf * y + bf * hf - df * ff;

                    float tmpEResult = E * E - 4f * D * F;
                    if (tmpBResult < 0) //TODO: he meant tmpEResult, no?
                        return null;

                    tmpBResult = (float)Math.Sqrt(tmpBResult); //TODO: he meant tmpEResult, no?

                    // If our result is going to be outside our rect, use the alternative value for the quadratic equation.
                    v = (-E + tmpEResult) / (2f * D);

                    if ((v < 0) || (v > 1))
                    {
                        float v2 = (-E - tmpEResult) / (2f * D);
                        // If v2 is in the correct range, use u2
                        if ((v2 >= 0) && (v2 <= 1))
                            v = v2;
                        else // pick whichever is closest to range (0...1)
                            if (Math.Abs(v2 - 0.5f) < Math.Abs(v - 0.5f))
                                v = v2;
                    }
                }
            }
            else
            {
                float E = ef * x - af * y + af * hf - df * ef - cf * ff + bf * gf;
                float F = ff * x - bf * y + bf * hf - df * ff;

                if ((int)Math.Round(E) != 0)
                    v = -F / E;
                else
                    return null; //Error Quad mapping VE= 0")
            }

            //--JB 041001:
            float pToh = u * rect.Width + rect.X; //-rect.X + prect[1] + 0.0) --+ 0.5
            float pTov = v * rect.Height + rect.Y; //-prect[2] + prect[2] + 0.0) --+ 0.5

            return new EPointF(pToh, pTov);
        }
    }
}
