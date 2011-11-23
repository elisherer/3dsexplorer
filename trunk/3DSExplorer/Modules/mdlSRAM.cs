using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace _3DSExplorer
{

    public class SRAMContext : Context
    {
        public bool Encrypted;
        public bool FirstSave;

        //Wear-Level stuff

        public byte[] Key;

        public byte[] MemoryMap;
        public SRAMHeaderEntry[] Blockmap;
        public SRAMLongSectorEntry[] Journal;
        public uint JournalSize;
        public SRAMHeader fileHeader;
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
        public SRAMBlockMapEntry[] BlockMap;
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

    public static class Sizes
    {
        public const int SHA256 = 0x20;
        public const int SHA512 = 0x40;
        public const int SHA1 = 0x10;
        public const int MD5 = 0x10;
        public const int CRC16 = 0x02;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SRAMHeader
    {
        public uint Unknown1;
        public uint Unknown2;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SRAMHeaderEntry
    {
        public byte PhysicalSector; // when bit7 is set, block has checksums, otherwise checksums are all zero
        public byte AllocationCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] CheckSums; // 8*0x200=0x1000, each byte hashes 0x200 block with ModbusCRC16 XORed to 1 byte
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SRAMSectorEntry
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
    public struct SRAMLongSectorEntry
    {
        public SRAMSectorEntry Sector;
        public SRAMSectorEntry Dupe;
        public uint Magic; //constant through the journal
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SRAMBlockMapEntry
    {
        public uint StartBlock;
        public uint EndBlock;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileSystemFolderEntry
    {
        public uint ParentFolderIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public char[] FolderName;
        public uint Index;
        public uint Unknown1;
        public uint LastFileIndex;
        public uint Unknown2;
        public uint Unknown3;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileSystemFileEntry
    {
        public uint ParentFolderIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public char[] Filename;
        public uint Index;
        public uint Magic;
        public uint BlockOffset;
        public ulong FileSize;
        public uint Unknown2; // flags and/or date?
        public uint Unknown3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DISA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Unknown0;
        public ulong TableSize;
        public ulong PrimaryTableOffset;
        public ulong SecondaryTableOffset;
        public ulong TableLength;
        public ulong SAVEEntryOffset;
        public ulong SAVEEntryLength;
        public ulong DATAEntryOffset;
        public ulong DATAEntryLength;
        public ulong SAVEPartitionOffset;
        public ulong SAVEPartitionLength;
        public ulong DATAPartitionOffset;
        public ulong DATAPartitionLength;

        public uint ActiveTable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Hash;

        public uint ZeroPad0;
        public uint Flag0;
        public uint Unknown1;
        public uint ZeroPad1; 
        public uint Unknown2; //Magic
        public ulong DataFsLength; //Why??
        public ulong Unknown3;
        public uint Unknown4; 
        public uint Unknown5; 
        public uint Unknown6;
        public uint Unknown7;
        public uint Unknown8;
        public uint Flag1;
        public uint Flag2;
        public uint Flag3;
        public uint Flag4;
        public uint Unknown14;
        public uint Flag5;
        public uint Unknown16;
        public uint Magic17;
        public uint Unknown18;
        public uint Flag6;
        public uint Flag7;
        public uint Flag8;
        public uint Unknown21;
        public uint Unknown22;
        public uint Unknown23;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DIFI
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint MagicPadding;
        public ulong IVFCOffset;
        public ulong IVFCSize;
        public ulong DPFSOffset;
        public ulong DPFSSize;
        public ulong HashOffset;
        public ulong HashSize;
        public uint Flags;
        public ulong FileBase;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVFC
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint MagicPadding;
        public ulong Unknown1;

        public ulong FirstHashOffset;
        public ulong FirstHashLength;
        public ulong FirstHashBlock;
        public ulong SecondHashOffset;
        public ulong SecondHashLength;
        public ulong SecondHashBlock;

        public ulong HashTableOffset;
        public ulong HashTableLength;
        public ulong HashTableBlock;
        public ulong FileSystemOffset;
        public ulong FileSystemLength;
        public ulong FileSystemBlock;
        public ulong Unknown3; //0x78
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DPFS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint MagicPadding;

        public ulong FirstTableOffset;
        public ulong FirstTableLength;
        public ulong FirstTableBlock;
        public ulong SecondTableOffset;
        public ulong SecondTableLength;
        public ulong SecondTableBlock;
        public ulong OffsetToData;
        public ulong DataLength;
        public ulong DataBlock;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAVE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;

        public uint MagicPadding;
        public ulong Unknown1;
        public ulong PartitionSize;
        public uint PartitionMediaSize;
        public ulong Unknown3;
        public uint Unknown4;
        public ulong FolderMapOffset;
        public uint FolderMapSize;
        public uint FolderMapMediaSize; 
        public ulong FileMapOffset;
        public uint FileMapSize;
        public uint FileMapMediaSize;
        public ulong BlockMapOffset;
        public uint BlockMapSize;
        public uint BlockMapMediaSize;
        public ulong FileStoreOffset;
        public uint FileStoreLength;
        public uint FileStoreMedia;
        public uint FolderTableOffset;
        public uint FolderTableLength;
        public uint FolderTableUnknown;
        public uint FolderTableMedia; 
        public uint FSTOffset;
        public uint FSTLength;
        public uint FSTUnknown;
        public uint FSTMedia;
    }

    public class SRAMTool
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

        public static byte[] FindKey2(byte[] input)
        {
            MemoryStream ms = new MemoryStream(input);
            byte[] disa = new byte[4];
            ms.Seek(0x100,SeekOrigin.Begin);
            ms.Read(disa, 0, disa.Length);
            ms.Seek(0x200, SeekOrigin.Current);
            byte[] check = new byte[4];
            while (ms.Position < ms.Length)
            {
                ms.Read(check, 0, check.Length);
                XorByteArray(check, disa, 0);
                if (check[0] == 'D' && check[1] == 'I' && check[2] == 'S' && check[3] == 'A')
                {
                    ms.Seek(-0x104, SeekOrigin.Current);
                    byte[] key = new byte[0x200];
                    ms.Read(key, 0, key.Length);
                    return key;
                }
                ms.Seek(0x200, SeekOrigin.Current);
            }
            return null; //key not found
        }

        public static byte[] MakeKey(byte[] input)
        {
            byte[] keyArray = new byte[0x200];
            
            //copy from 0x0010
            for (int i = 0x10; i < 0x100; i++)
                keyArray[i] = (byte)(input[i] ^ 0);
            //copy from 0x0100
            byte[] x00100 = new byte[] { 0x44, 0x49, 0x53, 0x41, 00, 00, 04, 00 };
            for (int i = 0; i < 8; i++)
                keyArray[0x100+i] = (byte)(input[0x100+i] ^ x00100[i]);
            //copy from 0x1000
            byte[] x01000 = new byte[] { 0, 0, 0, 8 };
            for (int i = 0; i < 4; i++)
                keyArray[i] = (byte)(input[0x1000 + i] ^ x01000[i]);
            //find where SAVE is
            int saveOffset = 0x2400;
            if ((input[saveOffset] ^ keyArray[0]) != 'S' ||
                (input[saveOffset + 1] ^ keyArray[1]) != 'A' ||
                (input[saveOffset + 2] ^ keyArray[2]) != 'S' ||
                (input[saveOffset + 3] ^ keyArray[3]) != 'S')
                saveOffset += 0xC00;
            //copy from SAVE
            byte[] xSave = new byte[] { 00, 00, 04, 00, 0x20, 00, 00, 00, 00, 00, 00, 00};
            for (int i = 0x04; i < 0x10; i++)
                keyArray[i] = (byte)(input[saveOffset + i] ^ xSave[i - 0x04]);
            
            return keyArray;
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

            hash_list = new HashEntry[(input.Length / 0x200) + 1];

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

            //final DISA check
            int foundOffset = hash_list[rec_idx].BlockIndex * 0x200;
            if (((input[0x100] ^ input[foundOffset + 0x100]) != 'D') ||
                ((input[0x101] ^ input[foundOffset + 0x101]) != 'I') ||
                ((input[0x102] ^ input[foundOffset + 0x102]) != 'S') ||
                ((input[0x103] ^ input[foundOffset + 0x103]) != 'A'))
                return null; //That's not it

            outbuf = new byte[0x200];
            Buffer.BlockCopy(input, foundOffset, outbuf, 0, 0x200);

            return outbuf;
        }

        public static bool isEncrypted(string path)
        {
            //check if encrypted
            FileStream fs = File.OpenRead(path);
            byte[] magic = new byte[4];
            fs.Seek(0x1000, SeekOrigin.Begin); //Start of information
            while ((fs.Length - fs.Position > 0x200) & !SRAMTool.isSaveMagic(magic))
            {
                fs.Read(magic, 0, 4);
                fs.Seek(0x200 - 4, SeekOrigin.Current);
            }
            long result = fs.Length - fs.Position;
            fs.Close();
            return (result <= 0x200);
        }

        public static byte[] ReadByteArray(Stream fs, int size)
        {
            byte[] buffer = new byte[size];
            fs.Read(buffer, 0, size);
            return buffer;
        }

        public static uint ReadUInt32(Stream fs)
        {
            byte[] buffer = new byte[4];
            fs.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static ulong ReadUInt64(Stream fs)
        {
            byte[] buffer = new byte[8];
            fs.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static SRAMContext Open(string path, ref string errorMessage)
        {
            SRAMContext cxt = new SRAMContext();

            cxt.Encrypted = isEncrypted(path);

            //get the file into buffer to find the key if needed
            byte[] fileBuffer = File.ReadAllBytes(path);
            MemoryStream ms = new MemoryStream(fileBuffer);

            cxt.fileHeader = MarshalTool.ReadStruct<SRAMHeader>(ms);

            //get the blockmap headers
            int bmSize = (int)(ms.Length >> 12) - 1;
            cxt.Blockmap = new SRAMHeaderEntry[bmSize];
            cxt.MemoryMap = new byte[bmSize];
            for (int i = 0; i < cxt.Blockmap.Length; i++)
            {
                cxt.Blockmap[i] = MarshalTool.ReadStruct<SRAMHeaderEntry>(ms);
                cxt.MemoryMap[i] = cxt.Blockmap[i].PhysicalSector;
            }
            //Check crc16
            byte[] twoBytes = new byte[2], crcBytes = new byte[2];
            ms.Read(crcBytes, 0, 2);
            twoBytes = CRC16.GetCRC(fileBuffer, 0, ms.Position - 2);
            if (crcBytes[0] != twoBytes[0] || crcBytes[1] != twoBytes[1])
            {
                errorMessage = "CRC Error or Corrupt Save file.";
                ms.Close();
                return null;
            }
            else
            {
                //get journal updates
                int jSize = (int)(0x1000 - ms.Position) / Marshal.SizeOf(typeof(SRAMLongSectorEntry));
                cxt.Journal = new SRAMLongSectorEntry[jSize];
                cxt.JournalSize = 0;
                uint jc = 0;
                while (ms.Position < 0x1000) //assure stopping
                {
                    cxt.Journal[jc] = MarshalTool.ReadStruct<SRAMLongSectorEntry>(ms);
                    if (!SRAMTool.isFF(cxt.Journal[jc].Sector.CheckSums)) //check if we got a valid checksum
                    {
                        cxt.MemoryMap[cxt.Journal[jc].Sector.VirtualSector] = cxt.Journal[jc].Sector.PhysicalSector;
                        jc++;
                    }
                    else //if not then it's probably the end of the journal
                        break;
                }
                cxt.JournalSize = jc;

                //rearragne by virtual
                cxt.image = new byte[fileBuffer.Length - 0x1000];
                for (int i = 0; i < cxt.MemoryMap.Length; i++)
                    Buffer.BlockCopy(fileBuffer, (cxt.MemoryMap[i] & 0x7F) * 0x1000, cxt.image, i * 0x1000, 0x1000);

                if (cxt.Encrypted)
                {
                    byte[] key = FindKey(cxt.image);
                    if (key == null)
                    {
                        ms.Close();
                        key = MakeKey(cxt.image);
                        File.WriteAllBytes(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_img.bin", cxt.image);
                        XorByteArray(cxt.image, key, 0);
                        File.WriteAllBytes(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_dec.bin", cxt.image);
                        File.WriteAllBytes(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_key.bin", key);
                        errorMessage = "Can't find key in binary file." + Environment.NewLine + 
                                        "Tried to create a key and saved the binaries to " + Environment.NewLine + 
                                        Environment.NewLine +
                                        "_img, _dec & _key";
                        return null;
                    }
                    else
                    {
                        XorByteArray(cxt.image, key, 0);
                        //XorExperimental(fileBuffer, key, 0x1000);
                        cxt.Key = key;
                    }
                }

                /*
                if ((cxt.image[0x100] != 'D') || (cxt.image[0x101] != 'I') || (cxt.image[0x102] != 'S') || (cxt.image[0x103] != 'A'))
                {   //Might be second encryption
                    byte[] key = FindKey(fileBuffer);
                    if (key != null)
                    {
                        XorByteArray(cxt.image, key, 0);
                        cxt.Key = key;
                    }
                    File.WriteAllBytes("image.bin", cxt.image);
                }*/

                MemoryStream ims = new MemoryStream(cxt.image);
                cxt.ImageHash = ReadByteArray(ims, Sizes.MD5);
                //Go to start of image
                ims.Seek(0x100, SeekOrigin.Begin);
                cxt.Disa = MarshalTool.ReadStruct<DISA>(ims);
                cxt.isData = cxt.Disa.TableSize > 1;
                if (!SRAMTool.isDisaMagic(cxt.Disa.Magic))
                {
                    errorMessage = "Corrupt Save File!";
                    ms.Close();
                    ims.Close();
                    return null;
                }
                //Which table to read
                if ((cxt.Disa.ActiveTable & 1) == 1) //second table
                    ims.Seek((long)cxt.Disa.PrimaryTableOffset, SeekOrigin.Begin);
                else
                    ims.Seek((long)cxt.Disa.SecondaryTableOffset, SeekOrigin.Begin);

                cxt.Partitions = new Partition[cxt.Disa.TableSize];
                for (int i = 0; i < cxt.Partitions.Length; i++)
                {
                    long startOfDifi = ims.Position;
                    cxt.Partitions[i] = new Partition();
                    cxt.Partitions[i].Difi = MarshalTool.ReadStruct<DIFI>(ims);
                    //ims.Seek(startOfDifi + cxt.Partitions[i].Difi.IVFCOffset, SeekOrigin.Begin);
                    cxt.Partitions[i].Ivfc = MarshalTool.ReadStruct<IVFC>(ims);
                    //ims.Seek(startOfDifi + cxt.Partitions[i].Difi.DPFSOffset, SeekOrigin.Begin);
                    cxt.Partitions[i].Dpfs = MarshalTool.ReadStruct<DPFS>(ims);
                    //ims.Seek(startOfDifi + cxt.Partitions[i].Difi.HashOffset, SeekOrigin.Begin);
                    cxt.Partitions[i].Hash = ReadByteArray(ims, Sizes.SHA256);
                    ims.Seek(4, SeekOrigin.Current); // skip garbage
                }

                for (int p = 0; p < cxt.Partitions.Length; p++)
                {
                    if (p == 0)
                        ims.Seek((long)cxt.Disa.SAVEPartitionOffset, SeekOrigin.Begin);
                    else
                        ims.Seek((long)cxt.Disa.DATAPartitionOffset, SeekOrigin.Begin);

                    cxt.Partitions[p].OffsetInImage = (ulong)ims.Position;

                    ims.Seek((long)cxt.Partitions[p].Dpfs.FirstTableOffset, SeekOrigin.Current);
                    cxt.Partitions[p].FirstFlag = ReadUInt32(ims);
                    cxt.Partitions[p].FirstFlagDupe = ReadUInt32(ims);
                    cxt.Partitions[p].SecondFlag = ReadUInt32(ims);
                    ims.Seek((long)cxt.Partitions[p].Dpfs.SecondTableLength - 4, SeekOrigin.Current);
                    cxt.Partitions[p].SecondFlagDupe = ReadUInt32(ims);
                    /*
                    cxt.Partitions[p].FirstFlagTableDupe = new byte[cxt.Partitions[p].Dpfs.FirstTableLength / 4];
                    for (int i = 0; i < cxt.Partitions[p].FirstFlagTableDupe.Length; i++)
                        cxt.Partitions[p].FirstFlagTableDupe[i] = ReadUInt32(ims);
                    cxt.Partitions[p].SecondFlagTable = new byte[cxt.Partitions[p].Dpfs.SecondTableLength / 4];
                    for (int i = 0; i < cxt.Partitions[p].SecondFlagTable.Length; i++)
                        cxt.Partitions[p].SecondFlagTable[i] = ReadUInt32(ims);
                    cxt.Partitions[p].SecondFlagTableDupe = new byte[cxt.Partitions[p].Dpfs.SecondTableLength / 4];
                    for (int i = 0; i < cxt.Partitions[p].SecondFlagTableDupe.Length; i++)
                        cxt.Partitions[p].SecondFlagTableDupe[i] = ReadUInt32(ims); 
                    */

                    ims.Seek((long)(cxt.Partitions[p].OffsetInImage + cxt.Partitions[p].Dpfs.OffsetToData), SeekOrigin.Begin);

                    //Get hashes table
                    ims.Seek((long)cxt.Partitions[p].Ivfc.HashTableOffset, SeekOrigin.Current);
                    cxt.Partitions[p].HashTable = new byte[cxt.Partitions[p].Ivfc.HashTableLength / 0x20][];
                    for (int i = 0; i < cxt.Partitions[p].HashTable.Length; i++)
                        cxt.Partitions[p].HashTable[i] = ReadByteArray(ims, 0x20);

                    ims.Seek((long)(cxt.Partitions[p].OffsetInImage + cxt.Partitions[p].Dpfs.OffsetToData), SeekOrigin.Begin);

                    //jump to dupe if needed (SAVE partition is written twice)
                    if ((cxt.Partitions[p].SecondFlag & 0x20000000) == 0) //*** EXPERIMENTAL ***
                        ims.Seek((long)cxt.Partitions[p].Dpfs.DataLength, SeekOrigin.Current);

                    ims.Seek((long)cxt.Partitions[p].Ivfc.FileSystemOffset, SeekOrigin.Current);

                    if (p == 0)
                    {
                        long saveOffset = ims.Position;
                        cxt.Save = MarshalTool.ReadStruct<SAVE>(ims);
                        //add SAVE information (if exists) (suppose to...)
                        if (SRAMTool.isSaveMagic(cxt.Save.Magic)) //read 
                        {
                            ims.Seek(saveOffset + (long)cxt.Save.FileMapOffset, SeekOrigin.Begin);
                            cxt.FilesMap = new uint[cxt.Save.FileMapSize];
                            for (int i = 0; i < cxt.FilesMap.Length; i++)
                                cxt.FilesMap[i] = ReadUInt32(ims);
                            ims.Seek(saveOffset + (long)cxt.Save.FolderMapOffset, SeekOrigin.Begin);
                            cxt.FoldersMap= new uint[cxt.Save.FolderMapSize];
                            for (int i = 0; i < cxt.FoldersMap.Length; i++)
                                cxt.FoldersMap[i] = ReadUInt32(ims);
                            ims.Seek(saveOffset + (long)cxt.Save.BlockMapOffset, SeekOrigin.Begin);
                            SRAMBlockMapEntry first = MarshalTool.ReadStruct<SRAMBlockMapEntry>(ims);
                            cxt.BlockMap = new SRAMBlockMapEntry[first.EndBlock + 2];
                            cxt.BlockMap[0] = first;
                            for (uint i = 1; i < cxt.BlockMap.Length; i++)
                                cxt.BlockMap[i] = MarshalTool.ReadStruct<SRAMBlockMapEntry>(ims);
                            
                            //-- Get folders -- (and set filebase 'while at it')
                            if (!cxt.isData)
                            {
                                cxt.fileBase = saveOffset + (long)cxt.Save.FileStoreOffset;
                                ims.Seek(cxt.fileBase + cxt.Save.FolderTableOffset * 0x200, SeekOrigin.Begin);
                            }
                            else
                            {   //file base is remote
                                cxt.fileBase = (long)(cxt.Disa.DATAPartitionOffset + cxt.Partitions[1].Difi.FileBase);
                                ims.Seek(saveOffset + cxt.Save.FolderTableOffset, SeekOrigin.Begin);
                            }
                            FileSystemFolderEntry froot = MarshalTool.ReadStruct<FileSystemFolderEntry>(ims);
                            cxt.Folders = new FileSystemFolderEntry[froot.ParentFolderIndex - 1];
                            if (froot.ParentFolderIndex > 1) //if has folders
                                for (int i = 0; i < cxt.Folders.Length; i++)
                                    cxt.Folders[i] = MarshalTool.ReadStruct<FileSystemFolderEntry>(ims);

                            //-- Get files --
                            //go to FST
                            if (!cxt.isData)
                                ims.Seek(cxt.fileBase + cxt.Save.FSTOffset * 0x200, SeekOrigin.Begin);
                            else //file base is remote
                                ims.Seek(saveOffset + cxt.Save.FSTOffset, SeekOrigin.Begin);

                            FileSystemFileEntry root = MarshalTool.ReadStruct<FileSystemFileEntry>(ims);
                            cxt.Files = new FileSystemFileEntry[root.ParentFolderIndex - 1];
                            if ((root.ParentFolderIndex > 1) && (root.Magic == 0)) //if has files
                                for (int i = 0; i < cxt.Files.Length; i++)
                                    cxt.Files[i] = MarshalTool.ReadStruct<FileSystemFileEntry>(ims);
                        }
                        else
                        {   //Not a legal SAVE filesystem
                            cxt.Folders = new FileSystemFolderEntry[0];
                            cxt.Files = new FileSystemFileEntry[0]; 
                        }
                    } // end if (p == 0)
                } //end foreach (partitions)
                ims.Close();
            } //end if crc is ok
            ms.Close();

            return cxt;
        }

        public static byte[] createSAV(SRAMContext cxt)
        {
            //Recompute the partitions' hash tables
            HashAlgorithm ha = SHA256.Create();
            int offset, hashSize;
            for (int i = 0; i < cxt.Partitions.Length; i++)
            {
                // itrerate thorugh the hashes table
                hashSize = (1 << (int)cxt.Partitions[i].Ivfc.FileSystemBlock);
                for (int j = 0; j < cxt.Partitions[i].HashTable.Length; j++)
                    if (!is00(cxt.Partitions[i].HashTable[j])) //hash isn't zero
                    {
                        offset = (int)(cxt.Partitions[i].OffsetInImage + cxt.Partitions[i].Dpfs.OffsetToData  + cxt.Partitions[i].Ivfc.FileSystemOffset);
                        offset += j * hashSize;
                        cxt.Partitions[i].HashTable[j] = ha.ComputeHash(cxt.image, offset, hashSize);
                        //write it into the image
                        offset = (int)(cxt.Partitions[i].OffsetInImage + cxt.Partitions[i].Dpfs.OffsetToData + cxt.Partitions[i].Ivfc.HashTableOffset);
                        offset += j * Sizes.SHA256;
                        Buffer.BlockCopy(cxt.Partitions[i].HashTable[j], 0, cxt.image, offset, Sizes.SHA256);
                    }
            }

            SHA256 sha256 = SHA256.Create();

            /*TODO:
            
            [ ] (Unknwon) Make the 'Partition Hash Table Header'
            [ ] (Unknown) Correct The Partition table hashes (DIFIs).
            
            */

            offset = (int)((cxt.Disa.ActiveTable & 1) == 1 ? cxt.Disa.PrimaryTableOffset : cxt.Disa.SecondaryTableOffset);
            byte[] newDisaHash = sha256.ComputeHash(cxt.image, offset, (int)cxt.Disa.TableLength);
            Buffer.BlockCopy(newDisaHash, 0, cxt.Disa.Hash, 0, Sizes.SHA256); //fix context
            Buffer.BlockCopy(newDisaHash, 0, cxt.image, 0x16C, Sizes.SHA256); //fix image

            /*TODO:
            
            [ ] (Unknown) Correct The Image 128bit hash.
            
            */


            MemoryStream ms = new MemoryStream();

            int blockmapEntrySize = Marshal.SizeOf(typeof(SRAMHeaderEntry));
            int sfHeaderSize = Marshal.SizeOf(typeof(SRAMHeader));
            byte[] crcBlock = new byte[blockmapEntrySize * cxt.Blockmap.Length + sfHeaderSize];
            
            //Prepare the file header
            byte[] temp = MarshalTool.StructureToByteArray<SRAMHeader>(cxt.fileHeader);
            Buffer.BlockCopy(temp, 0, crcBlock, 0, temp.Length);

            //Update & Prepare the blockmap (straight)
            for (byte i = 0; i < cxt.MemoryMap.Length ; i++)
                cxt.MemoryMap[i] = i;
            cxt.Journal = new SRAMLongSectorEntry[0];
            cxt.JournalSize = 0;
            
            for (byte i = 0; i < cxt.Blockmap.Length; i++)
            {
                cxt.Blockmap[i].AllocationCount = 0;
                cxt.Blockmap[i].PhysicalSector = (byte)(i + 0x81); //checksum flag + 1 (offset from file header)
                for (int j = 0; j < cxt.Blockmap[i].CheckSums.Length; j++)
                    cxt.Blockmap[i].CheckSums[j] = CRC16.CS(CRC16.GetCRC(cxt.image, 0x1000 * i + 0x200 * j, 0x200));
                temp = MarshalTool.StructureToByteArray<SRAMHeaderEntry>(cxt.Blockmap[i]);
                Buffer.BlockCopy(temp,0,crcBlock, sfHeaderSize + i * blockmapEntrySize,blockmapEntrySize);
            }
            //Write the header and the blockmap
            ms.Write(crcBlock, 0, crcBlock.Length);

            //Write the CRC16
            ms.Write(CRC16.GetCRC(crcBlock), 0, 2);

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

        public enum SRAMView
        {
            Image,
            Partition,
            Tables
        };

        public static void View(frmExplorer f, SRAMContext cxt, SRAMView view)
        {
            f.ClearInformation();
            switch (view)
            {
                case SRAMView.Image:
                    DISA disa = cxt.Disa;
                    f.SetGroupHeaders("SRAM", "Image");
                    f.AddListItem(0x000, 4, "Unknown 1", cxt.fileHeader.Unknown1, 0);
                    f.AddListItem(0x004, 4, "Unknown 2", cxt.fileHeader.Unknown2, 0);
                    f.AddListItem(0, 4, "** Blockmap length", (ulong)cxt.Blockmap.Length, 0);
                    f.AddListItem(0, 4, "** Journal size", cxt.JournalSize, 0);
                    f.AddListItem(0, 0x10, "** Image Hash", cxt.ImageHash, 1);
                    f.AddListItem(0x000, 4, "DISA Magic", disa.Magic, 1);
                    f.AddListItem(0x004, 4, "Unknown", disa.Unknown0, 1);
                    f.AddListItem(0x008, 8, "Table Size", disa.TableSize, 1);
                    f.AddListItem(0x010, 8, "Primary Table offset", disa.PrimaryTableOffset, 1);
                    f.AddListItem(0x018, 8, "Secondary Table offset", disa.SecondaryTableOffset, 1);
                    f.AddListItem(0x020, 8, "Table Length", disa.TableLength, 1);
                    f.AddListItem(0x028, 8, "SAVE Entry Table offset", disa.SAVEEntryOffset, 1);
                    f.AddListItem(0x030, 8, "SAVE Entry Table length", disa.SAVEEntryLength, 1);
                    f.AddListItem(0x038, 8, "DATA Entry Table offset", disa.DATAEntryOffset, 1);
                    f.AddListItem(0x040, 8, "DATA Entry Table length", disa.DATAEntryLength, 1);
                    f.AddListItem(0x048, 8, "SAVE Partition Offset", disa.SAVEPartitionOffset, 1);
                    f.AddListItem(0x050, 8, "SAVE Partition Length", disa.SAVEPartitionLength, 1);
                    f.AddListItem(0x058, 8, "DATA Partition Offset", disa.DATAPartitionOffset, 1);
                    f.AddListItem(0x060, 8, "DATA Partition Length", disa.DATAPartitionLength, 1);
                    f.AddListItem(0x068, 4, "Active Table is " + ((disa.ActiveTable & 1) == 1 ? "Primary" : "Secondary"), disa.ActiveTable, 1);
                    f.AddListItem(0x06C, 0x20, "Hash", disa.Hash, 1);
                    f.AddListItem(0x08C, 4, "Zero Padding 0(to 8 bytes)", disa.ZeroPad0, 1);
                    f.AddListItem(0x090, 4, "Flag 0 ?", disa.Flag0, 1);
                    f.AddListItem(0x094, 4, "Zero Padding 1(to 8 bytes)", disa.ZeroPad1, 1);
                    f.AddListItem(0x098, 4, "Unknown 1", disa.Unknown1, 1);
                    f.AddListItem(0x09C, 4, "Unknown 2 (Magic?)", disa.Unknown2, 1);
                    f.AddListItem(0x0A0, 8, "Data FS Length", disa.DataFsLength, 1);
                    f.AddListItem(0x0A8, 8, "Unknown 3", disa.Unknown3, 1);
                    f.AddListItem(0x0B0, 4, "Unknown 4", disa.Unknown4, 1);
                    f.AddListItem(0x0B4, 4, "Unknown 5", disa.Unknown5, 1);
                    f.AddListItem(0x0B8, 4, "Unknown 6", disa.Unknown6, 1);
                    f.AddListItem(0x0BC, 4, "Unknown 7", disa.Unknown7, 1);
                    f.AddListItem(0x0C0, 4, "Unknown 8", disa.Unknown8, 1);
                    f.AddListItem(0x0C4, 4, "Flag 1 ?", disa.Flag1, 1);
                    f.AddListItem(0x0C8, 4, "Flag 2 ?", disa.Flag2, 1);
                    f.AddListItem(0x0CC, 4, "Flag 3 ?", disa.Flag3, 1);
                    f.AddListItem(0x0D0, 4, "Flag 4 ?", disa.Flag4, 1);
                    f.AddListItem(0x0D4, 4, "Unknown 14", disa.Unknown14, 1);
                    f.AddListItem(0x0D8, 4, "Flag 5 ?", disa.Flag5, 1);
                    f.AddListItem(0x0DC, 4, "Unknown 16", disa.Unknown16, 1);
                    f.AddListItem(0x0E0, 4, "Magic 17", disa.Magic17, 1);
                    f.AddListItem(0x0E4, 4, "Unknown 18", disa.Unknown18, 1);
                    f.AddListItem(0x0E8, 4, "Flag 6 ?", disa.Flag6, 1);
                    f.AddListItem(0x0EC, 4, "Flag 7 ?", disa.Flag7, 1);
                    f.AddListItem(0x0F0, 4, "Flag 8 ?", disa.Flag8, 1);
                    f.AddListItem(0x0F4, 4, "Unknown 21", disa.Unknown21, 1);
                    f.AddListItem(0x0F8, 4, "Unknown 22", disa.Unknown22, 1);
                    f.AddListItem(0x0FC, 4, "Unknown 23", disa.Unknown23, 1);
                    break;
                case SRAMView.Partition:
                    DIFI difi = cxt.Partitions[cxt.currentPartition].Difi;
                    IVFC ivfc = cxt.Partitions[cxt.currentPartition].Ivfc;
                    DPFS dpfs = cxt.Partitions[cxt.currentPartition].Dpfs;
                    SAVE save = cxt.Save;

                    f.SetGroupHeaders("DIFI", "IVFC", "DPFS", "Hash", "SAVE", "Folders", "Files");
                    f.AddListItem(0x000, 4, "Magic DIFI", difi.Magic, 0);
                    f.AddListItem(0x004, 4, "Magic Padding", difi.MagicPadding, 0);
                    f.AddListItem(0x008, 8, "IVFC Offset", difi.IVFCOffset, 0);
                    f.AddListItem(0x010, 8, "IVFC Size", difi.IVFCSize, 0);
                    f.AddListItem(0x018, 8, "DPFS Offset", difi.DPFSOffset, 0);
                    f.AddListItem(0x020, 8, "DPFS Size", difi.DPFSSize, 0);
                    f.AddListItem(0x028, 8, "Hash Offset", difi.HashOffset, 0);
                    f.AddListItem(0x030, 8, "Hash Size", difi.HashSize, 0);
                    f.AddListItem(0x038, 4, "Flags", difi.Flags, 0);
                    f.AddListItem(0x03C, 8, "File Base (for DATA partitions)", difi.FileBase, 0);

                    f.AddListItem(0x000, 4, "Magic IVFC", ivfc.Magic, 1);
                    f.AddListItem(0x004, 4, "Magic Padding", ivfc.MagicPadding, 1);
                    f.AddListItem(0x008, 8, "Unknown 1", ivfc.Unknown1, 1);
                    f.AddListItem(0x010, 8, "FirstHash Offset", ivfc.FirstHashOffset, 1);
                    f.AddListItem(0x018, 8, "FirstHash Length", ivfc.FirstHashLength, 1);
                    f.AddListItem(0x020, 8, "FirstHash Block" + " (=" + (1 << (int)ivfc.FirstHashBlock) + ")", ivfc.FirstHashBlock, 1);
                    f.AddListItem(0x028, 8, "SecondHash Offset", ivfc.SecondHashOffset, 1);
                    f.AddListItem(0x030, 8, "SecondHash Length", ivfc.SecondHashLength, 1);
                    f.AddListItem(0x038, 8, "SecondHash Block" + " (=" + (1 << (int)ivfc.SecondHashBlock) + ")", ivfc.SecondHashBlock, 1);
                    f.AddListItem(0x040, 8, "HashTable Offset", ivfc.HashTableOffset, 1);
                    f.AddListItem(0x048, 8, "HashTable Length", ivfc.HashTableLength, 1);
                    f.AddListItem(0x050, 8, "HashTable Block" + " (=" + (1 << (int)ivfc.HashTableBlock) + ")", ivfc.HashTableBlock, 1);
                    f.AddListItem(0x058, 8, "FileSystem Offset", ivfc.FileSystemOffset, 1);
                    f.AddListItem(0x060, 8, "FileSystem Length", ivfc.FileSystemLength, 1);
                    f.AddListItem(0x068, 8, "FileSystem Block" + " (=" + (1 << (int)ivfc.FileSystemBlock) + ")", ivfc.FileSystemBlock, 1);
                    f.AddListItem(0x070, 8, "Unknown 3 (?=0x78)", ivfc.Unknown3, 1);

                    f.AddListItem(0x000, 4, "Magic DPFS", dpfs.Magic, 2);
                    f.AddListItem(0x004, 4, "Magic Padding", dpfs.MagicPadding, 2);
                    f.AddListItem(0x008, 8, "First Table Offset", dpfs.FirstTableOffset, 2);
                    f.AddListItem(0x010, 8, "First Table Length", dpfs.FirstTableLength, 2);
                    f.AddListItem(0x018, 8, "First Table Block", dpfs.FirstTableBlock, 2);
                    f.AddListItem(0x020, 8, "Second Table Offset", dpfs.SecondTableOffset, 2);
                    f.AddListItem(0x028, 8, "Second Table Length", dpfs.SecondTableLength, 2);
                    f.AddListItem(0x030, 8, "Second Table Block", dpfs.SecondTableBlock, 2);
                    f.AddListItem(0x038, 8, "Offset to Data", dpfs.OffsetToData, 2);
                    f.AddListItem(0x040, 8, "Data Length", dpfs.DataLength, 2);
                    f.AddListItem(0x048, 8, "Data Block", dpfs.DataBlock, 2);

#if DEBUG
            f.AddListItem(0x000, 4, "* First Flag", cxt.Partitions[cxt.currentPartition].FirstFlag, 2);
            f.AddListItem(0x000, 4, "* First Flag Dupe", cxt.Partitions[cxt.currentPartition].FirstFlagDupe,2);
            f.AddListItem(0x000, 4, "* Second Flag", cxt.Partitions[cxt.currentPartition].SecondFlag, 2);
            f.AddListItem(0x000, 4, "* Second Flag Dupe", cxt.Partitions[cxt.currentPartition].SecondFlagDupe, 2);
#endif

                    f.AddListItem(0x000, 0x20, "Hash", cxt.Partitions[cxt.currentPartition].Hash, 3);

                    if (cxt.currentPartition == 0)
                    {
                        f.AddListItem(0x000, 4, "SAVE Magic", save.Magic, 4);
                        f.AddListItem(0x004, 4, "Magic Padding", save.MagicPadding, 4);
                        f.AddListItem(0x008, 8, "Unknown 1 (?=0x020)", save.Unknown1, 4);
                        f.AddListItem(0x010, 8, "Size of data Partition [medias]", save.PartitionSize, 4);
                        f.AddListItem(0x018, 4, "Partition Media Size", save.PartitionMediaSize, 4);
                        f.AddListItem(0x01C, 8, "Unknown 3 (?=0x000)", save.Unknown3, 4);
                        f.AddListItem(0x024, 4, "Unknown 4 (?=0x200)", save.Unknown4, 4);
                        f.AddListItem(0x028, 8, "File Map Offset", save.FileMapOffset, 4);
                        f.AddListItem(0x030, 4, "File Map Size", save.FileMapSize, 4);
                        f.AddListItem(0x034, 4, "File Map MediaSize", save.FileMapMediaSize, 4);
                        f.AddListItem(0x038, 8, "Folder Map Offset", save.FolderMapOffset, 4);
                        f.AddListItem(0x040, 4, "Folder Map Size", save.FolderMapSize, 4);
                        f.AddListItem(0x044, 4, "Folder Map Media Size", save.FolderMapMediaSize, 4);
                        f.AddListItem(0x048, 8, "Block Map Offset", save.BlockMapOffset, 4);
                        f.AddListItem(0x050, 4, "Block Map Size", save.BlockMapSize, 4);
                        f.AddListItem(0x054, 4, "Block Map Media Size", save.BlockMapMediaSize, 4);
                        f.AddListItem(0x058, 8, "Filestore Offset (from SAVE)", save.FileStoreOffset, 4);
                        f.AddListItem(0x060, 4, "Filestore Length (medias)", save.FileStoreLength, 4);
                        f.AddListItem(0x064, 4, "Filestore Media", save.FileStoreMedia, 4);
                        f.AddListItem(0x068, 4, "Folders Table offset (medias/exact)", save.FolderTableOffset, 4);
                        f.AddListItem(0x06C, 4, "Folders Table Length (medias)", save.FolderTableLength, 4);
                        f.AddListItem(0x070, 4, "Folders Table Unknown", save.FolderTableUnknown, 4);
                        f.AddListItem(0x074, 4, "Folders Table Media Size", save.FolderTableMedia, 4);
                        f.AddListItem(0x078, 4, "Files Table Offset (medias/exact)", save.FSTOffset, 4);
                        f.AddListItem(0x07C, 4, "Files Table Length", save.FSTLength, 4);
                        f.AddListItem(0x080, 4, "Files Table Unknown", save.FSTUnknown, 4);
                        f.AddListItem(0x084, 4, "Files Table Media Size", save.FSTMedia, 4);

                        if (SRAMTool.isSaveMagic(save.Magic))
                        {
                            int i = 1;
                            foreach (FileSystemFolderEntry fse in cxt.Folders)
                                f.AddListItem(i++.ToString(),
                                            fse.Index.ToString(),
                                            Util.charArrayToString(fse.FolderName),
                                            fse.ParentFolderIndex.ToString(),
                                            Util.toHexString(8, fse.LastFileIndex),
                                            5);
                            i = 1;
                            foreach (FileSystemFileEntry fse in cxt.Files)
                                f.AddListItem(i++.ToString(),
                                            fse.BlockOffset.ToString(),
                                            "[" + fse.Index + "] " + Util.charArrayToString(fse.Filename) + ", (" + fse.FileSize + "b)",
                                            fse.ParentFolderIndex.ToString(),
                                            Util.toHexString(8, fse.Unknown2) + " " + Util.toHexString(8, fse.Magic),
                                            6);
                        }
                    }
                    break;
                case SRAMView.Tables:
                    f.SetGroupHeaders("Files", "Folders", "Unknown");
                    if (SRAMTool.isSaveMagic(cxt.Save.Magic))
                    {
                        for (int i = 0; i < cxt.FilesMap.Length; i++)
                            f.AddListItem(i, 4, "UInt32", cxt.FilesMap[i], 0);
                        for (int i = 0; i < cxt.FoldersMap.Length; i++)
                            f.AddListItem(i, 4, "UInt32", cxt.FoldersMap[i], 1);

                        f.AddListItem("", "", "Start", "Start:" + (cxt.BlockMap[0].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[0].EndBlock & 0xff), "Start:" + cxt.BlockMap[0].StartBlock.ToString("X8") + ", End: " + cxt.BlockMap[0].EndBlock.ToString("X8"), 2);
                        for (int i = 1; i < cxt.BlockMap.Length - 1; i++)
                            f.AddListItem("", (i - 1).ToString(), "Block " + i + (cxt.BlockMap[i].EndBlock == 0x80000000 && cxt.BlockMap[i].StartBlock == 0x80000000 ? " (Start of data)" : ""), "Start:" + (cxt.BlockMap[i].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[i].EndBlock & 0xff), "Start:" + cxt.BlockMap[i].StartBlock.ToString("X8") + ", End: " + cxt.BlockMap[i].EndBlock.ToString("X8"), 2);
                        f.AddListItem("", "", "End", "", "Start:" + (cxt.BlockMap[cxt.BlockMap.Length - 1].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[cxt.BlockMap.Length - 1].EndBlock & 0xff), 2);
                    }
                    break;
            }
            f.AutoAlignColumns();
        }
    }
}
