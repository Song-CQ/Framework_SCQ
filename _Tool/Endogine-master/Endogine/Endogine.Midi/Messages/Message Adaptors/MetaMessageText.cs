/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/02/2004
 */

using System;
using System.Text;

namespace Endogine.Midi
{
	/// <summary>
	/// Provides easy to use functionality for meta message text messages.
	/// </summary>
	public class MetaMessageText : IMetaMessageAdaptor
	{
        #region MetaMessageText Members

        #region Fields

        // The text based meta message.
        private MetaMessage message;

        // The meta message type - must be one of the text based types.
        private MetaType type = MetaType.Text;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MetaMessageText class.
        /// </summary>
        public MetaMessageText()
        {
            Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the MetaMessageText class with the 
        /// specified type.
        /// </summary>
        /// <param name="type">
        /// The type of meta message.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the meta message is not a text based type.
        /// </exception>
        /// <remarks>
        /// The meta message type must be one of the following text based 
        /// types:
        /// <list>
        /// <item>
        /// Copyright
        /// </item>
        /// <item>
        /// Cuepoint
        /// </item>
        /// <item>
        /// DeviceName
        /// </item>
        /// <item>
        /// InstrumentName
        /// </item>
        /// <item>
        /// Lyric
        /// </item>
        /// <item>
        /// Marker
        /// </item>
        /// <item>
        /// ProgramName
        /// </item>
        /// <item>
        /// Text
        /// </item>
        /// <item>
        /// TrackName
        /// </item>
        /// </list>
        /// If the meta message is not a text based type, an exception 
        /// will be thrown.
        /// </remarks>
        public MetaMessageText(MetaType type)
        {
            // Enforce preconditions.
            if(!IsTextType(message.Type))
                throw new ArgumentException("Not text based meta message type.",
                    "message");

            this.type = type;
            Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the MetaMessageText class with the
        /// specified meta message.
        /// </summary>
        /// <param name="message">
        /// The meta message to use for initialization.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the meta message is not a text based type.
        /// </exception>
        /// <remarks>
        /// The meta message must be one of the following text based types:
        /// <list>
        /// <item>
        /// Copyright
        /// </item>
        /// <item>
        /// Cuepoint
        /// </item>
        /// <item>
        /// DeviceName
        /// </item>
        /// <item>
        /// InstrumentName
        /// </item>
        /// <item>
        /// Lyric
        /// </item>
        /// <item>
        /// Marker
        /// </item>
        /// <item>
        /// ProgramName
        /// </item>
        /// <item>
        /// Text
        /// </item>
        /// <item>
        /// TrackName
        /// </item>
        /// </list>
        /// If the meta message is not a text based type, an exception will be 
        /// thrown.
        /// </remarks>
		public MetaMessageText(MetaMessage message)
		{
            // Enforce preconditions.
            if(!IsTextType(message.Type))
                throw new ArgumentException("Not text based meta message.",
                    "message");
           
            this.message = (MetaMessage)message.Clone();
            this.type = message.Type;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Indicates whether or not a meta message type is a text based type.
        /// </summary>
        /// <param name="type">
        /// The meta message type to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the meta message type is a text based type; 
        /// otherwise, <b>false</b>.
        /// </returns>
        private bool IsTextType(MetaType type)
        {
            if(type == MetaType.Copyright || 
                type == MetaType.CuePoint ||
                type == MetaType.DeviceName ||
                type == MetaType.InstrumentName ||
                type == MetaType.Lyric ||
                type == MetaType.Marker ||
                type == MetaType.ProgramName ||
                type == MetaType.Text ||
                type == MetaType.TrackName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text for the meta message.
        /// </summary>
        public string Text
        {
            get
            {
                ASCIIEncoding encoding = new ASCIIEncoding();

                return new string(encoding.GetChars(message.GetDataBytes()));
            }
            set
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] text = encoding.GetBytes(value);
                message = new MetaMessage(type, text);
            }
        }

        #endregion

        #endregion

        #region IMetaMessageAdaptor Members

        /// <summary>
        /// Returns a clone of the adapted text based meta message.
        /// </summary>
        /// <returns>
        /// A clone of the adapted text based meta message.
        /// </returns>
        public MetaMessage ToMessage()
        {
            return (MetaMessage)message.Clone();
        }

        #endregion
    }
}
