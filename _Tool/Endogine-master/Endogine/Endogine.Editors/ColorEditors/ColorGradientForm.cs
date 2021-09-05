using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorGradientForm : Form
    {
        public event EventHandler GradientChanged;

        public ColorGradientForm()
        {
            InitializeComponent();
            this.colorGradient1.GradientChanged += new EventHandler(colorGradient1_GradientChanged);
        }

        void colorGradient1_GradientChanged(object sender, EventArgs e)
        {
            if (this.GradientChanged != null)
                this.GradientChanged(this, e);
        }

        public System.Drawing.Drawing2D.ColorBlend ColorBlend
        {
            get { return this.colorGradient1.ColorBlend; }
            set { this.colorGradient1.ColorBlend = value; }
        }
        public Endogine.Interpolation.InterpolatorColor InterpolatorColor
        {
            get { return this.colorGradient1.InterpolatorColor; }
            set { this.colorGradient1.InterpolatorColor = value; }
        }

        private void ColorGradientForm_Load(object sender, EventArgs e)
        {

        }

        private void ColorGradientForm_Resize(object sender, EventArgs e)
        {
            this.colorGradient1.Size = this.ClientRectangle.Size;
        }

    }
}
