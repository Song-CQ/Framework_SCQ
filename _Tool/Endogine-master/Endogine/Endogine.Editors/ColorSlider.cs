using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class ColorSlider : Slider
    {
        private Endogine.BitmapHelpers.Canvas _canvas;
        private Endogine.ColorEx.ColorBase _colorObject;
        private int _axis;
        public ColorSlider()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(1, 100, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            _canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);

            this._colorObject = new Endogine.ColorEx.ColorHsb();
            this._colorObject.Vector = new Endogine.Vector4(1, 1, 1, 1);

            this.DrawBackground();
        }

        private void DrawBackground()
        {
            _canvas.Locked = true;

            Endogine.Vector4 vClrOrg = this._colorObject.Vector.Copy();
            Endogine.Vector4 vClr = this._colorObject.Vector.Copy();
            for (int i = 0; i < this._canvas.Height; i++)
            {
                vClr[this._axis] = 1f - (float)i / this._canvas.Height;
                this._colorObject.Vector = vClr;
                this._canvas.SetPixel(0,i, this._colorObject.ColorRGBA);
            }
            this._colorObject.Vector = vClrOrg;

            _canvas.Locked = false;
            this.BackgroundImage = _canvas.ToBitmap();
            this.Invalidate();
            //this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public Endogine.ColorEx.ColorBase ColorObject
        {
            set
            {
                this._colorObject = value;
                this.DrawBackground();
            }
        }

        public int AffectedAxis
        {
            set { this._axis = value; this.DrawBackground(); }
        }
    }
}
