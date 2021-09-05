using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Endogine.BitmapHelpers
{
    public unsafe class PixelDataProviderGDI : PixelDataProvider
    {
        Bitmap _bmp;
        BitmapData _bmpData;
        
        public PixelDataProviderGDI(Bitmap bmp) : base()
		{
            this._bmp = bmp;
		}

        public PixelDataProviderGDI(int width, int height, int numChannels)
        {
            PixelFormat pf = PixelFormat.Format8bppIndexed;
            switch (numChannels)
            {
                case 0:
                    pf = this._bmp.PixelFormat;
                    break;
                case 1:
                    pf = PixelFormat.Format8bppIndexed;
                    break;
                case 3:
                    pf = PixelFormat.Format24bppRgb;
                    break;
                case 4:
                    pf = PixelFormat.Format32bppArgb;
                    break;
                case 6:
                    pf = PixelFormat.Format48bppRgb;
                    break;
                case 8:
                    pf = PixelFormat.Format64bppArgb;
                    break;
            }
            this._bmp = new Bitmap(width, height, pf);
            if (numChannels == 1)
            {
                if (this._bmp.Palette.Entries.Length > 0) //default: grayscale
                {
                    System.Drawing.Imaging.ColorPalette pal = this._bmp.Palette;
                    for (int i = 0; i < 255; i++)
                        pal.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                    this._bmp.Palette = pal;
                }
            }
        }

        public override PixelDataProvider CreateSimilar(int width, int height, int numChannels)
        {
            if (numChannels <= 0)
                return new PixelDataProviderGDI(new Bitmap(width, height, this._bmp.PixelFormat));
            else
            {
                PixelDataProviderGDI pdp = new PixelDataProviderGDI(width, height, numChannels);
                if (numChannels == 1)
                {
                    if (this._bmp.Palette.Entries.Length > 0) //default: grayscale
                    {
                        for (int i = 0; i < this._bmp.Palette.Entries.Length; i++)
                            pdp._bmp.Palette.Entries[i] = this._bmp.Palette.Entries[i];
                    }
                }
                return pdp;
            }
        }

        public override int Height
        {
            get { return this._bmp.Height; }
        }
        public override int Width
        {
            get { return this._bmp.Width; }
        }
        public override int Stride
        {
            get { return this._bmpData.Stride; }
        }
        public override int BitsPerPixel
        {
            get
            {
                switch (this._bmp.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format32bppRgb:
                        return 32;

                    case PixelFormat.Format24bppRgb:
                        return 24;

                    case PixelFormat.Format8bppIndexed:
                        return 8;

                    case PixelFormat.Format48bppRgb:
                        return 48;

                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format64bppPArgb:
                        return 64;

                }
                return 0;
            }
        }


        public override void Unlock()
        {
            if (this._graphics != null)
            {
                this._graphics.Dispose();
                this._graphics = null;
            }
            else if (this._bmpData != null)
            {
                this._bmp.UnlockBits(this._bmpData);
                this._bmpData = null;
            }
            this._locked = false;
        }

        public override byte* Lock()
        {
            if (this._bmpData == null)
            {
                this._bmpData = this._bmp.LockBits(new Rectangle(0,0,this.Width,this.Height),
                    ImageLockMode.ReadWrite, this._bmp.PixelFormat);
            }
            this._locked = true;
            return (byte*)this._bmpData.Scan0;
        }
        public override byte* Pointer
        { get { return (byte*)this._bmpData.Scan0; } }

        public override Graphics GetGraphics()
        {
            if (this._graphics == null)
            {
                if (this._locked)
                    this.Unlock();

                this._graphics = Graphics.FromImage(this._bmp);

                if (this._graphics != null)
                    this._locked = true;
            }
            return this._graphics;
        }

        public override System.Drawing.Bitmap ToBitmap()
        {
            return this._bmp;
        }
        //public override System.Drawing.Bitmap GetAsBitmap()
        //{
        //    return this._bmp;
        //}

    }
}
