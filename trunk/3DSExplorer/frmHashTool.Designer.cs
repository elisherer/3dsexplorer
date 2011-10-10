namespace _3DSExplorer
{
    partial class frmHashTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHashTool));
            this.txtList = new System.Windows.Forms.TextBox();
            this.btnHash = new System.Windows.Forms.Button();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cbAlgo = new System.Windows.Forms.ComboBox();
            this.lblAlgo = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.lblOffset = new System.Windows.Forms.Label();
            this.txtBlocks = new System.Windows.Forms.TextBox();
            this.lblBlocks = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnBrute = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtList
            // 
            this.txtList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtList.HideSelection = false;
            this.txtList.Location = new System.Drawing.Point(10, 115);
            this.txtList.Multiline = true;
            this.txtList.Name = "txtList";
            this.txtList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList.Size = new System.Drawing.Size(601, 297);
            this.txtList.TabIndex = 0;
            // 
            // btnHash
            // 
            this.btnHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHash.Enabled = false;
            this.btnHash.Location = new System.Drawing.Point(483, 47);
            this.btnHash.Name = "btnHash";
            this.btnHash.Size = new System.Drawing.Size(130, 24);
            this.btnHash.TabIndex = 1;
            this.btnHash.Text = "Compute Hashes";
            this.btnHash.UseVisualStyleBackColor = true;
            this.btnHash.Click += new System.EventHandler(this.btnOpenGo_Click);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(285, 17);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(58, 13);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "Block size:";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(349, 14);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(65, 20);
            this.txtSize.TabIndex = 3;
            this.txtSize.Text = "512";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(10, 418);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(600, 17);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 4;
            // 
            // cbAlgo
            // 
            this.cbAlgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAlgo.FormattingEnabled = true;
            this.cbAlgo.Items.AddRange(new object[] {
            "SHA-2, 256bit",
            "SHA-2, 512bit",
            "SHA-1",
            "MD-5",
            "CRC16 MODBUS"});
            this.cbAlgo.Location = new System.Drawing.Point(479, 14);
            this.cbAlgo.Name = "cbAlgo";
            this.cbAlgo.Size = new System.Drawing.Size(134, 21);
            this.cbAlgo.TabIndex = 5;
            // 
            // lblAlgo
            // 
            this.lblAlgo.AutoSize = true;
            this.lblAlgo.Location = new System.Drawing.Point(420, 17);
            this.lblAlgo.Name = "lblAlgo";
            this.lblAlgo.Size = new System.Drawing.Size(53, 13);
            this.lblAlgo.TabIndex = 6;
            this.lblAlgo.Text = "Algorithm:";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(102, 50);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(69, 20);
            this.txtOffset.TabIndex = 7;
            this.txtOffset.Text = "0";
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(12, 53);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(84, 13);
            this.lblOffset.TabIndex = 8;
            this.lblOffset.Text = "Start from offset:";
            // 
            // txtBlocks
            // 
            this.txtBlocks.Location = new System.Drawing.Point(222, 14);
            this.txtBlocks.Name = "txtBlocks";
            this.txtBlocks.Size = new System.Drawing.Size(57, 20);
            this.txtBlocks.TabIndex = 9;
            this.txtBlocks.Text = "-1";
            // 
            // lblBlocks
            // 
            this.lblBlocks.AutoSize = true;
            this.lblBlocks.Location = new System.Drawing.Point(148, 17);
            this.lblBlocks.Name = "lblBlocks";
            this.lblBlocks.Size = new System.Drawing.Size(64, 13);
            this.lblBlocks.TabIndex = 10;
            this.lblBlocks.Text = "# of Blocks:";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 84);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(59, 13);
            this.lblSearch.TabIndex = 11;
            this.lblSearch.Text = "Search for:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(86, 81);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(390, 20);
            this.txtSearch.TabIndex = 12;
            // 
            // btnBrute
            // 
            this.btnBrute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrute.Enabled = false;
            this.btnBrute.Location = new System.Drawing.Point(483, 78);
            this.btnBrute.Name = "btnBrute";
            this.btnBrute.Size = new System.Drawing.Size(130, 24);
            this.btnBrute.TabIndex = 13;
            this.btnBrute.Text = "Brute-Search for Hash";
            this.btnBrute.UseVisualStyleBackColor = true;
            this.btnBrute.Click += new System.EventHandler(this.btnBrute_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(10, 11);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(128, 24);
            this.btnOpen.TabIndex = 14;
            this.btnOpen.Text = "&Open File...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // frmHashTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnBrute);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblBlocks);
            this.Controls.Add(this.txtBlocks);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.lblAlgo);
            this.Controls.Add(this.cbAlgo);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.btnHash);
            this.Controls.Add(this.txtList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "frmHashTool";
            this.Text = "HashTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.Button btnHash;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox cbAlgo;
        private System.Windows.Forms.Label lblAlgo;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.TextBox txtBlocks;
        private System.Windows.Forms.Label lblBlocks;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnBrute;
        private System.Windows.Forms.Button btnOpen;
    }
}