using System;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for Player.
	/// </summary>
	public class Player : Sprite
	{
		private KeysSteering m_keys;
		private PlayerShot m_shot; //only allow one shot at a time!
		private int m_nExplodeCnt;

		public Player()
		{
			this.Color = GameMain.Instance.m_clrOffwhite;

			this.m_keys = new KeysSteering(KeysSteering.KeyPresets.ArrowsSpace);
            this.m_keys.AddKeyPreset(KeysSteering.KeyPresets.awsdCtrlShift);
			this.m_keys.KeyEvent+=new KeyEventHandler(m_keys_KeyEvent);

            this.SetGraphics("Player");
			this.CenterRegPoint();

			ERectangleF rct = ERectangleF.FromLTRB(145,418,495,419);
			Endogine.GameHelpers.BhConstrain bh = new Endogine.GameHelpers.BhConstrain();
			bh.AddArea(rct, null, false, null);
			this.AddBehavior(bh);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}

		public void Explode()
		{
            this.SetGraphics("PlayerExplosion");
            this.Animator.Animator.StepSize = 0.2f;
         	m_nExplodeCnt = 24;
		}

		public override void EnterFrame()
		{
			if (m_nExplodeCnt > 0)
			{

			}
			else
			{
				EPointF pntMove = new EPointF();
				if (this.m_keys.GetKeyActive("left"))
					pntMove.X = -2;
				else if (this.m_keys.GetKeyActive("right"))
					pntMove.X = 2;

				this.Move(pntMove);
			}
			base.EnterFrame ();
		}

		public bool CheckCollision(Sprite sp)
		{
			if (m_nExplodeCnt > 0)
				return false; //can't be hit again while burning

			EPointF pntHit = this.GetCollisionPoint(sp);
			if (pntHit == null)
				return false;
			this.Explode();
			return true;
		}
		private void m_keys_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			if (!bDown)
				return;
			if (this.m_keys.GetActionForKey(e.KeyCode) == "up") //action
			{
				//shoot!
				if (this.m_shot == null || this.m_shot.Disposing)
				{
					this.m_shot = new PlayerShot(this.Loc);
					this.m_shot.LocZ = this.LocZ;
				}
			}
		}
	}
}
