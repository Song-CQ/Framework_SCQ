using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorPickerMulti : UserControl
    {
        public event EventHandler ColorChanged;

        bool _dragging;
        Endogine.ColorEx.ColorBase _colorObject;
        Endogine.BitmapHelpers.Canvas _canvas;
        Bitmap _indicator;

        public ColorPickerMulti()
        {
            InitializeComponent();

            this._colorObject = new Endogine.ColorEx.ColorHsb(1, 1, 1, 1);

            this.CreateIndicator();
            this.CreateCanvas();
        }

        private void CreateCanvas()
        {
            int downscale = 4;
            Bitmap bmp = new Bitmap(this.pictureBox1.Width / downscale, this.pictureBox1.Height / downscale, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            this._canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);
            this.pictureBox1.BackgroundImage = bmp;
        }

        public Endogine.ColorEx.ColorBase ColorObject
        {
            get
            {
                return _colorObject;
            }
            set
            {
                _colorObject = value;
                this.colorSlider1.ColorObject = value;
                this.colorSlider1.Value = _colorObject.Vector[this._sliderRepresentsAxis];
                this.DrawRectangle();
            }
        }

        private int _sliderRepresentsAxis;

        public int SliderRepresentsAxis
        {
            get { return _sliderRepresentsAxis; }
            set
            {
                _sliderRepresentsAxis = value;
                this.colorSlider1.AffectedAxis = value;
                this.DrawRectangle();
            }
        }

        public void GetXYIndices(out int x, out int y)
        {
            if (_sliderRepresentsAxis == 0)
            {
                x = 1;
                y = 2;
            }
            else if (_sliderRepresentsAxis == 1)
            {
                x = 0;
                y = 2;
            }
            else
            {
                x = 0;
                y = 1;
            }
        }

        private void DrawRectangle()
        {
            _canvas.Locked = true;
            Endogine.Vector4 clrOrg = _colorObject.Vector;
            clrOrg[_sliderRepresentsAxis] = this.colorSlider1.Value;

            Endogine.Vector4 clr = new Endogine.Vector4(0, 0, 0, 1);
            clr[_sliderRepresentsAxis] = this.colorSlider1.Value;

            int posX, posY;
            this.GetXYIndices(out posX, out posY);

            for (int y = _canvas.Height - 1; y >= 0; y--)
            {
                clr[posY] = 1f-(float)y/_canvas.Height;
                for (int x = _canvas.Width - 1; x >= 0; x--)
                {
                    clr[posX] = (float)x/_canvas.Width;
                    _colorObject.Vector = clr;
                    _canvas.SetPixel(x, y, _colorObject.ColorRGBA);
                }
            }
            _canvas.Locked = false;

            _colorObject.Vector = clrOrg;
            this.pictureBox1.Invalidate();
        }

        public void RenderVivisection(Endogine.Vector3 rotation, float fraction)
        {

        }

        //private void slider1_ValueChanged(object sender, EventArgs e)
        //{
        //    this.DrawRectangle();
        //    this.pictureBox1.Invalidate();
        //}

        private void ColorPickerMulti_Resize(object sender, EventArgs e)
        {
            //this.pictureBox1.Height
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            this.DrawRectangle();
            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }


        private Endogine.Vector4 ScreenToVector()
        {
            return null;
        }

        private void CreateIndicator()
        {
            this._indicator = new Bitmap(15, 15, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(this._indicator);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            float width = 2.0f;
            float off = width / 2;
            g.DrawEllipse(new Pen(Color.Black, width), new RectangleF(off, off, this._indicator.Width - 2 - off, this._indicator.Height - 2 - off));
            g.Dispose();
        }

        private void SetColorFromLocationInRect(Point pnt)
        {
            float x = (float)pnt.X / this.pictureBox1.Width;
            float y = (float)pnt.Y / this.pictureBox1.Height;
            x = Math.Min(1, Math.Max(0, x));
            y = Math.Min(1, Math.Max(0, y));

            Endogine.ColorEx.ColorBase clr = this.ColorObject;
            Endogine.Vector4 v = clr.Vector;

            int posX, posY;
            this.GetXYIndices(out posX, out posY);
            v[posX] = x;
            v[posY] = 1f - y;
            clr.Vector = v;
            this._colorObject = clr;
            this.colorSlider1.ColorObject = clr;

            //this.ColorObject = clr;
            this.pictureBox1.Invalidate();

            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this._dragging = true;
            this.SetColorFromLocationInRect(e.Location);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                this.SetColorFromLocationInRect(e.Location);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this._dragging = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int x,y;
            this.GetXYIndices(out x, out y);

            Endogine.Vector4 v = this.ColorObject.Vector;
            Point p = new Point((int)(v[x] * this.pictureBox1.Width) - this._indicator.Width / 2, (int)((1f - v[y]) * this.pictureBox1.Height) - this._indicator.Height / 2);
            e.Graphics.DrawImage(this._indicator, p);
        }
    }
}
