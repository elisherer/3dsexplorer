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
    public struct CIAHeader
    {
        public ulong PaddingLength;
        public uint CertificateChainLength;
        public uint TicketLength;
        public uint TMDLength;
        public uint BannerLength;
        public ulong AppLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIABannerHeaderEntry
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
        public long AppOffset;
        public long BannerOffset;

        //public ArrayList Certificates; //of CertificateEntry
        //public Ticket Ticket;

        public CertificatesContext CertificatesContext;
        public TicketContext TicketContext;
        public TMDContext TMDContext;

        public ArrayList BannerHeaderEntries; //of CIABannerHeaderEntry
        public ICNContext ICN;

        public enum CIAView
        {
            CIA,
            Banner
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
            AppOffset = TMDOffset + Header.TMDLength; ;
            if (AppOffset % 64 != 0)
                AppOffset += (64 - AppOffset % 64);
            BannerOffset = AppOffset + (long)Header.AppLength;
            if (BannerOffset % 64 != 0)
                BannerOffset += (64 - BannerOffset % 64);

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

            if (Header.BannerLength > 0)
            {
                fs.Seek(BannerOffset, SeekOrigin.Begin);
                BannerHeaderEntries = new ArrayList();
                var bannerHeaderEntry = MarshalUtil.ReadStruct<CIABannerHeaderEntry>(fs);
                while (bannerHeaderEntry.Type != 0)
                {
                    BannerHeaderEntries.Add(bannerHeaderEntry);
                    bannerHeaderEntry = MarshalUtil.ReadStruct<CIABannerHeaderEntry>(fs);
                }
                fs.Seek(BannerOffset + 0x400, SeekOrigin.Begin); //Jump to the header
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
                    f.AddListItem(20, 4, "Banner Length", cia.BannerLength, 0);
                    f.AddListItem(24, 8, "App Length", cia.AppLength, 0);

                    f.AddListItem(0, 8, "Certificate Chain Offset", (ulong)CertificateChainOffset, 1);
                    f.AddListItem(0, 8, "Ticket Offset", (ulong)TicketOffset, 1);
                    f.AddListItem(0, 8, "TMD Offset", (ulong)TMDOffset, 1);
                    f.AddListItem(0, 8, "App Offset", (ulong)AppOffset, 1);
                    f.AddListItem(0, 8, "Banner Offset", (ulong)BannerOffset, 1);
                    break;
                case CIAView.Banner:
                    CIABannerHeaderEntry entry;
                    f.SetGroupHeaders("CIA Banner");
                    for (var i = 0; i < BannerHeaderEntries.Count; i++)
                    {
                        entry = (CIABannerHeaderEntry)BannerHeaderEntries[i];
                        f.AddListItem(i, 2, "Type " + entry.Type, entry.Index, 0);
                        f.AddListItem(i, 4, "Magic", entry.Magic, 0);
                    }
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return Header.BannerLength > 0;
        }

        public void Activate(string filePath, int type, object[] values)
        {
            throw new System.NotImplementedException();
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
            if (Header.BannerLength > 0)
            {
                var bNode = new TreeNode("Banner") {Tag = TreeViewContextTag.Create(this, (int) CIAView.Banner)};
                bNode.Nodes.Add(ICN.GetExplorerTopNode());
                tNode.Nodes.Add(bNode);
            }
            return tNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var topNode = new TreeNode("CIA", 1, 1);
            topNode.Nodes.Add(ICN.GetFileSystemTopNode());
            return topNode;
        }
    }

}
