using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Midi.UI
{
	/// <summary>
	/// Provides functionality for choosing Midi notes.
	/// </summary>
	public class NoteSelector : UserControl
	{
        #region NoteSelector Members

        #region Constants

        // Note per octave.
        private const int NotePerOctave = 12;

        // Maximum octave value.
        private const int OctaveMax = 10;

        #endregion

        #region Fields

        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.ComboBox cboNote;
        private System.Windows.Forms.ComboBox cboOctave;
        private System.Windows.Forms.Label lblOctave;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the note number has changed.
        /// </summary>
        public event EventHandler NoteNumberChanged;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the NoteSelector class.
        /// </summary>
		public NoteSelector()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            // Initialize note number;
            NoteNumber = 0;
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
            this.lblNote = new System.Windows.Forms.Label();
            this.cboNote = new System.Windows.Forms.ComboBox();
            this.cboOctave = new System.Windows.Forms.ComboBox();
            this.lblOctave = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNote
            // 
            this.lblNote.Location = new System.Drawing.Point(8, 8);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(32, 16);
            this.lblNote.TabIndex = 0;
            this.lblNote.Text = "Note";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // cboNote
            // 
            this.cboNote.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNote.Items.AddRange(new object[] {
                                                         "C",
                                                         "C#",
                                                         "D",
                                                         "D#",
                                                         "E",
                                                         "F",
                                                         "F#",
                                                         "G",
                                                         "G#",
                                                         "A",
                                                         "A#",
                                                         "B"});
            this.cboNote.Location = new System.Drawing.Point(8, 32);
            this.cboNote.Name = "cboNote";
            this.cboNote.Size = new System.Drawing.Size(40, 21);
            this.cboNote.TabIndex = 1;
            this.cboNote.SelectedIndexChanged += new System.EventHandler(this.cboNote_SelectedIndexChanged);
            // 
            // cboOctave
            // 
            this.cboOctave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOctave.Items.AddRange(new object[] {
                                                           "-1",
                                                           "0",
                                                           "1",
                                                           "2",
                                                           "3",
                                                           "4",
                                                           "5",
                                                           "6",
                                                           "7",
                                                           "8",
                                                           "9"});
            this.cboOctave.Location = new System.Drawing.Point(64, 32);
            this.cboOctave.Name = "cboOctave";
            this.cboOctave.Size = new System.Drawing.Size(40, 21);
            this.cboOctave.TabIndex = 3;
            this.cboOctave.SelectedIndexChanged += new System.EventHandler(this.cboOctave_SelectedIndexChanged);
            // 
            // lblOctave
            // 
            this.lblOctave.Location = new System.Drawing.Point(64, 10);
            this.lblOctave.Name = "lblOctave";
            this.lblOctave.Size = new System.Drawing.Size(40, 16);
            this.lblOctave.TabIndex = 2;
            this.lblOctave.Text = "Octave";
            this.lblOctave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // NoteSelector
            // 
            this.Controls.Add(this.cboOctave);
            this.Controls.Add(this.lblOctave);
            this.Controls.Add(this.cboNote);
            this.Controls.Add(this.lblNote);
            this.Name = "NoteSelector";
            this.Size = new System.Drawing.Size(120, 64);
            this.ResumeLayout(false);

        }
		#endregion

        // Note selection change handler.
        private void cboNote_SelectedIndexChanged(object sender, System.EventArgs e)
        { 
            // If the note is set above G.
            if(cboNote.SelectedIndex > (int)Endogine.Audio.Note.G)
            {
                // If the octave range extends to the maximum octave value.
                if(cboOctave.Items.Count - 1 == OctaveMax)
                {
                    // Remove highest octave. This prevents a note value above
                    // 127 (maximum Midi note number) from being chosen.
                    cboOctave.Items.Remove("9");
                }
            }
            // Else the note is set at G or below.
            else
            {
                // If the highest octave value has been removed.
                if(cboOctave.Items.Count - 1 < OctaveMax)
                {
                    // Add the highest octave value back.
                    cboOctave.Items.Add("9");
                }
            }

            // If anyone is listening for the note number to change.
            if(NoteNumberChanged != null)
            {
                NoteNumberChanged(this, new EventArgs());
            }
        }

        // Octave selection change handler.
        private void cboOctave_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // If the octave is set to the highest value.
            if(cboOctave.SelectedIndex == OctaveMax)
            {
                // If the note range extends beyond note G.
                if(cboNote.Items.Count - 1 > (int)Endogine.Audio.Note.G)
                {
                    //
                    // Remove notes above G. This prevents a note value above
                    // 127 (maximum Midi note number) from being chosen.
                    //

                    cboNote.Items.Remove("G#");
                    cboNote.Items.Remove("A");
                    cboNote.Items.Remove("A#");
                    cboNote.Items.Remove("B");
                }
            }
            // Else the octave is set below the highest value.
            else
            {
                // If the notes above G have been removed.
                if(cboNote.Items.Count - 1 < (int)Endogine.Audio.Note.GSharp)
                {
                    //
                    // Add the note back.
                    // 

                    cboNote.Items.Add("G#");
                    cboNote.Items.Add("A");
                    cboNote.Items.Add("A#");
                    cboNote.Items.Add("B");
                }
            }

            // If anyone is listening for the note number to change.
            if(NoteNumberChanged != null)
            {
                NoteNumberChanged(this, new EventArgs());
            }        
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the note number.
        /// </summary>
        public int NoteNumber
        {
            get
            {
                int octave = cboOctave.SelectedIndex;
                int note = cboNote.SelectedIndex;

                return octave * NotePerOctave + note;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value > ShortMessage.DataValueMax)
                    throw new ArgumentOutOfRangeException("NoteNumber", value,
                        "Note number out of range.");

                int octave = value / NotePerOctave;
                int note = value - NotePerOctave * octave;
                    
                // Remove event handler to prevent the NoteNumberChanged 
                // event from being triggered twice.
                cboOctave.SelectedIndexChanged -= 
                    new EventHandler(cboOctave_SelectedIndexChanged);

                cboOctave.SelectedIndex = octave;

                // Add event handler back.
                cboOctave.SelectedIndexChanged += 
                    new EventHandler(cboOctave_SelectedIndexChanged);

                cboNote.SelectedIndex = note;
            }
        }

        #endregion

        #endregion
	}
}
