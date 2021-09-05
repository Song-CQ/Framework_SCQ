using System;
using Endogine;

namespace MusicGame.Midi.Label
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : Endogine.Forms.Label
	{
		public Default()
		{
			this.FontGenerator = Main.Instance.FontGenerator;
		}
	}
}
