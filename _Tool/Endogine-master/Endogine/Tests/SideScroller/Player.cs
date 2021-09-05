using System;
using System.Collections;
using System.Collections.Generic;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for Player.
	/// </summary>
	public class Player : Endogine.GameHelpers.GameSprite
	{
		protected KeysSteering m_keysSteering;
		private float m_fAngleStep = 0;

		private float m_fThrustPower = 0.1f;
		private float m_fThrustParticles;

		private GameMain m_gameMain;

		private Endogine.ParticleSystem.ParticleEmitter m_particleSystem;

		public Player(GameMain a_gameMain)
		{
			m_gameMain = a_gameMain;

			this.Name = "Player";
			m_fAngleStep = (float)(5.0*Math.PI/180);
			MemberName = "Ship";
			this.RegPoint = (this.Member.Size.ToEPointF()*0.5f).ToEPoint();

			#region Set up particle system (for thrusters)
			m_particleSystem = new Endogine.ParticleSystem.ParticleEmitter();
			//m_particleSystem.Parent = this;
			m_particleSystem.LocZ = LocZ-1;

			SortedList aColors = new SortedList();
			aColors.Add(0.0, System.Drawing.Color.FromArgb(255,255,0));
			aColors.Add(0.5, System.Drawing.Color.FromArgb(255,0,0));
			aColors.Add(1.0, System.Drawing.Color.FromArgb(0,0,0));
			m_particleSystem.SetColorList(aColors);

			SortedList aSizes = new SortedList();
			aSizes.Add(0.0, 1.0);
			aSizes.Add(1.0, 0.0);
			m_particleSystem.SetSizeList(aSizes);

			m_particleSystem.MaxParticles = 100;
			m_particleSystem.NumNewParticlesPerFrame = 0;

			m_fThrustParticles = 2;
			
			m_particleSystem.Gravity = 0;
			m_particleSystem.Speed = 5;
			m_particleSystem.SizeFact = 0.3f;

            m_particleSystem.ParticlePicRef = PicRef.GetOrCreate("Particle");
			m_particleSystem.SourceRect = new ERectangle(0,0,10,10);
			m_particleSystem.RegPoint = new EPoint(5,5);
			m_particleSystem.LocZ = 100;
			#endregion

			#region Keys setup
            m_keysSteering = new KeysSteering();
            this.m_keysSteering.AddKeyPreset(KeysSteering.KeyPresets.ArrowsSpace);
            this.m_keysSteering.AddKeyPreset(KeysSteering.KeyPresets.awsdCtrlShift);
            this.m_keysSteering.AddPair("left", "right");
            this.m_keysSteering.AddPair("up", "down");
            //m_keysSteering.ReceiveEndogineKeys(m_endogine);
            m_keysSteering.KeyEvent+=new KeyEventHandler(m_keysSteering_KeyEvent);
			#endregion
		}

		public override void EnterFrame()
		{
			if (m_keysSteering.GetKeyActive("left"))
				Rotation-=m_fAngleStep;
			else if (m_keysSteering.GetKeyActive("right"))
				Rotation+=m_fAngleStep;
			
			float fThrust = 0;
			if (m_keysSteering.GetKeyActive("up"))
			{
				m_particleSystem.NumNewParticlesPerFrame = m_fThrustParticles;
				fThrust = m_fThrustPower;
			}
			else if (m_keysSteering.GetKeyActive("down"))
				fThrust = -m_fThrustPower;

			if (fThrust <= 0)
				m_particleSystem.NumNewParticlesPerFrame = 0;

			float fRot = Rotation;
			Velocity+=new EPointF((float)Math.Sin(fRot), (float)-Math.Cos(fRot))*fThrust;

			base.EnterFrame();

			//TODO: should be able to just set the particleSystem's parent to the player, but inherited rotation isn't implemented yet.
			m_particleSystem.Loc = Loc;
			m_particleSystem.Rotation = Rotation+(float)Math.PI;
			m_particleSystem.AddedVelocity = Velocity;

			m_endogine.Stage.Camera.CenterLoc = this.Loc;

			//collision detection with asteroids:
			foreach (Asteroid sp in m_gameMain.Asteroids)
			{
				if (sp.Rect.IntersectsWith(this.Rect))
				{
					//TODO: lose a life!
					sp.Hit();
					break;
				}
			}
		}

		private void m_keysSteering_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			if (!bDown)
				return;

			if (m_keysSteering.GetActionForKey(e.KeyCode) == "shoot")
			{
				Shot shot = new Shot(m_gameMain);
				EPointF pntVel = new EPointF((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation))*5;
				shot.Velocity = pntVel + Velocity;
				shot.Loc = this.Loc;
			}
		}
	}
}
