using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Endogine.ResourceManagement;

namespace Endogine
{
	/// <summary>
	/// Summary description for SpriteRenderDDStrategy.
	/// </summary>
	public class SpriteRenderGDIStrategy: SpriteRenderStrategy
	{
		public SpriteRenderGDIStrategy()
		{}

		public override void Dispose()
		{
		}

		public override void Init()
		{
			if (m_endogine.Stage != null && m_endogine.Stage.RootSprite != null)
			{
				m_sp.DrawToSprite = m_endogine.Stage.RootSprite;
			}
		}

		public override void SetColor(Color a_clr)
		{
		}
		
		public override void SetMember(MemberBase a_mb)
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

		public override void SubDraw() 
		{
			ERectangleF rctDraw = m_sp.CalcRectInDrawTarget();
			
			//attribs.SetColorMatrix(new ColorMatrix(), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);


			if (m_sp.Ink == RasterOps.ROPs.Copy || m_sp.Ink == RasterOps.ROPs.BgTransparent || m_sp.DrawToSprite == null) //TODO: allow RasterOps on root sprite.
			{
				if (m_sp.Rect.Width <= 0 || m_sp.Rect.Height <= 0)
					return;

				PointF ulCorner1 = new PointF(rctDraw.X, rctDraw.Y);
				PointF urCorner1 = new PointF(rctDraw.OppositeX, rctDraw.Y);
				PointF llCorner1 = new PointF(rctDraw.X, rctDraw.OppositeY);
				PointF[] destPara1 = {ulCorner1, urCorner1, llCorner1};

				ERectangle rctSrc = m_sp.SourceRect; //m_sp.Member.GetRectForFrame(m_sp.MemberAnimationFrame);
				//RectangleF rctfCropped = m_sp.GetPortionOfMemberToDisplay();
			
				//g.FillRectangle(new SolidBrush(Color.Red), rctDraw);

				Graphics g = Graphics.FromImage(m_sp.DrawToSprite.Member.Bitmap);
				ImageAttributes attribs = new ImageAttributes();
				attribs.SetWrapMode(WrapMode.Tile);
				if (m_sp.Ink == RasterOps.ROPs.BgTransparent)
					attribs.SetColorKey(m_sp.Member.ColorKey, m_sp.Member.ColorKey);

				g.SmoothingMode = SmoothingMode.None;
				g.CompositingMode = CompositingMode.SourceOver;
				g.CompositingQuality = CompositingQuality.Invalid;
				g.DrawImage(m_sp.Member.Bitmap, destPara1, rctSrc.ToRectangleF(), GraphicsUnit.Pixel, attribs);
				g.Dispose();
			}
			else
			{
				//since it's difficult to write a RasterOp algorithm that both does effects and scales/interpolates properly,
				//I cheat by creating a temporary scaled bitmap
				if (m_sp.Rect.ToERectangle().Width <= 0 || m_sp.Rect.ToERectangle().Height <= 0)
					return;

				Bitmap bmp = m_sp.Member.Bitmap;
				ERectangle rctSrc = m_sp.SourceRect;
				if (m_sp.Scaling.X != 1 || m_sp.Scaling.Y != 1 || m_sp.Color != Color.White)
				{
					//TODO: other/faster resizing algorithms at
					//http://www.codeproject.com/csharp/ImgResizOutperfGDIPlus.asp
					rctSrc = m_sp.Rect.ToERectangle();
					rctSrc.Offset(-rctSrc.X, -rctSrc.Y);
					bmp = new Bitmap(m_sp.Rect.ToERectangle().Width, m_sp.Rect.ToERectangle().Height, m_sp.Member.Bitmap.PixelFormat); //m_sp.Member.Bitmap, new Size(m_sp.RectInt.Width, m_sp.RectInt.Height));
					Graphics g = Graphics.FromImage(bmp);
					ImageAttributes attribs = new ImageAttributes();

					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)m_sp.Color.R/255;
					colorMatrix.Matrix11 = (float)m_sp.Color.G/255;
					colorMatrix.Matrix22 = (float)m_sp.Color.B/255;
					colorMatrix.Matrix33 = 1.00f; // alpha
					colorMatrix.Matrix44 = 1.00f; // w
					attribs.SetColorMatrix(colorMatrix);

					g.DrawImage(m_sp.Member.Bitmap, rctSrc.ToRectangle(), 
						m_sp.SourceRect.X, m_sp.SourceRect.Y, m_sp.SourceRect.Width, m_sp.SourceRect.Height, 
						GraphicsUnit.Pixel, attribs);
					g.Dispose();
				}

				RasterOps.CopyPixels(m_sp.DrawToSprite.Member.Bitmap, bmp,
					rctDraw, rctSrc, m_sp.DrawToSprite.SourceRect, (int)m_sp.Ink, m_sp.Blend);
			}
		}
	}
}
