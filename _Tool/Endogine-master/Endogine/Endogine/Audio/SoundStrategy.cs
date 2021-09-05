using System;

namespace Endogine.Audio
{
	/// <summary>
	/// //TODO: instead of inheriting Sound, subsystems should inherit SoundStrategy - same as with sprite rendering
	/// </summary>
	public abstract class SoundStrategy
	{
		protected Sound _sound;

		public Sound Sound
		{
//			get {return this._sound;}
			set {this._sound = value;}
		}
		abstract public void Init();

		abstract public float Pitch
		{set;} //{get; set;}
		abstract public float Volume
		{set;}
		abstract public float Pan
		{set;}

		abstract public bool Play(int nFromPosition);
		abstract public void Pause();
		abstract public void Stop();

		abstract public void Free();
		abstract public void Dispose();

		abstract public bool Looping
		{set;} //get; 

		abstract public string Filename
		{ set; }


		abstract public short BitsPerSample
		{get;}
		abstract public short Channels
		{get;}
		abstract public int SampleRate
		{get;}
		abstract public int NumSamples
		{get;}


		abstract public bool Playing 
		{get;}
		abstract public bool CanPlay
		{get;}

	}
}
