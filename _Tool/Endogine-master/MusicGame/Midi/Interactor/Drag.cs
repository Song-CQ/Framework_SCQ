using System;
using Endogine;

namespace MusicGame.Midi.Interactor
{
	/// <summary>
	/// Summary description for InteractorDrag.
	/// </summary>
	public class Drag : Base
	{
		private EPointF _locOrg;

		public Drag()
		{
		}

		public override void Start()
		{
			this.MouseActive = true;
			this.MemberName = "BallOrange";
		}

		protected override void OnMouse(System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.Down)
			{
				float accuracy = this.GetAccuracy();
				if (accuracy > 0)
					this.Scaling = new Endogine.EPointF(accuracy+1,accuracy+1);
				else
					this.Scaling = new Endogine.EPointF(0.5f,0.5f);
			}
		}
	}
}
