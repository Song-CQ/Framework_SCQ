using System;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for Asteroid.
	/// </summary>
	public class Asteroid : WrappingSprite
	{
		protected int m_nSize;
		protected GameMain m_gameMain;

		public Asteroid(GameMain a_gameMain, int a_nSize)
		{
			this.WrapRect = new ERectangleF(new EPointF(0,0), EndogineHub.Instance.Stage.Size.ToEPointF()) + new ERectangleF(-80,-80,160,160);

			m_gameMain = a_gameMain;
			m_gameMain.Asteroids.Add(this);
			m_nSize = a_nSize;

            Endogine.Animation.BhAnimator bh = new Endogine.Animation.BhAnimator();
            bh.FrameSet = "asteroid01";
            bh.Parent = this;
            bh.Animator.Mode = Endogine.Animation.Animator.Modes.Loop;
            
			this.Scaling = new EPointF(2,2)*(1f/(float)(4-m_nSize));
            //this.TextureFilter = TextureFilters.High;
		}

		public override void Dispose()
		{
			base.Dispose ();
			m_gameMain.Asteroids.Remove(this);
		}

		public void Hit()
		{
			if (m_nSize > 1)
			{
				//create two smaller asteroids
				Asteroid part;
				part = new Asteroid(m_gameMain, m_nSize-1);
				part.Velocity.Angle = this.Velocity.Angle+(float)Math.PI/2;
				part.Velocity.Length = this.Velocity.Length*1.5f;
				part.Loc = this.Loc + part.Velocity*30*(m_nSize-1);

				part = new Asteroid(m_gameMain, m_nSize-1);
				part.Velocity.Angle = this.Velocity.Angle-(float)Math.PI/2;
				part.Velocity.Length = this.Velocity.Length*1.5f;
				part.Loc = this.Loc + part.Velocity*30*(m_nSize-1);
			}
			this.Dispose();
		}
	}
}
