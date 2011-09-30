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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hash Every block of size:";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(145, 15);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(102, 20);
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
            // HashTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 373);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenGo);
            this.Controls.Add(this.txtList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HashTool";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "HashTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.Button btnOpenGo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}