using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    public enum TMDSignatureType
    {
        RSA_2048_SHA256 = 0x04000100,
        RSA_4096_SHA256 = 0x03000100,
        RSA_2048_SHA1   = 0x01000100,
        RSA_4096_SHA1   = 0x00000100
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] Reserved0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Issuer;
        public byte Version;
        public byte CarCrlVersion;
        public byte SignerVersion;
        public byte Reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] SystemVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] TitleID;
        public uint TitleType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] GroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
        public byte[] Reserved2;
        public uint AccessRights;
        public ushort TitleVersion;
        public ushort ContentCount;
        public ushort BootContent;
        public ushort Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentInfoRecordsHash;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDContentInfoRecord
    {
        public ushort ContentIndexOffset;
        public ushort ContentCommandCount; //K
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] NextContentHash; //SHA-256 hash of the next k content records that have not been hashed yet
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDContentChunkRecord
    {
        public uint ContentID;
        public ushort ContentIndex;
        public ushort ContentType;
        public ulong ContentSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentHash; //SHA-256
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDCertificate
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] Reserved0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Issuer;
        public uint Tag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Name; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x104)]
        public byte[] Key;
        public ushort Unknown1;
        public ushort Unknown2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
        public byte[] Padding;
    }

    public class TMDTool
    {
        public static string typeToString(ushort type)
        {
            string ret = "";
            if ((type & 1) != 0)
                ret += "[encrypted]";
            if ((type & 2) != 0)
                ret += "[disc]";
            if ((type & 4) != 0)
                ret += "[cfm]";
            if ((type & 0x4000) != 0)
                ret += "[optional]";
            if ((type & 0x8000) != 0)
                ret += "[shared]";
            return ret;
        }

        public static TMDContext Open(string path)
        {
            TMDContext cxt;
            FileStream fs = File.OpenRead(path);
            cxt = OpenFromStream(fs, 0, fs.Length);
            if (cxt != null)
                OpenCertificatesFromStream(fs, fs.Position, fs.Length, cxt);
            fs.Close();
            return cxt;
        }

        public static TMDContext OpenFromStream(FileStream fs, long offset, long tmdLength)
        {
            TMDContext cxt = new TMDContext();

            fs.Seek(offset, SeekOrigin.Begin);

            bool supported = true;

            byte[] intBytes = new byte[4];
            fs.Read(intBytes, 0, 4);
            cxt.SignatureType = (TMDSignatureType)BitConverter.ToInt32(intBytes, 0);
            // Read the TMD RSA Type 
            if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256)
                cxt.tmdSHA = new byte[256];
            else if (cxt.SignatureType == TMDSignatureType.RSA_4096_SHA256)
                cxt.tmdSHA = new byte[512];
            else
                supported = false;
            if (supported)
            {
                fs.Read(cxt.tmdSHA, 0, cxt.tmdSHA.Length);
                //Continue reading header
                cxt.head = MarshalTool.ReadStructBE<TMDHeader>(fs); //read header
                cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                for (int i = 0; i < cxt.ContentInfoRecords.Length; i++)
                    cxt.ContentInfoRecords[i] = MarshalTool.ReadStructBE<TMDContentInfoRecord>(fs);
                cxt.chunks = new ArrayList();
                for (int i = 0; i < cxt.head.ContentCount; i++)
                    cxt.chunks.Add(MarshalTool.ReadStructBE<TMDContentChunkRecord>(fs));
            }
            return (supported ? cxt : null);
        }

        public static void OpenCertificatesFromStream(FileStream fs, long offset, long length, TMDContext cxt)
        {
            byte[] intBytes = new byte[4];
            fs.Seek(offset, SeekOrigin.Begin);
            cxt.certs = new ArrayList();
            while ((fs.Position < offset + length) && (fs.Position < fs.Length))
            {
                TMDCertContext tcert = new TMDCertContext();
                fs.Read(intBytes, 0, 4);
                tcert.SignatureType = (TMDSignatureType)BitConverter.ToInt32(intBytes, 0);
                // RSA Type
                if (tcert.SignatureType == TMDSignatureType.RSA_2048_SHA256 || tcert.SignatureType == TMDSignatureType.RSA_2048_SHA1)
                    tcert.tmdSHA = new byte[256];
                else if (tcert.SignatureType == TMDSignatureType.RSA_4096_SHA256 || tcert.SignatureType == TMDSignatureType.RSA_4096_SHA1)
                    tcert.tmdSHA = new byte[512];
                else
                    break; //no more certificates
                fs.Read(tcert.tmdSHA, 0, tcert.tmdSHA.Length);
                tcert.cert = MarshalTool.ReadStructBE<TMDCertificate>(fs);
                cxt.certs.Add(tcert);
            }
        }
    }
}
