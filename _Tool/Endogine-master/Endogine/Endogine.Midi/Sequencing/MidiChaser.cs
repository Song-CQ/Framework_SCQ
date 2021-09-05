/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/29/2004
 */

using System;

namespace Endogine.Midi
{
	/// <summary>
	/// Provides functionality for updating playback settings.
	/// </summary>
	internal class MidiChaser : MidiMessageVisitor
	{
        #region Fields

        // The MIDI sender to use for sending MIDI messages.
        private IMidiSender midiSender;

        // The tick generator to use for setting the tempo.
        private TickGenerator tickGenerator;

        // For storing control change messages.
        private ChannelMessage[] controllers;

        // For storing the channel pressure message.
        private ChannelMessage channelPressureMessage = null;

        // For storing the poly pressure message.
        private ChannelMessage polyPressureMessage = null;

        // For storing the program change message.
        private ChannelMessage programChangeMessage = null;

        // For storing the pitch bend message.
        private ChannelMessage pitchBendMessage = null;

        // For storing the tempo change message.
        private MetaMessage tempoChangeMessage = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiChaser class with the 
        /// specified MIDI sender and tick generator.
        /// </summary>
        /// <param name="midiSender">
        /// The MIDI sender to use for sending MIDI messages.
        /// </param>
        /// <param name="tickGenerator">
        /// The tick generator to use for setting the tempo.
        /// </param>
		public MidiChaser(IMidiSender midiSender, TickGenerator tickGenerator)
		{
            this.midiSender = midiSender;
            this.tickGenerator = tickGenerator;

            controllers = new ChannelMessage[ShortMessage.DataValueMax];

            Initialize();
		}

        #endregion

        #region Methods

        /// <summary>
        /// Adds a MIDI message to the chaser to use for updating the playback
        /// settings later.
        /// </summary>
        /// <param name="message">
        /// The MIDI message to add to the chaser.
        /// </param>
        /// <remarks>
        /// Only certain MIDI messages are accepted by the chaser. If the
        /// message is not accepted, no operation occurs.
        /// </remarks>
        public void Add(IMidiMessage message)
        {
            message.Accept(this);           
        }

        /// <summary>
        /// Update playback settings based on the stored set of MIDI messages.
        /// </summary>
        public void Chase()
        {
            if(channelPressureMessage != null)
                midiSender.Send(channelPressureMessage);

            if(polyPressureMessage != null)
                midiSender.Send(polyPressureMessage);

            if(programChangeMessage != null)
                midiSender.Send(programChangeMessage);

            if(pitchBendMessage != null)
                midiSender.Send(pitchBendMessage);

            for(int i = 0; i < controllers.Length; i++)
            {
                if(controllers[i] != null)
                    midiSender.Send((ChannelMessage)controllers[i]);
            }

            if(tempoChangeMessage != null)
            {
                TempoChange tChange = new TempoChange(tempoChangeMessage);

                tickGenerator.Tempo = 60000000f/tChange.Tempo;
            }

            Initialize();
        }

        /// <summary>
        /// Visits channel messages.
        /// </summary>
        /// <param name="message">
        /// The channel message to visit.
        /// </param>
        /// <remarks>
        /// This method should not be called by an outside source.
        /// </remarks>
        public override void Visit(ChannelMessage message)
        {
            if(message.Command == ChannelCommand.ChannelPressure)
            {
                channelPressureMessage = message;
            }
            else if(message.Command == ChannelCommand.PolyPressure)
            {
                polyPressureMessage = message;
            }
            else if(message.Command == ChannelCommand.ProgramChange)
            {       
                programChangeMessage = message;
            }
            else if(message.Command == ChannelCommand.PitchWheel)
            {
                pitchBendMessage = message;
            }
            else if(message.Command == ChannelCommand.Controller)
            {    
                controllers[message.Data1] = message;
            }
        }

        /// <summary>
        /// Visits meta messages.
        /// </summary>
        /// <param name="message">
        /// The meta message to visit.
        /// </param>
        /// <remarks>
        /// This method should not be called by an outside source.
        /// </remarks>
        public override void Visit(MetaMessage message)
        {
            if(message.Type == MetaType.Tempo)
            {
                tempoChangeMessage = message;
            }
        }

        /// <summary>
        /// Initializes the MIDI chaser.
        /// </summary>
        private void Initialize()
        {
            channelPressureMessage = null;
            polyPressureMessage = null;
            programChangeMessage = null;
            pitchBendMessage = null;

            for(int i = 0; i < controllers.Length; i++)
            {
                controllers[i] = null;
            }

            tempoChangeMessage = null;
        }

        #endregion
	}
}
