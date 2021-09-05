using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Endogine.ColorEx;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorChip : UserControl
    {
        public event EventHandler ColorChanged;
        ColorBase _colorObject;
        public ColorChip()
        {
            InitializeComponent();
            //this.BackgroundImage = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.Click += new EventHandler(ColorChip_Click);
        }

        bool _autoOpenEditor;
        public bool AutoOpenEditor
        {
            get { return _autoOpenEditor; }
            set { _autoOpenEditor = value; }
        }

        void ColorChip_Click(object sender, EventArgs e)
        {
            if (this._autoOpenEditor)
            {
                ColorPickerForm form = new ColorPickerForm();
                form.ColorObject = this.ColorObject;
                form.MdiParent = ((System.Windows.Forms.Form)this.TopLevelControl).MdiParent;
                form.ColorChanged += new EventHandler(form_ColorChanged);
                form.ShowDialog();
            }
        }

        void form_ColorChanged(object sender, EventArgs e)
        {
            this.ColorObject = ((ColorPickerForm)sender).ColorObject;
            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }

        public ColorBase ColorObject
        {
            get { return this._colorObject; }
            set
            {
                this._colorObject = value;
                this.Redraw();
            }
        }

        private void Redraw()
        {
            if (this._colorObject != null)
            {
                Graphics g = Graphics.FromImage(this.BackgroundImage);
                BackgroundPattern.Fill(g);

                Rectangle rct = new Rectangle(0, 0, this.Width, this.Height); // - 1, this.Height - 1);

                Color clr = this._colorObject.ColorRGBA;
                //clr = Color.FromArgb(30, clr.R, clr.G, clr.B);
                g.FillRectangle(new SolidBrush(clr), rct);

                clr = Color.FromArgb(255, clr.R, clr.G, clr.B);
                g.FillPolygon(new SolidBrush(clr), new Point[] { new Point(rct.X, rct.Y), new Point(rct.Right, rct.Y), new Point(rct.Right, rct.Bottom) });

                this.Invalidate();
            }
        }

        private void ColorChip_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ColorChip_Resize(object sender, EventArgs e)
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.Redraw();
        }
    }
}
