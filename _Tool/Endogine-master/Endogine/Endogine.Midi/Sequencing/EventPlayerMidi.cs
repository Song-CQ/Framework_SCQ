using System;

namespace Endogine.Midi.Sequencing
{
	/// <summary>
	/// Summary description for EventPlayerMidi.
	/// </summary>
	public class EventPlayerMidi
	{
		public EventPlayerMidi()
		{
		}

		public void RegisterTrackPlayer(TrackPlayer tp)
		{
			tp.TrackEvent+=new Endogine.Midi.TrackPlayer.TrackEventDelegate(tp_TrackEvent);
		}
		public void UnregisterTrackPlayer(TrackPlayer tp)
		{
			tp.TrackEvent-=new Endogine.Midi.TrackPlayer.TrackEventDelegate(tp_TrackEvent);
		}

		private void tp_TrackEvent(object sender, MidiEvent anEvent)
		{
			anEvent.Message.Accept((TrackPlayer)sender);
		}
	}
}
