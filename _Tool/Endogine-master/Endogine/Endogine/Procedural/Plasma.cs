using System;
using System.Drawing;
using Endogine.BitmapHelpers;

namespace Endogine.Procedural
{
	/// <summary>
	/// Summary description for NoiseMarble.
	/// </summary>
	public class Plasma : Noise
	{
		public Plasma()
		{
		}

        public override void WriteToBitmap(Canvas canvas)
		{
            this.PrepareWriteBitmap(canvas);

			Random rnd = new Random();

			double dMasterWL = 0.001/m_fFreq;
			double dXWLFact = rnd.NextDouble()*2+2;
			double dXYWLFact = rnd.NextDouble()*2+2;
			double dYWLFact = rnd.NextDouble()*2+2;
			double[] aOffsetsPerOctave = new double[3*m_nOctaves];
			for (int nOctave = 0; nOctave < m_nOctaves; nOctave++)
			{
				for (int i = 0; i < 3; i++)
					aOffsetsPerOctave[nOctave*3+i] = rnd.NextDouble()*3.1;
			}

			double dMax = -1000;
			double dMin = 1000;
            double[,] array = new double[canvas.Width, canvas.Height];

            for (int x = 0; x < canvas.Width; x++) 
			{
                for (int y = 0; y < canvas.Height; y++) 
				{
					double dAmplitude = 1.0;
					double dTotal = 0;
					for (int nOctave = 1; nOctave <= m_nOctaves; nOctave++)
					{
						double z1 = Math.Sin(dXWLFact*x*nOctave*dMasterWL +aOffsetsPerOctave[(nOctave-1)*3+0]);
						double z2 = Math.Sin((dXYWLFact*x*nOctave+y*nOctave)*dMasterWL +aOffsetsPerOctave[(nOctave-1)*3+1]);
						double z3 = Math.Sin(dYWLFact*y*nOctave*dMasterWL+aOffsetsPerOctave[(nOctave-1)*3+2]);
						dTotal+=dAmplitude*Math.Abs(z1+z2+z3);
						dAmplitude*=m_fDecay;
					}
					array[x,y] = dTotal;
					dMax = Math.Max(dMax, dTotal);
					dMin = Math.Min(dMin, dTotal);
				}
			}

            for (int x = canvas.Width - 1; x >= 0; x--)
			{
                for (int y = canvas.Height - 1; y >= 0; y--) 
                {
					Color clr = (Color)m_aColorTable[(int)((array[x,y]-dMin)/(dMax+dMin)*255)];
                    canvas.SetPixel(x, y, clr);
				}
			}
		}
	}
}
