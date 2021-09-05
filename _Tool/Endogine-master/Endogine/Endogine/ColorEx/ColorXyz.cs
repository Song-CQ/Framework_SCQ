using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    /// <summary>
    /// Summary description for ColorXyz.
    /// </summary>
    public class ColorXyz : ColorBase
    {
        int _a;

        private float _x;

        [Category("Axis"), ColorInfo("X", 100)] //Description("X"), DefaultValue("1"), DisplayName("0")]
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        private float _y;

        [Category("Axis"), ColorInfo("Y", 100)] //Description("Y"), DefaultValue("1"), DisplayName("0")]
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }
        private float _z;

        [Category("Axis"), ColorInfo("Z", 100)] //Description("Z"), DefaultValue("1"), DisplayName("0")]
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public ColorXyz(float x, float y, float z)
        {
            this._a = 255;
            this._x = x;
            this._y = y;
            this._z = z;
        }
        public ColorXyz(int a, float x, float y, float z)
        {
            this._a = a;
            this._x = x;
            this._y = y;
            this._z = z;
        }

        public ColorXyz()
        { }

        public ColorXyz(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                // Standards used Observer = 2, Illuminant = D65
                // ref_X = 95.047, ref_Y = 100.000, ref_Z = 108.883

                float var_X = this._x; // / 100.0;
                float var_Y = this._y; // / 100.0;
                float var_Z = this._z; // / 100.0;

                float r = var_X * 3.2406f + var_Y * -1.5372f + var_Z * -0.4986f;
                float g = var_X * -0.9689f + var_Y * 1.8758f + var_Z * 0.0415f;
                float b = var_X * 0.0557f + var_Y * -0.2040f + var_Z * 1.0570f;

                if (r > 0.0031308f)
                    r = 1.055f * (float)(Math.Pow(r, 1.0 / 2.4)) - 0.055f;
                else
                    r = 12.92f * r;

                if (g > 0.0031308f)
                    g = 1.055f * (float)(Math.Pow(g, 1.0 / 2.4)) - 0.055f;
                else
                    g = 12.92f * g;

                if (b > 0.0031308f)
                    b = 1.055f * (float)(Math.Pow(b, 1.0 / 2.4)) - 0.055f;
                else
                    b = 12.92f * b;

                r = Math.Min(Math.Max(r, 0f), 1f);
                g = Math.Min(Math.Max(g, 0f), 1f);
                b = Math.Min(Math.Max(b, 0f), 1f);

                return new ColorRgbFloat(255, r, g, b); //TODO: why doesn't this.A work??
            }
            set
            {
                float var_R = value.R;
                float var_G = value.G;
                float var_B = value.B;

                if (var_R > 0.04045f) var_R = (float)Math.Pow(((var_R + 0.055f) / 1.055f), 2.4);
                else var_R = var_R / 12.92f;

                //var_R = var_R * 100;
                //var_G = var_G * 100;
                //var_B = var_B * 100;

                //Observer. = 2°, Illuminant = D65
                this._x = var_R * 0.4124f + var_G * 0.3576f + var_B * 0.1805f;
                this._y = var_R * 0.2126f + var_G * 0.7152f + var_B * 0.0722f;
                this._z = var_R * 0.0193f + var_G * 0.1192f + var_B * 0.9505f;

                this.A = value.A;
            }
        }

        public override Color ColorRGBA
        {
            get
            {
                return this.RgbFloat.ColorRGBA;
                //// Standards used Observer = 2, Illuminant = D65
                //// ref_X = 95.047, ref_Y = 100.000, ref_Z = 108.883

                //double var_X = (double)this._x; // / 100.0;
                //double var_Y = (double)this._y; // / 100.0;
                //double var_Z = (double)this._z; // / 100.0;

                //double var_R = var_X * 3.2406 + var_Y * (-1.5372) + var_Z * (-0.4986);
                //double var_G = var_X * (-0.9689) + var_Y * 1.8758 + var_Z * 0.0415;
                //double var_B = var_X * 0.0557 + var_Y * (-0.2040) + var_Z * 1.0570;

                //if (var_R > 0.0031308)
                //    var_R = 1.055 * (Math.Pow(var_R, 1 / 2.4)) - 0.055;
                //else
                //    var_R = 12.92 * var_R;

                //if (var_G > 0.0031308)
                //    var_G = 1.055 * (Math.Pow(var_G, 1 / 2.4)) - 0.055;
                //else
                //    var_G = 12.92 * var_G;

                //if (var_B > 0.0031308)
                //    var_B = 1.055 * (Math.Pow(var_B, 1 / 2.4)) - 0.055;
                //else
                //    var_B = 12.92 * var_B;

                //int r = (int)(var_R * 255.0);
                //int g = (int)(var_G * 255.0);
                //int b = (int)(var_B * 255.0);

                //r = Math.Min(Math.Max(r, 0), 255);
                //g = Math.Min(Math.Max(g, 0), 255);
                //b = Math.Min(Math.Max(b, 0), 255);

                //return Color.FromArgb(this._a, r, g, b);
            }
            set
            {
                this.RgbFloat = new ColorRgbFloat(value);
                //return;

                //float var_R = (float)value.R / 255f;
                //float var_G = (float)value.G / 255f;
                //float var_B = (float)value.B / 255f;

                //if (var_R > 0.04045f) var_R = (float)Math.Pow(((var_R + 0.055f) / 1.055f), 2.4);
                //else var_R = var_R / 12.92f;

                ////var_R = var_R * 100;
                ////var_G = var_G * 100;
                ////var_B = var_B * 100;

                ////Observer. = 2°, Illuminant = D65
                //this._x = var_R * 0.4124f + var_G * 0.3576f + var_B * 0.1805f;
                //this._y = var_R * 0.2126f + var_G * 0.7152f + var_B * 0.0722f;
                //this._z = var_R * 0.0193f + var_G * 0.1192f + var_B * 0.9505f;
            }
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._x, this._y, this._z, (float)this.A / 255f);
            }
            set
            {
                this._x = value.X;
                this._y = value.Y;
                this._z = value.Z;
                this.A = (int)(value.W * 255);
            }
        }
    }
}
