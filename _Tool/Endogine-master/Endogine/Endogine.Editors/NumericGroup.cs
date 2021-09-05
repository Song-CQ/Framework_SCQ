using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class NumericGroup : UserControl
    {
        public event EventHandler ValueChanged;
        protected List<NumericSlider> _sliders;
        protected List<Label> _labels;
        List<string> _labelTexts;
        EPoint _spacing = new EPoint(0, 4);

        public NumericGroup()
        {
            this._sliders = new List<NumericSlider>();
            this._labels = new List<Label>();

            InitializeComponent();
        }

        public string Labels
        {
            set
            {
                string passedValue = value;
                if (passedValue == null)
                    passedValue = "";

                string[] ss = passedValue.Split(';');
                if (ss.Length == 0)
                {
                }

                if (this.NumControls < ss.Length)
                    this.NumControls = ss.Length;

                if (this._labelTexts == null)
                    this._labelTexts = new List<string>();

                int num = Math.Min(this._labels.Count, ss.Length);
                Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                for (int i = 0; i < num; i++)
                {
                    this._labels[i].Text = ss[i];
                    if (this._labelTexts.Count < i + 1)
                        this._labelTexts.Add("");
                    this._labelTexts[i] = ss[i];
                    SizeF size = g.MeasureString(ss[i], this._labels[i].Font);
                    this._labels[i].Width = (int)size.Width;
                }
                this.ResizeSliders();
            }
            get
            {
                string s = "";
                if (this._labelTexts == null || this._labelTexts.Count == 0)
                    return s;
                for (int i = 0; i < this._labelTexts.Count; i++)
                    s += this._labelTexts[i] + ";";
                return s.Remove(s.Length - 1);
            }
        }

        public NumericSlider GetSlider(int index)
        {
            return this._sliders[index];
        }

        public float SlidersStepSize
        {
            get
            {
                return this._sliders[0].StepSize;
            }
            set
            {
                foreach (NumericSlider slider in this._sliders)
                    slider.StepSize = value;
            }
        }

        public EPointF SlidersRange
        {
            get
            {
                return new EPointF(this._sliders[0].Min, this._sliders[0].Max);
            }
            set
            {
                foreach (NumericSlider slider in this._sliders)
                {
                    slider.Min = value.X;
                    slider.Max = value.Y;
                }
            }
        }

        public Endogine.Vector4 Vector
        {
            get
            {
                Endogine.Vector4 val = new Endogine.Vector4();
                float[] vals = this.Values;
                val.X = vals[0];
                val.Y = vals[1];
                if (vals.Length > 2)
                    val.Z = vals[2];
                if (vals.Length > 3)
                    val.W = vals[3];
                return val;
            }
            set
            {
                this._sliders[0].Value = value.X;
                this._sliders[1].Value = value.Y;
                if (this._sliders.Count > 2)
                    this._sliders[2].Value = value.Z;
                if (this._sliders.Count > 3)
                    this._sliders[3].Value = value.W;
            }
        }

        public float[] Values
        {
            get
            {
                float[] vals = new float[this._sliders.Count];
                for (int i = 0; i < this._sliders.Count; i++)
                    vals[i] = this._sliders[i].Value;
                return vals;
            }
        }

        public bool Horizontal
        {
            get { return this._spacing.X != 0; } //_horizontal;
            set
            {
                if (value)
                {
                    this._spacing.X = this._spacing.Y;
                    this._spacing.Y = 0;
                }
                else
                {
                    this._spacing.Y = this._spacing.X;
                    this._spacing.X = 0;
                }
                this.ResizeSliders();
                //this._horizontal = value;
            }
        }

        public EPoint Spacing
        {
            get { return _spacing; }
            set { this._spacing = value; }
        }

        public int NumControls
        {
            get { return this._sliders.Count; }
            set
            {
                if (this.NumControls == value)
                    return;

                foreach (NumericSlider slider in this._sliders)
                    slider.Dispose();
                this._sliders = new List<NumericSlider>();

                foreach (Label label in this._labels)
                    label.Dispose();
                this._labels = new List<Label>();

                this._labelTexts = new List<string>();

                EPoint pnt = new EPoint(0, 0);
                //Point pnt = new Point(0, 0);
                for (int i = 0; i < value; i++)
                {
                    NumericSlider slider = new NumericSlider();
                    this.Controls.Add(slider);
                    slider.Location = pnt.ToPoint();
                    slider.Width = 70;
                    this._sliders.Add(slider);
                    slider.ValueChanged += new EventHandler(slider_ValueChanged);
                    slider.Font = new Font("Verdana", 7);
                    slider.BringToFront();

                    Label label = new Label();
                    this.Controls.Add(label);
                    label.Location = pnt.ToPoint();
                    label.AutoSize = true;
                    label.Resize += new EventHandler(label_Resize);
                    this._labels.Add(label);
                    label.SendToBack();

                    if (this._spacing.X != 0)
                        pnt.X += slider.Width;
                    else
                        pnt.Y += slider.Height;
                    pnt += this._spacing;
                    //pnt.Y += 28;
                }
                this.ResizeSliders();
            }
        }

        void label_Resize(object sender, EventArgs e)
        {
            int rightmost = 0;
            for (int i = 0; i < this._labels.Count; i++)
            {
                rightmost = Math.Max(rightmost, this._labels[i].Right);
                this._labels[i].Top = this._sliders[i].Top;
            }
            for (int i = 0; i < this._sliders.Count; i++)
            {
                this._sliders[i].Left = rightmost;
            }
        }

        void slider_ValueChanged(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
                this.ValueChanged(this, null);
        }

        private void NumericGroup_Resize(object sender, EventArgs e)
        {
            this.ResizeSliders();
        }

        protected void ResizeSliders()
        {
            if (this._sliders.Count == 0)
                return;

            if (this.Horizontal)
            {
                //TODO: change each textbox size and move both labels and textboxes
                //get space left for textboxes:
                int totalLabelWidth = 0;
                //Graphics g = Graphics.FromImage
                for (int i = 0; i < this._sliders.Count; i++)
                    totalLabelWidth += this._labels[i].Width;
                totalLabelWidth += this._spacing.X * (this._labels.Count - 1);
                int sliderWidth = (this.Width - totalLabelWidth) / this._sliders.Count;

                EPoint pnt = new EPoint(0, 0);
                for (int i = 0; i < this._sliders.Count; i++)
                {
                    this._labels[i].Location = pnt.ToPoint();
                    pnt.X = this._labels[i].Right;
                    this._sliders[i].Location = pnt.ToPoint();
                    this._sliders[i].Width = sliderWidth;
                    pnt.X = this._sliders[i].Right;
                    pnt += this._spacing;
                }
            }
            else
            {
                for (int i = 0; i < this._sliders.Count; i++)
                    this._sliders[i].Width = this.Width - this._sliders[i].Left;
            }
        }
    }
}
