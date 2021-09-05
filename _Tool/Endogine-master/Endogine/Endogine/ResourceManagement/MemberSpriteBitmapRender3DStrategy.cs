using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.DirectX.Direct3D;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRender3DStrategy.
	/// </summary>
	public class MemberSpriteBitmapRender3DStrategy : MemberSpriteBitmapRenderStrategy
	{
		private Texture m_tx;
		//private ImageInformation m_info;


		public MemberSpriteBitmapRender3DStrategy()
		{
		}

		public override void Dispose()
		{
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
			//Tests:
			//nColorKey = (unchecked((int)0xff000000));
			//nColorKey = (unchecked((int)0xffffffff));

			m_tx = TextureLoader.FromStream(
				m_endogine.Stage.D3DDevice, stream, m_info.Width, m_info.Height, 
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
						m_endogine.Stage.D3DDevice, stream, m_info.Width, m_info.Height, 
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
			m_tx = new Texture(m_endogine.Stage.D3DDevice, a_bmp, Usage.None, Pool.Managed);
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
				// Locks the surface
				int nPitch=0;
				int nLevelToLock = 0;
				Microsoft.DirectX.GraphicsStream gs = m_tx.LockRectangle(nLevelToLock, LockFlags.None, out nPitch);
				// AUTOGENMIPMAP

				// Creates the bitmap from the surface internal data
				//SurfaceDescription sd = m_tx.GetLevelDescription(nLevelToLock);
				//nPitch*GetFormatNumBytes(sd.Format)
				Bitmap bmp = new Bitmap(this.m_mb.TotalSize.X, this.m_mb.TotalSize.Y, nPitch,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb, gs.InternalData);
				//Bitmap MyBitmap = new Bitmap(sd.Width, sd.Height, nPitch*sd.Format, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ImageStream.InternalData);
				// Dispose the stuff you just used
				m_tx.UnlockRectangle(nLevelToLock);

				return bmp;
			}
			set
			{
				this.CreateFromBitmap(value);
			}
		}
		public override unsafe void SetPixels(int[,] pixels)
		{
			SurfaceDescription sd = this.m_tx.GetLevelDescription(0);

			int nPitch;
			uint* pData = (uint*)this.m_tx.LockRectangle(0, 
				LockFlags.None, out nPitch).InternalData.ToPointer();

			int strideDiff = nPitch - sd.Width*GetFormatNumBytes(sd.Format);

			for (int y = 0; y < sd.Height; y++)
			{
				for (int x = 0; x < sd.Width; x++)
				{
					*pData = (uint)pixels[x,y];
					pData++;
				}
				pData+=strideDiff;
			}
			this.m_tx.UnlockRectangle(0);
		}

		public override unsafe void SetPixels(System.Collections.Hashtable coordsAndColors)
		{
			SurfaceDescription sd = this.m_tx.GetLevelDescription(0);
			int nPitch;

			//uint* pData = (uint*)this.m_tx.LockRectangle(0, 
			//	LockFlags.None, out nPitch).InternalData.ToPointer();
			uint[,] data = (uint[,])this.m_tx.LockRectangle(typeof(uint), 0,
				LockFlags.None, sd.Width, sd.Height);

			System.Collections.IDictionaryEnumerator en = coordsAndColors.GetEnumerator();
			while (en.MoveNext())
			{
				Point pnt = (Point)en.Key;
				data[pnt.X,pnt.Y] = (uint)((Color)en.Value).ToArgb();
			}

//			for (int x = 0; x < sd.Width; x++)
//			{
//				for (int y = 0; y < sd.Height; y++)
//					data[x,y] = (uint)Color.FromArgb(100,200,233).ToArgb();
//			}

			this.m_tx.UnlockRectangle(0);
		}
	}
}
