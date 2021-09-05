using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Midi.UI
{
    /// <summary>
	/// Provides functionality for choosing General MIDI instruments.
	/// </summary>
    public class GMProgramSelector : UserControl
    {
        #region GMProgramSelector Members

        #region Constants

        // Number of instrument families.
        private const int InstrFamilyCount = 16;

        // Number of instruments per family group.
        private const int InstrPerFamily = 8;

        // Maximum program number for the GM instrument set.
        private const int ProgramNumMax = 127;

        #endregion

        #region Instrument Family Names

        private readonly string[] InstrFamilies = 
        {
            "Piano",
            "Chromatic Percussion",
            "Organ",
            "Guitar",
            "Bass",
            "Orchestra",
            "Ensemble",
            "Brass",
            "Reed",
            "Pipe",
            "Synth Lead",
            "Synth Pad",
            "Synth Effects",
            "Ethnic",
            "Percussive",
            "Sound Effects"
        };

        #endregion

        #region Instrument Names

        private readonly string[][] InstrNames;

        #region Piano

        private readonly string[] Piano =
        {
            "Acoustic Grand",
            "Bright Acoustic",
            "Electric Grand",  
            "Honky-Tonk",
            "Electric Piano 1",
            "Electric Piano 2",
            "Harpsicord",
            "Clavinet"
        };      
 
        #endregion

        #region Chromatic Percussion
        
        private readonly string[] ChromPerc =
        {  
            "Celesta",
            "Glockenspiel",
            "Music Box",
            "Vibraphone",
            "Marimba",
            "Xylophone",
            "Tubular Bells",
            "Dulcimer"
        };

        #endregion

        #region Organ

        private readonly string[] Organ =
        {
            "Drawbar Organ",
            "Percussive Organ",
            "Rock Organ",
            "Church Organ",
            "Reed Organ",
            "Accordian",
            "Harmonica",
            "Tango Accordian"
        };

        #endregion 

        #region Guitar

        private readonly string[] Guitar =
        {
            "Nylon String Guitar",
            "Steel String Guitar",
            "Electric Jazz Guitar",
            "Electric Clean Guitar",
            "Electric Muted Guitar",
            "Overdriven Guitar",
            "Distortion Guitar",
            "Guitar Harmonics"
        };

        #endregion

        #region Bass

        private readonly string[] Bass =
        {
            "Acoustic Bass",
            "Electric Bass (finger)",
            "Electric Bass (pick)",
            "Fretless Bass",
            "Slap Bass 1",
            "Slap Bass 2",
            "Synth Bass 1",
            "Synth Bass 2"
        };

        #endregion

        #region Orchestra

        private readonly string[] Orchestra =
        {
            "Violin",
            "Viola",
            "Cello",
            "Contrabass",
            "Tremolo Strings",
            "Pizzicato Strings",
            "Orchestral Harp",
            "Timpani"
        };

        #endregion

        #region Ensemble

        private readonly string[] Ensemble = 
        {
            "String Ensemble 1",
            "String Ensemble 2",
            "Synth Strings 1",
            "Synth Strings 2",
            "Choir Aahs",
            "Choir Oohs",
            "Synth Voice",
            "Orchestra Hit"
        };

        #endregion

        #region Brass

        private readonly string[] Brass =
        {
            "Trumpet",
            "Trombone",
            "Tuba",
            "Muted Trumpet",
            "French Horn",
            "Brass Section",
            "Synth Brass 1",
            "Synth Brass 2"
        };

        #endregion

        #region Reed

        private readonly string[] Reed =
        {
            "Soprano Sax",
            "Alto Sax",
            "Tenor Sax",
            "Baritone Sax",
            "Oboe",
            "English Horn",
            "Bassoon",
            "Clarinet"
        };

        #endregion

        #region Pipe

        private readonly string[] Pipe =
        {
            "Piccolo",
            "Flute",
            "Recorder",
            "Pan Flute",
            "Blown Bottle",
            "Shakuhachi",
            "Whistle",
            "Ocarina"
        };

        #endregion

        #region Synth Lead

        private readonly string[] SynthLead =
        {
            "Square",
            "Sawtooth",
            "Calliope",
            "Chiff",
            "Charang",
            "Voice",
            "Fifths",
            "Bass + Lead"
        };

        #endregion 

        #region Synth Pad

        private readonly string[] SynthPad =
        {
            "New Age",
            "Warm",
            "Polysynth",
            "Choir",
            "Bowed",
            "Metallic",
            "Halo",
            "Sweep"
        };

        #endregion 
        
        #region Synth Effects

        private readonly string[] SynthEffects =
        {
            "Rain",
            "Soundtrack",
            "Crystal",
            "Atmosphere",
            "Brightness",
            "Goblins",
            "Echoes",
            "Sci-Fi"
        };

        #endregion 

        #region Ethnic

        private readonly string[] Ethnic = 
        {
            "Sitar",
            "Banjo",
            "Shamisen",
            "Koto",
            "Kalimba",
            "Bagpipe",
            "Fiddle",
            "Shanai"
        };

        #endregion

        #region Percussive

        private readonly string[] Percussive =
        {
            "Tinkle Bell",
            "Agogo",
            "Steel Drums",
            "Woodblock",
            "Taiko Drum",
            "Melodic Drum",
            "Synth Drum",
            "Reverse Cymbal"
        };

        #endregion

        #region Sound Effects

        private readonly string[] SoundEffects =
        {
            "Guitar Fret Noise",
            "Breath Noise",
            "Seashore",
            "Bird Tweet",
            "Telephone Ring",
            "Helicopter",
            "Applause",
            "Gunshot"
        };

        #endregion

        #endregion

        #region Fields

        // Program number.
        private int programNumber = 0;

        // Boolean value used to indicate whether or not the 
        // SelectedProgramChanged event should be triggered.
        private bool notify = true;

        private System.Windows.Forms.GroupBox grpInstrument;
        private System.Windows.Forms.GroupBox grpInstrFamily;
        private System.Windows.Forms.ComboBox cboInstrFamily;
        private System.Windows.Forms.ComboBox cboInstrName;
        private System.Windows.Forms.GroupBox grpGMPrograms;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selected program is changed.
        /// </summary>
        public event EventHandler SelectedProgramChanged;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the GMProgramSelector class.
        /// </summary>
		public GMProgramSelector()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            // Add instrument family names to instrument family combo box.
            for(int i = 0; i < InstrFamilies.Length; i++)
            {
                cboInstrFamily.Items.Add(InstrFamilies[i]);
            }

            InstrNames = new string[InstrFamilyCount][];

            //
            // Initialize instrument names.
            //

            InstrNames[0] = Piano;
            InstrNames[1] = ChromPerc;
            InstrNames[2] = Organ;
            InstrNames[3] = Guitar;
            InstrNames[4] = Bass;
            InstrNames[5] = Orchestra;
            InstrNames[6] = Ensemble;
            InstrNames[7] = Brass;
            InstrNames[8] = Reed;
            InstrNames[9] = Pipe;
            InstrNames[10] = SynthLead;
            InstrNames[11] = SynthPad;
            InstrNames[12] = SynthEffects;
            InstrNames[13] = Ethnic;
            InstrNames[14] = Percussive;
            InstrNames[15] = SoundEffects;

            ProgramNumber = 0;
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
            this.grpGMPrograms = new System.Windows.Forms.GroupBox();
            this.grpInstrFamily = new System.Windows.Forms.GroupBox();
            this.cboInstrFamily = new System.Windows.Forms.ComboBox();
            this.grpInstrument = new System.Windows.Forms.GroupBox();
            this.cboInstrName = new System.Windows.Forms.ComboBox();
            this.grpGMPrograms.SuspendLayout();
            this.grpInstrFamily.SuspendLayout();
            this.grpInstrument.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGMPrograms
            // 
            this.grpGMPrograms.Controls.Add(this.grpInstrFamily);
            this.grpGMPrograms.Controls.Add(this.grpInstrument);
            this.grpGMPrograms.Location = new System.Drawing.Point(8, 8);
            this.grpGMPrograms.Name = "grpGMPrograms";
            this.grpGMPrograms.Size = new System.Drawing.Size(384, 104);
            this.grpGMPrograms.TabIndex = 0;
            this.grpGMPrograms.TabStop = false;
            this.grpGMPrograms.Text = "GM Programs";
            // 
            // grpInstrFamily
            // 
            this.grpInstrFamily.Controls.Add(this.cboInstrFamily);
            this.grpInstrFamily.Location = new System.Drawing.Point(200, 24);
            this.grpInstrFamily.Name = "grpInstrFamily";
            this.grpInstrFamily.Size = new System.Drawing.Size(168, 64);
            this.grpInstrFamily.TabIndex = 1;
            this.grpInstrFamily.TabStop = false;
            this.grpInstrFamily.Text = "Instrument Family";
            // 
            // cboInstrFamily
            // 
            this.cboInstrFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInstrFamily.Location = new System.Drawing.Point(16, 24);
            this.cboInstrFamily.Name = "cboInstrFamily";
            this.cboInstrFamily.Size = new System.Drawing.Size(136, 21);
            this.cboInstrFamily.TabIndex = 2;
            this.cboInstrFamily.SelectedIndexChanged += new System.EventHandler(this.cboInstrFamily_SelectedIndexChanged);
            // 
            // grpInstrument
            // 
            this.grpInstrument.Controls.Add(this.cboInstrName);
            this.grpInstrument.Location = new System.Drawing.Point(16, 24);
            this.grpInstrument.Name = "grpInstrument";
            this.grpInstrument.Size = new System.Drawing.Size(168, 64);
            this.grpInstrument.TabIndex = 0;
            this.grpInstrument.TabStop = false;
            this.grpInstrument.Text = "Instrument";
            // 
            // cboInstrName
            // 
            this.cboInstrName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInstrName.Location = new System.Drawing.Point(16, 24);
            this.cboInstrName.Name = "cboInstrName";
            this.cboInstrName.Size = new System.Drawing.Size(136, 21);
            this.cboInstrName.TabIndex = 1;
            this.cboInstrName.SelectedIndexChanged += new System.EventHandler(this.cboInstrName_SelectedIndexChanged);
            // 
            // GMProgramSelector
            // 
            this.Controls.Add(this.grpGMPrograms);
            this.Name = "GMProgramSelector";
            this.Size = new System.Drawing.Size(400, 120);
            this.grpGMPrograms.ResumeLayout(false);
            this.grpInstrFamily.ResumeLayout(false);
            this.grpInstrument.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion        

        // Handles choosing an instrument family.
        private void cboInstrFamily_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int familyIndex = cboInstrFamily.SelectedIndex; 
            int instrIndex = cboInstrName.SelectedIndex;

            // Clear the instrument names from the previous instrument family.
            cboInstrName.Items.Clear();               

            // Add the instrument names for the current instrument family.
            for(int i = 0; i < InstrPerFamily; i++)
            {
                cboInstrName.Items.Add(InstrNames[familyIndex][i]);
            }            
            
            // Set to the right index.
            cboInstrName.SelectedIndex = instrIndex;
        }

        // Handles choosing an instrument.
        private void cboInstrName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // If the control should trigger the SelectedProgramChanged event
            if(notify)
            {
                int familyIndex = cboInstrFamily.SelectedIndex;
                int instrIndex = cboInstrName.SelectedIndex;                

                // Calculate program number.
                programNumber = instrIndex + familyIndex * InstrPerFamily;

                // If anyone is listening for the selected program changed 
                // event
                if(SelectedProgramChanged != null)
                {
                    // Trigger event.
                    SelectedProgramChanged(this, new EventArgs());
                } 
            }
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the program number.
        /// </summary>
        public int ProgramNumber
        {
            get
            {
                return programNumber;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value > ProgramNumMax)
                    throw new ArgumentOutOfRangeException("ProgramNumber",
                        value, "Program number out of range.");

                //
                // To prevent the SelectedProgramChanged event from being
                // triggered twice, indicate that the control should not 
                // trigger the SelectedProgramChanged event at this time.
                //

                notify = false;

                // Set the instrument family combo box to the new 
                // instrument family.
                cboInstrFamily.SelectedIndex = value / InstrPerFamily;
                    
                //
                // Indicate that the control can now trigger the 
                // SelectedProgramChanged event.
                //

                notify = true;

                // Set the instrument combo box to the new instrument.
                cboInstrName.SelectedIndex = (value + InstrPerFamily) % InstrPerFamily;
            }
        }

        #endregion

        #endregion
	}
}
