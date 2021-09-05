/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/24/2004
 */

namespace Endogine.Midi
{
    /// <summary>
    /// Represents the method that will handle the event that occurs when an
    /// invalid short message is received.
    /// </summary>
    public delegate void InvalidShortMessageEventHandler(object sender, InvalidShortMsgEventArgs e);

    /// <summary>
    /// Represents the basic functionality provided by a device capable of 
    /// receiving Midi messages.
    /// </summary>
    public interface IMidiReceiver
    {
        /// <summary>
        /// Occurs when a channel message is received.
        /// </summary>
        event ChannelMessageEventHandler ChannelMessageReceived;

        /// <summary>
        /// Occures when a system common message is received.
        /// </summary>
        event SysCommonEventHandler SysCommonReceived;

        /// <summary>
        /// Occurs when a system exclusive message is received.
        /// </summary>
        event SysExEventHandler SysExReceived;

        /// <summary>
        /// Occurs when a system realtime message is received.
        /// </summary>
        event SysRealtimeEventHandler SysRealtimeReceived;

        /// <summary>
        /// Occures when an invalid short message is received.
        /// </summary>
        event InvalidShortMessageEventHandler InvalidShortMessageReceived;
        
        /// <summary>
        /// Starts receiving Midi messages.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops receiving Midi messages.
        /// </summary> 
        void Stop();

        /// <summary>
        /// Indicates whether or not the MIDI receiver is recording.
        /// </summary>
        /// <returns>
        /// <b>true</b> if the MIDI receiver is recording; otherwise, 
        /// <b>false</b>.
        /// </returns>
        bool IsRecording();
    }    
}