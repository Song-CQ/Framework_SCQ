using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Endogine;
//using ThisMovie;

namespace Endogine.Renderer.Direct3D
{
	/// <summary>
	/// Summary description for Stage3D.
	/// </summary>
	public class Stage : StageBase
	{
		public Device _device = null;

		//GDI and 3D works differently in z-ordering. GDI will follow sprite hierarchy strictly, and 3D only goes on z.
		//Normally, we want 3D to act like GDI, so keep a counter for emulating draw order: 
		public float ZCurrent;
		public float ZStep = 0.1f;

        Surface _defaultRenderTarget;

        public Stage(Control RenderControl)
            : base(RenderControl) //EndogineHub a_endogine
		{
		}

        protected override void PreInit()
        {
            this.CreateDevice();

            this._shaders = new Shaders();
            ((Shaders)this._shaders).Device = this._device;
        }

        //public override void Init()
        //{
        //    this.CreateRootSprite(new ERectangle(0, 0, this.m_device.Viewport.Width, this.m_device.Viewport.Height));
        //}

		public override Control RenderControl
		{
			get {	return base.RenderControl;	}
			set
			{
				base.RenderControl = value;
				this.CreateDevice();
			}
		}

		private void CreateDevice()
		{
			if (this._device!=null)
				this._device.Dispose();

			PresentParameters presentParams = new PresentParameters();
			AdapterInformation adapter = Manager.Adapters.Default;

			DisplayMode dm;
			if (m_bFullscreen)
			{
				//TODO: DK kolla caps upplösning/refreshrate
//				foreach (Microsoft.DirectX.Direct3D.DisplayMode dispMode in Manager.Adapters.Default.SupportedDisplayModes)
//					dispMode.
				dm = new DisplayMode();
				dm.Width = 1024; //800;
				dm.Height = 768; //600;
				dm.Format = Format.X8R8G8B8;
			}
			else
				dm = adapter.CurrentDisplayMode;

            presentParams.SwapEffect = SwapEffect.Discard; // Discard; Copy
			presentParams.BackBufferFormat = Format.Unknown;
			if (m_bFullscreen)
			{
				presentParams.BackBufferFormat = dm.Format;
				presentParams.BackBufferWidth = dm.Width;
				presentParams.BackBufferHeight = dm.Height;
				presentParams.FullScreenRefreshRateInHz = 60;
				presentParams.PresentFlag = PresentFlag.LockableBackBuffer;
				presentParams.BackBufferCount = 1;
				presentParams.PresentationInterval = PresentInterval.One; //Immediate;
			}
			presentParams.AutoDepthStencilFormat = DepthFormat.D16;
			presentParams.EnableAutoDepthStencil = true;
			presentParams.Windowed = !m_bFullscreen;

			//http://www.dotnetforums.net/t73717.html
			//http://www.dotnetforums.net/showthread.php?t=92593

			// Store the default adapter
			int adapterOrdinal = Manager.Adapters.Default.Adapter;
			CreateFlags flags;

			flags = CreateFlags.SoftwareVertexProcessing;
			// Check to see if we can use a pure hardware device
			Caps caps = Manager.GetDeviceCaps(adapterOrdinal, DeviceType.Hardware);

			if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
				flags = CreateFlags.HardwareVertexProcessing;
            
			// Do we support a pure device?
			if (caps.DeviceCaps.SupportsPureDevice)
				flags |= CreateFlags.PureDevice;

			this._device = new Device(0, DeviceType.Hardware, this._renderControl, flags, presentParams);

            //TODO: for some unknown reason, this suddenly stopped working on my new Acer laptop... Something to do with switching to DirectX debug runtime? Worked a while after that though.
            try
            {
                this._device.SamplerState[0].MagFilter = TextureFilter.None; //Linear;
                this._device.SamplerState[0].MinFilter = TextureFilter.None; //.Linear;
                this._device.SamplerState[0].MipFilter = TextureFilter.None;
            }
            catch { }

			this._device.DeviceReset += new System.EventHandler(this.OnResetDevice);
			this.OnResetDevice(this._device, null);

			this.SetUpViews();

            this._defaultRenderTarget = this._device.GetRenderTarget(0);
		}

		public override void Dispose()
		{
			_spRoot.Dispose();
			this._device.Dispose();
		}

		private void SetUpViews() 
		{
			//Set up views
			this._device.Transform.Projection = Matrix.OrthoLH((float)this.ControlSize.X, (float)this.ControlSize.Y, 0.1f, 10000f);
			//.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height , 0.1f, 100.0f ); 
			this._device.RenderState.CullMode = Cull.None;
			this._device.RenderState.Lighting = true;

			this._device.RenderState.Ambient = System.Drawing.Color.FromArgb(0xFFFFFF);
		}

		public void OnResetDevice(object sender, EventArgs e) 
		{
			Device device = (Device)sender;
			SetUpViews();
		}

        public override Endogine.BitmapHelpers.PixelDataProvider CreateRenderTarget(int width, int height, int numChannels)
        {
            //Surface surf = this._device.CreateRenderTarget(width, height, Format.X8R8G8B8, MultiSampleType.None, 0, true);
            //return new PixelDataProvider(surf);
            return new PixelDataProvider(width, height, numChannels, this._device, Usage.RenderTarget);
        }

        public override Endogine.BitmapHelpers.PixelDataProvider UglyConvertRenderTargetToReadable(Endogine.BitmapHelpers.PixelDataProvider pdpTarget)
        {
            PixelDataProvider pdp3DTarget = (PixelDataProvider)pdpTarget;

            //http://www.gamedev.net/community/forums/topic.asp?topic_id=388869
            //http://www.gamedev.net/community/forums/topic.asp?topic_id=384658&whichpage=1&
            //http://www.gamedev.net/community/forums/topic.asp?topic_id=388736

            //TODO: EXTREMELY ugly!! First copy to system memory, then back to card!!
            Surface sfSrc = pdp3DTarget.Texture.GetSurfaceLevel(0);

            //this._device.StretchRectangle(sfSrc, new System.Drawing.Rectangle(0,0,pdpTarget.Width,pdpTarget.Height), 

            Surface sfDst = this._device.CreateOffscreenPlainSurface(pdp3DTarget.Width, pdp3DTarget.Height,
                pdp3DTarget.SurfaceDescription.Format, Pool.SystemMemory); //Pool.SystemMemory Managed

            this._device.GetRenderTargetData(sfSrc, sfDst);

            //Surface sfDst2 = new Surface(this._device, 
            //SurfaceLoader.FromSurface(sfDst2, 
            Texture tx = new Texture(this._device, pdp3DTarget.Width, pdp3DTarget.Height, 1, Usage.None, pdp3DTarget.SurfaceDescription.Format, Pool.Managed);
            //this._device.UpdateSurface(sfDst, sf);

            //unsafe
            //{
            //    byte* ptr;
            //    //byte[] array = (byte[])ptr;
            //    Buffer.BlockCopy(ptr, 0, ptr, 0, 20000);
            //}
            PixelDataProvider pdpCP = new PixelDataProvider(sfDst);
            Endogine.BitmapHelpers.Canvas cCP = Endogine.BitmapHelpers.Canvas.Create(pdpCP);
            cCP.Locked = true;

            PixelDataProvider pdp = new PixelDataProvider(tx);
            Endogine.BitmapHelpers.Canvas c1 = Endogine.BitmapHelpers.Canvas.Create(pdp);
            c1.Locked = true;
            for (int y = c1.Height-1; y >= 0; y--)
            {
                for (int x = c1.Width - 1; x >= 0; x--)
                    c1.SetPixel(x, y, cCP.GetPixel(x, y));
            }
            c1.Locked = false;
            cCP.Locked = false;

            //return pdpCP;
            return pdp;
        }

        public override Endogine.BitmapHelpers.PixelDataProvider RenderTarget
        {
            get { return null; } //TODO:!
            set
            {
                Surface newTarget = null;
                EPoint size = null;
                if (value == null)
                {
                    newTarget = this._defaultRenderTarget;
                    size = this.ControlSize;
                }
                else
                {
                    this._defaultRenderTarget = this._device.GetRenderTarget(0);
                    Surface surf = ((PixelDataProvider)value).Texture.GetSurfaceLevel(0);
                    bool isTarget = ((int)surf.Description.Usage & (int)Usage.RenderTarget) != 0;
                    if (!isTarget)
                        throw new Exception("Texture isn't Usage.RenderTarget!");
                    newTarget = surf;
                    size = new EPoint(surf.Description.Width, surf.Description.Height);
                }
                this._device.SetRenderTarget(0, newTarget);
                this._device.Transform.Projection = Matrix.OrthoLH((float)size.X, (float)size.Y, 0.1f, 10000f);
            }
        }

        public override Endogine.BitmapHelpers.PixelDataProvider TransformIntoRenderTarget(Endogine.BitmapHelpers.PixelDataProvider pdp)
        {
            Surface surf = ((PixelDataProvider)pdp).Texture.GetSurfaceLevel(0);
            bool isTarget = ((int)surf.Description.Usage & (int)Usage.RenderTarget) != 0;
            if (isTarget)
                return pdp;

            Surface oldSurfRT = this._device.GetRenderTarget(0);

            PixelDataProvider pdpRT = new PixelDataProvider(pdp.Width, pdp.Height, pdp.BitsPerPixel / 8, this._device, Usage.RenderTarget);
            this.RenderTarget = pdpRT;

            Endogine.SpriteRenderStrategy rs = this.CreateRenderStrategy();
            rs.Init();
            rs.SetPixelDataProvider(pdp);
            ERectangleF rctDrawTarget = new ERectangleF(0,0, pdp.Width, pdp.Height);//-pdp.Width, -pdp.Height, pdp.Width * 2, pdp.Height * 2) * 2;
            //rs.SourceClipRect = sourceClipRect;
            rs.CalcRenderRegion(rctDrawTarget, 0, new EPoint(), new EPoint(pdp.Width, pdp.Height));

            //render:
            this.PreUpdate();
            rs.SubDraw();
            this.EndUpdate();

            //reset renderer and finish up:
            this._device.SetRenderTarget(0, oldSurfRT);
            this.PostUpdate();

            return pdpRT;
        }


        public override void PreUpdate()
        {
            if (this._clearTarget)
                this._device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, this.Color, 1.0f, 0); //Color  System.Drawing.Color.AliceBlue
            else
                this._device.Clear(ClearFlags.ZBuffer, this.Color, 1.0f, 0); //Color  System.Drawing.Color.AliceBlue

            if (_spRoot != null)
                _spRoot.EnterFrame();

            this._device.BeginScene();

            this._device.Transform.World =
                Matrix.RotationYawPitchRoll(0.0f, 0.0f, 0.0f) *
                Matrix.Translation(0.0f, 0.0f, 1.0f);
            //Geometry.DegreeToRadian(spinX), Geometry.DegreeToRadian(spinY)

            this.ZCurrent = 0;
            base.PreUpdate();
        }

		public override void Update()
		{
            //TODO: set up a pattern for RenderToTexture stuff - they must all be rendered before main render.
            this.PreUpdate();
            if (_spRoot != null)
                _spRoot.Draw();
            this.EndUpdate();

            this.PostUpdate();
		}

        public override void EndUpdate()
        {
            this._device.EndScene();
            base.EndUpdate();
        }
        public override void PostUpdate()
        {
            this._device.Present();
            this._renderControl.Invalidate();
            base.PostUpdate();
        }

        public override Endogine.BitmapHelpers.Canvas Capture() //Endogine.BitmapHelpers.PixelDataProvider
        {
            // surface = device.CreateOffscreenPlainSurface(1024, 768, Format.A8R8G8B8, Pool.SystemMemory)
            //this.m_device.GetFrontBufferData(0, null);

            //http://groups.google.com/group/microsoft.public.win32.programmer.directx.managed/browse_thread/thread/ef600178de0b4fe2/4f8f0fc9f6f9dbef?q=back+buffer&rnum=4#4f8f0fc9f6f9dbef
            //The debug runtime fills the back buffer with solid green/red to alert 
            //you to situations where you are not completely writing the back buffer 
            //when using DISCARD swap effects.
            Surface bb = this._device.GetBackBuffer(0, 0, BackBufferType.Mono);
            return Endogine.BitmapHelpers.Canvas.Create(new PixelDataProvider(bb));
        }


		public override Endogine.SpriteRenderStrategy CreateRenderStrategy()
		{
            Endogine.SpriteRenderStrategy rs = new SpriteRenderStrategy();
			((SpriteRenderStrategy)rs).Device = this._device;
			return rs;
		}

		public override Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy CreateMemberStrategy()
		{
			Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy rs = new MemberSpriteBitmapRenderStrategy();
			((MemberSpriteBitmapRenderStrategy)rs).Device = this._device;
			return rs;
		}

        public override Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(System.Drawing.Bitmap bmp)
        {
            PixelDataProvider pdpTx = null;
            Texture tx = null;
            if (true)
            {
                //TODO: Saving bitmap to a MemoryStream and using TextureLoader.FromStream() might be faster
                tx = new Texture(this._device, bmp.Width, bmp.Height, 1, Usage.None, Format.X8R8G8B8, Pool.Managed);
                pdpTx = new PixelDataProvider(tx);
                Endogine.BitmapHelpers.Canvas canvasTx = Endogine.BitmapHelpers.Canvas.Create(pdpTx);
                canvasTx.Locked = true;

                //TODO: remove when MDX 2.0 gets up to speed...
                Endogine.BitmapHelpers.PixelDataProviderGDI pdpGDI = new Endogine.BitmapHelpers.PixelDataProviderGDI(bmp);
                Endogine.BitmapHelpers.Canvas canvasSrc = Endogine.BitmapHelpers.Canvas.Create(pdpGDI);
                canvasSrc.Locked = true;
                for (int y = canvasSrc.Height-1; y >= 0 ; y--)
                {
                    for (int x = canvasSrc.Width - 1; x >= 0; x--)
                        canvasTx.SetPixel(x, y, canvasSrc.GetPixel(x, y));
                }
                canvasSrc.Locked = false;
                canvasTx.Locked = false;
            }
            else
            {
                tx = new Texture(this._device, bmp, Usage.None, Pool.Managed);
                pdpTx = new PixelDataProvider(tx);
            }
            return pdpTx;
        }
        public override Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(int width, int height, int numChannels)
        {
            return new PixelDataProvider(width, height, numChannels, this._device, Usage.None);
        }
	}
}