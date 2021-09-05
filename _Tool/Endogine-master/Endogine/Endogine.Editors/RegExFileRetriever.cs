using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class RegExFileRetriever : Form
    {
        public RegExFileRetriever()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void RegExFileRetriever_Resize(object sender, EventArgs e)
        {
            this.regExFiles1.Width = ((Control)sender).Width - this.regExFiles1.Left;
            this.regExFiles1.Height = ((Control)sender).Height - this.regExFiles1.Top;

            //this.btnCancel.Location =
            //this.btnOK.Location =
        }

        public string[] FileNames
        {
            get { return this.regExFiles1.FileNames; }
        }
    }
}