/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/04/2004
 */

using System;

using System.Windows.Forms;

namespace Endogine.Midi
{
    /// <summary>
    /// Represents methods for handling position changed events.
    /// </summary>
    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);

	/// <summary>
	/// Provides functionality for controlling playback timing with slave 
	/// capabilities.
	/// </summary>
	public class SlaveClock : MasterClock
	{
        #region SlaveClock Members

        #region Delegates

        /// <summary>
        /// Represents state handler methods.
        /// </summary>
        private delegate void SlaveModeCallback(SysRealtimeEventArgs e);

        #endregion

        #region Constants

        // A value used to calculate microseconds per beat.
        private const int TempoScale = 24000;

        #endregion

        #region Fields

        // Indicates whether or not the slave mode is enabled.
        private bool slaveEnabled = false;

        // For receiving MIDI messages.
        private IMidiReceiver midiReceiver;

        // Represents the current state handler.
        private SlaveModeCallback state;

        // The previous time stamp of the last clock message.
        private int prevTimeStamp;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a song position pointer message is received from a 
        /// master device.
        /// </summary>
        public event PositionChangedEventHandler PositionChanged;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes an instance of the SlaveClock class with the specified
        /// MIDI receiver, MIDI sender, and tick generator.
        /// </summary>
        /// <param name="midiReceiver">
        /// The MIDI receiver used for receiving MIDI messages from a master 
        /// device.
        /// </param>
        /// <param name="midiSender">
        /// The MIDI sender used for sending MIDI messages to slave devices.
        /// </param>
        /// <param name="tickGenerator">
        /// The tick generator the MIDI clock will control.
        /// </param>
		public SlaveClock(IMidiReceiver midiReceiver, IMidiSender midiSender, 
            TickGenerator tickGenerator) : base(midiSender, tickGenerator)
		{
            this.midiReceiver = midiReceiver;            
		}

        #endregion

        #region Methods

        /// <summary>
        /// Starts the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the slave mode has been enabled, this method has no effect.
        /// </remarks>
        public override void Start()
        {
            // If the slave mode is not enabled, use base class functionality.
            if(!SlaveEnabled)
            {
                base.Start();
                return;
            }
        }

        /// <summary>
        /// Continues the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the slave mode has been enabled, this method has no effect.
        /// </remarks>
        public override void Continue()
        {
            // If the slave mode is not enabled, use base class functionality.
            if(!SlaveEnabled)
            {
                base.Continue();
                return;
            }
        }

        /// <summary>
        /// Stops the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the slave mode has been enabled, this method has no effect.
        /// </remarks>
        public override void Stop()
        {
            // If the slave mode is not enabled, use base class functionality.
            if(!SlaveEnabled)
            {
                base.Stop();
                return;
            }
        }

        /// <summary>
        /// Disposes of the MIDI clock.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not to dispose of the MIDI clock.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                SlaveEnabled = false;
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Handles system realtime received events.
        /// </summary>
        /// <param name="sender">
        /// The MIDI receiver responsible for the event.
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void SysRealtimeReceivedHandler(object sender, SysRealtimeEventArgs e)
        {
            // Pass on the event to the current state.
            state(e);
        }

        /// <summary>
        /// Handles system common received events.
        /// </summary>
        /// <param name="sender">
        /// The MIDI receiver responsible for the event.
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void SysCommonReceivedHandler(object sender, SysCommonEventArgs e)
        {
            // Guard.
            if(!SlaveEnabled)
                return;            

            // If the position has changed.
            if(e.Message.Type == SysCommonType.SongPositionPointer &&
                PositionChanged != null)
            {
                SongPositionPointer spp = 
                    new SongPositionPointer(tickGenerator.Ppqn, e.Message);

                bool wasRunning = IsRunning();

                // If the tick Generator is running, stop it momentarily to 
                // give listeners a chance to update their position when the 
                // PositionChanged event is raised.
                //
                // Ideally, any master device sending a song position pointer 
                // messages would send a stop message first so that the slave 
                // is not running when the position is changed. 
                if(wasRunning)
                    tickGenerator.Stop();

                PositionChanged(this, 
                    new PositionChangedEventArgs(spp.PositionInTicks));

                if(MasterEnabled)
                    midiSender.Send(e.Message);

                // Restart tick generator if it was previously running.
                if(wasRunning)
                    tickGenerator.Start();
            }
        }

        #region State Handlers

        /// <summary>
        /// Initial state.
        /// </summary>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void Initial(SysRealtimeEventArgs e)
        {
            // If this is a start event.
            if(e.Message.Type == SysRealtimeType.Start)
            {
                if(MasterEnabled)
                    midiSender.Send(e.Message);                

                // Transition to the waiting state.
                state = new SlaveModeCallback(Waiting);

                // Raise starting event.
                OnStarting();
            }
            // Else if this is a continue event.
            else if(e.Message.Type == SysRealtimeType.Continue)
            { 
                // Transition to the waiting state.
                state = new SlaveModeCallback(Waiting);

                // Raise contining event.
                OnContinuing();
            }
        }

        /// <summary>
        /// Waiting state.
        /// </summary>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void Waiting(SysRealtimeEventArgs e)
        {
            // If this is a clock event.
            if(e.Message.Type == SysRealtimeType.Clock)
            {
                // Keep track of time stamp.
                prevTimeStamp = e.TimeStamp;

                if(MasterEnabled)
                    midiSender.Send(e.Message);

                // The first clock message has been received, start the tick
                // generator.
                tickGenerator.Start();

                // Transition to the running state.
                state = new SlaveModeCallback(Running);
            }
            // Else if this is a stop event.
            else if(e.Message.Type == SysRealtimeType.Stop)
            {
                if(MasterEnabled)
                    midiSender.Send(e.Message);

                // Transition to the initial state.
                state = new SlaveModeCallback(Initial);

                // Raise the stopping event.
                OnStopping();
            }
        }

        /// <summary>
        /// Running state.
        /// </summary>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void Running(SysRealtimeEventArgs e)
        {
            // If this is a clock event.
            if(e.Message.Type == SysRealtimeType.Clock)
            {
                // Calculate tempo based on the time that has elapsed since the 
                // last clock message.
                //
                // To calculate the tempo based on clock messages, determine 
                // the time in milliseconds that have elapsed since the last 
                // clock message. Since there are 24 clock messages per beat,
                // multiply the elapsed time by 24 to get the milliseconds per
                // beat value. And since the tempo is measured in microseconds
                // per beat, multiply this value by 1000. The TempoScale 
                // constant takes care of combining the number of clock 
                // messages per beat with the microsecond scale.
                int tempo = (e.TimeStamp - prevTimeStamp) * TempoScale;

                // If the tempo has changed, change the tick generator's tempo.
                if(tempo != tickGenerator.Tempo && 
                    tempo >= TickGenerator.TempoMin &&
                    tempo <= TickGenerator.TempoMax)
                {
                    tickGenerator.Tempo = tempo;
                }

                if(MasterEnabled)
                    midiSender.Send(e.Message);

                // Keep track of timestamp.
                prevTimeStamp = e.TimeStamp;
            }
            // Else if this is a stop message.
            else if(e.Message.Type == SysRealtimeType.Stop)
            {                
                tickGenerator.Stop();

                if(MasterEnabled)
                    midiSender.Send(e.Message);                

                // Transition to the initial state.
                state = new SlaveModeCallback(Initial);

                // Raise stopping event.
                OnStopping();
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not the slave mode is
        /// enabled.
        /// </summary>
        /// <remarks>
        /// Enabling the slave mode causes the MIDI clock to begin waiting for 
        /// MIDI messages for synchronizing playback.
        /// </remarks>
        public bool SlaveEnabled
        {
            get
            {
                return slaveEnabled;
            }
            set
            {
                if(slaveEnabled == value)
                    return;                

                // If the slave mode has been enabled.
                if(value)
                {
                    Stop();                    

                    // Initialize state handler.
                    state = new SlaveModeCallback(Initial);

                    // Connect to MIDI receiver and begin receiving MIDI 
                    // messages.
                    midiReceiver.SysRealtimeReceived += 
                        new SysRealtimeEventHandler(SysRealtimeReceivedHandler);
                    midiReceiver.SysCommonReceived += 
                        new SysCommonEventHandler(SysCommonReceivedHandler);
                    midiReceiver.Start();
                }
                // Else the slave mode has not been enabled.
                else
                {
                    if(IsRunning())
                    {
                        tickGenerator.Stop();
                        OnStopping();
                    }

                    // Stop MIDI receiver and disconnect from it.
                    midiReceiver.Stop();
                    midiReceiver.SysRealtimeReceived -= 
                        new SysRealtimeEventHandler(SysRealtimeReceivedHandler);
                    midiReceiver.SysCommonReceived -= 
                        new SysCommonEventHandler(SysCommonReceivedHandler);
                }

                slaveEnabled = value;
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Provides data for the PositionChanged event.
    /// </summary>
    public class PositionChangedEventArgs : EventArgs
    {
        #region PositionChangedEventArgs Members

        #region Fields 

        // The position in ticks.
        private int position;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the PositionChangeEventArgs class 
        /// with the specified position.
        /// </summary>
        /// <param name="position">
        /// The position in ticks.
        /// </param>
        public PositionChangedEventArgs(int position)
        {
            this.position = position;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the position in ticks.
        /// </summary>
        public int Position
        {
            get
            {
                return position;
            }
        }

        #endregion

        #endregion
    }
}
