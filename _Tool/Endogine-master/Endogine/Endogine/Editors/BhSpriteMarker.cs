using System;
using System.Drawing;
using System.Collections;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for SpriteMarker.
	/// </summary>
	public class BhSpriteMarker : Behavior
	{
		protected int m_nCnt = 0;
		protected ArrayList m_aSprites;

		public BhSpriteMarker()
		{
			m_aSprites = new ArrayList();
			for (int i = 0; i < 4; i++)
			{
				Sprite sp = new Sprite();
				sp.MemberName = "Cross";
				sp.LocZ = 1000;
				m_aSprites.Add(sp);
			}
		}

		public override void Dispose()
		{
			foreach (Sprite sp in m_aSprites)
				sp.Dispose();
			base.Dispose();
		}
		protected override void EnterFrame()
		{
			base.EnterFrame();

			if (m_sp.Rect == null)
				return;

			ERectangleF rct = new ERectangleF(
				m_sp.ConvParentLocToRootLoc(m_sp.Rect.Location),
				m_sp.ConvParentLocToRootLoc(m_sp.Rect.Size));


			for (int i = 0; i < m_aSprites.Count; i++)
			{
				Sprite sp = (Sprite)m_aSprites[i];
				sp.Loc = GetWhereOnSides((float)((m_nCnt+100*i/m_aSprites.Count) % 100)/100, rct);
			}

			m_nCnt++;
		}

		protected EPointF GetWhereOnSides(float fPhase, ERectangleF rct)
		{
			float fCircum = rct.Width*2 + rct.Height*2;
			EPointF pnt = new EPointF();
			if (fPhase < rct.Width/fCircum)
			{
				pnt.X = rct.X+fPhase*fCircum;
				pnt.Y = rct.Y;
			}
			else
			{
				fPhase-= rct.Width/fCircum;
				if (fPhase < rct.Height/fCircum)
				{
					pnt.X = rct.Right;
					pnt.Y = rct.Y+fPhase*fCircum;
				}
				else
				{
					fPhase-= rct.Height/fCircum;
					if (fPhase < rct.Width/fCircum)
					{
						pnt.Y = rct.Bottom;
						pnt.X = rct.Right-fPhase*fCircum;
					}
					else
					{
						fPhase-= rct.Width/fCircum;
						pnt.X = rct.X;
						pnt.Y = rct.Bottom-fPhase*fCircum;
					}
				}
			}
			return pnt;
		}
	}
}
