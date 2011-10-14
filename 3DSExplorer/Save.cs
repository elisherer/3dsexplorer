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
        public int Magic;
        public int BlockOffset;
        public long FileSize;
        public int Unknown2; // flags and/or date?
        public int Unknown3;
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

        public int ZeroPad0;
        public int Flag0;
        public int Unknown1;
        public int ZeroPad1; 
        public int Unknown2; //Magic
        public long DataFsLength; //Why??
        public long Unknown3;
        public int Unknown4; 
        public int Unknown5; 
        public int Unknown6;
        public int Unknown7;
        public int Unknown8;
        public int Flag1;
        public int Flag2;
        public int Flag3;
        public int Flag4;
        public int Unknown14;
        public int Flag5;
        public int Unknown16;
        public long Magic17;
        public int Flag6;
        public int Flag7;
        public int Flag8;
        public int Unknown21;
        public int Unknown22;
        public int Unknown23;
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
        public int MagicPadding;
        public long Unknown1;

        public long FirstHashOffset;
        public long FirstHashLength;
        public long FirstHashBlock;
        public long SecondHashOffset;
        public long SecondHashLength;
        public long SecondHashBlock;

        public long HashTableOffset;
        public long HashTableLength;
        public long HashTableBlock;
        public long FileSystemOffset;
        public long FileSystemLength;
        public long FileSystemBlock;
        public long Unknown3; //0x78
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DPFS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public int MagicPadding;

        public long Unknown1;
        public long Unknown2;
        public long Unknown3;
        public long Unknown4;
        public long Unknown5;
        public long Unknown6;
        public long Unknown7;
        
        public long OffsetToNextPartition;
        public long Unknown9;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAVE
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;

        public int MagicPadding;
        public long Unknown1;
        public long PartitionSize;
        public int Unknown2;
        public long Unknown3;
        public int Unknown4;
        public long Unknown5; //length of header?
        public int Unknown6;
        public int Unknown7; 
        public long Unknown8; //offset?
        public int Unknown9; //length?
        public int Unknown10; 
        public long Unknown11; //offset to table
        public int Unknown12;
        public int Unknown13;
        public long LocalFileBaseOffset;
        public int FileStoreLength;
        public int Unknown16;
        public int Unknown17;
        public int FSTBlockOffset;
        public int Unknown18;
        public int Unknown19; 
        public int FSTExactOffset;
        public int Unknown20;
        public int Unknown21;
        public int Unknown22;
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

        public static bool isEncrypted(string path)
        {
            //check if encrypted
            FileStream fs = File.OpenRead(path);
            byte[] magic = new byte[4];
            fs.Seek(0x1000, SeekOrigin.Begin); //Start of information
            while ((fs.Length - fs.Position > 0x200) & !SaveTool.isSaveMagic(magic))
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
            cxt.fileHeader = MarshalTool.ReadStruct<SFHeader>(ms);

            //get the blockmap headers
            int bmSize = (int)(ms.Length >> 12) - 1;
            cxt.Blockmap = new SFHeaderEntry[bmSize];
            cxt.MemoryMap = new byte[bmSize];
            for (int i = 0; i < cxt.Blockmap.Length; i++)
            {
                cxt.Blockmap[i] = MarshalTool.ReadStruct<SFHeaderEntry>(ms);
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
                int jSize = (int)(0x1000 - ms.Position) / Marshal.SizeOf(typeof(SFLongSectorEntry));
                cxt.Journal = new SFLongSectorEntry[jSize];
                cxt.JournalSize = 0;
                int jc = 0;
                while (ms.Position < 0x1000) //assure stopping
                {
                    cxt.Journal[jc] = MarshalTool.ReadStruct<SFLongSectorEntry>(ms);
                    if (!SaveTool.isFF(cxt.Journal[jc].Sector.CheckSums)) //check if we got a valid checksum
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
                if (!SaveTool.isDisaMagic(cxt.Disa.Magic))
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
                        ims.Seek(cxt.Disa.PrimaryTableOffset, SeekOrigin.Begin);
                    else
                        ims.Seek(cxt.Disa.SecondaryTableOffset, SeekOrigin.Begin);

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
                            ims.Seek(cxt.Disa.SAVEPartitionOffset + 0x1000, SeekOrigin.Begin);
                        else
                            ims.Seek(cxt.Disa.DATAPartitionOffset + 0x1000, SeekOrigin.Begin);

                        cxt.Partitions[p].offsetInImage = ims.Position;

                        //Get hashes table
                        ims.Seek(cxt.Partitions[p].Ivfc.HashTableOffset, SeekOrigin.Current);
                        cxt.Partitions[p].HashTable = new byte[cxt.Partitions[p].Ivfc.HashTableLength / 0x20][];
                        for (int i = 0; i < cxt.Partitions[p].HashTable.Length; i++)
                            cxt.Partitions[p].HashTable[i] = ReadByteArray(ims, 0x20);

                        if (p == 0)
                        {
                            ims.Seek(cxt.Partitions[0].offsetInImage, SeekOrigin.Begin);

                            //jump to backup if needed (SAVE partition is written twice)
                            if (cxt.isData) //Apperantly in 2 Partition files the second SAVE is more updated ???
                                ims.Seek(cxt.Partitions[0].Dpfs.OffsetToNextPartition, SeekOrigin.Current);

                            ims.Seek(cxt.Partitions[0].Ivfc.FileSystemOffset, SeekOrigin.Current);
                            long saveOffset = ims.Position;

                            cxt.Save = MarshalTool.ReadStruct<SAVE>(ims);
                            //add SAVE information (if exists) (suppose to...)
                            if (SaveTool.isSaveMagic(cxt.Save.Magic)) //read 
                            {
                                //go to FST
                                if (!cxt.isData)
                                {
                                    cxt.fileBase = saveOffset + cxt.Save.LocalFileBaseOffset;
                                    ims.Seek(cxt.fileBase + cxt.Save.FSTBlockOffset * 0x200, SeekOrigin.Begin);
                                }
                                else //file base is remote
                                {
                                    cxt.fileBase = cxt.Disa.DATAPartitionOffset + cxt.Partitions[1].Difi.FileBase;
                                    ims.Seek(saveOffset + cxt.Save.FSTExactOffset, SeekOrigin.Begin);
                                }

                                FileSystemEntry root = MarshalTool.ReadStruct<FileSystemEntry>(ims);
                                cxt.Files = new FileSystemEntry[root.NodeCount - 1];
                                if ((root.NodeCount > 1) && (root.Magic == 0)) //if has files
                                    for (int i = 0; i < cxt.Files.Length; i++)
                                        cxt.Files[i] = MarshalTool.ReadStruct<FileSystemEntry>(ims);
                            }
                            else
                                cxt.Files = new FileSystemEntry[0]; //Not a legal SAVE filesystem

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
                        offset = (int)(cxt.Partitions[i].offsetInImage + cxt.Partitions[i].Ivfc.FileSystemOffset);
                        offset += j * hashSize;
                        cxt.Partitions[i].HashTable[j] = ha.ComputeHash(cxt.image, offset, hashSize);
                        //write it into the image
                        offset = (int)(cxt.Partitions[i].offsetInImage + cxt.Partitions[i].Ivfc.HashTableOffset);
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

            int blockmapEntrySize = Marshal.SizeOf(typeof(SFHeaderEntry));
            int sfHeaderSize = Marshal.SizeOf(typeof(SFHeader));
            byte[] crcBlock = new byte[blockmapEntrySize * cxt.Blockmap.Length + sfHeaderSize];
            
            //Prepare the file header
            byte[] temp = MarshalTool.StructureToByteArray<SFHeader>(cxt.fileHeader);
            Buffer.BlockCopy(temp, 0, crcBlock, 0, temp.Length);

            //Update & Prepare the blockmap (straight)
            for (byte i = 0; i < cxt.MemoryMap.Length ; i++)
                cxt.MemoryMap[i] = i;
            cxt.Journal = new SFLongSectorEntry[0];
            cxt.JournalSize = 0;
            
            for (byte i = 0; i < cxt.Blockmap.Length; i++)
            {
                cxt.Blockmap[i].AllocationCount = 0;
                cxt.Blockmap[i].PhysicalSector = (byte)(i + 0x81); //checksum flag + 1 (offset from file header)
                for (int j = 0; j < cxt.Blockmap[i].CheckSums.Length; j++)
                    cxt.Blockmap[i].CheckSums[j] = CRC16.CS(CRC16.GetCRC(cxt.image, 0x1000 * i + 0x200 * j, 0x200));
                temp = MarshalTool.StructureToByteArray<SFHeaderEntry>(cxt.Blockmap[i]);
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
