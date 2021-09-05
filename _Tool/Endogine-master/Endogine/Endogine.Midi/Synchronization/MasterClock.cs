/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/29/2004
 */

using System;
using System.Threading;

namespace Endogine.Midi
{
	/// <summary>
	/// Provides functionality for controlling playback.
	/// </summary>
	public class MasterClock : MidiClock
	{
        #region Fields

        // Indicates whether or not the master mode is enabled.
        private bool masterEnabled = false;

        /// <summary>
        /// The MIDI sender for sending system realtime messages.
        /// </summary>
        protected IMidiSender midiSender;

        // Keeps track of the number of ticks that have occurred - used for 
        // determining when to send clock messages.
        private int tickCounter;

        // Number of ticks per MIDI clock.
        private int ticksPerClock;

        #region System Realtime Messages

        private SysRealtimeMessage startMessage = 
            new SysRealtimeMessage(SysRealtimeType.Start);
        private SysRealtimeMessage stopMessage = 
            new SysRealtimeMessage(SysRealtimeType.Stop);
        private SysRealtimeMessage continueMessage = 
            new SysRealtimeMessage(SysRealtimeType.Continue);
        private SysRealtimeMessage clockMessage = 
            new SysRealtimeMessage(SysRealtimeType.Clock);

        #endregion        

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MasterClock class with the 
        /// specified MIDI sender and tick generator.
        /// </summary>
        /// <param name="midiSender">
        /// The MIDI sender used for sending system realtime messages.
        /// </param>
        /// <param name="tickGenerator">
        /// The tick generator the MIDI clock will control.
        /// </param>
		public MasterClock(IMidiSender midiSender, 
            TickGenerator tickGenerator) : base(tickGenerator)
		{
            this.midiSender = midiSender;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Starts the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the master mode is enabled, the MIDI clock will send a start
        /// system realtime message to its slaves.
        /// </remarks>
        public override void Start()
        {
            // If the master mode is not enabled, use base class functionality.
            if(!MasterEnabled)
            {
                base.Start();
                return;
            }
             
            // Guard.
            if(IsRunning())
                return;

            // It is possible that the tick resolution has changed, recalculate
            // the number of ticks per clock.
            ticksPerClock = tickGenerator.Ppqn / TickGenerator.PpqnMin;

            // Register tick handler.
            tickGenerator.Tick += new EventHandler(TickHandler);            

            // Raise starting event.
            OnStarting();

            // Send start message.
            midiSender.Send(startMessage);

            // Pause to let slave device prepare for playback.
            Thread.Sleep(1);

            tickGenerator.Start();
        }

        /// <summary>
        /// Continues the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the master mode is enabled, the MIDI clock will send a continue
        /// system realtime message to its slaves.
        /// </remarks>
        public override void Continue()
        {
            // If the master mode is not enabled, use base class functionality.
            if(!MasterEnabled)
            {
                base.Continue();
                return;
            }

            // Guard.
            if(IsRunning())
                return;

            // It is possible that the tick resolution has changed, recalculate
            // the number of ticks per clock.
            ticksPerClock = tickGenerator.Ppqn / TickGenerator.PpqnMin;

            // Register tick handler.
            tickGenerator.Tick += new EventHandler(TickHandler);

            // Raise continuing event.
            OnContinuing();

            // Send continue message.
            midiSender.Send(continueMessage);            

            // Pause to let slave device prepare for playback.
            Thread.Sleep(1);

            tickGenerator.Start();
        }

        /// <summary>
        /// Stops the MIDI clock.
        /// </summary>
        /// <remarks>
        /// If the master mode is enabled, the MIDI clock will send a stop
        /// system realtime message to its slaves.
        /// </remarks>
        public override void Stop()
        {
            // If the master mode is not enabled, use base class functionality.
            if(!MasterEnabled)
            {
                base.Stop();
                return;
            }

            // Guard.
            if(!IsRunning())
                return;

            tickGenerator.Stop();

            // Remove tick handler from tick generator.
            tickGenerator.Tick -= new EventHandler(TickHandler);            

            // Send stop message.
            midiSender.Send(stopMessage); 

            // Raise stopping event.
            OnStopping();
        }

        /// <summary>
        /// Sends song position pointer to its slaves.
        /// </summary>
        /// <param name="spp">
        /// The sont position pointer.
        /// </param>
        /// <remarks>
        /// If the master mode is not enabled, this method has no effect.
        /// </remarks>
        public void SendSongPositionPointer(SongPositionPointer spp)
        {
            // Guard.
            if(!MasterEnabled)
                return;

            // Keep track of whether or not the clock is running.
            bool wasRunning = IsRunning();

            // If the clock is running, stop it momentarily. It is better not
            // to change the position while a sequencer is running.
            if(wasRunning)
                Stop();

            // Send song position pointer.
            midiSender.Send(spp.ToMessage());

            // If the clock was running, resume running.
            if(wasRunning)
                Continue();
        }

        /// <summary>
        /// Disposes of the MIDI clock.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether or not to dispose the MIDI clock.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(IsRunning())
                {
                    tickGenerator.Stop();
                    tickGenerator.Tick -= new EventHandler(TickHandler);
                }

                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Handles tick events.
        /// </summary>
        /// <param name="sender">
        /// The tick generator responsible for the event.
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
        private void TickHandler(object sender, EventArgs e)
        {
            // Keep track of the number of ticks that have occurred since the
            // last clock message.
            tickCounter++;

            // If it is time to send a clock message.
            if(tickCounter == ticksPerClock)
            {
                // Send clock message.
                midiSender.Send(clockMessage);

                // Reset counter.
                tickCounter = 0;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not the master mode is
        /// enabled.
        /// </summary>
        /// <remarks>
        /// When the master mode is enabled, system realtime messages are sent
        /// to slave devices for controlling playback.
        /// </remarks>
        public bool MasterEnabled
        {
            get
            {
                return masterEnabled;
            }
            set
            {
                if(masterEnabled == value)
                    return;

                Stop();

                masterEnabled = value;
            }
        }

        #endregion       
	}
}
