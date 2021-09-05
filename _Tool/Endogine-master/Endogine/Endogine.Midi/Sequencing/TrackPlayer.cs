/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/29/2004
 */

using System;
using System.Collections;

namespace Endogine.Midi
{
	//JB why internal?
	/// <summary>
	/// Handles playback of a single Track.
	/// </summary>
	public class TrackPlayer : MidiMessageVisitor, IDisposable
	{
		//JB
		public delegate void TrackEventDelegate(object sender, Endogine.Midi.MidiEvent anEvent);
		public event TrackEventDelegate TrackEvent;
		public delegate void TrackMessageDelegate(object sender, Endogine.Midi.IMidiMessage message);
		public event TrackMessageDelegate TrackMessage;

        #region TrackPlayer Members

        #region Constants

        // Minimum value for turning on the hold pedal.
        private const int HoldPedalOnValue = 64;

        #endregion

        #region Fields

        // Indicates whether or not the track is muted.
        private bool mute = false;

        // Indicates whether or not the track is soloed.
        private bool solo = false;

        // Indicates whether or not the solo mode is enabled.
        private bool soloModeEnabled = false;

        // The MIDI sender for sending MIDI messages.
//        private IMidiSender midiSender;

        // The tick generator used for generating ticks.
        private TickGenerator tickGen;

        // The track to playback.
        private Track trk;

        // The enumerator for iterating through the track's MIDI events.
        private IEnumerator enumerator;

        // Indicates whether or not the end of the track has been reached.
        private bool endReached;

        // The current MIDI event to play.
        private MidiEvent currentEvent;

		private IMidiSender _midiSender;

        // The table to keep track of all currently playing notes.
        private ChannelMessage[] noteOnTable =
            new ChannelMessage[ShortMessage.DataValueMax + 1];

        // The hold pedal message. Should be null if the pedal is not currently
        // on.
        private ChannelMessage holdPedalMessage = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the end of the track has been reached.
        /// </summary>
        public event EventHandler EndOfTrackReached;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the TrackPlayer class with the 
        /// specified MIDI sender, tick generator, and track.
        /// </summary>
        /// <param name="midiSender">
        /// The MIDI sender to use for sending MIDI messages.
        /// </param>
        /// <param name="tickGen">
        /// The tick generator used for timing the playback of MIDI messages.
        /// </param>
        /// <param name="trk">
        /// The track to play back.
        /// </param>
		public TrackPlayer(IMidiSender midiSender, TickGenerator tickGen, Track trk)
		{
            this._midiSender = midiSender;
            this.tickGen = tickGen;
            this.trk = trk;

            tickGen.Tick += new EventHandler(TickHandler);

            // Set playback position at the beginning of the track.
            Seek(0);
		}

        #endregion

        #region Methods

        /// <summary>
        /// Turns all currently sounding notes off.
        /// </summary>
        /// <remarks>
        /// When a track is muted, all of the currently sounding notes need to
        /// be turned off. Also, a sequencer can call this method if one of the
        /// other tracks are soloed.
        /// </remarks>
        public void AllSoundsOff()
        {
            ChannelMessage message;

            // If the hold pedal is currently on, turn it off.
            if(holdPedalMessage != null)                
            {
                message =
                    new ChannelMessage(ChannelCommand.Controller, 
                    holdPedalMessage.MidiChannel, holdPedalMessage.Data1, 0);

                //this._midiSender.Send(message);
				this.SendMessage(message);
            }

            // Created channel message for turning notes off.
            message = new ChannelMessage(ChannelCommand.NoteOff, 0, 0, 0);

            // Iterate through the note-on table turning off any currently
            // turned on notes.
            for(int i = 0; i < ShortMessage.DataValueMax; i++)
            {
                // If the note has been turned on, turn it off.
                if(noteOnTable[i] != null)
                {
                    message.MidiChannel = noteOnTable[i].MidiChannel;
                    message.Data1 = noteOnTable[i].Data1;
                    //this._midiSender.Send(message);
					this.SendMessage(message);
                }
            }

            holdPedalMessage = null;
            ResetNoteOnTable();
        }

        /// <summary>
        /// Seeks the specified position in the track.
        /// </summary>
        /// <param name="position">
        /// The position in ticks to seek.
        /// </param>
        public void Seek(int position)
        {
            // Enforce preconditions.
            if(position < 0)
                throw new ArgumentOutOfRangeException("position", position,
                    "Position out of range.");

            enumerator = trk.GetEnumerator();
            enumerator.MoveNext();
            currentEvent = (MidiEvent)enumerator.Current;

            endReached = false;

            // Tick accumulator.
            int ticks = currentEvent.Ticks;

            // Create MIDI chaser to chase MIDI messages so that the sequence 
            // is updated correctly at the specified position.
            MidiChaser chaser = new MidiChaser(this._midiSender, tickGen);

            if(ticks <= position)
            {
                // Add first message.
                chaser.Add(currentEvent.Message);
            }

            // While the position being sought has not been reached.
            while(ticks < position)
            {
                // Move to the next event in the track.
                endReached = !enumerator.MoveNext();

                // If the position being sought lies beyond the end of the 
                // track, trigger event to notify listeners and return.
                if(endReached)
                {
                    if(EndOfTrackReached != null)
                        EndOfTrackReached(this, EventArgs.Empty);

                    // Chase MIDI messages so that the sequence sounds correctly from 
                    // the specified position.
                    chaser.Chase();
                    
                    return;
                }
                    // Else the position has not yet been reached.
                else
                {
                    // Get the current MIDI event.
                    currentEvent = (MidiEvent)enumerator.Current;

                    // Accumulate ticks.
                    ticks += currentEvent.Ticks;

                    // If we haven't gone beyond the specified position.
                    if(ticks <= position)
                    {
                        // Add message to chaser.
                        chaser.Add(currentEvent.Message);
                    }
                }
            }

            // Initialize the current MIDI event ticks to the number of ticks 
            // remaining until the specified position is reached.
            currentEvent.Ticks = ticks - position;

            // Chase MIDI messages so that the sequence sounds correctly from 
            // the specified position.
            chaser.Chase();
        }

        /// <summary>
        /// Resets the table for keeping track of note-on messages.
        /// </summary>
        private void ResetNoteOnTable()
        {
            for(int i = 0; i < noteOnTable.Length; i++)
                noteOnTable[i] = null;
        }

        /// <summary>
        /// Visit channel messages.
        /// </summary>
        /// <param name="message">
        /// The channel message to visit.
        /// </param>
        public override void Visit(ChannelMessage message)
        {
            // If this is a note-on message.
            if(message.Command == ChannelCommand.NoteOn)
            {
                // If the velocity is greater than zero, keep track of message.
                // This message turns a note on.
                if(message.Data2 > 0)
                {
                    noteOnTable[message.Data1] = message;
                }
                // Else the velocity is zero, treat this note-on message as a
                // note off message.
                else
                {
                    // Indicate that this note is not currently playing.
                    noteOnTable[message.Data1] = null;
                }

                // Send with filtering.
                Send(message);
            }
            // Else if this is a note-off message.
            else if(message.Command == ChannelCommand.NoteOff)
            {
                // Indicate that this note is note currently playing.
                noteOnTable[message.Data1] = null;

                // Send with filtering.
                Send(message);
            }
            // Else if this is a controller message.
            else if(message.Command == ChannelCommand.Controller)
            {                 
                // If this is a hold pedal message.
                if(message.Data1 == (int)ControllerType.HoldPedal)
                {
                    // If the hold pedal is on, indicate that the hold pedal
                    // is on.
                    if(message.Data2 >= HoldPedalOnValue)
                        holdPedalMessage = message;
                    // Else the hold pedal is off, indicate that it is off.
                    else
                        holdPedalMessage = null;
                }

                // Send message without filtering.
                //this._midiSender.Send(message);
				this.SendMessage(message);
            }
            // Else this is another type of MIDI message.
            else
            {
                // Send message without filtering.
				this.SendMessage(message);
				//this._midiSender.Send(message);
            }
        }

        /// <summary>
        /// Visit meta messages.
        /// </summary>
        /// <param name="message">
        /// The message to visit.
        /// </param>
        public override void Visit(MetaMessage message)
        {
            // If this is a tempo change message, change the tempo of the tick
            // generator.
            if(message.Type == MetaType.Tempo)
            {
                TempoChange change = new TempoChange(message);
				//tickGen.Tempo = change.Tempo;
				tickGen.Tempo = 60000000f/change.Tempo;
            }
        }

        /// <summary>
        /// Visit system exclusive messages.
        /// </summary>
        /// <param name="message">
        /// The message to visit.
        /// </param>
        public override void Visit(SysExMessage message)
        {
            // Send message.
			this.SendMessage(message);
			//this._midiSender.Send(message);            
        }

        /// <summary>
        /// Sends a channel message.
        /// </summary>
        /// <param name="message">
        /// The message to send.
        /// </param>
        /// <remarks>
        /// This method filters messages based on the state of the track.
        /// </remarks>
        private void Send(ChannelMessage message)
        { 
            if(mute)
                return;
            else if(soloModeEnabled && !solo)
                return;
            else
				this.SendMessage(message);
			//this._midiSender.Send(message);
        }

		protected void SendMessage(IMidiMessage message)
		{
			Type type = message.GetType();
			if (type == typeof(ChannelMessage))
				this._midiSender.Send((ChannelMessage)message);
			else if (type == typeof(SysExMessage))
				this._midiSender.Send((SysExMessage)message);

			if (TrackMessage!=null)
				TrackMessage(this, message);
		}
        /// <summary>
        /// Handles tick events generated by the tick generator.
        /// </summary>
        /// <param name="sender">
        /// The tick generator responsible for the tick event.
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void TickHandler(object sender, EventArgs e)
        {
            // Guard.
            if(endReached)
                return;

            // While it is time to play the current MIDI event.
            while(currentEvent.Ticks == 0)
            {
                // Visit the MIDI message.
                currentEvent.Message.Accept(this);

				//JB
				if (TrackEvent!=null)
					TrackEvent(this, currentEvent);

                // Move to the next MIDI event in the track.
                endReached = !enumerator.MoveNext();

                // If the end of the track has been reached, raise event.
                if(endReached)
                {
                    if(EndOfTrackReached != null)
                        EndOfTrackReached(this, EventArgs.Empty);

                    return;
                }
                // Else the end of the track has not been reached, get next 
                // MIDI event.
                else
                {
                    currentEvent = (MidiEvent)enumerator.Current;
                }
            }

            // Move the current event forward in time by one tick.
            currentEvent.Ticks--;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not the track is muted.
        /// </summary>
        public bool Mute
        {
            get
            {
                return mute;
            }
            set
            {
                // Guard.
                if(mute == value)
                    return;

                mute = value;

                // If the track is muted, turn off all currently sounding 
                // notes.
                if(mute)
                    AllSoundsOff();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the track is soloed.
        /// </summary>
        public bool Solo
        {
            get
            {
                return solo;
            }
            set
            {
                solo = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether or not the solo mode is
        /// enabled.
        /// </summary>
        /// <remarks>
        /// When a sequence is being played back and any one of the tracks is
        /// soloed, only the soloed tracks should sound; all other tracks 
        /// should be quieted. The sequencer can enable the solo mode of its 
        /// track players whenever any of its tracks are soloed. This lets the 
        /// track players know to filter its track based on whether or not it 
        /// is soloed.
        /// </remarks>
        public bool SoloModeEnabled
        {
            get
            {
                return soloModeEnabled;
            }
            set
            {
                soloModeEnabled = value;
            }
        }

        /// <summary>
        /// Gets the track being played back.
        /// </summary>
        public Track Track
        {
            get
            {
                return trk;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the track player.
        /// </summary>
        public void Dispose()
        {
			this.AllSoundsOff();
            tickGen.Tick -= new EventHandler(TickHandler);
        }

        #endregion
    }
}
