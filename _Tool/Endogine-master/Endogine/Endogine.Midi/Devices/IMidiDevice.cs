/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/24/2004
 */

using System;

namespace Endogine.Midi
{
	/// <summary>
	/// Represents the basic functionality provided by MIDI devices.
	/// </summary>
	public interface IMidiDevice
	{
        /// <summary>
        /// Occures when the MIDI device encounters an error processing 
        /// system exclusive headers.
        /// </summary>
        event SysExHeaderErrorHandler SysExHeaderErrorOccurred;

        /// <summary>
        /// Opens the MIDI device with the specified device ID.
        /// </summary>
        /// <param name="deviceID">
        /// The device ID.
        /// </param>
        void Open(int deviceID);

        /// <summary>
        /// Closes the MIDI device.
        /// </summary>
        void Close();

        /// <summary>
        /// Indicates whether or not the MIDI device is open.
        /// </summary>
        /// <returns>
        /// <b>true</b> if the MIDI device is open; otherwise, <b>false</b>.
        /// </returns>
        bool IsOpen();

        /// <summary>
        /// Gets the MIDI device's handle.
        /// </summary>
        int DeviceHandle
        {
            get;
        }

        /// <summary>
        /// Gets the MIDI device's ID.
        /// </summary>
        int DeviceID
        {
            get;
        }
	}

    /// <summary>
    /// Represents the method for handling system exclusive header errors.
    /// </summary>
    public delegate void SysExHeaderErrorHandler(object sender, SysExHeaderErrorEventArgs e);

    /// <summary>
    /// Provides data for the SysExHeaderErrorOccurred event.
    /// </summary>
    public class SysExHeaderErrorEventArgs : EventArgs
    {
        #region SysExHeaderErrorEventArgs Members

        #region Fields

        private string message;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysExHeaderErrorEventArgs class 
        /// with the specified string message.
        /// </summary>
        /// <param name="message">
        /// The text message describing the error.
        /// </param>
        public SysExHeaderErrorEventArgs(string message)
        {
            this.message = message;
        }

        #endregion

        #region Properties        

        /// <summary>
        /// Gets the text description of the error.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
        }

        #endregion

        #endregion
    }
}
