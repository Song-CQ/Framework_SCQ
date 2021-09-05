using System;
using System.Drawing;
using System.ComponentModel;

using Endogine.ResourceManagement;

//TODO: should be part of Endogine.ResourceManagement, but this takes so long to write...
namespace Endogine
{
	/// <summary>
    /// Bitmap media for sprites. If it's a D3D sprite, a texture is used. If it's a GDI+ sprite, a GDI+ Bitmap is used. 
	/// </summary>
	public class MemberSpriteBitmap : MemberBitmapBase
	{
		protected MemberSpriteBitmapRenderStrategy m_renderStrategy;
        Endogine.BitmapHelpers.Canvas _canvas;

		public MemberSpriteBitmap(string a_sFilename)
		{
			//TODO: important! See to it that this isn't used when inappropriate - 
			//the same image may be loaded multiple times!
			if (EH.Instance.CastLib.GetByName(a_sFilename) != null)
			{
				//Could we "return" the already existing member instead??
			}
			AutoSetStrategy();
			m_renderStrategy.Load(a_sFilename);
		}

		public MemberSpriteBitmap(Bitmap a_bmp)
		{
			AutoSetStrategy();
			Bitmap = a_bmp;
		}

		public MemberSpriteBitmap()
		{
			AutoSetStrategy();
		}

        public MemberSpriteBitmap(MemberSpriteBitmapRenderStrategy mrs)
        {
            this.m_renderStrategy = mrs;
        }

		public override void Dispose()
		{
			m_renderStrategy.Dispose();
			if (m_renderStrategy.HasCachedBitmap)
				Bitmap.Dispose();

            if (this._canvas != null)
                this._canvas.Locked = false;

			base.Dispose ();
		}

		protected void AutoSetStrategy()
		{
			m_renderStrategy = m_endogine.Stage.CreateMemberStrategy();
			
			m_renderStrategy.SetMemberBitmap(this);
			m_renderStrategy.SetEndogine(m_endogine);
		}


		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap Bitmap
		{
			get
			{
				return m_renderStrategy.Bitmap;
			}
			set
			{
				m_sizeTotal = new EPoint(value.Width, value.Height);
				m_renderStrategy.Bitmap = value;
			}
		}

        public Endogine.BitmapHelpers.Canvas Canvas
        {
            get
            {
                if (this._canvas==null)
                    this._canvas = Endogine.BitmapHelpers.Canvas.Create(this.m_renderStrategy.PixelDataProvider);
                return this._canvas;
            }
        }

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MemberSpriteBitmapRenderStrategy RenderStrategy
		{
			get {return this.m_renderStrategy;}
		}

		public void Load(string a_sFilename)
		{
			Bitmap bmp = this.LoadIntoBitmap(a_sFilename);
			this.Bitmap = bmp;
		}
	}
}
