using System;
using Endogine;

namespace MusicGame.Midi.HitFeedback
{
	/// <summary>
	/// Summary description for Base.
	/// </summary>
	public class Base : Endogine.GameHelpers.GameSprite
	{
		protected Endogine.Forms.Label _lbl;

		public Base()
		{
		}

		public virtual int Points
		{
			get {return 0;}
			set
			{
				if (this._lbl!=null)
				{
					this._lbl.Text = value.ToString();
					this._lbl.Parent = this;
				}
				Main.Instance.Score.Value+=value;
			}
		}
	}
}
