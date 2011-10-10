using System;
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
        public int TitleType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] GroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
        public byte[] Reserved2;
        public int AccessRights;
        public short TitleVersion;
        public short ContentCount;
        public short BootContent;
        public short Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentInfoRecordsHash;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDContentInfoRecord
    {
        public short ContentIndexOffset;
        public short ContentCommandCount; //K
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] NextContentHash; //SHA-256 hash of the next k content records that have not been hashed yet
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDContentChunkRecord
    {
        public int ContentID;
        public short ContentIndex;
        public short ContentType;
        public Int64 ContentSize;
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
        public int Tag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Name; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x104)]
        public byte[] Key;
        public short Unknown1;
        public short Unknown2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
        public byte[] Padding;
    }

    public class TMDTool
    {
        public static string typeToString(short type)
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
    }
}
