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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] PartitionFStype;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] PartitionCrypttype;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public CCICXIEntry[] CXIEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] UnknownZeros0;

        public ulong CXIFlags;
        //CXI Flags: 
        // byte[5]-byte[7] indicate content type ( system update, application, ... )
        // byte[6] size of media units ( 512*2^byte[6] ) and encryption

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ulong[] CXITitleIDs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] UnknownZeros1;

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
        private string _errorMessage = string.Empty;
        public CCIHeader Header;
        private CXIContext[] CXIContexts;
        private long _romSize;

        private enum CCIView
        {
            NCSD
        };

        private enum CCIActivation
        {
            SaveNCCH
        };

        public bool Open(Stream fs)
        {
            Header = MarshalUtil.ReadStruct<CCIHeader>(fs);
            _romSize = Header.CCILength * 512;
            CXIContexts = new CXIContext[8];
            // Read the CXIs
            for (var i = 0; i < CXIContexts.Length;i++ )
                if (Header.CXIEntries[i].Length > 0)
                {
                    CXIContexts[i] = new CXIContext();
                    var offset = Header.CXIEntries[i].Offset*0x200;
                    if (!(fs is MemoryStream) ||  offset < fs.Length) //fix for reading from memory streams (archived)
                        fs.Seek(offset, SeekOrigin.Begin);
                    else
                        fs.Seek(0, SeekOrigin.End);
                    CXIContexts[i].Open(fs);
                }
            return true;
        }

        private static void Fill(Stream stream, byte value, int count)
        {
            var buffer = new byte[64];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = value;
            }
            while (count > buffer.Length)
            {
                stream.Write(buffer, 0, buffer.Length);
                count -= buffer.Length;
            }
            stream.Write(buffer, 0, count);
        }

        public void ToggleTrimmed(Stream fs)
        {
            if (fs.Length <= Header.UsedRomLength) //Trimmed
            {
                fs.SetLength(_romSize);
                fs.Seek(Header.UsedRomLength, SeekOrigin.Begin);
                Fill(fs, 0xFF, (int)(_romSize - Header.UsedRomLength));
            }
            else
            {
                fs.SetLength(Header.UsedRomLength);
            }
        }

        public void SuperTrim(Stream fs)
        {
            var newLength = Header.CXIEntries[7].Offset * 512;
            if (newLength == 0 || fs.Length <= newLength) //can't be trimmed
            {
                throw new Exception("This file can't be trimmed any more!");
            }
            else
            {
                fs.SetLength(newLength);
                //delete last ncch from table
                var empty64 = new byte[8];
                fs.Seek(0x158, SeekOrigin.Begin);
                fs.Write(empty64, 0, 8);
                fs.Seek(0x1C8, SeekOrigin.Begin);
                fs.Write(empty64, 0, 8);
            }
        }

        public string GetErrorMessage()
        {
            return _errorMessage;
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
                    f.AddListItem(0x104, 4, "NCSD length [medias]", Header.CCILength, 1);
                    f.AddListItem(0x108, 8, "Main Title ID", Header.MainTitleID, 1);
                    f.AddListItem(0x110, 8, "Partitions FS Type", Header.PartitionFStype, 1);
                    f.AddListItem(0x118, 8, "Partitions Crypt Type", Header.PartitionCrypttype, 1);
                    for (var i = 0; i < 8; i++)
                    {
                        f.AddListItem(0x120 + i * 8, 4, "NCCH " + i + " offset [medias]", Header.CXIEntries[i].Offset, 1);
                        f.AddListItem(0x124 + i * 8, 4, "NCCH " + i + " length [medias]", Header.CXIEntries[i].Length, 1);
                    }
                    f.AddListItem(0x160,40, "Unknown Zeros 0", Header.UnknownZeros0, 1);
                    f.AddListItem(0x188, 8, "NCCH Flags", Header.CXIFlags, 1);
                    for (var i = 0; i < 8; i++)
                    {
                        f.AddListItem(0x190 + i * 8, 8, "NCCH " + i + " Title ID", Header.CXITitleIDs[i], 1);
                    }
                    f.AddListItem(0x1D0, 40, "Unknown Zeros 1", Header.UnknownZeros1, 1);
                    f.AddListItem(0x1F8, 8, "Unknown 1", Header.Unknown1, 1);
                    f.AddListItem(0x200, 4, "Always 0xFFFFFFFF", Header.PaddingFF, 1);
                    f.AddListItem(0x204, 252, "Unknown 2", Header.Unknown2, 1);
                    f.AddListItem(0x300, 4, "Used ROM length [bytes]", Header.UsedRomLength, 1);
                    f.AddListItem(0x304, 28, "Unknown 3", Header.Unknown3, 1);
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
            switch ((CCIActivation)type)
            {
                case CCIActivation.SaveNCCH:
                    var cxiIndex = (int)values[0];
                    var saveFileDialog = new SaveFileDialog() { Filter = CXIContexts[cxiIndex].GetFileFilter(), FileName = CXIContexts[cxiIndex].TitleInfo.ProductCode };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var infs = File.OpenRead(filePath);
                        infs.Seek(Header.CXIEntries[cxiIndex].Offset * 0x200, SeekOrigin.Begin);
                        SaverProcess.Run("Saving NCCH",infs,saveFileDialog.FileName,Header.CXIEntries[cxiIndex].Length*0x200);
                    }
                    break;
            }
        }

        public string GetFileFilter()
        {
            return "CTR Cartridge Images (*.cci/3ds/csu)|*.3ds;*.cci;*.csu";
        }

        public TreeNode GetExplorerTopNode()
        {
            var tNode = new TreeNode("NCSD") { Tag = TreeViewContextTag.Create(this, (int)CCIView.NCSD) };
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
            var tNode = new TreeNode("NCSD", 1, 1);
            for (var i = 0; i < CXIContexts.Length; i++)
                if (CXIContexts[i] != null)
                {
                    var cxiNode = CXIContexts[i].GetFileSystemTopNode();
                    cxiNode.Text = cxiNode.Text + @" " + i;
                    cxiNode.Tag = new[] {TreeViewContextTag.Create(this, (int) CCIActivation.SaveNCCH, "Save NCCH...",new object[] { i } )};
                    tNode.Nodes.Add(cxiNode);
                }
            return tNode;
        }
    }

}
