using System;
using Endogine;
using Endogine.Forms;
using System.Threading;

namespace Tests.DrumMachine
{
	/// <summary>
	/// Summary description for DrumForm.
	/// </summary>
	public class DrumForm : Form
	{
		private Track[] m_tracks;
		private int m_nPlaybackHead = 0;
		private int m_nNumNotes = 16;
		private Timer m_timer;

		private EPoint m_pntGridCellSize;
		private EPointF m_pntGridStart;
		private Sprite m_spPlaybackHead;

		private Endogine.Audio.Listener _listener;
		private PlayKeyboard _pk;

		public DrumForm()
		{
            if (Endogine.Audio.SoundManager.DefaultSoundManager == null)
            {
                throw new Exception("No sound system .dll found!");
            }
			this.Rect = new ERectangleF(0,0,600,150);
			this.m_pntGridCellSize = new EPoint(35,40);
			this.m_pntGridStart = new EPointF(10,50);
			this.MouseActive = true;

			this.m_tracks = new Track[2];

			string[] aSounds = new string[]{"drumsnare.wav", "drumbass.wav"};
			for (int nChannel = 0; nChannel < aSounds.Length; nChannel++)
			{
				Track track = new Track(aSounds[nChannel], m_nNumNotes, this.m_pntGridCellSize);
				track.Parent = this;
				track.Loc = new EPointF(0,nChannel*this.m_pntGridCellSize.Y)+m_pntGridStart;
				this.m_tracks[nChannel] = track;
				//track.Visible=false;
			}

			this.m_tracks[0].LoadPattern(new int[]{0,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0});
			this.m_tracks[1].LoadPattern(new int[]{1,0,1,0,0,0,1,0,1,0,1,0,0,0,0,0});

			this.m_spPlaybackHead = new Sprite();
			this.m_spPlaybackHead.MemberName = "Cross";
			this.m_spPlaybackHead.CenterRegPoint();
			this.m_spPlaybackHead.Parent = this;
			this.m_spPlaybackHead.Loc = m_pntGridStart;

			this.m_timer = new Timer(new TimerCallback(this.Tick), null, 0, 125);

			this._listener = new Endogine.Audio.Listener();

			this._pk = new PlayKeyboard();
		}

		public override void Dispose()
		{
			foreach (Track track in m_tracks)
				track.Dispose();
			m_timer.Dispose();
			base.Dispose();
		}

		public void Tick(object state)
		{
			this.PlayNext();
		}

		private void PlayNext()
		{
			this.m_spPlaybackHead.LocX = m_pntGridStart.X+m_nPlaybackHead*this.m_pntGridCellSize.X;

			foreach (Track track in m_tracks)
				track.PlayPosition(m_nPlaybackHead);

			m_nPlaybackHead++;
			if (m_nPlaybackHead == m_nNumNotes)
				m_nPlaybackHead = 0;
		}

		public override void EnterFrame()
		{
			this._listener.Position = new Vector3(this.MouseLoc.X - this.Rect.Width/2, this.MouseLoc.Y, 0);
			base.EnterFrame ();
		}
	}
}
