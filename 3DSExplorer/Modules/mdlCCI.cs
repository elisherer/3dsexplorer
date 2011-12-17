using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer.Modules
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //, Size = 0x330
    public struct CCIHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] NCSDHeaderSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] Magic;
        public uint CCILength;
        public ulong MainTitleID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Reserved0;

        public uint FirstCXIOffset;
        public uint FirstCXILength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved1;

        public uint SecondCXIOffset;
        public uint SecondCXILength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved2;

        public uint ThirdCXIOffset;
        public uint ThirdCXILength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] Reserved3;

        public ulong CXIFlags;
        //CXI Flags: 
        // byte[5]-byte[7] indicate content type ( system update, application, ... )
        // byte[6] size of media units ( 512*2^byte[6] ) and encryption

        public ulong FirstCXITitleID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved4;

        public ulong SecondCXITitleID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved5;

        public ulong ThirdCXITitleID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Reserved6;

        public uint PaddingFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 252)]
        public byte[] Reserved7;
        
        public uint UsedRomSize; //in bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Reserved8;

        public uint Region;

        public uint Unknown9;
        public ulong Unknown10;
    }

    public class CCIContext : IContext
    {
        private string errorMessage = string.Empty;
        public CCIHeader Header;
        public CXIContext[] CXIContexts;

        public enum CCIView
        {
            NCSD
        };

        public bool Open(Stream fs)
        {
            Header = MarshalUtil.ReadStruct<CCIHeader>(fs);
            CXIContexts = new CXIContext[3];
            // Read the CXIs
            if (Header.FirstCXILength > 0)
            {
                CXIContexts[0] = new CXIContext();
                fs.Seek(Header.FirstCXIOffset * 0x200, SeekOrigin.Begin);
                CXIContexts[0].Open(fs);
            }
            if (Header.SecondCXILength > 0)
            {
                CXIContexts[1] = new CXIContext();
                fs.Seek(Header.SecondCXIOffset * 0x200, SeekOrigin.Begin);
                CXIContexts[1].Open(fs);
            }
            if (Header.ThirdCXILength > 0)
            {
                CXIContexts[2] = new CXIContext();
                fs.Seek(Header.ThirdCXIOffset * 0x200, SeekOrigin.Begin);
                CXIContexts[2].Open(fs);
            }
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
            //var i = values != null ? (int)values[0] : -1;
            f.ClearInformation();
            switch ((CCIView)view)
            {
                case CCIView.NCSD:
                    f.SetGroupHeaders("Hash", "NCSD");
                    f.AddListItem(0x000, 0x100, "RSA-2048 signature of the NCSD header [SHA-256]", Header.NCSDHeaderSignature, 0);
                    f.AddListItem(0x100, 4, "Magic (='NCSD')", Header.Magic, 1);
                    f.AddListItem(0x104, 4, "CCI length [medias]", Header.CCILength, 1);
                    f.AddListItem(0x108, 8, "Main Title ID", Header.MainTitleID, 1);
                    //f.AddListItem(0x110, 16, "Unknown 0", Header.Reserved0, 1);
                    f.AddListItem(0x120, 4, "1st CXI offset [medias]", Header.FirstCXIOffset, 1);
                    f.AddListItem(0x124, 4, "1st CXI length [medias]", Header.FirstCXILength, 1);
                    //f.AddListItem(0x128, 8, "Unknown 1", Header.Reserved1, 1);
                    f.AddListItem(0x130, 4, "2nd CXI offset [medias]", Header.SecondCXIOffset, 1);
                    f.AddListItem(0x134, 4, "2nd CXI length [medias]", Header.SecondCXILength, 1);
                    //f.AddListItem(0x138, 32, "Unknown 2", Header.Reserved2, 1);
                    f.AddListItem(0x158, 4, "3rd CXI offset [medias]", Header.ThirdCXIOffset, 1);
                    f.AddListItem(0x15C, 4, "3rd CXI length [medias]", Header.ThirdCXILength, 1);
                    //f.AddListItem(0x160, 40, "Unknown 3", Header.Reserved3, 1);
                    f.AddListItem(0x188, 8, "CXI Flags", Header.CXIFlags, 1);
                    f.AddListItem(0x190, 8, "1st CXI Title ID", Header.FirstCXITitleID, 1);
                    //f.AddListItem(0x198, 8, "Unknown 4", Header.Reserved4, 1);
                    f.AddListItem(0x1A0, 8, "2nd CXI Title ID", Header.SecondCXITitleID, 1);
                    //f.AddListItem(0x1A8, 32, "Unknown 5", Header.Reserved5, 1);
                    f.AddListItem(0x1C8, 8, "3rd CXI Title ID", Header.ThirdCXITitleID, 1);
                    //f.AddListItem(0x1D0, 48, "Unknown 6", Header.Reserved6, 1);
                    f.AddListItem(0x200, 4, "Always 0xFFFFFFFF", Header.PaddingFF, 1);
                    //f.AddListItem(0x204, 252, "Unknown 7", Header.Reserved7, 1);
                    f.AddListItem(0x300, 4, "Used ROM size [bytes]", Header.UsedRomSize, 1);
                    //f.AddListItem(0x204, 252, "Unknown 8", Header.Reserved8, 1);
                    f.AddListItem(0x320, 4, "Region", Header.Region, 1);
                    f.AddListItem(0x324, 4, "Unknown 9", Header.Unknown9, 1);
                    f.AddListItem(0x328, 8, "Unknown 10", Header.Unknown10, 1);
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
            throw new NotImplementedException();
        }

        public TreeNode GetExplorerTopNode()
        {
            var tNode = new TreeNode("CCI") { Tag = TreeViewContextTag.Create(this, (int)CCIView.NCSD) };
            for (var i = 0; i < CXIContexts.Length; i++)
                if (CXIContexts[i] != null)
                    tNode.Nodes.Add(CXIContexts[i].GetExplorerTopNode());
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var tNode = new TreeNode("CCI", 1, 1);
            for (var i = 0; i < CXIContexts.Length; i++)
                if (CXIContexts[i] != null)
                    tNode.Nodes.Add(CXIContexts[i].GetFileSystemTopNode());
            return tNode;
        }
    }

}
