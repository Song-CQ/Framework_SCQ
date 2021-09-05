using System;
using Endogine;

namespace MusicGame.Midi.LocSetter
{
	/// <summary>
	/// Summary description for LocSetterSwirl.
	/// </summary>
	public class Swirl : Base
	{
		private EPointF _locOrg;
		public Swirl()
		{		
		}

		public override void Start()
		{
			Random rnd = new Random();
			this.Interactor.Loc = new EPointF(rnd.Next(100), rnd.Next(100)) + new EPointF(300,300);
			this._locOrg = this.Interactor.Loc.Copy();
		}

		public override void EnterFrame()
		{
			this.Interactor.Loc = this._locOrg + EPointF.FromLengthAndAngle(this.Interactor.PartLeftToNoteOn*7, (float)(this.Interactor.PartLeftToNoteOn*Math.PI*2*4));
		}
	}
}
