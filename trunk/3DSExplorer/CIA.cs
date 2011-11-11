using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CIAHeader
    {
        public ulong PaddingLength;
        public uint CertificateChainLength;
        public uint TicketLength;
        public uint TMDLength;
        public uint BannerLength;
        public ulong AppLength;
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
            cxt.BannerOffset = cxt.TMDOffset + cxt.header.TMDLength;
            if (cxt.BannerOffset % 64 != 0)
                cxt.BannerOffset += (64 - cxt.BannerOffset % 64);
            cxt.AppOffset = cxt.BannerOffset + cxt.header.BannerLength;
            if (cxt.AppOffset % 64 != 0)
                cxt.AppOffset += (64 - cxt.AppOffset % 64);

            cxt.Ticket = TMDTool.OpenFromStream(fs, cxt.TicketOffset, cxt.header.TicketLength);
            TMDTool.OpenCertificatesFromStream(fs, cxt.CertificateChainOffset, cxt.header.CertificateChainLength, cxt.Ticket);
            cxt.tmdContext = TMDTool.OpenFromStream(fs, cxt.TMDOffset, cxt.header.TMDLength);
            fs.Close();
            
            return cxt;
        }
    }
}
