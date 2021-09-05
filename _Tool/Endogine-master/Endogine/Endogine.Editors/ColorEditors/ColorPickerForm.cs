using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Endogine.ColorEx;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorPickerForm : Form, IColorPickerForm
    {
        private ColorPickerMultiEx colorPickerMultiEx1;
        public event EventHandler ColorChanged;
        bool updateFromTextBox = true;
        //http://userfs.cec.wustl.edu/~cse452/lectures/Color.4pp.pdf

        public ColorPickerForm()
        {
            InitializeComponent();

            this.colorPickerPainter1.ColorChanged += new EventHandler(colorPickerPainter1_ColorChanged);

            this.colorNumericHSB.ColorChanged += new EventHandler(colorNumericHSB_ColorChanged);
            this.colorNumericRGB.ColorChanged += new EventHandler(colorNumericRGB_ColorChanged);
            this.numAlpha.ValueChanged += new EventHandler(numAlpha_ValueChanged);
        }

        void colorNumericHSB_ColorChanged(object sender, EventArgs e)
        {
            this.colorPickerPainter1.ColorObject = this.colorNumericHSB.ColorObject;
            this.UpdateAllExcept("HSB");
        }

        void colorNumericRGB_ColorChanged(object sender, EventArgs e)
        {
            this.colorPickerPainter1.ColorObject = this.colorNumericRGB.ColorObject;
            this.UpdateAllExcept("RGB");
        }

        public void SetStartPositionRelativeLoc(Endogine.EPoint loc)
        {
            if (loc == null)
                loc = new Endogine.EPoint(Cursor.Position);
            //TODO: set a prop instead and automatically do this on Show(). But: can't override show, which event is it?
            this.Location = (loc + new Endogine.EPoint(-this.Width / 2, 15)).ToPoint();
            //TODO: if too far to left, right or bottom
        }

        void numAlpha_ValueChanged(object sender, EventArgs e)
        {
            ColorBase clr = this.colorPickerPainter1.ColorObject;
            clr.A = (int)this.numAlpha.Value;
            this.colorPickerPainter1.ColorObject = clr;
            this.UpdateAllExcept("Alpha");
        }

        void colorPickerPainter1_ColorChanged(object sender, EventArgs e)
        {
            this.UpdateAllExcept(null);
        }

        private void UpdateAllExcept(string exceptWhat)
        {
            if (exceptWhat == null)
                exceptWhat = "";

            //Color clr = this.colorPickerPainter1.Color;
            ColorBase clr = this.colorPickerPainter1.ColorObject;
            clr.Validate();
            this.colorChip1.ColorObject = clr;

            if (exceptWhat.IndexOf("Alpha") < 0)
                this.numAlpha.Value = clr.A;

            if (exceptWhat.IndexOf("RGB") < 0)
            {
                //if (clr is ColorRgb)
                //    this.colorNumericRGB.ColorObject = clr;
                //else
                this.colorNumericRGB.ColorObject = new ColorRgb(clr.RgbFloat);
            }
            if (exceptWhat.IndexOf("HSB") < 0)
            {
                if (clr is ColorHsb)
                    this.colorNumericHSB.ColorObject = clr;
                else
                    this.colorNumericHSB.ColorObject = new ColorHsb(clr.RgbFloat);
            }
            if (exceptWhat.IndexOf("Multi") < 0 && this.colorPickerMultiEx1 != null)
                this.colorPickerMultiEx1.ColorObject = clr;

            if (exceptWhat.IndexOf("ColorTextBox") < 0)
            {
                this.updateFromTextBox = false;
                Endogine.ColorEx.ColorRgb rgb = new Endogine.ColorEx.ColorRgb(clr.ColorRGBA);
                this.tbColorText.Text = rgb.ToString("X");
                this.updateFromTextBox = true;
            }

            if (exceptWhat.IndexOf("Event") < 0)
                if (this.ColorChanged != null)
                    this.ColorChanged(this, null);
        }

        //public Color Color
        //{
        //    get { return this.colorPickerPainter1.Color; }
        //    set
        //    {
        //        this.colorPickerPainter1.Color = value;
        //        this.UpdateAllExcept("Event");
        //    }
        //}

        public Endogine.ColorEx.ColorBase ColorObject
        {
            get { return this.colorPickerPainter1.ColorObject; }
            set
            {
                //MessageBox.Show("1: " + value.Vector.ToString() + " " + value.GetType());
                this.colorPickerPainter1.ColorObject = value;
                //MessageBox.Show("2: " + value.Vector.ToString() + " " + value.GetType());
                this.UpdateAllExcept("Event");
            }
        }

        private void colorPickerMultiEx1_ColorChanged(object sender, EventArgs e)
        {
            this.colorPickerPainter1.ColorObject = this.colorPickerMultiEx1.ColorObject;
            this.UpdateAllExcept("Multi");
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            if (this.btnMore.Text == "More =>")
            {
                this.btnMore.Text = "<= Less";
                this.colorPickerMultiEx1 = new ColorPickerMultiEx();
                this.colorPickerMultiEx1.ColorChanged+=new EventHandler(colorPickerMultiEx1_ColorChanged);
                this.Controls.Add(this.colorPickerMultiEx1);
                this.colorPickerMultiEx1.ColorObject = this.ColorObject;
                this.colorPickerMultiEx1.Location = new Point(this.btnMore.Right + 4, 0);
                this.Width += this.colorPickerMultiEx1.Width;
                //this.btnMore.Top = this.colorPickerMultiEx1.Bottom - this.btnMore.Height;
                int dragbarHeight = this.Height - this.ClientRectangle.Height;
                this.Height = this.colorPickerMultiEx1.Bottom + 3 + dragbarHeight;
            }
            else
            {
                this.btnMore.Text = "More =>";
                this.Width -= this.colorPickerMultiEx1.Width;
                this.colorPickerMultiEx1.Dispose();
                this.colorPickerMultiEx1 = null;
                //this.btnMore.Top = this.tbColorText.Bottom - this.btnMore.Height;

                int dragbarHeight = this.Height - this.ClientRectangle.Height;
                this.Height = this.tbColorText.Bottom + 3 + dragbarHeight;
            }
        }



        private void tbColorText_TextChanged(object sender, EventArgs e)
        {
            if (this.updateFromTextBox)
                this.SetFromColorTextBox();
        }

        private void SetFromColorTextBox()
        {
            try
            {
                Endogine.ColorEx.ColorRgb clr = new Endogine.ColorEx.ColorRgb(this.tbColorText.Text);
                this.colorPickerPainter1.ColorObject = clr;
                this.UpdateAllExcept("ColorTextBox");
            }
            catch
            {
            }
        }

        private void tbColorText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                this.SetFromColorTextBox();
        }
    }
}