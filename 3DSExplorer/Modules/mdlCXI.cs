using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using _3DSExplorer.Utils;

namespace _3DSExplorer.Modules
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct String8
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] text;
    }

    public class CXIPlaingRegion
    {
        public string[] PlainRegionStrings;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] NCCHHeaderSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] Magic;
        public uint CXILength;
        public ulong TitleID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] MakerCode;
        public ushort Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ProgramID;
        public char TempFlag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 47)]
        public byte[] Reserved1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ProductCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ExtendedHeaderHash;
        public uint ExtendedHeaderSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved2;

        public ulong Flags;
        public uint PlainRegionOffset;
        public uint PlainRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved3;

        public uint ExeFSOffset;
        public uint ExeFSLength;
        public uint ExeFSHashRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved4;

        public uint RomFSOffset;
        public uint RomFSLength;
        public uint RomFSHashRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved5;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExeFSSuperBlockhash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] RomFSSuperBlockhash;

        public static CXIPlaingRegion getPlainRegionStringsFrom(byte[] buffer)
        {
            CXIPlaingRegion temp = new CXIPlaingRegion();
            string bigstring = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            string[] splited = bigstring.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            temp.PlainRegionStrings = splited;
            return temp;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeader
    {
        public CXIExtendedHeaderCodeSetInfo CodeSetInfo;
        public CXIExtendedHeaderDependencyList DependencyList;
        public CXIExtendedHeaderSystemInfo SystemInfo;
        public CXIExtendedHeaderArm11SystemLocalCaps Arm11SystemLocalCaps;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSetInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Name;
        public CXIExtendedHeaderSystemInfoFlags Flags;
        public CXIExtendedHeaderCodeSegmentInfo Text;
        public uint StackSize;
        public CXIExtendedHeaderCodeSegmentInfo Ro;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
        public CXIExtendedHeaderCodeSegmentInfo Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] BssSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfoFlags
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserved;
        public byte Flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] RemasterVersion;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSegmentInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Address;
        public uint NumMaxPages;
        public uint CodeSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderDependencyList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public String8[] ProgramID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfo
    {
        public uint SaveDataSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] JumpID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[] Reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderStorageInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ExtSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] SystemSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] AccessInfo;
        public byte OtherAttributes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ResourceLimit
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderArm11SystemLocalCaps
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ProgramID;
        public ulong Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public ResourceLimit[] ResourceLimitDescriptor;
        public CXIExtendedHeaderStorageInfo StorageInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public String8[] ServiceAccessControl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1f)]
        public byte[] Reserved;
        public byte ResourceLimitCategory;
    }


    public class CXIContext : IContext
    {
        public CXIHeader Header;
        private string errorMessage = string.Empty;
        public CXIPlaingRegion PlainRegion;
        public TitleInfo TitleInfo;
        //public CXIExtendedHeader ExtendedHeader;

        public long OffsetInCCI;

        public enum CXIView
        {
            NCCH,
            NCCHPlainRegion
        };

        public enum CXIActivation
        {
            RomFS,
            ExeFS,
            ReplaceRomFS,
            ReplaceExeFS
        }

        public bool Open(Stream fs)
        {
            PlainRegion = new CXIPlaingRegion();
            byte[] plainRegionBuffer;
            OffsetInCCI = fs.Position;
            Header = MarshalUtil.ReadStruct<CXIHeader>(fs);
            // get Plaing Region
            fs.Seek(OffsetInCCI + Header.PlainRegionOffset * 0x200, SeekOrigin.Begin);
            plainRegionBuffer = new byte[Header.PlainRegionSize * 0x200];
            fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
            PlainRegion = CXIHeader.getPlainRegionStringsFrom(plainRegionBuffer);
            // byte[] exhBytes = new byte[2048];
            // fs.Read(exhBytes, 0, exhBytes.Length); //TODO: read extended header
            // Array.Reverse(exh);
            TitleInfo = TitleInfo.Resolve(Header.ProductCode, Header.MakerCode);
            return true;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        public void Create(FileStream fs, FileStream src)
        {
            throw new NotImplementedException();
        }

        public void View(frmExplorer f, int view, object[] values)
        {
            //var i = values != null ? values[0] : -1;
            f.ClearInformation();
            switch ((CXIView)view)
            {
                case CXIView.NCCH:
                    f.SetGroupHeaders("Title", "Hash", "NCCH");
                    f.AddListItem(string.Empty, string.Empty, "Full Title (Name & Region)", string.Empty, TitleInfo.Title + " - " + TitleInfo.Region, 0);
                    f.AddListItem(string.Empty, string.Empty, "Title Type", string.Empty, TitleInfo.Type, 0);
                    f.AddListItem(string.Empty, string.Empty, "Developer", string.Empty, TitleInfo.Developer, 0);

                    f.AddListItem(0x000, 0x100, "RSA-2048 signature of the NCCH header [SHA-256]", Header.NCCHHeaderSignature, 1);

                    f.AddListItem(0x100, 4, "Magic (='NCCH')", Header.Magic, 2);
                    f.AddListItem(0x104, 4, "CXI length [medias]", Header.CXILength, 2);
                    f.AddListItem(0x108, 8, "Title ID", Header.TitleID, 2);
                    f.AddListItem(0x110, 2, "Maker Code", Header.MakerCode, 2);
                    f.AddListItem(0x112, 2, "Version", Header.Version, 2);
                    f.AddListItem(0x118, 8, "Program ID", Header.ProgramID, 2);
                    f.AddListItem(0x120, 1, "Temp Flag", Header.TempFlag, 2);
                    f.AddListItem(0x150, 0x10, "Product Code", Header.ProductCode, 2);
                    f.AddListItem(0x160, 0x20, "Extended Header Hash", Header.ExtendedHeaderHash, 2);
                    f.AddListItem(0x180, 4, "Extended header size", Header.ExtendedHeaderSize, 2);
                    f.AddListItem(0x188, 8, "Flags", Header.Flags, 2);
                    f.AddListItem(0x190, 4, "Plain region offset [medias]", Header.PlainRegionOffset, 2);
                    f.AddListItem(0x194, 4, "Plain region length [medias]", Header.PlainRegionSize, 2);
                    f.AddListItem(0x1A0, 4, "ExeFS offset [medias]", Header.ExeFSOffset, 2);
                    f.AddListItem(0x1A4, 4, "ExeFS length [medias]", Header.ExeFSLength, 2);
                    f.AddListItem(0x1A8, 4, "ExeFS hash region length [medias]", Header.ExeFSHashRegionSize, 2);
                    f.AddListItem(0x1B0, 4, "RomFS offset [medias]", Header.RomFSOffset, 2);
                    f.AddListItem(0x1B4, 4, "RomFS length [medias]", Header.RomFSLength, 2);
                    f.AddListItem(0x1B8, 4, "RomFS hash region length [medias]", Header.RomFSHashRegionSize, 2);
                    f.AddListItem(0x1C0, 0x20, "ExeFS superblock hash", Header.ExeFSSuperBlockhash, 2);
                    f.AddListItem(0x1E0, 0x20, "RomFS superblock hash", Header.RomFSSuperBlockhash, 2);
                    break;
                case CXIView.NCCHPlainRegion:
                    f.SetGroupHeaders("Plain Regions");
                    for (var j = 0; j < PlainRegion.PlainRegionStrings.Length; j++)
                        f.AddListItem(0, 4, PlainRegion.PlainRegionStrings[j], (ulong)PlainRegion.PlainRegionStrings[j].Length, 0);
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return false;
        }

        public void Activate(string filePath, int type, object[] values)
        {
            switch ((CXIActivation)type)
            {
                case CXIActivation.RomFS:
                case CXIActivation.ExeFS:
                    var isRom = (CXIActivation) type == CXIActivation.RomFS;
                    var saveFileDialog = new SaveFileDialog() { Filter = "Binary files (*.bin)|*.bin"};
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var strKey = InputBox.ShowDialog("Please Enter Key:\nPress OK with empty key to save encrypted");
                        if (strKey != null) //Cancel wasn't pressed
                        {
                            // returns (null if error, byte[0] on Empty, byte[16] on valid)
                            var key = StringUtil.ParseKeyStringToByteArray(strKey);

                            if (key == null)
                                MessageBox.Show(@"Error parsing key string (must be a multiple of 2 and made of hex letters).");
                            else
                            {
                                var infs = File.OpenRead(filePath);
                                infs.Seek((OffsetInCCI + (isRom ? Header.RomFSOffset :Header.ExeFSOffset)) * 0x200, SeekOrigin.Begin);
                                var buffer = new byte[(isRom ? Header.RomFSLength : Header.ExeFSLength) * 0x200];
                                infs.Read(buffer, 0, buffer.Length);
                                infs.Close();
                                if (key.Length > 0)
                                {
                                    var iv = new byte[0x10];
                                    for (var i = 0; i < 8; i++)
                                        iv[i] = 0;
                                    Buffer.BlockCopy(Header.ProgramID, 0, iv, 8, 8); //TODO: change to TitleID

                                    var aes = new Aes128Ctr(key,iv);
                                    aes.TransformBlock(buffer);
                                }
                                var outpath = saveFileDialog.FileName;
                                var outfs = File.OpenWrite(outpath);
                                outfs.Write(buffer, 0, buffer.Length);
                                outfs.Close();
                            }
                        }
                    }
                    break;
            }
        }

        public string GetFileFilter()
        {
            return "CTR Executable (*.cxi)|*.cxi";
        }

        public TreeNode GetExplorerTopNode()
        {
            var tNode = new TreeNode(string.Format("CXI ({0})", StringUtil.CharArrayToString(Header.ProductCode))) { Tag = TreeViewContextTag.Create(this, (int)CXIView.NCCH) };
            if (Header.PlainRegionSize > 0) //Add PlainRegions
                tNode.Nodes.Add("PlainRegion").Tag = TreeViewContextTag.Create(this, (int)CXIView.NCCHPlainRegion);
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var tNode = new TreeNode(string.Format("CXI ({0})",StringUtil.CharArrayToString(Header.ProductCode)), 1, 1);
                // Thanks to Ris312 for that fix!
                if (Header.ExeFSLength > 0)
                    tNode.Nodes.Add(new TreeNode(
                        TreeListView.TreeListViewControl.CreateMultiColumnNodeText(
                            "ExeFS.bin",
                            (Header.ExeFSLength * 0x200).ToString(),
                            StringUtil.ToHexString(6, (ulong)OffsetInCCI + Header.ExeFSOffset * 0x200)
                            )) 
                            { Tag = new[] {TreeViewContextTag.Create(this, (int)CXIActivation.ExeFS,"Save...")} });

                if (Header.RomFSLength > 0)
                    tNode.Nodes.Add(new TreeNode(
                            TreeListView.TreeListViewControl.CreateMultiColumnNodeText(
                                "RomFS.bin",
                                (Header.RomFSLength * 0x200).ToString(),
                                StringUtil.ToHexString(6, (ulong)OffsetInCCI + Header.RomFSOffset * 0x200)
                                )) { Tag = new[] { TreeViewContextTag.Create(this, (int)CXIActivation.RomFS, "Save...") } });
            return tNode;
        }
    }

}
