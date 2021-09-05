using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Endogine.BitmapHelpers
{
    public unsafe class Canvas : IDisposable
    {
        PixelDataProvider _pdp;
        protected byte* _data;
        protected int _stride;

        ERectangleF _clipRectangle;
        public ERectangleF ClipRectangle
        {
            get
            {
                if (this._clipRectangle == null)
                    this._clipRectangle = new ERectangleF(0, 0, this.Width, this.Height);
                return _clipRectangle;
            }
            set { _clipRectangle = value; }
        }

        ERectangleF _usedRectangle;
        /// <summary>
        /// Not used by class itself - for the user to keep track of the painted areas. (Auto-updating this property would slow down overall processing)
        /// </summary>
        public ERectangleF UsedRectangle
        {
            get { return _usedRectangle; }
            set { _usedRectangle = value; }
        }
        
        public Canvas()
        {
        }

        public static Canvas Create(System.Drawing.Bitmap bmp)
        {
            return Canvas.Create(new PixelDataProviderGDI(bmp));
        }

        public static Canvas Create(MemberSpriteBitmap mb)
        {
            return mb.Canvas;
            //return Canvas.Create(mb.PixelDataProvider);
        }

        public static Canvas Create(PixelDataProvider pdp)
        {
            Canvas c = null;
            switch (pdp.BitsPerPixel)
            {
                case 32:
                    c = new Canvas32();
                    break;
                case 24:
                    c = new Canvas24();
                    break;
                case 8:
                    c = new Canvas8();
                    break;
                default:
                    return null;
            }
            c.PixelDataProvider = pdp;
            return c;
        }

        public Canvas Copy()
        {
            Canvas canvas = this.CreateSimilar();
            this.Locked = false;
            //TODO: very slow if it's something else than a GDI+ bitmap...
            Graphics g = canvas.GetGraphics();
            Bitmap bmp = this.ToBitmap();
            g.DrawImageUnscaled(bmp, new Point(0, 0));
            canvas.Locked = false;
            return canvas;
        }

        public void Dispose()
        {
            this._pdp.Unlock();
            this._data = null;
        }

        public int BitsPerPixel
        {
            get { return this._pdp.BitsPerPixel; }
        }
        public bool HasAlpha
        {
            get { return this._pdp.BitsPerPixel == 32 || this._pdp.BitsPerPixel == 64; }
        }
        public PixelDataProvider PixelDataProvider
        {
            set { this._pdp = value; } //TODO: what if other BitsPerPixel than the current!? I really need a Strategy Pattern instead of inheriting
            get { return this._pdp; }
        }

        public int Width
        {
            get { return this._pdp.Width; }
        }
        public int Height
        {
            get { return this._pdp.Height; }
        }
        public ERectangle Rectangle
        {
            get { return new ERectangle(0, 0, this.Width, this.Height); }
        }

        public bool Locked
        {
            get
            {
                return this._pdp.Locked;
            }
            set
            {
                if (value)
                {
                    if (this._pdp.HasGraphicsLock)
                        this._pdp.Unlock();
                    if (!this._pdp.Locked)
                        this._pdp.Lock();

                    this._data = this._pdp.Pointer;
                    this._stride = this._pdp.Stride;
                }
                else
                {
                    if (this._pdp.Locked)
                    {
                        this._pdp.Unlock();
                        this._data = null;
                    }
                }
            }
        }

        public Graphics GetGraphics()
        {
            return this._pdp.GetGraphics();
        }

        public System.Drawing.Bitmap ToBitmap()
        {
            //bool wasLocked = this.Locked;
            //if (!wasLocked)
            //    this.Locked = true;
            System.Drawing.Bitmap bmp = this._pdp.ToBitmap();
            //if (!wasLocked)
            //    this.Locked = false;
            return bmp;
        }

        public virtual void SetPixel(int x, int y, Color clr)
        { }
        public virtual Color GetPixel(int x, int y)
        { return Color.Black; }

        public virtual void SetPixelInt(int x, int y, int clr)
        { }
        public virtual int GetPixelInt(int x, int y)
        { return 0; }

        public virtual void SetPixelByte(int x, int y, byte clr)
        { }
        public virtual byte GetPixelByte(int x, int y)
        { return 0; }

        public virtual void Fill(Color clr)
        { }

        public Canvas CreateSimilar(int width, int height, int numChannels)
        {
            if (width < 0)
                width = this.Width;
            if (height < 0)
                height = this.Height;
            //if (numChannels < 1)
            //    numChannels = this.

            PixelDataProvider pdp = this._pdp.CreateSimilar(width, height, numChannels);
            Canvas c = Canvas.Create(pdp);
            c.ClipRectangle = this.ClipRectangle.Copy();
            return c;
        }
        public Canvas CreateSimilar()
        {
            return this.CreateSimilar(this.Width, this.Height, 0);
        }

        public void DrawPath()
        {
            //System.Drawing.Imaging.GraphicsStream
            //Graphics g = 
        }

        public void FillRectangle(Color color, ERectangle rectangle)
        {
            this.Locked = true;
            int clr = color.ToArgb();
            if (rectangle == null)
                rectangle = this.Rectangle;

            int oppositeX = rectangle.OppositeX;
            for (int y = rectangle.Y; y < rectangle.OppositeY; y++)
            {
                for (int x = rectangle.X; x < oppositeX; x++)
                {
                    this.SetPixelInt(x, y, clr);
                }
            }
        }

        public ERectangle GetTrimmedRect(int channelNum, int trimRangeStart, int trimRangeEnd)
        {
            int shift = (3 - channelNum) * 8;
            this.Locked = true;

            ERectangle rctBounds = this.Rectangle.Copy();

            for (int side = 0; side < 2; side++)
            {
                int yDir = 1;
                int yUse = 0;
                if (side == 1)
                {
                    yDir = -1;
                    yUse = this.Height - 2;
                }

                bool bFound = false;
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = this.Width-1; x >= 0 ; x--)
                    {
                        int val = (this.GetPixelInt(x, yUse) >> shift) & 0xff;
                        if (val < trimRangeStart || val > trimRangeEnd)
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
                    xUse = this.Width - 1;
                }

                bool bFound = false;
                for (int x = 0; x < this.Width; x++)
                {
                    xUse += xDir;
                    for (int y = this.Height-1; y >= 0; y--)
                    {
                        int val = (this.GetPixelInt(xUse, y) >> shift) & 0xff;
                        if (val < trimRangeStart || val > trimRangeEnd)
                        {
                            if (side == 0)
                                rctBounds.Right = xUse;
                            else
                                rctBounds.Left = xUse;

                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        break;
                }
            }

            this.Locked = false;

            return rctBounds;
        }
    }
}
