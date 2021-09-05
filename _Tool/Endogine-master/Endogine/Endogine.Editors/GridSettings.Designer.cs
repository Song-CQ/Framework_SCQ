namespace Endogine.Editors
{
    partial class GridSettings
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
            Endogine.Vector4 vector42 = new Endogine.Vector4();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridSettings));
            Endogine.Vector4 vector41 = new Endogine.Vector4();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbDisplayLines = new System.Windows.Forms.CheckBox();
            this.cbActive = new System.Windows.Forms.CheckBox();
            this.ngOffset = new Endogine.Editors.NumericGroup();
            this.ngSpacing = new Endogine.Editors.NumericGroup();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ngSpacing);
            this.groupBox1.Location = new System.Drawing.Point(5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(123, 39);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spacing";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ngOffset);
            this.groupBox2.Location = new System.Drawing.Point(5, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(123, 41);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Offset";
            // 
            // cbDisplayLines
            // 
            this.cbDisplayLines.AutoSize = true;
            this.cbDisplayLines.Location = new System.Drawing.Point(5, 90);
            this.cbDisplayLines.Name = "cbDisplayLines";
            this.cbDisplayLines.Size = new System.Drawing.Size(104, 17);
            this.cbDisplayLines.TabIndex = 5;
            this.cbDisplayLines.Text = "Display grid lines";
            this.cbDisplayLines.UseVisualStyleBackColor = true;
            this.cbDisplayLines.CheckedChanged += new System.EventHandler(this.cbDisplayLines_CheckedChanged);
            // 
            // cbActive
            // 
            this.cbActive.AutoSize = true;
            this.cbActive.Location = new System.Drawing.Point(5, 113);
            this.cbActive.Name = "cbActive";
            this.cbActive.Size = new System.Drawing.Size(56, 17);
            this.cbActive.TabIndex = 6;
            this.cbActive.Text = "Active";
            this.cbActive.UseVisualStyleBackColor = true;
            this.cbActive.CheckedChanged += new System.EventHandler(this.cbActive_CheckedChanged);
            // 
            // ngOffset
            // 
            this.ngOffset.Horizontal = true;
            this.ngOffset.Labels = "X;Y";
            this.ngOffset.Location = new System.Drawing.Point(5, 15);
            this.ngOffset.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ngOffset.Name = "ngOffset";
            this.ngOffset.NumControls = 2;
            this.ngOffset.Size = new System.Drawing.Size(113, 21);
            this.ngOffset.Spacing = ((Endogine.EPoint)(resources.GetObject("ngOffset.Spacing")));
            this.ngOffset.TabIndex = 0;
            this.ngOffset.Vector = vector42;
            this.ngOffset.ValueChanged += new System.EventHandler(this.ngOffset_ValueChanged);
            // 
            // ngSpacing
            // 
            this.ngSpacing.Horizontal = true;
            this.ngSpacing.Labels = "X;Y";
            this.ngSpacing.Location = new System.Drawing.Point(5, 16);
            this.ngSpacing.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ngSpacing.Name = "ngSpacing";
            this.ngSpacing.NumControls = 2;
            this.ngSpacing.Size = new System.Drawing.Size(113, 21);
            this.ngSpacing.Spacing = ((Endogine.EPoint)(resources.GetObject("ngSpacing.Spacing")));
            this.ngSpacing.TabIndex = 0;
            this.ngSpacing.Vector = vector41;
            this.ngSpacing.ValueChanged += new System.EventHandler(this.ngSpacing_ValueChanged);
            // 
            // GridSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(134, 132);
            this.Controls.Add(this.cbActive);
            this.Controls.Add(this.cbDisplayLines);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GridSettings";
            this.Text = "GridSettings";
            this.Load += new System.EventHandler(this.GridSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbDisplayLines;
        private System.Windows.Forms.CheckBox cbActive;
        private NumericGroup ngSpacing;
        private NumericGroup ngOffset;
    }
}