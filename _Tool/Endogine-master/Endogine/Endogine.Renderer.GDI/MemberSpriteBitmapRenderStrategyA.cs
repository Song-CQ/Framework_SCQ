using System;
using System.Drawing;
using Endogine;

namespace Endogine.Renderer.GDI
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRenderDDStrategy.
	/// </summary>
	public class MemberSpriteBitmapRenderStrategyA : Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy
	{
		protected Bitmap m_bmp;

		public MemberSpriteBitmapRenderStrategyA()
		{
		}

		public override void Dispose()
		{
            //if (this._pixelManipulator!=null)
            //    this._pixelManipulator.Dispose();

            if (this._pixelDataProvider == null)
                this._pixelDataProvider.Unlock();

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

        //public override Endogine.BitmapHelpers.PixelManipulatorBase PixelManipulator
        //{
        //    get
        //    {
        //        if (this._pixelManipulator==null)
        //            this._pixelManipulator = new Endogine.BitmapHelpers.PixelManipulator(this.m_bmp);
        //        return this._pixelManipulator;
        //    }
        //    set {if (this._pixelManipulator==null)return; this._pixelManipulator.Dispose(); this._pixelManipulator=null;}
        //}

        public override Endogine.BitmapHelpers.PixelDataProvider PixelDataProvider
        {
            get
            {
                if (this._pixelDataProvider == null)
                    this._pixelDataProvider = new Endogine.BitmapHelpers.PixelDataProviderGDI(this.m_bmp);
                return this._pixelDataProvider;
            }
        }

        //TODO: use _pixelManipulator
        //public override void SetPixels(System.Collections.Hashtable coordsAndColors)
        //{
        //    this._pixelManipulator.SetPixels(coordsAndColors);
        //    //Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, coordsAndColors);
        //}
        //public override void SetPixels(int[,] pixels)
        //{
        //    this._pixelManipulator.SetPixels(pixels);
        //    //Endogine.BitmapHelpers.BitmapHelper.SetPixels(this.m_bmp, pixels);
        //}
	}
}
