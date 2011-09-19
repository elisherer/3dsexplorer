using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    enum TMDSignatureType {
        RSA_2048_SHA256 = 0x04000100,
        RSA_4096_SHA256 = 0x03000100,
        RSA_2048_SHA1   = 0x01000100,
        RSA_4096_SHA1   = 0x00000100
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMD2048
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Signature;
        public TMDHeader Header;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMD4096
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] Signature;
        public TMDHeader Header;
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
        public short GroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
        public byte[] Reserved2;
        public int AccessRights;
        public short TitleVersion;
        public short ContentCount;
        public short BootContent;
        public short Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentInfoRecordsHash;

        //These doesn't work!!

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        //public TMDContentInfoRecord[] ContentInfoRecords;
        //TMDContentChunkRecord[ContentCount]
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
    public class TMDCertificate2048
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Signature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Issuer;
        public int Tag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Name; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x104)]
        public byte[] Key;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TMDCertificate4096
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] Signature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Issuer;
        public int Tag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x104)]
        public byte[] Key;
    }
}
