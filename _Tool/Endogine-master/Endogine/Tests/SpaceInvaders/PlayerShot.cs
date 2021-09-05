using System;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for PlayerShot.
	/// </summary>
	public class PlayerShot : Endogine.GameHelpers.GameSprite
	{
		public PlayerShot(EPointF pntStart)
		{
			this.Velocity = new EPointF(0,-4);
			this.Loc = pntStart;

            string anim = "PlayerShot";
            if (!EH.Instance.CastLib.FrameSets.Exists(anim))
                PicRef.CreatePicRefs("SpaceInv\\PlayerShot", 2, 2);
            Endogine.Animation.BhAnimator an = new Endogine.Animation.BhAnimator(this, anim);

			this.Color = GameMain.Instance.m_clrOffwhite;
			//TODO: never disposed properly
		}

		public override void EnterFrame()
		{
			base.EnterFrame ();
			if (this.LocY < 30)
				this.Dispose();
			else
			{
				foreach (Invader invader in GameMain.Instance.m_invadersGrid._invaders)
				{
					if (invader.CheckCollision(this))
					{
						this.Dispose();
						break;
					}
				}
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
