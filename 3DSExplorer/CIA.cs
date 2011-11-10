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
        public ulong CertificateChainOffset;
        public uint CertificateChainSize;
        public uint TicketSize;
        public uint TMDSize;
        public uint BannerSize;
        public ulong AppSize;
    }

    public class CIATool
    {
        public static CIAContext Open(string path)
        {
            CIAContext cxt = new CIAContext();

            FileStream fs = File.OpenRead(path);

            byte[] intBytes = new byte[4];
            cxt.header = MarshalTool.ReadStruct<CIAHeader>(fs); //read header
            fs.Close();

            cxt.CertificateChainOffset = Marshal.SizeOf(cxt.header) + (long)cxt.header.CertificateChainOffset;
            cxt.TicketOffset = cxt.CertificateChainOffset + cxt.header.CertificateChainSize;
            if (cxt.TicketOffset % 64 != 0)
                cxt.TicketOffset += (64 - cxt.TicketOffset % 64);
            cxt.TMDOffset = cxt.TicketOffset + cxt.header.TicketSize;
            if (cxt.TMDOffset % 64 != 0)
                cxt.TMDOffset += (64 - cxt.TMDOffset % 64);
            cxt.BannerOffset = cxt.TMDOffset + cxt.header.TMDSize;
            if (cxt.BannerOffset % 64 != 0)
                cxt.BannerOffset += (64 - cxt.BannerOffset % 64);
            cxt.AppOffset = cxt.BannerOffset + cxt.header.BannerSize;
            if (cxt.AppOffset % 64 != 0)
                cxt.AppOffset += (64 - cxt.AppOffset % 64);
            return cxt;
        }
    }
}
