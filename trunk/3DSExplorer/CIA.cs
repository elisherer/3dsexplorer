using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace _3DSExplorer
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
    }
}
