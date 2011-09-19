using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFHeader
    {
        public int Unknown1;
        public int Unknown2;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFHeaderEntry
    {
        public byte PhysicalSector; // when bit7 is set, block has checksums, otherwise checksums are all zero
        public byte AllocationCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] CheckSums; // 8*0x200=0x1000, each byte hashes 0x200 block with ModbusCRC16 XORed to 1 byte
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFSectorEntry
    {
        public byte VirtualSector;             // Mapped to sector
        public byte PreviousVirtualSector;     // Physical sector previously mapped to
        public byte PhysicalSector;            // Mapped from sector
        public byte PreviousPhysicalSector;    // Virtual sector previously mapped to
        public byte PhysSecReallocCount;       // Amount of times physical sector has been remapped
        public byte VirtSecReallocCount;       // Amount of times virtual sector has been remapped
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] CheckSums;
}
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFLongSectorEntry
    {
        public SFSectorEntry Sector;
        public SFSectorEntry Dupe;
        public int Magic; //constant  0x080d6ce0
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFFileSystemEntry
    {
        public int NodeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public char[] Filename;
        public int Index;
        public uint Magic;
        public int BlockOffset;
        public int FileSize;
        public int Unknown2;
        public int Unknown3; // flags and/or date?
        public int Unknown4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFDISABlob
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xc)]
        public byte[] Unknown0;
        public ulong FirstDifiOffset;
        public ulong SecondDifiOffset;
        public ulong FirstDifiSize;
        public ulong Padding;
        public ulong SecondDifiSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[] Unknown1;
        public uint FirstHashed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Hash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x74)]
        public byte[] Unknown2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFImageHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Hash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xF0)]
        public byte[] Padding;
        public SFDISABlob DISA;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFDIFIBlob
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] MagicDIFI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x35)]
        public byte[] Unknown0;
        public int Index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x7)]
        public byte[] Unknown1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] MagicIVFC; //74
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x54)]
        public byte[] Unknown2;
        public ulong HashTableLength;
        public ulong FileSystemLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Unknown3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] MagicDPFS;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x6C)]
        public byte[] Unknown4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Garbage;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFSave
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] MagicSAVE;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x54)]
        public byte[] Unknown0;
        public int FSTOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Unknown1;
        public int FSTBlockOffset; //FST is in [BlockOffset] * 0x200 + [Offset]
        //or
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
        public byte[] Unknown2;
        public int FSTExactOffset;
    }
}
