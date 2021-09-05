namespace Endogine.Editors
{
    partial class RegExFiles
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
            this.components = new System.ComponentModel.Container();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbRegEx = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbFiles = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.treeView1 = new Endogine.Editors.TriStateTreeView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParentFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParentsToRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(317, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 21);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(354, 244);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.Resize += new System.EventHandler(this.tabControl1_Resize);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbRegEx);
            this.tabPage1.Controls.Add(this.btnBrowse);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(346, 218);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RegEx";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbRegEx
            // 
            this.tbRegEx.Location = new System.Drawing.Point(3, 33);
            this.tbRegEx.Multiline = true;
            this.tbRegEx.Name = "tbRegEx";
            this.tbRegEx.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbRegEx.Size = new System.Drawing.Size(337, 179);
            this.tbRegEx.TabIndex = 0;
            this.tbRegEx.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(294, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter files; plain, wildcards, regex (use prefix @@), or browse";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbFiles);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(346, 218);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Files";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbFiles
            // 
            this.tbFiles.Location = new System.Drawing.Point(0, 0);
            this.tbFiles.Multiline = true;
            this.tbFiles.Name = "tbFiles";
            this.tbFiles.ReadOnly = true;
            this.tbFiles.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbFiles.Size = new System.Drawing.Size(326, 215);
            this.tbFiles.TabIndex = 0;
            this.tbFiles.WordWrap = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.treeView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(346, 218);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Treeview";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(340, 212);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(245, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(44, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(296, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(44, 23);
            this.btnLoad.TabIndex = 6;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showContentsToolStripMenuItem,
            this.addParentFolderToolStripMenuItem,
            this.addParentsToRootToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(177, 70);
            // 
            // showContentsToolStripMenuItem
            // 
            this.showContentsToolStripMenuItem.Name = "showContentsToolStripMenuItem";
            this.showContentsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.showContentsToolStripMenuItem.Text = "Show contents";
            // 
            // addParentFolderToolStripMenuItem
            // 
            this.addParentFolderToolStripMenuItem.Name = "addParentFolderToolStripMenuItem";
            this.addParentFolderToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.addParentFolderToolStripMenuItem.Text = "Show parent folder";
            // 
            // addParentsToRootToolStripMenuItem
            // 
            this.addParentsToRootToolStripMenuItem.Name = "addParentsToRootToolStripMenuItem";
            this.addParentsToRootToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.addParentsToRootToolStripMenuItem.Text = "Show parents to root";
            // 
            // RegExFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.Name = "RegExFiles";
            this.Size = new System.Drawing.Size(360, 259);
            this.Resize += new System.EventHandler(this.RegExFiles_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbRegEx;
        private System.Windows.Forms.TextBox tbFiles;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addParentFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addParentsToRootToolStripMenuItem;
        private TriStateTreeView treeView1;
    }
}
