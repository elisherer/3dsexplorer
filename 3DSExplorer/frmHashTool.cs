using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public partial class frmHashTool : Form
    {
        [DllImport("msvcrt.dll")]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private string filePath;

        public frmHashTool()
        {
            InitializeComponent();
            cbAlgo.SelectedIndex = 0;
        }

        private string byteArrayToString(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length ; i++)
                arraystring += array[i].ToString("X2");
            return arraystring;
        }

        private void btnOpenGo_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fs = File.OpenRead(filePath);
                
                int blockSize = Int32.Parse(txtSize.Text);
                int blocks = Int32.Parse(txtBlocks.Text);

                byte[] block = new byte[blockSize];
                byte[] hash;
                HashAlgorithm ha = null;
                switch (cbAlgo.SelectedIndex)
                {
                    case 0:
                        ha = SHA256.Create();
                        break;
                    case 1:
                        ha = SHA512.Create();
                        break;
                    case 2:
                        ha = SHA1.Create();
                        break;
                    case 3:
                        ha = MD5.Create();
                        break;
                    case 4:
                        //stays null for Modbus-CRC16
                        break;
                }
                
                
                progressBar.Maximum = (blocks > 0 ? blocks : (int)fs.Length / blockSize);
                progressBar.Value = 0;
                StringBuilder sb = new StringBuilder();
                fs.Seek(Int32.Parse(txtOffset.Text), SeekOrigin.Begin);
                int readBytes = 0;
                long pos;
                do
                {
                    pos = fs.Position;
                    readBytes = fs.Read(block, 0, blockSize);
                    if (ha != null)
                        hash = ha.ComputeHash(block);
                    else
                        hash = CRC16.GetCRC(block);
                    sb.Append("@" + pos.ToString("X7") + ": " + byteArrayToString(hash) + Environment.NewLine);
                    blocks--;
                    progressBar.PerformStep();
                } while (readBytes == blockSize && blocks != 0);                    
                // Show results
                txtList.Text = sb.ToString();
                
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                btnHash.Enabled = true;
                btnBrute.Enabled = true;
                filePath = openFileDialog.FileName;
            }
        }

        private byte[] parseByteArray(string baString)
        {
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

        private void btnBrute_Click(object sender, EventArgs e)
        {
            byte[] key = parseByteArray(txtSearch.Text);
            if (key == null)
                MessageBox.Show("Error with search string!");
            else
            {
                try
                {
                    FileStream fs = File.OpenRead(filePath);
                    int blockSize = Int32.Parse(txtSize.Text);
                    int blocks = (int)fs.Length / blockSize;

                    byte[] block = new byte[blockSize];
                    byte[] hash;
                    HashAlgorithm ha = null;
                    switch (cbAlgo.SelectedIndex)
                    {
                        case 0:
                            ha = SHA256.Create();
                            break;
                        case 1:
                            ha = SHA512.Create();
                            break;
                        case 2:
                            ha = SHA1.Create();
                            break;
                        case 3:
                            ha = MD5.Create();
                            break;
                        case 4:
                            //stays null for Modbus-CRC16
                            break;
                    }
                    

                    progressBar.Maximum = blocks * blockSize;
                    progressBar.Value = 0;
                    StringBuilder sb = new StringBuilder();
                    long pos;
                    int readBytes, blockCount;
                    for (int i = 0; i < blockSize; i++) // Each iteration the starting offset is different
                    {
                        fs.Seek(i, SeekOrigin.Begin);
                        readBytes = 0;
                        blockCount = blocks;
                        do
                        {
                            pos = fs.Position;
                            readBytes = fs.Read(block, 0, blockSize);
                            if (ha != null)
                                hash = ha.ComputeHash(block);
                            else
                                hash = CRC16.GetCRC(block);
                            if (memcmp(key,hash,key.Length) == 0) //are equal
                                sb.Append("@" + pos.ToString("X7") + Environment.NewLine);
                            blockCount--;
                            progressBar.PerformStep();
                        } while (readBytes == blockSize && blockCount != 0);
                    }
                    // Show results
                    if (sb.Length == 0)
                        txtList.Text = "Search Key not found!";
                    else
                        txtList.Text = sb.ToString();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
