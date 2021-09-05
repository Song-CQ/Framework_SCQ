using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Endogine.BitmapHelpers
{
	/// <summary>
	/// Summary description for Filters.
	/// </summary>
	public class Filters
	{
		public Filters()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void EmbossFrom24BitGrayscale(Bitmap bmp)
		{
			BitmapData bmpData =	bmp.LockBits(
				new Rectangle(0,0,bmp.Width,bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite,
				bmp.PixelFormat);

			unsafe
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					byte* ptr = (byte*)bmpData.Scan0;
					ptr+= bmpData.Stride*y;

					int last = 0;

					for (int x = 0; x < bmp.Width; x++)
					{
						int nClr = BitmapHelper.GetNBits(ptr, 3);
						int nDiff = nClr-last;
						last = nClr;

						nDiff/=5000;

						int nNew = Math.Max(0,Math.Min(255,127+nDiff));

						for (int bit = 0; bit < 3; bit++)
							*(ptr+bit) = (byte)nNew;

						ptr+=3;
					}
				}
			}
			bmp.UnlockBits(bmpData);
		}

		public static void Emboss(Bitmap bmp)
		{
			System.Drawing.Imaging.BitmapData bmpData =	bmp.LockBits(
				new Rectangle(0,0,bmp.Width,bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite,
				bmp.PixelFormat);

			unsafe
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					byte* ptr = (byte*)bmpData.Scan0;
					ptr+= bmpData.Stride*y;

					int last = 0;
					for (int bit = 0; bit < 3; bit++)
						last += *(ptr+bit);

					for (int x = 0; x < bmp.Width; x++)
					{
						int nClr = 0;
						for (int bit = 0; bit < 3; bit++)
							nClr += *(ptr+bit);

						//nClr/=3;

						int nDiff = nClr-last;
						nDiff*=-1*20;
						last = nClr;

						int nNew = Math.Max(0,Math.Min(255,127-nDiff));

						for (int bit = 0; bit < 3; bit++)
							*(ptr+bit) = (byte)nNew;

						ptr+=3;
					}
				}
			}
			bmp.UnlockBits(bmpData);
		}

        //public static void FoldOut(Bitmap bmpSrc, Bitmap bmpDst, float angle, int numSteps)
        //{
        //}

		public static Bitmap FoldOut(Bitmap bmpSource, float angle, int numSteps)
		{
			//TODO: right now it just takes angle = PI*2 (360 degrees) and numSteps = 3
			Bitmap bmp = new Bitmap(bmpSource.Width*2,bmpSource.Height*2,
				bmpSource.PixelFormat);
			Graphics g = Graphics.FromImage(bmp);

            //TODO:
            //float angleStep = angle / (numSteps + 1);
            //for (int i = 0; i <= numSteps; i++)
            //{
            //    float tmpAngle = angleStep * i;
            //    g.ResetTransform();
            //    g.RotateTransform(tmpAngle);
            //}

            g.ResetTransform();
            g.DrawImageUnscaled(bmpSource, 0, 0);

			g.TranslateTransform(bmpSource.Width*2-1,0);
			g.RotateTransform(90);
			g.DrawImageUnscaled(bmpSource, 0,0);

			g.ResetTransform();
			g.TranslateTransform(bmpSource.Width*2-1, bmpSource.Width*2-1);
			g.RotateTransform(180);
			g.DrawImageUnscaled(bmpSource, 0,0);

			g.ResetTransform();
			g.TranslateTransform(0,bmpSource.Width*2-1);
			g.RotateTransform(270);
			g.DrawImageUnscaled(bmpSource, 0,0);

			return bmp;
		}

		public static void MaskFiles(string filesearch, Bitmap mask24Bit, string outputPath, EPoint offset)
		{
			if (offset == null)
				offset = new EPoint();

			Bitmap bmpSrc = mask24Bit;
			Graphics g = null;
			Bitmap bmpA = null;
			EPoint pntNewSize = new EPoint();
			System.IO.FileInfo[] files = Endogine.Files.FileFinder.GetFiles(filesearch);
			foreach (System.IO.FileInfo fi in files)
			{
				Bitmap bmpDst = new Bitmap(fi.FullName);
				//Find the rect that encloses both bitmaps
				ERectangle rctCommon = new ERectangle(0,0,bmpSrc.Width,bmpSrc.Height);
				rctCommon.Offset(offset);
				rctCommon.Expand(new ERectangle(0,0,bmpDst.Width,bmpDst.Height));

				//and make both bitmaps the same size (without scaling)
				Bitmap bmpX;
				EPoint pnt;

				bmpX = new Bitmap(bmpDst, rctCommon.Size.ToSize());
				g = Graphics.FromImage(bmpX);
				//g.FillRectangle(new SolidBrush(Color.FromArgb(255,255,255,255)), 0,0,bmpX.Width,bmpX.Height);
				g.Clear(Color.FromArgb(0,0,0,0));
				pnt = new EPoint();
				if (offset.X < 0) pnt.X = -offset.X;
				if (offset.Y < 0) pnt.Y = -offset.Y;
				g.DrawImage(bmpDst,
					new Rectangle(pnt.X,pnt.Y, bmpDst.Width, bmpDst.Height),
					0,0,bmpDst.Width, bmpDst.Height, GraphicsUnit.Pixel);
				//g.DrawImageUnscaled(bmpDst, pnt.X,pnt.Y, bmpDst.Width, bmpDst.Height); //(bmpX.Width-bmpDst.Width)/2,  (bmpX.Height-bmpDst.Height)/2);
				bmpDst = bmpX;

				bmpX = new Bitmap(bmpSrc, rctCommon.Size.ToSize());
				g = Graphics.FromImage(bmpX);
				g.FillRectangle(new SolidBrush(Color.Black), 0,0, bmpX.Width,bmpX.Height);
				pnt = new EPoint();
				if (offset.X > 0) pnt.X = offset.X;
				if (offset.Y > 0) pnt.Y = offset.Y;
				g.DrawImage(bmpSrc,
					new Rectangle(pnt.X,pnt.Y, bmpSrc.Width, bmpSrc.Height),
					0,0,bmpSrc.Width, bmpSrc.Height, GraphicsUnit.Pixel);

				//g.DrawImageUnscaled(bmpSrc, pnt.X,pnt.Y); //(bmpX.Width-bmpSrc.Width)/2,  (bmpX.Height-bmpSrc.Height)/2);
				bmpSrc = bmpX;

				if (true)
				{
					//this will be done the first time, and each time the bmpDst.Size is different from the last
					//with the resizing above, this will only happen once
					if (bmpDst.Size != pntNewSize.ToSize())
					{
						pntNewSize = new EPoint(bmpDst.Size.Width,bmpDst.Size.Height);
						bmpA = new Bitmap(bmpDst.Size.Width,bmpDst.Size.Height, PixelFormat.Format24bppRgb);
						g = Graphics.FromImage(bmpA);
						g.DrawImage(bmpSrc,
							new Rectangle(0,0,bmpDst.Width,bmpDst.Height),
							new Rectangle(0,0,bmpSrc.Width,bmpSrc.Height), GraphicsUnit.Pixel);
						bmpA = Endogine.BitmapHelpers.BitmapHelper.ExtractChannel(bmpA,0);
						//bmpA.Save(fi.DirectoryName+"\\a__maskX"+fi.Name);
					}
				}

				Endogine.BitmapHelpers.BitmapHelper.Mask(bmpDst, bmpA);
				string sOut = outputPath+fi.Name;
				bmpDst.Save(sOut);
			}
		}
	}
}
