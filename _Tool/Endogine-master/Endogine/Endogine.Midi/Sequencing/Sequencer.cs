///*
// * Created by: Leslie Sanford
// * 
// * Contact: jabberdabber@hotmail.com
// * 
// * Last modified: 09/27/2004
// */
//
//using System;
//using System.ComponentModel;
//using System.Collections;
//using System.Diagnostics;
//
//namespace Endogine.Midi
//{
//	/// <summary>
//	/// Plays back sequences.
//	/// </summary>
//	public sealed class Sequencer : System.ComponentModel.Component //SequencerBase
//	{
//        #region Sequencer Members
//
//        #region Fields
//
//        // The MIDI clock for controlling playback timing.
//        private SlaveClock clock;
//
//        // The input device used for receiving MIDI messages.
//        private InputDevice inDevice;
//
//        // The output device used for sending MIDI messages.
//        private OutputDevice outDevice;
//
//        #endregion
//
//        #region Construction
//
//        /// <summary>
//        /// Initializes an instance of the Sequencer class with the specified 
//        /// component container.
//        /// </summary>
//        /// <param name="container">
//        /// The component container.
//        /// </param>
//		public Sequencer(System.ComponentModel.IContainer container)
//		{
//			//
//			// Required for Windows.Forms Class Composition Designer support
//			//
//			container.Add(this);
//			InitializeComponent();
//		}
//
//        /// <summary>
//        /// Initializes an instance of the Sequencer class.
//        /// </summary>
//		public Sequencer()
//		{
//			//
//			// Required for Windows.Forms Class Composition Designer support
//			//
//			InitializeComponent();
//		}
//
//        #endregion
//
//        #region Methods
//
//		/// <summary> 
//		/// Clean up any resources being used.
//		/// </summary>
//		protected override void Dispose( bool disposing )
//		{
//			if( disposing )
//			{
//                inDevice.Close();
//                outDevice.Close();
////                clock.Dispose();
//
//				if(components != null)
//				{
//					components.Dispose();
//				}
//			}
//			base.Dispose( disposing );
//		}
//
//		#region Component Designer generated code
//		/// <summary>
//		/// Required method for Designer support - do not modify
//		/// the contents of this method with the code editor.
//		/// </summary>
//		private void InitializeComponent()
//		{
//            this.components = new System.ComponentModel.Container();
//            this.inDevice = new Endogine.Midi.InputDevice(this.components);
//            this.outDevice = new Endogine.Midi.OutputDevice(this.components);
//            // 
//            // outDevice
//            // 
//            this.outDevice.RunningStatusEnabled = true;
//        }
//		#endregion
//
//        /// <summary>
//        /// Initializes the sequencer.
//        /// </summary>
//        protected void InitializeSequencer()
//        {
////			clock = new SlaveClock(inDevice, outDevice, tickGen);
////            clock.Starting += new EventHandler(StartingHandler);
////            clock.Continuing += new EventHandler(ContinuingHandler);
////            clock.Stopping += new EventHandler(StoppingHandler);
////            clock.PositionChanged += new PositionChangedEventHandler(PositionChangedHandler);
//            if(InputDevice.DeviceCount > 0)
//                InputDeviceID = 0;
//
//            if(OutputDevice.DeviceCount > 0)
//                OutputDeviceID = 0;               
//        }
//
//		
//        /// <summary>
//        /// Handles the starting event generated by the MIDI clock.
//        /// </summary>
//        /// <param name="sender">
//        /// The MIDI clock responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        protected void StartingHandler(object sender, EventArgs e)
//        {
//            outDevice.Reset();
//			//base.StartingHandler(sender, e);
//        }
//
//        #endregion
//
//        #region Properties
//
//        /// <summary>
//        /// Gets or sets the input device's id.
//        /// </summary>
//        public int InputDeviceID
//        {
//            get
//            {
//                return inDevice.DeviceID;
//            }
//            set
//            {
//                Stop();
//
//                inDevice.Close();
//                inDevice.Open(value);
//            }
//        }
//
//        /// <summary>
//        /// Gets or sets the output device's id.
//        /// </summary>
//        public int OutputDeviceID
//        {
//            get
//            {
//                return outDevice.DeviceID;
//            }
//            set
//            {
//                Stop();
//
//                outDevice.Close();
//                outDevice.Open(value);
//            }            
//        }       
//        #endregion        
//
//        #endregion
//    }
//}
