using System;
using Endogine;

namespace MusicGame.Midi
{
	/// <summary>
	/// Summary description for Score.
	/// </summary>
	public class Score : Endogine.Forms.Label
	{
		private int _value;
		public Score()
		{
		}

		public int Value
		{
			get {return this._value;}
			set
			{
				this._value = value;
				string s = this._value.ToString();
				this.Text = s.PadLeft(5,'0');
				for (int i=4; i>=5-s.Length;i--)
				{
					SpriteEffect.Jerk fx = new MusicGame.Midi.SpriteEffect.Jerk();
					fx.Intensity = (float)(5-i+1)*0.2f;
					fx.Parent = this.GetChildByIndex(i);
				}
			}
		}
	}
}
