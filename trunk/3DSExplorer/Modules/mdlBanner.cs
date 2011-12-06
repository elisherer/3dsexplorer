using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CBMD
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Padding0;
        public uint CompressedCGFXOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x78)]
        public byte[] Padding1;
        public uint CWAVOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATABlobHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Length;
    }

    public class BannerContext : IContext
    {
        public enum BannerView
        {
            Banner
        };

        private string errorMessage = string.Empty;
        public CBMD Header;
        public byte[] DecompressedCGFX;
        public CGFXContext CGFXContext;
        public CWAVContext CWAVContext;        

        public bool Open(Stream fs)
        {
            Header = MarshalUtil.ReadStruct<CBMD>(fs); //read header

            //-- Graphics Reading --

            //Read ahead the size of the uncompressed file
            fs.Seek(Header.CompressedCGFXOffset + 1, SeekOrigin.Begin);
            var intBytes = new byte[4];
            fs.Read(intBytes, 0, 4);
            DecompressedCGFX = new byte[BitConverter.ToUInt32(intBytes, 0)];
            //Read again from the start
            fs.Seek(Header.CompressedCGFXOffset, SeekOrigin.Begin);
            var ms = new MemoryStream(DecompressedCGFX);
            try
            {
                var lz11 = new DSDecmp.Formats.Nitro.LZ11();
                lz11.Decompress(fs, Header.CWAVOffset - fs.Position, ms);
            }
            catch
            { //might throw exception if size of compressed is bigger than it should be
            }
            ms.Seek(0, SeekOrigin.Begin);
            CGFXContext = new CGFXContext();
            CGFXContext.Open(ms);
            fs.Seek(Header.CWAVOffset, SeekOrigin.Begin);
            CWAVContext = new CWAVContext();
            CWAVContext.Open(fs);
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

        public void View(frmExplorer f, int view, int[] values)
        {
            f.ClearInformation();
            switch ((BannerView)view)
            {
                case BannerView.Banner:
                    var bmd = Header;
                    f.SetGroupHeaders("CBMD");
                    f.AddListItem(0, 4, "Magic", bmd.Magic, 0);
                    f.AddListItem(4, 4, "Padding 0", bmd.Padding0, 0);
                    f.AddListItem(8, 4, "Compressed CGFX Offset", bmd.CompressedCGFXOffset, 0);
                    f.AddListItem(0x10, 0x78, "Padding 1", bmd.Padding1, 0);
                    f.AddListItem(0x84, 4, "CWAV Offset", bmd.CWAVOffset, 0);
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return false;
        }

        public TreeNode GetExplorerTopNode()
        {
            var topNode = new TreeNode("CBMD") { Tag = TreeViewContextTag.Create(this, (int)BannerView.Banner) };
            topNode.Nodes.Add(CGFXContext.GetExplorerTopNode());
            topNode.Nodes.Add(CWAVContext.GetExplorerTopNode());
            return topNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var topNode = new TreeNode("CBMD", 1, 1);
            topNode.Nodes.Add(CGFXContext.GetFileSystemTopNode());
            topNode.Nodes.Add(CWAVContext.GetFileSystemTopNode());
            return topNode;
        }
    }
}