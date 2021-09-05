using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Endogine.ResourceManagement;

namespace Endogine.Renderer.GDI
{
	/// <summary>
	/// Summary description for SpriteRenderDDStrategy.
	/// </summary>
	public class SpriteRenderStrategyA: SpriteRenderStrategy
	{
		public SpriteRenderStrategyA()
		{}

		public override void Dispose()
		{
		}

		public override void Init()
		{
			if (m_endogine.Stage != null && m_endogine.Stage.RootSprite != null)
			{
				_sp.DrawToSprite = m_endogine.Stage.RootSprite;
			}
		}

		public override void SetColor(Color a_clr)
		{
		}
		
        //TODO: a lot to fix here so it's more independent of Endogine's sprite system


  		public override void SetMember(MemberBase a_mb)
		{
		}
        public override void SetPixelDataProvider(Endogine.BitmapHelpers.PixelDataProvider pdp)
        {
        }
		public override void SetMemberAnimationFrame(int a_n)
		{
		}
//		public override void SetSourceRect(ERectangle rct)
//		{
//		}
		public override void RecalcedParentOutput()
		{
		}


		public override void EnterFrame()
		{
		}

        public override void CalcRenderRegion(ERectangleF rctDrawTarget, float rotation, EPoint regPoint, EPoint sourceRectSize)
        {
            //this._matrix = this.CreateMatrix(rctDrawTarget, rotation, regPoint, sourceRectSize);
        }

        protected override void SetSourceClipRect(ERectangleF rct)
        {
            //TODO: replace _sp.SourceRect in SubDraw()
        }

		public override void SubDraw() 
		{
			ERectangleF rctDraw = _sp.CalcRectInDrawTarget();
			
			//attribs.SetColorMatrix(new ColorMatrix(), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);


			if (_sp.Ink == RasterOps.ROPs.Copy || _sp.Ink == RasterOps.ROPs.BgTransparent || _sp.DrawToSprite == null) //TODO: allow RasterOps on root sprite.
            //if (false)
			{
				if (_sp.Rect.Width <= 0 || _sp.Rect.Height <= 0)
					return;

				PointF ulCorner1 = new PointF(rctDraw.X, rctDraw.Y);
				PointF urCorner1 = new PointF(rctDraw.OppositeX, rctDraw.Y);
				PointF llCorner1 = new PointF(rctDraw.X, rctDraw.OppositeY);
				PointF[] destPara1 = {ulCorner1, urCorner1, llCorner1};

				ERectangle rctSrc = _sp.SourceRect; //m_sp.Member.GetRectForFrame(m_sp.MemberAnimationFrame);
				//RectangleF rctfCropped = m_sp.GetPortionOfMemberToDisplay();
			
				//g.FillRectangle(new SolidBrush(Color.Red), rctDraw);

				Graphics g = Graphics.FromImage(_sp.DrawToSprite.Member.Bitmap);
				ImageAttributes attribs = new ImageAttributes();
				attribs.SetWrapMode(WrapMode.Tile);
				if (_sp.Ink == RasterOps.ROPs.BgTransparent)
					attribs.SetColorKey(_sp.Member.ColorKey, _sp.Member.ColorKey);

				g.SmoothingMode = SmoothingMode.None;
				g.CompositingMode = CompositingMode.SourceOver;
				g.CompositingQuality = CompositingQuality.Invalid;
				g.DrawImage(_sp.Member.Bitmap, destPara1, rctSrc.ToRectangleF(), GraphicsUnit.Pixel, attribs);
				g.Dispose();
			}
			else
			{
				//since it's difficult to write a RasterOp algorithm that both does effects and scales/interpolates properly,
				//I cheat by creating a temporary scaled bitmap
				if (_sp.Rect.ToERectangle().Width <= 0 || _sp.Rect.ToERectangle().Height <= 0)
					return;

				Bitmap bmp = _sp.Member.Bitmap;
				ERectangle rctSrc = _sp.SourceRect;
				if (_sp.Scaling.X != 1 || _sp.Scaling.Y != 1 || _sp.Color != Color.White)
				{
					//TODO: other/faster resizing algorithms at
					//http://www.codeproject.com/csharp/ImgResizOutperfGDIPlus.asp
					rctSrc = _sp.Rect.ToERectangle();
					rctSrc.Offset(-rctSrc.X, -rctSrc.Y);
					bmp = new Bitmap(_sp.Rect.ToERectangle().Width, _sp.Rect.ToERectangle().Height, _sp.Member.Bitmap.PixelFormat); //m_sp.Member.Bitmap, new Size(m_sp.RectInt.Width, m_sp.RectInt.Height));
					Graphics g = Graphics.FromImage(bmp);
					ImageAttributes attribs = new ImageAttributes();

					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)_sp.Color.R/255;
					colorMatrix.Matrix11 = (float)_sp.Color.G/255;
					colorMatrix.Matrix22 = (float)_sp.Color.B/255;
					colorMatrix.Matrix33 = 1.00f; // alpha
					colorMatrix.Matrix44 = 1.00f; // w
					attribs.SetColorMatrix(colorMatrix);

					g.DrawImage(_sp.Member.Bitmap, rctSrc.ToRectangle(), 
						_sp.SourceRect.X, _sp.SourceRect.Y, _sp.SourceRect.Width, _sp.SourceRect.Height, 
						GraphicsUnit.Pixel, attribs);
					g.Dispose();
				}

				RasterOps.CopyPixels(_sp.DrawToSprite.Member.Bitmap, bmp,
					rctDraw, rctSrc, _sp.DrawToSprite.SourceRect, (int)_sp.Ink, _sp.Blend);
			}
		}
	}
}
