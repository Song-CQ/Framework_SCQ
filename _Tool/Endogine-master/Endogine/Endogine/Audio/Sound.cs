using System;

namespace Endogine.Audio
{
	/// <summary>
	/// Summary description for Sound.
	/// </summary>
	public class Sound
	{
		protected string _requestedFile;
		protected string _loadedFile;
		protected bool _autoDispose;
		protected bool _muted;
		protected SoundManager _soundManager;

		protected Vector3 _position = new Vector3(); //TODO: should use a Matrix instead (
		protected bool _positionRelative;
		protected float _vol = 100;
		private float _gain = 1;
		protected float _pan = 0;
		protected int _freq;
		protected bool _looping;
		protected bool _enabled = true;

		private SoundStrategy _strategy;

		public Sound(string filename, bool autoDisposeWhenDone)
		{
//			this.Init();
//			this._autoDispose = autoDisposeWhenDone;
//			this.Load(filename);
//			if (this.CanPlay)
//				this.Play();
		}

		public Sound()
		{
			this._soundManager = SoundManager.DefaultSoundManager;
			this._strategy = this._soundManager.CreateSoundStrategy();
			this._strategy.Sound = this;
			this.Init();
			this._soundManager.SoundCreated(this);
		}

		public static Sound Create() //TODO: remove!
		{
			return new Sound();
		}
		public static Sound Create(string filename, bool autoDisposeWhenDone)
		{
			Sound snd = new Sound(); //Sound.Create();
			snd.AutoDispose = autoDisposeWhenDone;
			snd.Filename = filename;
			if (snd.CanPlay)
				snd.Play();
			return snd;
		}

		protected virtual void Init()
		{
			this._strategy.Init();
		}

		public virtual void Dispose()
		{
			this.Free();
			this._soundManager.SoundDisposed(this);
			this._soundManager = null;
		}

		public virtual string Filename
		{
			get {return this._requestedFile;}
			set
			{
				//TODO: this._originalFile = value;
				this._requestedFile = value;
				this._loadedFile = this._soundManager.FindSound(value);
				this._strategy.Free();
				this._strategy.Filename = this._loadedFile;
			}		
		}

		public bool Play(string sFileName)
		{
			this.Stop();
			this.Filename = sFileName;
			return this.Play();
		}
		public bool Play()
		{
			//TODO: if (m_bPaused)...
			this.Stop();
			return this.Play(0);
		}
		public bool Play (int nFromPosition)
		{
			if (!this._enabled)
				return false;
			return this._strategy.Play(nFromPosition);
		}
		public void Pause()
		{
			this._strategy.Pause();
		}
		public void Stop()
		{
			this._strategy.Stop();
		}
		/// <summary>
		/// Stops playing and frees all resources
		/// </summary>
		public void Free()
		{
			this._strategy.Free();
		}

		public bool Looping
		{
			get { return this._looping; }
			set { this._looping = value; this._strategy.Looping = value;}
		}

		/// <summary>
		/// A value between -100 (full left) and 100 (full right)
		/// </summary>
		public float Pan
		{
			get { return this._pan; }
			set { this._pan = this.EnsurePan(value); this._strategy.Pan = value;}
		}
		protected float EnsurePan(float val)
		{
			return (float)Math.Min(Math.Max(-100, val), 100);
		}

		/// <summary>
		/// what to multiply the original frequency with (2.0 = 1 octave higher, 0.5 = 1 octave lower)
		/// </summary>
		public float Pitch
		{
			get { return 0; }
			set { this._strategy.Pitch = value;}
		}
		protected float EnsurePitch(float val)
		{
			return (float)Math.Min(Math.Max(0.000001f, val), 100);
		}

		public float ActualVolume
		{
			get {return this._gain*this._vol;}
		}
		public float Gain
		{
			get {return this._gain;}
			set {this._gain = value; this.Volume = this.Volume; }
		}
		/// <summary>
		/// 0 (silent) to 100 (max)
		/// </summary>
		public float Volume
		{
			get { return this._vol; }
			set { this._vol = this.EnsureVolume(value); this._strategy.Volume = this._vol;}
		}
		protected float EnsureVolume(float val)
		{
			return (float)Math.Min(Math.Max(0, val), 100);
		}


        //private int PercentToDB(float volume)
        //{
        //    float db = 20f * (float)Math.Log10(volume);
        //    return (int)(100f * db);
        //}
        //private int PercentToDB(float volume)
        //{
        //    if (volume <= 0.00001f)
        //    {
        //        return -10000;
        //    }
        //    else
        //    {
        //        float db = 20f * (float)Math.Log10(volume);
        //        return (int)(100f * db);
        //    }
        //}

		public bool Muted
		{
			get { return this._muted; }
			set
			{
				this._muted = value;
				//this._strategy.
				//TODO:
			}
		}

		public Vector3 Position
		{
			get { return this._position; }
			set
			{
				this._position = value;
				//TODO: notify listener if it exists
			}
		}
		public bool PositionIsRelative
		{
			get {return this._positionRelative;}
			set {this._positionRelative = value;}
		}


		public short BitsPerSample
		{ get { return this._strategy.BitsPerSample; }}

		public short Channels
		{	get { return this._strategy.Channels;}}

		public int SampleRate
		{	get { return this._strategy.SampleRate;}}

		public int NumSamples
		{get{ return this._strategy.NumSamples;}}

		public double LengthInSeconds
		{
			get { return (double)this.NumSamples / this.SampleRate; }
		}

		public float TransposedNotes
		{
			get { return (float)Math.Log(this.Pitch, 2);	}
			set {	this.Pitch = (float)Math.Pow(2, value/12);	}
		}

		public bool Playing 
		{	get { return this._strategy.Playing; }}
		public bool CanPlay
		{ get {return this._strategy.CanPlay;}}

		public bool AutoDispose
		{
			get {return this._autoDispose;}
			set {this._autoDispose = value;}
		}

		public bool Enabled
		{
			get {return this._enabled;}
			set
			{
				this._enabled = value;
				if (this.Playing)
					this.Stop();
			}
		}


	}
}
