namespace Endogine.Editors.ColorEditors
{
    partial class ColorGradientForm
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
            System.Drawing.Drawing2D.ColorBlend colorBlend1 = new System.Drawing.Drawing2D.ColorBlend();
            Endogine.Interpolation.InterpolatorColor interpolatorColor1 = new Endogine.Interpolation.InterpolatorColor();
            System.Drawing.Drawing2D.ColorBlend colorBlend2 = new System.Drawing.Drawing2D.ColorBlend();
            this.colorGradient1 = new Endogine.Editors.ColorEditors.ColorGradient();
            this.SuspendLayout();
            // 
            // colorGradient1
            // 
            colorBlend1.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))))};
            colorBlend1.Positions = new float[] {
        0F,
        0.5F,
        1F};
            this.colorGradient1.ColorBlend = colorBlend1;
            colorBlend2.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))))};
            colorBlend2.Positions = new float[] {
        0F,
        0.5F,
        1F};
            interpolatorColor1.ColorBlend = colorBlend2;
            this.colorGradient1.InterpolatorColor = interpolatorColor1;
            this.colorGradient1.Location = new System.Drawing.Point(0, 0);
            this.colorGradient1.Margin = new System.Windows.Forms.Padding(2);
            this.colorGradient1.Name = "colorGradient1";
            this.colorGradient1.Size = new System.Drawing.Size(261, 68);
            this.colorGradient1.TabIndex = 0;
            this.colorGradient1.UseSeparateAlpha = true;
            // 
            // ColorGradientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 78);
            this.Controls.Add(this.colorGradient1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ColorGradientForm";
            this.Text = "ColorGradientForm";
            this.Resize += new System.EventHandler(this.ColorGradientForm_Resize);
            this.Load += new System.EventHandler(this.ColorGradientForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorGradient colorGradient1;
    }
}