namespace _3DSExplorer
{
    partial class frm3DVideo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm3DVideo));
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.chk3D = new System.Windows.Forms.CheckBox();
            this.cmbOrientation = new System.Windows.Forms.ComboBox();
            this.lblOrientation = new System.Windows.Forms.Label();
            this.lblYoutube = new System.Windows.Forms.Label();
            this.txtYoutube = new System.Windows.Forms.TextBox();
            this.btnSourceBrowse = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.radSourceYoutube = new System.Windows.Forms.RadioButton();
            this.radSourceFile = new System.Windows.Forms.RadioButton();
            this.grpDestination = new System.Windows.Forms.GroupBox();
            this.chkDeleteTempFiles = new System.Windows.Forms.CheckBox();
            this.lblQuality = new System.Windows.Forms.Label();
            this.tbQuality = new System.Windows.Forms.TrackBar();
            this.lblOutputFile = new System.Windows.Forms.Label();
            this.btnDestinationBrowse = new System.Windows.Forms.Button();
            this.txtQuality = new System.Windows.Forms.TextBox();
            this.txtOutputFile = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.openFfmpegDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.btnSetLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.txtFFmpeg = new System.Windows.Forms.ToolStripTextBox();
            this.picThumb = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numFps = new System.Windows.Forms.NumericUpDown();
            this.grpSource.SuspendLayout();
            this.grpDestination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbQuality)).BeginInit();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picThumb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.picThumb);
            this.grpSource.Controls.Add(this.chk3D);
            this.grpSource.Controls.Add(this.cmbOrientation);
            this.grpSource.Controls.Add(this.lblOrientation);
            this.grpSource.Controls.Add(this.lblYoutube);
            this.grpSource.Controls.Add(this.txtYoutube);
            this.grpSource.Controls.Add(this.btnSourceBrowse);
            this.grpSource.Controls.Add(this.txtSourceFile);
            this.grpSource.Controls.Add(this.radSourceYoutube);
            this.grpSource.Controls.Add(this.radSourceFile);
            this.grpSource.Location = new System.Drawing.Point(12, 31);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(582, 135);
            this.grpSource.TabIndex = 0;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source:";
            // 
            // chk3D
            // 
            this.chk3D.AutoSize = true;
            this.chk3D.Checked = true;
            this.chk3D.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk3D.Enabled = false;
            this.chk3D.Location = new System.Drawing.Point(16, 82);
            this.chk3D.Name = "chk3D";
            this.chk3D.Size = new System.Drawing.Size(40, 17);
            this.chk3D.TabIndex = 8;
            this.chk3D.Text = "3D";
            this.chk3D.UseVisualStyleBackColor = true;
            this.chk3D.CheckedChanged += new System.EventHandler(this.chk3D_CheckedChanged);
            // 
            // cmbOrientation
            // 
            this.cmbOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrientation.FormattingEnabled = true;
            this.cmbOrientation.Items.AddRange(new object[] {
            "Side-by-side (Left-Right)",
            "Side-by-side (Right-Left)",
            "Top-over-Bottom (Left-Right)",
            "Top-over-Bottom (Right-Left)"});
            this.cmbOrientation.Location = new System.Drawing.Point(103, 99);
            this.cmbOrientation.Name = "cmbOrientation";
            this.cmbOrientation.Size = new System.Drawing.Size(287, 21);
            this.cmbOrientation.TabIndex = 7;
            // 
            // lblOrientation
            // 
            this.lblOrientation.AutoSize = true;
            this.lblOrientation.Location = new System.Drawing.Point(36, 102);
            this.lblOrientation.Name = "lblOrientation";
            this.lblOrientation.Size = new System.Drawing.Size(61, 13);
            this.lblOrientation.TabIndex = 6;
            this.lblOrientation.Text = "Orientation:";
            // 
            // lblYoutube
            // 
            this.lblYoutube.AutoSize = true;
            this.lblYoutube.Location = new System.Drawing.Point(100, 47);
            this.lblYoutube.Name = "lblYoutube";
            this.lblYoutube.Size = new System.Drawing.Size(178, 13);
            this.lblYoutube.TabIndex = 5;
            this.lblYoutube.Text = "http://www.youtube.com/watch?v=";
            this.lblYoutube.Visible = false;
            // 
            // txtYoutube
            // 
            this.txtYoutube.Location = new System.Drawing.Point(276, 44);
            this.txtYoutube.Name = "txtYoutube";
            this.txtYoutube.Size = new System.Drawing.Size(114, 20);
            this.txtYoutube.TabIndex = 4;
            this.txtYoutube.Visible = false;
            this.txtYoutube.TextChanged += new System.EventHandler(this.txtYoutube_TextChanged);
            // 
            // btnSourceBrowse
            // 
            this.btnSourceBrowse.Location = new System.Drawing.Point(340, 20);
            this.btnSourceBrowse.Name = "btnSourceBrowse";
            this.btnSourceBrowse.Size = new System.Drawing.Size(50, 21);
            this.btnSourceBrowse.TabIndex = 3;
            this.btnSourceBrowse.Text = "...";
            this.btnSourceBrowse.UseVisualStyleBackColor = true;
            this.btnSourceBrowse.Click += new System.EventHandler(this.btnSourceBrowse_Click);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Location = new System.Drawing.Point(103, 21);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.ReadOnly = true;
            this.txtSourceFile.Size = new System.Drawing.Size(231, 20);
            this.txtSourceFile.TabIndex = 2;
            // 
            // radSourceYoutube
            // 
            this.radSourceYoutube.AutoSize = true;
            this.radSourceYoutube.Location = new System.Drawing.Point(16, 45);
            this.radSourceYoutube.Name = "radSourceYoutube";
            this.radSourceYoutube.Size = new System.Drawing.Size(69, 17);
            this.radSourceYoutube.TabIndex = 1;
            this.radSourceYoutube.Text = "YouTube";
            this.radSourceYoutube.UseVisualStyleBackColor = true;
            this.radSourceYoutube.CheckedChanged += new System.EventHandler(this.RadioSourceCheckedChanged);
            // 
            // radSourceFile
            // 
            this.radSourceFile.AutoSize = true;
            this.radSourceFile.Checked = true;
            this.radSourceFile.Location = new System.Drawing.Point(16, 22);
            this.radSourceFile.Name = "radSourceFile";
            this.radSourceFile.Size = new System.Drawing.Size(41, 17);
            this.radSourceFile.TabIndex = 0;
            this.radSourceFile.TabStop = true;
            this.radSourceFile.Text = "File";
            this.radSourceFile.UseVisualStyleBackColor = true;
            this.radSourceFile.CheckedChanged += new System.EventHandler(this.RadioSourceCheckedChanged);
            // 
            // grpDestination
            // 
            this.grpDestination.Controls.Add(this.numFps);
            this.grpDestination.Controls.Add(this.label1);
            this.grpDestination.Controls.Add(this.chkDeleteTempFiles);
            this.grpDestination.Controls.Add(this.lblQuality);
            this.grpDestination.Controls.Add(this.tbQuality);
            this.grpDestination.Controls.Add(this.lblOutputFile);
            this.grpDestination.Controls.Add(this.btnDestinationBrowse);
            this.grpDestination.Controls.Add(this.txtQuality);
            this.grpDestination.Controls.Add(this.txtOutputFile);
            this.grpDestination.Location = new System.Drawing.Point(12, 172);
            this.grpDestination.Name = "grpDestination";
            this.grpDestination.Size = new System.Drawing.Size(582, 131);
            this.grpDestination.TabIndex = 1;
            this.grpDestination.TabStop = false;
            this.grpDestination.Text = "Destination:";
            // 
            // chkDeleteTempFiles
            // 
            this.chkDeleteTempFiles.AutoSize = true;
            this.chkDeleteTempFiles.Location = new System.Drawing.Point(16, 100);
            this.chkDeleteTempFiles.Name = "chkDeleteTempFiles";
            this.chkDeleteTempFiles.Size = new System.Drawing.Size(206, 17);
            this.chkDeleteTempFiles.TabIndex = 9;
            this.chkDeleteTempFiles.Text = "Delete temporary files after conversion";
            this.chkDeleteTempFiles.UseVisualStyleBackColor = true;
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(13, 51);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(42, 13);
            this.lblQuality.TabIndex = 8;
            this.lblQuality.Text = "Quality:";
            // 
            // tbQuality
            // 
            this.tbQuality.AutoSize = false;
            this.tbQuality.Location = new System.Drawing.Point(134, 46);
            this.tbQuality.Maximum = 31;
            this.tbQuality.Minimum = 1;
            this.tbQuality.Name = "tbQuality";
            this.tbQuality.Size = new System.Drawing.Size(200, 29);
            this.tbQuality.TabIndex = 7;
            this.tbQuality.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbQuality.Value = 1;
            this.tbQuality.Scroll += new System.EventHandler(this.tbQuality_Scroll);
            // 
            // lblOutputFile
            // 
            this.lblOutputFile.AutoSize = true;
            this.lblOutputFile.Location = new System.Drawing.Point(13, 23);
            this.lblOutputFile.Name = "lblOutputFile";
            this.lblOutputFile.Size = new System.Drawing.Size(58, 13);
            this.lblOutputFile.TabIndex = 6;
            this.lblOutputFile.Text = "Output file:";
            // 
            // btnDestinationBrowse
            // 
            this.btnDestinationBrowse.Location = new System.Drawing.Point(340, 19);
            this.btnDestinationBrowse.Name = "btnDestinationBrowse";
            this.btnDestinationBrowse.Size = new System.Drawing.Size(50, 21);
            this.btnDestinationBrowse.TabIndex = 4;
            this.btnDestinationBrowse.Text = "...";
            this.btnDestinationBrowse.UseVisualStyleBackColor = true;
            this.btnDestinationBrowse.Click += new System.EventHandler(this.btnDestinationBrowse_Click);
            // 
            // txtQuality
            // 
            this.txtQuality.Location = new System.Drawing.Point(103, 48);
            this.txtQuality.Name = "txtQuality";
            this.txtQuality.ReadOnly = true;
            this.txtQuality.Size = new System.Drawing.Size(25, 20);
            this.txtQuality.TabIndex = 4;
            this.txtQuality.Text = "1";
            this.txtQuality.TextChanged += new System.EventHandler(this.txtYoutube_TextChanged);
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Location = new System.Drawing.Point(103, 20);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.ReadOnly = true;
            this.txtOutputFile.Size = new System.Drawing.Size(231, 20);
            this.txtOutputFile.TabIndex = 3;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Video Files (avi/flv/mpg/...)|*.avi;*.mpg;*.mpeg;*.flv;*.mp4;*.wmv";
            this.openFileDialog.Title = "Choose source file";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "AVI files (*.avi)|*.avi";
            this.saveFileDialog.Title = "Output AVI file (suggested to name it in the format (XXX_####.avi)";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 309);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(582, 20);
            this.progressBar.TabIndex = 2;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(510, 343);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(84, 24);
            this.btnGo.TabIndex = 3;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 343);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(102, 343);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(402, 24);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status:";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openFfmpegDialog
            // 
            this.openFfmpegDialog.Filter = "ffmpeg.exe|ffmpeg.exe";
            this.openFfmpegDialog.Title = "Find ffmpeg.exe";
            // 
            // menuStrip
            // 
            this.menuStrip.BackgroundImage = global::_3DSExplorer.Properties.Resources.menuBack;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSetLocation,
            this.txtFFmpeg});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(607, 26);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip";
            // 
            // btnSetLocation
            // 
            this.btnSetLocation.Name = "btnSetLocation";
            this.btnSetLocation.Size = new System.Drawing.Size(151, 22);
            this.btnSetLocation.Text = "Set ffmpeg.exe location";
            this.btnSetLocation.Click += new System.EventHandler(this.btnSetLocation_Click);
            // 
            // txtFFmpeg
            // 
            this.txtFFmpeg.Name = "txtFFmpeg";
            this.txtFFmpeg.ReadOnly = true;
            this.txtFFmpeg.Size = new System.Drawing.Size(250, 22);
            // 
            // picThumb
            // 
            this.picThumb.Location = new System.Drawing.Point(413, 19);
            this.picThumb.Name = "picThumb";
            this.picThumb.Size = new System.Drawing.Size(153, 101);
            this.picThumb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picThumb.TabIndex = 7;
            this.picThumb.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "FPS:";
            // 
            // numFps
            // 
            this.numFps.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numFps.Location = new System.Drawing.Point(103, 73);
            this.numFps.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numFps.Name = "numFps";
            this.numFps.Size = new System.Drawing.Size(59, 20);
            this.numFps.TabIndex = 11;
            this.numFps.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            // 
            // frm3DVideo
            // 
            this.AcceptButton = this.btnGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(607, 380);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.grpDestination);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "frm3DVideo";
            this.Text = "Create 3D Videos";
            this.Activated += new System.EventHandler(this.frm3DVideo_Activated);
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpDestination.ResumeLayout(false);
            this.grpDestination.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbQuality)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picThumb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.TextBox txtYoutube;
        private System.Windows.Forms.Button btnSourceBrowse;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.RadioButton radSourceYoutube;
        private System.Windows.Forms.RadioButton radSourceFile;
        private System.Windows.Forms.Label lblYoutube;
        private System.Windows.Forms.GroupBox grpDestination;
        private System.Windows.Forms.Button btnDestinationBrowse;
        private System.Windows.Forms.TextBox txtOutputFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblOutputFile;
        private System.Windows.Forms.CheckBox chk3D;
        private System.Windows.Forms.ComboBox cmbOrientation;
        private System.Windows.Forms.Label lblOrientation;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.TrackBar tbQuality;
        private System.Windows.Forms.TextBox txtQuality;
        private System.Windows.Forms.CheckBox chkDeleteTempFiles;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem btnSetLocation;
        private System.Windows.Forms.OpenFileDialog openFfmpegDialog;
        private System.Windows.Forms.ToolStripTextBox txtFFmpeg;
        private System.Windows.Forms.PictureBox picThumb;
        private System.Windows.Forms.NumericUpDown numFps;
        private System.Windows.Forms.Label label1;
    }
}