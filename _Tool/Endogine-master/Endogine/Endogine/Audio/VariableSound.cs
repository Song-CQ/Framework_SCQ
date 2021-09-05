using System;

namespace Endogine.Audio
{
	/// <summary>
	/// Summary description for VariableSound.
	/// </summary>
	public class VariableSound
	{
		private EPointF m_volumeRange;
		private EPointF m_panRange;
		private EPointF m_pitchRange;

		public VariableSound()
		{
		}

//		public string[] SoundFiles
//		{
//			get {return 1;}
//			set {x = value;}
//		}
		public EPointF VolumeRange
		{
			get {return m_volumeRange;}
			set {m_volumeRange = value;}
		}
		public EPointF PanRange
		{
			get {return m_panRange;}
			set {m_panRange = value;}
		}
		public EPointF PitchRange
		{
			get {return m_pitchRange;}
			set {m_pitchRange = value;}
		}
	}
}
