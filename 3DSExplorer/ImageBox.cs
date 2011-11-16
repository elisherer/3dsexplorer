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
    public partial class ImageBox : Form
    {
        public ImageBox()
        {
            InitializeComponent();
        }

        public void setImage(Image image)
        {
            pictureBox.Image = image;
        }

        public static DialogResult ShowDialog(Image image)
        {
            ImageBox imBox = new ImageBox();
            imBox.setImage(image);
            return imBox.ShowDialog();
        }
    }
}
