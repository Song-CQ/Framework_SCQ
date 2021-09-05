using System;
using Endogine;

namespace MusicGame.Midi.SpriteEffect
{
	/// <summary>
	/// Summary description for Wiggle.
	/// </summary>
	public class Wiggle : Sprite
	{
		private int _frameNum;
		private EPointF _ptOffset;
		public Wiggle()
		{
		}

		public override void EnterFrame()
		{
			this._frameNum++;

			float f = (float)Math.Sin((float)this._frameNum/7f);
			EPointF ptNew = EPointF.FromLengthAndAngle(50, f*0.5f);
			ptNew.Y*=0.2f;

			if (this._ptOffset == null)
				this._ptOffset = ptNew;
			EPointF ptDiff = ptNew - this._ptOffset;
			this._ptOffset = ptNew;
			this.Parent.Loc+=ptDiff;
		}
	}
}
