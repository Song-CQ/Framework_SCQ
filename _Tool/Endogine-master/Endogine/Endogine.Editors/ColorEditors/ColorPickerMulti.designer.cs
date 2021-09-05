namespace Endogine.Editors.ColorEditors
{
    partial class ColorPickerMulti
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerMulti));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.colorSlider1 = new Endogine.Editors.ColorSlider();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 256);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("colorSlider1.BackgroundImage")));
            this.colorSlider1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.colorSlider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorSlider1.Dragging = false;
            this.colorSlider1.Location = new System.Drawing.Point(262, 0);
            this.colorSlider1.MajorTicks = 10;
            this.colorSlider1.Max = 1F;
            this.colorSlider1.Min = 0F;
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Size = new System.Drawing.Size(46, 256);
            this.colorSlider1.StepSize = 0F;
            this.colorSlider1.TabIndex = 4;
            this.colorSlider1.Value = 0.5F;
            this.colorSlider1.ValueAs0to1 = 0.5F;
            this.colorSlider1.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
            // 
            // ColorPickerMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.colorSlider1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ColorPickerMulti";
            this.Size = new System.Drawing.Size(312, 258);
            this.Resize += new System.EventHandler(this.ColorPickerMulti_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private ColorSlider colorSlider1;
    }
}
