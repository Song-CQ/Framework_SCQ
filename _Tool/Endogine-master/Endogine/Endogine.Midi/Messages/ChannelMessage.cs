/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/06/2004
 */

using System;

namespace Endogine.Midi
{
    /// <summary>
    /// Represents the method that will handle the event that occurs when a 
    /// channel message is received.
    /// </summary>
    public delegate void ChannelMessageEventHandler(object sender, ChannelMessageEventArgs e);

    #region Channel Command Types

    /// <summary>
    /// Specifies constants defining channel message types.
    /// </summary>
    public enum ChannelCommand 
    {
        /// <summary>
        /// Represents the note-off command type.
        /// </summary>
        NoteOff = 0x80,

        /// <summary>
        /// Represents the note-on command type.
        /// </summary>
        NoteOn = 0x90,

        /// <summary>
        /// Represents the poly pressure (aftertouch) command type.
        /// </summary>
        PolyPressure = 0xA0,

        /// <summary>
        /// Represents the controller command type.
        /// </summary>
        Controller = 0xB0,  
  
        /// <summary>
        /// Represents the program change command type.
        /// </summary>
        ProgramChange = 0xC0,

        /// <summary>
        /// Represents the channel pressure (aftertouch) command 
        /// type.
        /// </summary>
        ChannelPressure = 0xD0,   
     
        /// <summary>
        /// Represents the pitch wheel command type.
        /// </summary>
        PitchWheel = 0xE0
    }

    #endregion

    #region Controller Types

    /// <summary>
    /// Specifies constants defining controller types.
    /// </summary>
    public enum ControllerType
    {
        /// <summary>
        /// The Bank Select coarse.
        /// </summary>
        BankSelect,

        /// <summary>
        /// The Modulation Wheel coarse.
        /// </summary>
        ModulationWheel,

        /// <summary>
        /// The Breath Control coarse.
        /// </summary>
        BreathControl,

        /// <summary>
        /// The Foot Pedal coarse.
        /// </summary>
        FootPedal = 4,

        /// <summary>
        /// The Portamento Time coarse.
        /// </summary>
        PortamentoTime,

        /// <summary>
        /// The Data Entry Slider coarse.
        /// </summary>
        DataEntrySlider,

        /// <summary>
        /// The Volume coarse.
        /// </summary>
        Volume,

        /// <summary>
        /// The Balance coarse.
        /// </summary>
        Balance,

        /// <summary>
        /// The Pan position coarse.
        /// </summary>
        Pan = 10,

        /// <summary>
        /// The Expression coarse.
        /// </summary>
        Expression,

        /// <summary>
        /// The Effect Control 1 coarse.
        /// </summary>
        EffectControl1,

        /// <summary>
        /// The Effect Control 2 coarse.
        /// </summary>
        EffectControl2,

        /// <summary>
        /// The General Puprose Slider 1
        /// </summary>
        GeneralPurposeSlider1 = 16,

        /// <summary>
        /// The General Puprose Slider 2
        /// </summary>
        GeneralPurposeSlider2,

        /// <summary>
        /// The General Puprose Slider 3
        /// </summary>
        GeneralPurposeSlider3,

        /// <summary>
        /// The General Puprose Slider 4
        /// </summary>
        GeneralPurposeSlider4,

        /// <summary>
        /// The Bank Select fine.
        /// </summary>
        BankSelectFine = 32,

        /// <summary>
        /// The Modulation Wheel fine.
        /// </summary>
        ModulationWheelFine,

        /// <summary>
        /// The Breath Control fine.
        /// </summary>
        BreathControlFine,

        /// <summary>
        /// The Foot Pedal fine.
        /// </summary>
        FootPedalFine = 36,

        /// <summary>
        /// The Portamento Time fine.
        /// </summary>
        PortamentoTimeFine,

        /// <summary>
        /// The Data Entry Slider fine.
        /// </summary>
        DataEntrySliderFine,

        /// <summary>
        /// The Volume fine.
        /// </summary>
        VolumeFine,

        /// <summary>
        /// The Balance fine.
        /// </summary>
        BalanceFine,

        /// <summary>
        /// The Pan position fine.
        /// </summary>
        PanFine = 42,

        /// <summary>
        /// The Expression fine.
        /// </summary>
        ExpressionFine,

        /// <summary>
        /// The Effect Control 1 fine.
        /// </summary>
        EffectControl1Fine,

        /// <summary>
        /// The Effect Control 2 fine.
        /// </summary>
        EffectControl2Fine,

        /// <summary>
        /// The Hold Pedal.
        /// </summary>
        HoldPedal = 64,

        /// <summary>
        /// The Portamento.
        /// </summary>
        Portamento,

        /// <summary>
        /// The Sustenuto Pedal.
        /// </summary>
        SustenutoPedal,

        /// <summary>
        /// The Soft Pedal.
        /// </summary>
        SoftPedal,

        /// <summary>
        /// The Legato Pedal.
        /// </summary>
        LegatoPedal,

        /// <summary>
        /// The Hold Pedal 2.
        /// </summary>
        HoldPedal2,

        /// <summary>
        /// The Sound Variation.
        /// </summary>
        SoundVariation,

        /// <summary>
        /// The Sound Timbre.
        /// </summary>
        SoundTimbre,

        /// <summary>
        /// The Sound Release Time.
        /// </summary>
        SoundReleaseTime,

        /// <summary>
        /// The Sound Attack Time.
        /// </summary>
        SoundAttackTime,

        /// <summary>
        /// The Sound Brightness.
        /// </summary>
        SoundBrightness,

        /// <summary>
        /// The Sound Control 6.
        /// </summary>
        SoundControl6,

        /// <summary>
        /// The Sound Control 7.
        /// </summary>
        SoundControl7,

        /// <summary>
        /// The Sound Control 8.
        /// </summary>
        SoundControl8,

        /// <summary>
        /// The Sound Control 9.
        /// </summary>
        SoundControl9,

        /// <summary>
        /// The Sound Control 10.
        /// </summary>
        SoundControl10,

        /// <summary>
        /// The General Purpose Button 1.
        /// </summary>
        GeneralPurposeButton1,

        /// <summary>
        /// The General Purpose Button 2.
        /// </summary>
        GeneralPurposeButton2,

        /// <summary>
        /// The General Purpose Button 3.
        /// </summary>
        GeneralPurposeButton3,

        /// <summary>
        /// The General Purpose Button 4.
        /// </summary>
        GeneralPurposeButton4,

        /// <summary>
        /// The Effects Level.
        /// </summary>
        EffectsLevel = 91,

        /// <summary>
        /// The Tremelo Level.
        /// </summary>
        TremeloLevel,
        
        /// <summary>
        /// The Chorus Level.
        /// </summary>
        ChorusLevel,

        /// <summary>
        /// The Celeste Level.
        /// </summary>
        CelesteLevel,

        /// <summary>
        /// The Phaser Level.
        /// </summary>
        PhaserLevel,

        /// <summary>
        /// The Data Button Increment.
        /// </summary>
        DataButtonIncrement,

        /// <summary>
        /// The Data Button Decrement.
        /// </summary>
        DataButtonDecrement,

        /// <summary>
        /// The Nonregistered Parameter Fine.
        /// </summary>
        NonRegisteredParameterFine,

        /// <summary>
        /// The Nonregistered Parameter Coarse.
        /// </summary>
        NonRegisteredParameterCoarse,

        /// <summary>
        /// The Registered Parameter Fine.
        /// </summary>
        RegisteredParameterFine,

        /// <summary>
        /// The Registered Parameter Coarse.
        /// </summary>
        RegisteredParameterCoarse,

        /// <summary>
        /// The All Sound Off.
        /// </summary>
        AllSoundOff = 120,

        /// <summary>
        /// The All Controllers Off.
        /// </summary>
        AllControllersOff,

        /// <summary>
        /// The Local Keyboard.
        /// </summary>
        LocalKeyboard,
        
        /// <summary>
        /// The All Notes Off.
        /// </summary>
        AllNotesOff,

        /// <summary>
        /// The Omni Mode Off.
        /// </summary>
        OmniModeOff,

        /// <summary>
        /// The Omni Mode On.
        /// </summary>
        OmniModeOn,

        /// <summary>
        /// The Mono Operation.
        /// </summary>
        MonoOperation,

        /// <summary>
        /// The Poly Operation.
        /// </summary>
        PolyOperation
    }

    #endregion

	/// <summary>
	/// Represents Midi channel messages.
	/// </summary>
	public class ChannelMessage : ShortMessage
	{
        #region ChannelMessage Members

        #region Constants

        //
        // Bit manipulation constants.
        //

        private const int ChannelMask = 16777200;
        private const int CommandMask = 16776975;

        /// <summary>
        /// Maximum value allowed for Midi channel.
        /// </summary> 
        public const int ChannelValueMax = 15;

        #endregion

        #region Fields

        // MIDI Channel.
        private int channel;

        // Command.
        private ChannelCommand command;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the 
        /// specified command value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by this message.
        /// </param>
        public ChannelMessage(ChannelCommand command)
        {
            Command = command;
            MidiChannel = 0;
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the 
        /// specified command value and Midi channel.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel is set to a value less than zero or 
        /// greater than ChannelValueMax.
        /// </exception>
		public ChannelMessage(ChannelCommand command, int channel)
		{
            Command = command;
            MidiChannel = channel;
		}

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the
        /// specified command value, Midi channel, and first data value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel or the first data value is out of range.
        /// </exception>
        public ChannelMessage(ChannelCommand command, int channel, 
            int data1)
        {
            Command = command;
            MidiChannel = channel;
            Data1 = data1;
        }        

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with the
        /// specified command value, Midi channel, first data value, and 
        /// second data value.
        /// </summary>
        /// <param name="command">
        /// The Midi command represented by the message.
        /// </param>
        /// <param name="channel">
        /// The Midi channel associated with the message.
        /// </param>
        /// <param name="data1">
        /// The first data value.
        /// </param>
        /// <param name="data2">
        /// The second data value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Midi channel, first data value, or second data value 
        /// is out of range.
        /// </exception>
        public ChannelMessage(ChannelCommand command, int channel, 
            int data1, int data2)
        {
            Command = command;
            MidiChannel = channel;
            Data1 = data1;
            Data2 = data2;
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with a 
        /// channel message packed into an integer.
        /// </summary>
        /// <param name="message">
        /// The packed channel message to use for initialization.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the message does not represent a channel message.
        /// </exception>
        public ChannelMessage(int message)
        {
            // Get status byte.
            int status = UnpackStatus(message);

            // Enforce preconditions.
            if(!ChannelMessage.IsChannelMessage(status))
                throw new ArgumentException("Message is not a channel message.",
                    "message");            

            //
            // Initialize properties.
            //

            Command = (ChannelCommand)(message & ~CommandMask);
            MidiChannel = message & ~ChannelMask;
            SetData1(ShortMessage.UnpackData1(message));
            SetData2(ShortMessage.UnpackData2(message));
        }

        /// <summary>
        /// Initializes a new instance of the ChannelMessage class with 
        /// another instance of the ChannelMessage class.
        /// </summary>
        /// <param name="message">
        /// The ChannelMessage instance to use for initialization.
        /// </param>
        public ChannelMessage(ChannelMessage message)
        {
            Command = message.Command;
            MidiChannel = message.MidiChannel;
            Data1 = message.Data1;
            Data2 = message.Data2;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Accepts a MIDI message visitor.
        /// </summary>
        /// <param name="visitor">
        /// The visitor to accept.
        /// </param>
        public override void Accept(MidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Creates a deep copy of this message.
        /// </summary>
        /// <returns>
        /// A deep copy of this message.
        /// </returns>
        public override object Clone()
        {
            return new ChannelMessage(this);
        }        

        /// <summary>
        /// Returns a value indicating how many bytes are used for the 
        /// specified channel command type.
        /// </summary>
        /// <param name="cmd">
        /// The channel command type to test.
        /// </param>
        /// <returns>
        /// The number of bytes used for the specified channel command type.
        /// </returns>
        public static int BytesPerType(ChannelCommand cmd)
        {
            if(cmd == ChannelCommand.ChannelPressure ||
                cmd == ChannelCommand.ProgramChange)
                return 1;
            else
                return 2;
        }

        /// <summary>
        /// Tests to see if a status value belongs to a channel message.
        /// </summary>
        /// <param name="status">
        /// The message to test.
        /// </param>
        /// <returns>
        /// <b>true</b> if the status value belongs to a channel message;
        /// otherwise, <b>false</b>.
        /// </returns>
        public static bool IsChannelMessage(int status)
        {
            // If the status value is in range for channel messages
            if(status >= (int)ChannelCommand.NoteOff &&
                status <= (int)ChannelCommand.PitchWheel + ChannelValueMax)
                return true;

            return false;
        }        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Midi command type.
        /// </summary>
        public ChannelCommand Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;

                SetStatus(MidiChannel | (int)command);
            }
        }

        /// <summary>
        /// Gets or sets the Midi channel.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Midichannel</b> is set to a value less than zero or greater than 
        /// ChannelValueMax.
        /// </exception>
        public int MidiChannel
        {
            get
            {
                return channel;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value > ChannelValueMax)
                    throw new ArgumentOutOfRangeException("MidiChannel",
                        value, "MIDI channel out of range.");
                
                // Assign new Midi channel.
                channel = value;
                SetStatus(channel | (int)command);
            }
        }

        /// <summary>
        /// Gets or sets the first data value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Data1</b> is set to a value less than zero or greater than 
        /// DataValueMax.
        /// </exception>
        public int Data1
        {
            get
            {
                return GetData1();
            }
            set
            {
                SetData1(value);
            }
        }

        /// <summary>
        /// Gets or sets the second data value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <b>Data2</b> is set to a value less than zero or greater than 
        /// DataValueMax.
        /// </exception>
        public int Data2
        {
            get
            {
                return GetData2();
            }
            set
            {
                SetData2(value);
            }
        }

        #endregion        

        #endregion        
    }

    /// <summary>
    /// Provides data for ChannelMessageReceived events.
    /// </summary>
    public class ChannelMessageEventArgs : EventArgs
    {
        #region ChannelMessageEventArgs Members

        #region Fields

        // The ChannelMessage for this event.
        private ChannelMessage message;

        // Time in milliseconds since the input device began recording.
        private int timeStamp;    
    
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ChannelMessageEventArgs class with the 
        /// specified ChannelMessage and time stamp.
        /// </summary>
        /// <param name="message">
        /// The ChannelMessage for this event.
        /// </param>
        /// <param name="timeStamp">
        /// The time in milliseconds since the input device began recording.
        /// </param>
        public ChannelMessageEventArgs(ChannelMessage message, int timeStamp)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ChannelMessage for this event.
        /// </summary>
        public ChannelMessage Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Gets the time in milliseconds since the input device began 
        /// recording.
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
