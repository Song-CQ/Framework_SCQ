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
using Multimedia;

namespace Endogine.Midi
{      
    /// <summary>
    /// Represents Midi input device capabilities.
    /// </summary>
    public struct MidiInCaps
    {
        #region MidiInCaps Members

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
        /// Optional functionality supported by the device. 
        /// </summary>
        public int support; 

        #endregion
    }

	/// <summary>
	/// Represents Midi input devices.
	/// </summary>
	public class InputDevice : Component, IMidiDevice, IMidiReceiver
	{
        #region InputDevice Members

        #region Delegates

        // Represents the method that handles messages from Windows.
        private delegate void MidiInProc(int handle, int msg, int instance,
            int param1, int param2); 

        // Represents the method that handles system exclusive headers.
        private delegate void SysExHeaderHandler(IntPtr header);
 
        #endregion

        #region Win32 Midi Input Functions and Constants
 
        [DllImport("winmm.dll")]
        private static extern int midiInOpen(ref int handle, int deviceID,
            MidiInProc proc, int instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiInClose(int handle);

        [DllImport("winmm.dll")]
        private static extern int midiInStart(int handle);

        [DllImport("winmm.dll")]
        private static extern int midiInReset(int handle);

        [DllImport("winmm.dll")]
        private static extern int midiInPrepareHeader(int handle, 
            IntPtr header, int sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiInUnprepareHeader(int handle, 
            IntPtr header, int sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiInAddBuffer(int handle, 
            IntPtr header, int sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiInGetDevCaps(int handle, 
            ref MidiInCaps caps, int sizeOfmidiInCaps);

        [DllImport("winmm.dll")]
        private static extern int midiInGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiConnect(int inHandle, int outHandle, 
            int reserved);         

        [DllImport("winmm.dll")]
        private static extern int midiDisconnect(int inHandle, int outHandle, 
            int reserved); 

        private const int MMSYSERR_NOERROR = 0;
        private const int CALLBACK_FUNCTION = 0x30000; 
        private const int MIM_DATA = 0x3C3;
        private const int MIM_ERROR = 0x3C5;
        private const int MIM_LONGDATA = 0x3C4;
        private const int MHDR_DONE = 0x00000001;

        #endregion

        #region Constants

        // Number of system exclusive headers to use.
        private const int HeaderCount = 4;

        // System exclusive buffer size.
        private const int SysExBufferSize = 32000;

        #endregion

        #region Fields

        // Device handle.
        private int handle;

        // Device Identifier.
        private int deviceID;

        // Indicates whether or not the device is open.
        private bool opened = false;

        // Indicates whether or not the device is recording.
        private bool recording = false;        

        // Represents the method that handles messages from Windows.
        private MidiInProc messageHandler; 
        
        // Midi headers for storing system exclusive messages.
        private MidiHeader[] headers = new MidiHeader[HeaderCount];

        // Pointers to headers. 
        private IntPtr[] ptrHeaders = new IntPtr[HeaderCount];        

        // Thread for processing system exclusive headers.
        private Thread sysExHeaderThread;

        // Queue for storing system exclusive headers ready to be processed.
        private Queue sysExHeaderQueue;

        private readonly object lockObject = new object();

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the InputDevice class.
        /// </summary>
        public InputDevice()
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            InitializeComponent();

            InitializeInputDevice();
        }

        /// <summary>
        /// Initializes an instance of the InputDevice class with the specified
        /// component container.
        /// </summary>
        /// <param name="container">
        /// The component container to add this instance of the InputDevice 
        /// class.
        /// </param>
		public InputDevice(IContainer container)
		{
			//
			// Required for Windows.Forms Class Composition Designer support
			//
			container.Add(this);
			InitializeComponent();

            InitializeInputDevice();
		}

        /// <summary>
        /// Initializes a new instance of the InputDevice class with the 
        /// specified device Id.
        /// </summary>
        /// <param name="deviceID">
        /// The device Id.
        /// </param>
        public InputDevice(int deviceID)
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            InitializeComponent();

            InitializeInputDevice();

            // Open device.
            Open(deviceID);
        }

        /// <summary>
        /// Initializes an instance of the InputDevice class with the specified
        /// component container and device Id.
        /// </summary>
        /// <param name="container">
        /// The component container to add this instance of the InputDevice 
        /// class.
        /// </param>
        /// <param name="deviceID">
        /// The device Id.
        /// </param>
        public InputDevice(IContainer container, int deviceID)
        {
            //
            // Required for Windows.Forms Class Composition Designer support
            //
            container.Add(this);
            InitializeComponent();

            InitializeInputDevice();

            // Open device.
            Open(deviceID);
        }

        #endregion        

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
                    if(IsRecording())
                    {
                        Stop();
                    }

                    Close();
                }
            }
            base.Dispose( disposing );
        }        

        /// <summary>
        /// Connects to an output device for Midi thru operations.
        /// </summary>
        /// <param name="outDevice">
        /// The output device to connect to.
        /// </param>
        public void Connect(OutputDevice outDevice)
        {
            if(IsOpen() && outDevice.IsOpen())
            {
                midiConnect(handle, outDevice.DeviceHandle, 0);
            }
        }

        /// <summary>
        /// Disconnects from an output device to cease Midi thru operations.
        /// </summary>
        /// <param name="outDevice">
        /// The output device from which to disconnect.
        /// </param>
        public void Disconnect(OutputDevice outDevice)
        {
            if(IsOpen() && outDevice.IsOpen())
            {
                midiDisconnect(handle, outDevice.DeviceHandle, 0);
            }
        }        

        /// <summary>
        /// Gets the input device capabilities.
        /// </summary>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="InputDeviceException">
        /// Thrown if an error occurred while retrieving the input device
        /// capabilities.
        /// </exception>
        /// <returns>
        /// The Midi intput device's capabilities.
        /// </returns>
        public static MidiInCaps GetCapabilities(int deviceID)
        {
            MidiInCaps caps = new MidiInCaps();

            ThrowOnError(midiInGetDevCaps(deviceID, ref caps, 
                Marshal.SizeOf(caps)));

            return caps;
        }

        /// <summary>
        /// Initializes input device.
        /// </summary>
        private void InitializeInputDevice()
        {
            // Create delegate for handling messages from Windows.
            messageHandler = new MidiInProc(OnMessage); 

            // Create queue for holding system exclusive headers ready to be
            // processed.
            sysExHeaderQueue = Queue.Synchronized(new Queue());
        }

        /// <summary>
        /// Handles messages from Windows.
        /// </summary>
        private void OnMessage(int handle, int msg, int instance,
            int param1, int param2)
        { 
            if(IsRecording())
            { 
                if(msg == MIM_DATA)
                {
                    DispatchShortMessage(param1, param2);
                }
                else if(msg == MIM_ERROR)
                {
                    DispatchInvalidShortMsg(param1, param2);
                }
                else if(msg == MIM_LONGDATA)
                {
                    ManageSysExMessage(param1, param2);
                }
            }
        }

        /// <summary>
        /// Throw exception on error.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        private static void ThrowOnError(int errCode)
        {
            // If an error occurred.
            if(errCode != MMSYSERR_NOERROR)
            {
                // Throw exception.
                throw new InputDeviceException(errCode);
            }
        }

        /// <summary>
        /// Determines the type of message received and triggers the correct
        /// event in response.
        /// </summary>
        /// <param name="message">
        /// The short Midi message received.
        /// </param>
        /// <param name="timeStamp">
        /// Number of milliseconds that have passed since the input device 
        /// began recording.
        /// </param>
        private void DispatchShortMessage(int message, int timeStamp)
        {
            // Unpack status value.
            int status = ShortMessage.UnpackStatus(message);

            // If a channel message was received.
            if(ChannelMessage.IsChannelMessage(status))
            {
                // If anyone is listening for channel messages.
                if(ChannelMessageReceived != null)
                {
                    // Create channel message.
                    ChannelMessage msg = new ChannelMessage(message);

                    // Create channel message event argument.
                    ChannelMessageEventArgs e = 
                        new ChannelMessageEventArgs(msg, timeStamp);

                    // Trigger channel message received event.
                    ChannelMessageReceived(this, e);
                }
            }
            // Else if a system common message was received
            else if(SysCommonMessage.IsSysCommonMessage(status))
            {
                // If anyone is listening for system common messages
                if(SysCommonReceived != null)
                { 
                    // Create system common message.
                    SysCommonMessage msg = new SysCommonMessage(message);                    
                    
                    // Create system common event argument.
                    SysCommonEventArgs e = new SysCommonEventArgs(msg, timeStamp);

                    // Trigger system common received event.
                    SysCommonReceived(this, e);
                }
            }
            // Else if a system realtime message was received
            else if(SysRealtimeMessage.IsSysRealtimeMessage(status))
            {
                // If anyone is listening for system realtime messages
                if(SysRealtimeReceived != null)
                {
                    // Create system realtime message.
                    SysRealtimeMessage msg = new SysRealtimeMessage(message);

                    // Create system realtime event argument.
                    SysRealtimeEventArgs e = new SysRealtimeEventArgs(msg, timeStamp);

                    // Trigger system realtime received event.
                    SysRealtimeReceived(this, e);
                }
            }
        }

        /// <summary>
        /// Handles triggering the invalid short message received event.
        /// </summary>
        /// <param name="message">
        /// The invalid short message received.
        /// </param>
        /// <param name="timeStamp">
        /// Number of milliseconds that have passed since the input device 
        /// began recording.
        /// </param>
        private void DispatchInvalidShortMsg(int message, int timeStamp)
        {
            if(InvalidShortMessageReceived != null)
            {
                InvalidShortMsgEventArgs e = 
                    new InvalidShortMsgEventArgs(message, timeStamp);

                InvalidShortMessageReceived(this, e);
            }
        }

        /// <summary>
        /// Manages system exclusive messages received by the input device.
        /// </summary>
        /// <param name="param1">
        /// Integer pointer to the header containing the received system
        /// exclusive message.
        /// </param>
        /// <param name="timeStamp">
        /// Number of milliseconds that have passed since the input device 
        /// began recording.
        /// </param>
        private void ManageSysExMessage(int param1, int timeStamp)
        {
            // Get pointer to header.
            IntPtr ptrHeader = new IntPtr(param1);

            // If anyone is listening for system exclusive messages.
            if(SysExReceived != null)
            {
                // Imprint raw pointer on to structure.
                MidiHeader header = (MidiHeader)Marshal.PtrToStructure(ptrHeader, typeof(MidiHeader));
                
                // Dispatches system exclusive messages.
                DispatchSysExMessage(header, timeStamp);
            }

            // Enqueue next system exclusive header and signal the worker queue
            // that another header is ready to be processed.
            lock(lockObject)
            {
                sysExHeaderQueue.Enqueue(ptrHeader);
                Monitor.Pulse(lockObject);
            }
        }

        /// <summary>
        /// Unprepares/prepares and adds a MIDIHDR header back to the buffer to
        /// record another system exclusive message.
        /// </summary>
        private void ManageSysExHeaders()
        {
            lock(lockObject)
            {
                while(IsRecording())
                {
                    Monitor.Wait(lockObject);

                    while(sysExHeaderQueue.Count > 0 && IsRecording())
                    {
                        IntPtr header = (IntPtr)sysExHeaderQueue.Dequeue();

                        // Unprepare header.
                        int result = midiInUnprepareHeader(handle, header, 
                            Marshal.SizeOf(typeof(MidiHeader))); 
            
                        if(result == MMSYSERR_NOERROR)
                        {
                            // Prepare header to be used again.
                            result = midiInPrepareHeader(handle, header, 
                                Marshal.SizeOf(typeof(MidiHeader))); 
                        }

                        if(result == MMSYSERR_NOERROR)
                        { 
                            // Add header back to buffer.
                            result = midiInAddBuffer(handle, header, 
                                Marshal.SizeOf(typeof(MidiHeader)));
                        }

                        if(result != MMSYSERR_NOERROR)
                        {
                            // Raise event letting clients know an error has occurred.
                            if(SysExHeaderErrorOccurred != null)
                            {
                                InputDeviceException ex = new InputDeviceException(result);
                                SysExHeaderErrorOccurred(this, 
                                    new SysExHeaderErrorEventArgs(ex.Message));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles triggering the system exclusive message received event.
        /// </summary>
        /// <param name="header">
        /// Midi header containing the system exclusive message.
        /// </param>
        /// <param name="timeStamp">
        /// Number of milliseconds that have passed since the input device 
        /// began recording.
        /// </param>
        private void DispatchSysExMessage(MidiHeader header, int timeStamp)
        {
            // Create array for holding system exclusive data.
            byte[] data = new byte[header.bytesRecorded - 1];

            // Get status byte.
            byte status = Marshal.ReadByte(header.data);
                
            // Copy system exclusive data into array (status byte is 
            // excluded).
            for(int i = 1; i < header.bytesRecorded; i++)
            {
                data[i - 1] = Marshal.ReadByte(header.data, i);
            }

            // Create message.
            SysExMessage msg = new SysExMessage((SysExType)status, data);

            // Raise event.
            SysExReceived(this, new SysExEventArgs(msg, timeStamp));
        }

        /// <summary>
        /// Create headers for system exclusive messages.
        /// </summary>
        private void CreateHeaders()
        {
            // Create headers.
            for(int i = 0; i < HeaderCount; i++)
            {
                // Initialize headers and allocate memory for system exclusive
                // data.
                headers[i].bufferLength = SysExBufferSize;
                headers[i].data = Marshal.AllocHGlobal(SysExBufferSize);

                // Allocate memory for pointers to headers. This is necessary 
                // to insure that garbage collection doesn't move the memory 
                // for the headers around while the input device is open.
                ptrHeaders[i] = 
                    Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MidiHeader)));
            }
        }

        /// <summary>
        /// Destroy headers.
        /// </summary>
        private void DestroyHeaders()
        {
            // Free memory for headers.
            for(int i = 0; i < HeaderCount; i++)
            {
                Marshal.FreeHGlobal(headers[i].data);
                Marshal.FreeHGlobal(ptrHeaders[i]);
            }
        }

        /// <summary>
        /// Unprepares headers.
        /// </summary>
        private void UnprepareHeaders()
        {
            // Unprepare each Midi header.
            for(int i = 0; i < HeaderCount; i++)
            {
                ThrowOnError(midiInUnprepareHeader(handle, ptrHeaders[i], 
                    Marshal.SizeOf(typeof(MidiHeader))));
            }
        }        

        #endregion

        #region Properties 
        
        /// <summary>
        /// Gets the number of intput devices present in the system.
        /// </summary>
        public static int DeviceCount
        {
            get
            {
                return midiInGetNumDevs();
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
        /// Opens the InputDevice with the specified device Identifier.
        /// </summary>
        /// <param name="deviceID">
        /// The device Identifier.
        /// </param>
        /// <exception cref="InputDeviceException">
        /// Thrown if an error occurred while opening the input device.
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
			//JB
			try
			{
				ThrowOnError(midiInOpen(ref handle, deviceID, messageHandler, 0, 
					CALLBACK_FUNCTION));
			}
			catch
			{
				opened = false;
				this.deviceID = -1;
				return;
			}
            // Create headers for system exclusive messages.
            CreateHeaders();            

            // Indicate that the device is open.
            opened = true;

            // Keep track of device Identifier.
            this.deviceID = deviceID;
        }     
      
        /// <summary>
        /// Closes the InputDevice.
        /// </summary>
        /// <exception cref="InputDeviceException">
        /// Thrown if an error occurred while closing the input device.
        /// </exception>
        public void Close()
        {
            // If the device is open.
            if(IsOpen())
            {
                // If the device is recording.
                if(IsRecording())
                {
                    // Stop recording before closing the device.
                    Stop();
                }

                // Destroy headers for system exclusive messages.
                DestroyHeaders();                

                // Close the device.
                ThrowOnError(midiInClose(handle));

                // Indicate that the device is closed.
                opened = false;
            }
        }

        /// <summary>
        /// Indicate whether or not the input device is open
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
        /// Gets the device ID.
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

        #region IMidiReceiver

        #region Events

        /// <summary>
        /// Occurs when a channel message is received.
        /// </summary>
        public event ChannelMessageEventHandler ChannelMessageReceived;

        /// <summary>
        /// Occurs when a system common message is received.
        /// </summary>
        public event SysCommonEventHandler SysCommonReceived;

        /// <summary>
        /// Occurs when a system exclusive message is received.
        /// </summary>
        public event SysExEventHandler SysExReceived;

        /// <summary>
        /// Occurs when a system realtime message is received.
        /// </summary>
        public event SysRealtimeEventHandler SysRealtimeReceived;

        /// <summary>
        /// Occurs when an invalid short message is received.
        /// </summary>
        public event InvalidShortMessageEventHandler InvalidShortMessageReceived;
        
        #endregion

        #region Methods

        /// <summary>
        /// Starts recording Midi messages.
        /// </summary>
        /// <exception cref="InputDeviceException">
        /// Thrown if there was an error starting the input device.
        /// </exception>
        public void Start()
        {
            // If the device is open and it is not already recording.
            if(IsOpen() && !IsRecording())
            { 
                // Initializes headers for system exclusive messages.
                for(int i = 0; i < HeaderCount; i++)
                { 
                    // Reset flags.
                    headers[i].flags = 0;

                    // Imprint header structure onto raw memory.
                    Marshal.StructureToPtr(headers[i], ptrHeaders[i], false); 

                    // Prepare header.
                    ThrowOnError(midiInPrepareHeader(handle, ptrHeaders[i], 
                        Marshal.SizeOf(typeof(MidiHeader))));

                    // Add header to buffer.
                    ThrowOnError(midiInAddBuffer(handle, ptrHeaders[i], 
                        Marshal.SizeOf(typeof(MidiHeader))));                  
                }

                // Indicate that the device is recording.
                recording = true;

                // Clear system exclusive header queue.
                sysExHeaderQueue.Clear();

                // Create thread for processing system exclusive headers.
                sysExHeaderThread = 
                    new Thread(new ThreadStart(ManageSysExHeaders));

                // Start worker thread.
                sysExHeaderThread.Start();

                // Start recording.
                ThrowOnError(midiInStart(handle));
            }
        }

        /// <summary>
        /// Stop recording Midi messages.
        /// </summary>
        public void Stop()
        {
            // If the device is open.
            if(IsOpen())
            {
                // If the device is recording.
                if(IsRecording())
                {
                    // Indicate that the device is not recording.
                    recording = false;

                    lock(lockObject)
                    {
                        Monitor.Pulse(lockObject);
                    }

                    // Wait for worker thread to finish.
                    sysExHeaderThread.Join();

                    // Stop recording.
                    ThrowOnError(midiInReset(handle));  

                    // Unprepare headers.
                    UnprepareHeaders();
                }
            }
        }

        /// <summary>
        /// Indicates whether or not the device is recording.
        /// </summary>
        /// <returns>
        /// true if the device is recording; otherwise, false.
        /// </returns>
        public bool IsRecording()
        {
            return recording;
        }

        #endregion

        #endregion        
	}

    /// <summary>
    /// The exception that is thrown when a error occurs with the InputDevice
    /// class.
    /// </summary>
    public class InputDeviceException : ApplicationException
    {
        #region InputDeviceException Members

        #region Win32 Midi Input Error Function

        [DllImport("winmm.dll")]
        private static extern int midiInGetErrorText(int errCode, 
            StringBuilder errMsg, int sizeOfErrMsg);

        #endregion

        #region Fields

        // Error message.
        private StringBuilder errMsg = new StringBuilder(128);

        #endregion 

        #region Construction

        /// <summary>
        /// Initializes a new instance of the InputDeviceException class with
        /// the specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        public InputDeviceException(int errCode)
        {
            // Get error message.
            midiInGetErrorText(errCode, errMsg, errMsg.Capacity);
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
                return errMsg.ToString();
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Provides data for the InvalidShortMsgEvent event.
    /// </summary>
    public class InvalidShortMsgEventArgs : EventArgs
    {
        #region InvalidShortMsgEventArgs Members

        #region Fields

        private int message;
        private int timeStamp;   
     
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the InvalidShortMsgEventArgs class 
        /// with the specified message and time stamp.
        /// </summary>
        /// <param name="message">
        /// The invalid short message as an integer. 
        /// </param>
        /// <param name="timeStamp">
        /// Time in milliseconds since the input device began recording.
        /// </param>
        public InvalidShortMsgEventArgs(int message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Invalid short message as an integer.
        /// </summary>
        public int Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Time in milliseconds since the input device began recording.
        /// </summary>
        public int TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        #endregion

        #endregion
    }
}
