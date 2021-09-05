using System;
using System.Drawing;
using Tao.OpenGl;
using Endogine.ResourceManagement;

namespace Endogine.Renderer.OpenGL
{
	/// <summary>
	/// Summary description for SpriteRenderDDStrategy.
	/// </summary>
	public class SpriteRenderStrategyA: SpriteRenderStrategy
	{
		public SpriteRenderStrategyA()
		{
		}

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

//			Matrix QuadMatrix = Matrix.Scaling(rctDraw.Width, rctDraw.Height, 1);

			EPointF pntRegOff = m_sp.RegPoint.ToEPointF()/new EPointF(m_sp.SourceRect.Width, m_sp.SourceRect.Height) * new EPointF(rctDraw.Width, rctDraw.Height);

//			QuadMatrix.Multiply(Matrix.Translation(-pntRegOff.X, pntRegOff.Y,0));
//			QuadMatrix.Multiply(Matrix.RotationZ(-m_sp.Rotation));
//			QuadMatrix.Multiply(Matrix.Translation(pntRegOff.X, -pntRegOff.Y,0));

			EPoint renderControlSize = new EPoint(800,600);
			EPointF pntLoc = new EPointF(rctDraw.X-renderControlSize.X/2, rctDraw.Y-renderControlSize.Y/2);

			//QuadMatrix.Multiply(Matrix.Translation(pntLoc.X, -pntLoc.Y, 0f));


			int tx = ((MemberSpriteBitmapRenderStrategyA)this.m_sp.Member.RenderStrategy).TextureId;
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, tx);

			//Gl.glRotatef(m_sp.Rotation, 0, 0, 1);

			Gl.glBegin(Gl.GL_QUADS);
			if (false)
			{
				Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-1, -1, 1);
				Gl.glTexCoord2f(1, 0); Gl.glVertex3f(1, -1, 1);
				Gl.glTexCoord2f(1, 1); Gl.glVertex3f(1, 1, 1);
				Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-1, 1, 1);
			}
			rctDraw = rctDraw*0.01f;
			rctDraw.Offset(-1f,-1f);
//			rctDraw.Y = 1f-rctDraw.Y;
			Gl.glTexCoord2f(0, 0); Gl.glVertex3f(rctDraw.Left, rctDraw.Top, 1);
			Gl.glTexCoord2f(1, 0); Gl.glVertex3f(rctDraw.Right, rctDraw.Top, 1);
			Gl.glTexCoord2f(1, 1); Gl.glVertex3f(rctDraw.Right, rctDraw.Bottom, 1);
			Gl.glTexCoord2f(0, 1); Gl.glVertex3f(rctDraw.Left, rctDraw.Bottom, 1);

			Gl.glEnd();
		}	
	}
}
