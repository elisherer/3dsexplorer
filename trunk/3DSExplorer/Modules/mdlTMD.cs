using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global
namespace _3DSExplorer
{
    public class TMDContext : IContext
    {
        public TMDHeader Head;
        public SignatureType SignatureType;
        public TMDContentInfoRecord[] ContentInfoRecords;
        public TMDContentChunkRecord[] Chunks;
        public byte[] Hash;
        public ArrayList Certificates; //of CertificateEntry
    }

    public enum SignatureType
    {
        RSA_2048_SHA256 = 0x04000100,
        RSA_4096_SHA256 = 0x03000100,
        RSA_2048_SHA1   = 0x01000100,
        RSA_4096_SHA1   = 0x00000100
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMDHeader
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
        public uint TitleType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] GroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
        public byte[] Reserved2;
        public uint AccessRights;
        public ushort TitleVersion;
        public ushort ContentCount;
        public ushort BootContent;
        public ushort Padding0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentInfoRecordsHash;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMDContentInfoRecord
    {
        public ushort ContentIndexOffset;
        public ushort ContentCommandCount; //K
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] NextContentHash; //SHA-256 hash of the next k content records that have not been hashed yet
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TMDContentChunkRecord
    {
        public uint ContentID;
        public ushort ContentIndex;
        public ushort ContentType;
        public ulong ContentSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ContentHash; //SHA-256
    }

    public class TMDTool
    {
        private static string TypeToString(ushort type)
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

        public static TMDContext Open(string path)
        {
            var fs = File.OpenRead(path);
            var cxt = OpenFromStream(fs, 0, fs.Length);
            if (cxt != null)
            {
                cxt.Certificates = new ArrayList();
                CertTool.OpenCertificatesFromStream(fs, fs.Position, fs.Length, cxt.Certificates);
            }
            fs.Close();
            return cxt;
        }

        public static TMDContext OpenFromStream(FileStream fs, long offset, long tmdLength)
        {
            try
            {
                var cxt = new TMDContext();

                fs.Seek(offset, SeekOrigin.Begin);

                var supported = true;

                var intBytes = new byte[4];
                fs.Read(intBytes, 0, 4);
                cxt.SignatureType = (SignatureType) BitConverter.ToInt32(intBytes, 0);
                // Read the TMD RSA Type 
                if (cxt.SignatureType == SignatureType.RSA_2048_SHA256)
                    cxt.Hash = new byte[256];
                else if (cxt.SignatureType == SignatureType.RSA_4096_SHA256)
                    cxt.Hash = new byte[512];
                else
                    supported = false;
                if (supported)
                {
                    fs.Read(cxt.Hash, 0, cxt.Hash.Length);
                    //Continue reading header
                    cxt.Head = MarshalTool.ReadStructBE<TMDHeader>(fs); //read header
                    cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                    for (var i = 0; i < cxt.ContentInfoRecords.Length; i++)
                        cxt.ContentInfoRecords[i] = MarshalTool.ReadStructBE<TMDContentInfoRecord>(fs);
                    cxt.Chunks = new TMDContentChunkRecord[cxt.Head.ContentCount]; // new ArrayList();
                    for (var i = 0; i < cxt.Head.ContentCount; i++)
                        cxt.Chunks[i] = MarshalTool.ReadStructBE<TMDContentChunkRecord>(fs);
                }
                return (supported ? cxt : null);
            }
            catch
            {
                return null;
            }
        }

        public enum TMDView{
            TMD,
            ContentInfoRecord,
            ContentChunkRecord
        };

        public static void View(frmExplorer f, TMDContext cxt, TMDView view)
        {
            f.ClearInformation();
            switch (view)
            {
                case TMDView.TMD:
                    TMDHeader head = cxt.Head;
                    f.SetGroupHeaders("TMD");
                    f.AddListItem(0, 4, "Signature Type", (ulong)cxt.SignatureType, 0);
                    int off = 4;
                    if (cxt.SignatureType == SignatureType.RSA_2048_SHA256 || cxt.SignatureType == SignatureType.RSA_2048_SHA1)
                    {
                        f.AddListItem(off, 0x100, "RSA-2048 signature of the TMD", cxt.Hash, 0);
                        off += 0x100;
                    }
                    else
                    {
                        f.AddListItem(off, 0x200, "RSA-4096 signature of the TMD", cxt.Hash, 0);
                        off += 0x200;
                    }
                    f.AddListItem(off, 60, "Reserved0", head.Reserved0, 0);
                    f.AddListItem(off + 60, 64, "Issuer", head.Issuer, 0);
                    f.AddListItem(off + 124, 4, "Version", head.Version, 0);
                    f.AddListItem(off + 128, 1, "Car Crl Version", head.CarCrlVersion, 0);
                    f.AddListItem(off + 129, 1, "Signer Version", head.SignerVersion, 0);
                    f.AddListItem(off + 130, 1, "Reserved1", head.Reserved1, 0);
                    f.AddListItem(off + 131, 8, "System Version", head.SystemVersion, 0);
                    f.AddListItem(off + 139, 8, "Title ID", head.TitleID, 0);
                    f.AddListItem(off + 147, 4, "Title Type", head.TitleType, 0);
                    f.AddListItem(off + 151, 2, "Group ID", head.GroupID, 0);
                    f.AddListItem(off + 153, 62, "Reserved2", head.Reserved2, 0);
                    f.AddListItem(off + 215, 4, "Access Rights", head.AccessRights, 0);
                    f.AddListItem(off + 219, 2, "Title Version", head.TitleVersion, 0);
                    f.AddListItem(off + 221, 2, "Content Count", head.ContentCount, 0);
                    f.AddListItem(off + 223, 2, "Boot Content", head.BootContent, 0);
                    f.AddListItem(off + 225, 2, "Padding", head.Padding0, 0);
                    f.AddListItem(off + 227, 32, "Content Info Records Hash", head.ContentInfoRecordsHash, 0);
                    break;
                case TMDView.ContentInfoRecord:
                    f.SetGroupHeaders("TMD Content Records");
                    for (var i = 0; i < 64; i++)
                    {
                        f.AddListItem(i * 36, 2, "Content Command Count", cxt.ContentInfoRecords[i].ContentCommandCount, 0);
                        f.AddListItem(i * 36 + 2, 2, "Content Index Offset", cxt.ContentInfoRecords[i].ContentIndexOffset, 0);
                        f.AddListItem(i * 36 + 4, 32, "Next Content Hash", cxt.ContentInfoRecords[i].NextContentHash, 0);
                    }
                    break;
                case TMDView.ContentChunkRecord:
                    f.SetGroupHeaders("TMD Content Chunks");
                    for (var i = 0; i < cxt.Chunks.Length; i++)
                    {
                        f.AddListItem(i, 4, "Content ID", cxt.Chunks[i].ContentID, 0);
                        f.AddListItem(0, 2, "Content Index", cxt.Chunks[i].ContentIndex, 0);
                        f.AddListItem(0, 2, "Content Type (=" + TypeToString(cxt.Chunks[i].ContentType) + ")", cxt.Chunks[i].ContentType, 0);
                        f.AddListItem(0, 8, "Content Size", cxt.Chunks[i].ContentSize, 0);
                        f.AddListItem(0, 32, "Content Hash", cxt.Chunks[i].ContentHash, 0);
                    }
                    break;
            }
            f.AutoAlignColumns();
        }

    }
}
// ReSharper enable MemberCanBePrivate.Global, FieldCanBeMadeReadOnly.Global, UnusedMember.Global, NotAccessedField.Global, ClassNeverInstantiated.Global