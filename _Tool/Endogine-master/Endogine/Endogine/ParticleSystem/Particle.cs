using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace Endogine.ParticleSystem
{
	/// <summary>
	/// Summary description for Particle.
	/// </summary>
	public class Particle : Endogine.GameHelpers.GameSprite
	{
		public float RotationRange;
		protected EPointF m_pntAcc;
		protected int m_nFrameCnt = 0;
		protected int m_nMaxFrames = 0;
		protected float m_fSizeFact = 1;
		protected ParticleEmitter m_psys;

		public Particle(ParticleEmitter a_psys):base()
		{
			Name = "Particle";
			m_psys = a_psys;
			this.Velocity = new EPointF(0,0);
			UpdateColorAndSize();
		}

		public override void Dispose()
		{
			base.Dispose ();
		}


		public int LifeCounter
		{
			get {return m_nFrameCnt;}
			set {m_nFrameCnt = value;}
		}
		public int LifeMax
		{
			get {return m_nMaxFrames;}
			set {m_nMaxFrames = value;}
		}

		public float SizeFact
		{
			set {m_fSizeFact = value;}
		}

		public override void EnterFrame()
		{
			base.EnterFrame();
			LifeCounter++;

			//Move(new EPointF(
			//	((float)m_psys.Random.NextDouble()*2f-1f)*m_psys.Chaos,
			//	((float)m_psys.Random.NextDouble()*2f-1f)*m_psys.Chaos));

			UpdateColorAndSize();
		}

		private void UpdateColorAndSize()
		{
			float l=(float)LifeCounter/(float)LifeMax;

			Color = m_psys.ColorInterpolator.GetValueAtTime(l);

			float fSize = (float)m_psys.SizeInterpolator.GetValueAtTime(l);
			fSize*=m_fSizeFact;
			Scaling = new EPointF(fSize,fSize);
			//Blend = (int)((((float)clr0.A)*(1f-l)+((float)clr1.A)*l));
		}
	}
}

