using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine.ColorEx
{
	/// <summary>
	/// Summary description for ColorCmyk.
	/// </summary>
	public class ColorCmyk : ColorBase
	{
		//Note that Alpha isn't really interesting for cmyk print, but probably good to not lose the info when converting between spaces
		float _c;
		float _m;
		float _y;
		float _k;

        public ColorCmyk()
        {
        }

		public ColorCmyk(int a, float c, float m, float y, float k)
		{
			this.A = a;
			this._c = c;
			this._m = m;
			this._y = y;
			this._k = k;
		}
		public ColorCmyk(float c, float m, float y, float k)
		{
			this._c = c;
			this._m = m;
			this._y = y;
			this._k = k;
		}

		/// <summary>
		/// Idealized conversion.
		/// </summary>
		/// <param name="c"></param>
		public ColorCmyk(Color c)
		{
            this.ColorRGBA = c;
		}

        [Category("Axis"), ColorInfo("Cyan", 100)]
		public float C
		{
			get {return this._c;}
			set {this._c = value;}
		}
        [Category("Axis"), ColorInfo("Magenta", 100)]
        public float M
		{
			get {return this._m;}
			set {this._m = value;}
		}
        [Category("Axis"), ColorInfo("Yellow", 100)]
        public float Y
		{
			get {return this._y;}
			set {this._y = value;}
		}
		public float K
		{
			get {return this._k;}
			set {this._k = value;}
		}

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override Color ColorRGBA
        {
            get
            {
                int r = (int)((1f - (this._c * (1f - this._k) + this._k)) * 255);
                int g = (int)((1f - (this._m * (1f - this._k) + this._k)) * 255);
                int b = (int)((1f - (this._y * (1f - this._k) + this._k)) * 255);

                r = Math.Min(Math.Max(r, 0), 255);
                g = Math.Min(Math.Max(g, 0), 255);
                b = Math.Min(Math.Max(b, 0), 255);

                return Color.FromArgb(this.A, r, g, b);
            }
            set
            {
                Color c = value;
                this.A = c.A;
                this._c = 1f - (float)c.R / 255;
                this._m = 1f - (float)c.G / 255;
                this._y = 1f - (float)c.B / 255;
                this._k = this._c;

                if (this._m < this._k)
                    this._k = this._m;
                if (this._y < this._k)
                    this._k = this._y;
                this._c = this._c - this._k;
                this._m = this._m - this._k;
                this._y = this._y - this._k;
            }
        }

        public override Vector4 Vector
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
	}
}
