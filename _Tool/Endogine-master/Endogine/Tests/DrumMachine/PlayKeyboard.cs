using System;
using System.Collections;
using Endogine;
using Endogine.Audio;

namespace Tests.DrumMachine
{
	/// <summary>
	/// Summary description for PlayKeyboard.
	/// </summary>
	public class PlayKeyboard
	{
		ArrayList _sounds;
		Hashtable _keys;

		public PlayKeyboard()
		{
			string[] keys = new string[]{"z","s","x","d","c","v","g","b","h","n","j","m",
										"q","d2","w","d3","e","r","d5","t","d6","y","d7","u","i","d9","o","d0","p"};
			this._keys = new Hashtable();
			int i=0;
			foreach (string s in keys)
				this._keys.Add(s.ToUpper(),i++);
			this._keys.Add("Oemcomma", this._keys["Q"]);

			this._sounds = new ArrayList();
			for (int j=0; j<4; j++)
			{
				Sound snd = new Sound();
				snd.Filename = "Tone.wav";
				this._sounds.Add(snd);
			}

			//EH.Instance.Stage.RenderControl.KeyDown+=new System.Windows.Forms.KeyEventHandler(RenderControl_KeyDown);
			EH.Instance.KeyEvent+=new KeyEventHandler(Instance_KeyEvent);

		}

		~PlayKeyboard()
		{
		}

		private void RenderControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
		}

		private void Instance_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			if (!bDown)
				return;
			string s = e.KeyCode.ToString();
			if (!this._keys.Contains(s))
				return;

			int transpose = (int)this._keys[s];
			Sound snd = (Sound)this._sounds[0];
			snd.TransposedNotes = transpose;
			snd.Play();
			this._sounds.RemoveAt(0);
			this._sounds.Add(snd);
			//this._snd.TransposedNotes = transpose;
			//this._snd.Play();
		}
	}
}
