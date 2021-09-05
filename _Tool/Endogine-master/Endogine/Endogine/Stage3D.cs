using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

//using ThisMovie;

namespace Endogine
{
	/// <summary>
	/// Summary description for Stage3D.
	/// </summary>
	public class Stage3D : StageBase
	{
		public Device m_device = null;

		//GDI and 3D works differently in z-ordering. GDI will follow sprite hierarchy strictly, and 3D only goes on z.
		//Normally, we want 3D to act like GDI, so keep a counter for emulating draw order: 
		public float ZCurrent;
		public float ZStep = 0.1f;

		public Stage3D(Control RenderControl, EndogineHub a_endogine) : base(RenderControl, a_endogine)
		{
		}

		public override Device D3DDevice
		{ get {return this.m_device;}}

		public override void Init()
		{
			this.CreateDevice();
			this.CreateRootSprite(new ERectangle(0,0, this.m_device.Viewport.Width, this.m_device.Viewport.Height));
		}

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
			if (this.m_device!=null)
				this.m_device.Dispose();

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

			presentParams.SwapEffect = SwapEffect.Discard;
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

			// Do we support hardware vertex processing?
			if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
				// Replace the software vertex processing
				flags = CreateFlags.HardwareVertexProcessing;
            
			// Do we support a pure device?
			if (caps.DeviceCaps.SupportsPureDevice)
				flags |= CreateFlags.PureDevice;

			//			try
			//			{
			this.m_device = new Device(0, DeviceType.Hardware, this.m_renderControl, flags, presentParams);

			this.m_device.SamplerState[0].MagFilter = TextureFilter.None; //Linear;
			this.m_device.SamplerState[0].MinFilter = TextureFilter.None; //.Linear;
			this.m_device.SamplerState[0].MipFilter = TextureFilter.None;

			this.m_device.DeviceReset += new System.EventHandler(this.OnResetDevice);
			this.OnResetDevice(this.m_device, null);

			this.SetUpViews();
			//			}
			//			catch
			//			{}
		}

		public override void Dispose()
		{
			m_spRoot.Dispose();
			this.m_device.Dispose();
		}

		

		private void SetUpViews() 
		{
			//Set up views
			this.m_device.Transform.Projection = Matrix.OrthoLH((float)this.ControlSize.X, (float)this.ControlSize.Y, 0.1f, 10000f);
			//.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height , 0.1f, 100.0f ); 
			this.m_device.RenderState.CullMode = Cull.None;
			this.m_device.RenderState.Lighting = true;

			/*device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Diffuse = System.Drawing.Color.DarkTurquoise;
			device.Lights[0].Direction = new Vector3((float)Math.Cos(Environment.TickCount / 250.0f), 1.0f, (float)Math.Sin(Environment.TickCount / 250.0f));
			device.Lights[0].Enabled = true;*/

			this.m_device.RenderState.Ambient = System.Drawing.Color.FromArgb(0xFFFFFF);
		}


		public void OnResetDevice(object sender, EventArgs e) 
		{
			Device device = (Device)sender;
			SetUpViews();
		}


		public override void Update()
		{
			this.m_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color, 1.0f, 0);

			m_spRoot.EnterFrame();

			this.m_device.BeginScene();

			this.m_device.Transform.World = 
				Matrix.RotationYawPitchRoll(0.0f, 0.0f, 0.0f) * 
				Matrix.Translation(0.0f, 0.0f, 1.0f);
			//Geometry.DegreeToRadian(spinX), Geometry.DegreeToRadian(spinY)
			
			this.ZCurrent = 0;
			m_spRoot.Draw();

			this.m_device.EndScene();
			this.m_device.Present();

			this.m_renderControl.Invalidate();
		}
	}
}
