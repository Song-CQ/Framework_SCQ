/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/24/2004
 */

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Endogine.Midi
{
    /// <summary>
    /// Represents Midi output device capabilities.
    /// </summary>
    public struct MidiOutCaps
    {
        #region MidiOutCaps Members

        /// <summary>
        /// Manufacturer identifier of the device driver for the Midi output 
        /// device. 
        /// </summary>
        public short mid; 

        /// <summary>
        /// Product identifier of the Midi output device. 
        /// </summary>
        public short pid; 

        /// <summary>
        /// Version number of the device driver for the Midi output device. The 
        /// high-order byte is the major version number, and the low-order byte 
        /// is the minor version number. 
        /// </summary>
        public int driverVersion;

        /// <summary>
        /// Product name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
        public byte[] name; 

        /// <summary>
        /// Flags describing the type of the Midi output device. 
        /// </summary>
        public short technology; 

        /// <summary>
        /// Number of voices supported by an internal synthesizer device. If 
        /// the device is a port, this member is not meaningful and is set 
        /// to 0. 
        /// </summary>
        public short voices; 

        /// <summary>
        /// Maximum number of simultaneous notes that can be played by an 
        /// internal synthesizer device. If the device is a port, this member 
        /// is not meaningful and is set to 0. 
        /// </summary>
        public short notes; 

        /// <summary>
        /// Channels that an internal synthesizer device responds to, where the 
        /// least significant bit refers to channel 0 and the most significant 
        /// bit to channel 15. Port devices that transmit on all channels set 
        /// this member to 0xFFFF. 
        /// </summary>
        public short channelMask; 

        /// <summary>
        /// Optional functionality supported by the device. 
        /// </summary>
        public int support; 

        #endregion
    }

	/// <summary>
	/// Represents Midi output devices.
	/// </summary>
	public class OutputDevice : Component, IMidiDevice, IMidiSender
	{
        #region OutputDevice Members

        #region Delegates

        // Represents the method handles messages from Windows.
        private delegate void MidiOutProc(int handle, int msg, int instance,
            int param1, int param2); 

        #endregion
 
        #region Win32 Midi Output Functions and Constants

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref int handle, int deviceID,
            MidiOutProc proc, int instance, int flags); 

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(int handle);

        [DllImport("winmm.dll")]
        private static extern int midiOutReset(int handle);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(int handle, int message);

        [DllImport("winmm.dll")]
        private static extern int midiOutPrepareHeader(int handle, 
            ref MidiHeader midiHeader, int sizeOfmidiHeader);
        
        [DllImport("winmm.dll")]
        private static extern int midiOutUnprepareHeader(int handle, 
            ref MidiHeader midiHeader, int sizeOfmidiHeader); 

        [DllImport("winmm.dll")]
        private static extern int midiOutLongMsg(int handle, 
            ref MidiHeader midiHeader, int sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(int handle, 
            ref MidiOutCaps caps, int sizeOfmidiOutCaps);
        
        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        private const int MMSYSERR_NOERROR = 0;
        private const int CALLBACK_FUNCTION = 0x30000;
        private const int MOM_DONE = 0x3C9;

        #endregion

        #region Constants

        // The amount to shift packed Midi short messages to omit the status
        // value.
        private const int StatusShift = 8;

        #endregion

        #region Fields

        // Running status enabled.
        private bool runningStatusEnabled = true;

        // Running status value.
        private int runningStatus = 0;

        // Device handle.
        private int handle;

        // device Identifier.
        private int deviceID;

        // Indicates whether or not the device is open.
        private bool opened;

        // Represents the method that handles messages from Windows.
        private MidiOutProc messageHandler;

        // Thread for managing headers.
        private Thread headerManager;

        // Event used to signal when the device is done with a header.
        private AutoResetEvent resetEvent = new AutoResetEvent(false);

        // Queue for storing headers.
        private Queue headerQueue = new Queue();

        // Synchronized queue.
        private Queue syncHeaderQueue; 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        #region Construction/Destruction

        /// <summary>
        /// Initializes a new instance of the OutputDevice class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes a new instance of the OutputDevice 
        /// class in a closed state.
        /// </remarks>
        public OutputDevice()
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            InitializeComponent();

            InitializeOutputDevice();            
        }

        /// <summary>
        /// Initializes a new instance of the OutputDevice class with the
        /// specified IContainer.
        /// </summary>
        /// <param name="container">
        /// The container to add this component to.
        /// </param>
        /// <remarks>
        /// This constructor initializes a new instance of the OutputDevice 
        /// class in a closed state.
        /// </remarks>
        public OutputDevice(IContainer container)
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            container.Add(this);
            InitializeComponent();

            InitializeOutputDevice();
        }

        /// <summary>
        /// Initializes a new instance of the OutputDevice class with the 
        /// specified device Identifier.
        /// </summary>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while opening the output device.
        /// </exception>
        /// <remarks>
        /// This constructor initializes a new instance of the OutputDevice 
        /// class and opens it with the specified device Identifier.
        /// </remarks>
        public OutputDevice(int deviceID)
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            InitializeComponent();

            InitializeOutputDevice();

            // Open device.
            Open(deviceID);
        }

        /// <summary>
        /// Initializes a new instance of the OutputDevice class with the 
        /// specified device Identifier.
        /// </summary>
        /// <param name="container">
        /// The container to add this component to.
        /// </param>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while opening the output device.
        /// </exception>
        /// <remarks>
        /// This constructor initializes a new instance of the OutputDevice 
        /// class and opens it with the specified device Identifier.
        /// </remarks>
        public OutputDevice(IContainer container, int deviceID)
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            container.Add(this);
            InitializeComponent();

            InitializeOutputDevice();

            // Open device.
            Open(deviceID);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~OutputDevice()
        {
            // If the device is still open.
            if(IsOpen())
            {
                midiOutClose(handle);
            }
        }

        #endregion

        #region Methods

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }

                if(IsOpen())
                {
                    Close();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion        
        
        /// <summary>
        /// Gets the output device capabilities.
        /// </summary>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while retrieving the output device
        /// capabilities.
        /// </exception>
        /// <returns>
        /// The Midi output device's capabilities.
        /// </returns>
        public static MidiOutCaps GetCapabilities(int deviceID)
        {
            MidiOutCaps caps = new MidiOutCaps();

            ThrowOnError(midiOutGetDevCaps(deviceID, ref caps, 
                Marshal.SizeOf(caps)));

            return caps;
        }

        /// <summary>
        /// Initializes output device.
        /// </summary>
        private void InitializeOutputDevice()
        {
            // Create delegate for handling messages from Windows.
            messageHandler = new MidiOutProc(OnMessage);

            // Create synchronized queue for holding headers.
            syncHeaderQueue = Queue.Synchronized(headerQueue);
        }

        /// <summary>
        /// Handles messages from Windows.
        /// </summary>
        private void OnMessage(int handle, int msg, int instance, int param1, 
            int param2)
        {
            // If the device has finished sending a system exclusive 
            // message.
            if(msg == MOM_DONE)
            {
                // If the device is open
                if(IsOpen())
                {
                    // Signal header thread that the device has finished with 
                    // a header.
                    resetEvent.Set();
                }
            }
        }

        /// <summary>
        /// Throws exception on error.
        /// </summary>
        /// <param name="errCode">
        /// The error code. 
        /// </param>
        private static void ThrowOnError(int errCode)
        {
            // If an error occurred
            if(errCode != MMSYSERR_NOERROR)
            {
                // Throw exception.
                throw new OutputDeviceException(errCode);
            }
        }

        /// <summary>
        /// Thread method for managing headers.
        /// </summary>
        private void ManageHeaders()
        {
            // While the device is open.
            while(IsOpen())
            {
                // Wait to be signalled when a header had finished being used.
                resetEvent.WaitOne();

                // While there are still headers in the queue.
                while(syncHeaderQueue.Count > 0)
                {
                    // Get header from the front of the queue.
                    MidiHeader header = (MidiHeader)syncHeaderQueue.Dequeue();

                    // Unprepare header.
                    int result = midiOutUnprepareHeader(handle, ref header, 
                        Marshal.SizeOf(header));

                    // If an error occurred with unpreparing the system 
                    // exclusive header.
                    if(result != MMSYSERR_NOERROR)
                    {
                        if(SysExHeaderErrorOccurred != null)
                        {
                            OutputDeviceException ex = 
                                new OutputDeviceException(result);

                            SysExHeaderErrorOccurred(this, 
                                new SysExHeaderErrorEventArgs(ex.Message));
                        }
                    }

                    // Free memory allocated for the system exclusive data.
                    Marshal.FreeHGlobal(header.data);
                }
            }
        }

        /// <summary>
        /// Empties header queue.
        /// </summary>
        private void EmptyHeaderQueue()
        {
            IEnumerator en = syncHeaderQueue.GetEnumerator();

            // While there are still headers in the queue.
            while(en.MoveNext())
            {
                // Get header.
                MidiHeader header = (MidiHeader)en.Current;

                // Unprepare header.
                ThrowOnError(midiOutUnprepareHeader(handle, ref header, 
                    Marshal.SizeOf(header)));

                // Free memory allocated for the system exclusive data.
                Marshal.FreeHGlobal(header.data);
            }

            // Clear queue.
            syncHeaderQueue.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of output devices present in the system.
        /// </summary>
        public static int DeviceCount
        {
            get
            {
                return midiOutGetNumDevs();
            }
        }

        #endregion

        #endregion

        #region IMidiDevice Members

        #region Events

        /// <summary>
        /// Occures when the MIDI receiver encounters an error processing 
        /// system exclusive headers.
        /// </summary>
        public event SysExHeaderErrorHandler SysExHeaderErrorOccurred;

        #endregion

        #region Methods
        
        /// <summary>
        /// Opens the OutputDevice with the specified device Identifier.
        /// </summary>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while opening the output device.
        /// </exception>
        public void Open(int deviceID)
        {
            // If the device is already open.
            if(IsOpen())
            {
                // Close device before attempting to open it again.
                Close();
            }            

            // Open the device.
            ThrowOnError(midiOutOpen(ref handle, deviceID, messageHandler, 
                0, CALLBACK_FUNCTION));

            // Indicate the the device is now open.
            opened = true;

            // Create thread for managing headers.
            headerManager = new Thread(new ThreadStart(ManageHeaders));

            // Start thread.
            headerManager.Start();

            // Wait for thread to become active.
            while(!headerManager.IsAlive)
                continue;

            // Keep track of device Identifier.
            this.deviceID = deviceID;            
        }

        /// <summary>
        /// Closes the OutputDevice.
        /// </summary>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while closing the output device.
        /// </exception>
        public void Close()
        {
            // If the device is open.
            if(IsOpen())
            {
                // Indicate that the device is closed.
                opened = false; 

                // Reset device.
                ThrowOnError(midiOutReset(handle));

                // Signal header thread that it is finished.
                resetEvent.Set();

                // Wait for header thread to finish.
                headerManager.Join();

                // Empty header queue.
                EmptyHeaderQueue();

                // Close device.
                ThrowOnError(midiOutClose(handle)); 
            }
        }

        /// <summary>
        /// Indicates whether or not the OutputDevice is open.
        /// </summary>
        /// <returns>
        /// true if the device is open; otherwise, false.
        /// </returns>
        public bool IsOpen()
        {
            return opened;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the device handle.
        /// </summary>
        public int DeviceHandle
        {
            get
            {
                return handle;
            }
        }

        /// <summary>
        /// Gets the device Identifier.
        /// </summary>
        public int DeviceID
        {
            get
            {
                return deviceID;
            }
        }

        #endregion

        #endregion

        #region IMidiSender Members

        /// <summary>
        /// Sends a channel message.
        /// </summary>
        /// <param name="message">
        /// The channel message to send.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while sending the message.
        /// </exception>
        public void Send(ChannelMessage message)
        {
            // Guard.
            if(!IsOpen())
                return;

            // If running status is enabled.
            if(runningStatusEnabled)
            {
                // If running status matches the status value of the message.
                if(runningStatus == message.Status)
                {
                    // Remove status value.
                    int msg = message.Message >> StatusShift;

                    // Send message without status value.
                    ThrowOnError(midiOutShortMsg(handle, msg));
                }
                // Else the running status does not match the status value of 
                // the message.
                else
                {
                    // Send message with status value.
                    ThrowOnError(midiOutShortMsg(handle, message.Message));

                    // Update running status.
                    runningStatus = message.Status;
                }
            }
        }

        /// <summary>
        /// Sends a system realtime message.
        /// </summary>
        /// <param name="message">
        /// The system realtime message to send.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while sending the message.
        /// </exception>
        public void Send(SysRealtimeMessage message)
        {
            // Guard.
            if(!IsOpen())
                return;

            ThrowOnError(midiOutShortMsg(handle, message.Message));
        }

        /// <summary>
        /// Sends a system common message.
        /// </summary>
        /// <param name="message">
        /// The system common message to send.
        /// </param>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while sending the message.
        /// </exception>
        public void Send(SysCommonMessage message)
        {
            // Guard.
            if(!IsOpen())
                return;

            ThrowOnError(midiOutShortMsg(handle, message.Message));

            // System common messages cancel running status.
            runningStatus = 0;
        }

        /// <summary>
        /// Sends system exclusive messages.
        /// </summary>
        /// <param name="message">
        /// The system exclusive message to send.
        /// </param>
        public void Send(SysExMessage message)
        {
            // Guard.
            if(!IsOpen())
                return;

            // Create header.
            MidiHeader header = new MidiHeader();

            // System exclusive message data.
            string msg = message.Message;

            //
            // Initialize header.
            //

            header.data = Marshal.StringToHGlobalAnsi(msg);
            header.bufferLength = msg.Length;
            header.flags = 0;

            // Prepare header.
            ThrowOnError(midiOutPrepareHeader(handle, ref header, 
                Marshal.SizeOf(header)));
                
            // Place header in queue to be retrieved later.
            syncHeaderQueue.Enqueue(header);

            // Send message.
            ThrowOnError(midiOutLongMsg(handle, ref header, 
                Marshal.SizeOf(header)));

            // System exclusive messages cancel running status.
            runningStatus = 0;
        }

        /// <summary>
        /// Turns off all Note on all Midi channels for the output device.
        /// </summary>
        /// <exception cref="OutputDeviceException">
        /// Thrown if an error occurred while resetting the output device.
        /// </exception>
        public void Reset()
        {
            // Guard.
            if(!IsOpen())
                return;

            // Reset device.
            ThrowOnError(midiOutReset(handle));   
             
            // Empty header queue. Any system exclusive messages in the 
            // queue will be marked done at this point.
            EmptyHeaderQueue();

            // Reset running status.
            runningStatus = 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use a running
        /// status.
        /// </summary>
        public bool RunningStatusEnabled
        {
            get
            {
                return runningStatusEnabled;
            }
            set
            {
                runningStatusEnabled = value;
            }
        }

        #endregion
	}

    /// <summary>
    /// The exception that is thrown when a error occurs with the OutputDevice
    /// class.
    /// </summary>
    public class OutputDeviceException : ApplicationException
    {
        #region OutputDeviceException Members

        #region Win32 Midi Output Error Function

        [DllImport("winmm.dll")]
        private static extern int midiOutGetErrorText(int errCode, 
            StringBuilder message, int sizeOfMessage);

        #endregion

        #region Fields

        // Error message.
        private StringBuilder message = new StringBuilder(128);

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the OutputDeviceException class with
        /// the specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        public OutputDeviceException(int errCode)
        {
            // Get error message.
            midiOutGetErrorText(errCode, message, message.Capacity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return message.ToString();
            }
        }

        #endregion

        #endregion
    }
}
