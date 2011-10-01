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
            this.txtList = new System.Windows.Forms.TextBox();
            this.btnOpenGo = new System.Windows.Forms.Button();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblAlgo = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.lblOffset = new System.Windows.Forms.Label();
            this.txtBlocks = new System.Windows.Forms.TextBox();
            this.lblBlocks = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtList
            // 
            this.txtList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtList.HideSelection = false;
            this.txtList.Location = new System.Drawing.Point(10, 43);
            this.txtList.Multiline = true;
            this.txtList.Name = "txtList";
            this.txtList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList.Size = new System.Drawing.Size(721, 300);
            this.txtList.TabIndex = 0;
            // 
            // btnOpenGo
            // 
            this.btnOpenGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenGo.Location = new System.Drawing.Point(602, 12);
            this.btnOpenGo.Name = "btnOpenGo";
            this.btnOpenGo.Size = new System.Drawing.Size(130, 24);
            this.btnOpenGo.TabIndex = 1;
            this.btnOpenGo.Text = "Open && Go";
            this.btnOpenGo.UseVisualStyleBackColor = true;
            this.btnOpenGo.Click += new System.EventHandler(this.btnOpenGo_Click);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(149, 18);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(58, 13);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "Block size:";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(213, 15);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(65, 20);
            this.txtSize.TabIndex = 3;
            this.txtSize.Text = "512";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(10, 349);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(720, 17);
            this.progressBar1.TabIndex = 4;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "SHA-2, 256bit",
            "SHA-1",
            "CRC16 MODBUS"});
            this.comboBox1.Location = new System.Drawing.Point(343, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(134, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.Text = "SHA-2, 256bit";
            // 
            // lblAlgo
            // 
            this.lblAlgo.AutoSize = true;
            this.lblAlgo.Location = new System.Drawing.Point(284, 18);
            this.lblAlgo.Name = "lblAlgo";
            this.lblAlgo.Size = new System.Drawing.Size(53, 13);
            this.lblAlgo.TabIndex = 6;
            this.lblAlgo.Text = "Algorithm:";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(527, 14);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(69, 20);
            this.txtOffset.TabIndex = 7;
            this.txtOffset.Text = "0";
            this.txtOffset.TextChanged += new System.EventHandler(this.txtOffset_TextChanged);
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(483, 17);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(38, 13);
            this.lblOffset.TabIndex = 8;
            this.lblOffset.Text = "Offset:";
            // 
            // txtBlocks
            // 
            this.txtBlocks.Location = new System.Drawing.Point(86, 15);
            this.txtBlocks.Name = "txtBlocks";
            this.txtBlocks.Size = new System.Drawing.Size(57, 20);
            this.txtBlocks.TabIndex = 9;
            this.txtBlocks.Text = "-1";
            // 
            // lblBlocks
            // 
            this.lblBlocks.AutoSize = true;
            this.lblBlocks.Location = new System.Drawing.Point(12, 18);
            this.lblBlocks.Name = "lblBlocks";
            this.lblBlocks.Size = new System.Drawing.Size(64, 13);
            this.lblBlocks.TabIndex = 10;
            this.lblBlocks.Text = "# of Blocks:";
            // 
            // frmHashTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 373);
            this.Controls.Add(this.lblBlocks);
            this.Controls.Add(this.txtBlocks);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.lblAlgo);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.btnOpenGo);
            this.Controls.Add(this.txtList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmHashTool";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "HashTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.Button btnOpenGo;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblAlgo;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.TextBox txtBlocks;
        private System.Windows.Forms.Label lblBlocks;
    }
}