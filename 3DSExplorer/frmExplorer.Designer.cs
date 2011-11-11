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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Save Flash", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Image", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("DIFI", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("IVFC", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("DPFS", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Hash", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("SAVE", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Folders", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("TMD", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("NCSD", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("NCCH", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup13 = new System.Windows.Forms.ListViewGroup("Plain Regios", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup14 = new System.Windows.Forms.ListViewGroup("Unknown", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup15 = new System.Windows.Forms.ListViewGroup("CIA", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup16 = new System.Windows.Forms.ListViewGroup("CIA Offsets", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup17 = new System.Windows.Forms.ListViewGroup("Certificate", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExplorer));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lstInfo = new System.Windows.Forms.ListView();
            this.chOffset = new System.Windows.Forms.ColumnHeader();
            this.chSize = new System.Windows.Forms.ColumnHeader();
            this.chDescription = new System.Windows.Forms.ColumnHeader();
            this.chValue = new System.Windows.Forms.ColumnHeader();
            this.chHexValue = new System.Windows.Forms.ColumnHeader();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.lblCaptionTree = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.lblCaptionFiles = new System.Windows.Forms.Label();
            this.cxtFile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cxtFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.cxtFileReplaceWith = new System.Windows.Forms.ToolStripMenuItem();
            this.imlFS = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep0 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileSaveImageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveKeyFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsXORTool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsHashTool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpVisit3DBrew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpSep0 = new System.Windows.Forms.ToolStripSeparator();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.lvFileTree = new TreeListView.TreeListViewControl();
            this.cName = new System.Windows.Forms.ColumnHeader();
            this.cSize = new System.Windows.Forms.ColumnHeader();
            this.cOffset = new System.Windows.Forms.ColumnHeader();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.splitContainerLeft.Panel1.SuspendLayout();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            this.cxtFile.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstInfo
            // 
            this.lstInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chOffset,
            this.chSize,
            this.chDescription,
            this.chValue,
            this.chHexValue});
            this.lstInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstInfo.FullRowSelect = true;
            listViewGroup1.Header = "Save Flash";
            listViewGroup1.Name = "lvgSaveFlash";
            listViewGroup2.Header = "Image";
            listViewGroup2.Name = "lvgImage";
            listViewGroup3.Header = "DIFI";
            listViewGroup3.Name = "lvgDifi";
            listViewGroup4.Header = "IVFC";
            listViewGroup4.Name = "lvgIvfc";
            listViewGroup5.Header = "DPFS";
            listViewGroup5.Name = "lvgDpfs";
            listViewGroup6.Header = "Hash";
            listViewGroup6.Name = "lvgHash";
            listViewGroup7.Header = "SAVE";
            listViewGroup7.Name = "lvgSave";
            listViewGroup8.Header = "Folders";
            listViewGroup8.Name = "lvgFolders";
            listViewGroup9.Header = "Files";
            listViewGroup9.Name = "lvgFiles";
            listViewGroup10.Header = "TMD";
            listViewGroup10.Name = "lvgTmd";
            listViewGroup11.Header = "NCSD";
            listViewGroup11.Name = "lvgNCSD";
            listViewGroup12.Header = "NCCH";
            listViewGroup12.Name = "lvgNCCH";
            listViewGroup13.Header = "Plain Regios";
            listViewGroup13.Name = "lvgPlainRegions";
            listViewGroup14.Header = "Unknown";
            listViewGroup14.Name = "lvgUnknown";
            listViewGroup15.Header = "CIA";
            listViewGroup15.Name = "lvgCia";
            listViewGroup16.Header = "CIA Offsets";
            listViewGroup16.Name = "lvgCiaOffsets";
            listViewGroup17.Header = "Certificate";
            listViewGroup17.Name = "lvgCertificate";
            this.lstInfo.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10,
            listViewGroup11,
            listViewGroup12,
            listViewGroup13,
            listViewGroup14,
            listViewGroup15,
            listViewGroup16,
            listViewGroup17});
            this.lstInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstInfo.HideSelection = false;
            this.lstInfo.Location = new System.Drawing.Point(0, 0);
            this.lstInfo.MultiSelect = false;
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(811, 425);
            this.lstInfo.TabIndex = 1;
            this.lstInfo.UseCompatibleStateImageBehavior = false;
            this.lstInfo.View = System.Windows.Forms.View.Details;
            this.lstInfo.DoubleClick += new System.EventHandler(this.lstInfo_DoubleClick);
            // 
            // chOffset
            // 
            this.chOffset.Text = "Offset";
            this.chOffset.Width = 48;
            // 
            // chSize
            // 
            this.chSize.Text = "Size";
            // 
            // chDescription
            // 
            this.chDescription.Text = "Description";
            this.chDescription.Width = 254;
            // 
            // chValue
            // 
            this.chValue.Text = "Decimal Value";
            this.chValue.Width = 139;
            // 
            // chHexValue
            // 
            this.chHexValue.Text = "Hex Value";
            this.chHexValue.Width = 278;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.lstInfo);
            this.splitContainer.Size = new System.Drawing.Size(1076, 425);
            this.splitContainer.SplitterDistance = 263;
            this.splitContainer.SplitterWidth = 2;
            this.splitContainer.TabIndex = 2;
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.BackgroundImage = global::_3DSExplorer.Properties.Resources.menuBack;
            this.splitContainerLeft.Panel1.Controls.Add(this.lblCaptionTree);
            this.splitContainerLeft.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.BackgroundImage = global::_3DSExplorer.Properties.Resources.menuBack;
            this.splitContainerLeft.Panel2.Controls.Add(this.lblCaptionFiles);
            this.splitContainerLeft.Panel2.Controls.Add(this.lvFileTree);
            this.splitContainerLeft.Size = new System.Drawing.Size(263, 425);
            this.splitContainerLeft.SplitterDistance = 200;
            this.splitContainerLeft.TabIndex = 2;
            // 
            // lblCaptionTree
            // 
            this.lblCaptionTree.BackColor = System.Drawing.Color.Transparent;
            this.lblCaptionTree.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblCaptionTree.Location = new System.Drawing.Point(6, 0);
            this.lblCaptionTree.Name = "lblCaptionTree";
            this.lblCaptionTree.Size = new System.Drawing.Size(255, 25);
            this.lblCaptionTree.TabIndex = 3;
            this.lblCaptionTree.Text = "(Open a file from the \'File\' Menu)";
            this.lblCaptionTree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(263, 175);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "bullet_black.png");
            // 
            // lblCaptionFiles
            // 
            this.lblCaptionFiles.BackColor = System.Drawing.Color.Transparent;
            this.lblCaptionFiles.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblCaptionFiles.Location = new System.Drawing.Point(6, 0);
            this.lblCaptionFiles.Name = "lblCaptionFiles";
            this.lblCaptionFiles.Size = new System.Drawing.Size(255, 25);
            this.lblCaptionFiles.TabIndex = 2;
            this.lblCaptionFiles.Text = "File List";
            this.lblCaptionFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cxtFile
            // 
            this.cxtFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cxtFileSaveAs,
            this.cxtFileReplaceWith});
            this.cxtFile.Name = "contextMenuStrip1";
            this.cxtFile.Size = new System.Drawing.Size(157, 48);
            this.cxtFile.MouseEnter += new System.EventHandler(this.cxtFile_MouseEnter);
            // 
            // cxtFileSaveAs
            // 
            this.cxtFileSaveAs.Image = global::_3DSExplorer.Properties.Resources.disk;
            this.cxtFileSaveAs.Name = "cxtFileSaveAs";
            this.cxtFileSaveAs.Size = new System.Drawing.Size(156, 22);
            this.cxtFileSaveAs.Text = "&Save file as...";
            this.cxtFileSaveAs.Click += new System.EventHandler(this.cxtFileSaveAs_Click);
            // 
            // cxtFileReplaceWith
            // 
            this.cxtFileReplaceWith.Image = global::_3DSExplorer.Properties.Resources.page_white_copy;
            this.cxtFileReplaceWith.Name = "cxtFileReplaceWith";
            this.cxtFileReplaceWith.Size = new System.Drawing.Size(156, 22);
            this.cxtFileReplaceWith.Text = "&Replace with...";
            this.cxtFileReplaceWith.Click += new System.EventHandler(this.cxtFileReplaceWith_Click);
            // 
            // imlFS
            // 
            this.imlFS.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlFS.ImageStream")));
            this.imlFS.TransparentColor = System.Drawing.Color.Transparent;
            this.imlFS.Images.SetKeyName(0, "page_white_text.png");
            this.imlFS.Images.SetKeyName(1, "folder.png");
            // 
            // menuStrip
            // 
            this.menuStrip.BackgroundImage = global::_3DSExplorer.Properties.Resources.menuBack;
            this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools,
            this.menuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(1076, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "Main Menu";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSep0,
            this.menuFileSaveImageFile,
            this.menuFileSaveKeyFile,
            this.menuFileSep1,
            this.menuFileExit});
            this.menuFile.Image = global::_3DSExplorer.Properties.Resources.page_white;
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(52, 20);
            this.menuFile.Text = "&File";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Image = global::_3DSExplorer.Properties.Resources.folder;
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.Size = new System.Drawing.Size(169, 22);
            this.menuFileOpen.Text = "&Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Enabled = false;
            this.menuFileSave.Image = global::_3DSExplorer.Properties.Resources.disk;
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.Size = new System.Drawing.Size(169, 22);
            this.menuFileSave.Text = "&Save...";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            // 
            // menuFileSep0
            // 
            this.menuFileSep0.Name = "menuFileSep0";
            this.menuFileSep0.Size = new System.Drawing.Size(166, 6);
            // 
            // menuFileSaveImageFile
            // 
            this.menuFileSaveImageFile.Enabled = false;
            this.menuFileSaveImageFile.Image = global::_3DSExplorer.Properties.Resources.drive_disk;
            this.menuFileSaveImageFile.Name = "menuFileSaveImageFile";
            this.menuFileSaveImageFile.Size = new System.Drawing.Size(169, 22);
            this.menuFileSaveImageFile.Text = "Save I&mage file...";
            this.menuFileSaveImageFile.Click += new System.EventHandler(this.menuFileSaveImageFile_Click);
            // 
            // menuFileSaveKeyFile
            // 
            this.menuFileSaveKeyFile.Enabled = false;
            this.menuFileSaveKeyFile.Image = global::_3DSExplorer.Properties.Resources.key1;
            this.menuFileSaveKeyFile.Name = "menuFileSaveKeyFile";
            this.menuFileSaveKeyFile.Size = new System.Drawing.Size(169, 22);
            this.menuFileSaveKeyFile.Text = "Save &key file...";
            this.menuFileSaveKeyFile.Click += new System.EventHandler(this.menuFileSaveKeyFile_Click);
            // 
            // menuFileSep1
            // 
            this.menuFileSep1.Name = "menuFileSep1";
            this.menuFileSep1.Size = new System.Drawing.Size(166, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Image = global::_3DSExplorer.Properties.Resources.door_in;
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(169, 22);
            this.menuFileExit.Text = "&Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolsXORTool,
            this.menuToolsHashTool});
            this.menuTools.Image = global::_3DSExplorer.Properties.Resources.toolbox;
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(64, 20);
            this.menuTools.Text = "&Tools";
            // 
            // menuToolsXORTool
            // 
            this.menuToolsXORTool.Image = global::_3DSExplorer.Properties.Resources.select_by_intersection;
            this.menuToolsXORTool.Name = "menuToolsXORTool";
            this.menuToolsXORTool.Size = new System.Drawing.Size(128, 22);
            this.menuToolsXORTool.Text = "&XOR Tool";
            this.menuToolsXORTool.Click += new System.EventHandler(this.menuToolsXORTool_Click);
            // 
            // menuToolsHashTool
            // 
            this.menuToolsHashTool.Image = global::_3DSExplorer.Properties.Resources.magnifier;
            this.menuToolsHashTool.Name = "menuToolsHashTool";
            this.menuToolsHashTool.Size = new System.Drawing.Size(128, 22);
            this.menuToolsHashTool.Text = "&Hash Tool";
            this.menuToolsHashTool.Click += new System.EventHandler(this.menuToolsHashTool_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpVisit3DBrew,
            this.menuHelpSep0,
            this.menuHelpAbout});
            this.menuHelp.Image = global::_3DSExplorer.Properties.Resources.help;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(59, 20);
            this.menuHelp.Text = "&Help";
            // 
            // menuHelpVisit3DBrew
            // 
            this.menuHelpVisit3DBrew.Name = "menuHelpVisit3DBrew";
            this.menuHelpVisit3DBrew.Size = new System.Drawing.Size(165, 22);
            this.menuHelpVisit3DBrew.Text = "&Visit 3DBrew.org";
            this.menuHelpVisit3DBrew.Click += new System.EventHandler(this.menuHelpVisit3DBrew_Click);
            // 
            // menuHelpSep0
            // 
            this.menuHelpSep0.Name = "menuHelpSep0";
            this.menuHelpSep0.Size = new System.Drawing.Size(162, 6);
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Enabled = false;
            this.menuHelpAbout.Image = global::_3DSExplorer.Properties.Resources.information;
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(165, 22);
            this.menuHelpAbout.Text = "by elisherer";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // lvFileTree
            // 
            this.lvFileTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFileTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvFileTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cName,
            this.cSize,
            this.cOffset});
            this.lvFileTree.ContextMenuStrip = this.cxtFile;
            this.lvFileTree.ImageList = this.imlFS;
            this.lvFileTree.Location = new System.Drawing.Point(0, 25);
            this.lvFileTree.Name = "lvFileTree";
            this.lvFileTree.Size = new System.Drawing.Size(263, 200);
            this.lvFileTree.TabIndex = 1;
            this.lvFileTree.TreeDoubleClicked += new System.EventHandler(this.lvFileTree_DoubleClick);
            // 
            // cName
            // 
            this.cName.Text = "Name";
            this.cName.Width = 130;
            // 
            // cSize
            // 
            this.cSize.Text = "Size";
            this.cSize.Width = 50;
            // 
            // cOffset
            // 
            this.cOffset.Text = "Offset";
            this.cOffset.Width = 65;
            // 
            // frmExplorer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 449);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmExplorer";
            this.Text = "3DS Explorer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmExplorer_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmExplorer_DragEnter);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.splitContainerLeft.Panel1.ResumeLayout(false);
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            this.splitContainerLeft.ResumeLayout(false);
            this.cxtFile.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListView lstInfo;
        private System.Windows.Forms.ColumnHeader chOffset;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.ImageList imlFS;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuToolsXORTool;
        private System.Windows.Forms.ToolStripMenuItem menuToolsHashTool;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator menuFileSep0;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveImageFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveKeyFile;
        private System.Windows.Forms.ToolStripSeparator menuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem menuHelpVisit3DBrew;
        private System.Windows.Forms.ToolStripSeparator menuHelpSep0;
        private System.Windows.Forms.ContextMenuStrip cxtFile;
        private System.Windows.Forms.ToolStripMenuItem cxtFileSaveAs;
        private System.Windows.Forms.ToolStripMenuItem cxtFileReplaceWith;
        private System.Windows.Forms.ColumnHeader chHexValue;
        private TreeListView.TreeListViewControl lvFileTree;
        private System.Windows.Forms.ColumnHeader cName;
        private System.Windows.Forms.ColumnHeader cSize;
        private System.Windows.Forms.ColumnHeader cOffset;
        private System.Windows.Forms.Label lblCaptionFiles;
        private System.Windows.Forms.Label lblCaptionTree;
    }
}

