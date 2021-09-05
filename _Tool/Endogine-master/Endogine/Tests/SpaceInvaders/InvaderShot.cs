using System;
using System.Collections.Generic;
using Endogine;
using Endogine.GameHelpers;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for InvaderShot.
	/// </summary>
	public class InvaderShot : GameSprite
	{
		public InvaderShot()
		{
            this.SetGraphics("shot01");
			this.Velocity = new EPointF(0,4);
			this.Color = GameMain.Instance.m_clrOffwhite;
		}

		public override void EnterFrame()
		{
			base.EnterFrame ();

			if (this.LocY > 420)
				this.Dispose();
			else if (this.LocY > 390)
			{
				if (GameMain.Instance.m_player.CheckCollision(this))
					this.Dispose();
			}
			else if (this.LocY > 340)
			{
				foreach (Cover cover in GameMain.Instance.m_covers)
				{
					if (cover.CheckCollision(this))
					{
						this.Dispose();
						break;
					}
				}
			}
		}
	}
}
