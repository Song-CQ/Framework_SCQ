using System;
using Endogine;

namespace MusicGame.Midi.Interactor
{
	/// <summary>
	/// Summary description for InteractorX.
	/// </summary>
	public class X : Base
	{
		private Sprite _spHit;
		private Sprite _spTimer;
		public X()
		{
		}

		public override void Start()
		{
			this.Setup();
			this.DisappearDurationMsecs = 200;

			this.Visible = false;
		}

		public override void NoteOn()
		{
			//this.Setup();
			this.Visible = true;
		}

		public override void Update()
		{
			//float scale = this.PartLeftToNoteOn+1;
			if (this.PartLeftToDisappear > 1 || this.PartLeftToDisappear < 0)
				return;
			if (this._spTimer==null)
				return;
			float scale = this.PartLeftToDisappear+1;
			scale*=(float)this.Strength/127;
			this._spTimer.Scaling = new EPointF(scale, scale);

			this._spTimer.Blend = (int)(this.PartLeftToDisappear*255);
			this._spHit.Blend = this._spTimer.Blend;
		}


		private void Setup()
		{
			Random rnd = new Random();
			//Console.WriteLine(this.Note.ToString());
			
			//this.Loc = new EPointF((this.Note-36)*30+80, rnd.Next(400));

			this._spTimer = new Sprite(false);
			this._spTimer.Parent = this;
			this._spTimer.MemberName = "BallGreen";
			this._spTimer.CenterRegPoint();
//			this._spTimer.Scaling = new EPointF(2,2);

			this._spHit = new Sprite(false);
			this._spHit.LocZ = 2;
			this._spHit.MemberName = "BallRed";
			this._spHit.CenterRegPoint();
			this._spHit.Parent = this;
		}

	}
}
