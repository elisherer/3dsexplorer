using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
        public int Magic; //constant through the journal
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileSystemEntry
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
    public struct DISA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public int Unknown0;
        public long TableSize;
        public long PrimaryTableOffset;
        public long SecondaryTableOffset;
        public long TableLength;
        public long SAVEEntryOffset;
        public long SAVEEntryLength;
        public long DATAEntryOffset;
        public long DATAEntryLength;
        public long SAVEPartitionOffset;
        public long SAVEPartitionLength;
        public long DATAPartitionOffset;
        public long DATAPartitionLength;

        public uint ActiveTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Hash;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x74)]
        public byte[] Unknown7;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DIFI
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public int Unknown0;
        public long IVFCOffset;
        public long IVFCSize;
        public long DPFSOffset;
        public long DPFSSize;
        public long HashOffset;
        public long HashSize;
        public int Flags;
        public long FileBase;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVFC
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Unknown2_0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1C)]
        public byte[] Unknown2_1;
        public long HashTableOffset;
        public long HashTableLength;
        public long Unknown2_2;
        public long FileSystemOffset;
        public long FileSystemLength;
        public long HashedBlockLength; //shift
        public long Unknown3; //0x78
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DPFS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] DPFSData_0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1C)]
        public byte[] DPFSData_1;
        public long OffsetToNextPartition;
        public long DPFSUnknown;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAVE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;

        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint PartitionSize;

        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;
        public uint Unknown7;
        public uint Unknown8;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2C)]
        public byte[] Unknown9;
        
        
        public int LocalFileBaseOffset;
        public uint Unknown10;
        public uint Unknown11;
        public uint Unknown12;
        public uint Unknown13;
        public int FSTBlockOffset; //FST is in [BlockOffset] * 0x200 + [Offset]
        //or
        public uint Unknown14;
        public uint Unknown15; 
        public int FSTExactOffset;
    }

    public class SaveTool
    {
        [DllImport("msvcrt.dll")]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private struct HashEntry
        {
            public byte[] Hash;
            public int BlockIndex;
            public int Count;
        }

        public static bool isJournalMagic(byte[] buf, int offset)
        {
            return (buf[offset] == 0xE0 && buf[offset + 1] == 0x6C && buf[offset + 2] == 0x0D && buf[offset + 3] == 0x08);
        }

        public static bool isDifiMagic(char[] buf)
        {
            return (buf[0] == 'D' && buf[1] == 'I' && buf[2] == 'F' && buf[3] == 'I');
        }

        public static bool isDisaMagic(char[] buf)
        {
            return (buf[0] == 'D' && buf[1] == 'I' && buf[2] == 'S' && buf[3] == 'A');
        }

        public static bool isSaveMagic(char[] buf)
        {
            return (buf[0] == 'S' && buf[1] == 'A' && buf[2] == 'V' && buf[3] == 'E');
        }
        public static bool isSaveMagic(byte[] buf)
        {
            return (buf[0] == 'S' && buf[1] == 'A' && buf[2] == 'V' && buf[3] == 'E');
        }

        public static bool isFF(byte[] buf)
        {
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] != 0xFF)
                    return false;
            return true;
        }

        public static bool is00(byte[] buf)
        {
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] != 0x00)
                    return false;
            return true;
        }

        public static void XorByteArray(byte[] array, byte[] mask, int start)
        {
            for (int i = start; i < array.Length; i++)
                array[i] ^= mask[i % mask.Length];
        }

        public static void XorExperimental(byte[] array, byte[] mask, int start)
        {
            for (int j = start; j < array.Length; j += 0x200)
            {
                int lastNonFF = j + 0x200 - 1 < array.Length ? j + 0x200 - 1 : array.Length - 1;
                //find to what point need to be xored
                while (array[lastNonFF] == 0xFF)
                    lastNonFF--;
                //xor it
                for (int i = j; i <= lastNonFF; i++)
                    array[i] ^= mask[i % mask.Length];
            }
        }

        public static byte[] FindKey(byte[] input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            int count = 0, rec_idx = 0, rec_count = 0;
            bool found;
            HashEntry[] hash_list;
            byte[] outbuf;

            byte[] hash;
            byte[] ff_hash = new byte[] { 0xde, 0x03, 0xfe, 0x65, 0xa6, 0x76, 0x5c, 0xaa, 0x8c, 0x91, 0x34, 0x3a, 0xcc, 0x62, 0xcf, 0xfc };

            hash_list = new HashEntry[4 * ((input.Length / 0x200) + 1)];

            for (int i = 0; i < (input.Length / 0x200); i++)
            {
                hash = md5.ComputeHash(input, i * 0x200, 0x200);

                if (memcmp(hash, ff_hash, 16) == 0) //skip ff blocks...
                    continue;

                found = false;
                // see if we already came up with that hash
                for (int j = 0; j < count; j++)
                    if (memcmp(hash_list[j].Hash, hash, 16) == 0)
                    {
                        hash_list[j].Count++;
                        found = true;
                        break;
                    }

                // push new hashlist entry
                if (!found)
                {
                    hash_list[count] = new HashEntry();
                    hash_list[count].Hash = new byte[hash.Length];
                    Buffer.BlockCopy(hash, 0, hash_list[count].Hash, 0, hash.Length);
                    hash_list[count].Count = 1;
                    hash_list[count].BlockIndex = i;
                    count++;
                }
            }
            // find the most common hash
            for (int i = 0; i < count; i++)
                if (hash_list[i].Count > rec_count)
                {
                    rec_count = hash_list[i].Count;
                    rec_idx = i;
                }

            if (rec_count == 0)
                return null;

            outbuf = new byte[0x200];
            Buffer.BlockCopy(input, hash_list[rec_idx].BlockIndex * 0x200, outbuf, 0, 0x200);

            return outbuf;
        }



        public static byte[] createSAV(SFContext cxt)
        {
            //Recompute the partitions' hash tables
            HashAlgorithm ha = SHA256.Create();
            int offset, hashSize;
            for (int i = 0; i < cxt.Partitions.Length; i++)
            {
                // itrerate thorugh the hashes table
                hashSize = (1 << (int)cxt.Partitions[i].Ivfc.HashedBlockLength);
                for (int j = 0; j < cxt.Partitions[i].HashTable.Length; j++)
                    if (!is00(cxt.Partitions[i].HashTable[j])) //hash isn't zero
                    {
                        offset = (int)(cxt.Partitions[i].offsetInImage + cxt.Partitions[i].Ivfc.FileSystemOffset);
                        offset += j * hashSize;
                        cxt.Partitions[i].HashTable[j] = ha.ComputeHash(cxt.image, offset, hashSize);
                        //write it into the image
                        offset = (int)(cxt.Partitions[i].offsetInImage + cxt.Partitions[i].Ivfc.HashTableOffset);
                        offset += j * Partition.HASH_LENGTH;
                        Buffer.BlockCopy(cxt.Partitions[i].HashTable[j], 0, cxt.image, offset, Partition.HASH_LENGTH);
                    }
            }

            /*TODO:
            
            [ ] (Unknwon) Make the 'Partition Hash Table Header'
            [ ] (Unknown) Correct The Partition table hashes (DIFIs).
            [ ] (Partly) Hash the proper partition table into the DISA struct.
            
            */

            byte[] temp;
            MemoryStream ms = new MemoryStream();

            //Write the file header
            temp = MarshalTool.StructureToByteArray<SFHeader>(cxt.fileHeader);
            ms.Write(temp, 0, temp.Length);

            //Update & Write the blockmap (straight)
            for (byte i = 0; i < cxt.MemoryMap.Length ; i++)
                cxt.MemoryMap[i] = i;
            cxt.Journal = new SFLongSectorEntry[0];
            cxt.JournalSize = 0;
            int blockmapEntrySize = Marshal.SizeOf(typeof(SFHeaderEntry));
            byte[] blockmapBlock = new byte[blockmapEntrySize * cxt.Blockmap.Length];
            for (byte i = 0; i < cxt.Blockmap.Length; i++)
            {
                cxt.Blockmap[i].AllocationCount = 0;
                cxt.Blockmap[i].PhysicalSector = (byte)(i + 0x81); //checksum flag + 1 (offset from file header)
                for (int j = 0; j < cxt.Blockmap[i].CheckSums.Length; j++)
                    cxt.Blockmap[i].CheckSums[j] = CRC16.CS(CRC16.GetCRC(cxt.image, 0x1000 * i + 0x200 * j, 0x200));
                temp = MarshalTool.StructureToByteArray<SFHeaderEntry>(cxt.Blockmap[i]);
                Buffer.BlockCopy(temp,0,blockmapBlock, i * blockmapEntrySize,blockmapEntrySize);
            }
            ms.Write(blockmapBlock, 0, blockmapBlock.Length);

            //Write the CRC16
            ms.Write(CRC16.GetCRC(blockmapBlock), 0, 2);

            //Write an empty journal
            while (ms.Position < 0x1000)
                ms.WriteByte(0xFF);

            //Write the image
            ms.Write(cxt.image, 0, cxt.image.Length);
            byte[] buffer = ms.ToArray();

            //XOR with the key
            XorByteArray(buffer, cxt.Key, 0x1000);
            
            return buffer;
        }
    }
}
