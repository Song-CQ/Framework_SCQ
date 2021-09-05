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
//	public sealed class Sequencer : System.ComponentModel.Component
//	{
//        #region Sequencer Members
//
//        #region Fields
//
//        // The sequence position.
//        private int position = 0;
//
//        // Indicates whether or not the sequencer should seek the position 
//        // before beginning playback.
//        private bool seekPosition = false;
//
//        // The sequence to playback.
//        private Sequence seq;
//
//        // The version of the sequence.
//        private int sequenceVersion;
//
//        // The sequence player for controlling sequence playback.
//        private SequencePlayer player;
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
//        // The tick generator used for timing the playback of MIDI messages.
//        private TickGenerator tickGen;
//
//        // The component container.
//        private IContainer components;
//
//        #endregion
//
//        #region Events
//
//        /// <summary>
//        /// Occurs when the tempo changes.
//        /// </summary>
//        public event EventHandler TempoChanged;
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
//
//			InitializeSequencer();
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
//
//			InitializeSequencer();
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
//                clock.Dispose();
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
//            this.tickGen = new Endogine.Midi.TickGenerator(this.components);
//            // 
//            // outDevice
//            // 
//            this.outDevice.RunningStatusEnabled = true;
//            // 
//            // tickGen
//            // 
//            this.tickGen.Ppqn = 96;
//            this.tickGen.Tempo = 500000;
//            this.tickGen.Tick += new System.EventHandler(this.TickHandler);
//
//        }
//		#endregion
//
//        /// <summary>
//        /// Initializes the sequencer.
//        /// </summary>
//        private void InitializeSequencer()
//        {
//            seq = new Sequence();
//            sequenceVersion = seq.Version;
//            player = new SequencePlayer(outDevice, tickGen, seq);
//            player.EndOfSequenceReached += 
//                new EventHandler(EndOfSequenceReachedHandler);
//            clock = new SlaveClock(inDevice, outDevice, tickGen);
//
//            clock.Starting += new EventHandler(StartingHandler);
//            clock.Continuing += new EventHandler(ContinuingHandler);
//            clock.Stopping += new EventHandler(StoppingHandler);
//            clock.PositionChanged += new PositionChangedEventHandler(PositionChangedHandler);
//
//            tickGen.TempoChanged += new EventHandler(OnTempoChanged);
//
//            if(InputDevice.DeviceCount > 0)
//                InputDeviceID = 0;
//
//            if(OutputDevice.DeviceCount > 0)
//                OutputDeviceID = 0;               
//        }
//
//        /// <summary>
//        /// Starts playback at the beginning of the sequence.
//        /// </summary>
//        public void Start()
//        {
//            clock.Start();
//        }
//
//        /// <summary>
//        /// Continues playback from the current position.
//        /// </summary>
//        public void Continue()
//        {
//            // Guard.
//            if(Position >= Sequence.Length)
//                return;
//
//            clock.Continue();
//        }
//
//        /// <summary>
//        /// Stops playback.
//        /// </summary>
//        public void Stop()
//        {
//            clock.Stop();
//        }
//
//        /// <summary>
//        /// Attaches a delegate to the internal tick generator the sequencer
//        /// uses for timing playback.
//        /// </summary>
//        /// <param name="handler">
//        /// The delegate to attach to the tick generator.
//        /// </param>
//        /// <remarks>
//        /// Attaching a delegate to a sequencer's tick generator allows a 
//        /// client to synchronize itself with the sequencer's playback.
//        /// </remarks>
//        public void AttachToTickGenerator(EventHandler handler)
//        {
//            tickGen.Tick += handler;
//        }
//
//        /// <summary>
//        /// Detaches a delegate from the internal tick generator the sequencer
//        /// uses for timing playback.
//        /// </summary>
//        /// <param name="handler">
//        /// The delegate to detach from the tick generator.
//        /// </param>
//        public void DetachFromTickGenerator(EventHandler handler)
//        {
//            tickGen.Tick -= handler;
//        }
//
//        /// <summary>
//        /// Sets the mute state of a track.
//        /// </summary>
//        /// <param name="index">
//        /// The index into the sequence of the track to mute.
//        /// </param>
//        /// <param name="mute">
//        /// A value indicating whether or not to mute the track.
//        /// </param>
//        public void MuteTrack(int index, bool mute)
//        {
//            player.MuteTrack(index, mute);
//        }
//
//        /// <summary>
//        /// Sets the solo state of a track.
//        /// </summary>
//        /// <param name="index">
//        /// The index into the sequence of the track to solo.
//        /// </param>
//        /// <param name="solo">
//        /// A value indicating whether or not to solo the track.
//        /// </param>
//        public void SoloTrack(int index, bool solo)
//        {
//            player.SoloTrack(index, solo);
//        }
//
//        /// <summary>
//        /// Handles tick events generated by the tick generator.
//        /// </summary>
//        /// <param name="sender">
//        /// The tick generator responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void TickHandler(object sender, EventArgs e)
//        {
//            position++;
//        }
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
//        private void StartingHandler(object sender, EventArgs e)
//        {
//            // Lock the sequence so that it cannot be modified during playback.
//            Sequence.LockSequence(this, true);
//
//            outDevice.Reset();
//
//            if(sequenceVersion != Sequence.Version)
//            {
//                PreparePlayer();
//                sequenceVersion = Sequence.Version;
//            }
//
//            Position = 0;
//            player.Seek(0);
//        }
//
//        /// <summary>
//        /// Handles the continuing event generated by the MIDI clock.
//        /// </summary>
//        /// <param name="sender">
//        /// The MIDI clock responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void ContinuingHandler(object sender, EventArgs e)
//        {
//            // Lock the sequence so that it cannot be modified during playback.
//            Sequence.LockSequence(this, true);
//
//            if(sequenceVersion != Sequence.Version)
//            {
//                PreparePlayer();
//                sequenceVersion = Sequence.Version;
//                seekPosition = true;
//            }
//
//            if(seekPosition)
//            {
//                player.Seek(position);
//                seekPosition = false;
//            }
//        }
//
//        /// <summary>
//        /// Handles the stopping event generated by the MIDI clock.
//        /// </summary>
//        /// <param name="sender">
//        /// The MIDI clock responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void StoppingHandler(object sender, EventArgs e)
//        {
//            // Unlock the sequence so that it can not be modified.
//            Sequence.LockSequence(this, false);
//
//            player.AllSoundsOff();
//            seekPosition = false;
//        }
//
//        /// <summary>
//        /// Handles the position changed event generated by the MIDI clock.
//        /// </summary>
//        /// <param name="sender">
//        /// The MIDI clock responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void PositionChangedHandler(object sender, PositionChangedEventArgs e)
//        {
//            position = e.Position;
//            player.Seek(position);
//        }
//
//        /// <summary>
//        /// Handles the end of sequence reached event generated by the sequence 
//        /// player.
//        /// </summary>
//        /// <param name="sender">
//        /// The sequence player responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void EndOfSequenceReachedHandler(object sender, EventArgs e)
//        {
//            clock.Stop();
//        }
//
//        /// <summary>
//        /// Handles and raises the tempo changed event.
//        /// </summary>
//        /// <param name="sender">
//        /// The tick generator responsible for the event.
//        /// </param>
//        /// <param name="e">
//        /// Information about the event.
//        /// </param>
//        private void OnTempoChanged(object sender, EventArgs e)
//        {
//            if(TempoChanged != null)
//                TempoChanged(this, EventArgs.Empty);
//        }        
//
//        /// <summary>
//        /// Prepares the sequence player for playback.
//        /// </summary>
//        private void PreparePlayer()
//        {
//            player.EndOfSequenceReached -= 
//                new EventHandler(EndOfSequenceReachedHandler);
//            player.Dispose();
//            player = new SequencePlayer(outDevice, tickGen, seq);
//            player.EndOfSequenceReached += 
//                new EventHandler(EndOfSequenceReachedHandler);
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
//
//        /// <summary>
//        /// Gets or sets a value indicating whether or not the master mode
//        /// is enabled.
//        /// </summary>
//        public bool MasterEnabled
//        {
//            get
//            {
//                return clock.MasterEnabled;
//            }
//            set
//            {
//                clock.MasterEnabled = value;
//            }
//        }
//
//        /// <summary>
//        /// Gets or sets a value indicating whether or not the slave mode is 
//        /// enabled.
//        /// </summary>
//        public bool SlaveEnabled
//        {
//            get
//            {
//                return clock.SlaveEnabled;
//            }
//            set
//            {
//                clock.SlaveEnabled = value;
//            }
//        }
//
//        /// <summary>
//        /// Gets or sets the playback position in ticks.
//        /// </summary>
//        /// <remarks>
//        /// Attempting to set the position while the slave mode is enabled has
//        /// no effect.
//        /// </remarks>
//        public int Position
//        {
//            get
//            {
//                return position;
//            }
//            set
//            {
//                // Attempting to set the position in slave mode has no effect.
//                if(SlaveEnabled)
//                    return;
//
//                if(value < 0)
//                    throw new ArgumentOutOfRangeException("Positon", value,
//                        "Position out of range.");
//
//                bool wasRunning = clock.IsRunning();
//                
//                if(wasRunning)
//                    clock.Stop();
//
//                position = value;
//                seekPosition = true;
//
//                if(wasRunning)
//                    clock.Continue();
//            }
//        }
//
//        /// <summary>
//        /// Gets or sets the song position pointer.
//        /// </summary>
//        /// <remarks>
//        /// 
//        /// </remarks>
//        public int SongPositionPointer
//        {
//            get
//            {
//                SongPositionPointer spp = 
//                    new SongPositionPointer(tickGen.Ppqn);
//
//                spp.PositionInTicks = Position;
//
//                return spp.SongPosition;
//            }
//            set
//            {
//                // Attempting to set the position in slave mode has no effect.
//                if(SlaveEnabled)
//                    return;
//
//                if(value < 0)
//                    throw new ArgumentOutOfRangeException("SongPositionPointer", 
//                        value, "Song position pointer out of range.");
//
//                SongPositionPointer spp = 
//                    new SongPositionPointer(tickGen.Ppqn);
//
//                spp.SongPosition = value;
//
//                bool wasRunning = clock.IsRunning();
//                
//                if(wasRunning)
//                    clock.Stop();
//
//                position = spp.PositionInTicks;
//                seekPosition = true;
//
//                clock.SendSongPositionPointer(spp);
//
//                if(wasRunning)
//                    clock.Continue();
//            }
//        }
//
//        /// <summary>
//        /// Gets or sets the sequence to play back.
//        /// </summary>
//        public Sequence Sequence
//        {
//            get
//            {
//                return seq;
//            }
//            set
//            {               
//                Stop();
//
//                seq = value;
//                tickGen.Ppqn = seq.Division;   
//                PreparePlayer();
//                sequenceVersion = seq.Version;
//                Position = 0;
//            }
//        }
//
//		//JB
//		/// <summary>
//		/// Gets the SequencePlayer
//		/// </summary>
//		public SequencePlayer Player
//		{
//			get {return this.player;}
//		}
//
//        /// <summary>
//        /// Gets or sets the tempo in microseconds.
//        /// </summary>
//        public int Tempo
//        {
//            get
//            {
//                return tickGen.Tempo;
//            }
//            set
//            {
//                tickGen.Tempo = value;
//            }
//        }
//
//        #endregion        
//
//        #endregion
//    }
//}
