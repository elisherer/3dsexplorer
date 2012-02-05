using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using _3DSExplorer.Utils;

namespace _3DSExplorer.Modules
{
    public enum ICNDataRegion
    {
        Japan = 0x01,
        NorthAmerica = 0x02,
        Europe = 0x0C,
        China = 0x10,
        Korea = 0x20,
        Taiwan = 0x40,
        All = 0x7FFFFFFF
    }

    [Flags]
    public enum ICNDataFlags
    {
        AllowStereoscopic3D = 0x04,
        RequireAcceptingCTREula = 0x08,
        AutoSaveOnExit = 0x10,
        UsingAnExternalBanner = 0x20,
        UsesSaveData = 0x80
    }
    
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ICNDataHeader
    {
        public ulong ZeroHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Ratings;
        public uint Region;
        public uint MatchMakerId;
        public ulong MatchMakerBitId;
        public byte Flags;
        public uint Unknown0; //24x24 color-format??
        public uint Unknown1; //48x48 color-format??
        public byte Unknown2;
        public ushort OptimalAnimationDefaultFrame;
        public uint Cec;
        public ulong ZeroTail;
    }

    public class ICNContext : IContext
    {
        private string errorMessage = string.Empty;
        public long StartOffset;
        public ICNHeader Header;
        public ICNDataHeader DataHeader;
        public Bitmap SmallIcon, LargeIcon;

        public enum ICNView
        {
            ICN,
            Data
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
            DataHeader = MarshalUtil.ReadStruct<ICNDataHeader>(fs); //read data header
            //fs.Seek(0x40, SeekOrigin.Current); //skip header
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
                case ICNView.Data:
                    f.SetGroupHeaders("ICN Data","Ratings","Flags*");
                    f.AddListItem(0, 8, "Zero Head", DataHeader.ZeroHead, 0);
                    f.AddListItem(8, 16, "Ratings*", DataHeader.Ratings, 0);
                    f.AddListItem(24, 4, "Regions", ((ICNDataRegion)DataHeader.Region).ToString().ToCharArray(), 0);
                    f.AddListItem(28, 4, "MatchMakerId", DataHeader.MatchMakerId, 0);
                    f.AddListItem(32, 8, "MatchMakerBitId", DataHeader.MatchMakerBitId, 0);
                    f.AddListItem(40, 1, "Flags*", DataHeader.Flags, 0);
                    f.AddListItem(41, 4, "Unknown0", DataHeader.Unknown0, 0);
                    f.AddListItem(45, 4, "Unknown1", DataHeader.Unknown1, 0);
                    f.AddListItem(49, 1, "Unknown2", DataHeader.Unknown2, 0);
                    f.AddListItem(50, 2, "Optimal Animation Default Frame", DataHeader.OptimalAnimationDefaultFrame, 0);
                    f.AddListItem(52, 4, "CEC", DataHeader.Cec, 0);
                    f.AddListItem(56, 8, "Zero Tail", DataHeader.ZeroTail, 0);
                    f.AddListItem(0, 1, "CERO (Japan)", (byte)(DataHeader.Ratings[0] & 0x7F), 1);
                    f.AddListItem(0, 1, "ESRB (North America)", (byte)(DataHeader.Ratings[1] & 0x7F), 1);
                    f.AddListItem(0, 1, "USK (Germany)", (byte)(DataHeader.Ratings[3] & 0x7F), 1);
                    f.AddListItem(0, 1, "PEGI (Europe)", (byte)(DataHeader.Ratings[4] & 0x7F), 1);
                    f.AddListItem(0, 1, "PEGI (Portugal)", (byte)(DataHeader.Ratings[6] & 0x7F), 1);
                    f.AddListItem(0, 1, "BBFC (England)", (byte)(DataHeader.Ratings[7] & 0x7F), 1);
                    f.AddListItem(0, 1, "COB", (byte)(DataHeader.Ratings[8] & 0x7F), 1);
                    f.AddListItem(0, 1, "Is Rated?", (byte)(DataHeader.Ratings[9] & 0x7F), 1);
                    f.AddListItem("", "", "Allow Stereoscopic 3D", (((uint)ICNDataFlags.AllowStereoscopic3D & DataHeader.Flags) > 0).ToString(), "", 2);
                    f.AddListItem("", "", "Require Accepting CTR Eula", (((uint)ICNDataFlags.RequireAcceptingCTREula & DataHeader.Flags) > 0).ToString(), "", 2);
                    f.AddListItem("", "", "Auto Save On Exit", (((uint)ICNDataFlags.AutoSaveOnExit & DataHeader.Flags) > 0).ToString(), "", 2);
                    f.AddListItem("", "", "Using An External Banner", (((uint)ICNDataFlags.UsingAnExternalBanner & DataHeader.Flags) > 0).ToString(), "", 2);
                    f.AddListItem("", "", "Uses Save Data", (((uint)ICNDataFlags.UsesSaveData & DataHeader.Flags) > 0).ToString(), "", 2);
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
            tNode.Nodes.Add(new TreeNode("Data") { Tag = TreeViewContextTag.Create(this, (int)ICNView.Data) });
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
