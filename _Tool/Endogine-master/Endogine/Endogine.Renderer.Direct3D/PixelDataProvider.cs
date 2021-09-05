using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace Endogine.Renderer.Direct3D
{
    public unsafe class PixelDataProvider : Endogine.BitmapHelpers.PixelDataProvider
    {
        //TODO: this class should only wrap a Surface, *not* a texture.

   		Texture _tx;
        Surface _surf;
		SurfaceDescription _sd;
        Microsoft.DirectX.GraphicsStream _graphicsStream; //Requires DirectX Feb '06 (used to be Microsoft.DirectX.Direct3D.GraphicsStream)
        byte* _ptr;
        int _stride;
        
        public PixelDataProvider(Texture tx)
            : base()
		{
            this._tx = tx;
			//this._sd = this._tx.GetLevelDescription(0);
            this._surf = this._tx.GetSurfaceLevel(0); //AOAO
            this._sd = this._surf.Description;
		}

        public PixelDataProvider(Surface sf)
            : base()
        {
            this._surf = sf;
            this._sd = sf.Description;
        }

        public PixelDataProvider(int width, int height, int numChannels, Device device, Usage usage)
            : base()
        {
            Format pf = Format.X8R8G8B8;
            switch (numChannels)
            {
                case 1:
                    pf = Format.A8;
                    break;
                case 3:
                    pf = Format.R8G8B8;
                    break;
                case 4:
                    pf = Format.X8R8G8B8;
                    break;
                case 8:
                    pf = Format.A16B16G16R16;
                    break;
            }
            //TODO: how to find out which Formats are supported??
            //device.DeviceCaps.TextureCaps
            if (pf == Format.R8G8B8)
                pf = Format.X8R8G8B8;

            //RenderToSurface sf = new RenderToSurface(device, width, height, pf, false, null);
            //this._tx = new Texture(device, width, height, 1, Usage.RenderTarget, pf, Pool.Default); // Pool.Managed doesn't work with Usage.RenderTarget
            Pool pool = Pool.Managed;
            if (usage == Usage.RenderTarget)
                pool = Pool.Default;

            this._tx = new Texture(device, width, height, 1, usage, pf, pool);
            //this._sd = this._tx.GetLevelDescription(0);
            this._surf = this._tx.GetSurfaceLevel(0); //AOAO
            this._sd = this._surf.Description;
        }
        

        public override int Height
        {
            get { return this._sd.Height; }
        }
        public override int Width
        {
            get { return this._sd.Width; }
        }
        public override int Stride
        {
            get { return this._stride; }
        }
        public override int BitsPerPixel
        {
            get
            {
                switch (this._sd.Format)
                {
                    case Format.A8B8G8R8:
                    case Format.X8B8G8R8:
                    case Format.A8R8G8B8:
                    case Format.X8R8G8B8:
                        return 32;

                    case Format.A1R5G5B5:
                    case Format.X1R5G5B5:
                    case Format.A4R4G4B4:
                    case Format.A8R3G3B2:
                        return 16;

                    case Format.R8G8B8:
                        return 24;

                    case Format.A8:
                        return 8;

                    case Format.A16B16G16R16:
                    case Format.A16B16G16R16F:
                    case Format.G32R32F:
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
                Surface s = null;
                //if (this._tx != null)
                //    s = this._tx.GetSurfaceLevel(0);
                //else if (this._surf != null) //AOAO
                if (this._surf != null)
                    s = this._surf;
                s.ReleaseGraphics();
            }
            else if (this._ptr != null)
            {
                //if (this._tx != null)
                //    this._tx.UnlockRectangle(0);
                //else if (this._surf != null)
                if (this._surf != null) //AOAO
                    this._surf.UnlockRectangle();

                this._ptr = null;
                this._graphicsStream.Dispose();
                this._graphicsStream = null;
            }
            this._locked = false;
        }

        public override byte* Lock()
        {
            if (this._locked && this._ptr == null)
                this.Unlock();

            if (this._ptr == null)
            {
                System.Drawing.Rectangle rctLock = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);

                //if (this._tx != null)
                //    this._graphicsStream = this._tx.LockRectangle(0, rctLock, LockFlags.None, out this._stride);
                //else if (this._surf != null)
                if (this._surf != null) //AOAO
                    this._graphicsStream = this._surf.LockRectangle(rctLock, LockFlags.None, out this._stride);

                this._ptr = (byte*)this._graphicsStream.InternalData.ToPointer();
                this._locked = true;
            }
            return this._ptr;
        }
        public override byte* Pointer
        { get { return this._ptr; } }
        //public override IntPtr Pointer
        //{ get { return (IntPtr)this._ptr; } }

        public override System.Drawing.Graphics GetGraphics()
        {
            if (this._graphics == null)
            {
                if (this._ptr != null)
                    this.Unlock();

                Surface s = null;
                //if (this._tx != null)
                //    s = this._tx.GetSurfaceLevel(0);
                //else if (this._surf != null)
                if (this._surf != null) //AOAO
                    s = this._surf;

                this._graphics = s.GetGraphics();

                if (this._graphics != null)
                    this._locked = true;
            }
            return this._graphics;
        }

        public override Endogine.BitmapHelpers.PixelDataProvider CreateSimilar(int width, int height, int numChannels)
        {
            Format pf = this._sd.Format;
            switch (numChannels)
            {
                case 1:
                    pf = Format.A8;
                    break;
                case 3:
                    pf = Format.R8G8B8;
                    break;
                case 4:
                    pf = Format.X8R8G8B8;
                    break;
                case 8:
                    pf = Format.A16B16G16R16;
                    break;
            }

            //TODO: find possible formats...
            if (pf == Format.R8G8B8)
                pf = Format.X8R8G8B8;

            //TODO: merge with constructor above...
            PixelDataProvider pdp = null;
            if (this._tx != null)
            {
                Texture tx = new Texture(this._tx.Device, width, height, 1, this._sd.Usage, pf, this._sd.Pool); //Pool.Managed);
                pdp = new PixelDataProvider(tx);
            }
            else if (this._surf != null)
            {
                //TODO: Is there no way I can use the current surface to set pixelformat etc?
                Texture tx = new Texture(this._surf.Device, width, height, 1, this._sd.Usage, pf, this._sd.Pool); //Pool.Managed);
                pdp = new PixelDataProvider(tx);
                //Surface surf = new Surface(this._surf.Device, (System.Drawing.Bitmap)null, this._sd.Pool); //Pool.Managed);
                //pdp = new PixelDataProvider(surf);
            }
            return pdp;
        }

        /// <summary>
        /// Must already be locked before attempting this.
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Bitmap ToBitmap()
        {
            System.Drawing.Bitmap bmp = null;

            if (false) //why doesn't this work??
            {
                if (this._graphics == null)
                {
                    if (this._ptr != null)
                    {
                        if (this._graphicsStream.InternalBufferPointer != null)
                            bmp = new System.Drawing.Bitmap(this._graphicsStream);
                        else
                            this.Unlock();
                    }
                    if (bmp == null)
                        this.GetGraphics();
                }

                if (bmp == null)
                {
                    bmp = new System.Drawing.Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    this._graphics.DrawImageUnscaled(bmp, 0, 0);
                }
                this._graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), new System.Drawing.Rectangle(0, 0, 30, 30));
                //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                //g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), new System.Drawing.Rectangle(0, 0, 30, 30));
                return bmp;
            }
            else
            {
                //TODO: optionally keep internal bitmap - if (this._bmp==null)
                if (!this.Locked)
                    this.Lock();

                if (this._graphicsStream.InternalBufferPointer == null) //this._surf != null)
                {
                    //For some reason, creating a bitmap from the stream fails when using a surface...
                    Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(this);
                    canvas.Locked = true;
                    bmp = new System.Drawing.Bitmap(canvas.Width, canvas.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Endogine.BitmapHelpers.Canvas canvasDst = Endogine.BitmapHelpers.Canvas.Create(bmp);
                    canvasDst.Locked = true;
                    for (int y = 0; y < canvas.Height; y++)
                    {
                        for (int x = 0; x < canvas.Width; x++)
                            canvasDst.SetPixel(x, y, canvas.GetPixel(x, y));
                    }
                    canvas.Locked = false;
                    canvasDst.Locked = false;
                }
                else
                {
                    //return (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(this._graphicsStream);
                    bmp = new System.Drawing.Bitmap(this._graphicsStream);
                }
                return bmp;
            }
        }

        public Texture Texture
        {
            get { return this._tx; }
        }

        public Surface Surface
        {
            get { return this._surf; }
        }
        public SurfaceDescription SurfaceDescription
        {
            get { return this._sd; }
        }
    }
}
