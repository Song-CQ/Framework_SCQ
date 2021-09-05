using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    partial class SceneGraphViewer
    {
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem miView;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem miMark;
        private System.Windows.Forms.MenuItem miDelete;
        private System.Windows.Forms.MenuItem miExport;
        private System.Windows.Forms.MenuItem miImport;
        private System.Windows.Forms.MenuItem miProperties;
        private System.Windows.Forms.MenuItem miOpenNodeInNew;
        private System.Windows.Forms.MenuItem miCenterCamera;
        private System.Windows.Forms.MenuItem miLocScaleRot;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem miExportThisAndChildren;
        private System.Windows.Forms.MenuItem miExportOnlyChildren;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem miImportReplace;
        private System.Windows.Forms.MenuItem miImportMerge;
        private System.Windows.Forms.MenuItem miImportMovie;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.miMark = new System.Windows.Forms.MenuItem();
            this.miDelete = new System.Windows.Forms.MenuItem();
            this.miProperties = new System.Windows.Forms.MenuItem();
            this.miLocScaleRot = new System.Windows.Forms.MenuItem();
            this.miCenterCamera = new System.Windows.Forms.MenuItem();
            this.miOpenNodeInNew = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miExportThisAndChildren = new System.Windows.Forms.MenuItem();
            this.miExportOnlyChildren = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.miImportReplace = new System.Windows.Forms.MenuItem();
            this.miImportMerge = new System.Windows.Forms.MenuItem();
            this.miImportMovie = new System.Windows.Forms.MenuItem();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miExport = new System.Windows.Forms.MenuItem();
            this.miImport = new System.Windows.Forms.MenuItem();
            this.miView = new System.Windows.Forms.MenuItem();
            this.miCopy = new System.Windows.Forms.MenuItem();
            this.miPaste = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.ContextMenu = this.contextMenu1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(288, 232);
            this.treeView1.TabIndex = 0;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miMark,
            this.miDelete,
            this.miProperties,
            this.miLocScaleRot,
            this.miCenterCamera,
            this.miOpenNodeInNew,
            this.menuItem2,
            this.menuItem5,
            this.miCopy,
            this.miPaste});
            // 
            // miMark
            // 
            this.miMark.Index = 0;
            this.miMark.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.miMark.Text = "Mark";
            this.miMark.Click += new System.EventHandler(this.miMark_Click);
            // 
            // miDelete
            // 
            this.miDelete.Index = 1;
            this.miDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.miDelete.Text = "Delete";
            this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
            // 
            // miProperties
            // 
            this.miProperties.Index = 2;
            this.miProperties.Shortcut = System.Windows.Forms.Shortcut.F2;
            this.miProperties.Text = "Properties";
            this.miProperties.Click += new System.EventHandler(this.miProperties_Click);
            // 
            // miLocScaleRot
            // 
            this.miLocScaleRot.Index = 3;
            this.miLocScaleRot.Text = "Loc/Scale/Rot control";
            this.miLocScaleRot.Click += new System.EventHandler(this.miLocScaleRot_Click);
            // 
            // miCenterCamera
            // 
            this.miCenterCamera.Index = 4;
            this.miCenterCamera.Text = "Center camera on it";
            this.miCenterCamera.Click += new System.EventHandler(this.miCenterCamera_Click);
            // 
            // miOpenNodeInNew
            // 
            this.miOpenNodeInNew.Index = 5;
            this.miOpenNodeInNew.Text = "Open in new window";
            this.miOpenNodeInNew.Click += new System.EventHandler(this.miOpenNodeInNew_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 6;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miExportThisAndChildren,
            this.miExportOnlyChildren});
            this.menuItem2.Text = "Export";
            // 
            // miExportThisAndChildren
            // 
            this.miExportThisAndChildren.Index = 0;
            this.miExportThisAndChildren.Text = "This and children";
            this.miExportThisAndChildren.Click += new System.EventHandler(this.miExportThisAndChildren_Click);
            // 
            // miExportOnlyChildren
            // 
            this.miExportOnlyChildren.Index = 1;
            this.miExportOnlyChildren.Text = "Only children";
            this.miExportOnlyChildren.Click += new System.EventHandler(this.miExportOnlyChildren_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 7;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miImportReplace,
            this.miImportMerge,
            this.miImportMovie});
            this.menuItem5.Text = "Import";
            // 
            // miImportReplace
            // 
            this.miImportReplace.Index = 0;
            this.miImportReplace.Text = "Replace";
            this.miImportReplace.Click += new System.EventHandler(this.miImportReplace_Click);
            // 
            // miImportMerge
            // 
            this.miImportMerge.Index = 1;
            this.miImportMerge.Text = "Merge (N/A)";
            this.miImportMerge.Click += new System.EventHandler(this.miImportMerge_Click);
            // 
            // miImportMovie
            // 
            this.miImportMovie.Index = 2;
            this.miImportMovie.Text = "Movie";
            this.miImportMovie.Click += new System.EventHandler(this.miImportMovie_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.miView});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miExport,
            this.miImport});
            this.menuItem1.Text = "File";
            // 
            // miExport
            // 
            this.miExport.Index = 0;
            this.miExport.Text = "Export...";
            this.miExport.Click += new System.EventHandler(this.miExport_Click);
            // 
            // miImport
            // 
            this.miImport.Index = 1;
            this.miImport.Text = "Import...";
            this.miImport.Click += new System.EventHandler(this.miImport_Click);
            // 
            // miView
            // 
            this.miView.Index = 1;
            this.miView.Text = "View";
            // 
            // miCopy
            // 
            this.miCopy.Index = 8;
            this.miCopy.Text = "Copy";
            this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miPaste
            // 
            this.miPaste.Index = 9;
            this.miPaste.Text = "Paste";
            // 
            // SceneGraphViewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(288, 241);
            this.Controls.Add(this.treeView1);
            this.Menu = this.mainMenu1;
            this.Name = "SceneGraphViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SceneGraphViewer";
            this.Resize += new System.EventHandler(this.SceneGraphViewer_Resize);
            this.Activated += new System.EventHandler(this.SceneGraphViewer_Activated);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.MenuItem miCopy;
        private System.Windows.Forms.MenuItem miPaste;
        private System.ComponentModel.IContainer components;

    }
}
