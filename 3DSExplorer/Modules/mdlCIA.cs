using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace _3DSExplorer
{
    public class CIAContext : Context
    {
        public CIAHeader header;
        public long CertificateChainOffset;
        public long TicketOffset;
        public long TMDOffset;
        public long AppOffset;
        public long BannerOffset;
        public ArrayList Certificates; //of CertificateEntry
        public Ticket Ticket;
        public TMDContext TMD;

        public ArrayList BannerHeaderEntries; //of CIABannerHeaderEntry
        public CIABanner Banner;
        public Bitmap SmallIcon, LargeIcon;
    }

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
    public struct CIAMetaDataEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] FirstTitle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] SecondTitle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] Publisher;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIABanner
    {
        //SMDH - Header
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Padding0;
        // Entries
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public CIAMetaDataEntry[] MDEntries;
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
    
    public class CIATool
    {
        public static CIAContext Open(string path)
        {
            CIAContext cxt = new CIAContext();

            FileStream fs = File.OpenRead(path);

            byte[] intBytes = new byte[4];
            cxt.header = MarshalTool.ReadStruct<CIAHeader>(fs); //read header

            cxt.CertificateChainOffset = Marshal.SizeOf(cxt.header) + (long)cxt.header.PaddingLength;
            cxt.TicketOffset = cxt.CertificateChainOffset + cxt.header.CertificateChainLength;
            if (cxt.TicketOffset % 64 != 0)
                cxt.TicketOffset += (64 - cxt.TicketOffset % 64);
            cxt.TMDOffset = cxt.TicketOffset + cxt.header.TicketLength;
            if (cxt.TMDOffset % 64 != 0)
                cxt.TMDOffset += (64 - cxt.TMDOffset % 64);
            cxt.AppOffset = cxt.TMDOffset + cxt.header.TMDLength; ;
            if (cxt.AppOffset % 64 != 0)
                cxt.AppOffset += (64 - cxt.AppOffset % 64);
            cxt.BannerOffset = cxt.AppOffset + (long)cxt.header.AppLength;
            if (cxt.BannerOffset % 64 != 0)
                cxt.BannerOffset += (64 - cxt.BannerOffset % 64);

            cxt.Ticket = TicketTool.OpenFromStream(fs, cxt.TicketOffset);
            cxt.Certificates = new ArrayList();
            CertTool.OpenCertificatesFromStream(fs, cxt.CertificateChainOffset, cxt.header.CertificateChainLength, cxt.Certificates);
            cxt.TMD = TMDTool.OpenFromStream(fs, cxt.TMDOffset, cxt.header.TMDLength);

            if (cxt.header.BannerLength > 0)
            {
                fs.Seek(cxt.BannerOffset, SeekOrigin.Begin);
                cxt.BannerHeaderEntries = new ArrayList();
                CIABannerHeaderEntry bannerHeaderEntry = MarshalTool.ReadStruct<CIABannerHeaderEntry>(fs);
                while (bannerHeaderEntry.Type != 0)
                {
                    cxt.BannerHeaderEntries.Add(bannerHeaderEntry);
                    bannerHeaderEntry = MarshalTool.ReadStruct<CIABannerHeaderEntry>(fs);
                }
                fs.Seek(cxt.BannerOffset + 0x400, SeekOrigin.Begin); //Jump to the header
                cxt.Banner = MarshalTool.ReadStruct<CIABanner>(fs);
                fs.Seek(cxt.BannerOffset + 0x2400, SeekOrigin.Begin); //Jump to the icons
                fs.Seek(0x40, SeekOrigin.Current); //skip header
                cxt.SmallIcon = RawDecoder.CIAIcoDecode(fs, 24, 8);
                cxt.LargeIcon = RawDecoder.CIAIcoDecode(fs, 48, 8);
            }
            
            fs.Close();
            
            return cxt;
        }

        static class RawDecoder
        {
            private const double CNV_5BIT_TO_8BIT = 0xFF / 0x1F;
            private const double CNV_6BIT_TO_8BIT = 0xFF / 0x3F;

            public static Color colorFrom2Bytes(byte[] bytes) //Using GBR655
            {
                int green = (bytes[0] & 0xFC) >> 2;
                int blue = ((bytes[0] & 0x03) << 3);
                blue += ((bytes[1] & 0xE0) >> 5);
                int red = bytes[1] & 0x1F;
                return Color.FromArgb((int)(red * CNV_5BIT_TO_8BIT), (int)(green * CNV_6BIT_TO_8BIT), (int)(blue * CNV_5BIT_TO_8BIT));
            }

            private static void fillBitmap(int iconSize, int tileSize, int ax, int ay, Bitmap bmp, FileStream fs)
            {
                if (tileSize == 0)
                {
                    byte[] rgbVal = new byte[2];
                    fs.Read(rgbVal, 0, 2);
                    bmp.SetPixel(ax, ay, colorFrom2Bytes(rgbVal));
                }
                else
                    for (int y = 0; y < iconSize; y += tileSize)
                        for (int x = 0; x < iconSize; x += tileSize)
                            fillBitmap(tileSize, tileSize / 2, x + ax, y + ay, bmp, fs);
            }

            public static Bitmap CIAIcoDecode(FileStream fs, int iconSize, int tileSize)
            {
                Bitmap bmp = new Bitmap(iconSize, iconSize);
                fillBitmap(iconSize, tileSize, 0, 0, bmp, fs);
                return bmp;
            }
        }

        public enum CIAView
        {
            CIA,
            Banner,
            BannerMetaData
        };

        public static void View(frmExplorer f, CIAContext cxt, CIAView view)
        {
            f.ClearInformation();
            switch (view)
            {
                case CIAView.CIA:
                    CIAHeader cia = cxt.header;
                    f.SetGroupHeaders("CIA", "CIA Offsets");
                    f.AddListItem(0, 8, "Padding Length", cia.PaddingLength, 0);
                    f.AddListItem(8, 4, "Certificate Chain Length", cia.CertificateChainLength, 0);
                    f.AddListItem(12, 4, "Ticket Length", cia.TicketLength, 0);
                    f.AddListItem(16, 4, "TMD Length", cia.TMDLength, 0);
                    f.AddListItem(20, 4, "Banner Length", cia.BannerLength, 0);
                    f.AddListItem(24, 8, "App Length", cia.AppLength, 0);

                    f.AddListItem(0, 8, "Certificate Chain Offset", (ulong)cxt.CertificateChainOffset, 1);
                    f.AddListItem(0, 8, "Ticket Offset", (ulong)cxt.TicketOffset, 1);
                    f.AddListItem(0, 8, "TMD Offset", (ulong)cxt.TMDOffset, 1);
                    f.AddListItem(0, 8, "App Offset", (ulong)cxt.AppOffset, 1);
                    f.AddListItem(0, 8, "Banner Offset", (ulong)cxt.BannerOffset, 1);
                    break;
                case CIAView.Banner:
                    CIABannerHeaderEntry entry;
                    f.SetGroupHeaders("CIA Banner");
                    for (int i = 0; i < cxt.BannerHeaderEntries.Count; i++)
                    {
                        entry = (CIABannerHeaderEntry)cxt.BannerHeaderEntries[i];
                        f.AddListItem(i, 2, "Type " + entry.Type, entry.Index, 0);
                        f.AddListItem(i, 4, "Magic", entry.Magic, 0);
                    }
                    break;
                case CIAView.BannerMetaData:
                    f.SetGroupHeaders("Banner Meta-Data", "Icons");
                    string pubString, firString, secString;
                    f.AddListItem(0, 4, "Banner Meta-Data Magic (SMDH)", cxt.Banner.Magic, 0);
                    f.AddListItem(4, 4, "Padding 0", cxt.Banner.Padding0, 0);

                    for (int i = 0; i < cxt.Banner.MDEntries.Length; i++)
                    {
                        pubString = Encoding.Unicode.GetString(cxt.Banner.MDEntries[i].Publisher);
                        firString = Encoding.Unicode.GetString(cxt.Banner.MDEntries[i].FirstTitle);
                        secString = Encoding.Unicode.GetString(cxt.Banner.MDEntries[i].SecondTitle);
                        f.AddListItem(i.ToString(), "0x200", firString, secString, pubString, 0);
                    }

                    f.AddListItem(0, 0, "Small Icon", 24, 1);
                    f.AddListItem(0, 0, "Large Icon", 48, 1);
                    break;
            }
            f.AutoAlignColumns();
        }
    }
}
