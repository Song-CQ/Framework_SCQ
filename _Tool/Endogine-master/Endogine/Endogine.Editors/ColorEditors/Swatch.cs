using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors.ColorEditors
{
    public partial class Swatch : UserControl
    {
        ColorEx.ColorBase _color;

        public Swatch()
        {
            InitializeComponent();
        }

        public ColorEx.ColorBase Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                if (this._color != null)
                    this.BackColor = value.ColorRGBA; //ForeColor
            }
        }


        void form_ColorChanged(object sender, EventArgs e)
        {
            //this.Color = new Endogine.ColorEx.ColorRgb(((ColorPickerForm)sender).Color);
            this.Color = ((ColorPickerForm)sender).ColorObject;
            this.Invalidate();
        }

        private void Swatch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColorPickerForm form = new ColorPickerForm();
            form.ColorObject = this._color; //.ColorRGBA;
            form.ColorChanged += new EventHandler(form_ColorChanged);
            form.Show();
        }

        public bool Selected
        {
            get { return (this.BorderStyle == BorderStyle.None); }
            set
            {
                //TODO: would like to change border color, but not possible?
                if (value)
                    this.BorderStyle = BorderStyle.None;
                else
                    this.BorderStyle = BorderStyle.FixedSingle;
            }
        }
    }
}

