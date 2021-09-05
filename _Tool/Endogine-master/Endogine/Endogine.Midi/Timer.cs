/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 05/07/2003
 */


using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;

namespace Multimedia
{
    /// <summary>
    /// Specifies constants for multimedia timer event types.
    /// </summary>
    public enum TimerMode 
    { 
        /// <summary>
        /// Timer event occurs once.
        /// </summary>
        OneShot, 
        
        /// <summary>
        /// Timer event occurs periodically.
        /// </summary>
        Periodic 
    };

    /// <summary>
    /// Represents the method that handles calls from a multimedia timer.
    /// </summary>
    /// <param name="state">
    /// An object containing application-specific information relevant to the 
    /// method invoked by this delegate, or a null reference.
    /// </param>
    /// <remarks>
    /// Use a TimerCallback delegate to specify the method that is called by a 
    /// multimedia timer.
    /// </remarks>
    public delegate void TimerCallback(object state);

    /// <summary>
    /// Represents information about the timer's capabilities.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TimerCaps
    {
        /// <summary>
        /// Minimum supported period.
        /// </summary>
        public int periodMin;

        /// <summary>
        /// Maximum supported period.
        /// </summary>
        public int periodMax;
    }

    /// <summary>
    /// Represents a multimedia timer.
    /// </summary>
    /// <remarks>
    /// <para>The multimedia timer allows applications to schedule timer events 
    /// with the greatest resolution (or accuracy) possible for the hardware 
    /// platform. The multimedia timer allows you to schedule timer events at a 
    /// higher resolution than other timer services.</para>
    /// 
    /// <para>The multimedia timer is useful for applications that demand 
    /// high-resolution timing. For example, a Midi sequencer requires a 
    /// high-resolution timer because it must maintain the pace of Midi events 
    /// within a resolution of 1 millisecond.</para>
    /// </remarks>
    public sealed class Timer : IDisposable
    {
        #region Timer Members

        #region Delegates

        // Represents the method that is called by Windows when a timer event
        // occurs.
        private delegate void TimeProc(int id, int msg, int user, int param1, 
            int param2);

        #endregion

        #region Win32 Multimedia Timer Functions

        // Gets timer capabilities.
        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, 
            int sizeOfTimerCaps);

        // Creates and starts the timer.
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, 
            TimeProc proc, int user, int mode);

        // Stops and destroys the timer.
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);      
        
        #endregion

        #region Fields

        // Timer identifier.
        private int timerId;

        // Timer mode.
        private TimerMode mode;

        // Period between timer events in milliseconds.
        private int period;

        // Timer resolution in milliseconds.
        private int resolution;

        // Indicates whether or not the timer is running.
        private bool running;

        // Called by Windows when a timer periodic event occurs.
        private TimeProc timeProcPeriodic;

        // Called by Windows when a timer one shot event occurs.
        private TimeProc timeProcOneShot;

        // User supplied callback that is called when a timer event occurs.
        private TimerCallback callback;

        // User supplied state information.
        private object state; 

        // Multimedia timer capabilities.
        private static TimerCaps caps;

        // Resource manager - gets error messages for exceptions.
        private static ResourceManager resManager = new 
            ResourceManager("Multimedia.Resource", 
            Assembly.GetExecutingAssembly());

        #endregion

        #region Construction/Destruction

        /// <summary>
        /// Initialize class.
        /// </summary>
        static Timer()
        {
            // Get multimedia timer capabilities.
            timeGetDevCaps(ref caps, Marshal.SizeOf(caps));            
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with the user 
        /// supplied callback and state information, timer period, and 
        /// resolution.
        /// </summary>
        /// <param name="callback">
        /// A TimerCallback delegate representing a method to be executed when 
        /// a timer event occurs. 
        /// </param>
        /// <param name="state">
        /// An object containing information to be used by the callback method, 
        /// or a null reference.
        /// </param>
        /// <param name="period">
        /// The time between timer events in milliseconds.
        /// </param>
        /// <param name="resolution">
        /// The timer resolution in milliseconds.
        /// </param>
        public Timer(TimerCallback callback, object state, int period, 
            int resolution)
        {
            // 
            // Initialize fields.
            //

            this.callback = callback;
            this.state = state;
            timeProcPeriodic = new TimeProc(OnTimerPeriodicEvent);
            timeProcOneShot = new TimeProc(OnTimerOneShotEvent);
            Mode = TimerMode.Periodic;
            Period = period;
            Resolution = resolution;
            running = false;
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with the user 
        /// supplied callback and state information, timer period, resolution,
        /// and timer mode.
        /// </summary>
        /// <param name="callback">
        /// A TimerCallback delegate representing a method to be executed when 
        /// a timer event occurs. 
        /// </param>
        /// <param name="state">
        /// An object containing information to be used by the callback method, 
        /// or a null reference.
        /// </param>
        /// <param name="period">
        /// The time between timer events in milliseconds.
        /// </param>
        /// <param name="resolution">
        /// The timer resolution in milliseconds.
        /// </param>
        /// <param name="mode">
        /// The timer mode.
        /// </param>
        public Timer(TimerCallback callback, object state, int period, 
            int resolution, TimerMode mode)
        {
            // 
            // Initialize fields.
            //

            this.callback = callback;
            this.state = state;
            timeProcPeriodic = new TimeProc(OnTimerPeriodicEvent);
            timeProcOneShot = new TimeProc(OnTimerOneShotEvent);
            Mode = mode;
            Period = period;
            Resolution = resolution;
            running = false;            
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~Timer()
        {
            if(running)
            {
                Stop();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <exception cref="TimerStartException">
        /// Thrown if unable to start timer.
        /// </exception>
        public void Start()
        {
            // If the timer is already running.
            if(running)
            {
                // Stop timer.
                Stop();
            }

            // If the periodic event callback should be used.
            if(mode == TimerMode.Periodic)
            {
                // Create and start timer.
                timerId = timeSetEvent(Period, Resolution, timeProcPeriodic, 0, 
                    (int)Mode);
            }
            // Else the one shot event callback should be used.
            else
            {
                // Create and start timer.
                timerId = timeSetEvent(Period, Resolution, timeProcOneShot, 0, 
                    (int)Mode);
            }

            // If the timer was created successfully.
            if(timerId != 0)
            {
                // Indicate that the timer is now running.
                running = true;
            }
            // Else an error occurred. 
            else
            {
                // Get error message and throw exception.
                string msg = resManager.GetString("TimerStartFailed");
                throw new TimerStartException(msg);
            }
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        public void Stop()
        {
            // If the timer is running.
            if(running)
            {
                // Stop and destroy timer.
                timeKillEvent(timerId);

                // Indicate that the timer is not running.
                running = false;
            }
        }

        /// <summary>
        /// Indicates whether or not the timer is running.
        /// </summary>
        /// <returns>
        /// Returns true if the timer is running; otherwise, false.
        /// </returns>
        public bool IsRunning()
        {
            return running;
        }

        /// <summary>
        /// Callback method called by the Win32 multimedia timer when a timer
        /// periodic event occurs.
        /// </summary>
        private void OnTimerPeriodicEvent(int id, int msg, int user, 
            int param1, int param2)
        {
            // Call user supplied callback.
            callback(state);
        }

        /// <summary>
        /// Callback method called by the Win32 multimedia timer when a timer
        /// one shot event occurs.
        /// </summary>
        private void OnTimerOneShotEvent(int id, int msg, int user, int param1, 
            int param2)
        {
            // Call user supplied callback.
            callback(state);

            // Stop timer.
            Stop();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the timer mode.
        /// </summary>
        /// <remarks>
        /// If the timer is running when the mode is changed, the timer is 
        /// stopped and restarted with the new mode value.
        /// </remarks>
        public TimerMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;

                // If the timer is running.
                if(IsRunning())
                {
                    // Stop and restart timer.
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets the timer period.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the period is set to a value out of range for the 
        /// multimedia timer.
        /// </exception>
        /// <remarks>
        /// <para>The multimedia timer period is the time between timer events 
        /// in milliseconds.</para>
        /// 
        /// <para>If the timer is running when the period is changed, the timer 
        /// is stopped and restarted with the new period value.</para>   
        /// </remarks>     
        public int Period
        {
            get
            {
                return period;
            }
            set
            {
                // If period is in range.
                if(value >= caps.periodMin && value <= caps.periodMax)
                {
                    // Assign period.
                    period = value;

                    // If the timer is running.
                    if(IsRunning())
                    {
                        // Stop and restart timer.
                        Stop();
                        Start();
                    }
                }
                // Else period is out of range.
                else
                {
                    // Stop timer before throwing exception (has no effect if
                    // the timer is not running).
                    Stop();

                    // Get error message and throw exception.
                    string msg = resManager.GetString("TimerPeriodOutOfRange");
                    throw new ArgumentOutOfRangeException("Period", value, msg);
                }
            }
        }

        /// <summary>
        /// Gets or sets the timer resolution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the resolution is set to a value less than zero.
        /// </exception>
        /// <remarks>
        /// <para>The resolution is in milliseconds. The resolution increases 
        /// with smaller values; a resolution of 0 indicates periodic events 
        /// should occur with the greatest possible accuracy. To reduce system 
        /// overhead, however, you should use the maximum value appropriate 
        /// for your application.</para>
        /// 
        /// <para>If the timer is running when the resolution is changed, the 
        /// timer is stopped and restarted with the new resolution value.
        /// </para>        
        /// </remarks>
        public int Resolution
        {
            get
            {
                return resolution;
            }
            set
            {
                // If resolution is in range.
                if(value >= 0)
                {
                    // Assign resolution.
                    resolution = value;

                    // If the timer is running.
                    if(IsRunning())
                    {
                        // Stop and restart timer.
                        Stop();
                        Start();
                    }
                }
                // Else resolution is out of range.
                else
                {
                    // Stop timer before throwing exception (has no effect if
                    // the timer is not running).
                    Stop();

                    // Get error message and throw exception.
                    string msg = resManager.GetString("TimerResolutionOutOfRange");
                    throw new ArgumentOutOfRangeException("Resolution", value, msg);
                }
            }
        }

        /// <summary>
        /// Gets the timer capabilities.
        /// </summary>
        public static TimerCaps Capabilities
        {
            get
            {
                return caps;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Frees timer resources.
        /// </summary>
        public void Dispose()
        {
            // If the timer is still running.
            if(running)
            {
                // Stop timer.
                Stop();
            }
        }

        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a timer fails to start.
    /// </summary>
    public class TimerStartException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the TimerStartException class.
        /// </summary>
        public TimerStartException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TimerStartException class.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        public TimerStartException(string message) : base(message)
        {
        }
    }
}
