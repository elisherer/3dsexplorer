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

        private byte[] parseByteArray(string baString)
        {
            baString.Replace(" ", ""); //delete all spaces
            if (baString.Length % 2 != 0)
                return null;
            try
            {
                byte[] ret = new byte[(int)baString.Length / 2];
                for (int i = 0, j = 0; i < baString.Length; i += 2, j++)
                    ret[j] = Convert.ToByte(baString.Substring(i, 2), 16);
                return ret;
            }
            catch
            {
                return null;
            }
        }

        // returns the bigger array xored with the smaller cyclicly
        // dst suppose to be the size of the bigger array
        private void XorBlock(byte[] dst, byte[] first, byte[] second)
        {
            byte[] big = first, small = second;
            if (first.Length < second.Length)
            {
                big = second;
                small = first;
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string byteArrayToString(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length; i++)
                arraystring += String.Format("{0:X2}", array[i]);
            return arraystring;
        }

        private void btnXorArrays_Click(object sender, EventArgs e)
        {
            byte[] one = parseByteArray(txtBox1.Text);
            byte[] two = parseByteArray(txtBox2.Text);

            if (one != null && two != null && one.Length == two.Length)
            {
                byte[] dst = new byte[one.Length];
                for (int i = 0; i < dst.Length; i++)
                    dst[i] = (byte)(one[i] ^ two[i]);
                txtBoxResult.Text = byteArrayToString(dst);
            }
            else
                MessageBox.Show("Error with length (must be a multiple of 2 or same size)");
        }

        private void btnAesGo_Click(object sender, EventArgs e)
        {
            byte[] key = parseByteArray(txtKey.Text);
            byte[] iv = parseByteArray(txtIV.Text);
            byte[] data = parseByteArray(txtEncData.Text);

            if (key != null && iv != null && data != null && key.Length == 16 && iv.Length == 16)
            {
                Aes128Ctr aes = new Aes128Ctr(key, iv);
                aes.TransformBlock(data);
                txtDecData.Text = byteArrayToString(data);
            }
            else
                MessageBox.Show("Error with length (must be a multiple of 2 or key & iv must be 16 bytes)");
        }
    }
}
