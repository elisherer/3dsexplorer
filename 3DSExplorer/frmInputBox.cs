using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public partial class InputBox : Form
    {
        public string Input;

        public InputBox()
        {
            InitializeComponent();
            Text = Application.ProductName;
        }

        private void setLabels(string text)
        {
            lblMessage.Text = text;
        }

        public static string ShowDialog(string MessageText)
        {
            InputBox inBox = new InputBox();
            inBox.setLabels(MessageText);
            if (inBox.ShowDialog() == DialogResult.OK)
                return inBox.Input;
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Input = txtInput.Text;
        }
    }
}
