/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/12/2004
 */

using System;
using Multimedia;

namespace Endogine.Midi
{
	/// <summary>
	/// Provides easy to use functionality for song position pointer messages.
	/// </summary>
	public class SongPositionPointer : ISysCommonMessageAdaptor
	{
        #region Constants

        // The number of ticks per 16th note.
        private const int TicksPer16thNote = 6;

        // Used for packing and unpacking the song position.
        private const int Shift = 7;

        // Used for packing and unpacking the song position.
        private const int Mask = 127;

        #endregion

        #region Fields

        // The adapted system common message.
        private SysCommonMessage message;

        // The scale used for converting from the song position to the position
        // in ticks.
        private int tickScale;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes an instance of the SongPositionPointer class with the
        /// specified pulses per quarter note.
        /// </summary>
        /// <param name="ppqn">
        /// Pulses per quarter note resolution.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the pulses per quarter note value is invalid.
        /// </exception>
        public SongPositionPointer(int ppqn)
        {
            // Enforce preconditions
            if(ppqn < TickGenerator.PpqnMin || 
                ppqn % TickGenerator.PpqnMin != 0)
                throw new ArgumentException(
                    "Pulses per quarter note invalid value.", "ppqn");

            message = new SysCommonMessage(SysCommonType.SongPositionPointer);

            tickScale = ppqn / TickGenerator.PpqnMin;            
        }

        /// <summary>
        /// Initializes a new instance of the SongPositionPointer class with 
        /// the specified system common message and pulses per quarter note.
        /// </summary>
        /// <param name="ppqn">
        /// Pulses per quarter note resolution.
        /// </param>
        /// <param name="message">
        /// The system common message to use for initialization.
        /// </param>      
        /// <exception cref="ArgumentException">
        /// If the specified message is not a song position pointer message or
        /// the pulses per quarter note value is invalid.
        /// </exception>
		public SongPositionPointer(int ppqn, SysCommonMessage message)
		{
            // Enforce preconditions.
            if(ppqn < TickGenerator.PpqnMin || 
                ppqn % TickGenerator.PpqnMin != 0)
                throw new ArgumentException(
                    "Pulses per quarter note invalid value.", "ppqn");
            else if(message.Type != SysCommonType.SongPositionPointer)
                throw new ArgumentException(
                "Not song position pointer message.", "message");            

            this.message = (SysCommonMessage)message.Clone();

            tickScale = ppqn / TickGenerator.PpqnMin;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sequence position in ticks.
        /// </summary>
        public int PositionInTicks
        {
            get
            {
                return SongPosition * tickScale * TicksPer16thNote;
            }
            set
            {
                SongPosition = value / (tickScale * TicksPer16thNote);
            }
        }

        /// <summary>
        /// Gets or sets the song position.
        /// </summary>
        public int SongPosition
        {
            get
            {
                int songPosition;

                songPosition = message.Data1;
                songPosition |= message.Data2 << Shift;

                return songPosition;
            }
            set
            {
                if(BitConverter.IsLittleEndian)
                {
                    message.Data1 = (value >> Shift) & Mask;
                    message.Data2 = value & Mask;
                }
                else
                {
                    message.Data2 = (value >> Shift) & Mask;
                    message.Data1 = value & Mask;
                }
            }
        }

        #endregion


        #region ISysCommonMessageAdaptor Members

        /// <summary>
        /// Returns a clone of the adapted song position pointer message.
        /// </summary>
        /// <returns>
        /// A clone of the adapted song position pointer message.
        /// </returns>
        public SysCommonMessage ToMessage()
        {
            return (SysCommonMessage)message.Clone();
        }

        #endregion
    }
}
