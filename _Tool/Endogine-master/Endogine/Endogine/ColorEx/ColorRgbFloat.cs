using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.ColorEx
{
    public class ColorRgbFloat : ColorBase
    {
        //public new int A
        //{
        //    get { return this._color.A; }
        //    set { this._color = System.Drawing.Color.FromArgb(value, this.R, this.G, this.B); }
        //}

        float _r;
        [Category("Axis"), ColorInfo("Red", 255)] //Description("Red"), DefaultValue("1")]
        public float R
        {
            get { return this._r; }
            set { this._r = value; }
        }

        float _g;
        [Category("Axis"), ColorInfo("Green", 255)] //Description("Green"), DefaultValue("1")]
        public float G
        {
            get { return this._g; }
            set { this._g = value; }
        }

        float _b;
        [Category("Axis"), ColorInfo("Blue", 255)] //Description("Blue"), DefaultValue("1")]
        public float B
        {
            get { return this._b; }
            set { this._b = value; }
        }

        public ColorRgbFloat()
        {
            this.A = 255;
        }

        public ColorRgbFloat(int a, float r, float g, float b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public ColorRgbFloat(float r, float g, float b)
        {
            this.A = 255;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public ColorRgbFloat(System.Drawing.Color color)
        {
            this.ColorRGBA = color;
        }

        public override ColorRgbFloat RgbFloat
        {
            get { return this; }
            set
            {
                this._r = value._r;
                this._g = value._g;
                this._b = value._b;
                this.A = value.A;
            }
        }

        public override System.Drawing.Color ColorRGBA
        {
            get
            {
                return System.Drawing.Color.FromArgb(this.A, (int)(255 * this._r), (int)(255 * this._g), (int)(255 * this._b));
            }
            set
            {
                this.A = value.A;
                this._r = (float)value.R / 255;
                this._g = (float)value.G / 255;
                this._b = (float)value.B / 255;
            }
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._r, this._g, this._b, (float)this.A / 255);
            }
            set
            {
                this.A = (int)(value.W * 255);
                this._r = value.X;
                this._g = value.Y;
                this._b = value.Z;
            }
        }

        public override float[] Array
        {
            get
            {
                return new float[] { this._r, this._g, this._b, (float)this.A / 255 };
            }
            set
            {
                if (value.Length == 4)
                    this.A = (int)(value[3] * 255);
                else
                    this.A = 255;
                this._r = value[0];
                this._g = value[1];
                this._b = value[2];
            }
        }
    }
}
