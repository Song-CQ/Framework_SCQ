/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/04/2004
 */

using System;

namespace Endogine.Midi
{
    /// <summary>
    /// Provides easy to use functionality for meta time signature messages.
    /// </summary>
	public class TimeSignature : IMetaMessageAdaptor
	{
        #region TimeSignature Members

        #region Constants

        // Default numerator value.
        private const byte DefaultNumerator = 4;

        // Default denominator value.
        private const byte DefaultDenominator = 4;

        // Default clocks per metronome click value.
        private const byte DefaultClocksPerMetronomeClick = 24;

        // Default thirty second notes per quarter note value.
        private const byte DefaultThirtySecondNotesPerQuarterNote = 8;

        #endregion

        #region Fields

        // The time signature meta message.
        private MetaMessage message;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the TimeSignature class.
        /// </summary>
        public TimeSignature()
        {
            message = new MetaMessage(MetaType.TimeSignature, 
                MetaMessage.TimeSigLength);

            Numerator = DefaultNumerator;
            Denominator = DefaultDenominator;
            ClocksPerMetronomeClick = DefaultClocksPerMetronomeClick;
            ThirtySecondNotesPerQuarterNote = DefaultThirtySecondNotesPerQuarterNote;
        }

        /// <summary>
        /// Initializes a new instance of the TimeSignature class with the 
        /// specified meta message.
        /// </summary>
        /// <param name="message">
        /// The meta message to use for initialization.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the specified meta message is not a time signature type.
        /// </exception>
		public TimeSignature(MetaMessage message)
		{
            // Enforce preconditions.
            if(message.Type != MetaType.TimeSignature)
                throw new ArgumentException("Wrong meta message type.",
                    "message");

            this.message = (MetaMessage)message.Clone();
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the numerator.
        /// </summary>
        public byte Numerator
        {
            get
            {
                return message[0];
            }
            set
            {
                // Enforce preconditions.
                if(value < 1)
                    throw new ArgumentOutOfRangeException("Numerator", value,
                        "Numerator out of range.");

                message[0] = value;
            }
        }

        /// <summary>
        /// Gets or sets the denominator.
        /// </summary>
        public byte Denominator
        {
            get
            {
                return (byte)Math.Pow(2, message[1]);
            }
            set
            {
                // Enforce preconditions.
                if(value < 2 || value % 2 != 0)
                    throw new ArgumentOutOfRangeException("Denominator", value,
                        "Denominator out of range.");

                message[1] = (byte)Math.Log(value, 2);
            }
        }

        /// <summary>
        /// Gets or sets the clocks per metronome click.
        /// </summary>
        /// <remarks>
        /// Clocks per metronome click determines how many MIDI clocks occur
        /// for each metronome click.
        /// </remarks>
        public byte ClocksPerMetronomeClick
        {
            get
            {
                return message[2];
            }
            set
            {
                message[2] = value;
            }
        }

        /// <summary>
        /// Gets or sets how many thirty second notes there are for each
        /// quarter note.
        /// </summary>
        public byte ThirtySecondNotesPerQuarterNote
        {
            get
            {
                return message[3];
            }
            set
            {
                message[3] = value;
            }
        }

        #endregion

        #endregion

        #region IMetaMessageAdaptor Members

        /// <summary>
        /// Returns a clone of the adapted meta message.
        /// </summary>
        /// <returns>
        /// A clone of the adapted meta message.
        /// </returns>
        public MetaMessage ToMessage()
        {
            return (MetaMessage)message.Clone();
        }

        #endregion
    }
}
