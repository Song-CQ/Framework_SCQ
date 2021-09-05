/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/21/2004
 */

using System;

namespace Endogine.Midi
{
    /// <summary>
    /// Provides easy to use functionality for meta tempo messages.
    /// </summary>
	public class TempoChange : IMetaMessageAdaptor
	{
        #region TempoChange Members

        #region Constants

        // Default tempo value.
        private const int DefaultTempo = 500000;

        // Value used for shifting bits for packing and unpacking tempo values.
        private const int Shift = 8;

        #endregion

        #region Fields

        // The tempo meta message.
        private MetaMessage message;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the TempoChange class.
        /// </summary>
        public TempoChange()
        {
            message = new MetaMessage(MetaType.Tempo, MetaMessage.TempoLength);

            Tempo = DefaultTempo;
        }

        /// <summary>
        /// Initialize a new instance of the TempoChange class with the 
        /// specified meta message.
        /// </summary>
        /// <param name="message">
        /// The meta message to use for initialization.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the specified meta message is not a tempo type.
        /// </exception>
        public TempoChange(MetaMessage message)
		{
            // Enforce preconditions.
            if(message.Type != MetaType.Tempo)
                throw new ArgumentException("Wrong meta message type.",
                    "message");

            this.message = (MetaMessage)message.Clone();
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tempo in microseconds per beat.
        /// </summary>
        public int Tempo
        {
            get
            {
                int tempo = 0;

                // If this platform uses little endian byte order.
                if(BitConverter.IsLittleEndian)
                {
                    int d = message.Length - 1;

                    // Pack tempo.
                    for(int i = 0; i < message.Length; i++)
                    {
                        tempo |= message[d] << (Shift * i);
                        d--;
                    }
                }
                // Else this platform uses big endian byte order.
                else
                {        
                    // Pack tempo.
                    for(int i = 0; i < message.Length; i++)
                    {
                        tempo |= message[i] << (Shift * i);
                    }                    
                }

                return tempo;
            }
            set
            {
                // If this platform uses little endian byte order.
                if(BitConverter.IsLittleEndian)
                {
                    int d = message.Length - 1;

                    // Unpack tempo.
                    for(int i = 0; i < message.Length; i++)
                    {
                        message[d] = (byte)(value >> (Shift * i));
                        d--;
                    }
                }
                // Else this platform uses big endian byte order.
                else
                {
                    // Unpack tempo.
                    for(int i = 0; i < message.Length; i++)
                    {
                        message[i] = (byte)(value >> (Shift * i));
                    }
                }
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
