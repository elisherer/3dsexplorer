using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    public enum SignatureType
    {
        RSA_2048_SHA256 = 0x04000100,
        RSA_4096_SHA256 = 0x03000100,
        RSA_2048_SHA1   = 0x01000100,
        RSA_4096_SHA1   = 0x00000100
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMDHeader
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
    public struct TMDContentInfoRecord
    {
        public ushort ContentIndexOffset;
        public ushort ContentCommandCount; //K
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] NextContentHash; //SHA-256 hash of the next k content records that have not been hashed yet
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMDContentChunkRecord
    {
        public uint ContentID;
        public ushort ContentIndex;
        public ushort ContentType;
        public ulong ContentSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentHash; //SHA-256
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
            {
                cxt.Certificates = new ArrayList();
                CertTool.OpenCertificatesFromStream(fs, fs.Position, fs.Length, cxt.Certificates);
            }
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
            cxt.SignatureType = (SignatureType)BitConverter.ToInt32(intBytes, 0);
            // Read the TMD RSA Type 
            if (cxt.SignatureType == SignatureType.RSA_2048_SHA256)
                cxt.Hash = new byte[256];
            else if (cxt.SignatureType == SignatureType.RSA_4096_SHA256)
                cxt.Hash = new byte[512];
            else
                supported = false;
            if (supported)
            {
                fs.Read(cxt.Hash, 0, cxt.Hash.Length);
                //Continue reading header
                cxt.head = MarshalTool.ReadStructBE<TMDHeader>(fs); //read header
                cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                for (int i = 0; i < cxt.ContentInfoRecords.Length; i++)
                    cxt.ContentInfoRecords[i] = MarshalTool.ReadStructBE<TMDContentInfoRecord>(fs);
                cxt.chunks = new TMDContentChunkRecord[cxt.head.ContentCount];// new ArrayList();
                for (int i = 0; i < cxt.head.ContentCount; i++)
                    cxt.chunks[i] = MarshalTool.ReadStructBE<TMDContentChunkRecord>(fs);
            }
            return (supported ? cxt : null);
        }
    }
}
