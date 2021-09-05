using System;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
using Endogine;

namespace Endogine.Audio.DirectX
{
	/// <summary>
	/// Summary description for SoundManager.
	/// </summary>
	public class SoundManager : Endogine.Audio.SoundManager
	{
		private Device _defaultDevice;
		private Control _owner;

		public SoundManager(Control owner) : base(owner)
		{
			this._owner = owner;
			try
			{
				this._defaultDevice = new Device();
				this._defaultDevice.SetCooperativeLevel(this._owner, CooperativeLevel.Priority);
			}
			catch (Exception e)
			{
				Console.WriteLine("No sound device? "+e.Message);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this._defaultDevice!=null)
				this._defaultDevice.Dispose();
			this._defaultDevice = null;
		}

//		public override Endogine.Audio.Sound CreateSound()
//		{
//			return (Endogine.Audio.Sound)new Endogine.Audio.DirectX.Sound();
//		}
		public override Endogine.Audio.SoundStrategy CreateSoundStrategy()
		{
			return new SoundStrategy();
		}


		public Device DefaultDevice
		{
			get { return this._defaultDevice;}
			set { this._defaultDevice = value; }
		}

		//TODO: keep track of playing sounds, max num allowed sounds, priority
	}
}
