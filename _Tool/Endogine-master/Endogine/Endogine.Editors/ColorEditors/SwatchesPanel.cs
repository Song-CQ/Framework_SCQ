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
    public partial class SwatchesPanel : UserControl
    {
        Endogine.ColorEx.Palette _palette;
        //Dictionary<ColorBase, Swatch> _swatches;
        PropList _swatches;
        Swatch _selectionStartSwatch;
        Swatch _lastClickedSwatch;

        bool _dragging;

        private EPoint _swatchSize = new EPoint(16,16);

        public EPoint SwatchSize
        {
            get { return _swatchSize; }
            set { _swatchSize = value; }
        }

        public SwatchesPanel()
        {
            InitializeComponent();
        }

        public void Test()
        {
            Endogine.ColorEx.Palette pal = new Palette();
            ColorHsb hsb = new ColorHsb();
            hsb.A = 255;
            hsb.S = 0.5f;
            Point p = new Point(15, 15);
            for (int i = 0; i < p.X; i++)
            {
                hsb.B = 1f - (float)i / p.X;
                for (int j = 0; j < p.Y; j++)
                {
                    hsb.H = 359f * (float)j / p.Y;
                    pal.Add("Red", new ColorRgb(hsb.ColorRGBA));
                }
            }
            this.Palette = pal;
        }

        public Endogine.ColorEx.Palette Palette
        {
            get { return this._palette; }
            set
            {
                this._palette = value;
                this.UpdateSwatches();
            }
        }

        private void UpdateSwatches()
        {
            if (this._swatches == null)
                this._swatches = new PropList(); // Dictionary<ColorBase, Swatch>();

            int newNumColors = 0;
            if (this._palette != null)
                newNumColors = this._palette.Count;
            
            int diffNum = newNumColors - this._swatches.Count;

            this.SuspendLayout();

            if (diffNum < 0) //less color in palette than we have swatches - remove!
            {
                for (int i = 0; i < -diffNum; i++)
                    this.RemoveAtNoUpdate(this._swatches.Count - 1);
                Console.WriteLine("Less");
            }
            else if (diffNum > 0)
            {
                for (int i = 0; i < diffNum; i++)
                    this.InsertAtNoUpdate(this._swatches.Count, "", null);
                Console.WriteLine("More");
            }
            else
                Console.WriteLine("No changre");

            if (this._palette != null)
            {
                PropList newSwatches = new PropList();
                Point currentLoc = new Point();

                int i = 0;
                foreach (KeyValuePair<string, ColorBase> kv in this._palette)
                {
                    Swatch s = (Swatch)this._swatches.GetByIndex(i);
                    s.SuspendLayout();
                    s.Color = kv.Value;
                    newSwatches.Add(s.Color, s);

                    s.Size = this._swatchSize.ToSize();
                    s.Location = currentLoc;
                    s.ResumeLayout();

                    currentLoc.X += this._swatchSize.X;
                    if (currentLoc.X + this._swatchSize.X >= this.Right - 5) //TODO: why need -5?
                    {
                        currentLoc.Y += this._swatchSize.Y;
                        currentLoc.X = 0;
                    }
                    i++;
                }
                this._swatches = newSwatches;
                this.Height = currentLoc.Y + this._swatchSize.Y;
            }
            this.ResumeLayout();
            this.Invalidate();
        }

        private void InsertAtNoUpdate(int index, string name, ColorBase color)
        {
            Swatch s = new Swatch();
            s.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(s);
            if (color != null)
                s.Color = color;
            s.Name = name;
            s.MouseDown += new MouseEventHandler(s_MouseDown);
            s.MouseMove += new MouseEventHandler(s_MouseMove);
            s.MouseUp += new MouseEventHandler(s_MouseUp);
            this._swatches.Add(color, s);
        }

        void s_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
            }
        }

        void s_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._dragging)
            {
                List<Swatch> selection = this.Selection;
                foreach (Swatch s in selection)
                {
                    int index = this._swatches.IndexOfValue(s);
                    //this._swatches.RemoveAt(index);
                    //this._palette.Remove(index);
                }
            }
            this._dragging = false;
        }

        void s_MouseDown(object sender, MouseEventArgs e)
        {
            this._dragging = true;
            Swatch s = (Swatch)sender;
            if (((int)System.Windows.Forms.Form.ModifierKeys & (int)Keys.Shift) == 0)
            {
                //holding SHIFT key

                if (((int)System.Windows.Forms.Form.ModifierKeys & (int)Keys.Control) == 0)
                {
                    //Control is NOT held down, just SHIFT
                    this.DeselectAll();
                    if (this._selectionStartSwatch != null)
                    {
                        int from = this._swatches.IndexOfValue(this._selectionStartSwatch);
                        int to = this._swatches.IndexOfValue(s);
                        int diff = to - from;
                        int dir = (diff > 0) ? 1 : -1;
                        for (int i = 0; i <= diff * dir; i++)
                        {
                            ((Swatch)this._swatches.GetByIndex(from + i * dir)).Selected = true;
                        }
                    }
                    else
                    {
                        //there was no selectionstart, so just select the clicked one
                        s.Selected = !s.Selected;
                        this._selectionStartSwatch = s;
                    }
                }
                else
                {
                    //Holding SHIFT and CONTROL
                    //TODO: make it work exactly like Explorer file selection
                }
            }
            else if (System.Windows.Forms.Form.ModifierKeys == Keys.Control)
            {
                s.Selected = !s.Selected;
                this._selectionStartSwatch = s;
            }
            else
            {
                bool wasSelected = s.Selected;
                this.DeselectAll();
                if (!wasSelected)
                    s.Selected = true;
                this._selectionStartSwatch = s;
            }

            this._lastClickedSwatch = s;
        }

        public void DeselectAll()
        {
            for (int i = 0; i < this._swatches.Count; i++)
            {
                Swatch s = (Swatch)this._swatches.GetByIndex(i);
                if (s.Selected)
                    s.Selected = false;
            }
        }
        public List<Swatch> Selection
        {
            get
            {
                List<Swatch> result = new List<Swatch>();
                for (int i = 0; i < this._swatches.Count; i++)
                {
                    Swatch s = (Swatch)this._swatches.GetByIndex(i);
                    if (s.Selected)
                        result.Add(s);
                }
                return result;
            }
        }

        private void RemoveAtNoUpdate(int index)
        {
            Swatch s = (Swatch)this._swatches.GetByIndex(index);
            s.Dispose();
            this._swatches.RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            this.RemoveAtNoUpdate(index);
            this.UpdateSwatches();
        }

        private void SwatchesPanel_Resize(object sender, EventArgs e)
        {
            this.UpdateSwatches();
        }

        public void LoadPalette(string filename)
        {
            this._palette.Load(filename);
            this.Palette = this._palette;
        }
    }
}
