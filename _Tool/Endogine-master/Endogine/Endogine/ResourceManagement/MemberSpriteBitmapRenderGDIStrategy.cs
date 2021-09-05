using System;
using System.Drawing;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRenderDDStrategy.
	/// </summary>
	public class MemberSpriteBitmapRenderGDIStrategy : MemberSpriteBitmapRenderStrategy
	{
		protected Bitmap m_bmp;

		public MemberSpriteBitmapRenderGDIStrategy()
		{
		}

		public override void Dispose()
		{
			if (m_bmp!=null)
				m_bmp.Dispose();
			m_bmp = null;
		}

		public override void Load(string a_sFilename)
		{
			m_bmp =  m_mb.LoadIntoBitmap(a_sFilename);
		}
		
		public override void CreateFromBitmap(Bitmap a_bmp)
		{
			if (m_bmp!=null)
				m_bmp.Dispose();
			m_bmp = a_bmp;
		}

		//TODO: use MemberBitmapBase's Bitmap instead?
		public override bool HasCachedBitmap
		{
			get {return this.Bitmap!=null;}
		}

		public override Bitmap Bitmap
		{
			get {return m_bmp;}
			set {this.CreateFromBitmap(value);}
		}

		public override void SetPixels(System.Collections.Hashtable coordsAndColors)
		{
			Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, coordsAndColors);
		}
		public override void SetPixels(int[,] pixels)
		{
			Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, pixels);
		}
	}
}
