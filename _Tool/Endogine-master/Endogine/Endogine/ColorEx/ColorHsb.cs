using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine.ColorEx
{
	/// <summary>
	/// Summary description for ColorHSB.
	/// </summary>
	public class ColorHsb : ColorBase
	{
		float _h;
		float _s;
		float _b;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h">hue, 0-360 degrees</param>
		/// <param name="s">saturation, 0f-1f</param>
		/// <param name="b">brightness, 0f-1f</param>
		public ColorHsb(float h, float s, float b)
		{
			this.A = 255;
			this._h = h;
			this._s = s;
			this._b = b;
		}
		public ColorHsb(int a, float h, float s, float b)
		{
			this.A = a;
			this._h = h;
			this._s = s;
			this._b = b;
		}

		public ColorHsb(Color c)
		{
            this.ColorRGBA = c;
		}

        public ColorHsb()
        {
        }

        public ColorHsb(ColorRgbFloat rgb)
        {
            this.RgbFloat = rgb;
        }


		//DisplayName is used for Ordinal
        [Category("Axis"), ColorInfo("Hue", 360, 360)] //Description("Hue"), DefaultValue("359"), DisplayName("0"), ColorInfo("Hue", 360f)]
		/// <summary>
		/// Hue, 0f-360f
		/// </summary>
		public float H
		{
			get {return this._h;}
			set {this._h = value;}
		}

        [Category("Axis"), ColorInfo("Saturation", 100)] //Description("Saturation"), DefaultValue("1"), DisplayName("1")]
        /// <summary>
		/// Saturation, 0f-1f
		/// </summary>
		public float S
		{
			get {return this._s;}
			set {this._s = value;}
		}

        [Category("Axis"), ColorInfo("Brightness", 100)] //Description("Brightness"), DefaultValue("1"), DisplayName("2")]
		/// <summary>
		/// Brightness, 0f-1f
		/// </summary>
		public float B
		{
			get {return this._b;}
			set {this._b = value;}
		}

		public static Color InterpolateRgbInHsbSpace(Color c1, Color c2, float position)
		{
			ColorHsb hsb1 = new ColorHsb(c1);
			hsb1.Interpolate(new ColorHsb(c2), position);
			return hsb1.ColorRGBA;
		}

		public void Interpolate(ColorHsb hsbTo, float position)
		{
			this.A = (int)(position*(hsbTo.A - this.A)+this.A);
			if (Math.Abs(hsbTo.H - this.H) > 180)
			{
				if (this.H < hsbTo.H)
					this.H+=360;
				else
					this.H-=360;
				this.H = position*(hsbTo.H - this.H)+this.H;
				if (this.H < 0)
					this.H+=360;
				else if (this.H > 360)
					this.H-=360;
			}
			else
				this.H = position*(hsbTo.H - this.H)+this.H;

			this.S = position*(hsbTo.S - this.S)+this.S;
			this.B = position*(hsbTo.B - this.B)+this.B;
		}

        public override ColorRgbFloat RgbFloat
        {
            get
            {
                if (this.S > 0)
                {
                    float fH, fS, fB;
                    fH = this.H / 60f;
                    fS = this.S;
                    fB = this.B;

                    uint nH = (uint)fH;
                    float fF, fP, fQ, fT;
                    fF = fH - (float)nH;

                    fP = fB * (1f - fS);
                    fQ = fB * (1f - fS * fF);
                    fT = fB * (1f - fS * (1f - fF));

                    float r, g, b;
                    r = g = b = 0;
                    switch (nH)
                    {
                        case 0: r = fB; g = fT; b = fP; break;
                        case 1: r = fQ; g = fB; b = fP; break;
                        case 2: r = fP; g = fB; b = fT; break;
                        case 3: r = fP; g = fQ; b = fB; break;
                        case 4: r = fT; g = fP; b = fB; break;
                        case 5: r = fB; g = fP; b = fQ; break;
                    }
                    return new ColorRgbFloat(this.A, r, g, b);
                }
                else
                {
                    return new ColorRgbFloat(this.A, this.B, this.B, this.B);
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
                float range = max - min;

                bool bIsV = true; //HSV or HSL? Which is which?
                this._h = 0;

                if (range == 0) //gray
                {
                    this._h = 0;
                    this._s = 0;
                    this._b = max;
                }
                else
                {
                    if (bIsV)
                    {
                        this._b = max;
                        this._s = range / max;
                    }
                    else
                    {
                        float l = (min + max) / 2;
                        this._b = l;
                        if (l < 0.5)
                            this._s = range / (min + max);
                        else
                            this._s = range / (2 - max - min);
                    }

                    float rX = (((max - r) / 6) + (range / 2)) / range;
                    float gX = (((max - g) / 6) + (range / 2)) / range;
                    float bX = (((max - b) / 6) + (range / 2)) / range;

                    if (r == max)
                        this._h = bX - gX;
                    else if (g == max)
                        this._h = 1f / 3 + rX - bX;
                    else if (b == max)
                        this._h = 2f / 3 + gX - rX;

                    if (this._h < 0)
                        this._h += 1;
                    if (this._h > 1)
                        this._h -= 1;

                    this._h *= 360;
                }
            }
        }

        public override Color ColorRGBA
        {
            get
            {
                if (this.S > 0)
                {
                    float fH, fS, fB;
                    fH = this.H / 60f;
                    fS = this.S;// / 100f;
                    fB = this.B;// / 100f;

                    uint nH = (uint)fH;
                    float fF, fP, fQ, fT;
                    fF = fH - (float)nH;

                    fP = fB * (1f - fS);
                    fQ = fB * (1f - fS * fF);
                    fT = fB * (1f - fS * (1f - fF));

                    int r, g, b;
                    r = g = b = 0;
                    switch (nH)
                    {
                        case 0: r = (int)(255f * fB); g = (int)(255f * fT); b = (int)(255f * fP); break;
                        case 1: r = (int)(255f * fQ); g = (int)(255f * fB); b = (int)(255f * fP); break;
                        case 2: r = (int)(255f * fP); g = (int)(255f * fB); b = (int)(255f * fT); break;
                        case 3: r = (int)(255f * fP); g = (int)(255f * fQ); b = (int)(255f * fB); break;
                        case 4: r = (int)(255f * fT); g = (int)(255f * fP); b = (int)(255f * fB); break;
                        case 5: r = (int)(255f * fB); g = (int)(255f * fP); b = (int)(255f * fQ); break;
                    }
                    return Color.FromArgb(this.A, r, g, b);
                }
                else
                {
                    int nGray = (int)(this.B * 255);
                    return Color.FromArgb(this.A, nGray, nGray, nGray);
                }
            }
            set
            {
                this.A = value.A;
                //			this._h= c.GetHue();
                //			this._s = c.GetSaturation();
                //			this._b = c.GetBrightness();

                float r = (float)value.R / 255;
                float g = (float)value.G / 255;
                float b = (float)value.B / 255;
                float min = Math.Min(Math.Min(r, g), b);
                float max = Math.Max(Math.Max(r, g), b);
                float range = max - min;

                bool bIsV = true; //HSV or HSL? Which is which?
                this._h = 0;

                if (range == 0) //gray
                {
                    this._h = 0;
                    this._s = 0;
                    this._b = max;
                }
                else
                {
                    if (bIsV)
                    {
                        this._b = max;
                        this._s = range / max;
                    }
                    else
                    {
                        float l = (min + max) / 2;
                        this._b = l;
                        if (l < 0.5)
                            this._s = range / (min + max);
                        else
                            this._s = range / (2 - max - min);
                    }

                    float rX = (((max - r) / 6) + (range / 2)) / range;
                    float gX = (((max - g) / 6) + (range / 2)) / range;
                    float bX = (((max - b) / 6) + (range / 2)) / range;

                    if (r == max)
                        this._h = bX - gX;
                    else if (g == max)
                        this._h = 1f / 3 + rX - bX;
                    else if (b == max)
                        this._h = 2f / 3 + gX - rX;

                    if (this._h < 0)
                        this._h += 1;
                    if (this._h > 1)
                        this._h -= 1;

                    this._h *= 360;
                }
            }
        }

        public override Vector4 Vector
        {
            get
            {
                return new Vector4(this._h / 360, this._s, this._b, (float)this.A / 255);
            }
            set
            {
                this.A = (int)(value.W * 255);
                this._h = value.X * 360;
                this._s = value.Y;
                this._b = value.Z;
            }
        }

        public Color To4Bytes()
        {
            return Color.FromArgb(this.A, (int)(this.H / 360f * 255), (int)(this.S * 255), (int)(this.B * 255));
        }

        public override void Validate()
        {
            if (this._h > 360)
                this._h -= 360f;
            else if (this._h < 0)
                this._h += 360f;

            if (this._s > 1)
                this._s = 1f;
            else if (this._s < 0)
                this._s = 0f;

            if (this._b > 1)
                this._b = 1f;
            else if (this._b < 0)
                this._b = 0f;
        }
	}
}
