using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace _3DSExplorer
{
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

        public static SFContext Open(string path, ref string errorMessage)
        {
            SFContext cxt = new SFContext();

            cxt.Encrypted = isEncrypted(path);

            //get the file into buffer to find the key if needed
            byte[] fileBuffer = File.ReadAllBytes(path);
            MemoryStream ms = new MemoryStream(fileBuffer);

            if (cxt.Encrypted)
            {
                byte[] key = FindKey(fileBuffer);
                if (key == null)
                {
                    ms.Close();
                    errorMessage = "Can't find key in binary file.";
                    return null;
                }
                else
                {
                    XorByteArray(fileBuffer, key, 0x1000);
                    //XorExperimental(fileBuffer, key, 0x1000);
                    cxt.Key = key;
                }
            }
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
                else
                {
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
                }
                ims.Close();
            } //end if crc is ok
            ms.Close();

            return cxt;
        }

        public static byte[] createSAV(SFContext cxt)
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
    }
}
