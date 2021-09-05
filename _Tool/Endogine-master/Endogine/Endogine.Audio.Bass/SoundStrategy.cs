using System;
using Endogine;
using Un4seen.Bass;
using System.Threading;

namespace Endogine.Audio.Bass
{
	/// <summary>
	/// Summary description for Sound.
	/// </summary>
	public class SoundStrategy : Endogine.Audio.SoundStrategy
	{
		private int _stream = 0;
//		private int _streamFx;
//		private int _tickCounter = 0;
//		private long _20msLength;
		private bool _looping;

		private Un4seen.Bass.BASS_SAMPLE _sampleInfo;


		public SoundStrategy()
		{
		}

		public override void Init()
		{

		}

		public override string Filename
		{
			set
			{
				//base.Filename = value;

				this._stream = Un4seen.Bass.Bass.BASS_StreamCreateFile(value, 0, 0,
					Un4seen.Bass.BASSStream.BASS_DEFAULT);
				//				Un4seen.Bass.BASSStream.BASS_STREAM_DECODE | 
				//				Un4seen.Bass.BASSStream.BASS_SAMPLE_FLOAT | 
				//				Un4seen.Bass.BASSStream.BASS_STREAM_PRESCAN);

				if (this._stream == 0)
				{
					throw new Exception("Sound file "+value+" could not be loaded");
				}

				this._sampleInfo = new BASS_SAMPLE();
				Un4seen.Bass.Bass.BASS_SampleGetInfo(this._stream, this._sampleInfo);

				//this._20msLength = Un4seen.Bass.Bass.BASS_ChannelSeconds2Bytes(this._stream, 0.02f);

				this.Pan = this._sound.Pan;
				this.Pitch = this._sound.Pitch;
				this.Volume = this._sound.Volume;
				this._looping = false; //?
				this.Looping = this._sound.Looping;
			}
		}

		public override void Dispose()
		{
			this.Free();
		}

		#region Information Properties

		public override short BitsPerSample
		{ get { return 8;	}} //TODO:!!

		public override short Channels
		{	get { return (short)this._sampleInfo.chans;}}

		public override int SampleRate
		{	get { return this._sampleInfo.freq;}}

		public override bool Playing 
		{
			get
			{
				if (this._stream==0)
					return false;

				return Un4seen.Bass.Bass.BASS_ChannelIsActive(this._stream) == (int)Un4seen.Bass.BASSActive.BASS_ACTIVE_PLAYING; //TODO: doesn't work
			}
		}

		public override int NumSamples
		{get{ return this._sampleInfo.length;}}

		public override bool CanPlay
		{ get {return this._stream!=0;}}

		#endregion

		#region Read/write properties

		public override bool Looping
		{
//			get
//			{
//				//if (this._stream==0)
//				//TODO:	return base.Looping;
//				Un4seen.Bass.BASS_CHANNELINFO info = new BASS_CHANNELINFO();
//				Un4seen.Bass.Bass.BASS_ChannelGetInfo(this._stream, info);
//				return (info.flags & (int)Un4seen.Bass.BASSStream.BASS_SAMPLE_LOOP) > 0;
//			}
			set
			{
				//Console.WriteLine("Try loop?" + this.Looping + "  "+value+" "+this._stream+" "+this.Filename);
				//if (this.Looping==value)
				//TODO:		return;
				//base.Looping = value;
				//Console.WriteLine("set base"+value);
				if (this._stream==0)
					return;
				if (this._looping == value)
					return;
				this._looping = value;
				Un4seen.Bass.BASS_CHANNELINFO info = new BASS_CHANNELINFO();
				Un4seen.Bass.Bass.BASS_ChannelGetInfo(this._stream, info);
				//Console.WriteLine("Looping " + (info.flags & (int)Un4seen.Bass.BASSStream.BASS_SAMPLE_LOOP));
				info.flags ^=(int)Un4seen.Bass.BASSStream.BASS_SAMPLE_LOOP;
				Un4seen.Bass.Bass.BASS_ChannelSetFlags(this._stream, info.flags);
			}
		}

		public override float Pan
		{
			//get { return base.Pan; }
			set
			{
				if (this._stream==0)
					return;
				Un4seen.Bass.Bass.BASS_ChannelSetAttributes(this._stream, -1, -1, (int)value);
				//this.UpdateAttributes();
			}
		}

//		private void UpdateAttributes()
//		{
//			Un4seen.Bass.Bass.BASS_ChannelSetAttributes(this._stream, (int)this._freq, (int)this._vol, (int)this._pan);
//		}

		public override float Pitch
		{
//			get { return (float)value/this.SampleRate; }
			set
			{
				int freq = (int)(value*this.SampleRate);
				if (this._stream==0)
					return;
				Un4seen.Bass.Bass.BASS_ChannelSetAttributes(this._stream, freq, -1, -101);
				//this.UpdateAttributes();
			}
		}

		public override float Volume
		{
			//get { return base.Volume; }
			set
			{
				if (this._stream==0)
					return;
				Un4seen.Bass.Bass.BASS_ChannelSetAttributes(this._stream, -1, (int)this._sound.ActualVolume, -101);
				//this.UpdateAttributes();
			}
		}

		#endregion



		public override bool Play (int nFromPosition)
		{
			if (this._stream == 0)
				return false;

			//bool bWasPlaying = this.IsPlaying;
			bool ok = false;
			ok = Un4seen.Bass.Bass.BASS_ChannelSetPosition(this._stream, (long)nFromPosition);
			if (!ok)
				throw new Exception("Couldn't set position of Bass sound!");
			
			ok = Un4seen.Bass.Bass.BASS_ChannelPlay(this._stream, false); //loop?
			if (!ok)
			{
				//TODO: store error message for later retrieval
				string err = Enum.GetName(typeof(Un4seen.Bass.BASSErrorCode), Un4seen.Bass.Bass.BASS_ErrorGetCode());
				throw new Exception("Couldn't start Bass sound: "+err + " ("+this._sound.Filename+")");
			}
			return ok;
		}

		public override void Pause()
		{}

		public override void Stop()
		{
			if (!this.Playing)
				return;
			Un4seen.Bass.Bass.BASS_ChannelStop(this._stream);
			//Un4seen.Bass.Bass.BASS_SampleStop(this._stream);
		}

		public override void Free()
		{
			if (this._stream==0)
				return;
			this.Stop();
			Un4seen.Bass.Bass.BASS_StreamFree(this._stream);
			this._stream = 0;
		}
	}
}
