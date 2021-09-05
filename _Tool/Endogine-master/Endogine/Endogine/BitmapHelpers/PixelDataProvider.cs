using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.BitmapHelpers
{
    public unsafe abstract class PixelDataProvider
    {
        public abstract byte* Lock();
        public abstract void Unlock();

        public abstract int Width
        { get; }
        public abstract int Height
        { get; }
        public abstract int Stride
        { get; }
        public abstract int BitsPerPixel
        { get; }
        public abstract byte* Pointer
        { get; }
        public abstract System.Drawing.Graphics GetGraphics();
        protected System.Drawing.Graphics _graphics;
        protected bool _locked;
        public bool Locked
        { get { return this._locked; } }
        public bool HasGraphicsLock
        { get { return this._graphics != null; } }

        public abstract PixelDataProvider CreateSimilar(int width, int height, int numChannels);
        public abstract System.Drawing.Bitmap ToBitmap();
        //public abstract System.Drawing.Bitmap GetAsBitmap();
        public Canvas CreateCanvas()
        {
            return Endogine.BitmapHelpers.Canvas.Create(this);
        }
    }
}
