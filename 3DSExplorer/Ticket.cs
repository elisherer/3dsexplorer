using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
    }
}
