using System;
using System.Drawing;
using Endogine.BitmapHelpers;

namespace Endogine.Procedural
{
	/// <summary>
	/// Summary description for NoiseMarble.
	/// </summary>
	public class Cellular : Noise
	{
		public Cellular()
		{
		}

        public override void WriteToBitmap(Canvas canvas)
		{
			Random rnd = new Random();
            this.PrepareWriteBitmap(canvas);
            int[,] array1 = new int[canvas.Width, canvas.Height];
            int[,] array2 = new int[canvas.Width, canvas.Height];
            for (int x = 0; x < canvas.Width; x++) 
			{
                for (int y = 0; y < canvas.Height; y++) 
				{
					array1[x,y] = rnd.Next(255);
				}
			}

			double m_dRate = 0.1;

			for (int nStep = 0; nStep < 50; nStep++)
			{
				//' Run the cellular automata step:
                for (int x = 0; x < canvas.Width; x++) 
				{
                    for (int y = 0; y < canvas.Height; y++) 
					{
						double dTot = 0;
						for(int iX=-1; iX<=1; iX++) 
						{
							int i = x + iX;
							int j = 0;
                            if (i < 0) i = canvas.Width - 1;
                            else if (i >= canvas.Width) i = 0;
							for(int iY=0; iY<=1; iY++) 
							{
								j = y + iY;
                                if (j < 0) j = canvas.Height - 1;
                                else if (j >= canvas.Height) j = 0;
								dTot+= array1[i,j];
							}
							dTot/= 9;
							dTot+= m_dRate;
							dTot = dTot - (int)dTot;
							array2[i, j] = (int)(dTot * 255.0);
						}
					}
				}
				int[,] arrayTmp;
				arrayTmp = array2;
				array2= array1;
				array1 = arrayTmp;
			}

            for (int x = canvas.Width - 1; x >= 0; x--)
            {
                for (int y = canvas.Height - 1; y >= 0; y--)
                {
					Color clr = (Color)m_aColorTable[array2[x,y]];
                    canvas.SetPixel(x, y, clr);
				}
			}
		}
	}
}
