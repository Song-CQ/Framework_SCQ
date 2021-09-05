using System;
using System.Collections;
using Endogine;
using Endogine.Midi;

namespace MusicGame.Midi.Interactor
{
	/// <summary>
	/// Summary description for InteractorBase.
	/// </summary>
	public class Base : Sprite
	{
		private EPointF _ptVelocity = new EPointF();
		protected long _stopTicks;
		private long _startTicks;
		private long _disappearDurationMsecs = 100;
		private long _startAcceptingInputTicks;
		private string _trackName;
		private int _channelNumber;
		private int _note;
		private int _strength;
		private int _duration;

		private bool _noteOnCalled;

		private static ArrayList _interactors;

		public Base() : base(false)
		{
			//read sysex messages to decide:
			//Type, LocX, LocY
			this.Ink = Endogine.RasterOps.ROPs.Copy;
			InteractorConstructed(this);
		}

		public override void Dispose()
		{
			InteractorDisposed(this);
			base.Dispose ();
		}

		public static void InteractorConstructed(Interactor.Base interactor)
		{
			if (_interactors==null)
				_interactors = new ArrayList();
			_interactors.Add(interactor);
		}
		public static void InteractorDisposed(Interactor.Base interactor)
		{
			_interactors.Remove(interactor);
		}
		public static ArrayList Interactors
		{
			get {return (ArrayList)_interactors.Clone();}
		}
		public static Interactor.Base GetMostAccurateInteractor()
		{
			float acc = 0;
			Base bestInteractor = null;
			//SortedList sl = new SortedList();
			foreach (Interactor.Base interactor in Interactor.Base.Interactors)
			{
				if (interactor.GetAccuracy() > acc)
				{
					bestInteractor = interactor;
					acc = interactor.GetAccuracy();
				}
				//sl.Add(interactor.GetAccuracy(), interactor);
			}
			return bestInteractor; //(Interactor.Base)sl.GetByIndex(sl.Count-1);
		}



		public string TrackName
		{
			get {return this._trackName;}
			set {this._trackName = value;}
		}
		public int ChannelNumber
		{
			get {return this._channelNumber;}
			set {this._channelNumber = value;}
		}
		public int Note
		{
			get {return this._note;}
			set {this._note = value;}
		}
		public int Strength
		{
			get {return this._strength;}
			set {this._strength = value;}
		}
		
		public int Duration
		{
			get {return this._duration;}
			set {this._duration = value;}
		}

		public EPointF Velocity
		{
			get {return this._ptVelocity;}
			set {this._ptVelocity = value;}
		}
		public long NoteOnTicks
		{
			get {return this._stopTicks;}
			set {this._stopTicks = value;}
		}
		public long DisappearTicks
		{
			get {return this._stopTicks+this._disappearDurationMsecs*1000*10;}
		}
		public long DisappearDurationMsecs
		{
			get {return this._disappearDurationMsecs;}
			set {this._disappearDurationMsecs = value;}
		}

		public virtual void Prepare(string trackName, int channel, int millisecondsToStart, int note, int strength, int duration, EPointF loc)
		{
			this._startTicks = DateTime.Now.Ticks;
			this._stopTicks = this._startTicks + millisecondsToStart*1000*10;
			this._trackName = trackName;
			this._channelNumber = channel;
			this._note = note;
			this._duration = duration;
			this._strength = strength;
			this.Loc = loc;
		}

		public virtual void ConnectToParent()
		{
			this.Parent = EH.Instance.Stage.DefaultParent;
		}

		public virtual void Start()
		{
		}

		/// <summary>
		/// Part left from start to NoteOn (ie when user should hit it). (float: 1 first, 0 when NoteOn)
		/// </summary>
		public float PartLeftToNoteOn
		{
			get
			{
				float f = 1f-(float)(DateTime.Now.Ticks-this._startTicks)/(this._stopTicks-this._startTicks);
				if (f < 0)
					return 0f;
				return f;
			}
		}
		/// <summary>
		/// Part left from NoteOn to when interactor disappears (float: 1 on NoteOn, 0 when disappear)
		/// </summary>
		public float PartLeftToDisappear
		{
			get {return 1f-(float)(DateTime.Now.Ticks-this._stopTicks)/(this.DisappearTicks-this._stopTicks);}
		}

		public float GetAccuracy()
		{
			int diffMsecs = (int)Math.Abs(DateTime.Now.Ticks - this._stopTicks)/1000/10;
			int limit = 500;
			if (diffMsecs > limit)
				return 0;
			return 1f-(float)diffMsecs/limit;
		}

		public override void EnterFrame()
		{
			this.Move(this._ptVelocity);
			this.Update();
			base.EnterFrame ();

			if (DateTime.Now.Ticks > this._stopTicks)
				if (!this._noteOnCalled)
				{
					this._noteOnCalled = true;
					this.NoteOn();
				}

			if (DateTime.Now.Ticks > this.DisappearTicks)
				this.Dispose();
		}

		public virtual void NoteOn()
		{
			//this.Color = System.Drawing.Color.FromArgb(255,0,0);
		}

		public virtual void Update()
		{
		}
	}
}
