using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Endogine;
using Endogine.ColorEx;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorPickerPainter : UserControl
    {
        ColorBase _colorObject;
        //ColorHsb _hsb;
        //Color _fullHueColor;
        public event EventHandler ColorChanged;
        Bitmap _overlayTriangle;
        Bitmap _indicator;

        Point _triangleSize = new Point(84, 84);
        EPointF _offset = new EPointF(43, 30);
        EPointF[] _trianglePoints;

        bool _changingHue;
        bool _changingSB;
        Quad _quad;

        //public Color Color
        //{
        //    get { return this._hsb.ColorRGBA; }
        //    set
        //    {
        //        this.HSB = Endogine.ColorEx.ColorFunctions.RgbToHsb(value);
        //        //this.Invalidate();
        //        //this._fullHueColor = Color.FromArgb(0, 255, 0);
        //    }
        //}

        public ColorBase ColorObject
        {
            get { return this._colorObject; }
            set
            {
                this._colorObject = value;
                this.pictureBox1.Invalidate();
            }
        }

        private ColorHsb HSB
        {
            get
            {
                if (this._colorObject is ColorHsb)
                    return (ColorHsb)this._colorObject;
                else
                    return new ColorHsb(this._colorObject.ColorRGBA);
            }
            set
            {
                if (!(value is ColorHsb))
                    throw new Exception("Not a HSB value");

                if (this._colorObject is ColorHsb)
                    this._colorObject = value;
                else
                    this._colorObject.ColorRGBA = value.ColorRGBA; //TODO: lossless conversion please... ColorRGBA uses integers!
                //this._hsb = value;
                //this._fullHueColor = new ColorHsb(this._hsb.H, 1, 1).ColorRGBA;
                //this.pictureBox1.Invalidate();
            }
        }

        public ColorPickerPainter()
        {
            InitializeComponent();

            //this.Color = Color.Red;
            this.ColorObject = new ColorRgb(255,0,0);

            this.CreateHueIndicator();

            this.CreateOverlayTriangle();

            this._quad = new Quad(this._trianglePoints[0], this._trianglePoints[1], this._trianglePoints[2], this._trianglePoints[2]);
        }

        private void CreateHueIndicator()
        {
            this._indicator = new Bitmap(15, 15, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(this._indicator);
            g.SmoothingMode = SmoothingMode.HighQuality;
            float width = 2.0f;
            float off = width / 2;
            g.DrawEllipse(new Pen(Color.Black, width), new RectangleF(off, off, this._indicator.Width - 2 - off, this._indicator.Height - 2 - off));
            g.Dispose();
        }

        private void CreateOverlayTriangle()
        {
            this._overlayTriangle = new Bitmap(this._triangleSize.X, this._triangleSize.Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(this._overlayTriangle);
            g.FillRectangle(new SolidBrush(Color.Transparent), new Rectangle(0, 0, this._triangleSize.X, this._triangleSize.Y));
            this._trianglePoints = new EPointF[] { new EPointF(0, 0), new EPointF(this._triangleSize.X, this._triangleSize.Y / 2), new EPointF(0, this._triangleSize.Y) };
            GraphicsPath path = new GraphicsPath(
                new Point[] { this._trianglePoints[0].ToPoint(), this._trianglePoints[1].ToPoint(), this._trianglePoints[2].ToPoint() },
                new byte[] {
                    (byte)PathPointType.Start,
                    (byte)PathPointType.Line,
                    (byte)PathPointType.Line});
            PathGradientBrush pgb = new PathGradientBrush(path);
            pgb.SurroundColors = new Color[] { Color.FromArgb(255, 255, 255, 255), Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0) };
            pgb.CenterColor = Color.FromArgb(0, 0, 0, 0);
            pgb.CenterPoint = this._trianglePoints[1].ToPoint();
            g.FillPath(pgb, path);
        }


        private EPointF Center
        {
            get { return ((this._trianglePoints[0] + this._trianglePoints[1] + this._trianglePoints[2]) / 3 + this._offset); }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this._changingSB = false;
            this._changingHue = false;
            EPointF pMouse = new EPointF(e.X, e.Y);
            EPointF diff = pMouse - this.Center;
            float circleStart = this._triangleSize.X/2*1.414f - 8;
            if (diff.Length > circleStart && diff.Length < circleStart + 17)
            {
                this._changingHue = true;
                //TODO: change color!
            }
            else if (Endogine.Collision.PointLine.PointInTriangle(pMouse - this._offset, this._trianglePoints[0], this._trianglePoints[1], this._trianglePoints[2]))
            {
                this._changingSB = true;
                this.SetSBFromLoc(pMouse);
            }
        }

        private EPointF GetLocFromSB()
        {
            ColorHsb hsb = this.HSB;

            EPointF pnt = this._quad.MapFromRect(new EPointF(hsb.S, 1f - hsb.B), new ERectangleF(0, 0, 1, 1));
            return pnt  + this._offset;
        }

        private void SetSBFromLoc(EPointF pMouse)
        {
            ColorHsb hsb = this.HSB;
            pMouse -= this._offset;

            EPointF vals = this._quad.MapToRect(new EPointF(pMouse.X, pMouse.Y), new ERectangleF(0, 0, 1, 1));
            hsb.S = vals.X;
            hsb.B = 1f - vals.Y;
            hsb.Validate();
            this.HSB = hsb;
        }

        private EPointF GetLocFromHue()
        {
            float angle = (this.HSB.H - 60) / 180 * (float)Math.PI;
            return this.Center + EPointF.FromLengthAndAngle(0.72f * this._trianglePoints[1].X, angle);
        }

        public static EPointF MapPointInQuadToRect()
        {
            return null;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._changingHue)
            {
                EPointF diff = new EPointF(e.X, e.Y) - this.Center;
                float angle = diff.Angle * 180 / (float)Math.PI;
                angle += 60;
                if (angle < 0)
                    angle += 360;
                else if (angle > 360)
                    angle -= 360;
                ColorHsb hsb = this.HSB;
                hsb.H = angle;
                this.HSB = hsb;
                //this.Invalidate();
                //this.pictureBox1.Invalidate();
            }
            else if (this._changingSB)
            {
                this.SetSBFromLoc(new EPointF(e.X, e.Y));
            }

            this.pictureBox1.Invalidate();

            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this._changingHue = false;
            this._changingSB = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(this._triangleSize.X, this._triangleSize.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ColorHsb fullColor = new ColorHsb();
            fullColor.H = this.HSB.H;
            fullColor.S = 1;
            fullColor.B = 1;
            fullColor.A = 255;
            e.Graphics.FillPolygon(new SolidBrush(fullColor.ColorRGBA), new Point[] { (this._trianglePoints[0] + this._offset).ToPoint(), (this._trianglePoints[1] + this._offset).ToPoint(), (this._trianglePoints[2] + this._offset).ToPoint() });

            e.Graphics.DrawImage(this._overlayTriangle, this._offset.ToPoint());
            e.Graphics.DrawImage(this._indicator, (this.GetLocFromHue() - new EPointF(this._indicator.Width, this._indicator.Height)/2 + new EPointF(1,1)).ToPoint());
            //e.Graphics.DrawString(this._fullHueColor.ToString(), new Font("Arial", 6), new SolidBrush(Color.Blue), new PointF(0, 0));

            e.Graphics.DrawImage(this._indicator, (this.GetLocFromSB() - new EPointF(this._indicator.Width, this._indicator.Height) / 2 + new EPointF(1, 1)).ToPoint());
        }
    }
}
