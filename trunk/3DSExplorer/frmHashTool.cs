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
                    byte[] block = new byte[blockSize];
                    byte[] hash;
                    SHA256 sha = SHA256.Create();
                    int readBytes = 0;
                    txtList.Text = "";
                    do
                    {
                        txtList.Text += "Line " + fs.Position.ToString("X7") + ": ";
                        readBytes = fs.Read(block, 0, blockSize);
                        hash = sha.ComputeHash(block);
                        txtList.Text += byteArrayToString(hash) + Environment.NewLine;

                    } while (readBytes == blockSize);
                    
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
