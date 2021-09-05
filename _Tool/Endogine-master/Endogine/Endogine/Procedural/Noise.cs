using System;
using System.Drawing;
using System.Collections.Generic;
using Endogine.BitmapHelpers;

namespace Endogine.Procedural
{
	/// <summary>
	/// Summary description for Noise.
	/// </summary>
	public class Noise
	{
		protected float m_fFreq, m_fDecay, m_fMaxVal;
		protected int m_nOctaves;
		protected EPointF m_pntOffset;
		protected bool m_bAutoNewRandomSeed = true;

		private int[,] m_aNoise;
		protected int m_nNoiseWidth, m_nNoiseHeight;
		private float m_fScaleX, m_fScaleY;
		protected List<Color> m_aColorTable;
		//http://dunnbypaul.net/perlin/ Hardware implementation

		public Noise()
		{
			Frequency = 0.5f;
			Decay = 0.5f;
			Octaves = 2;
			m_pntOffset = new EPointF();

			//default grayscale colortable:
            m_aColorTable = new List<Color>();
			for (int i = 0; i < 256; i++)
				m_aColorTable.Add(Color.FromArgb(i,i,i));

			m_nNoiseWidth = 100;
			m_nNoiseHeight = 100;
			Random rnd = new Random();
			m_aNoise = new int[m_nNoiseWidth,m_nNoiseHeight];
			for (int x = 0; x<m_nNoiseWidth; x++)
			{
				for (int y = 0; y<m_nNoiseHeight; y++)
				{
					m_aNoise[x,y] = rnd.Next(255);
					//m_aNoise[x,y] = (int)(127.5+127.5*Math.Sin((double)x*0.5+(double)y*0.5));
				}
			}
		}

        float _seed;
        /// <summary>
        /// 0-1
        /// </summary>
        public float Seed
        {
            set { this._seed = value; }
        }
		public float Frequency
		{
			set {m_fFreq = value;}
		}
		public int Octaves
		{
			set {m_nOctaves = value; CalcMaxVal();}
		}
		public float Decay
		{
			set {m_fDecay = value; CalcMaxVal();}
		}
		public EPointF Offset
		{
			get {return m_pntOffset;}
			set {m_pntOffset = value;}
		}

		private void CalcMaxVal()
		{
			m_fMaxVal = 0;
			for (int i = 0; i < m_nOctaves; i++)
				m_fMaxVal+=m_fDecay*(float)Math.Pow(m_fDecay, i);
		}

		private float Interpolate(float x, float y, float a) 
		{
			float b = 1-a;
			float fac1 = (float)(3*b*b - 2*b*b*b);
			float fac2 = (float)(3*a*a - 2*a*a*a);

			return x*fac1 + y*fac2; //add the weighted factors
//			double ft = a * 3.1415927;
//			double f = (1.0 - Math.Cos(ft)) * .5;
//			return  (float)(x*(1.0-f) +y*f);
		}


		protected float GetRandomValue(int x, int y)
		{
			x = (x+m_nNoiseWidth) % m_nNoiseWidth;
			y = (y+m_nNoiseHeight) % m_nNoiseHeight;
			float fVal = (float)m_aNoise[(int)(m_fScaleX*x), (int)(m_fScaleY*y)];
			return fVal/255*2-1f;
		}

		private float Smooth_Noise(int x, int y) 
		{
			float corners = ( Noise2d(x-1, y-1) + Noise2d(x+1, y-1) + Noise2d(x-1, y+1) + Noise2d(x+1, y+1) ) / 16.0f;
			float sides = ( Noise2d(x-1, y) +Noise2d(x+1, y) + Noise2d(x, y-1) + Noise2d(x, y+1) ) / 8.0f;
			float center = Noise2d(x, y) / 4.0f;
			return corners + sides + center;
		}

		private float Noise2d(int x, int y)
		{
//			if (false)
//			{
//				float fX = (float)x;
//				float fY = (float)y;
//
//				//get fractional part of x and y
//				float fractX = fX - (int)fX;
//				float fractY = fY - (int)fY;
//   
//				//wrap around
//				int x1 = ((int)fX + m_nNoiseWidth) % m_nNoiseWidth;
//				int y1 = ((int)fY + m_nNoiseHeight) % m_nNoiseHeight;
//   
//				//neighbor values
//				int x2 = (x1 + m_nNoiseWidth - 1) % m_nNoiseWidth;
//				int y2 = (y1 + m_nNoiseHeight - 1) % m_nNoiseHeight;
//
//				//smooth the noise with bilinear interpolation
//				float fValNew = 0.0f;
//				fValNew += fractX       * fractY       * m_aNoise[x1,y1];
//				fValNew += fractX       * (1 - fractY) * m_aNoise[x1,y2];
//				fValNew += (1 - fractX) * fractY       * m_aNoise[x2,y1];
//				fValNew += (1 - fractX) * (1 - fractY) * m_aNoise[x2,y2];
//
//				return fValNew/255*2-1f;
//			}

			x = (x+m_nNoiseWidth) % m_nNoiseWidth;
			y = (y+m_nNoiseHeight) % m_nNoiseHeight;
			float fVal = (float)m_aNoise[(int)(x), (int)(y)]; //m_fScaleX*x m_fScaleY*y
			return fVal/255*2-1f;
//
//			int n;
//			n = x + y * 57;
//			n = (n<<13) ^ n;
//			float res = (float)( 1.0 - ( (n * (n * n * 15731 + 789221)
//				+ 1376312589) & 0x7fffffff ) / 1073741824.0);
//			return res;
		}

		float GetValue(float x, float y) 
		{
			int Xint = (int)x;
			int Yint = (int)y;
			float Xfrac = x - Xint;
			float Yfrac = y - Yint;

			//float x0y0 = Noise2d(Xint, Yint);  //find the noise values of the four corners
			//float x1y0 = Noise2d(Xint+1, Yint);
			//float x0y1 = Noise2d(Xint, Yint+1);
			//float x1y1 = Noise2d(Xint+1, Yint+1);

			float x0y0 = Smooth_Noise(Xint, Yint);  //find the noise values of the four corners
			float x1y0 = Smooth_Noise(Xint+1, Yint);
			float x0y1 = Smooth_Noise(Xint, Yint+1);
			float x1y1 = Smooth_Noise(Xint+1, Yint+1);

			//interpolate between those values according to the x and y fractions
			float v1 = Interpolate(x0y0, x1y0, Xfrac); //interpolate in x direction (y)
			float v2 = Interpolate(x0y1, x1y1, Xfrac); //interpolate in x direction (y+1)
			float fin = Interpolate(v1, v2, Yfrac);  //interpolate in y direction

			return fin;
		}

		protected float GetNoiseValue(float x, float y, float size)
		{
			float val = 0.0f;
    
				float freq_factor = m_fFreq;
				float fAmplitude = m_fDecay;
				for(int k=0; k<m_nOctaves; k++) 
				{
					val += GetValue(x*freq_factor, y*freq_factor) * fAmplitude;
					fAmplitude *= m_fDecay;
					freq_factor *= 2;
				}
				return val/m_fMaxVal;
		}

		protected void PrepareWriteBitmap(Canvas canvas)
		{
            m_fScaleX = (float)m_nNoiseWidth / canvas.Width;
            m_fScaleY = (float)m_nNoiseHeight / canvas.Height;
		}

		public void SetColors(System.Collections.SortedList a_aColorsToInterpolate)
		{
			Endogine.Interpolation.InterpolatorColor clrIp= new Endogine.Interpolation.InterpolatorColor(a_aColorsToInterpolate);
			m_aColorTable = clrIp.GenerateList(0,1, 256);
		}
		public void SetColors(List<Color> a_aColorTable)
		{
			m_aColorTable = a_aColorTable;
		}

        public virtual void FillArray(System.Collections.ArrayList a_array)
		{
			m_fScaleX = (float)m_nNoiseWidth/a_array.Count;
			m_fScaleY = 1f;

			//Fills an array with noise values from 0 to 1
			for(int x=0; x<a_array.Count; x++) 
			{
				//TODO: implement m_pntOffset in derived classes as well!
				float total = GetNoiseValue((x+m_pntOffset.X)*m_fFreq, 1, m_nOctaves);
				a_array[x] = total*0.5f + 0.5f;
			}
		}
        public virtual void WriteToBitmap(Canvas canvas)
		{
            this.PrepareWriteBitmap(canvas);

            for (int x = canvas.Width - 1; x >= 0; x--)
			{
                for (int y = canvas.Height - 1; y >= 0; y--) 
				{
					float total = GetNoiseValue((x+m_pntOffset.X)*m_fFreq, (y+m_pntOffset.Y)*m_fFreq, m_nOctaves);

					total = total*0.5f + 0.5f;
					int fin = (int)(total*255);
					
					Color clr = (Color)m_aColorTable[fin];
                    canvas.SetPixel(x, y, clr);
				}
			}
		}
	}
}
