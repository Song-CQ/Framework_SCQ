using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Endogine.Editors
{
    public partial class Slider : UserControl
    {
        public event EventHandler ValueChanged;
        public event EventHandler DragMouseUp;

        bool _dragging;

        private float _min = 0;
        private float _max = 1;
        private float _value = 0.5f;
        private int _majorTicks = 10;
        private float _stepSize = 0;

        bool _justFaked;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //const int MOUSEEVENTF_MOVE = 0x00000001;
        const int MOUSEEVENTF_LEFTDOWN = 0x00000002;
        const int MOUSEEVENTF_LEFTUP = 0x00000004;
        //const int MOUSEEVENTF_RIGHTDOWN = 0x00000008;
        //const int MOUSEEVENTF_RIGHTUP = 0x00000010;
        //const int MOUSEEVENTF_MIDDLEDOWN = 0x00000020;
        //const int MOUSEEVENTF_MIDDLEUP = 0x00000040;
        //const int MOUSEEVENTF_WHEEL = 0x00000800;
        //const int MOUSEEVENTF_ABSOLUTE = 0x00008000;



        public float StepSize
        {
            get { return _stepSize; }
            set { _stepSize = value; }
        }


        public int MajorTicks
        {
            get { return _majorTicks; }
            set { _majorTicks = value; }
        }


        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }
        
        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }
        
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.panel1.Top = this.PointToClient(this.ValueToLoc(value)).Y;
            }
        }

        public float ValueAs0to1
        {
            get { return (this._value - this._min) / (this._max - this._min); }
            set { this._value = value * (this._max - this._min) + this._min; }
        }

        public Point ValueToLoc(float val)
        {
            val = 1f - (val - this._min) / (this._max - this._min);
            Point p = new Point(0, (int)(val * (this.Height - this.panel1.Height)));
            return this.PointToScreen(p);
        }
        public Point ValueToCenterLoc(float val)
        {
            Point p = this.ValueToLoc(val);
            p = new Point(p.X + this.panel1.Width / 2, p.Y + this.panel1.Height / 2);
            return p;
        }

        /// <summary>
        /// Location on screen to value in slider
        /// </summary>
        /// <param name="p">Screen loc</param>
        /// <returns></returns>
        public float LocToValue(Point p)
        {
            p = this.PointToClient(p);
            float val = 1f - (float)p.Y / (this.Height -this.panel1.Height);
            if (val > 1) val = 1;
            else if (val < 0) val = 0;
            return val * (this._max - this._min) + this._min;
        }

        public bool Dragging
        {
            get { return this._dragging; }
            set { this._dragging = value; }
        }

        public Slider()
        {
            InitializeComponent();

            this.Value = this._value;
        }

        private void MoveTo(Point newLoc)
        {
            Point p = this.PointToClient(newLoc);
            int top = p.Y - this.panel1.Height / 2;
            if (top < 0)
                top = 0;
            else if (top > this.Height - this.panel1.Height)
                top = this.Height - this.panel1.Height;

            this.panel1.Top = top;
            float newValue = this.LocToValue(this.PointToScreen(this.panel1.Location));
            if (this._stepSize > 0)
            {
                float val = newValue - this._min;
                val = (float)Math.Round(val / this._stepSize) * this._stepSize;
                newValue = val + this._min;
                //float val = (this._value - this._min) / (this._max - this._min);
                //float factor = this._minorTicks +0;
                //val = (float)Math.Round(val * factor);
                //val = val / factor;
                //this._value = val * (this._max - this._min) + this._min;
            }

            if (Form.ModifierKeys == Keys.Control)
            {
                //TODO: lock to step!
            }

            if (newValue == this._value)
                return;

            this._value = newValue;

            if (this.ValueChanged != null)
                this.ValueChanged(this, null);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this._dragging = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                this.MoveTo(panel1.PointToScreen(e.Location));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            this._dragging = false;
            if (this.DragMouseUp != null)
                this.DragMouseUp(this, e);
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
                this.MoveTo(this.PointToScreen(e.Location));
        }

        private void Slider_Resize(object sender, EventArgs e)
        {
            this.panel1.Width = this.Width-1;
        }

        private void Slider_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._justFaked)
                return;

            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            this.MoveTo(this.PointToScreen(e.Location));
            this._justFaked = true;
            this.FakeMouse();
            this._justFaked = false;
        }

        public void FakeMouse()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
    }
}
