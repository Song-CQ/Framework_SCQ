/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/26/2004
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Endogine.Midi.UI
{
    /// <summary>
    /// Represents a dialog box for selecting a Midi output device.
    /// </summary>
    public class MidiOutDeviceDialog : System.Windows.Forms.Form
    {
        #region MidiOutDeviceDialog Members

        #region Fields

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboOutDevices;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiOutDeviceDlg.
        /// </summary>
        public MidiOutDeviceDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Initialize ComboBox.
            InitializeComboBox();
            
            // If there are any output devices available, set the selected 
            // device ID to the first device.
            if(OutputDevice.DeviceCount > 0)
            {
                SelectedDeviceID = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MidiOutDeviceDlg with the 
        /// specified output device Id.
        /// </summary>
        public MidiOutDeviceDialog(int deviceID)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Initialize ComboBox.
            InitializeComboBox();

            // Set the selected device Id.
            SelectedDeviceID = deviceID;            
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboOutDevices = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(32, 72);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(144, 72);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            // 
            // cboOutDevices
            // 
            this.cboOutDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutDevices.Location = new System.Drawing.Point(16, 24);
            this.cboOutDevices.Name = "cboOutDevices";
            this.cboOutDevices.Size = new System.Drawing.Size(216, 21);
            this.cboOutDevices.TabIndex = 0;
            // 
            // MidiOutDeviceDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(248, 110);
            this.Controls.Add(this.cboOutDevices);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "MidiOutDeviceDialog";
            this.Text = "Output Devices";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Initializes ComboBox.
        /// </summary>
        private void InitializeComboBox()
        {
            // The number of output devices available.
            int deviceCount = OutputDevice.DeviceCount;

            // The capabilities of an output device.
            MidiOutCaps caps;

            // For converting ASCII byte text to a string.
            ASCIIEncoding encoder = new ASCIIEncoding();

            try
            {
                // Add output device names to the combo box.
                for(int i = 0; i < deviceCount; i++)
                {
                    caps = OutputDevice.GetCapabilities(i);
                    cboOutDevices.Items.Add(encoder.GetString(caps.name));
                }
            }
            catch(OutputDeviceException ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, 
                    MessageBoxIcon.Stop);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected device ID.
        /// </summary>
        public int SelectedDeviceID
        {
            get
            {
                return cboOutDevices.SelectedIndex;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value >= cboOutDevices.Items.Count)
                    throw new ArgumentOutOfRangeException("SelectedDeviceID",
                        value, "Selected device ID out of range.");

                // Set the combo box to the selected device.
                cboOutDevices.SelectedIndex = value;
            }
        }

        #endregion

        #endregion
    }
}
