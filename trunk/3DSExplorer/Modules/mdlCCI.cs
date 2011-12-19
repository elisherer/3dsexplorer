using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer.Modules
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //, Size = 0x330
    public struct CCICXIEntry
    {
        public uint Offset;
        public uint Length;
    }

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
        public byte[] Unknown0;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public CCICXIEntry[] CXIEntries;
        
        public ulong CXIFlags;
        //CXI Flags: 
        // byte[5]-byte[7] indicate content type ( system update, application, ... )
        // byte[6] size of media units ( 512*2^byte[6] ) and encryption

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public ulong[] CXITitleIDs;

        public ulong Unknown1;

        public uint PaddingFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 252)]
        public byte[] Unknown2;
        
        public uint UsedRomLength; //in bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Unknown3;

        public ulong LoaderTitleId;
        public ulong LoaderTitleVersion;
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
            CXIContexts = new CXIContext[13];
            // Read the CXIs
            for (var i = 0; i < CXIContexts.Length;i++ )
                if (Header.CXIEntries[i].Length > 0)
                {
                    CXIContexts[i] = new CXIContext();
                    fs.Seek(Header.CXIEntries[i].Offset * 0x200, SeekOrigin.Begin);
                    CXIContexts[i].Open(fs);
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
                    f.AddListItem(0x110, 8, "Unknown 0", Header.Unknown0, 1);
                    for (var i = 0; i < Header.CXIEntries.Length; i++)
                    {
                        f.AddListItem(0x120 + i * 8, 4, "CXI " + i + " offset [medias]", Header.CXIEntries[i].Offset, 1);
                        f.AddListItem(0x124 + i * 8, 4, "CXI " + i + " length [medias]", Header.CXIEntries[i].Length, 1);
                    }
                    f.AddListItem(0x188, 8, "CXI Flags", Header.CXIFlags, 1);
                    for (var i = 0; i < Header.CXITitleIDs.Length; i++)
                    {
                        f.AddListItem(0x190 + i * 8, 8, "CXI " + i + " Title ID", Header.CXITitleIDs[i], 1);
                    }
                    f.AddListItem(0x1F8, 8, "Unknown 1", Header.Unknown1, 1);
                    f.AddListItem(0x200, 4, "Always 0xFFFFFFFF", Header.PaddingFF, 1);
                    f.AddListItem(0x204, 252, "Unknown 2", Header.Unknown2, 1);
                    f.AddListItem(0x300, 4, "Used ROM length [bytes]", Header.UsedRomLength, 1);
                    f.AddListItem(0x204, 28, "Unknown 3", Header.Unknown3, 1);
                    f.AddListItem(0x320, 8, "Loader Title ID", Header.LoaderTitleId, 1);
                    f.AddListItem(0x328, 8, "Loader Title Version", Header.LoaderTitleVersion, 1);
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
                {
                    var cxiNode = CXIContexts[i].GetExplorerTopNode();
                    cxiNode.Text = cxiNode.Text + @" " + i;
                    tNode.Nodes.Add(cxiNode);
                }
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var tNode = new TreeNode("CCI", 1, 1);
            for (var i = 0; i < CXIContexts.Length; i++)
                if (CXIContexts[i] != null)
                {
                    var cxiNode = CXIContexts[i].GetFileSystemTopNode();
                    cxiNode.Text = cxiNode.Text + @" " + i;
                    tNode.Nodes.Add(cxiNode);
                }
            return tNode;
        }
    }

}
