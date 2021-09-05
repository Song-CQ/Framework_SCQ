using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Endogine.ColorEx;
namespace Endogine.Editors.ColorEditors
{
    public partial class ColorGradient : UserControl
    {
        public event EventHandler GradientChanged;
        List<ColorGradientChip> _colorChips;
        Bitmap _bmpBg;
        Size _chipSize = new Size(10, 10);

        public ColorGradient()
        {
            InitializeComponent();

            this.DoPanelLayout();

            this.Test();
        }

        public void Test()
        {
            ColorBlend blend = new ColorBlend(3);
            blend.Positions[0] = 0;
            blend.Colors[0] = Color.FromArgb(20, 255, 0, 0);

            blend.Positions[1] = 0.5f;
            blend.Colors[1] = Color.FromArgb(130, 255, 255, 0);

            blend.Positions[2] = 1;
            blend.Colors[2] = Color.FromArgb(255, 255, 255, 255);

            this.ColorBlend = blend;
        }

        private bool _useSeparateAlphaChips = true;
        public bool UseSeparateAlpha
        {
            get { return _useSeparateAlphaChips; }
            set
            {
                _useSeparateAlphaChips = value;
                if (this._useSeparateAlphaChips)
                {
                }
            }
        }


        public ColorBlend ColorBlend
        {
            get
            {
                if (this._colorChips == null)
                    return null;

                ColorBlend blend = new ColorBlend();
                float[] positions = new float[this._colorChips.Count];
                Color[] colors = new Color[this._colorChips.Count];
                for (int i = 0; i < this._colorChips.Count; i++)
                {
                    ColorGradientChip chip = this._colorChips[i];
                    positions[i] = chip.Position;
                    colors[i] = chip.ColorObject.ColorRGBA;
                }
                blend.Positions = positions;
                blend.Colors = colors;
                return blend;
            }
            set
            {
                if (this._colorChips != null)
                {
                    foreach (ColorGradientChip chip in this._colorChips)
                        chip.Dispose();
                    this._colorChips = null;
                }

                if (value == null)
                    return;

                this._colorChips = new List<ColorGradientChip>();
                for (int i = 0; i < value.Colors.Length; i++)
                {
                    this.CreateChip(new ColorRgb(value.Colors[i]), value.Positions[i]);
                }

                this.RenderGradient();
            }
        }

        //TODO: InterpolatorColor: this old ugly construction should be abandoned...
        public Endogine.Interpolation.InterpolatorColor InterpolatorColor
        {
            get
            {
                Endogine.Interpolation.InterpolatorColor interpol = new Endogine.Interpolation.InterpolatorColor();
                interpol.ColorBlend = this.ColorBlend;
                return interpol;
            }
            set
            {
                this.ColorBlend = value.ColorBlend;
            }
        }

        public ColorGradientChip CreateChip(ColorBase color, float position)
        {
            ColorGradientChip chip = new ColorGradientChip();
            this.Controls.Add(chip);
            chip.Size = this._chipSize;
            chip.Location = new Point(30, this.panel1.Bottom);
            this._colorChips.Add(chip);
            chip.ColorObject = color;
            chip.Position = position;
            chip.Dragged += new EventHandler(chip_Dragged);
            chip.ColorChanged += new EventHandler(chip_ColorChanged);
            chip.Removed += new EventHandler(chip_Removed);
            return chip;
        }

        void chip_Removed(object sender, EventArgs e)
        {
            ColorGradientChip chip = (ColorGradientChip)sender;
            this._colorChips.Remove(chip);
            chip.Dispose();
            this.RenderGradient();
        }

        void chip_ColorChanged(object sender, EventArgs e)
        {
            this.RenderGradient();
        }

        void chip_Dragged(object sender, EventArgs e)
        {
            this.RenderGradient();
        }

        private Endogine.Interpolation.InterpolatorColor GetInterpolator()
        {
            Endogine.Interpolation.InterpolatorColor interpol = new Endogine.Interpolation.InterpolatorColor();
            float pos = 0;
            foreach (ColorGradientChip chip in this._colorChips)
            {
                if (!chip.Visible)
                    continue;
                if (chip.Position == pos)
                    pos += 0.001f;
                else
                    pos = chip.Position;
                interpol.Add(pos, chip.ColorObject.ColorRGBA);
            }
            return interpol;
        }

        public static void RenderGradient(Endogine.Interpolation.InterpolatorColor interpol, Endogine.BitmapHelpers.Canvas canvas)
        {
            canvas.Locked = true;
            for (int x = 0; x < canvas.Width; x++)
            {
                Color clr = interpol.GetValueAtTime((double)x / canvas.Width);
                canvas.SetPixel(x, 0, clr);
            }
            canvas.Locked = false;
        }
        public static void RenderGradientOnPattern(Endogine.Interpolation.InterpolatorColor interpol, Endogine.BitmapHelpers.Canvas canvas)
        {
            Graphics g = canvas.GetGraphics();
            BackgroundPattern.Fill(g);

            Bitmap bmp = new Bitmap(255, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Endogine.BitmapHelpers.Canvas c2 = Endogine.BitmapHelpers.Canvas.Create(bmp);
            RenderGradient(interpol, c2);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(bmp, new Rectangle(0, 0, canvas.Width, canvas.Height * 2));
        }

        private void RenderGradient()
        {
            Bitmap bmp = new Bitmap(255, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);

            Endogine.Interpolation.InterpolatorColor interpol = GetInterpolator();

            RenderGradient(interpol, canvas);

            if (this._bmpBg == null)
            {
                this._bmpBg = new Bitmap(this.panel1.Width, this.panel1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                this.panel1.BackgroundImage = this._bmpBg;
            }
            Graphics g = Graphics.FromImage(this._bmpBg);
            BackgroundPattern.Fill(g);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(bmp, new Rectangle(0, 0, this._bmpBg.Width, this._bmpBg.Height * 2));

            //ColorBlend blend = this.ColorBlend;
            //GraphicsPath path = new GraphicsPath();
            ////path.AddRectangle(new RectangleF(0, 0, bmp.Width, bmp.Height));
            //path.AddLine(0, 0, bmp.Width, 0);
            //PathGradientBrush pthGrBrush = new PathGradientBrush(path);
            //pthGrBrush.InterpolationColors = blend;

            //g.FillRectangle(pthGrBrush, 0, 0, bmp.Width, bmp.Height);
            this.panel1.Invalidate();

            if (this.GradientChanged != null)
                GradientChanged(this, null);
        }

        private void DoPanelLayout()
        {
            this.panel1.Left = this._chipSize.Width / 2;
            this.panel1.Width = this.Width - this.panel1.Left * 2;

            int heightToScaleChipTo = this.Height;
            if (heightToScaleChipTo > 70)
                heightToScaleChipTo = 70;
            else if (heightToScaleChipTo < 40)
                heightToScaleChipTo = 40;
            int chipsize = (int)(0.13f * (float)heightToScaleChipTo);
            this._chipSize = new Size(chipsize, chipsize);

            if (this._useSeparateAlphaChips)
                this.panel1.Top = chipsize;
            else
                this.panel1.Top = 0;

            this.panel1.Height = this.Height - this.panel1.Top - chipsize;

            if (this._colorChips != null)
            {
                foreach (ColorGradientChip chip in this._colorChips)
                {
                    chip.Position = chip.Position;
                    chip.Size = this._chipSize;
                    chip.Top = this.panel1.Bottom;
                }
            }
        }
        private void ColorGradient_Resize(object sender, EventArgs e)
        {
            this.DoPanelLayout();
        }

        private void ColorGradient_MouseDown(object sender, MouseEventArgs e)
        {
            float position = (float)(e.X - this.panel1.Left) / (this.panel1.Right - this.panel1.Left);
            if (position < 0 || position > 1)
                return;

            Endogine.Interpolation.InterpolatorColor interpol = GetInterpolator();
            Color clr = interpol.GetValueAtTime(position);

            if (e.Y > this.panel1.Bottom)
            {
                this.CreateChip(new ColorRgb(clr), position);
                this.RenderGradient();
            }
            else if (e.Y < this.panel1.Top)
            {
            }
        }
    }
}
