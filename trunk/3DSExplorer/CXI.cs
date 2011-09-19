using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct String8
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        char[] text;
    }

    public class CXIPlaingRegion
    {
        public string[] PlainRegionStrings;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXI
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] NCCHHeaderSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] MagicID;
        public int CXISize;
        public UInt64 PartitionID; //maybe number of ncch?
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] MakerCode;
        public ushort Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved0;
       
        public UInt64 ProgramID;
        public char TempFlag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 47)]
        public byte[] Reserved1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ProductCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ExtendedHeaderHash;
        public uint ExtendedHeaderSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Flags;

        public uint PlainRegionOffset;
        public uint PlainRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved3;

        public uint ExeFSOffset;
        public uint ExeFSSize;
        public uint ExeFSHashRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved4;

        public uint RomFSOffset;
        public uint RomFSSize;
        public uint RomFSHashRegionSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved5;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExeFSSuperBlockhash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] RomFSSuperBlockhash;

        public static CXIPlaingRegion getPlainRegionStringsFrom(byte[] buffer)
        {
            CXIPlaingRegion temp = new CXIPlaingRegion();
            string bigstring = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            string[] splited = bigstring.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            temp.PlainRegionStrings = splited;
            return temp;
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeader
    {
        CXIExtendedHeaderCodeSetInfo CodeSetInfo;
        CXIExtendedHeaderDependencyList DependencyList;
        CXIExtendedHeaderSystemInfo SystemInfo;
        CXIExtendedHeaderArm11SystemLocalCaps Arm11SystemLocalCaps;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSetInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Name;
        CXIExtendedHeaderSystemInfoFlags Flags;
        CXIExtendedHeaderCodeSegmentInfo Text;
        int StackSize;
        CXIExtendedHeaderCodeSegmentInfo Ro;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] Reserved;
        CXIExtendedHeaderCodeSegmentInfo Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] BssSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfoFlags
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        byte[] Reserved;
        byte Flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        byte[] RemasterVersion;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSegmentInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] Address;
        int NumMaxPages;
        int CodeSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderDependencyList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        String8[] ProgramID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfo
    {
        int SaveDataSize;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] JumpID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        byte[] Reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderStorageInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] ExtSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] SystemSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        byte[] AccessInfo;
        byte OtherAttributes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ResourceLimit
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderArm11SystemLocalCaps
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] ProgramID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        byte[] Flags;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        ResourceLimit[] ResourceLimitDescriptor;
        CXIExtendedHeaderStorageInfo StorageInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        String8[] ServiceAccessControl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1f)]
        byte[] Reserved;
        byte ResourceLimitCategory;
    }
    

}
