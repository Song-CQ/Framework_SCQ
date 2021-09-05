namespace Endogine.Editors
{
    partial class PicRefTool
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
            this.btnCreateBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxSource = new System.Windows.Forms.ComboBox();
            this.btnCompress = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreateBrowse
            // 
            this.btnCreateBrowse.Location = new System.Drawing.Point(185, 54);
            this.btnCreateBrowse.Name = "btnCreateBrowse";
            this.btnCreateBrowse.Size = new System.Drawing.Size(24, 19);
            this.btnCreateBrowse.TabIndex = 0;
            this.btnCreateBrowse.Text = "...";
            this.btnCreateBrowse.UseVisualStyleBackColor = true;
            this.btnCreateBrowse.Click += new System.EventHandler(this.btnCreateBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Compress bitmaps to PicRef texture";
            // 
            // cbxSource
            // 
            this.cbxSource.FormattingEnabled = true;
            this.cbxSource.Location = new System.Drawing.Point(12, 54);
            this.cbxSource.Name = "cbxSource";
            this.cbxSource.Size = new System.Drawing.Size(167, 21);
            this.cbxSource.TabIndex = 2;
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(215, 54);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(31, 19);
            this.btnCompress.TabIndex = 3;
            this.btnCompress.Text = "Go";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Inspect PicRef texture";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(130, 121);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 19);
            this.button3.TabIndex = 5;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(57, 87);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(188, 20);
            this.tbOutput.TabIndex = 6;
            this.tbOutput.Text = "<default>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.helpProvider1.SetHelpString(this.label3, "<default> uses the same directory as the input files, and takes the filename exce" +
                    "pt the regex/wildcard characters to form the name");
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.helpProvider1.SetShowHelp(this.label3, true);
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Output";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Input";
            // 
            // PicRefTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 154);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.cbxSource);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreateBrowse);
            this.Controls.Add(this.label4);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PicRefTool";
            this.Text = "PicRefTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxSource;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label label4;
    }
}