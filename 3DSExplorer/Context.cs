using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DSExplorer
{
    public static class Sizes
    {
        public const int SHA256 = 0x20;
        public const int SHA512 = 0x40;
        public const int SHA1 = 0x10;
        public const int MD5 = 0x10;
        public const int CRC16 = 0x02;
    }

    interface Context
    {
    }

    public class RomContext : Context
    {
        public CCI cci;
        public CXI[] cxis;
        public CXIPlaingRegion[] cxiprs;
        public int currentNcch;
    }

    public class Partition
    {
        public ulong OffsetInImage;

        public DIFI Difi;
        public IVFC Ivfc;
        public DPFS Dpfs;
        public byte[] Hash; //0x20 - SHA256

        public uint FirstFlag;
        public uint FirstFlagDupe;
        public uint SecondFlag;
        public uint SecondFlagDupe;
        /*
        public byte[] SecondFlagTable;
        public byte[] SecondFlagTableDupe;
        */
        public byte[][] HashTable;
         
    }

    public class SFContext : Context
    {
        public bool Encrypted;
        public bool FirstSave;

        //Wear-Level stuff

        public byte[] Key;

        public byte[] MemoryMap;
        public SFHeaderEntry[] Blockmap;
        public SFLongSectorEntry[] Journal;
        public uint JournalSize;
        public SFHeader fileHeader;
        public byte[] image;
        
        //Image stuff

        public bool isData;

        public byte[] ImageHash; //0x10 - ??
        public DISA Disa;
        
        public int currentPartition;
        public Partition[] Partitions;

        //SAVE Stuff
        public SAVE Save;
        public FileSystemFolderEntry[] Folders;
        public FileSystemFileEntry[] Files;
        public long fileBase;
        public uint[] FilesMap;
        public uint[] FoldersMap;
        public SFBlockMapEntry[] BlockMap;
    }

    public class TMDCertContext
    {
        
        public TMDCertificate cert;
        public TMDSignatureType SignatureType;
        public byte[] tmdSHA;
    }

    public class TMDContext : Context
    {
        public TMDHeader head;
        public TMDSignatureType SignatureType;
        public TMDContentInfoRecord[] ContentInfoRecords;
        public ArrayList chunks; // of TMDContentChunkRecord
        public byte[] tmdSHA;

        public ArrayList certs; //of TMDCertContext
    }

    public class CIAContext : Context
    {
        public CIAHeader header;
        public long CertificateChainOffset;
        public long TicketOffset;
        public long TMDOffset;
        public long BannerOffset;
        public long AppOffset;
        public TMDContext tmdContext;
    }
}
