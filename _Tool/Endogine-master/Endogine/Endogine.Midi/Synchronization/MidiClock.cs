/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/28/2004
 */

using System;

namespace Endogine.Midi
{
	/// <summary>
	/// Provides functionality for controlling playback.
	/// </summary>
	public class MidiClock : IDisposable
	{
        #region MidiClock Members

        #region Fields

        /// <summary>
        /// The tick generator that is controlled by the MIDI clock.
        /// </summary>
        protected TickGenerator tickGenerator;        

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the MIDI clock is in the process of starting.
        /// </summary>
        public event EventHandler Starting;

        /// <summary>
        /// Occurs when the MIDI clock is in the process of continuing.
        /// </summary>
        public event EventHandler Continuing;

        /// <summary>
        /// Occurs when the MIDI clock is in the process of stopping.
        /// </summary>
        public event EventHandler Stopping;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiClock class with the 
        /// specified tick generator.
        /// </summary>
        /// <param name="tickGenerator">
        /// The tick generator the MIDI clock will control.
        /// </param>
		public MidiClock(TickGenerator tickGenerator)
		{
            this.tickGenerator = tickGenerator;
		}

        #endregion 

        #region Methods

        /// <summary>
        /// Starts the MIDI clock.
        /// </summary>
        public virtual void Start()
        {
            // Guard.
            if(IsRunning())
                return;

            OnStarting();
            tickGenerator.Start();            
        }

        /// <summary>
        /// Continues the MIDI clock.
        /// </summary>
        public virtual void Continue()
        {
            // Guard.
            if(IsRunning())
                return;

            OnContinuing();
            tickGenerator.Start();
        }

        /// <summary>
        /// Stops the MIDI clock.
        /// </summary>
        public virtual void Stop()
        {
            // Guard.
            if(!IsRunning())
                return;

            OnStopping();
            tickGenerator.Stop();
        }

        /// <summary>
        /// Returns a value indicating whether or not the MIDI clock is 
        /// running.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the MIDI clock is running.
        /// </returns>
        public bool IsRunning()
        {
            return tickGenerator.IsRunning();
        }

        /// <summary>
        /// Raises the started event.
        /// </summary>
        protected virtual void OnStarting()
        {
            if(Starting != null)
                Starting(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the continued event.
        /// </summary>
        protected virtual void OnContinuing()
        {
            if(Continuing != null)
                Continuing(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the stopped event.
        /// </summary>
        protected virtual void OnStopping()
        {
            if(Stopping != null)
                Stopping(this, EventArgs.Empty);
        }

        /// <summary>
        /// Disposes of the MIDI clock.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether or not to dispose of the MIDI clock.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Do nothing.
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the MIDI clock.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
