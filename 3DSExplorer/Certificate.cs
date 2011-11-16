using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public Certificate cert;
        public SignatureType SignatureType;
        public byte[] Hash;
    }

    public static class CertTool
    {
        public static void OpenCertificatesFromStream(FileStream fs, long offset, long length, ArrayList list)
        {
            byte[] intBytes = new byte[4];
            fs.Seek(offset, SeekOrigin.Begin);
            //cxt.certs = new ArrayList();
            while ((fs.Position < offset + length) && (fs.Position < fs.Length))
            {
                CertificateEntry tcert = new CertificateEntry();
                fs.Read(intBytes, 0, 4);
                tcert.SignatureType = (SignatureType)BitConverter.ToInt32(intBytes, 0);
                // RSA Type
                if (tcert.SignatureType == SignatureType.RSA_2048_SHA256 || tcert.SignatureType == SignatureType.RSA_2048_SHA1)
                    tcert.Hash = new byte[256];
                else if (tcert.SignatureType == SignatureType.RSA_4096_SHA256 || tcert.SignatureType == SignatureType.RSA_4096_SHA1)
                    tcert.Hash = new byte[512];
                else
                    break; //no more certificates
                fs.Read(tcert.Hash, 0, tcert.Hash.Length);
                tcert.cert = MarshalTool.ReadStructBE<Certificate>(fs);
                list.Add(tcert);
            }
        }
    }
}
