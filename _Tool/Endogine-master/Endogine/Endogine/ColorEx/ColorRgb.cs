using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    public class ColorRgb : ColorBase
    {
        System.Drawing.Color _color;

        public ColorRgb()
        {
            this.A = 255;
        }

        public ColorRgb(int a, int r, int g, int b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public ColorRgb(int r, int g, int b)
        {
            this.A = 255;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public ColorRgb(System.Drawing.Color color)
        {
            this._color = color;
        }

        public ColorRgb(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }

        public override int A
        {
            get { return base.A; }
            set { this._color = System.Drawing.Color.FromArgb(value, this.R, this.G, this.B); base.A = value; }
        }
        [Category("Axis"), ColorInfo("Red", 255,255)] //Description("Red"), DefaultValue("255")]
        public int R
        {
            get { return this._color.R; }
            set { this._color = System.Drawing.Color.FromArgb(this.A, value, this.G, this.B); }
        }
        [Category("Axis"), ColorInfo("Green", 255, 255)] //Description("Green"), DefaultValue("255")]
        public int G
        {
            get { return this._color.G; }
            set { this._color = System.Drawing.Color.FromArgb(this.A, this.R, value, this.B); }
        }
        [Category("Axis"), ColorInfo("Blue", 255,255)] //Description("Blue"), DefaultValue("255")]
        public int B
        {
            get { return this._color.B; }
            set { this._color = System.Drawing.Color.FromArgb(this.A, this.R, this.G, value); }
        }


        public ColorRgb(string color)
        {
            color = color.Trim();
            if (color.IndexOf(" ") < 0 && color.IndexOf(";") < 0 && color.IndexOf(",") < 0)
            {
                int[] channels = new int[4];
                int offset = 1;
                if (color.Length == 8)
                {
                    offset = 0;
                }
                else
                {
                    channels[0] = 255;
                }

                for (int i = offset; i < 4; i++)
                    channels[i] = (System.Int32.Parse(color.Substring(i*2, (i*2+2) - (i*2)), System.Globalization.NumberStyles.AllowHexSpecifier));
                this._color = System.Drawing.Color.FromArgb(channels[0], channels[1], channels[2], channels[3]);
            }
        }

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                return new ColorRgbFloat(this.A, (float)this.R / 255, (float)this.G / 255, (float)this.B / 255);
            }
            set
            {
                this.A = value.A;
                this.R = (int)(value.R * 255);
                this.G = (int)(value.G * 255);
                this.B = (int)(value.B * 255);
            }
        }

        public override System.Drawing.Color ColorRGBA
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
            }
        }

        public override Vector4 Vector
        {
            get
            {
                System.Drawing.Color color = this.ColorRGBA;
                return new Vector4((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
            }
            set
            {
                this.ColorRGBA = System.Drawing.Color.FromArgb((int)(value.W * 255), (int)(value.X * 255), (int)(value.Y * 255), (int)(value.Z * 255));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">X=hex</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            if (format == "X")
                return this.A.ToString("X").PadLeft(2, '0') + this.R.ToString("X").PadLeft(2, '0') + this.G.ToString("X").PadLeft(2, '0') + this.B.ToString("X").PadLeft(2, '0');
            return this._color.ToString();
        }
    }
}
