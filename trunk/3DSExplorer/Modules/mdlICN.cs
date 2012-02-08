using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using _3DSExplorer.Utils;

namespace _3DSExplorer.Modules
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ICNLocalizedDescription
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] FirstTitle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] SecondTitle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] Publisher;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ICNHeader
    {
        //SMDH - Header
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Padding0;
        // Entries
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public ICNLocalizedDescription[] Descriptions;
    }

    public class ICNContext : IContext
    {
        private string errorMessage = string.Empty;
        public long StartOffset;
        public ICNHeader Header;
        public Bitmap SmallIcon, LargeIcon;

        public enum ICNView
        {
            ICN
        };

        enum Localization
        {
            Japanese = 0,
            English,
            French,
            German,
            Italian,
            Spanish,
            Chinese,
            Korean,
            Dutch,
            Portuguese,
            Russian
        }

        public bool Open(Stream fs)
        {
            StartOffset = fs.Position;
            Header = MarshalUtil.ReadStruct<ICNHeader>(fs); //read header
            fs.Seek(StartOffset + 0x2000, SeekOrigin.Begin); //Jump to the icons
            fs.Seek(0x40, SeekOrigin.Current); //skip header
            SmallIcon = ImageUtil.ReadImageFromStream(fs, 24, 24, ImageUtil.PixelFormat.RGB565);
            LargeIcon = ImageUtil.ReadImageFromStream(fs, 48, 48, ImageUtil.PixelFormat.RGB565);
            return true;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        public void Create(FileStream fs, FileStream src)
        {
            ImageUtil.WriteImageToStream(SmallIcon, fs, ImageUtil.PixelFormat.RGB565);
            ImageUtil.WriteImageToStream(LargeIcon, fs, ImageUtil.PixelFormat.RGB565);
        }

        public void View(frmExplorer f, int view, object[] values)
        {
            f.ClearInformation();
            switch ((ICNView)view)
            {
                case ICNView.ICN:
                    f.SetGroupHeaders("ICN", "Icons");
                    string pubString, firString, secString;
                    f.AddListItem(0, 4, "Magic (='SMDH')", Header.Magic, 0);
                    f.AddListItem(4, 4, "Padding 0", Header.Padding0, 0);

                    for (var i = 0; i < Header.Descriptions.Length; i++)
                    {
                        pubString = Encoding.Unicode.GetString(Header.Descriptions[i].Publisher);
                        firString = Encoding.Unicode.GetString(Header.Descriptions[i].FirstTitle);
                        secString = Encoding.Unicode.GetString(Header.Descriptions[i].SecondTitle);
                        f.AddListItem(i.ToString(), ((Localization)i).ToString(), firString, secString, pubString, 0);
                    }

                    f.AddListItem(0, 0, "Small Icon", 24, 1);
                    f.AddListItem(0, 0, "Large Icon", 48, 1);
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return true;
        }

        public void Activate(string filePath, int type, object[] values)
        {
            switch (type)
            {
                case 0:
                    ImageBox.ShowDialog((Image)values[0]);
                    break;
                case 1:
                    var openFileDialog = new OpenFileDialog() {Filter = @"All Files|*.*"};
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var iconImage = (Image)values[0];
                        var graphics = Graphics.FromImage(iconImage);
                        try
                        {
                            var newImage = Image.FromFile(openFileDialog.FileName);
                            graphics.DrawImage(newImage, 0, 0, iconImage.Width, iconImage.Height);
                            MessageBox.Show(@"File replaced.");
                            newImage.Dispose();
                        }
                        catch
                        {
                            MessageBox.Show(@"The file selected is not a valid image!");
                        }
                    }
                    break;
            }
        }

        public string GetFileFilter()
        {
            return "CTR Icons (*.icn)|*.icn";
        }

        public TreeNode GetExplorerTopNode()
        {
            var tNode = new TreeNode("ICN") {Tag = TreeViewContextTag.Create(this, (int) ICNView.ICN)};
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var tNode = new TreeNode("ICN", 1, 1);
            tNode.Nodes.Add(new TreeNode(TreeListView.TreeListViewControl.CreateMultiColumnNodeText("Small Icon", "24x24")) 
            { Tag = new[] { TreeViewContextTag.Create(this, 0, "Show...", new object[] { SmallIcon }), TreeViewContextTag.Create(this, 1, "Replace...", new object[] { SmallIcon }) } });
            tNode.Nodes.Add(new TreeNode(TreeListView.TreeListViewControl.CreateMultiColumnNodeText("Large Icon", "48x48")) 
            { Tag = new[] { TreeViewContextTag.Create(this, 0, "Show...", new object[] { LargeIcon }), TreeViewContextTag.Create(this, 1, "Replace...", new object[] { LargeIcon }) } });
            return tNode;
        }
    }

}
