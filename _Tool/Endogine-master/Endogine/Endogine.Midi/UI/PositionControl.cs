using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Midi.UI
{
	/// <summary>
	/// Summary description for PositionControl.
	/// </summary>
	public class PositionControl : UserControl
	{
        //private int pulsesPerQuarterNote = TickGenerator.PpqnMin;
        //private int beatsPerBar;

        private System.Windows.Forms.Label barLabel;
        private System.Windows.Forms.NumericUpDown barNumericUpDown;
        private System.Windows.Forms.NumericUpDown beatNumericUpDown;
        private System.Windows.Forms.Label beatLabel;
        private System.Windows.Forms.NumericUpDown tickNumericUpDown;
        private System.Windows.Forms.Label tickLabel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        /// <summary>
        /// 
        /// </summary>
		public PositionControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

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
            this.barLabel = new System.Windows.Forms.Label();
            this.barNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.beatNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.beatLabel = new System.Windows.Forms.Label();
            this.tickNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tickLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.barNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beatNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tickNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // barLabel
            // 
            this.barLabel.Location = new System.Drawing.Point(16, 8);
            this.barLabel.Name = "barLabel";
            this.barLabel.Size = new System.Drawing.Size(24, 16);
            this.barLabel.TabIndex = 0;
            this.barLabel.Text = "Bar";
            this.barLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // barNumericUpDown
            // 
            this.barNumericUpDown.Location = new System.Drawing.Point(8, 32);
            this.barNumericUpDown.Maximum = new System.Decimal(new int[] {
                                                                             10000,
                                                                             0,
                                                                             0,
                                                                             0});
            this.barNumericUpDown.Name = "barNumericUpDown";
            this.barNumericUpDown.Size = new System.Drawing.Size(48, 20);
            this.barNumericUpDown.TabIndex = 1;
            this.barNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // beatNumericUpDown
            // 
            this.beatNumericUpDown.Location = new System.Drawing.Point(64, 32);
            this.beatNumericUpDown.Maximum = new System.Decimal(new int[] {
                                                                              16,
                                                                              0,
                                                                              0,
                                                                              0});
            this.beatNumericUpDown.Name = "beatNumericUpDown";
            this.beatNumericUpDown.Size = new System.Drawing.Size(36, 20);
            this.beatNumericUpDown.TabIndex = 2;
            this.beatNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // beatLabel
            // 
            this.beatLabel.Location = new System.Drawing.Point(64, 8);
            this.beatLabel.Name = "beatLabel";
            this.beatLabel.Size = new System.Drawing.Size(32, 16);
            this.beatLabel.TabIndex = 3;
            this.beatLabel.Text = "Beat";
            this.beatLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tickNumericUpDown
            // 
            this.tickNumericUpDown.Location = new System.Drawing.Point(104, 32);
            this.tickNumericUpDown.Maximum = new System.Decimal(new int[] {
                                                                              960,
                                                                              0,
                                                                              0,
                                                                              0});
            this.tickNumericUpDown.Name = "tickNumericUpDown";
            this.tickNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.tickNumericUpDown.TabIndex = 4;
            this.tickNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tickLabel
            // 
            this.tickLabel.Location = new System.Drawing.Point(104, 8);
            this.tickLabel.Name = "tickLabel";
            this.tickLabel.Size = new System.Drawing.Size(32, 16);
            this.tickLabel.TabIndex = 5;
            this.tickLabel.Text = "Tick";
            this.tickLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // PositionControl
            // 
            this.Controls.Add(this.tickLabel);
            this.Controls.Add(this.tickNumericUpDown);
            this.Controls.Add(this.beatLabel);
            this.Controls.Add(this.beatNumericUpDown);
            this.Controls.Add(this.barNumericUpDown);
            this.Controls.Add(this.barLabel);
            this.Name = "PositionControl";
            this.Size = new System.Drawing.Size(152, 64);
            ((System.ComponentModel.ISupportInitialize)(this.barNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beatNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tickNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion
	}
}
