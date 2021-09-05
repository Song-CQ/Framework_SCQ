using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Endogine.BitmapHelpers
{
	/// <summary>
	/// Summary description for BitmapHelper.
	/// </summary>
	public class BitmapHelper
	{
		//TODO: many functions should use a GDI+ -based Endogine with sprites drawing to a bitmap!!
        //TODO: At least the pixelmanipulator should be used.
        public BitmapHelper()
		{
		}

		public static BitmapData GetBitmapData(Bitmap bmp)
		{
			System.Drawing.Imaging.BitmapData bmpData =	bmp.LockBits(
				new Rectangle(0,0,bmp.Width,bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite,
				bmp.PixelFormat);
			return bmpData;
		}

		public static int GetBytesPerPixel(Bitmap bmp)
		{
			//TODO: 16-bit, eg PixelFormat.Format16bppArgb1555
			return (bmp.PixelFormat == PixelFormat.Format24bppRgb)?3:
				((bmp.PixelFormat == PixelFormat.Format8bppIndexed)?1:4);
		}

		unsafe public static byte* GetPointerToLineStart(BitmapData bmpData, int nLineNr)
		{
			return ((byte*)bmpData.Scan0) + bmpData.Stride*nLineNr;
		}

		unsafe public static int GetNBits(byte* ptr, int numBits)
		{
			if (numBits == 3)
				return *(ptr+2)*65536 + *(ptr+1)*256 + *(ptr+0);
			else if (numBits == 4)
				return *(ptr+3)*16777216 + *(ptr+2)*65536 + *(ptr+1)*256 + *(ptr+0);
			return 0;
		}


		public static unsafe void SetPixels(Bitmap bmp, int[,] pixels)
		{
			BitmapData bmpData=bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			System.IntPtr Scan0 = bmpData.Scan0;
			byte* scan0=(byte*)(void*)Scan0;
			int* ptr = (int*)scan0;

			int strideDiff = bmpData.Stride - bmp.Width*4; // TODO: bytesPerPixel

			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					*ptr = pixels[x,y];
					ptr++;
				}
				ptr+=strideDiff;
			}
			bmp.UnlockBits(bmpData);
		}

        //public static void SetPixels(Bitmap bmp, System.Collections.Hashtable coordsAndColors)
        //{
        //}

		/// <summary>
		/// Just a simpler way to copy one bitmap into another with stretching. Too many parameters in the standard DrawImage method...
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		public static void DrawImage(Image dst, Image src)
		{
			Graphics g = Graphics.FromImage(dst);
			g.DrawImage(src, new Rectangle(0,0,dst.Width,dst.Height), new Rectangle(0,0,src.Width,src.Height), GraphicsUnit.Pixel);
		}


		public static bool GetIsTransparent(int nClr, int nKeyClr)
		{
			Color clr = Color.FromArgb(nClr);
			return (clr.A == 0); // || (clr.R==255&&clr.G==255&&clr.B==255));
		}

		public static unsafe Bitmap TrimWhitespace(Bitmap bmp, out EPoint topLeftCorner)
		{
            //TODO: name should be TrimAlpha
            int nThreshold = 0;
            Canvas canvas = Canvas.Create(bmp);
            canvas.Locked = true;

            ERectangle rctBounds = ERectangle.FromLTRB(0, 0, canvas.Width, canvas.Height);

            for (int side = 0; side < 2; side++)
            {
                int yDir = 1;
                int yUse = 0;
                if (side == 1)
                {
                    yDir = -1;
                    yUse = canvas.Height - 2;
                }

                bool bFound = false;
                for (int y = 0; y < canvas.Height; y++)
                {
                    for (int x = 0; x < canvas.Width; x++)
                    {
                        if (canvas.GetPixel(x, yUse).A > nThreshold)
                        {
                            if (side == 0)
                                rctBounds.Top = yUse;
                            else
                                rctBounds.Bottom = yUse;

                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        break;
                    yUse += yDir;
                }
            }

            for (int side = 0; side < 2; side++)
            {
                int xDir = 1;
                int xUse = 0;
                if (side == 1)
                {
                    xDir = -1;
                    xUse = canvas.Width - 1;
                }

                bool bFound = false;
                for (int x = 0; x < canvas.Width; x++)
                {
                    xUse += xDir;
                    for (int y = 0; y < canvas.Height; y++)
                    {
                        if (canvas.GetPixel(xUse, y).A > nThreshold)
                        {
                            if (side == 0)
                                rctBounds.Left = xUse;
                            else
                                rctBounds.Right = xUse;

                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        break;
                }
            }

            canvas.Locked = false;

			if (rctBounds.Width==0 || rctBounds.Height==0)
			{
				topLeftCorner = new EPoint();
				return null;
			}

			Bitmap trimmedBmp = new Bitmap(rctBounds.Width, rctBounds.Height);
			Graphics g = Graphics.FromImage(trimmedBmp);
			g.DrawImage(bmp, new Rectangle(0,0,rctBounds.Width,rctBounds.Height),
				rctBounds.X,rctBounds.Y,rctBounds.Width,rctBounds.Height,
				GraphicsUnit.Pixel);

			topLeftCorner = new EPoint(rctBounds.X, rctBounds.Y);

			return trimmedBmp;
		}

        public static void Save(Bitmap bmp, string filename)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            ImageCodecInfo codec = GetEncoderInfo(fi.Extension);
            bmp.Save(filename, codec, null);
        }

		public static ImageCodecInfo GetEncoderInfo(string format)
		{
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            format = format.ToLower();
            if (format.StartsWith("."))
                format = format.Remove(0, 1);
            if (format == "jpg")
                format = "jpeg";
            else if (format == "tif")
                format = "tiff";

			for(int j = 0; j < encoders.Length; ++j)
			{
				if(encoders[j].FormatDescription.ToLower() == format)
					return encoders[j];
			}
			return null;
		}

		public static Bitmap CreateFilledBitmap(EPoint pntSize, System.Drawing.Color clr)
		{
			PixelFormat pf = PixelFormat.Format24bppRgb;
			if (clr.A != 255)
				pf = PixelFormat.Format32bppArgb;
			System.Drawing.Bitmap bmp = new Bitmap(pntSize.X, pntSize.Y, pf);
			Graphics g = Graphics.FromImage(bmp);
			g.FillRectangle(new SolidBrush(clr), 0,0,pntSize.X, pntSize.Y);
			g.Dispose();
			return bmp;
		}

		/// <summary>
		/// Masks a bitmap by combining its alpha with the provided alpha mask.
		/// </summary>
		/// <param name="bmpDst">32-bit</param>
		/// <param name="bmpMask">8-bit grayscale</param>
		public static void Mask(Bitmap bmpDst, Bitmap bmpMask)
		{
			Bitmap bmpSrc = bmpMask;

			System.Drawing.Imaging.BitmapData dataSrc = bmpSrc.LockBits(new Rectangle(new Point(0,0), bmpSrc.Size),
				System.Drawing.Imaging.ImageLockMode.ReadOnly, bmpSrc.PixelFormat);

			System.Drawing.Imaging.BitmapData dataDst = bmpDst.LockBits(new Rectangle(new Point(0,0), bmpSrc.Size),
				System.Drawing.Imaging.ImageLockMode.ReadWrite, bmpDst.PixelFormat);

			int nBytesPerPix = GetBytesPerPixel(bmpDst);
			unsafe
			{
				int nWidthSrc = bmpSrc.Width*1;
				int nWidthDst = bmpDst.Width*nBytesPerPix;
				byte* ptrSrc = ((byte*)dataSrc.Scan0);
				byte* ptrDst = ((byte*)dataDst.Scan0) + 3; //alpha is 4th byte (?)
				for (int y=0; y<bmpSrc.Height; y++)
				{
					for (int x=0; x<bmpSrc.Width; x++)
					{
						//ucTemp = (unsigned char)((unsigned int)pPSrc1[ulS1P+3]*(255-pPSrc2[ulS2P+3]) >> 8);
						//pPDst[ulDP+3] = ucTemp + pPSrc2[ulS2P+3];
						*ptrDst = Math.Min(*ptrDst,*ptrSrc);
//						byte val = (byte)((uint)*ptrSrc*(255-*ptrDst) >> 8);
//						*ptrDst = (byte)(val + *ptrDst);
						ptrSrc++;
						ptrDst+=nBytesPerPix;
					}
					ptrSrc+=dataSrc.Stride-nWidthSrc;
					ptrDst+=dataDst.Stride-nWidthDst + (bmpDst.Width-bmpSrc.Width)*nBytesPerPix;
				}
			}
			bmpSrc.UnlockBits(dataSrc);
			bmpDst.UnlockBits(dataDst);
		}

		/// <summary>
		/// Creates an 8-bit grayscale bitmap with the contents of the supplied bitmap's specified channel
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public static Bitmap ExtractChannel(Bitmap bmpSrc, int channel)
		{
			Bitmap bmpDst = new Bitmap(bmpSrc.Width, bmpSrc.Height,
				System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
			//ColorPalette pal = bmpDst.Palette;
			//bmpDst.Palette = pal; //System.Drawing.Imaging.PaletteFlags.GrayScale;

			System.Drawing.Imaging.BitmapData dataSrc = bmpSrc.LockBits(new Rectangle(new Point(0,0), bmpSrc.Size),
				System.Drawing.Imaging.ImageLockMode.ReadOnly, bmpSrc.PixelFormat);

			System.Drawing.Imaging.BitmapData dataDst = bmpDst.LockBits(new Rectangle(new Point(0,0), bmpSrc.Size),
				System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpDst.PixelFormat);

			int nBytesPerPix = GetBytesPerPixel(bmpSrc);
			unsafe
			{
				int nWidthSrc = bmpSrc.Width*nBytesPerPix;
				byte* ptrSrc = ((byte*)dataSrc.Scan0) + channel;
				byte* ptrDst = ((byte*)dataDst.Scan0);
				for (int y=0; y<bmpSrc.Height; y++)
				{
					for (int x=0; x<bmpSrc.Width; x++)
					{
						*ptrDst = *ptrSrc;
						ptrSrc+=nBytesPerPix;
						ptrDst++;
					}
					ptrSrc+=dataSrc.Stride-nWidthSrc;
					ptrDst+=dataDst.Stride-bmpDst.Width;
				}
			}
			bmpSrc.UnlockBits(dataSrc);
			bmpDst.UnlockBits(dataDst);

			return bmpDst;
		}

		public static Bitmap CreateLineBitmap(ERectangleF rctLine, Color clrBg, Color clrPen, float penWidth, out EPointF locOffset)
		{
			//Because GDI+ arrows are so limited
			Bitmap bmp = null;
			locOffset = new EPointF();
		
			if (rctLine.Width != 0 && rctLine.Height != 0)
			{
				bmp = new Bitmap((int)Math.Abs(rctLine.Width)+1, (int)Math.Abs(rctLine.Height)+1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				Graphics g = Graphics.FromImage(bmp);
		
				g.FillRectangle(new SolidBrush(clrBg), 0,0,bmp.Width,bmp.Height);
		
				ERectangleF rctOrigo = rctLine.Copy();
				rctOrigo.MakeTopLeftAtOrigo();
		
				Pen pen = new Pen(clrPen, penWidth);
				g.DrawLine(pen, rctOrigo.X, rctOrigo.Y, rctOrigo.X+rctOrigo.Width, rctOrigo.Y+rctOrigo.Height);
				g.Dispose();
						
				if (rctOrigo.Width < 0)
					locOffset.X = rctOrigo.Width;
				if (rctOrigo.Height < 0)
					locOffset.Y = rctOrigo.Height;
			}
			return bmp;
		}

	}
}