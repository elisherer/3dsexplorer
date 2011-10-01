using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public partial class frmHashTool : Form
    {
        public frmHashTool()
        {
            InitializeComponent();
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
                int blockSize = Int32.Parse(txtSize.Text);
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = File.OpenRead(openFileDialog.FileName);
                    fs.Seek(Int32.Parse(txtOffset.Text),SeekOrigin.Begin);
                    byte[] block = new byte[blockSize];
                    byte[] hash;
                    HashAlgorithm ha = null;
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            ha = SHA256.Create();
                            break;
                        case 1:
                            ha = SHA1.Create();
                            break;
                        case 2:
                            break;
                    }
                    int readBytes = 0;
                    txtList.Text = "";
                    int blocks = Int32.Parse(txtBlocks.Text);
                    do
                    {
                        txtList.Text += "Line " + fs.Position.ToString("X7") + ": ";
                        readBytes = fs.Read(block, 0, blockSize);
                        if (ha != null)
                            hash = ha.ComputeHash(block);
                        else
                            hash = CRC16.GetCRC(block);
                        txtList.Text += byteArrayToString(hash) + Environment.NewLine;
                        blocks--;

                    } while (readBytes == blockSize && blocks != 0);
                    
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtOffset_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
