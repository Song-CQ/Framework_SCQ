using System;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for Score.
	/// </summary>
	public class Score
	{
		private Sprite m_sp;

		public Score()
		{
			this.m_sp = new Sprite();
			this.m_sp.SetGraphics("ScoreText");
			this.m_sp.Color = GameMain.Instance.m_clrOffwhite;
			this.m_sp.Loc = new EPointF(188,17);
		}

		public void Dispose()
		{
			this.m_sp.Dispose();
		}
	}
}
