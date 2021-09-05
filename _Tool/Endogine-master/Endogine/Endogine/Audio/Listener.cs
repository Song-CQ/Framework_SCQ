using System;
using System.Collections;

namespace Endogine.Audio
{
	/// <summary>
	/// Summary description for Listener.
	/// </summary>
	public class Listener
	{
		private Vector3 _position; //TODO: Matrix, so orientation is included
		public float MaxVolumeDistance = 40;
		public Listener Instance;

		public Listener()
		{
		}

		public Vector3 Position
		{
			get {return this._position;}
			set
			{
				this._position = value;
				ArrayList sounds = SoundManager.DefaultSoundManager.Sounds;
				foreach (Sound snd in sounds)
				{
					if (snd.PositionIsRelative)
						continue;

					//float fDiff = (value - snd.Position).Length;
					EPointF ptSnd = new EPointF(snd.Position.X, snd.Position.Y);
					EPointF ptListener = new EPointF(value.X, value.Y);
					EPointF ptDiff = ptSnd-ptListener;

					float fMute = 0;
					if (ptDiff.Length > this.MaxVolumeDistance)
						fMute = (float)Math.Log(ptDiff.Length/this.MaxVolumeDistance, 2)*20;
					//float fMute = ptDiff.Length;
					snd.Volume = 100f-fMute;

					ptDiff.Length = 1;
					//float angle = ptDiff.Angle;
					snd.Pan = (int)(ptDiff.X*100);
					//EH.Put(value.X.ToString());
					//EH.Put(((int)(ptDiff.X*100)).ToString() + "  " + snd.Pan);
				}
			}
		}
	}
}
