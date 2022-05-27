
namespace TabularSystemTest
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLocalFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStripRightClickTreeViewNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToTheTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFromTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuTreeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripRightClickTreeViewNode.SuspendLayout();
            this.contextMenuStripFolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.treeViewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1189, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newTreeToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveCurrentFileToolStripMenuItem,
            this.saveAllToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.openToolStripMenuItem.Text = "New";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // newTreeToolStripMenuItem
            // 
            this.newTreeToolStripMenuItem.Name = "newTreeToolStripMenuItem";
            this.newTreeToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.newTreeToolStripMenuItem.Text = "New Graph";
            this.newTreeToolStripMenuItem.Click += new System.EventHandler(this.newTreeToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(222, 26);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveCurrentFileToolStripMenuItem
            // 
            this.saveCurrentFileToolStripMenuItem.Name = "saveCurrentFileToolStripMenuItem";
            this.saveCurrentFileToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.saveCurrentFileToolStripMenuItem.Text = "Save Current File As";
            this.saveCurrentFileToolStripMenuItem.Click += new System.EventHandler(this.saveCurrentFileToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.saveAllToolStripMenuItem.Text = "Save All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // treeViewToolStripMenuItem
            // 
            this.treeViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLocalFolderToolStripMenuItem,
            this.addFolderToolStripMenuItem});
            this.treeViewToolStripMenuItem.Name = "treeViewToolStripMenuItem";
            this.treeViewToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.treeViewToolStripMenuItem.Text = "TreeView";
            // 
            // createLocalFolderToolStripMenuItem
            // 
            this.createLocalFolderToolStripMenuItem.Name = "createLocalFolderToolStripMenuItem";
            this.createLocalFolderToolStripMenuItem.Size = new System.Drawing.Size(220, 26);
            this.createLocalFolderToolStripMenuItem.Text = "Create Local Folder";
            this.createLocalFolderToolStripMenuItem.Click += new System.EventHandler(this.createLocalFolderToolStripMenuItem_Click);
            // 
            // addFolderToolStripMenuItem
            // 
            this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            this.addFolderToolStripMenuItem.Size = new System.Drawing.Size(220, 26);
            this.addFolderToolStripMenuItem.Text = "Add Folder";
            this.addFolderToolStripMenuItem.Click += new System.EventHandler(this.addFolderToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openNoteToolStripMenuItem,
            this.connectNotesToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 76);
            // 
            // openNoteToolStripMenuItem
            // 
            this.openNoteToolStripMenuItem.Name = "openNoteToolStripMenuItem";
            this.openNoteToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.openNoteToolStripMenuItem.Text = "Open Note";
            this.openNoteToolStripMenuItem.Click += new System.EventHandler(this.openNoteToolStripMenuItem_Click);
            // 
            // connectNotesToolStripMenuItem
            // 
            this.connectNotesToolStripMenuItem.Name = "connectNotesToolStripMenuItem";
            this.connectNotesToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.connectNotesToolStripMenuItem.Text = "Connect Notes";
            this.connectNotesToolStripMenuItem.Click += new System.EventHandler(this.connectNotesToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // contextMenuTreeView
            // 
            this.contextMenuTreeView.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDirectoryToolStripMenuItem,
            this.addDirectoryToolStripMenuItem});
            this.contextMenuTreeView.Name = "contextMenuTreeView";
            this.contextMenuTreeView.Size = new System.Drawing.Size(174, 52);
            // 
            // newDirectoryToolStripMenuItem
            // 
            this.newDirectoryToolStripMenuItem.Name = "newDirectoryToolStripMenuItem";
            this.newDirectoryToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            this.newDirectoryToolStripMenuItem.Text = "New Directory";
            this.newDirectoryToolStripMenuItem.Click += new System.EventHandler(this.newDirectoryToolStripMenuItem_Click);
            // 
            // addDirectoryToolStripMenuItem
            // 
            this.addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
            this.addDirectoryToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            this.addDirectoryToolStripMenuItem.Text = "Add Directory";
            this.addDirectoryToolStripMenuItem.Click += new System.EventHandler(this.addDirectoryToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "file.png");
            this.imageList1.Images.SetKeyName(1, "2930366.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Size = new System.Drawing.Size(1189, 513);
            this.splitContainer1.SplitterDistance = 159;
            this.splitContainer1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(159, 513);
            this.treeView1.TabIndex = 3;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // contextMenuStripRightClickTreeViewNode
            // 
            this.contextMenuStripRightClickTreeViewNode.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripRightClickTreeViewNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToTheTreeToolStripMenuItem});
            this.contextMenuStripRightClickTreeViewNode.Name = "contextMenuStripRightClickTreeViewNode";
            this.contextMenuStripRightClickTreeViewNode.Size = new System.Drawing.Size(211, 56);
            // 
            // addToTheTreeToolStripMenuItem
            // 
            this.addToTheTreeToolStripMenuItem.Name = "addToTheTreeToolStripMenuItem";
            this.addToTheTreeToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.addToTheTreeToolStripMenuItem.Text = "Add to the graph";
            this.addToTheTreeToolStripMenuItem.Click += new System.EventHandler(this.addToTheTreeToolStripMenuItem_Click);
            // 
            // contextMenuStripFolder
            // 
            this.contextMenuStripFolder.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFromTreeToolStripMenuItem});
            this.contextMenuStripFolder.Name = "contextMenuStripFolder";
            this.contextMenuStripFolder.Size = new System.Drawing.Size(201, 28);
            this.contextMenuStripFolder.Text = "FolderSetting";
            // 
            // removeFromTreeToolStripMenuItem
            // 
            this.removeFromTreeToolStripMenuItem.Name = "removeFromTreeToolStripMenuItem";
            this.removeFromTreeToolStripMenuItem.Size = new System.Drawing.Size(200, 24);
            this.removeFromTreeToolStripMenuItem.Text = "Remove from Tree";
            this.removeFromTreeToolStripMenuItem.Click += new System.EventHandler(this.removeFromTreeToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1189, 541);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "OpenFlow";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuTreeView.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripRightClickTreeViewNode.ResumeLayout(false);
            this.contextMenuStripFolder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveCurrentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem newTreeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectNotesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuTreeView;
        private System.Windows.Forms.ToolStripMenuItem newDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDirectoryToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRightClickTreeViewNode;
        private System.Windows.Forms.ToolStripMenuItem addToTheTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem treeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createLocalFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFolder;
        private System.Windows.Forms.ToolStripMenuItem removeFromTreeToolStripMenuItem;
    }
}

