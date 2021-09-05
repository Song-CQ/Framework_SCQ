using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for TestCircle.
	/// </summary>
	public class TestCircle : Sprite
	{
		private EPointF m_pntVel;
		private EPoint m_pntThrust;
		private ArrayList aKeysPressedX;
		private ArrayList aKeysPressedY;

		private ArrayList m_aLines;

		public TestCircle()
		{
			m_pntVel = new EPointF(0,0);
			m_pntThrust = new EPoint(0,0);

			aKeysPressedX = new ArrayList(); 
			aKeysPressedY = new ArrayList(); 

			Bitmap bmp = new Bitmap(40,40, PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(bmp);
			g.FillEllipse(new SolidBrush(Color.Red), 0,0,40,40);
			g.Dispose();
			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			mb.CenterRegPoint();
			Member = mb;

			m_aLines = new ArrayList();
			for (int i = 0; i < 1; i++)
			{
				TestLine line = new TestLine(m_endogine);
				line.SetLine(new EPointF((i)*150,0), new EPointF((i+1)*150,150));
				m_aLines.Add(line);
			}

			LocZ = 10;
			Loc = new EPointF(171,171);
			m_endogine.KeyEvent+=new KeyEventHandler(m_endogine_KeyEvent);
		}

		public override void EnterFrame()
		{
			base.EnterFrame();

			bool bCollided = false;
			m_pntVel.X = m_pntThrust.X*3;
			//m_pntVel.X = -4;
			m_pntVel.Y = m_pntThrust.Y*3;
			//m_pntVel.Y = -4;
			EPointF pntCollision;
			EPointF pntCircleAtCollision;
			EPointF pntNormal = new EPointF(0,0);
			for (int i = 0; i < m_aLines.Count; i++)
			{
				TestLine line = (TestLine)m_aLines[i];
				if (Endogine.Collision.Collision.CalcCircleLineCollision(Loc, SourceRect.Width/2, m_pntVel, line.m_rct, out pntCollision, out pntCircleAtCollision))
				{
					//pntCollision, out pntCircleAtCollision
					EPointF pntDiff = new EPointF(pntCircleAtCollision.X-pntCollision.X, pntCircleAtCollision.Y-pntCollision.Y);

					SpriteOneFrame sp = new SpriteOneFrame(m_endogine);
					sp.MemberName = "Cross";
					sp.Loc = pntCollision;
					sp.LocZ = 11;
					
					sp = new SpriteOneFrame(m_endogine);
					sp.MemberName = "Cross";
					sp.Loc = pntCircleAtCollision;
					sp.LocZ = 11;
					sp.Color = Color.Green;

					double dAngle = Math.Atan2(pntDiff.X, -pntDiff.Y);
					float fDist = 1;
					Loc = new EPointF(fDist*(float)Math.Sin(dAngle)+pntCircleAtCollision.X, -fDist*(float)Math.Cos(dAngle)+pntCircleAtCollision.Y);
					bCollided = true;
				}
			}
			if (!bCollided)
				Move(m_pntVel);
		}


		private void m_endogine_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			int nFact = bDown?1:0;
				switch (e.KeyCode)
				{
					case System.Windows.Forms.Keys.Left:
					case System.Windows.Forms.Keys.Right:
						if (bDown)
						{
							if (!aKeysPressedX.Contains(e.KeyCode))
								aKeysPressedX.Add(e.KeyCode);
						}
						else
							aKeysPressedX.Remove(e.KeyCode);
						break;
					case System.Windows.Forms.Keys.Up:
					case System.Windows.Forms.Keys.Down:
						if (bDown)
						{
							if (!aKeysPressedY.Contains(e.KeyCode))
								aKeysPressedY.Add(e.KeyCode);
						}
						else
							aKeysPressedY.Remove(e.KeyCode);
						break;
				}

			m_pntThrust.X = 0;
			if (aKeysPressedX.Count > 0)
				m_pntThrust.X = (System.Windows.Forms.Keys)aKeysPressedX[aKeysPressedX.Count-1] == System.Windows.Forms.Keys.Left?-1:1;

			m_pntThrust.Y = 0;
			if (aKeysPressedY.Count > 0)
				m_pntThrust.Y = (System.Windows.Forms.Keys)aKeysPressedY[aKeysPressedY.Count-1] == System.Windows.Forms.Keys.Up?-1:1;

		}
	}
}

