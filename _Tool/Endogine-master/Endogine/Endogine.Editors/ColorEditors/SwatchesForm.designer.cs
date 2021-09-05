namespace Endogine.Editors.ColorEditors
{
    partial class SwatchesForm
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
            this.components = new System.ComponentModel.Container();
            Endogine.ColorEx.Palette palette1 = new Endogine.ColorEx.Palette();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwatchesForm));
            this.swatchesPanel1 = new Endogine.Editors.ColorEditors.SwatchesPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // swatchesPanel1
            // 
            this.swatchesPanel1.Location = new System.Drawing.Point(1, 1);
            this.swatchesPanel1.Name = "swatchesPanel1";
            this.swatchesPanel1.Palette = palette1;
            this.swatchesPanel1.Size = new System.Drawing.Size(60, 16);
            this.swatchesPanel1.SwatchSize = ((Endogine.EPoint)(resources.GetObject("swatchesPanel1.SwatchSize")));
            this.swatchesPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.swatchesPanel1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(213, 144);
            this.panel1.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadReplaceToolStripMenuItem,
            this.loadMergeToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveAsToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(178, 114);
            // 
            // loadReplaceToolStripMenuItem
            // 
            this.loadReplaceToolStripMenuItem.Name = "loadReplaceToolStripMenuItem";
            this.loadReplaceToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadReplaceToolStripMenuItem.Text = "Load Replace...";
            this.loadReplaceToolStripMenuItem.Click += new System.EventHandler(this.loadReplaceToolStripMenuItem_Click);
            // 
            // loadMergeToolStripMenuItem
            // 
            this.loadMergeToolStripMenuItem.Name = "loadMergeToolStripMenuItem";
            this.loadMergeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadMergeToolStripMenuItem.Text = "Load Merge...";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveAsToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            this.saveAsToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.saveAsToolStripMenuItem1.Text = "Save As...";
            // 
            // SwatchesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 173);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SwatchesForm";
            this.Text = "Color Swatches";
            this.Resize += new System.EventHandler(this.SwatchesForm_Resize);
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SwatchesPanel swatchesPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMergeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;

    }
}