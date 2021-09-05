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
    /// Represents a dialog box for selecting a Midi input device.
    /// </summary>
    public class MidiInDeviceDialog : System.Windows.Forms.Form
    {
        #region MidiInDeviceDialog Members

        #region Fields

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboInDevices;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiInDeviceDlg.
        /// </summary>
        public MidiInDeviceDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Initialize ComboBox.
            InitializeComboBox();
            
            // If there are any input devices available, set the selected 
            // device ID to the first device.
            if(InputDevice.DeviceCount > 0)
            {
                SelectedDeviceID = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MidiInDeviceDlg with the 
        /// specified intput device Id.
        /// </summary>
        public MidiInDeviceDialog(int deviceID)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Initialize ComboBox.
            InitializeComboBox();

            // Set the selected device Id.
			if (deviceID >= 0)
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
            this.cboInDevices = new System.Windows.Forms.ComboBox();
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
            // cboInDevices
            // 
            this.cboInDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInDevices.Location = new System.Drawing.Point(16, 24);
            this.cboInDevices.Name = "cboInDevices";
            this.cboInDevices.Size = new System.Drawing.Size(216, 21);
            this.cboInDevices.TabIndex = 0;
            // 
            // MidiInDeviceDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(248, 110);
            this.Controls.Add(this.cboInDevices);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "MidiInDeviceDialog";
            this.Text = "Input Devices";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Initializes ComboBox.
        /// </summary>
        private void InitializeComboBox()
        {
            // The number of input devices available.
            int deviceCount = InputDevice.DeviceCount;

            // The capabilities of an input device.
            MidiInCaps caps;

            // For converting ASCII byte text to a string.
            ASCIIEncoding encoder = new ASCIIEncoding();

            try
            {
                // Add input device names to the combo box.
                for(int i = 0; i < deviceCount; i++)
                {
                    caps = InputDevice.GetCapabilities(i);
                    cboInDevices.Items.Add(encoder.GetString(caps.name));
                }
            }
            catch(InputDeviceException ex)
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
                return cboInDevices.SelectedIndex;
            }
            set
            {
                // Enforce preconditions.
                if(value < 0 || value >= cboInDevices.Items.Count)
                    throw new ArgumentOutOfRangeException("SelectedDeviceID",
                        value, "Selected device ID out of range.");

                // Set the combo box to the selected device.
                cboInDevices.SelectedIndex = value;
            }
        }

        #endregion

        #endregion
    }
}
