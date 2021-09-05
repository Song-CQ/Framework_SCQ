namespace Endogine.Editors.ColorEditors
{
    partial class ColorPickerMultiEx
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Endogine.ColorEx.ColorHsb colorHsb1 = new Endogine.ColorEx.ColorHsb();
            Endogine.Vector4 vector41 = new Endogine.Vector4();
            Endogine.Vector4 vector42 = new Endogine.Vector4();
            this.comboColorSpace = new System.Windows.Forms.ComboBox();
            this.comboSliderAxis = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorPickerMulti1 = new Endogine.Editors.ColorEditors.ColorPickerMulti();
            this.SuspendLayout();
            // 
            // comboColorSpace
            // 
            this.comboColorSpace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboColorSpace.FormattingEnabled = true;
            this.comboColorSpace.Location = new System.Drawing.Point(90, 0);
            this.comboColorSpace.Name = "comboColorSpace";
            this.comboColorSpace.Size = new System.Drawing.Size(85, 24);
            this.comboColorSpace.TabIndex = 1;
            this.comboColorSpace.SelectedIndexChanged += new System.EventHandler(this.comboColorSpace_SelectedIndexChanged);
            // 
            // comboSliderAxis
            // 
            this.comboSliderAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSliderAxis.FormattingEnabled = true;
            this.comboSliderAxis.Location = new System.Drawing.Point(263, 0);
            this.comboSliderAxis.Name = "comboSliderAxis";
            this.comboSliderAxis.Size = new System.Drawing.Size(45, 24);
            this.comboSliderAxis.TabIndex = 2;
            this.comboSliderAxis.SelectedIndexChanged += new System.EventHandler(this.comboSliderAxis_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Color space:";
            // 
            // colorPickerMulti1
            // 
            colorHsb1.A = 255;
            colorHsb1.B = 0F;
            colorHsb1.ColorRGBA = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            colorHsb1.H = 0F;
            colorHsb1.S = 0F;
            colorHsb1.Vector = vector41;
            colorHsb1.VectorRGBA = vector42;
            this.colorPickerMulti1.ColorObject = colorHsb1;
            this.colorPickerMulti1.Location = new System.Drawing.Point(0, 30);
            this.colorPickerMulti1.Name = "colorPickerMulti1";
            this.colorPickerMulti1.Size = new System.Drawing.Size(321, 258);
            this.colorPickerMulti1.SliderRepresentsAxis = 1;
            this.colorPickerMulti1.TabIndex = 0;
            this.colorPickerMulti1.ColorChanged += new System.EventHandler(this.colorPickerMulti1_ColorChanged);
            // 
            // ColorPickerMultiEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboColorSpace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboSliderAxis);
            this.Controls.Add(this.colorPickerMulti1);
            this.Name = "ColorPickerMultiEx";
            this.Size = new System.Drawing.Size(315, 288);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorPickerMulti colorPickerMulti1;
        private System.Windows.Forms.ComboBox comboColorSpace;
        private System.Windows.Forms.ComboBox comboSliderAxis;
        private System.Windows.Forms.Label label1;
    }
}
