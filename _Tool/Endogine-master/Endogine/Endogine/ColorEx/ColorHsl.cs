using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    public class ColorHsl : ColorBase
    {

        public ColorHsl(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }

        public ColorHsl()
        {
        }

        private float _h;

        [Category("Axis"), ColorInfo("Hue", 360, 360)] //Description("Hue"), DefaultValue("359"), DisplayName("0")]
        public float H
        {
            get { return _h; }
            set { _h = value; }
        }
        private float _s;

        [Category("Axis"), ColorInfo("Saturation", 100)] //Description("Saturation"), DefaultValue("1"), DisplayName("1")]
        public float S
        {
            get { return _s; }
            set { _s = value; }
        }
        private float _l;

        [Category("Axis"), ColorInfo("Luminosity", 100)] //Description("Luminosity"), DefaultValue("1"), DisplayName("2")]
        public float L
        {
            get { return _l; }
            set { _l = value; }
        }

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                if (S == 0)
                {
                    return new ColorRgbFloat(this.A, this.L, this.L, this.L);
                }
                else
                {
                    float var_2 = 0;
                    if (L < 0.5f) var_2 = L * (1f + S);
                    else var_2 = L + S - (S * L);

                    float var_1 = 2f * L - var_2;

                    float h = H / 360;
                    float R = Hue_2_RGB(var_1, var_2, h + (1f / 3));
                    if (R < 0) R = 0; //TODO: this wasn't in specification...
                    float G = Hue_2_RGB(var_1, var_2, h);
                    if (G < 0) G = 0;
                    float B = Hue_2_RGB(var_1, var_2, h - (1f / 3));
                    if (B < 0) B = 0;
                    return new ColorRgbFloat(this.A, R, G, B);
                }
            }
            set
            {
                this.A = value.A;
                float r = value.R;
                float g = value.G;
                float b = value.B;

                float min = Math.Min(Math.Min(r, g), b);
                float max = Math.Max(Math.Max(r, g), b);
                float spread = max - min;

                L = (max + min) / 2;
                if (spread == 0)
                {
                    H = 0;
                    S = 0;
                }
                else
                {
                    if (L < 0.5) S = spread / (max + min);
                    else S = spread / (2f - max - min);

                    float del_R = (((max - r) / 6f) + (spread / 2f)) / spread;
                    float del_G = (((max - g) / 6f) + (spread / 2f)) / spread;
                    float del_B = (((max - b) / 6f) + (spread / 2f)) / spread;

                    if (r == max) H = del_B - del_G;
                    else if (g == max) H = (1f / 3) + del_R - del_B;
                    else if (b == max) H = (2f / 3) + del_G - del_R;

                    if (H < 0)
                        H += 1;
                    if (H > 1)
                        H -= 1;
                    H *= 360;
                }
            }
        }


        public override System.Drawing.Color ColorRGBA
        {
            get
            {
                if (S == 0)
                {
                    int val = (int)(L * 255);
                    return System.Drawing.Color.FromArgb(this.A, val, val, val);
                }
                else
                {
                    float var_2 = 0;
                    if (L < 0.5f) var_2 = L * (1f + S);
                    else var_2 = L + S - (S * L);
                    
                    float var_1 = 2f * L - var_2;

                    float h = H / 360;
                    int R = (int)(Hue_2_RGB(var_1, var_2, h + (1f / 3)) * 255);
                    if (R < 0) R = 0; //TODO: this wasn't in specification...
                    int G = (int)(Hue_2_RGB(var_1, var_2, h) * 255);
                    if (G < 0) G = 0;
                    int B = (int)(Hue_2_RGB(var_1, var_2, h - (1f / 3)) * 255);
                    if (B < 0) B = 0;
                    return System.Drawing.Color.FromArgb(this.A, R, G, B);
                }
            }
            set
            {
                this.A = value.A;
                float r = (float)value.R / 255;
                float g = (float)value.G / 255;
                float b = (float)value.B / 255;

                float min = Math.Min(Math.Min(r, g), b);
                float max = Math.Max(Math.Max(r, g), b);
                float spread = max - min;

                L = (max + min) / 2;
                if (spread == 0)
                {
                    H = 0;
                    S = 0;
                }
                else
                {
                    if (L < 0.5) S = spread / (max + min);
                    else S = spread / (2f - max - min);

                    float del_R = (((max - r) / 6f) + (spread / 2f)) / spread;
                    float del_G = (((max - g) / 6f) + (spread / 2f)) / spread;
                    float del_B = (((max - b) / 6f) + (spread / 2f)) / spread;

                    if (r == max) H = del_B - del_G;
                    else if (g == max) H = (1f / 3) + del_R - del_B;
                    else if (b == max) H = (2f / 3) + del_G - del_R;

                    if (H < 0)
                        H += 1;
                    if (H > 1)
                        H -= 1;
                    H *= 360;
                }
            }
        }

        private float Hue_2_RGB(float v1, float v2, float vH)             //Function Hue_2_RGB
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
            if ((2 * vH) < 1) return (v2);
            if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2f / 3) - vH) * 6);
            return (v1);
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._h / 360, this._s, this._l, (float)this.A / 255);
            }
            set
            {
                this.A = (int)(value.W * 255);
                this._h = value.X*360;
                this._s = value.Y;
                this._l = value.Z;
            }
        }
    }
}
