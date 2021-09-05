using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml;

using Endogine.Interpolation;
using Endogine.ResourceManagement;


namespace Endogine.ParticleSystem
{
	/// <summary>
	/// Based on code from
	/// http://www.programmersheaven.com/zone30/cat848/28782.htm
	/// by Gustavo Arranhado
	/// </summary>
	public class ParticleEmitter : Sprite
	{
        PicRef _particlePicRef;
		//private MemberSpriteBitmap m_mbParticle;
		private float m_fNewParticlesPerFrame;
		private int m_nMaxNumParticles;

		//private SizeF size,sizerange;
		private float sprayangle,sprayanglerange,gravityangle,windangle;
		private float speed,speedrange,gravity,wind;
		private float chaos,windchaos;
		private float rotationrange;
		private int life,liferange;
		private ArrayList particlelist;
		private Random rnd=new Random();

		protected float m_particleSizeFact, m_particleSizeFactRange;
		protected Endogine.Interpolation.InterpolatorColor m_interpolator;
		protected Endogine.Interpolation.Interpolator m_interpolatorSize;

		protected EPointF m_pntAddedVelocity;

		public ParticleEmitter()
		{
			m_bMeInvisibleButNotChildren = true;
			Name = "ParticleEmitter";
			m_endogine = EndogineHub.Instance;
			particlelist=new ArrayList();
			this.Ink = RasterOps.ROPs.AddPin;

			SortedList aColors = new SortedList();
			aColors.Add(0.0, Color.FromArgb(255,255,255));
			aColors.Add(1.0, Color.FromArgb(0,0,0));
			m_interpolator = new InterpolatorColor(aColors);

			SortedList aSize = new SortedList();
			aSize.Add(0.0, 1.0);
			aSize.Add(1.0, 0.0);
			m_interpolatorSize = new Interpolator();
			m_interpolatorSize.KeyFramesList = aSize;

			m_particleSizeFact = 1;
			m_particleSizeFactRange = 0.5f;

			chaos=0f;

			m_fNewParticlesPerFrame = 1;
			m_nMaxNumParticles = 200;

			rotationrange=90f;

			m_pntAddedVelocity = new EPointF();

			life = 50;
			liferange = 20;
			sprayangle = 0f;
			sprayanglerange = 0.0f;
			speed = 3f;
			speedrange = 0.5f;
			gravity = 0.3f;
			gravityangle = 0f;
			wind = 0f;
			windangle = 0f;
		}

		public override void Dispose()
		{
			for(int i = particlelist.Count-1; i>=0; i--)
			{
				Particle particle=(Particle)particlelist[i];
				particle.Dispose();
			}
			particlelist.Clear();
			m_interpolator.Dispose();
			m_interpolatorSize.Dispose();
	
			base.Dispose();
		}

		public void Reset()
		{
			particlelist=new ArrayList();
		}

		public override void EnterFrame()
		{
			base.EnterFrame();
			Update();
		}

		public void Update()
		{
			for(int i = particlelist.Count-1; i>=0; i--)
			{
				Particle particle=(Particle)particlelist[i];
				if(particle.LifeCounter<particle.LifeMax)
				{
					float a=windangle+((float)rnd.NextDouble()*2f-1f)*windchaos;
					particle.Velocity+=new EPointF(
						((float)Math.Sin(gravityangle)*gravity+(float)Math.Sin(a)*wind),
						((float)Math.Cos(gravityangle)*gravity+(float)Math.Cos(a)*wind));
				}
				else
				{
					particlelist.Remove(particle);
					particle.Dispose();
				}
			}

			int nNumNew = (int)m_fNewParticlesPerFrame;
			float fMaybeNew = m_fNewParticlesPerFrame-nNumNew;
			if (rnd.Next(1000) < 1000*fMaybeNew)
				nNumNew++;

			for(int i=0;i<nNumNew;i++)
			{
				Particle particle=new Particle(this);
				//particle.Member = m_mbParticle;
                particle.PicRef = this._particlePicRef;
				particle.Ink = Ink;
				particle.LocZ = LocZ;
                particle.TextureFilter = TextureFilters.Low;
				particle.SizeFact = m_particleSizeFact+((float)rnd.NextDouble()*m_particleSizeFactRange/2f);
				//particle.Scaling=new PointF(size.Width+((float)rnd.NextDouble()*sizerange.Width-sizerange.Width/2f),size.Height+((float)rnd.NextDouble()*sizerange.Height-sizerange.Height/2f));
				particle.LocX = Rect.Location.X + Rect.Width*(float)rnd.NextDouble();
				particle.LocY = Rect.Location.Y + Rect.Height*(float)rnd.NextDouble();

				float fStartAngle = Rotation + ((float)rnd.NextDouble()*sprayanglerange-sprayanglerange/2f);
				float fNewSpeed = speed+((float)rnd.NextDouble()*speedrange-speedrange/2f);
				particle.Velocity = new EPointF((float)Math.Sin(fStartAngle),-(float)Math.Cos(fStartAngle))*fNewSpeed
					+ m_pntAddedVelocity;
				//TODO: 3D render strategy has problems with rotated sprites, don't use it here:
				//particle.Rotation = fStartAngle;
				particle.LifeMax=life+rnd.Next(-liferange/2,liferange/2);
				particlelist.Add(particle);
			}
		}
		#region Properties
		public float SizeFact
		{
			set {m_particleSizeFact = value;}
		}
		public void SetColorList(SortedList a_aColorsToInterpolate)
		{
			m_interpolator = new InterpolatorColor(a_aColorsToInterpolate);
		}
		public void SetSizeList(SortedList a_aSizesToInterpolate)
		{
			m_interpolatorSize = new Interpolator();
			m_interpolatorSize.KeyFramesList = a_aSizesToInterpolate;
		}


		public PicRef ParticlePicRef
		{
            get { return this._particlePicRef; }
            set { this._particlePicRef = value; }
		}
		public Random Random
		{
			get {return rnd;}
		}

		public Endogine.Interpolation.InterpolatorColor ColorInterpolator
		{
			get {return this.m_interpolator;}
		}
		public Endogine.Interpolation.Interpolator SizeInterpolator
		{
			get {return this.m_interpolatorSize;}
		}

		public EPointF AddedVelocity
		{
			get {return m_pntAddedVelocity;}
			set {m_pntAddedVelocity = value;}
		}
		public int MaxParticles
		{
			set
			{
				m_nMaxNumParticles=value;
			}
			get
			{
				return m_nMaxNumParticles;
			}
		}
		public float NumNewParticlesPerFrame
		{
			set
			{
				m_fNewParticlesPerFrame=value;
			}
			get
			{
				return m_fNewParticlesPerFrame;
			}
		}
		public float AngleOffset
		{
			set
			{
				sprayangle=value;
			}
			get
			{
				return sprayangle;
			}
		}
		public float SprayangleRange
		{
			set
			{
				sprayanglerange=value;
			}
			get
			{
				return sprayanglerange;
			}
		}
		public float Speed
		{
			set
			{
				speed=value;
			}
			get
			{
				return speed;
			}
		}
		public float SpeedRange
		{
			set
			{
				speedrange=value;
			}
			get
			{
				return speedrange;
			}
		}
		public float Chaos
		{
			set
			{
				chaos=value;
			}
			get
			{
				return chaos;
			}
		}
//		public override float Rotation
//		{
//			set
//			{
//				rotation=value;
//			}
//			get
//			{
//				return rotation;
//			}
//		}
		public float RotationRange
		{
			set
			{
				rotationrange=value;
			}
			get
			{
				return rotationrange;
			}
		}
		public int Life
		{
			set
			{
				life=value;
			}
			get
			{
				return life;
			}
		}
		public int LifeRange
		{
			set
			{
				liferange=value;
			}
			get
			{
				return liferange;
			}
		}
		public float Gravity
		{
			set
			{
				gravity=value;
			}
			get
			{
				return gravity;
			}
		}
		public float GravityAngle
		{
			set
			{
				gravityangle=value;
			}
			get
			{
				return gravityangle;
			}
		}
		public float Wind
		{
			set
			{
				wind=value;
			}
			get
			{
				return wind;
			}
		}
		public float WindAngle
		{
			set
			{
				windangle=value;
			}
			get
			{
				return windangle;
			}
		}
		public float WindChaos
		{
			set
			{
				windchaos=value;
			}
			get
			{
				return windchaos;
			}
		}
		#endregion
	}
}
