using System;
using System.Collections;
using Endogine;

namespace CaveHunter
{
	/// <summary>
	/// Summary description for Player.
	/// </summary>
	public class Player : Endogine.GameHelpers.GameSprite
	{
		private EPointF m_pntGravity = new EPointF(0,0.2f);
		protected KeysSteering m_keysSteering;
		private int m_nExplodingFrame = 0;
		private bool m_bDying = false;
		private Endogine.ParticleSystem.ParticleEmitter m_explosion;
		private Endogine.ParticleSystem.ParticleEmitter m_smoke;

		public Player()
		{
			this.MemberName = "CaveShip";
			this.Velocity+=new EPointF(2,0);
			this.LocZ = 10;
			float[] wallsY = GameMain.Instance.CaveWalls.GetWallsYOnX(0);
			this.LocY = wallsY[0]+30;

            m_keysSteering = new KeysSteering();
            m_keysSteering.AddActionAndKey("up", System.Windows.Forms.Keys.Up);
            m_keysSteering.AddActionAndKey("up", System.Windows.Forms.Keys.W);
            m_keysSteering.KeyEvent += new KeyEventHandler(m_keysSteering_KeyEvent);
		}

		public override void EnterFrame()
		{
			if (m_explosion != null)
			{
				m_nExplodingFrame++;
				if (m_nExplodingFrame <= 5)
					m_explosion.NumNewParticlesPerFrame = 1.0f*(1.0f - (float)m_nExplodingFrame/5);
				m_explosion.Loc = this.Loc;

//				m_explosion.Dispose();
//				m_explosion = null;
			}
			if (m_smoke != null)
				m_smoke.Loc = this.Loc;

			if (!m_bDying)
			{
				if (m_keysSteering.GetKeyActive("up"))
					this.Velocity-=m_pntGravity*2;
			}
			this.Velocity+=m_pntGravity;

			base.EnterFrame ();

			if (GameMain.Instance.CaveWalls.CheckCollision(this) || GameMain.Instance.Obstacles.CheckCollision(this))
			{
				if (m_explosion == null)
				{
					m_explosion = new Endogine.ParticleSystem.ParticleEmitter();
					m_explosion.Rect = new ERectangleF(0,0,20,20);
					m_explosion.SprayangleRange = (float)Math.PI*2;
					m_explosion.AddedVelocity.X = this.Velocity.X;
					m_explosion.Speed = 5;
					m_explosion.Gravity = m_pntGravity.Y;
                    //MemberSpriteBitmap mbParticle = (MemberSpriteBitmap)EndogineHub.Instance.CastLib.GetOrCreate("Particle");
                    //mbParticle.CenterRegPoint();
                    m_explosion.ParticlePicRef = PicRef.GetOrCreate("Particle");

					SortedList aColors = new SortedList();
					aColors.Add(0.0, System.Drawing.Color.FromArgb(255,255,0));
					aColors.Add(0.5, System.Drawing.Color.FromArgb(255,0,0));
					aColors.Add(1.0, System.Drawing.Color.FromArgb(0,0,0));
					m_explosion.SetColorList(aColors);

					SortedList aSizes = new SortedList();
					aSizes.Add(0.0, 0.3);
					m_explosion.SetSizeList(aSizes);

					m_explosion.NumNewParticlesPerFrame = 10;
				}
				m_explosion.Loc = this.Loc;
				if (m_nExplodingFrame > 6)
					m_explosion.NumNewParticlesPerFrame = 10;
				m_nExplodingFrame = 1;

				this.LocY -= this.Velocity.Y*2;
				this.Velocity.Y*=-0.7f;
				
				if (!m_bDying)
				{
					m_bDying = true;

					m_smoke = new Endogine.ParticleSystem.ParticleEmitter();
                    m_smoke.ParticlePicRef = PicRef.GetOrCreate("Particle");
					m_smoke.Rect = new ERectangleF(0,0,20,20);
					m_smoke.SprayangleRange = (float)Math.PI*2;
					m_smoke.AddedVelocity.Y = -1;
					m_smoke.Speed = 0;
					m_smoke.Gravity = 0;
					m_smoke.NumNewParticlesPerFrame = 0.4f;

					SortedList aColors = new SortedList();
					aColors.Add(0.0, System.Drawing.Color.FromArgb(127,127,127));
					aColors.Add(1.0, System.Drawing.Color.FromArgb(0,0,0));
					m_smoke.SetColorList(aColors);

					SortedList aSizes = new SortedList();
					aSizes.Add(0.0, 1.0);
					m_smoke.SetSizeList(aSizes);
				}
			}
		}

        private void m_keysSteering_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
        {
			if (!m_bDying)
				return;

			if (e.KeyCode == System.Windows.Forms.Keys.Space)
				GameMain.Instance.Start();
		}

		public override void Dispose()
		{
			if (m_smoke!=null)
				m_smoke.Dispose();
			if (m_explosion!=null)
				m_explosion.Dispose();

			base.Dispose ();
		}

	}
}
