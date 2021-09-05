using System;
using System.Collections.Generic;
using System.Drawing;

namespace Endogine.Interpolation
{
    //TODO: this class (and other interpolator) sucks badly. Should use custom AnimationKeys (where ease-in, or bezier handles etc takes care of interpolation)
	/// <summary>
	/// Summary description for InterpolatorColor.
	/// </summary>
	public class InterpolatorColor
	{
		private List<Interpolator> _interpolators;

        public InterpolatorColor()
        {
            this.Clear();
        }
        public InterpolatorColor(string gradient)
        {
            this.Clear();
            string[] parts = gradient.Split('@');
            System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                string[] posAndColor = parts[i].Split(':');
                float pos = Convert.ToSingle(posAndColor[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat); //TODO: should always be . separator! FormatProvider.
                blend.Positions[i] = pos;
                ColorEx.ColorRgb clr = new Endogine.ColorEx.ColorRgb(posAndColor[1]);
                blend.Colors[i] = clr.ColorRGBA;
            }
            this.ColorBlend = blend;
        }

        private void Clear()
        {
            this.Dispose();
            _interpolators = new List<Interpolator>();
            for (int i = 0; i < 4; i++)
            {
                Interpolator pol = new Interpolator();
                pol.Max = 255;
                _interpolators.Add(pol);
            }
        }

		public InterpolatorColor(System.Collections.SortedList colorKeyFrames)
		{
			//_colors = a_colorKeyFrames;
            _interpolators = new List<Interpolator>();
			
			for (int i = 0; i < 4; i++)
			{
				Interpolator ip = new Interpolator();
				_interpolators.Add(ip);
                System.Collections.SortedList a = new System.Collections.SortedList();
                for (int nClrNum = 0; nClrNum < colorKeyFrames.Count; nClrNum++)
				{
                    Color clr = (Color)colorKeyFrames.GetByIndex(nClrNum);
					double dVal = 0;
					if (i == 0)
						dVal = clr.A;
                    else if (i == 1)
                        dVal = clr.R;
                    else if (i == 2)
						dVal = clr.G;
					else if (i == 3)
						dVal = clr.B;
                    a.Add(colorKeyFrames.GetKey(nClrNum), dVal);
				}
				ip.KeyFramesList = a;
			}
		}

        public void Add(float position, Color color)
        {
            _interpolators[0].KeyFramesList.Add((double)position, (double)color.A);
            _interpolators[1].KeyFramesList.Add((double)position, (double)color.R);
            _interpolators[2].KeyFramesList.Add((double)position, (double)color.G);
            _interpolators[3].KeyFramesList.Add((double)position, (double)color.B);
        }

		public void Dispose()
		{
			if (this._interpolators!=null)
			{
				foreach (Interpolator ip in _interpolators)
					ip.Dispose();
				_interpolators.Clear();
				_interpolators = null;
			}
		}

		public Color GetValueAtTime(double a_dTime)
		{
            byte[] channels = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				double dVal = ((Interpolator)_interpolators[i]).GetValueAtTime(a_dTime);
                channels[i] = (byte)dVal;
                //if (i == 0)
                //    clr = Color.FromArgb((byte)dVal, 0, 0, 0); //clr.R, clr.G, clr.B);
                //else if (i == 1)
                //    clr = Color.FromArgb(clr.A, (byte)dVal, clr.G, clr.B);
                //else if (i == 2)
                //    clr = Color.FromArgb(clr.A, clr.R, (byte)dVal, clr.B);
                //else if (i == 3)
                //    clr = Color.FromArgb(clr.A, clr.R, clr.G, (byte)dVal);
			}
			return Color.FromArgb(channels[0], channels[1], channels[2], channels[3]);
		}

        public System.Drawing.Drawing2D.ColorBlend ColorBlend
        {
            get
            {
                int cnt = this._interpolators[0].KeyFramesList.Count;
                float minTime = (float)(double)this._interpolators[0].KeyFramesList.GetKey(0);
                float maxTime = (float)(double)this._interpolators[0].KeyFramesList.GetKey(cnt - 1);
                System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend(cnt);
                for (int i = 0; i < cnt; i++)
                {
                    double dTime = (double)this._interpolators[0].KeyFramesList.GetKey(i);
                    float time = (float)dTime;
                    time = (time - minTime) / (maxTime - minTime);
                    blend.Positions[i] = time;
                    Color clr = this.GetValueAtTime(dTime);
                    blend.Colors[i] = clr;
                }
                return blend;
            }
            set
            {
                this.Clear();
                for (int i = 0; i < value.Colors.Length; i++)
                {
                    Color clr = value.Colors[i];
                    float time = value.Positions[i];
                    this.Add(time, clr);
                }
            }
        }

        public List<Color> GenerateList(float a_dFromTime, float a_dToTime, int a_nNumEntries)
		{
            List<Color> aList = new List<Color>();
			double dStep = (a_dToTime-a_dFromTime)/a_nNumEntries;
			for (int i = 0; i < a_nNumEntries; i++)
				aList.Add(GetValueAtTime(dStep*i+a_dFromTime));
			return aList;
		}

        public override string ToString()
        {
            string result = "";
            System.Drawing.Drawing2D.ColorBlend blend = this.ColorBlend;
            for (int i = 0; i < blend.Colors.Length; i++)
            {
                ColorEx.ColorRgb clr = new Endogine.ColorEx.ColorRgb(blend.Colors[i]);
                string pos = "" + blend.Positions[i];
                result += pos.Replace(",",".") + ":" + clr.ToString("X") + "@";
            }
            return result.Remove(result.Length - 1);
        }
	}
}
