using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class NumericSlider : UserControl
    {
        public event EventHandler ValueChanged;
        Point _ptMouseStart;
        SliderForm _sliderForm;

        private float _min = 0;
        private float _max = 1;
        private float _stepSize = 0;
        private float _value;

        bool _internalTextboxChange = false;

        public float Value
        {
            get
            {
                //if (this.UIMax != 0 && this.UIMin != null)
                return _value;
            }
            set
            {
                _value = value;
                this.SetTextBoxValue(value);
            }
        }

        public float StepSize
        {
            get { return _stepSize; }
            set { _stepSize = value; }
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

        private float _uiMax;

        public float UIMax
        {
            get { return _uiMax; }
            set { _uiMax = value; }
        }
        
        private float _uiMin;
        /// <summary>
        /// When the user interface should show other values than the "actual"
        /// </summary>
        public float UIMin
        {
            get { return _uiMin; }
            set { _uiMin = value; }
        }

        public NumericSlider()
        {
            InitializeComponent();

            this.textBox1.Text = "0";
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            //Form form = new Form();
            _sliderForm = new SliderForm();
            //form.Size
            _sliderForm.Visible = false;
            _sliderForm.Slider.Min = this._min;
            _sliderForm.Slider.Max = this._max;
            _sliderForm.Slider.StepSize = this._stepSize;
            _sliderForm.Slider.Value = this._value;

            this._ptMouseStart = Cursor.Position;

            _sliderForm.Show();
            _sliderForm.Width = 12;
            _sliderForm.Height = 200;
            float val = Convert.ToSingle(this.textBox1.Text);
            //if (Math.Abs(this._uiMax) > 0.001f || Math.Abs(this._uiMin) > 0.001f)
            //    val = (this._value - this._uiMin) / (this._uiMax - this._uiMin);
            //_sliderForm.Slider.Value = val;



            Point loc = _sliderForm.Slider.ValueToCenterLoc(_sliderForm.Slider.Value);
            Point diffSliderToForm = new Point(loc.X - _sliderForm.Location.X, loc.Y - _sliderForm.Location.Y);
            _sliderForm.Location = new Point(_ptMouseStart.X - diffSliderToForm.X, _ptMouseStart.Y - diffSliderToForm.Y);
            Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;
            int diff = _sliderForm.Bottom - screenRect.Bottom;
            if (diff > 0)
                _sliderForm.Top -= diff;
            diff = _sliderForm.Top - screenRect.Top;
            if (diff < 0)
                _sliderForm.Top -= diff;

            //TODO: how to keep it from appearing quickly in the start position? Right now, start location is -50: ugly but it works
            _sliderForm.Visible = true;

            _sliderForm.Slider.ValueChanged += new EventHandler(Slider_ChangedValue);
            _sliderForm.CloseOnMouseUp = true;
            _sliderForm.Disposed += new EventHandler(_sliderForm_Disposed);

            Cursor.Position = _sliderForm.Slider.ValueToCenterLoc(_sliderForm.Slider.Value);
            _sliderForm.Slider.FakeMouse();
            //_sliderForm.Slider.Dragging = true;

            //form.Controls.Add(slider);
            //form.ShowDialog();

        }

        void _sliderForm_Disposed(object sender, EventArgs e)
        {
            Cursor.Position = this._ptMouseStart;
        }

        void Slider_ChangedValue(object sender, EventArgs e)
        {
            this._value = this._sliderForm.Slider.Value;
            this.SetTextBoxValue(this._value);
            //if (Math.Abs(this._uiMax) > 0.001f || Math.Abs(this._uiMin) > 0.001f)
            //    this.textBox1.Text = (this._sliderForm.Slider.ValueAs0to1 * (this._uiMax - this._uiMin) + this._uiMin).ToString();
            //else
            //    this.textBox1.Text = this._value.ToString();

            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }

        private void SetTextBoxValue(float val)
        {
            this._internalTextboxChange = true;
            if (Math.Abs(this._uiMax) > 0.001f || Math.Abs(this._uiMin) > 0.001f)
                this.textBox1.Text = (val * (this._uiMax - this._uiMin) + this._uiMin).ToString();//(value - this.UIMin) / (this.UIMax - this.UIMin);
            else
                this.textBox1.Text = val.ToString();
            this._internalTextboxChange = false;
        }

        private void NumericSlider_Resize(object sender, EventArgs e)
        {
            this.textBox1.Width = this.Width - this.button1.Width;
            this.button1.Left = this.textBox1.Right;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            float oldVal = this._value;

            float step = this._stepSize;
            if (step < 0.0001f)
                step = 1;
            if (e.Modifiers == Keys.Control)
                step *= 10;

            if (e.KeyCode == Keys.Up)
            {
                this._value += step;
                if (this._value > this._max) this._value = this._max;
            }
            else if (e.KeyCode == Keys.Down)
            {
                this._value -= step;
                if (this._value < this._min) this._value = this._min;
            }

            if (oldVal == this._value)
                return;

            this.SetTextBoxValue(this._value);
            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this._internalTextboxChange)
                return;
            float oldVal = this._value;
            try
            {
                this._value = Convert.ToSingle(this.textBox1.Text);
            }
            catch
            {
            }

            if (oldVal == this._value)
                return;

            this.SetTextBoxValue(this._value);
            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }
    }
}
