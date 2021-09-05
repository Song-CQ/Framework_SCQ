using System;
using System.Windows.Forms;
using System.Drawing;

namespace Endogine
{
	/// <summary>
	/// Summary description for Stage.
	/// </summary>
	public class StageGDI : StageBase
	{
		//protected Control target           = null;
	
		public StageGDI(Control RenderControl, EndogineHub a_endogine) : base(RenderControl, a_endogine)
		{
		}

		public override void Init()
		{
			this.CreateRootSprite(new ERectangle(0,0, this.ControlSize.X, this.ControlSize.Y));
			Bitmap bmp = new Bitmap(this.ControlSize.X, this.ControlSize.Y, this.m_renderControl.CreateGraphics());
			m_spRoot.Member = new MemberSpriteBitmap(bmp);
		}

		public override void Update()
		{
			Graphics g;

			bool bRenderAsText = true;

			g = Graphics.FromImage(m_spRoot.Member.Bitmap);
			g.Clear(Color);
			m_spRoot.EnterFrame();
			m_spRoot.Draw();


			bool bRenderASCII = false;

			if (bRenderASCII) //for fun: render graphics as ASCII art
			{
				string sAll = "";
				if (bRenderAsText)
				{
					string s = " .,:|IOMW";
					int nDivider = 255/(s.Length);
					for (int y = 0; y < m_spRoot.SourceRect.Height; y+=16)
					{
						for (int x = 0; x < m_spRoot.SourceRect.Width; x+=8)
						{
							Color clr = m_spRoot.Member.Bitmap.GetPixel(x,y);
							int nGray = (clr.R+clr.G+clr.B)/3;
							int n = nGray/nDivider;
							if (n >= s.Length) n = s.Length-1;
							sAll+=s[n];
						}
						sAll+="\n";
					}
					g.Clear(Color);
					Font font = new Font("Courier New", 10);
					g.DrawString(sAll, font, new SolidBrush(Color.White), 10,10);
				}
			}

			g = this.m_renderControl.CreateGraphics();
			g.DrawImage(m_spRoot.Member.Bitmap, new PointF(0,0));


			g.Dispose();

			ERectangle rct = m_spRoot.Rect.ToERectangle();
			Point pntScreenTopLeft = this.m_renderControl.PointToScreen(new System.Drawing.Point(0,0));
			rct.X+=pntScreenTopLeft.X;
			rct.Y+=pntScreenTopLeft.Y;

			this.m_renderControl.Invalidate();
		}
	}
}
