using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors.ColorEditors
{
    public partial class SwatchesForm : Form, Endogine.Editors.ISwatchesForm
    {
        public SwatchesForm()
        {
            InitializeComponent();

            this.SwatchesForm_Resize(null, null);
        }

        public SwatchesPanel Swatches
        {
            get { return this.swatchesPanel1; }
        }

        void SwatchesForm_Resize(object sender, System.EventArgs e)
        {
            this.panel1.Width = this.ClientSize.Width - this.panel1.Left;
            this.panel1.Height = this.ClientSize.Height - this.panel1.Top;// +(this.Height - this.ClientSize.Height);

            this.swatchesPanel1.SuspendLayout();
            this.swatchesPanel1.Width = this.ClientSize.Width - this.swatchesPanel1.Left;
            //this.swatchesPanel1.Height = this.Height - this.swatchesPanel1.Top + (this.Height - this.ClientSize.Height);

            if (this.swatchesPanel1.Height > this.panel1.Height)
            {
                this.swatchesPanel1.Width -= this.panel1.AutoScrollMinSize.Width + 20;
            }
            this.swatchesPanel1.ResumeLayout();
        }

        public void LoadPalette(string filename)
        {
            this.swatchesPanel1.LoadPalette(filename); //@"C:\Documents and Settings\Jonas\Desktop\Color Swatches\Windows.act"); //Web Spectrum.aco Web Spectrum.aco
        }
        private void loadReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "aco";
            ofd.Filter = "Palettes (*.aco,*.act,*.txt,*pal)|*.aco;*.act;*.txt;*.pal|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.LoadPalette(ofd.FileName);
            }
        }
    }
}