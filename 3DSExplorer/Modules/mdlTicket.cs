using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global
namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TimeLimitEntry
    {
        public uint EnableTimeLimit;
        public uint TimeLimitSeconds;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ticket
    {
        public SignatureType SignatureType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] Signature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C)]
        public byte[] Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public char[] Issuer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C)]
        public byte[] ECDSA;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3)]
        public byte[] Padding1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] EncryptedTitleKey;
        public byte Unknown0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] TicketID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ConsoleID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] TitleID;
        public ushort SystemAccess;
        public ushort TicketVersion;
        public uint PermittedTitlesMask;
        public uint PermitMask;
        public byte TitleExport;
        public byte CommonKeyIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[] Unknown1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public byte[] ContentPermissions;
        public ushort Padding2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public TimeLimitEntry[] TimeLimitEntries;
    }

    public static class TicketTool
    {
        public static Ticket OpenFromStream(FileStream fs, long offset)
        {
            Ticket tik = new Ticket();
            fs.Seek(offset, SeekOrigin.Begin);
            tik = MarshalTool.ReadStructBE<Ticket>(fs);
            //tik.TimeLimitEntries = new TimeLimitEntry[8];
            //for (int i=0; i<tik.TimeLimitEntries.Length; i++)
            //  tik.TimeLimitEntries[i] = MarshalTool.ReadStructBE<TimeLimitEntry>(fs);
            return tik;
        }

        public static void View(frmExplorer f, Ticket tik)
        {
            f.ClearInformation();
            f.SetGroupHeaders("Ticket", "Ticket Time Limits");
            f.AddListItem(0x000, 0x004, "Signature Type", (ulong)tik.SignatureType, 0);
            f.AddListItem(0x004, 0x100, "RSA-2048 signature of the Ticket", tik.Signature, 0);
            f.AddListItem(0x104, 0x03C, "Padding 0", tik.Padding0, 0);
            f.AddListItem(0x140, 0x040, "Issuer", tik.Issuer, 0);
            f.AddListItem(0x180, 0x03C, "ECDSA", tik.ECDSA, 0);
            f.AddListItem(0x1BC, 0x003, "Padding 1", tik.Padding1, 0);
            f.AddListItem(0x1BF, 0x010, "Encrypted Title Key", tik.EncryptedTitleKey, 0);
            f.AddListItem(0x1CF, 0x001, "Unknown 0", tik.Unknown0, 0);
            f.AddListItem(0x1D0, 0x008, "Ticket ID", tik.TicketID, 0);
            f.AddListItem(0x1D8, 0x004, "Console ID", tik.ConsoleID, 0);
            f.AddListItem(0x1DC, 0x008, "Title ID", tik.TitleID, 0);
            f.AddListItem(0x1E4, 0x002, "System Access", tik.SystemAccess, 0);
            f.AddListItem(0x1E6, 0x002, "Ticket Version", tik.TicketVersion, 0);
            f.AddListItem(0x1E8, 0x004, "Permitted Titles Mask", tik.PermittedTitlesMask, 0);
            f.AddListItem(0x1EC, 0x004, "Permit Mask", tik.PermitMask, 0);
            f.AddListItem(0x1F0, 0x001, "Title Export allowed using PRNG key", tik.TitleExport, 0);
            f.AddListItem(0x1F1, 0x001, "Common Key index (1=Korean,0=Normal)", tik.CommonKeyIndex, 0);
            f.AddListItem(0x1F2, 0x030, "Unknown1", tik.Unknown1, 0);
            f.AddListItem(0x222, 0x040, "Content access permissions (bit for each content)", tik.ContentPermissions, 0);
            f.AddListItem(0x262, 0x002, "Padding 2", tik.Padding2, 0);
            for (int i = 0; i < tik.TimeLimitEntries.Length; i++)
                f.AddListItem(0x264 + i * 8, 0x004, "Time Limit Enabled=" + tik.TimeLimitEntries[i].EnableTimeLimit + " For", tik.TimeLimitEntries[i].TimeLimitSeconds, 1);
            f.AutoAlignColumns();
        }
    }
}
// ReSharper enable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global