using System;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for LivesLeft.
	/// </summary>
	public class LivesLeft
	{
		private Sprite m_sp;

		public LivesLeft()
		{
			this.m_sp = new Sprite();
			this.m_sp.SetGraphics("Stats");
			this.m_sp.Color = GameMain.Instance.m_clrOffwhite;
			this.m_sp.Loc = new EPointF(96,448);
		}

		public void Dispose()
		{
			this.m_sp.Dispose();
		}
	}
}
