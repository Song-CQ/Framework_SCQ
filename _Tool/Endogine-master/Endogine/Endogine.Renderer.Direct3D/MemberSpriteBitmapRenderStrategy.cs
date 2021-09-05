using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.DirectX.Direct3D;

namespace Endogine.Renderer.Direct3D
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRender3DStrategy.
	/// </summary>
	public class MemberSpriteBitmapRenderStrategy : Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy
	{
		private Texture m_tx;
		private Device _device;

		public MemberSpriteBitmapRenderStrategy()
		{
		}

		public Device Device
		{
			get {return this._device;}
			set {this._device = value;}
		}

		public override void Dispose()
		{
            //if (this._pixelManipulator!=null)
            //    this._pixelManipulator.Dispose();
			if (m_tx!=null)
				m_tx.Dispose();
			m_tx = null;
		}


		public override void Load(string a_sFilename)
		{
			if (m_tx!=null)
				m_tx.Dispose();

			Bitmap bmp = m_mb.LoadIntoBitmap(a_sFilename);
			ImageInformation m_info;
			System.IO.Stream stream = null;

			if (m_mb.GotAnimation || m_mb.FileFullName.ToLower().IndexOf(".tif") > 0)
			{
				//it's probably an animated GIF. The loader above has already created a tileset of the animation frames
				//The following method is kind of silly...
				//writing the bitmap into a memory stream for the TextureLoader to read from,
				//because loading directly from a bitmap doesn't provide enough options.
				//Hopefully changed in a later version.

				//TODO: something goes wrong with alpha in GIFs - a lot of alpha where there should be none...
				stream = new System.IO.MemoryStream();
				ImageCodecInfo codec = Endogine.BitmapHelpers.BitmapHelper.GetEncoderInfo("PNG");
				bmp.Save(stream, codec, null);
				bmp.Dispose();
				stream.Position = 0;
			}
			else
			{
				bmp.Dispose();
				stream = new System.IO.FileStream(m_mb.FileFullName, System.IO.FileMode.Open);
			}


			m_info = TextureLoader.ImageInformationFromStream(stream);
			stream.Position = 0;
			
			int nMipLevels = 1;
			//format = EH.Instance.Stage.D3DDevice.PresentationParameters.BackBufferFormat;
			Format format = Format.A8R8G8B8; //TODO: should check render device format
			//TODO: should allow user to NOT create alpha for all textures

			int nColorKey = 0; //this.m_mb.ColorKey.ToArgb();

			m_tx = TextureLoader.FromStream(
				this._device, stream, m_info.Width, m_info.Height,  //m_endogine.Stage.D3DDevice
				nMipLevels, Usage.None, format, Pool.Managed,
				Filter.Linear, Filter.Point, nColorKey, ref m_info);
			stream.Position = 0;

			//TODO: Check pixel for alpha should be an option (enum with LeftTop, RightTop etc)
			bool bAlreadyGotAlpha = TextureFormatGotAlpha(m_info.Format);
			this.m_mb.GotAlpha = bAlreadyGotAlpha;

			bool bCheckPixelForAlpha = true;
			if (!bAlreadyGotAlpha && bCheckPixelForAlpha)
			{
				int nPitch=0;
				int nLevelToLock = 0;
				Microsoft.DirectX.GraphicsStream gs = m_tx.LockRectangle(
					nLevelToLock,
					new Rectangle(0,0,m_info.Width,m_info.Height),
					LockFlags.None, out nPitch);
				//TODO: this depends on texture format:
				byte[] buf = new byte[4];
				int nNumRead = gs.Read(buf, 0, 4);
				Color clr = Color.FromArgb((int)buf[3],(int)buf[0],(int)buf[1],(int)buf[2]);
				//If there already is a transparent pixel here, then the colorKey was right to begin with
				//I.e., only need to reload if Alpha != 0
				if (clr.A != 0)
				{
					nColorKey = clr.ToArgb();
					//TODO: manually process pixels and add alpha
					//I don't know what data formats to expect, though. E.g. can it be Yuv?
					//					for (int x = 0; x < m_info.Width; x++)
					//					{
					//						for (int y = 0; y < m_info.Height; y++)
					//						{
					//							gs.Read(buf, 0, 3);
					//						}
					//					}
					m_tx.UnlockRectangle(nLevelToLock);

					m_tx.Dispose();
						
					m_tx = TextureLoader.FromStream(
						this._device, stream, m_info.Width, m_info.Height, //m_endogine.Stage.D3DDevice
						nMipLevels, Usage.None, format, Pool.Managed,
						Filter.Linear, Filter.Point, nColorKey, ref m_info);
				}
			}
			stream.Close();
		}

		//TODO: move to general gfx function library
		private static bool TextureFormatGotAlpha(Microsoft.DirectX.Direct3D.Format f)
		{
			return (f==Format.A8B8G8R8 || f==Format.A8R8G8B8 ||
				f==Format.A16B16G16R16 || f==Format.A16B16G16R16F ||
				f==Format.A1R5G5B5 || f==Format.A2B10G10R10 ||
				f==Format.A2R10G10B10 || f==Format.A2W10V10U10 ||
				f==Format.A32B32G32R32F || f==Format.A4R4G4B4 ||
				f==Format.A8R3G3B2);
//				|| f==Format.X8R8G8B8 || f==Format.X8B8G8R8 ||
//				f==Format.X1R5G5B5 ||  f==Format.X4R4G4B4
//				);
		}
		private static int GetFormatNumBytes(Microsoft.DirectX.Direct3D.Format f)
		{
			if (f==Format.A8B8G8R8 || f==Format.A8R8G8B8 || f==Format.X8R8G8B8 || f==Format.X8B8G8R8)
				return 4;
			if (f==Format.A1R5G5B5 || f==Format.X1R5G5B5 || f==Format.A4R4G4B4 || f==Format.A8R3G3B2)
				return 2;
			return 16;

//				f==Format.A16B16G16R16 || f==Format.A16B16G16R16F ||
//					f== || f==Format.A2B10G10R10 ||
//							   f==Format.A2R10G10B10 || f==Format.A2W10V10U10 ||
//							   f==Format.A32B32G32R32F || f== Format.A4R4G4B4 ||
		}

		public override void CreateFromBitmap(Bitmap a_bmp)
		{
			if (m_tx!=null)
				m_tx.Dispose();
            

            if (true)
                m_tx = new Texture(this._device, a_bmp, Usage.None, Pool.Managed); //TODO: why is this so slow sometimes?
            else
            {
                Endogine.BitmapHelpers.Canvas canvasX = Endogine.BitmapHelpers.Canvas.Create(a_bmp);
                PixelDataProvider pdp = new PixelDataProvider(a_bmp.Width, a_bmp.Height, canvasX.BitsPerPixel / 8, this._device, Usage.None);

                Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(pdp);
                System.Drawing.Graphics g = canvas.GetGraphics();
                g.DrawImageUnscaled(a_bmp, new Point(0, 0));
                //for (int y = canvas.Height-1; y >= 0; y--)
                //{
                //    for (int x = canvas.Width - 1; x >= 0; x--)
                //    {
                //        canvas.SetPixel
                //    }
                //}
                canvas.Locked = false;

                m_tx = pdp.Texture;
            }
			
		}

		public Texture Texture
		{
			get {return m_tx;}
		}

		public override bool HasCachedBitmap
		{
			get {return false;}
		}

		public override Bitmap Bitmap
		{
			get
			{
                //user should be made aware that it's a costly operation
                //TODO: use pixeldataprovider
                return null;
			}
			set
			{
				this.CreateFromBitmap(value);
			}
		}

        public override Endogine.BitmapHelpers.PixelDataProvider PixelDataProvider
        {
            get
            {
                if (this._pixelDataProvider == null)
                    this._pixelDataProvider = new PixelDataProvider(this.m_tx);
                return this._pixelDataProvider;
            }
        }
	}
}
