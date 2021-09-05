using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Endogine.Renderer.OpenGL
{
	/// <summary>
	/// Summary description for StageOGL.
	/// </summary>
	public class Stage : StageBase
	{
		private IntPtr _hDC; // Private GDI Device Context
		private IntPtr _hRC; // Permanent Rendering Context
		//private static bool[] keys = new bool[256]; // Array Used For The Keyboard Routine
		
		public Stage(Control RenderControl, EndogineHub a_endogine) : base(RenderControl, a_endogine)
		{
		}

		public override void Init()
		{
			this.CreateDevice(this.ControlSize.X, this.ControlSize.Y, 24);
			this.CreateRootSprite(new ERectangle(0,0, this.ControlSize.X, this.ControlSize.Y));
		}


		private void CreateDevice(int width, int height, int bits) 
		{
			int pixelFormat;  // Holds The Results After Searching For A Match
			//this.Fullscreen = fullscreenflag;

			GC.Collect();  // Request A Collection
			// This Forces A Swap
			Kernel.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

			if(this.m_bFullscreen) // Attempt Fullscreen Mode?
			{
				Gdi.DEVMODE dmScreenSettings = new Gdi.DEVMODE();               // Device Mode
				// Size Of The Devmode Structure
				dmScreenSettings.dmSize = (short) Marshal.SizeOf(dmScreenSettings);
				dmScreenSettings.dmPelsWidth = width;                           // Selected Screen Width
				dmScreenSettings.dmPelsHeight = height;                         // Selected Screen Height
				dmScreenSettings.dmBitsPerPel = bits;                           // Selected Bits Per Pixel
				dmScreenSettings.dmFields = Gdi.DM_BITSPERPEL | Gdi.DM_PELSWIDTH | Gdi.DM_PELSHEIGHT;

				// Try To Set Selected Mode And Get Results.  NOTE: CDS_FULLSCREEN Gets Rid Of Start Bar.
				if(User.ChangeDisplaySettings(ref dmScreenSettings, User.CDS_FULLSCREEN) != User.DISP_CHANGE_SUCCESSFUL) 
				{
					// The Mode Fails
					throw new Exception("Fullscreen not supported");
				}
			}

			Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();    // pfd Tells Windows How We Want Things To Be
			pfd.nSize = (short) Marshal.SizeOf(pfd);                            // Size Of This Pixel Format Descriptor
			pfd.nVersion = 1;                                                   // Version Number
			pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW |                              // Format Must Support Window
				Gdi.PFD_SUPPORT_OPENGL |                                        // Format Must Support OpenGL
				Gdi.PFD_DOUBLEBUFFER;                                           // Format Must Support Double Buffering
			pfd.iPixelType = (byte) Gdi.PFD_TYPE_RGBA;                          // Request An RGBA Format
			pfd.cColorBits = (byte) bits;                                       // Select Our Color Depth
			pfd.cRedBits = 0;                                                   // Color Bits Ignored
			pfd.cRedShift = 0;
			pfd.cGreenBits = 0;
			pfd.cGreenShift = 0;
			pfd.cBlueBits = 0;
			pfd.cBlueShift = 0;
			pfd.cAlphaBits = 0;                                                 // No Alpha Buffer
			pfd.cAlphaShift = 0;                                                // Shift Bit Ignored
			pfd.cAccumBits = 0;                                                 // No Accumulation Buffer
			pfd.cAccumRedBits = 0;                                              // Accumulation Bits Ignored
			pfd.cAccumGreenBits = 0;
			pfd.cAccumBlueBits = 0;
			pfd.cAccumAlphaBits = 0;
			pfd.cDepthBits = 16;                                                // 16Bit Z-Buffer (Depth Buffer)
			pfd.cStencilBits = 0;                                               // No Stencil Buffer
			pfd.cAuxBuffers = 0;                                                // No Auxiliary Buffer
			pfd.iLayerType = (byte) Gdi.PFD_MAIN_PLANE;                         // Main Drawing Layer
			pfd.bReserved = 0;                                                  // Reserved
			pfd.dwLayerMask = 0;                                                // Layer Masks Ignored
			pfd.dwVisibleMask = 0;
			pfd.dwDamageMask = 0;

			this._hDC = User.GetDC(this.RenderControl.Handle); // Attempt To Get A Device Context
			if(this._hDC == IntPtr.Zero) 
			{
				this.KillGLWindow();
				throw new Exception("Can't Create A GL Device Context.");
			}

			pixelFormat = Gdi.ChoosePixelFormat(this._hDC, ref pfd);                  // Attempt To Find An Appropriate Pixel Format
			if(pixelFormat == 0) 
			{                                              // Did Windows Find A Matching Pixel Format?
				this.KillGLWindow();
				throw new Exception("Can't Find A Suitable PixelFormat.");
			}

			if(!Gdi.SetPixelFormat(this._hDC, pixelFormat, ref pfd)) // Are We Able To Set The Pixel Format?
			{
				this.KillGLWindow();  // Reset The Display
				throw new Exception("Can't Set The PixelFormat.");
			}

			this._hRC = Wgl.wglCreateContext(this._hDC);  // Attempt To Get The Rendering Context
			if(this._hRC == IntPtr.Zero) 
			{
				this.KillGLWindow();
				throw new Exception("Can't Create A GL Rendering Context.");
			}

			if(!Wgl.wglMakeCurrent(this._hDC, this._hRC)) // Try To Activate The Rendering Context
			{
				this.KillGLWindow(); // Reset The Display
				throw new Exception("Can't Activate The GL Rendering Context.");
			}

			//form.Show();
			//form.TopMost = true;
			//form.Focus();

			this.ReSizeGLScene(this.RenderControl.Width, this.RenderControl.Height);                                       // Set Up Our Perspective GL Screen

			if(!InitGL()) 
			{                                                     // Initialize Our Newly Created GL Window
				this.KillGLWindow();                                                 // Reset The Display
				throw new Exception("Initialization Failed.");
			}
		}

		private bool InitGL() 
		{
			Gl.glShadeModel(Gl.GL_SMOOTH);                                      // Enable Smooth Shading
			Gl.glClearColor(0, 0, 0, 0.5f);                                     // Black Background
			Gl.glClearDepth(1);                                                 // Depth Buffer Setup
			Gl.glEnable(Gl.GL_DEPTH_TEST);                                      // Enables Depth Testing
			Gl.glDepthFunc(Gl.GL_LEQUAL);                                       // The Type Of Depth Testing To Do
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);         // Really Nice Perspective Calculations
			return true;
		}

		private void KillGLWindow() 
		{
			if(this.Fullscreen)
			{ 
				User.ChangeDisplaySettings(IntPtr.Zero, 0); // switch Back To The Desktop
				//Cursor.Show();
			}

			if(this._hRC != IntPtr.Zero) // Do We Have A Rendering Context?
			{
				if(!Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero)) 
				{             // Are We Able To Release The DC and RC Contexts?
					throw new Exception("Release Of DC And RC Failed.");
				}

				if(!Wgl.wglDeleteContext(this._hRC)) // Are We Able To Delete The RC?
				{
					throw new Exception("Release Rendering Context Failed.");
				}
				this._hRC = IntPtr.Zero; // Set RC To Null
			}

			if(this._hDC != IntPtr.Zero) // Do We Have A Device Context?
			{
				if(this.RenderControl != null && !this.RenderControl.IsDisposed)  // Do We Have A Window?
				{
					if(this.RenderControl.Handle != IntPtr.Zero) // Do We Have A Window Handle?
					{
						if(!User.ReleaseDC(this.RenderControl.Handle, this._hDC)) // Are We Able To Release The DC?
						{
							throw new Exception("Release Device Context Failed.");
						}
					}
				}
				this._hDC = IntPtr.Zero; // Set DC To Null
			}
		}

		private void ReSizeGLScene(int width, int height) 
		{
			if(height == 0) 
				height = 1;

			Gl.glViewport(0, 0, width, height);                                 // Reset The Current Viewport
			Gl.glMatrixMode(Gl.GL_PROJECTION);                                  // Select The Projection Matrix
			Gl.glLoadIdentity();                                                // Reset The Projection Matrix
			Glu.gluPerspective(45, width / (double) height, 0.1, 100);          // Calculate The Aspect Ratio Of The Window
			Gl.glMatrixMode(Gl.GL_MODELVIEW);                                   // Select The Modelview Matrix
			Gl.glLoadIdentity();                                                // Reset The Modelview Matrix
		}

		public override void Update()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT); // Clear The Screen And The Depth Buffer
			Gl.glLoadIdentity(); // Reset The View
			Gl.glTranslatef(0, 0, -5); //
			//Gl.glClearColor(0,1,1,0);

			m_spRoot.EnterFrame();
			//this.ZCurrent = 0;
			m_spRoot.Draw();


//			Gl.glBegin(Gl.GL_TRIANGLES);                                        // Drawing Using Triangles
//			Gl.glVertex3f(0, 1, 0);                                         // Top
//			Gl.glVertex3f(-1, -1, 0);                                       // Bottom Left
//			Gl.glVertex3f(1, -1, 0);                                        // Bottom Right
//			Gl.glEnd();                                                         // Finished Drawing The Triangle
//			Gl.glTranslatef(3, 0, 0);                                           // Move Right 3 Units
//			Gl.glBegin(Gl.GL_QUADS);                                            // Draw A Quad
//			Gl.glVertex3f(-1, 1, 0);                                        // Top Left
//			Gl.glVertex3f(1, 1, 0);                                         // Top Right
//			Gl.glVertex3f(1, -1, 0);                                        // Bottom Right
//			Gl.glVertex3f(-1, -1, 0);                                       // Bottom Left
//			Gl.glEnd();                                                         // Done Drawing The Quad

			Gdi.SwapBuffers(this._hDC);
			this.m_renderControl.Invalidate();
		}


		public override SpriteRenderStrategy CreateRenderStrategy()
		{
			SpriteRenderStrategy rs = new SpriteRenderStrategyA();
			return rs;
		}

		public override Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy CreateMemberStrategy()
		{
			Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy rs = new MemberSpriteBitmapRenderStrategyA();
			return rs;
		}
	}
}
