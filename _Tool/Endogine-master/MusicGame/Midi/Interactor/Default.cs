using System;
using Endogine;
using Endogine.Midi;

namespace MusicGame.Midi.Interactor
{
	/// <summary>
	/// Summary description for VizNote.
	/// </summary>
	public class Default : Base
	{
		private Endogine.Forms.Label _lbl;
		private static int _cnt;

		public Default()
		{
		}

		public override void Start()
		{
			//this.DisappearDurationMsecs = 0;
			_cnt++;

			this.MemberName = "BallRed";
			this.Blend = this.Strength*2;
			this.LocX+= (this.Note-40)*5;
			this.Velocity = new EPointF(0,6);

			this._lbl = new Endogine.Forms.Label();
			this._lbl.Parent = this;
			this._lbl.FontGenerator = Main.Instance.FontGenerator;
			
			this._lbl.Text = (_cnt % 2 +1).ToString();
			this._lbl.Tag = "D"+this._lbl.Text;

			EH.Instance.KeyEvent+=new KeyEventHandler(Instance_KeyEvent);
		}

		public override void Dispose()
		{
			if (Main.Instance.LatestNoteOnInteractor == this)
				Main.Instance.LatestNoteOnInteractor = null;
			base.Dispose ();
		}

		private void Instance_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			if (!bDown)
				return;

			if (Enum.GetName(typeof(System.Windows.Forms.Keys), e.KeyCode) == (string)this._lbl.Tag)
			{
				Base interactor = Base.GetMostAccurateInteractor();
				if (interactor != this)
					return;
//				if (!(Main.Instance.LatestNoteOnInteractor == this || Main.Instance.LatestNoteOnInteractor == null))
//					return;

				float acc = this.GetAccuracy();
				if (acc > 0)
				{
					HitFeedback.Test points = new MusicGame.Midi.HitFeedback.Test();
					
					if (acc > 0.9)
						points.Points = 300;
					else if (acc > 0.5)
						points.Points = 100;
					else
						points.Points = 50;

					points.Loc = this.Loc;
					this.Dispose();
				}
			}
		}

		public override void NoteOn()
		{
			base.NoteOn ();
			Main.Instance.LatestNoteOnInteractor = this;
			this.Loc+= new EPointF(0,30);
			//this.Blend/=2;
			this.MemberName = "BallBlue";
		}
	}
}
