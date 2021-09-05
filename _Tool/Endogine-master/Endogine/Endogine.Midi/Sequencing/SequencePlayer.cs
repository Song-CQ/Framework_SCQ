/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/27/2004
 */

using System;
using System.Collections;

namespace Endogine.Midi
{

	//JB: why internal sealed? I need to access it!
	/// <summary>
	/// Handles playback of a sequence.
	/// </summary>
	public class SequencePlayer : IDisposable
	{
        #region Fields

        // The list of track players.
        private ArrayList trackPlayers = new ArrayList();

        // The number of tracks currently soloed.
        private int soloCount = 0;

        // The number of tracks currently playing.
        private int activeTrackCount;

		protected IMidiSender xmidiSender;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the end of the sequence has been reached.
        /// </summary>
        public event EventHandler EndOfSequenceReached;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SequencePlayer class with the 
        /// specified MIDI sender, tick generator and sequence.
        /// </summary>
        /// <param name="midiSender">
        /// The MIDI sender to use to send MIDI messages.
        /// </param>
        /// <param name="tickGen">
        /// The tick generator used for timing the playback of MIDI messages.
        /// </param>
        /// <param name="seq">
        /// The sequence to playback.
        /// </param>
		public SequencePlayer(IMidiSender midiSender, TickGenerator tickGen, 
            Sequence seq)
		{
			this.xmidiSender = midiSender;
            // For each track in the sequence.
            foreach(Track t in seq)
            {
                // Create track player for the track.
                TrackPlayer player = new TrackPlayer(this.xmidiSender, tickGen, t);
                trackPlayers.Add(player);

                // Register to be notified when the track player has reached 
                // the end of the track.
                player.EndOfTrackReached += 
                    new EventHandler(EndOfTrackReachedHandler);
            }

            activeTrackCount = trackPlayers.Count;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Turns all currently sounding notes off.
        /// </summary>
        public void AllSoundsOff()
        {
            foreach(TrackPlayer player in trackPlayers)
                player.AllSoundsOff();
        }

        /// <summary>
        /// Seeks a position within the sequence.
        /// </summary>
        /// <param name="position">
        /// The position in ticks to seek.
        /// </param>
        public void Seek(int position)
        {
            // Reset the active track count.
            activeTrackCount = trackPlayers.Count;

            // Seek position for each track player.
            foreach(TrackPlayer player in trackPlayers)
                player.Seek(position);
        }

		//JB
		public ArrayList TrackPlayers
		{
			get
			{
				return this.trackPlayers;
			}
//			Hashtable ht = new Hashtable();
//			foreach (TrackPlayer tp in this.trackPlayers)
//				ht.Add(tp.Track.Name, tp);
//			return ht;
		}
		public TrackPlayer GetTrackPlayer(int index)
		{
			return (TrackPlayer)this.trackPlayers[index];
		}
		public TrackPlayer GetTrackPlayer(string name)
		{
			foreach (TrackPlayer tp in this.trackPlayers)
				if (tp.Track.Name == name)
					return tp;
			return null;
		}

        /// <summary>
        /// Sets the mute state of a track.
        /// </summary>
        /// <param name="index">
        /// Index into the sequence of the track to solo.
        /// </param>
        /// <param name="mute">
        /// A value indicating whether or not to mute the track.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the track index is out of range.
        /// </exception>
        public void MuteTrack(int index, bool mute)
        {
            // Enforce preconditions.
            if(index < 0 || index >= trackPlayers.Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Track index out of range.");

            ((TrackPlayer)trackPlayers[index]).Mute = mute;
        }

        /// <summary>
        /// Sets the solo state of a track.
        /// </summary>
        /// <param name="index">
        /// Index into the sequence of the track to solo.
        /// </param>
        /// <param name="solo">
        /// A value indicating whether or not to solo the track.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the track index is out of range.
        /// </exception>
        public void SoloTrack(int index, bool solo)
        {
            // Enforce preconditions.
            if(index < 0 || index >= trackPlayers.Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Track index out of range.");

            // Get the track player responsible for the track at the specified
            // index.
            TrackPlayer player = (TrackPlayer)trackPlayers[index];

            // Guard.
            if(player.Solo == solo)
                return;

            // Set solo value.
            player.Solo = solo;

            // If the track is being soloed.
            if(solo)
            {
                // If no tracks have been soloed so far.
                if(soloCount == 0)
                {
                    // For each track player, enable solo mode and turn off all
                    // sounds of those tracks not soloed.
                    foreach(TrackPlayer p in trackPlayers)
                    {
                        p.SoloModeEnabled = true;

                        if(!p.Solo)
                            p.AllSoundsOff();
                    }
                }

                // Keep track of the number of tracks soloed.
                soloCount++;
            }
            // Else the track is not being soloed.
            else
            {
                // Keep track of the number of tracks soloed.
                soloCount--;

                // If no tracks are soloed, disable the solo mode of each track
                // player.
                if(soloCount == 0)
                    foreach(TrackPlayer p in trackPlayers)
                        p.SoloModeEnabled = false;
            }
        }

        /// <summary>
        /// Keeps track of how many tracks are still playing.
        /// </summary>
        /// <param name="sender">
        /// The track responsible for triggering the event.
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void EndOfTrackReachedHandler(object sender, EventArgs e)
        {
            // One less track is now playing.
            activeTrackCount--;

            // If there are no more tracks playing, raise end of sequence 
            // event.
            if(activeTrackCount == 0)
                OnEndOfSequenceReached();
        }

        /// <summary>
        /// Raises the end of sequence event.
        /// </summary>
        private void OnEndOfSequenceReached()
        {
            if(EndOfSequenceReached != null)
                EndOfSequenceReached(this, EventArgs.Empty);
        }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Disposes of the sequence player.
        /// </summary>
        public void Dispose()
        {
            foreach(TrackPlayer player in trackPlayers)
            {
                player.EndOfTrackReached -= new EventHandler(EndOfTrackReachedHandler);
                player.Dispose();
            }
        }

        #endregion
    }
}
