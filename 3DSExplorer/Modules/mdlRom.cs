using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    public class RomContext : Context
    {
        public CCI cci;
        public CXI[] cxis;
        public CXIPlaingRegion[] cxiprs;
        public int currentNcch;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)] //, Size = 0x330
    public struct CCI
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] NCSDHeaderSignature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] MagicID;
        public uint CCISize;
        public ulong TitleID; //maybe number of ncch?

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Reserved0;

        public uint FirstNCCHOffset;
        public uint FirstNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved1;

        public uint SecondNCCHOffset;
        public uint SecondNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved2;

        public uint ThirdNCCHOffset;
        public uint ThirdNCCHSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] Reserved3;

        public ulong NCCHFlags;
        //NCCH Flags: 
        // byte[5]-byte[7] indicate content type ( system update, application, ... )
        // byte[6] size of media units ( 512*2^byte[6] ) and encryption

        public ulong FirstNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved4;

        public ulong SecondNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved5;

        public ulong ThirdNCCHPartitionID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] Reserved6;

        public uint PaddingFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 252)]
        public byte[] Reserved7;
        
        public uint UsedRomSize; //in bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Reserved8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct String8
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] text;
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
        public uint CXISize;
        public ulong PartitionID; //maybe number of ncch?
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] MakerCode;
        public ushort Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ProgramID;
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

        public ulong Flags;
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
        public CXIExtendedHeaderCodeSetInfo CodeSetInfo;
        public CXIExtendedHeaderDependencyList DependencyList;
        public CXIExtendedHeaderSystemInfo SystemInfo;
        public CXIExtendedHeaderArm11SystemLocalCaps Arm11SystemLocalCaps;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSetInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] Name;
        public CXIExtendedHeaderSystemInfoFlags Flags;
        public CXIExtendedHeaderCodeSegmentInfo Text;
        public uint StackSize;
        public CXIExtendedHeaderCodeSegmentInfo Ro;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
        public CXIExtendedHeaderCodeSegmentInfo Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] BssSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfoFlags
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserved;
        public byte Flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] RemasterVersion;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderCodeSegmentInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Address;
        public uint NumMaxPages;
        public uint CodeSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderDependencyList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public String8[] ProgramID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderSystemInfo
    {
        public uint SaveDataSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] JumpID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[] Reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderStorageInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ExtSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] SystemSaveDataID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] AccessInfo;
        public byte OtherAttributes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ResourceLimit
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXIExtendedHeaderArm11SystemLocalCaps
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ProgramID;
        public ulong Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public ResourceLimit[] ResourceLimitDescriptor;
        public CXIExtendedHeaderStorageInfo StorageInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public String8[] ServiceAccessControl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1f)]
        public byte[] Reserved;
        public byte ResourceLimitCategory;
    }

    public static class RomTool
    {
        public static RomContext Open(string path)
        {
            RomContext cxt = new RomContext();

            FileStream fs = File.OpenRead(path);
            cxt.cci = MarshalTool.ReadStruct<CCI>(fs);

            // Read the NCCHs
            cxt.cxis = new CXI[3];
            cxt.cxiprs = new CXIPlaingRegion[3];
            byte[] plainRegionBuffer;
            if (cxt.cci.FirstNCCHSize > 0)
            {
                fs.Seek(cxt.cci.FirstNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[0] = MarshalTool.ReadStruct<CXI>(fs);
                // get Plaing Region
                fs.Seek((cxt.cxis[0].PlainRegionOffset + cxt.cci.FirstNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[0].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[0] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
                // byte[] exh = new byte[2048];
                // fs.Read(exh, 0, exh.Length);
                // Array.Reverse(exh);
                // File.OpenWrite(path.Substring(path.LastIndexOf('\\') + 1) + "-rev.exh").Write(exh, 0, exh.Length);

            }
            if (cxt.cci.SecondNCCHSize > 0)
            {
                fs.Seek(cxt.cci.SecondNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[1] = MarshalTool.ReadStruct<CXI>(fs);
                // get Plaing Region
                fs.Seek((cxt.cxis[1].PlainRegionOffset + cxt.cci.SecondNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[1].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[1] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
            }
            if (cxt.cci.ThirdNCCHSize > 0)
            {
                fs.Seek(cxt.cci.ThirdNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[2] = MarshalTool.ReadStruct<CXI>(fs);
                // get Plaing Region
                fs.Seek((cxt.cxis[2].PlainRegionOffset + cxt.cci.ThirdNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[2].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[2] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
            }
            fs.Close();
            return cxt;
        }

        public enum RomView
        {
            NCSD,
            NCCH,
            NCCHPlainRegion
        };

        public static void View(frmExplorer f, RomContext cxt, RomView view)
        {
            int i = cxt.currentNcch;
            f.ClearInformation();
            switch (view)
            {
                case RomView.NCSD:
                    f.SetGroupHeaders("Hash", "NCSD");
                    f.AddListItem(0x000, 0x100, "RSA-2048 signature of the NCSD header [SHA-256]", cxt.cci.NCSDHeaderSignature, 0);
                    f.AddListItem(0x100, 4, "Magic ID, always 'NCSD'", cxt.cci.MagicID, 1);
                    f.AddListItem(0x104, 4, "Content size [medias]", cxt.cci.CCISize, 1);
                    f.AddListItem(0x108, 8, "Title/Program ID", cxt.cci.TitleID, 1);
                    f.AddListItem(0x120, 4, "Offset to the first NCCH [medias]", cxt.cci.FirstNCCHOffset, 1);
                    f.AddListItem(0x124, 4, "Size of the first NCCH [medias]", cxt.cci.FirstNCCHSize, 1);
                    f.AddListItem(0x130, 4, "Offset to the second NCCH [medias]", cxt.cci.SecondNCCHOffset, 1);
                    f.AddListItem(0x134, 4, "Size of the second NCCH [medias]", cxt.cci.SecondNCCHSize, 1);
                    f.AddListItem(0x158, 4, "Offset to the third NCCH [medias]", cxt.cci.ThirdNCCHOffset, 1);
                    f.AddListItem(0x15C, 4, "Size of the third NCCH [medias]", cxt.cci.ThirdNCCHSize, 1);
                    f.AddListItem(0x188, 8, "NCCH Flags", cxt.cci.NCCHFlags, 1);
                    f.AddListItem(0x190, 8, "Partition ID of the first NCCH", cxt.cci.FirstNCCHPartitionID, 1);
                    f.AddListItem(0x1A0, 8, "Partition ID of the second NCCH", cxt.cci.SecondNCCHPartitionID, 1);
                    f.AddListItem(0x1C8, 8, "Partition ID of the third NCCH", cxt.cci.ThirdNCCHPartitionID, 1);
                    f.AddListItem(0x200, 4, "Always 0xFFFFFFFF", cxt.cci.PaddingFF, 1);
                    f.AddListItem(0x300, 4, "Used ROM size [bytes]", cxt.cci.UsedRomSize, 1);
                    f.AddListItem(0x320, 16, "Unknown", cxt.cci.Unknown, 1);
                    break;
                case RomView.NCCH:
                    f.SetGroupHeaders("Hash", "NCCH");
                    f.AddListItem(0x000, 0x100, "RSA-2048 signature of the NCCH header [SHA-256]", cxt.cxis[i].NCCHHeaderSignature, 0);
                    f.AddListItem(0x100, 4, "Magic ID, always 'NCCH'", cxt.cxis[i].MagicID, 1);
                    f.AddListItem(0x104, 4, "Content size [medias]", cxt.cxis[i].CXISize, 1);

                    f.AddListItem(0x108, 8, "Partition ID", cxt.cxis[i].PartitionID, 1);
                    f.AddListItem(0x110, 2, "Maker Code" + " (=" + MakerResolver.Resolve(cxt.cxis[i].MakerCode) + ")", cxt.cxis[i].MakerCode, 1);
                    f.AddListItem(0x112, 2, "Version", cxt.cxis[i].Version, 1);
                    f.AddListItem(0x118, 8, "Program ID", cxt.cxis[i].ProgramID, 1);
                    f.AddListItem(0x120, 1, "Temp Flag", cxt.cxis[i].TempFlag, 1);
                    f.AddListItem(0x150, 0x10, "Product Code " + " (=" + GameTitleResolver.Resolve(cxt.cxis[i].ProductCode) + ")", cxt.cxis[i].ProductCode, 1);
                    f.AddListItem(0x160, 0x20, "Extended Header Hash", cxt.cxis[i].ExtendedHeaderHash, 1);
                    f.AddListItem(0x180, 4, "Extended header size", cxt.cxis[i].ExtendedHeaderSize, 1);
                    f.AddListItem(0x188, 8, "Flags", cxt.cxis[i].Flags, 1);
                    f.AddListItem(0x190, 4, "Plain region offset [medias]", cxt.cxis[i].PlainRegionOffset, 1);
                    f.AddListItem(0x194, 4, "Plain region size [medias]", cxt.cxis[i].PlainRegionSize, 1);
                    f.AddListItem(0x1A0, 4, "ExeFS offset [medias]", cxt.cxis[i].ExeFSOffset, 1);
                    f.AddListItem(0x1A4, 4, "ExeFS size [medias]", cxt.cxis[i].ExeFSSize, 1);
                    f.AddListItem(0x1A8, 4, "ExeFS hash region size [medias]", cxt.cxis[i].ExeFSHashRegionSize, 1);
                    f.AddListItem(0x1B0, 4, "RomFS offset [medias]", cxt.cxis[i].RomFSOffset, 1);
                    f.AddListItem(0x1B4, 4, "RomFS size [medias]", cxt.cxis[i].RomFSSize, 1);
                    f.AddListItem(0x1B8, 4, "RomFS hash region size [medias]", cxt.cxis[i].RomFSHashRegionSize, 1);
                    f.AddListItem(0x1C0, 0x20, "ExeFS superblock hash", cxt.cxis[i].ExeFSSuperBlockhash, 1);
                    f.AddListItem(0x1E0, 0x20, "RomFS superblock hash", cxt.cxis[i].RomFSSuperBlockhash, 1);
                    break;
                case RomView.NCCHPlainRegion:
                    f.SetGroupHeaders("Plain Regions");
                    for (int j = 0; j < cxt.cxiprs[i].PlainRegionStrings.Length; j++)
                        f.AddListItem(0, 4, cxt.cxiprs[i].PlainRegionStrings[j], (ulong)cxt.cxiprs[i].PlainRegionStrings[j].Length, 0);
                    break;
            }
            f.AutoAlignColumns();
        }
    }
}
