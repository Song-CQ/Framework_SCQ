using System;
using System.Drawing;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for MemberSpriteBitmapRenderStrategy.
	/// </summary>
	public abstract class MemberSpriteBitmapRenderStrategy
	{
		protected MemberSpriteBitmap m_mb;
		protected EndogineHub m_endogine;
        //protected Endogine.BitmapHelpers.PixelManipulatorBase _pixelManipulator;
        protected Endogine.BitmapHelpers.PixelDataProvider _pixelDataProvider;

		public void SetMemberBitmap(MemberSpriteBitmap a_mb)
		{
			m_mb = a_mb;
		}
		public void SetEndogine(EndogineHub a_endogine)
		{
			m_endogine = a_endogine;
		}

		abstract public void Load(string a_sFilename);
		abstract public void CreateFromBitmap(Bitmap a_bmp);
        abstract public void Dispose();
        //abstract public void SetPixels(int[,] pixels);
        //abstract public void SetPixels(System.Collections.Hashtable coordsAndColors);
		abstract public bool HasCachedBitmap{get;}
		abstract public Bitmap Bitmap	{get;	set;}

        //abstract public Endogine.BitmapHelpers.PixelManipulatorBase PixelManipulator {get;set;}
        abstract public Endogine.BitmapHelpers.PixelDataProvider PixelDataProvider { get; }
    }
}
