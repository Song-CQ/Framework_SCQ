using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class ColorNumeric : NumericGroup
    {
        public event EventHandler ColorChanged;
        private Endogine.ColorEx.ColorBase _color;

        public ColorNumeric()
        {
            InitializeComponent();

            this.ColorObject = new Endogine.ColorEx.ColorRgb();
        }

        void slider_ValueChanged(object sender, EventArgs e)
        {
            //((NumericSlider)sender)
            Endogine.Vector4 v = new Endogine.Vector4();
            for (int i = 0; i < this._sliders.Count; i++)
			{
                v[i] = this._sliders[i].Value;
			}
            this.ColorObject.Vector = v;

            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }

        public Endogine.ColorEx.ColorBase ColorObject
        {
            get { return this._color; }
            set
            {
                if (value == null)
                {
                    return;
                }
                Type type = value.GetType();
                if (this._color == null || type != this._color.GetType())
                {
                    int newControlsStartIndex = this.NumControls-1;
                    Endogine.ColorEx.ColorBase.AxisInfo[] infos = value.GetAxisInfo();
                    this.NumControls = infos.Length;
                    for (int i = 0; i < infos.Length; i++)
                    {
                        Endogine.ColorEx.ColorBase.AxisInfo info = infos[i];
                        NumericSlider slider = this._sliders[i];
                        slider.Min = info.Min; //0;
                        slider.Max = info.Max; //1;
                        slider.UIMin = info.UIMin; //info.Min
                        slider.UIMax = info.UIMax; //info.Max
                        if (info.UIMax > 1) //Max
                            slider.StepSize = 1f / (info.UIMax - slider.UIMin); //(info.Max - info.Min);
                        else
                            slider.StepSize = 0f;

                        this._labels[i].Text = info.Name;

                        if (i >= newControlsStartIndex)
                        {
                            slider.ValueChanged+=new EventHandler(slider_ValueChanged);
                        }
                    }
                }

                Endogine.Vector4 v = value.Vector;
                for (int i = 0; i < this.NumControls; i++)
                {
                    this._sliders[i].Value = v[i];
                }

                this._color = value;
            }
        }

        private void ColorNumeric_Resize(object sender, EventArgs e)
        {
            //this.ResizeSliders();
            //MessageBox.Show("ksk");
        }
    }
}
