using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    /// <summary>
    /// Summary description for ColorLab.
    /// </summary>
    public class ColorLab : ColorBase
    {
        private float _l;

        [Category("Axis"), ColorInfo("L", 100)] //Description("L"), DefaultValue("1"), DisplayName("0")]
        public float L
        {
            get { return _l; }
            set { _l = value; }
        }
        private float _aa;

        [Category("Axis"), ColorInfo("*a", 100)] //Description("a"), DefaultValue("1"), DisplayName("0")]
        public float a
        {
            get { return _aa; }
            set { _aa = value; }
        }

        private float _b;

        [Category("Axis"), ColorInfo("*b", 100)] //Description("b"), DefaultValue("1"), DisplayName("0")]
        public float b
        {
            get { return _b; }
            set { _b = value; }
        }

        public ColorLab()
        { }

        public ColorLab(float l, float a, float b)
        {
            //this._a = 255;
            this._l = l;
            this._aa = a;
            this._b = b;
        }

        public ColorLab(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }


        //		public ColorLab(int a, float x, float y, float z)
        //		{
        //			this._a = a;
        //			this._x = x;
        //			this._y = y;
        //			this._z = z;
        //		}

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                // For the conversion we first convert values to XYZ and then to RGB
                // Standards used Observer = 2, Illuminant = D65

                float ref_X = 0.95047f;
                float ref_Y = 1.00000f;
                float ref_Z = 1.08883f;

                float var_Y = (this._l + 0.16f) / 1.16f;
                float var_X = var_Y + this._aa / 5f;
                float var_Z = var_Y - this._b / 2f;

                float pow = (float)Math.Pow(var_Y, 3);
                if (pow > 0.008856)
                    var_Y = pow;
                else
                    var_Y = (var_Y - 16f / 116) / 7.787f;

                pow = (float)Math.Pow(var_X, 3);
                if (pow > 0.008856)
                    var_X = pow;
                else
                    var_X = (var_X - 16f / 116) / 7.787f;

                pow = (float)Math.Pow(var_Z, 3);
                if (pow > 0.008856)
                    var_Z = pow;
                else
                    var_Z = (var_Z - 16f / 116) / 7.787f;

                float X = ref_X * var_X;
                float Y = ref_Y * var_Y;
                float Z = ref_Z * var_Z;

                ColorXyz xyz = new ColorXyz(X, Y, Z);
                xyz.A = this.A;
                return xyz.RgbFloat;
            }
            set
            {
                ColorXyz xyz = new ColorXyz();
                xyz.RgbFloat = value;

                float ref_X = 0.95047f;
                float ref_Y = 1.00000f;
                float ref_Z = 1.08883f;

                float var_X = xyz.X / ref_X;
                float var_Y = xyz.Y / ref_Y;
                float var_Z = xyz.Z / ref_Z;

                float third = 1.0f / 3;
                if (var_X > 0.008856f) var_X = (float)Math.Pow(var_X, third);
                else var_X = 7.787f * var_X + 16f / 116f;
                if (var_Y > 0.008856f) var_Y = (float)Math.Pow(var_Y, third);
                else var_Y = 7.787f * var_Y + 16f / 116f;
                if (var_Z > 0.008856f) var_Z = (float)Math.Pow(var_Z, third);
                else var_Z = 7.787f * var_Z + 16f / 116f;

                this._l = (1.16f * var_Y) - 0.16f;
                this._aa = 5f * (var_X - var_Y);
                this._b = 2f * (var_Y - var_Z);

                this.A = value.A;
            }
        }

        public override Color ColorRGBA
        {
            get
            {
                // For the conversion we first convert values to XYZ and then to RGB
                // Standards used Observer = 2, Illuminant = D65

                const double ref_X = 95.047;
                const double ref_Y = 100.000;
                const double ref_Z = 108.883;

                double var_Y = ((double)this._l + 16.0) / 116.0;
                double var_X = (double)this._aa / 500.0 + var_Y;
                double var_Z = var_Y - (double)this._b / 200.0;

                if (Math.Pow(var_Y, 3) > 0.008856)
                    var_Y = Math.Pow(var_Y, 3);
                else
                    var_Y = (var_Y - 16 / 116) / 7.787;

                if (Math.Pow(var_X, 3) > 0.008856)
                    var_X = Math.Pow(var_X, 3);
                else
                    var_X = (var_X - 16 / 116) / 7.787;

                if (Math.Pow(var_Z, 3) > 0.008856)
                    var_Z = Math.Pow(var_Z, 3);
                else
                    var_Z = (var_Z - 16 / 116) / 7.787;

                double X = ref_X * var_X;
                double Y = ref_Y * var_Y;
                double Z = ref_Z * var_Z;

                ColorXyz xyz = new ColorXyz((float)X, (float)Y, (float)Z);
                return xyz.ColorRGBA;
            }
            set
            {
                ColorXyz xyz = new ColorXyz();
                xyz.ColorRGBA = value;

                const float ref_X = 95.047f;
                const float ref_Y = 100.000f;
                const float ref_Z = 108.883f;

                float var_X = xyz.X / ref_X;
                float var_Y = xyz.Y / ref_Y;
                float var_Z = xyz.Z / ref_Z;

                float third = 1.0f / 3;
                if (var_X > 0.008856f) var_X = (float)Math.Pow(var_X, third);
                else var_X = (7.787f * var_X) + (16f / 116f);
                if (var_Y > 0.008856f) var_Y = (float)Math.Pow(var_Y, third);
                else var_Y = (7.787f * var_Y) + (16f / 116f);
                if (var_Z > 0.008856f) var_Z = (float)Math.Pow(var_Z, third);
                else var_Z = (7.787f * var_Z) + (16f / 116f);

                this._l = (116f * var_Y) - 16;
                this._aa = 500f * (var_X - var_Y);
                this._b = 200f * (var_Y - var_Z);
            }
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._l, this._aa, this._b, (float)this.A / 255f);
            }
            set
            {
                this._l = value.X;
                this._aa = value.Y;
                this._b = value.Z;
                this.A = (int)(value.W * 255);
            }
        }
        //TODO:

        /*
        on labToRGB aLab
  --http://www.easyrgb.com/math.php
  paWhitePointPresets = [\
2:[\
#A:[109.850, 100.0, 35.585],\
#C:[98.074, 100.0, 118.232],\
#D50:[96.422, 100.0, 82.521],\
#D55:[95.682, 100.0, 92.149],\
#D65:[95.047, 100.0, 108.883],\
#D75:[94.972, 100.0, 122.638],\
  #F2:[99.187, 100.0, 67.395],\
#F7:[95.044, 100.0, 108.755],\
#F11:[100.966, 100.0, 64.370]],\
10:[\
#A:[111.144, 100.0, 35.200],\
#C:[97.285, 100.0, 116.145],\
#D50:[96.720, 100.0, 81.427],\
#D55:[95.799, 100.0, 90.926],\
#D65:[94.811, 100.0, 107.304],\
#D75:[94.416, 100.0, 120.641],\
#F2:[103.280, 100.0, 69.026],\
#F7:[95.792, 100.0, 107.687],\
#F11:[103.866, 100.0, 65.627]\
]]
  
  
  nAngle = 2
  sbObserver = #D65
  aWhitePoint = paWhitePointPresets.getaProp(nAngle)[sbObserver]
  
  aWhitePoint = aWhitePoint/100

  LF = (L+16)/116
  if (L >= 29.0*b/50 + 8) then Z = power(LF - b/200, 3)
  else Z = (108.0/841)*(LF - 4.0/29 - b/200)
    
  if (L >= 8 - 29.0*a/125) then X = power(a/500 + LF, 3)
  else X = (108.0/841)*(a/500 + LF - 4.0/29)
    
  if (L >= 8) then Y = power(LF, 3)
  else Y = 27.0*L/24389
  
  x = x*aWhitePoint[1]
  y = y*aWhitePoint[2]
  z = z*aWhitePoint[3]
  
  R =  3.063219*X -1.393326*Y -0.475801*Z
  G = -0.969245*X +1.875968*Y +0.041555*Z
  B =  0.067872*X -0.228833*Y +1.069251*Z
  
  --          aRGB = [r,g,b]
  --          repeat with n = 1 to 3
  --            aRGB[n] = aRGB[n]*
  --          end repeat
  nFact = 255--*1.3
  clr = rgb(max(R, 0)*nFact, max(G, 0)*nFact, max(B, 0)*nFact)
  
  return clr
  
  --          X = 0.430574*R + 0.341550*G + 0.178325*B
  --          Y = 0.222015*R + 0.706655*G + 0.071330*B
  --          Z = 0.020183*R + 0.129553*G + 0.939180*B
  
end
         */

    }
}
