using System;
using System.Drawing;
using Endogine.BitmapHelpers;

namespace Endogine.Procedural
{
	/// <summary>
	/// Summary description for NoiseMarble.
	/// </summary>
	public class Marble : Noise
	{
		private float m_fTurbulence = 0.3f;
		private EPointF m_pntPeriods;
		public Marble()
		{
			m_pntPeriods = new EPointF(15,30);
		}

		public EPointF Periods //defines repetition of marble lines in x and y directions
		{
			set {m_pntPeriods = value;}
		}
		public float Turbulence
		{
			set {m_fTurbulence = value;}
		}

        public override void WriteToBitmap(Canvas canvas)
		{
            this.PrepareWriteBitmap(canvas);

            for (int x = canvas.Width - 1; x >= 0; x--)
            {
                for (int y = canvas.Height - 1; y >= 0; y--)
                {
					float xyValue = 1f*(float)x * m_pntPeriods.X / m_nNoiseWidth + (float)y * m_pntPeriods.Y / m_nNoiseHeight;
					//xyValue += (float)x * xPeriod / a_bmp.Width + (float)y * yPeriod / a_bmp.Height;
					xyValue += m_fTurbulence*GetNoiseValue(x, y, m_nOctaves);
					float total = (float)(Math.Sin(xyValue * 3.14159)); //Math.Abs

					total = total*0.5f + 0.5f;
					int fin = (int)(total*255);
					
					Color clr = (Color)m_aColorTable[fin];
                    canvas.SetPixel(x, y, clr);
				}
			}
		}
	}
}
