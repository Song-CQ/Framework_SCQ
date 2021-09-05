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
    public partial class ColorGradientChip : UserControl
    {
        //public static readonly Size ChipSize = new Size(10, 10);
        public event EventHandler Dragged;
        public event EventHandler ColorChanged;
        public event EventHandler Removed;

        ColorBase _color;
        private Endogine.EPoint _mouseDrag;
        private float _draggedDistance;

        public ColorGradientChip()
        {
            InitializeComponent();

            //this.Width = ChipSize.Width;
            //this.Height = ChipSize.Height;
        }

        private float _position;

        public float Position
        {
            get { return _position; }
            set
            {
                _position = value;
                //this.Left = (int)(this._position * this.Parent.Width) - this.Width / 2;
                this.Left = (int)(this._position * (this.Parent.Width-this.Width));
            }
        }

        //public Color Color
        //{
        //    get { return this._color; }
        //    set
        //    {
        //        this._color = value;
        //        this.BackColor = Color.FromArgb(255, value.R, value.G, value.B);
        //    }
        //}
        public ColorBase ColorObject
        {
            get { return this._color; }
            set
            {
                this._color = value;
                Color clr = value.ColorRGBA;
                this.BackColor = Color.FromArgb(255, clr.R, clr.G, clr.B);
            }
        }

        private void ColorGradientChip_MouseDown(object sender, MouseEventArgs e)
        {
            this._mouseDrag = new Endogine.EPoint(this.PointToScreen(e.Location));
            this._draggedDistance = 0;
        }

        private void ColorGradientChip_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._mouseDrag != null)
            {
                Endogine.EPoint pnt = new Endogine.EPoint(this.PointToScreen(e.Location));
                Endogine.EPoint diff = pnt - this._mouseDrag;
                this._mouseDrag = pnt;
                
                //this.Location = (new Endogine.EPoint(this.Location) + diff).ToPoint();
                int left = this.Left + diff.X;
                if (left < 0) left = 0;
                else if (left > this.Parent.Width - this.Width) left = this.Parent.Width - this.Width;
                this.Left = left;
                this._draggedDistance += diff.ToEPointF().Length;

                this._position = (float)this.Left / (this.Parent.Width-this.Width);

                if (Math.Abs(e.Y) > 20)
                {
                    if (this.Visible)
                        this.Visible = false;
                }
                else if (!this.Visible)
                    this.Visible = true;

                if (this.Dragged != null)
                    this.Dragged(this, e);
            }
        }

        private void ColorGradientChip_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._mouseDrag != null)
            {
                this._mouseDrag = null;
                if (Math.Abs(e.Y) > 20)
                {
                    //TODO: remove chip
                    if (this.Removed != null)
                        this.Removed(this, null);

                }
                else if (this._draggedDistance < 2)
                {
                    ColorPickerForm form = new ColorPickerForm();
                    form.Show();
                    form.SetStartPositionRelativeLoc(null);
                    form.ColorObject = this.ColorObject;
                    form.ColorChanged += new EventHandler(form_ColorChanged);
                }
            }
        }

        void form_ColorChanged(object sender, EventArgs e)
        {
            this.ColorObject = ((ColorPickerForm)sender).ColorObject;
            if (this.ColorChanged != null)
                this.ColorChanged(this, e);
        }
    }
}

