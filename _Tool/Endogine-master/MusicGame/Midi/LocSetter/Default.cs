using System;
using Endogine;

namespace MusicGame.Midi.LocSetter
{
	/// <summary>
	/// Summary description for LocSetter.
	/// </summary>
	public class Default : Base
	{
		private static EPointF _lastLoc = new EPointF(100,100);
		private static long _lastStart =-1;

		public Default()
		{
		}

		public override void Start()
		{
			if (_lastStart == -1)
				_lastStart = DateTime.Now.Ticks;

			long diff = DateTime.Now.Ticks-_lastStart;
			int msDiff = (int)diff/10000;

			//Random rnd = new Random();
			//this.Interactor.Loc = new EPointF(rnd.Next(400), rnd.Next(400));
			this.Interactor.Loc = _lastLoc.Copy();
			_lastLoc.X+=msDiff/5;

			if (_lastLoc.X > 600)
				_lastLoc.X = 100;
			_lastStart = DateTime.Now.Ticks;
		}

		public override void EnterFrame()
		{
			//			if (this.Interactor.PartLeftToDisappear >= 0 && this.Interactor.PartLeftToDisappear <= 1)
			//				this.Interactor.Velocity+=new EPointF(0,-0.5f);
		}
	}
}
