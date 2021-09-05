using System;
using Endogine;
using Endogine.Forms;
using Endogine.Audio;

namespace Tests.DrumMachine
{
	/// <summary>
	/// Summary description for Track.
	/// </summary>
	public class Track : Sprite
	{
		private Sound m_snd;

		public Track(string a_sSoundFile, int nNumNotes, EPoint pntGridCellSize)
		{
			m_snd = new Sound();
			m_snd.Filename = a_sSoundFile;

			for (int nNote = 0; nNote < nNumNotes; nNote++)
			{
				CheckBox cb = new CheckBox();
				cb.Parent = this;
				cb.Rect = new ERectangleF(nNote*pntGridCellSize.X,0,30,30);
				cb.Name = nNote.ToString();
			}
		}

		public override void Dispose()
		{
			m_snd.Dispose();
			base.Dispose();
		}

		public void LoadPattern(int[] pattern)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				CheckBox cb = (CheckBox)this.GetChildByName(i.ToString());
				cb.Checked = (pattern[i] > 0);
			  }
		}

		public void PlayPosition(int nPos)
		{
			CheckBox cb = (CheckBox)this.GetChildByName(nPos.ToString());
			if (cb == null)
				return;

			if (cb.Checked)
				this.m_snd.Play();
		}
	}
}
