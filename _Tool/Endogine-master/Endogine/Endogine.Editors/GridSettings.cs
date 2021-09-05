using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public partial class GridSettings : Form, IEditorForm
    {
        public GridSettings()
        {
            InitializeComponent();
        }

        private void GridSettings_Load(object sender, EventArgs e)
        {
            //ngSpacing.GetSlider(0);
            ngSpacing.SlidersRange = new EPointF(1, 200);
            ngSpacing.SlidersStepSize = 1;
            ngSpacing.Vector = new Vector4(Endogine.Editors.BhSpriteTransformer.GridSpacing.X, Endogine.Editors.BhSpriteTransformer.GridSpacing.Y, 0, 0);

            ngOffset.SlidersRange = new EPointF(1, 200);
            ngOffset.SlidersStepSize = 1;
            ngOffset.Vector = new Vector4(Endogine.Editors.BhSpriteTransformer.GridOffset.X, Endogine.Editors.BhSpriteTransformer.GridOffset.Y, 0, 0);
        }

        private void cbActive_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbDisplayLines_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ngOffset_ValueChanged(object sender, EventArgs e)
        {
            Vector4 v = ngOffset.Vector;
            Endogine.Editors.BhSpriteTransformer.GridOffset = new EPoint(v.X, v.Y);
        }

        private void ngSpacing_ValueChanged(object sender, EventArgs e)
        {
            Vector4 v = ngSpacing.Vector;
            Endogine.Editors.BhSpriteTransformer.GridSpacing = new EPoint(v.X, v.Y);
        }
    }
}