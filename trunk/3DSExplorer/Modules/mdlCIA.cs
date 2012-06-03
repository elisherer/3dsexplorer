using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer.Modules
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIAHeader
    {
        public ulong PaddingLength;
        public uint CertificateChainLength;
        public uint TicketLength;
        public uint TMDLength;
        public uint MetaLength;
        public ulong ContentLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIAMetaHeaderEntry
    {
        public byte Type;
        public byte Index;
        public ushort Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public byte[] Magic;
    }

    public class CIAContext : IContext
    {
        private string errorMessage = string.Empty;
        public CIAHeader Header;
        public long CertificateChainOffset;
        public long TicketOffset;
        public long TMDOffset;
        public long ContentOffset;
        public long MetaOffset;

        //public ArrayList Certificates; //of CertificateEntry
        //public Ticket Ticket;

        public CertificatesContext CertificatesContext;
        public TicketContext TicketContext;
        public TMDContext TMDContext;

        public ArrayList MetaHeaderEntries; //of CIAMetaHeaderEntry
        public ICNContext ICN;

        public enum CIAView
        {
            CIA,
            Meta
        };

        public bool Open(Stream fs)
        {
            var intBytes = new byte[4];
            Header = MarshalUtil.ReadStruct<CIAHeader>(fs); //read header

            CertificateChainOffset = Marshal.SizeOf(Header) + (long)Header.PaddingLength;
            TicketOffset = CertificateChainOffset + Header.CertificateChainLength;
            if (TicketOffset % 64 != 0)
                TicketOffset += (64 - TicketOffset % 64);
            TMDOffset = TicketOffset + Header.TicketLength;
            if (TMDOffset % 64 != 0)
                TMDOffset += (64 - TMDOffset % 64);
            ContentOffset = TMDOffset + Header.TMDLength; ;
            if (ContentOffset % 64 != 0)
                ContentOffset += (64 - ContentOffset % 64);
            MetaOffset = ContentOffset + (long)Header.ContentLength;
            if (MetaOffset % 64 != 0)
                MetaOffset += (64 - MetaOffset % 64);

            fs.Seek(TicketOffset, SeekOrigin.Begin);
            TicketContext = new TicketContext();
            if (!TicketContext.Open(fs))
            {
                errorMessage = TicketContext.GetErrorMessage();
                return false;
            }
            fs.Seek(CertificateChainOffset, SeekOrigin.Begin);
            CertificatesContext = new CertificatesContext();
            if (!CertificatesContext.Open(fs))
            {
                errorMessage = CertificatesContext.GetErrorMessage();
                return false;
            }
            fs.Seek(TMDOffset, SeekOrigin.Begin);
            TMDContext = new TMDContext();
            if (!TMDContext.Open(fs))
            {
                errorMessage = TMDContext.GetErrorMessage();
                return false;
            }

            if (Header.MetaLength > 0)
            {
                fs.Seek(MetaOffset, SeekOrigin.Begin);
                MetaHeaderEntries = new ArrayList();
                var metaHeaderEntry = MarshalUtil.ReadStruct<CIAMetaHeaderEntry>(fs);
                while (metaHeaderEntry.Type != 0)
                {
                    MetaHeaderEntries.Add(metaHeaderEntry);
                    metaHeaderEntry = MarshalUtil.ReadStruct<CIAMetaHeaderEntry>(fs);
                }
                fs.Seek(MetaOffset + 0x400, SeekOrigin.Begin); //Jump to the header
                ICN = new ICNContext();
                ICN.Open(fs);
            }
            return true;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        public void Create(FileStream fs, FileStream src)
        {
            if (ICN != null)
                ICN.Create(fs,src);
        }

        public void View(frmExplorer f, int view, object[] values)
        {
            f.ClearInformation();
            switch ((CIAView)view)
            {
                case CIAView.CIA:
                    var cia = Header;
                    f.SetGroupHeaders("CIA", "CIA Offsets");
                    f.AddListItem(0, 8, "Padding Length", cia.PaddingLength, 0);
                    f.AddListItem(8, 4, "Certificate Chain Length", cia.CertificateChainLength, 0);
                    f.AddListItem(12, 4, "Ticket Length", cia.TicketLength, 0);
                    f.AddListItem(16, 4, "TMD Length", cia.TMDLength, 0);
                    f.AddListItem(20, 4, "Meta Length", cia.MetaLength, 0);
                    f.AddListItem(24, 8, "Content Length", cia.ContentLength, 0);

                    f.AddListItem(0, 8, "Certificate Chain Offset", (ulong)CertificateChainOffset, 1);
                    f.AddListItem(0, 8, "Ticket Offset", (ulong)TicketOffset, 1);
                    f.AddListItem(0, 8, "TMD Offset", (ulong)TMDOffset, 1);
                    f.AddListItem(0, 8, "Content Offset", (ulong)ContentOffset, 1);
                    f.AddListItem(0, 8, "Meta Offset", (ulong)MetaOffset, 1);
                    break;
                case CIAView.Meta:
                    CIAMetaHeaderEntry entry;
                    f.SetGroupHeaders("CIA Meta");
                    for (var i = 0; i < MetaHeaderEntries.Count; i++)
                    {
                        entry = (CIAMetaHeaderEntry)MetaHeaderEntries[i];
                        f.AddListItem(i, 2, "Type " + entry.Type, entry.Index, 0);
                        f.AddListItem(i, 4, "Magic", entry.Magic, 0);
                    }
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return Header.MetaLength > 0;
        }

        public void Activate(string filePath, int type, object[] values)
        {
            var saveFileDialog = new SaveFileDialog { Filter = @"All Files|*.*" };
            var chunk = (TMDContentChunkRecord)values[0];
            var offset = (long)values[1];
            saveFileDialog.FileName = string.Format("content.{0}.{1:X}.bin", chunk.ContentIndex, chunk.ContentID);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var fs = File.OpenRead(filePath);
                    fs.Seek(offset, SeekOrigin.Begin);
                    var buffer = new byte[chunk.ContentSize];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    /*/decrypt
                    var iv = new byte[0x10];
                    iv[0] = (byte) ((chunk.ContentIndex >> 8) & 0xff);
                    iv[1] = (byte) (chunk.ContentIndex & 0xff);
                    var key = TicketContext.Ticket.EncryptedTitleKey;
                    var aes = new Aes128Ctr(key, iv);
                    aes.TransformBlock(buffer);*/
                    File.WriteAllBytes(saveFileDialog.FileName, buffer);
                }
                catch
                {
                    MessageBox.Show(@"Error saving file!");
                }
            }
        }

        public string GetFileFilter()
        {
            return "CTR Importable Archives (*.cia)|*.cia";
        }

        public TreeNode GetExplorerTopNode()
        {
            var tNode = new TreeNode("CIA") { Tag = TreeViewContextTag.Create(this, (int)CIAView.CIA) };
            if (CertificatesContext != null && CertificatesContext.List.Count > 0)
                tNode.Nodes.Add(CertificatesContext.GetExplorerTopNode());
            if ((uint) TicketContext.Ticket.SignatureType != 0)
                tNode.Nodes.Add(TicketContext.GetExplorerTopNode());
            if (TMDContext != null)
                tNode.Nodes.Add(TMDContext.GetExplorerTopNode());
            if (Header.MetaLength > 0)
            {
                var bNode = new TreeNode("Meta") {Tag = TreeViewContextTag.Create(this, (int) CIAView.Meta)};
                bNode.Nodes.Add(ICN.GetExplorerTopNode());
                tNode.Nodes.Add(bNode);
            }
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var topNode = new TreeNode("CIA", 1, 1);
            if (Header.MetaLength > 0)
            {
                topNode.Nodes.Add(ICN.GetFileSystemTopNode());
            }
            if (TMDContext != null && TMDContext.Chunks.Length > 0) //add content files
            {

                var contentNode = new TreeNode("Content", 1, 1);
                topNode.Nodes.Add(contentNode);
                var offset = (ulong)ContentOffset;
                for (var i = 0; i < TMDContext.Chunks.Length; i++)
                {
                    var filename = string.Format("content.{0}.{1:X}.bin", TMDContext.Chunks[i].ContentIndex,TMDContext.Chunks[i].ContentID);
                    contentNode.Nodes.Add(
                        new TreeNode(
                            TreeListView.TreeListViewControl.CreateMultiColumnNodeText(
                                filename,TMDContext.Chunks[i].ContentSize.ToString(),"0x" + offset.ToString("X")))
                            {
                                Tag = new[] { TreeViewContextTag.Create(this, 0, "Save...",new object[] {TMDContext.Chunks[i], (long)offset}) }
                            });
                    offset += TMDContext.Chunks[i].ContentSize;
                }
            }
            return topNode;
        }
    }

}
