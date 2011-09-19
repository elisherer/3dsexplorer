using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //, Size = 0x330
    public struct CCI
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] NCSDHeaderSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] MagicID;
        public int CCISize;
        public UInt64 TitleID; //maybe number of ncch?

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Reserved0;

        public int FirstNCCHOffset;
        public int FirstNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved1;

        public int SecondNCCHOffset;
        public int SecondNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved2;

        public int ThirdNCCHOffset;
        public int ThirdNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] Reserved3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NCCHFlags; //NCCH Flags: byte[5]-byte[7] indicate content type ( system update, application, ... ) size of media units ( 512*2^byte[6] ) and encryption
        public UInt64 FirstNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved4;

        public UInt64 SecondNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved5;

        public UInt64 ThirdNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Reserved6;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] PaddingFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 252)]
        public byte[] Reserved7;
        
        public int UsedRomSize; //in bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Reserved8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown;
    }
}
