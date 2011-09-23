namespace _3DSExplorer
{
    partial class frmExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExplorer));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSep0 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSaveImage = new System.Windows.Forms.ToolStripButton();
            this.btnSaveKey = new System.Windows.Forms.ToolStripButton();
            this.btnXOR = new System.Windows.Forms.ToolStripButton();
            this.lblCopyright = new System.Windows.Forms.ToolStripLabel();
            this.lblBrew = new System.Windows.Forms.ToolStripLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lstInfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvFileSystem = new System.Windows.Forms.ListView();
            this.imlFS = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSep0,
            this.btnSaveImage,
            this.btnSaveKey,
            this.btnXOR,
            this.lblCopyright,
            this.lblBrew});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1076, 39);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::_3DSExplorer.Properties.Resources.ico_folder_32;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(36, 36);
            this.btnOpen.Text = "Open File...";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSep0
            // 
            this.btnSep0.Name = "btnSep0";
            this.btnSep0.Size = new System.Drawing.Size(6, 39);
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveImage.Image = global::_3DSExplorer.Properties.Resources.ico_disk_32;
            this.btnSaveImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(36, 36);
            this.btnSaveImage.Text = "Save Image file";
            this.btnSaveImage.Visible = false;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // btnSaveKey
            // 
            this.btnSaveKey.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveKey.Image = global::_3DSExplorer.Properties.Resources.key;
            this.btnSaveKey.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveKey.Name = "btnSaveKey";
            this.btnSaveKey.Size = new System.Drawing.Size(36, 36);
            this.btnSaveKey.Text = "Save key file";
            this.btnSaveKey.Visible = false;
            this.btnSaveKey.Click += new System.EventHandler(this.btnSaveKey_Click);
            // 
            // btnXOR
            // 
            this.btnXOR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnXOR.Image = global::_3DSExplorer.Properties.Resources.ico_xor;
            this.btnXOR.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnXOR.Name = "btnXOR";
            this.btnXOR.Size = new System.Drawing.Size(36, 36);
            this.btnXOR.Text = "XOR 2 Files";
            this.btnXOR.Click += new System.EventHandler(this.btnXOR_Click);
            // 
            // lblCopyright
            // 
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(309, 36);
            this.lblCopyright.Text = "All structures and algorithms based on information from";
            // 
            // lblBrew
            // 
            this.lblBrew.ActiveLinkColor = System.Drawing.Color.DarkBlue;
            this.lblBrew.IsLink = true;
            this.lblBrew.Name = "lblBrew";
            this.lblBrew.Size = new System.Drawing.Size(144, 36);
            this.lblBrew.Text = "http://www.3DBrew.org";
            this.lblBrew.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lblBrew.Click += new System.EventHandler(this.lblBrew_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All Supported (3ds,cci,bin,sav,tmd)|*.3ds;*.cci;*.bin;*.sav;*.tmd|3DS Dump Files " +
                "(*.3ds,*.cci)|*.3ds;*.cci|Save Binary Files (*.bin,*.sav)|*.bin;*.sav|Title Meta" +
                "data (*.tmd)|*.tmd|All Files|*.*";
            // 
            // lstInfo
            // 
            this.lstInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lstInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstInfo.FullRowSelect = true;
            this.lstInfo.GridLines = true;
            this.lstInfo.HideSelection = false;
            this.lstInfo.Location = new System.Drawing.Point(0, 0);
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(874, 302);
            this.lstInfo.TabIndex = 1;
            this.lstInfo.UseCompatibleStateImageBehavior = false;
            this.lstInfo.View = System.Windows.Forms.View.Details;
            this.lstInfo.DoubleClick += new System.EventHandler(this.lstInfo_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Offset";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 484;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Value";
            this.columnHeader4.Width = 269;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 39);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer.Size = new System.Drawing.Size(1076, 410);
            this.splitContainer.SplitterDistance = 198;
            this.splitContainer.TabIndex = 2;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(198, 410);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "bullet_black.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstInfo);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvFileSystem);
            this.splitContainer1.Size = new System.Drawing.Size(874, 410);
            this.splitContainer1.SplitterDistance = 302;
            this.splitContainer1.TabIndex = 2;
            // 
            // lvFileSystem
            // 
            this.lvFileSystem.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.lvFileSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFileSystem.LargeImageList = this.imlFS;
            this.lvFileSystem.Location = new System.Drawing.Point(0, 0);
            this.lvFileSystem.MultiSelect = false;
            this.lvFileSystem.Name = "lvFileSystem";
            this.lvFileSystem.Size = new System.Drawing.Size(874, 104);
            this.lvFileSystem.TabIndex = 0;
            this.lvFileSystem.UseCompatibleStateImageBehavior = false;
            this.lvFileSystem.ItemActivate += new System.EventHandler(this.lvFileSystem_ItemActivate);
            // 
            // imlFS
            // 
            this.imlFS.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlFS.ImageStream")));
            this.imlFS.TransparentColor = System.Drawing.Color.Transparent;
            this.imlFS.Images.SetKeyName(0, "page_white_text.png");
            // 
            // frmExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 449);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmExplorer";
            this.Text = "3DS Explorer";
            this.Load += new System.EventHandler(this.frmExplorer_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListView lstInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripSeparator btnSep0;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripLabel lblCopyright;
        private System.Windows.Forms.ToolStripButton btnXOR;
        private System.Windows.Forms.ToolStripLabel lblBrew;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvFileSystem;
        private System.Windows.Forms.ImageList imlFS;
        private System.Windows.Forms.ToolStripButton btnSaveKey;
        private System.Windows.Forms.ToolStripButton btnSaveImage;
    }
}

