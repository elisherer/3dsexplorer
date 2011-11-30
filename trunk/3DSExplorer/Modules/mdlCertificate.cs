using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global
namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Certificate
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

    public class CertificateEntry
    {
        public Certificate Certificate;
        public SignatureType SignatureType;
        public byte[] Hash;
    }

    public static class CertTool
    {
        public static void OpenCertificatesFromStream(FileStream fs, long offset, long length, ArrayList list)
        {
            try
            {

                var intBytes = new byte[4];
                fs.Seek(offset, SeekOrigin.Begin);
                while ((fs.Position < offset + length) && (fs.Position < fs.Length))
                {
                    var tcert = new CertificateEntry();
                    fs.Read(intBytes, 0, 4);
                    tcert.SignatureType = (SignatureType) BitConverter.ToInt32(intBytes, 0);
                    // RSA Type
                    switch (tcert.SignatureType)
                    {
                        case SignatureType.RSA_2048_SHA1:
                        case SignatureType.RSA_2048_SHA256:
                            tcert.Hash = new byte[256];
                            break;
                        case SignatureType.RSA_4096_SHA1:
                        case SignatureType.RSA_4096_SHA256:
                            tcert.Hash = new byte[512];
                            break;
                    }
                    fs.Read(tcert.Hash, 0, tcert.Hash.Length);
                    tcert.Certificate = MarshalTool.ReadStructBE<Certificate>(fs);
                    list.Add(tcert);
                }
            }
            catch
            {
                list.Clear();
            }
        }

        public static void View(frmExplorer f, ArrayList certs, int i)
        {
            f.ClearInformation();
            if (i < 0)
            {
                f.SetGroupHeaders("Certificates");
                f.AddListItem(0, 4, "Certificate Count", (ulong)certs.Count, 0);
            }
            else
            {
                var entry = (CertificateEntry)certs[i];
                var cert = entry.Certificate;
                f.SetGroupHeaders("Certificate");
                f.AddListItem(0, 4, "Signature Type", (ulong)entry.SignatureType, 0);
                int off = 4;
                if (entry.SignatureType == SignatureType.RSA_2048_SHA256 || entry.SignatureType == SignatureType.RSA_2048_SHA1)
                {
                    f.AddListItem(off, 0x100, "RSA-2048 signature of the content", entry.Hash, 0);
                    off += 0x100;
                }
                else
                {
                    f.AddListItem(off, 0x200, "RSA-4096 signature of the content", entry.Hash, 0);
                    off += 0x200;
                }
                f.AddListItem(off, 60, "Reserved0", cert.Reserved0, 0);
                f.AddListItem(off + 60, 64, "Issuer", cert.Issuer, 0);
                f.AddListItem(off + 124, 4, "Tag", cert.Tag, 0);
                f.AddListItem(off + 128, 64, "Name", cert.Name, 0);
                f.AddListItem(off + 292, 0x104, "Key", cert.Key, 0);
                f.AddListItem(off + 552, 2, "Unknown0", cert.Unknown1, 0);
                f.AddListItem(off + 554, 2, "Unknown1", cert.Unknown2, 0);
                f.AddListItem(off + 556, 52, "Padding", cert.Padding, 0);
            }
            f.AutoAlignColumns();
        }
    }
}
// ReSharper enable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global