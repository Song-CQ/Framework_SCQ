using System;
using Endogine;

namespace MusicGame.Midi.SpriteEffect
{
	/// <summary>
	/// Summary description for Jerk.
	/// </summary>
	public class Jerk : Endogine.GameHelpers.GameSprite
	{
		private EPointF _ptOffset;
		private float _intensity;
		public Jerk()
		{
		}

		public float Intensity
		{
			set {this._intensity = value;}
		}

		public override void EnterFrame()
		{
			float f = (float)Math.Sin((double)this.FramesAlive*0.5f);
			float factor = 1f-(float)this.FramesAlive/70;

			EPointF ptNew = new EPointF(0,this._intensity*f*factor*20);
			if (this._ptOffset==null)
				this._ptOffset = ptNew;
			EPointF ptDiff = ptNew-this._ptOffset;
			this._ptOffset = ptNew;

			this.Parent.Loc+=ptDiff;
			//this.Parent.Rotation = f*(float)Math.PI*intensity;
			//EH.Put(this.Rotation.ToString());

			base.EnterFrame ();

			if (factor <= 0)
			{
				//this.Parent.Rotation = 0;
				this.Dispose();
			}
		}
	}
}
