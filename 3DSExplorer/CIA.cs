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
        public uint SMDLength;
        public ulong AppLength;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIAMetaDataEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] Publisher;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] FirstTitle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] SecondTitle;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CIASMetaData
    {
        //SMDH - Header
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Padding0;
        // Entries
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public CIAMetaDataEntry[] SMDEntries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CIASMDHeaderEntry
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
            cxt.SMDOffset = cxt.AppOffset + (long)cxt.header.AppLength;
            if (cxt.SMDOffset % 64 != 0)
                cxt.SMDOffset += (64 - cxt.SMDOffset % 64);

            cxt.Ticket = TMDTool.OpenFromStream(fs, cxt.TicketOffset, cxt.header.TicketLength);
            TMDTool.OpenCertificatesFromStream(fs, cxt.CertificateChainOffset, cxt.header.CertificateChainLength, cxt.Ticket);
            cxt.tmdContext = TMDTool.OpenFromStream(fs, cxt.TMDOffset, cxt.header.TMDLength);

            if (cxt.header.SMDLength > 0)
            {
                fs.Seek(cxt.SMDOffset, SeekOrigin.Begin);
                cxt.SMDHeaderEntries = new ArrayList();
                CIASMDHeaderEntry smdhEntry = MarshalTool.ReadStruct<CIASMDHeaderEntry>(fs);
                while (smdhEntry.Type != 0)
                {
                    cxt.SMDHeaderEntries.Add(smdhEntry);
                    smdhEntry = MarshalTool.ReadStruct<CIASMDHeaderEntry>(fs);
                }
                fs.Seek(cxt.SMDOffset + 0x400, SeekOrigin.Begin); //Jump to the header
                cxt.smd = MarshalTool.ReadStruct<CIASMetaData>(fs);
            }
            
            fs.Close();
            
            return cxt;
        }
    }
}
