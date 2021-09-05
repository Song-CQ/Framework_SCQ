using System;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for Shot.
	/// </summary>
	public class Shot : Endogine.GameHelpers.GameSprite
	{
		private GameMain m_gameMain;
		private int m_nLife = 250;

		public Shot(GameMain a_gameMain)
		{
			m_gameMain = a_gameMain;
			MemberName = "Particle";
		}

		public override void EnterFrame()
		{
			base.EnterFrame();

			//remove shot after a while:
			m_nLife--;
			if (m_nLife == 0)
			{
				Dispose();
				return;
			}

			//collision detection with asteroids:
			foreach (Asteroid sp in m_gameMain.Asteroids)
			{
				if (sp.Rect.IntersectsWith(this.Rect))
				{
					Dispose();
					sp.Hit();
					return;
				}
			}
		}
	}
}
