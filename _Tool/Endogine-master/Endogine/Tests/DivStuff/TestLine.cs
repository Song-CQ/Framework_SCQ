using System;
using System.Drawing;
using System.Drawing.Imaging;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for TestLine.
	/// </summary>
	public class TestLine : Sprite
	{
		public ERectangleF m_rct;
		public TestLine(EndogineHub a_endogine):base(a_endogine)
		{

		}

		public void SetLine(EPointF a_pnt1, EPointF a_pnt2)
		{
			m_rct = ERectangleF.FromLTRB(a_pnt1.X, a_pnt1.Y, a_pnt2.X, a_pnt2.Y);
			if (Member!=null)
				Member.Dispose();

			Loc = m_rct.Location;
			
			Bitmap bmp = new Bitmap((int)Math.Abs(m_rct.Width),(int)Math.Abs(m_rct.Height), PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(bmp);
			//Point pnt1 = a_pnt1;
			//if (a_pnt1.Y <= a_pnt2.Y && a_pnt1.X <= a_pnt2.X)
			g.DrawLine(new Pen(Color.White, 1), new Point(0,0), new Point((int)m_rct.Width, (int)m_rct.Height));
			g.Dispose();

			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			Member = mb;
		}
	}
}
