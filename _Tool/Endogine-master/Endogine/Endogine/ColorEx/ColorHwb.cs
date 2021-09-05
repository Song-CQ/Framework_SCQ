using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    public class ColorHwb : ColorBase
    {
        //http://alvyray.com/Papers/hwb2rgb.htm
        private float _h;

        [Category("Axis"), ColorInfo("Hue", 360, 360)] //Description("Hue"), DefaultValue("359")]
        public float H
        {
            get { return _h; }
            set { _h = value; }
        }
        private float _w;

        [Category("Axis"), ColorInfo("Whiteness", 100)] //Description("Whiteness"), DefaultValue("1")]
        public float W
        {
            get { return _w; }
            set { _w = value; }
        }
        private float _b;

        [Category("Axis"), ColorInfo("Blackness", 100)] //Description("Blackness"), DefaultValue("1")]
        public float B
        {
            get { return _b; }
            set { _b = value; }
        }

        public ColorHwb()
        {
        }

        public ColorHwb(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }


        public override ColorRgbFloat RgbFloat
        {
            get
            {
                float h = (float)this._h / 60;
                float w = this._w;
                float b = this._b;
                int i;
                float v = 1f - b;
                i = (int)Math.Floor(h);
                float f = h - i;
                if ((i & 1) > 0) f = 1 - f; // if i is odd 	
                float n = w + f * (v - w); // linear interpolation between w and v 	
                switch (i)
                {
                    case 0: return new ColorRgbFloat(this.A, v, n, w);
                    case 1: return new ColorRgbFloat(this.A, n, v, w);
                    case 2: return new ColorRgbFloat(this.A, w, v, n);
                    case 3: return new ColorRgbFloat(this.A, w, n, v);
                    case 4: return new ColorRgbFloat(this.A, n, w, v);
                    case 5: return new ColorRgbFloat(this.A, v, w, n);
                }
                return new ColorRgbFloat(1, 0, 0, 0);
            }
            set
            {
                float r, g, b;
                r = value.R;
                g = value.G;
                b = value.B;
                this.A = value.A;
                this._w = Math.Min(Math.Min(r, g), b);
                float v = Math.Max(Math.Max(r, g), b);
                this._b = 1f - v;

                if (v != this._w)
                {
                    float f = (r == this._w) ? g - b : ((g == this._w) ? b - r : r - g);
                    int i = (r == this._w) ? 3 : ((g == this._w) ? 5 : 1);
                    this._h = (60f * (float)(i - f) / (v - this._w));
                }
            }
        }

        public override System.Drawing.Color ColorRGBA
        {
            get
            {
                float h = (float)this._h / 60;
                float w = this._w;
                float b = this._b;
                int i;
                float v = 1f - b;
                i = (int)Math.Floor(h);
                float f = h - i;
                if ((i & 1) > 0) f = 1 - f; // if i is odd 	
                float n = w + f * (v - w); // linear interpolation between w and v 	
                int V = (int)(v * 255f);
                int W = (int)(w * 255f);
                int N = (int)(n * 255f);
                switch (i)
                {
                    case 0: return System.Drawing.Color.FromArgb(this.A, V, N, W);
                    case 1: return System.Drawing.Color.FromArgb(this.A, N, V, W);
                    case 2: return System.Drawing.Color.FromArgb(this.A, W, V, N);
                    case 3: return System.Drawing.Color.FromArgb(this.A, W, N, V);
                    case 4: return System.Drawing.Color.FromArgb(this.A, N, W, V);
                    case 5: return System.Drawing.Color.FromArgb(this.A, V, W, N);
                }
                return System.Drawing.Color.Black;
            }
            set
            {
                float r, g, b;
                r = (float)value.R / 255f;
                g = (float)value.G / 255f;
                b = (float)value.B / 255f;
                this.A = value.A;
                this._w = Math.Min(Math.Min(r, g), b);
                float v = Math.Max(Math.Max(r, g), b);
                this._b = 1f - v;

                if (v != this._w)
                {
                    float f = (r == this._w) ? g - b : ((g == this._w) ? b - r : r - g);
                    int i = (r == this._w) ? 3 : ((g == this._w) ? 5 : 1);
                    this._h = (60f * (float)(i - f) / (v - this._w));
                }
            }
        }

        public ColorHwb(System.Drawing.Color color)
        {
            this.ColorRGBA = color;
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._h / 360, this._w, this._b, (float)this.A / 255);
            }
            set
            {
                this.A = (int)(value.W * 255);
                this._h = value.X*360;
                this._w = value.Y;
                this._b = value.Z;
            }
        }
    }
}
