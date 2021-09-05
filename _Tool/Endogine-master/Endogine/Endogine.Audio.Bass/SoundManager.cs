using System;
using System.Windows.Forms;
using Un4seen.Bass;
using Endogine;

namespace Endogine.Audio.Bass
{
	/// <summary>
	/// Summary description for SoundManager.
	/// </summary>
	public class SoundManager : Endogine.Audio.SoundManager
	{
		public SoundManager(Control owner) : base(owner)
		{
			if (!Un4seen.Bass.Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, 0, null))
				throw new Exception("Bass init error: No device?");

			this._supportedExtensions.Add("wav");
			this._supportedExtensions.Add("mp3");
		}

		public override void Dispose()
		{
			base.Dispose();
			Un4seen.Bass.Bass.BASS_Stop();
			Un4seen.Bass.Bass.BASS_Free();
		}

		public override Endogine.Audio.SoundStrategy CreateSoundStrategy()
		{
			return (Endogine.Audio.SoundStrategy)new Endogine.Audio.Bass.SoundStrategy();
		}

		//TODO: keep track of playing sounds, max num allowed sounds, priority
	}
}
