using System;
using Endogine;
using Endogine.GameHelpers;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for Invader.
	/// </summary>
	public class Invader : GameSprite
	{
		private bool m_bExploding;

		public Invader()
		{
			this.Color = GameMain.Instance.m_clrOffwhite;
		}

		public override void Move(EPointF a_pnt)
		{
			base.Move (a_pnt);

			if (this.m_bExploding)
			{
				this.Dispose();
			}

            if (this.Animator != null)
            {
                this.Animator.Animator.StepSize = 1;
                this.Animator.Animator.Step();
                this.Animator.Animator.StepSize = 0;
            }
		}

		public void Fire()
		{
			InvaderShot shot = new InvaderShot();
			shot.Loc = this.Loc;
			shot.LocZ = this.LocZ;
		}

		public void Explode()
		{
			this.m_bExploding = true;
            this.SetGraphics("InvaderExplosion");
			//this.MemberName = "SpaceInv\\InvaderExplosion";
		}

		public bool CheckCollision(Sprite sp)
		{
			if (this.m_bExploding)
				return false; //can't be hit twice.

			EPointF pntHit = this.GetCollisionPoint(sp);
			if (pntHit == null)
				return false;
			this.Explode();
			return true;
		}

	}
}
