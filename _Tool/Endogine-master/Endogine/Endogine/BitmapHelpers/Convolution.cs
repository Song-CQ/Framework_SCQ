using System;
using System.Drawing;

namespace Endogine.BitmapHelpers
{
    /// <summary>
    /// Summary description for Convolution.
    /// </summary>
    public class Convolution
    {
        public Convolution()
        {
        }

        public static Bitmap GaussianBlur(Bitmap bmpSrc)
        {
            float[,] kernel = new float[,]
{
	//	{0.33f, 0, 0},
	//	{0, 0.33f, 0},
	//	{0, 0, 0.33f}
	{0.045f, 0.122f, 0.045f},
	{0.122f, 0.332f, 0.122f},
	{0.045f, 0.122f, 0.045f}
	//{0.111f, 0.111f, 0.111f},
	//{0.111f, 0.111f, 0.111f},
	//{0.111f, 0.111f, 0.111f}
	//{1}
};
            return Convolve(bmpSrc, kernel);
        }

        public static Canvas Convolve(Canvas canvas, float[,] kernel)
        {
            //Bitmap bmp = ((Canvas)canvas).ToBitmap();
            Canvas canvasDst = canvas.CreateSimilar();
            //Bitmap bmpDst = new Bitmap(canvas.Width, canvas.Height, bmp.PixelFormat);
            //Canvas canvasDst = Canvas.Create(bmpDst);
            canvasDst.Locked = true;
            canvas.Locked = true;

            EPoint pntKernelSize = new EPoint(kernel.GetLength(0), kernel.GetLength(1));

            int nBpp = canvas.BitsPerPixel / 8;
            //TODO: what to do with palettes in D3D..?
            //if (nBpp == 1)
            //    bmpDst.Palette = bmp.Palette;

            EPoint canvasSize = new EPoint(canvas.Width, canvas.Height);
            float[] vals = new float[nBpp];
            for (int y = pntKernelSize.Y / 2; y < canvasSize.Y - pntKernelSize.Y / 2; y++)
            {
                for (int x = pntKernelSize.X / 2; x < canvasSize.X - pntKernelSize.X / 2; x++)
                {
                    for (int channel = 0; channel < nBpp; channel++)
                        vals[channel] = 0;
                    
                    for (int yKernel = 0; yKernel < pntKernelSize.Y; yKernel++)
                    {
                        for (int xKernel = 0; xKernel < pntKernelSize.X; xKernel++)
                        {
                            int clr = canvas.GetPixelInt(
                                x + xKernel - pntKernelSize.X / 2,
                                y + yKernel - pntKernelSize.Y / 2);

                            for (int channel = 0; channel < nBpp; channel++)
                            {
                                vals[channel] += kernel[xKernel, yKernel] * (float)(clr & 0xff);
                                clr >>= 8;
                            }
                        }
                    }

                    if (nBpp == 1)
                        canvasDst.SetPixelInt(x, y, vals[0] > 255 ? 255 : (int)vals[0]);
                    else if (nBpp == 3)
                        canvasDst.SetPixel(x, y, Color.FromArgb(
                            vals[2] > 255 ? 255 : (int)vals[2],
                            vals[1] > 255 ? 255 : (int)vals[1],
                            vals[0] > 255 ? 255 : (int)vals[0]));
                }
            }

            canvas.Locked = false;
            canvasDst.Locked = false;

            return canvasDst;
        }

        public static Bitmap Convolve(Bitmap bmpSrc, float[,] kernel)
        {
            Bitmap bmpDst = new Bitmap(bmpSrc.Width, bmpSrc.Height, bmpSrc.PixelFormat);
            System.Drawing.Imaging.BitmapData bmpDstData = bmpDst.LockBits(
                new Rectangle(0, 0, bmpDst.Width, bmpDst.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmpDst.PixelFormat);

            System.Drawing.Imaging.BitmapData bmpSrcData = bmpSrc.LockBits(
                new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmpSrc.PixelFormat);

            EPoint pntKernelSize = new EPoint(kernel.GetLength(0), kernel.GetLength(1));
            //TODO: find out depth properly
            int nBpp = 3;
            if (bmpSrc.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                nBpp = 4;

            unsafe
            {
                for (int y = pntKernelSize.Y / 2; y < bmpSrc.Height - pntKernelSize.Y / 2; y++)
                {
                    byte* ptrSrc = (byte*)bmpSrcData.Scan0;
                    byte* ptrDst = (byte*)bmpDstData.Scan0;
                    ptrDst += bmpDstData.Stride * y;

                    for (int x = pntKernelSize.X / 2; x < bmpSrc.Width - pntKernelSize.X / 2; x++)
                    {
                        for (int bit = 0; bit < nBpp; bit++)
                        {
                            float fVal = 0;
                            for (int yKernel = 0; yKernel < pntKernelSize.Y; yKernel++)
                            {
                                for (int xKernel = 0; xKernel < pntKernelSize.X; xKernel++)
                                {
                                    fVal += kernel[xKernel, yKernel] *
                                        (float)*(ptrSrc +
                                        bit + (x + xKernel - pntKernelSize.X / 2) * nBpp +
                                        (y + yKernel - pntKernelSize.Y / 2) * bmpSrcData.Stride);
                                }
                            }
                            *ptrDst = (byte)fVal;
                            ptrDst++;
                        }
                    }
                }
            }
            bmpSrc.UnlockBits(bmpSrcData);
            bmpDst.UnlockBits(bmpDstData);

            return bmpDst;
        }

    }
}
