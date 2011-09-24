using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DSExplorer
{
    interface Context
    {
    }

    class CCIContext : Context
    {
        public CCI cci;
        public CXI[] cxis;
        public CXIPlaingRegion[] cxiprs;
        public int currentNcch;
    }

    class SFContext : Context
    {
        public int JournalMagic = 0x080D6CE0;
        public uint SaveFSEMagic = 0xD57B1100;

        public byte[] Key;

        public int BlockmapLength;
        public int JournalSize;
        public SFHeader fileHeader;
        public byte[] image;
        public SFImageHeader imageHeader;
        public int currentDifi;
        public ArrayList difis; // of SFDIFIBlob
        public ArrayList saves; // of SFSave
        public ArrayList savesFiles; // of SFFileSystemEntry
    }

    class TMDCertContext
    {
        
        public TMDCertificate cert;
        public TMDSignatureType SignatureType;
        public byte[] tmdSHA;
    }

    class TMDContext : Context
    {
        public TMDHeader head;
        public TMDSignatureType SignatureType;
        public TMDContentInfoRecord[] ContentInfoRecords;
        public ArrayList chunks; // of TMDContentChunkRecord
        public byte[] tmdSHA;

        public ArrayList certs; //of TMDCertContext
    }
}
