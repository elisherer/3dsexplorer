namespace _3DSExplorer
{
    partial class frmXORTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmXORTool));
            this.txtFirst = new System.Windows.Forms.TextBox();
            this.btnFirst = new System.Windows.Forms.Button();
            this.txtSecond = new System.Windows.Forms.TextBox();
            this.btnSecond = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFirst
            // 
            this.txtFirst.Location = new System.Drawing.Point(18, 80);
            this.txtFirst.Name = "txtFirst";
            this.txtFirst.ReadOnly = true;
            this.txtFirst.Size = new System.Drawing.Size(221, 20);
            this.txtFirst.TabIndex = 0;
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(245, 74);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(78, 26);
            this.btnFirst.TabIndex = 1;
            this.btnFirst.Text = "Browse...";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // txtSecond
            // 
            this.txtSecond.Location = new System.Drawing.Point(18, 134);
            this.txtSecond.Name = "txtSecond";
            this.txtSecond.ReadOnly = true;
            this.txtSecond.Size = new System.Drawing.Size(221, 20);
            this.txtSecond.TabIndex = 2;
            // 
            // btnSecond
            // 
            this.btnSecond.Location = new System.Drawing.Point(245, 128);
            this.btnSecond.Name = "btnSecond";
            this.btnSecond.Size = new System.Drawing.Size(78, 26);
            this.btnSecond.TabIndex = 3;
            this.btnSecond.Text = "Browse...";
            this.btnSecond.UseVisualStyleBackColor = true;
            this.btnSecond.Click += new System.EventHandler(this.btnSecond_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(161, 17);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(78, 26);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(245, 17);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 26);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "XORed files(*.xor)|*.xor";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "First File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Second File:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(227, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "The output file will be the size of the bigger file,\r\nand xored by the smaller fi" +
                "le repeatedly.";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 188);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(335, 55);
            this.panel1.TabIndex = 10;
            // 
            // frmXORTool
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(335, 243);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSecond);
            this.Controls.Add(this.txtSecond);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.txtFirst);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmXORTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XOR Tool (XOR 2 Files)";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFirst;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.TextBox txtSecond;
        private System.Windows.Forms.Button btnSecond;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;

    }
}