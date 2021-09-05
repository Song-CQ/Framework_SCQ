namespace Endogine.Editors.ColorEditors
{
    partial class ColorGradientChip
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
            this.SuspendLayout();
            // 
            // ColorGradientChip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.Name = "ColorGradientChip";
            this.Size = new System.Drawing.Size(14, 14);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorGradientChip_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorGradientChip_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorGradientChip_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
