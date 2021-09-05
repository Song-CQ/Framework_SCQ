using System;
using Endogine;

namespace MusicGame.Midi.Interactor
{
	/// <summary>
	/// Summary description for InteractorShake.
	/// </summary>
	public class Shake : Base
	{
		public Shake()
		{
		}

		public override void Start()
		{
			this.MemberName = "BallBlue";
		}

		public override void Update()
		{
			float f = this.PartLeftToNoteOn;
			//f = f*f;
			this.Blend = (int)((1f-f)*255f);
		}

	}
}
