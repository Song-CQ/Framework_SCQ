using System;
using System.Drawing;
using System.Drawing.Imaging;
using Endogine;
using Tao.OpenGl;

namespace Endogine.Renderer.OpenGL
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRenderDDStrategy.
	/// </summary>
	public class MemberSpriteBitmapRenderStrategyA : Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy
	{
		protected Bitmap m_bmp;
		protected int _textureId;

		public MemberSpriteBitmapRenderStrategyA()
		{
		}

		public override void Dispose()
		{
			if (this._textureId > 0)
			{
			}
		}

		public override void Load(string a_sFilename)
		{
			Bitmap bmp =  m_mb.LoadIntoBitmap(a_sFilename);

			Gl.glGenTextures(1, out this._textureId);                            // Create The Texture

			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); // Flip The Bitmap Along The Y-Axis
			Rectangle rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height); // Rectangle For Locking The Bitmap In Memory
			//Endogine.BitmapHelpers.BitmapHelper.
			BitmapData bitmapData = bmp.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb); //Format24bppRgb
			// Typical Texture Generation Using Data From The Bitmap
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, this._textureId);
			//Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, bmp.Width, bmp.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, bmp.Width, bmp.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
			
			bmp.UnlockBits(bitmapData);
			bmp.Dispose();
		}
		
		public override void CreateFromBitmap(Bitmap a_bmp)
		{
//			if (m_bmp!=null)
//				m_bmp.Dispose();
//			m_bmp = a_bmp;
		}

		public override bool HasCachedBitmap
		{
			get {return this.Bitmap!=null;}
		}

		public override Bitmap Bitmap
		{
			get {return m_bmp;}
			set {this.CreateFromBitmap(value);}
		}

		public int TextureId
		{
			get {return this._textureId;}
		}

		public override Endogine.BitmapHelpers.PixelManipulatorBase PixelManipulator
		{
			get
			{
				return null;
//				if (this._pixelManipulator==null)
//					this._pixelManipulator = new Endogine.BitmapHelpers.PixelManipulator(this.m_bmp);
//				return this._pixelManipulator;
			}
			set {if (this._pixelManipulator==null)return; this._pixelManipulator.Dispose(); this._pixelManipulator=null;}
		}


		public override void SetPixels(System.Collections.Hashtable coordsAndColors)
		{
			//Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, coordsAndColors);
		}
		public override void SetPixels(int[,] pixels)
		{
			//Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, pixels);
		}
	}
}
