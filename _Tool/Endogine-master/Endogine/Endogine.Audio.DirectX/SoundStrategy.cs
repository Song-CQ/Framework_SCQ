using System;
using Microsoft.DirectX.DirectSound;
using Endogine;
//using System.Diagnostics;
using System.Threading;

namespace Endogine.Audio.DirectX
{
	public delegate void SoundEventHandler(object sender, SoundEventArgs e);
	public class SoundEventArgs : EventArgs
	{
		private readonly int _type = 0;
		public SoundEventArgs(int type) {this._type = type;}
		public int Type{ get { return this._type;}}
	}

	//TODO: effects:
	//http://www.microsoft.com/belux/fr/msdn/community/columns/munoz/wmpphaser.mspx

	/// <summary>
	/// Summary description for Sound.
	/// </summary>
	public class SoundStrategy : Endogine.Audio.SoundStrategy
	{
		private Device _device = null;

		//private WaveFormat m_wfx;
		private int _numSamples;
		private SecondaryBuffer _sndBuf = null;
		//private Device m_sndDev = null;

		public BufferPositionNotify[] _bufPositionNotify = new BufferPositionNotify[1];  
		static AutoResetEvent _resetEvent = null;
		public Notify _notify = null;
		private Thread _threadNotify = null;

		public event SoundEventHandler SoundEvent;

		public SoundStrategy()
		{
		}

		public override void Init()
		{
			this._device = ((Endogine.Audio.DirectX.SoundManager)Endogine.EH.Instance.SoundManager).DefaultDevice;
		}

		public override string Filename
		{
			set
			{
//				base.Load(a_sFilename);
				float fPitch = 1;
				float pan = 0;
				float vol = 0;
				if (this._sndBuf!=null)
				{
					//we had a buffer. Remember settings so they can be re-applied:
					pan = this._sound.Pan;
					fPitch = this._sound.Pitch;
					vol = this._sound.Volume;
				}
				this._sndBuf = null;

				//BufferDescription bufdes = new BufferDescription();
				//this.m_sndBuf = new SecondaryBuffer(this._loadedFile, bufdes, this.m_device);
				this._sndBuf = new SecondaryBuffer(value, this._device);
				//TODO: streaming as an option! See "Using Streaming Buffers"
				//and http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncodefun/html/code4fun02032004.asp
				if (this._sndBuf.Format.BitsPerSample == 0 || this._sndBuf.Caps.BufferBytes == 0)
				{
					this._sndBuf = null;
					throw new Exception("Sound file "+value+" could not be loaded");
				}
				this._numSamples = this._sndBuf.Caps.BufferBytes/this.Channels/this.BitsPerSample*8;

				if (fPitch > 0)
				{
					this.Pan = pan;
					this.Pitch = fPitch;
					this.Volume = vol;
				}
			}
		}

		public override void Dispose()
		{
			this.Free();
		}

		#region Information Properties

		public override short BitsPerSample
		{ get { return this._sndBuf.Format.BitsPerSample;	}}

		public override short Channels
		{	get { return this._sndBuf.Format.Channels;}}

		public override int SampleRate
		{	get { return this._sndBuf.Format.SamplesPerSecond;}}

		public override bool Playing 
		{	get { if (this._sndBuf==null) return false; return this._sndBuf.Status.Playing;	}}

		public override int NumSamples
		{get{ return this._numSamples;}}

		#endregion

		#region Read/write properties

		public Device Device
		{
			get { return this._device; }
			set { if (this.Playing) this.Stop(); this._device = value; }
		}

		public override bool CanPlay
		{
			get {return this._sndBuf!=null;}
		}

		public override float Pan
		{
			//TODO: convert -100 - 100 to -10000 - 10000
			//get { return (float)this._sndBuf.Pan; }
			set
			{
				this._sndBuf.Pan = (int)value;
			}
		}

		public override float Pitch
		{
			//get { return (float)this._sndBuf.Frequency / this._sndBuf.Format.SamplesPerSecond; }
			set { this._sndBuf.Frequency = (int)(this._sndBuf.Format.SamplesPerSecond * value); }
		}

		public override float Volume
		{
			//TODO: convert 0-100 to 0 (max) to -10000 (silent)
			//get { return (float)this._sndBuf.Volume; }
			set
			{
				int val = (int)value - 100;
				val*=100; //TODO:logarithmic!
				this._sndBuf.Volume = val;
			}
		}

		public override bool Looping
		{
			set{}
		}


		#endregion



		public override bool Play (int nFromPosition)
		{
			if (this._sndBuf == null)
				return false;

			int nStartByteNum = nFromPosition/8*this.BitsPerSample/this.Channels;
			bool bWasPlaying = this.Playing;
			this._sndBuf.SetCurrentPosition(nStartByteNum);
			if (!bWasPlaying)
				InitNotifications();
			this._sndBuf.Play(0, BufferPlayFlags.Default);
//			this._sndBuf.SetEffects();
			this.FireEvent(new SoundEventArgs(1));
			return true;
		}

		public override void Pause()
		{
			//TODO
		}


		public override void Stop()
		{
			if (!this.Playing)
				return;
			this._sndBuf.Stop();
		}

		void InitNotifications()
		{
			//-----------------------------------------------------------------------------
			// Name: InitNotifications()
			// Desc: Inits the notifications on the capture buffer which are handled
			//       in the notify thread.
			//-----------------------------------------------------------------------------

			if (this._sndBuf == null)
				throw new NullReferenceException();
		
			// Create a thread to monitor the notify events
			if (this._threadNotify == null)
			{
				this._threadNotify = new Thread(new ThreadStart(WaitThread));
				this._threadNotify.Start();

				// Create a notification event, for when the sound stops playing
				SoundStrategy._resetEvent = new AutoResetEvent(false);
			}

			// Setup the notification positions
			this._bufPositionNotify[0].Offset = (int)(this.NumSamples/8*this.BitsPerSample/this.Channels)-1;
			this._bufPositionNotify[0].EventNotifyHandle = SoundStrategy._resetEvent.Handle;
		
			this._notify = new Notify(this._sndBuf);

			// Tell DirectSound when to notify the app. The notification will come in the from 
			// of signaled events that are handled in the notify thread.
			this._notify.SetNotificationPositions(this._bufPositionNotify, 1);
		}

		private void WaitThread()
		{
			while(this._sndBuf != null && !this._sndBuf.Disposed)
			{
				//Sit here and wait for a message to arrive
				if (SoundStrategy._resetEvent!=null)
					SoundStrategy._resetEvent.WaitOne(Timeout.Infinite, true);
				Stop();
				this.FireEvent(new SoundEventArgs(2));
			}
		}

		protected virtual void FireEvent(SoundEventArgs se) 
		{
			if (SoundEvent != null)
				SoundEvent(this, se);

//			if (this._autoDispose) //TODO:?
//				this.Dispose();
		}

		public override void Free()
		{
			this.Stop();
			if (this._sndBuf != null)
				this._sndBuf.Dispose();
		}
	}
}
