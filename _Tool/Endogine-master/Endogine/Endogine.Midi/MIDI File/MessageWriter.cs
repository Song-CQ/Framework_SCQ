/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/06/2004
 */

using System;
using System.IO;

namespace Endogine.Midi
{
	/// <summary>
	/// Writes MIDI events to a stream.
	/// </summary>
	internal class MessageWriter : MidiMessageVisitor
	{
        #region MessageWriter Members

        #region Constants

        // Number of bits to shift bytes in writing data.
        private const int Shift = 7;

        #endregion

        #region Field

        // The stream for writing MIDI events.
        private Stream midiStream;

        // The running status.
        private int runningStatus = 0;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MessageWriter class with the
        /// specified stream.
        /// </summary>
        /// <param name="midiStream">
        /// The stream for writing MIDI events.
        /// </param>
		public MessageWriter(Stream midiStream)
		{
            this.midiStream = midiStream;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Writes the next MIDI event to the stream.
        /// </summary>
        /// <param name="e">
        /// The next MIDI event to write.
        /// </param>
        public void WriteNextEvent(MidiEvent e)
        {
            WriteVariableLengthQuantity(e.Ticks);
            e.Message.Accept(this);
        }

        /// <summary>
        /// Writes variable length values to stream.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        private void WriteVariableLengthQuantity(int value)
        {
            int v = value;
            int buffer = v & 0x7F;

            v >>= Shift;

            while(v > 0)
            {
                buffer <<= 8;
                buffer |= (v & 0x7F) | 0x80;
                v >>= Shift;
            }

            midiStream.WriteByte((byte)buffer);

            while((buffer & 0x80) == 0x80)
            {
                buffer >>= 8;
                midiStream.WriteByte((byte)buffer);
            }
        }

        /// <summary>
        /// Visits channel messages.
        /// </summary>
        /// <param name="message">
        /// The channel message to visit.
        /// </param>
        public override void Visit(ChannelMessage message)
        {
            // If the running status does not match the message's status value.
            if(message.Status != runningStatus)
            {
                // Keep track of new running status.
                runningStatus = message.Status;

                // Write status value.
                midiStream.WriteByte((byte)message.Status);                
            }
            
            // Write the first data value.
            midiStream.WriteByte((byte)message.Data1);

            // The the channel message uses two data values.
            if(ChannelMessage.BytesPerType(message.Command) > 1)
            {
                // Write the second data value.
                midiStream.WriteByte((byte)message.Data2);
            }            
        }

        /// <summary>
        /// Visits meta messages.
        /// </summary>
        /// <param name="message">
        /// The meta message to visit.
        /// </param>
        public override void Visit(MetaMessage message)
        {
            midiStream.WriteByte((byte)message.Status);
            midiStream.WriteByte((byte)message.Type);
            WriteVariableLengthQuantity(message.Length);

            for(int i = 0; i < message.Length; i++)
            {
                midiStream.WriteByte(message[i]);
            } 
        }

        /// <summary>
        /// Visits system exclusive messages.
        /// </summary>
        /// <param name="message">
        /// The system exclusive message to visit.
        /// </param>
        public override void Visit(SysExMessage message)
        {
            midiStream.WriteByte((byte)message.Status);
            WriteVariableLengthQuantity(message.Length);

            for(int i = 0; i < message.Length; i++)
            {
                midiStream.WriteByte(message[i]);
            }

            runningStatus = 0;
        }

        #endregion

        #endregion
    }
}
