using System;
using System.Drawing;
using Endogine.BitmapHelpers;

namespace Endogine.Procedural
{
	/// <summary>
	/// Summary description for NoiseMarble.
	/// </summary>
	public class Wood : Noise
	{
		private float m_fNumCircles = 6;
		private float m_fTurbulence = 0.3f;
		public Wood()
		{
		}

		public float NumCircles
		{
			set {m_fNumCircles = value;}
		}
		public float Turbulence
		{
			set {m_fTurbulence = value;}
		}

        public override void WriteToBitmap(Canvas canvas)
		{
            PrepareWriteBitmap(canvas);

/*			int[,] array1 = new int[a_bmp.Width,a_bmp.Height];
			int[,] array2 = new int[a_bmp.Width,a_bmp.Height];
			int[,] arrayTmp;

			for(int x=0; x<a_bmp.Width; x++) 
			{
				for(int y=0; y<a_bmp.Height; y++) 
					array2[x,y] = (int)(255*(0.5*GetRandomValue(x, y)+0.5));
			}

			int m_nNumIterations = 1;
			for (int i=0; i<m_nNumIterations; i++)
			{
				for(int x=1; x<a_bmp.Width-1; x++) 
				{
					for(int y=1; y<a_bmp.Height-1; y++) 
					{
						array1[x,y] =
									   ((array2[x-1, y-1]+array2[x+1, y-1]+
									   array2[x-1, y+1]+array2[x+1, y+1]+
									   array2[x-1, y] +array2[x+1, y]+
									   array2[x, y-1] +array2[x, y+1]+
									   array2[x, y]) / 9)+1;
					}
					arrayTmp = (int[,])array2.Clone();
					array2 = (int[,])array1.Clone();
					array1 = (int[,])arrayTmp.Clone();
				}
			}

			for(int x=0; x<a_bmp.Width; x++) 
			{
				for(int y=0; y<a_bmp.Height; y++) 
					a_bmp.SetPixel(x,y, Color.FromArgb(array2[x,y],0,0));
			}
			return;
*/

            for (int x = canvas.Width - 1; x >= 0; x--)
			{
                for (int y = canvas.Height - 1; y >= 0; y--) 
                {
                    float xValue = (x - canvas.Width / 2) / (float)canvas.Width;
                    float yValue = (y - canvas.Height / 2) / (float)canvas.Height;
					double distValue = Math.Sqrt(xValue * xValue + yValue * yValue);
					double d = distValue + m_fTurbulence * GetNoiseValue(x, y, m_nOctaves);
					float total = (float)Math.Sin(2 * m_fNumCircles * d * 3.14159);

					total = total*0.5f + 0.5f;
					int fin = (int)(total*255);
					
					Color clr = (Color)m_aColorTable[fin];
                    canvas.SetPixel(x, y, clr);
				}
			}
		}
	}
}
