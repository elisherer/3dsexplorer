using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public partial class frmXORTool : Form
    {
        bool first = false, second = false;

        // returns the bigger array xored with the smaller cyclicly
        // dst suppose to be the size of the bigger array
        private void XorBlock(byte[] dst, byte[] first, byte[] second)
        {
            byte[] big = first, small = second;
            if (first.Length < second.Length)
            {
                big = first;
                small = second;
            }
            for (int i = 0; i < dst.Length; i++)
                dst[i] = (byte)(big[i] ^ small[i % small.Length]);
        }

        public frmXORTool()
        {
            InitializeComponent();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFirst.Text = openFileDialog.FileName;
                first = true;
                btnSave.Enabled = first && second;
            }
        }

        private void btnSecond_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtSecond.Text = openFileDialog.FileName;
                second = true;
                btnSave.Enabled = first && second;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] firstByteArray, secondByteArray;
                firstByteArray = File.ReadAllBytes(txtFirst.Text);
                secondByteArray = File.ReadAllBytes(txtSecond.Text);
                byte[] xored = new byte[Math.Max(firstByteArray.Length, secondByteArray.Length)];
                XorBlock(xored, firstByteArray, secondByteArray);
                File.WriteAllBytes(saveFileDialog.FileName, xored);
            }
        }
    }
}
