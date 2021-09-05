 /*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/21/2004
 */

namespace Endogine.Midi
{
    /// <summary>
    /// Represents the basic functionality provided by a device capable of 
    /// sending Midi messages.
    /// </summary>
    public interface IMidiSender
    {        
        /// <summary>
        /// Sends a channel message.
        /// </summary>
        /// <param name="message">
        /// The channel message to send.
        /// </param>
        void Send(ChannelMessage message);

        /// <summary>
        /// Sends a system realtime message.
        /// </summary>
        /// <param name="message">
        /// The system realtime message to send.
        /// </param>
        void Send(SysRealtimeMessage message);

        /// <summary>
        /// Sends a system common message.
        /// </summary>
        /// <param name="message">
        /// The system common message to send.
        /// </param>
        void Send(SysCommonMessage message);

        /// <summary>
        /// Sends a system exclusive message.
        /// </summary>
        /// <param name="message">
        /// The system exclusive message to send.
        /// </param>
        void Send(SysExMessage message);

        /// <summary>
        /// Resets the MIDI sender.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets or sets a value indicating whether or not to use a running
        /// status.
        /// </summary>
        bool RunningStatusEnabled
        {
            get;
            set;
        }
    }
}
