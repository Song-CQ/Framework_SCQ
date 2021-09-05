namespace Endogine.Editors.ColorEditors
{
    partial class ColorPickerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //Endogine.ColorEx.ColorRgb colorRgb3 = new Endogine.ColorEx.ColorRgb();
            //Endogine.Vector4 vector49 = new Endogine.Vector4();
            //Endogine.Vector4 vector410 = new Endogine.Vector4();
            //Endogine.Vector4 vector411 = new Endogine.Vector4();
            //Endogine.ColorEx.ColorRgb colorRgb4 = new Endogine.ColorEx.ColorRgb();
            //Endogine.Vector4 vector412 = new Endogine.Vector4();
            //Endogine.Vector4 vector413 = new Endogine.Vector4();
            //Endogine.Vector4 vector414 = new Endogine.Vector4();
            //Endogine.ColorEx.ColorHsb colorHsb2 = new Endogine.ColorEx.ColorHsb();
            //Endogine.Vector4 vector415 = new Endogine.Vector4();
            //Endogine.Vector4 vector416 = new Endogine.Vector4();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            this.btnMore = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbColorText = new System.Windows.Forms.TextBox();
            this.numAlpha = new Endogine.Editors.NumericSlider();
            this.colorNumericHSB = new Endogine.Editors.ColorNumeric();
            this.colorNumericRGB = new Endogine.Editors.ColorNumeric();
            this.colorChip1 = new Endogine.Editors.ColorEditors.ColorChip();
            this.colorPickerPainter1 = new Endogine.Editors.ColorEditors.ColorPickerPainter();
            this.SuspendLayout();
            // 
            // btnMore
            // 
            this.btnMore.Location = new System.Drawing.Point(148, 148);
            this.btnMore.Margin = new System.Windows.Forms.Padding(2);
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(52, 19);
            this.btnMore.TabIndex = 4;
            this.btnMore.Text = "More =>";
            this.btnMore.UseVisualStyleBackColor = true;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 151);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Alpha";
            // 
            // tbColorText
            // 
            this.tbColorText.Location = new System.Drawing.Point(135, 2);
            this.tbColorText.Margin = new System.Windows.Forms.Padding(2);
            this.tbColorText.Name = "tbColorText";
            this.tbColorText.Size = new System.Drawing.Size(62, 20);
            this.tbColorText.TabIndex = 1;
            this.tbColorText.TextChanged += new System.EventHandler(this.tbColorText_TextChanged);
            this.tbColorText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbColorText_KeyDown);
            // 
            // numAlpha
            // 
            this.numAlpha.Location = new System.Drawing.Point(35, 149);
            this.numAlpha.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numAlpha.Max = 255F;
            this.numAlpha.Min = 0F;
            this.numAlpha.Name = "numAlpha";
            this.numAlpha.Size = new System.Drawing.Size(38, 18);
            this.numAlpha.StepSize = 1F;
            this.numAlpha.TabIndex = 5;
            this.numAlpha.UIMax = 0F;
            this.numAlpha.UIMin = 0F;
            this.numAlpha.Value = 0F;
            // 
            // colorNumericHSB
            // 
            //colorRgb3.A = 0;
            //colorRgb3.B = 0;
            //colorRgb3.ColorRGBA = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            //colorRgb3.G = 0;
            //colorRgb3.R = 0;
            //colorRgb3.Vector = vector49;
            //colorRgb3.VectorRGBA = vector410;
            //this.colorNumericHSB.ColorObject = colorRgb3;
            this.colorNumericHSB.Horizontal = false;
            this.colorNumericHSB.Labels = "";
            this.colorNumericHSB.Location = new System.Drawing.Point(148, 89);
            this.colorNumericHSB.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.colorNumericHSB.Name = "colorNumericHSB";
            this.colorNumericHSB.NumControls = 3;
            this.colorNumericHSB.Size = new System.Drawing.Size(50, 64);
            this.colorNumericHSB.SlidersRange = ((Endogine.EPointF)(resources.GetObject("colorNumericHSB.SlidersRange")));
            this.colorNumericHSB.SlidersStepSize = 0.003921569F;
            this.colorNumericHSB.Spacing = ((Endogine.EPoint)(resources.GetObject("colorNumericHSB.Spacing")));
            this.colorNumericHSB.TabIndex = 3;
            //this.colorNumericHSB.Vector = vector411;
            // 
            // colorNumericRGB
            // 
            //colorRgb4.A = 0;
            //colorRgb4.B = 0;
            //colorRgb4.ColorRGBA = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            //colorRgb4.G = 0;
            //colorRgb4.R = 0;
            //colorRgb4.Vector = vector412;
            //colorRgb4.VectorRGBA = vector413;
            //this.colorNumericRGB.ColorObject = colorRgb4;
            this.colorNumericRGB.Horizontal = false;
            this.colorNumericRGB.Labels = "";
            this.colorNumericRGB.Location = new System.Drawing.Point(148, 25);
            this.colorNumericRGB.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.colorNumericRGB.Name = "colorNumericRGB";
            this.colorNumericRGB.NumControls = 3;
            this.colorNumericRGB.Size = new System.Drawing.Size(50, 64);
            this.colorNumericRGB.SlidersRange = ((Endogine.EPointF)(resources.GetObject("colorNumericRGB.SlidersRange")));
            this.colorNumericRGB.SlidersStepSize = 0.003921569F;
            this.colorNumericRGB.Spacing = ((Endogine.EPoint)(resources.GetObject("colorNumericRGB.Spacing")));
            this.colorNumericRGB.TabIndex = 2;
            //this.colorNumericRGB.Vector = vector414;
            // 
            // colorChip1
            // 
            this.colorChip1.BackColor = System.Drawing.SystemColors.Control;
            this.colorChip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("colorChip1.BackgroundImage")));
            this.colorChip1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.colorChip1.Color = System.Drawing.SystemColors.Control;
            this.colorChip1.Location = new System.Drawing.Point(96, 145);
            this.colorChip1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.colorChip1.Name = "colorChip1";
            this.colorChip1.Size = new System.Drawing.Size(21, 22);
            this.colorChip1.TabIndex = 8;
            this.colorChip1.TabStop = false;
            // 
            // colorPickerPainter1
            // 
            this.colorPickerPainter1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            //this.colorPickerPainter1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            //colorHsb2.A = 0;
            //colorHsb2.B = 0F;
            //colorHsb2.ColorRGBA = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            //colorHsb2.H = 0F;
            //colorHsb2.S = 0F;
            //colorHsb2.Vector = vector415;
            //colorHsb2.VectorRGBA = vector416;
            //this.colorPickerPainter1.HSB = colorHsb2;
            this.colorPickerPainter1.Location = new System.Drawing.Point(2, 2);
            this.colorPickerPainter1.Margin = new System.Windows.Forms.Padding(2);
            this.colorPickerPainter1.Name = "colorPickerPainter1";
            this.colorPickerPainter1.Size = new System.Drawing.Size(144, 147);
            this.colorPickerPainter1.TabIndex = 0;
            this.colorPickerPainter1.TabStop = false;
            // 
            // ColorPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 172);
            this.Controls.Add(this.tbColorText);
            this.Controls.Add(this.numAlpha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMore);
            this.Controls.Add(this.colorNumericHSB);
            this.Controls.Add(this.colorNumericRGB);
            this.Controls.Add(this.colorChip1);
            this.Controls.Add(this.colorPickerPainter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ColorPickerForm";
            this.Text = "ColorPicker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorPickerPainter colorPickerPainter1;
        private ColorChip colorChip1;
        private NumericSlider numAlpha;
        private ColorNumeric colorNumericRGB;
        private ColorNumeric colorNumericHSB;
        private System.Windows.Forms.Button btnMore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbColorText;
    }
}