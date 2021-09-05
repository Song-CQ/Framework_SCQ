using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class SliderForm : Form
    {

        public SliderForm()
        {
            InitializeComponent();
        }

        public Slider Slider
        {
            get { return this.slider1; }
        }

        private void SliderForm_SizeChanged(object sender, EventArgs e)
        {
            this.panel1.Width = this.Width;
            this.panel1.Height = this.Height;
            this.slider1.Width = this.Width - 1;
            this.slider1.Height = this.Height - this.slider1.Top;
        }

        public bool CloseOnMouseUp
        {
            set
            {
                if (value)
                    this.slider1.DragMouseUp += new EventHandler(slider1_DragMouseUp);
            }
        }

        void slider1_DragMouseUp(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}