using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Reflection;

namespace _3DSExplorer
{

    public partial class frmExplorer : Form
    {
        TreeNode topNode;
        TreeNode[] childNodes;
        Context currentContext;
        string filePath;

        public frmExplorer()
        {
            InitializeComponent();
        }

        public T ReadStruct<T>(Stream fs)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];

            fs.Read(buffer, 0, Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }
        public T ReadStructBE<T>(Stream fs)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];

            fs.Read(buffer, 0, Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            System.Type t = temp.GetType();
            FieldInfo[] fieldInfo = t.GetFields();
            foreach (FieldInfo fi in fieldInfo)
            {                 
                if (fi.FieldType == typeof(System.Int16))
                {
                    Int16 i16 = (Int16)fi.GetValue(temp);
                    byte[] b16 = BitConverter.GetBytes(i16);
                    byte[] b16r = b16.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt16(b16r, 0));
                }
                else if (fi.FieldType == typeof(System.Int32))
                {
                    Int32 i32 = (Int32)fi.GetValue(temp);
                    byte[] b32 = BitConverter.GetBytes(i32);
                    byte[] b32r = b32.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt32(b32r, 0));
                }
                else if (fi.FieldType == typeof(System.Int64))
                {
                    Int64 i64 = (Int64)fi.GetValue(temp);
                    byte[] b64 = BitConverter.GetBytes(i64);
                    byte[] b64r = b64.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt64(b64r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt16))
                {
                    UInt16 i16 = (UInt16)fi.GetValue(temp);
                    byte[] b16 = BitConverter.GetBytes(i16);
                    byte[] b16r = b16.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt16(b16r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt32))
                {
                    UInt32 i32 = (UInt32)fi.GetValue(temp);
                    byte[] b32 = BitConverter.GetBytes(i32);
                    byte[] b32r = b32.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt32(b32r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt64))
                {
                    UInt64 i64 = (UInt64)fi.GetValue(temp);
                    byte[] b64 = BitConverter.GetBytes(i64);
                    byte[] b64r = b64.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt64(b64r, 0));
                }
            }
            return temp;
        }
        private void makeNewListItem(string text, string sub1,string sub2, string sub3)
        {
            ListViewItem lvi = new ListViewItem(text);
            lvi.SubItems.Add(sub1);
            lvi.SubItems.Add(sub2);
            lvi.SubItems.Add(sub3);
            lstInfo.Items.Add(lvi);
        }

        #region ToString functions

        private string byteArrayToString(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length && i < 33; i++)
                arraystring += String.Format("{0:X2}", array[i]) + (i < array.Length - 1 ? " " : "");
            if (i == 33) return arraystring + "..."; //ellipsis
            return arraystring;
        }

        private string charArrayToString(char[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length; i++)
            {
                if (array[i] == 0) break;
                arraystring += array[i];
            }
            return arraystring + "";
        }

        private string toHexString(int digits, ulong number)
        {
            return "0x" + String.Format("{0:X" + digits + "}", number);
        }

        #endregion

        /*
         * SHA-256 Check Algorithm (using System.Security.Cryptography)
         * 
         * //'data' should be the ncsd header without the signature
         * byte[] result = (new SHA256Managed()).ComputeHash(data); 
         * // result should be equal to signature
         */

        #region CCIContext

        /**
         *   (1 media unit = 0x200 bytes)
         *   Flags: 5-7 content (update,app,...) size [medias] (0x200*2^byte[6]) and enc
         */

        private void showNCSD()
        {
            CCIContext cxt = (CCIContext)currentContext;
            lstInfo.Items.Clear();
            makeNewListItem("0x000", "0x100", "RSA-2048 signature of the NCSD header [SHA-256]", byteArrayToString(cxt.cci.NCSDHeaderSignature));
            makeNewListItem("0x100", "4", "Magic ID, always 'NCSD'", charArrayToString(cxt.cci.MagicID));
            makeNewListItem("0x104", "4", "Content size [medias]", cxt.cci.CCISize + " (=" + cxt.cci.CCISize * 0x200 + " bytes)");
            makeNewListItem("0x108", "8", "Title/Program ID", toHexString(16, cxt.cci.TitleID));
            makeNewListItem("0x120", "4", "Offset to the first NCCH [medias]", cxt.cci.FirstNCCHOffset + " (=" + cxt.cci.FirstNCCHOffset * 0x200 + " bytes)");
            makeNewListItem("0x124", "4", "Size of the first NCCH [medias]", cxt.cci.FirstNCCHSize + " (=" + cxt.cci.FirstNCCHSize * 0x200 + " bytes)");
            makeNewListItem("0x130", "4", "Offset to the second NCCH [medias]", cxt.cci.SecondNCCHOffset + " (=" + cxt.cci.SecondNCCHOffset * 0x200 + " bytes)");
            makeNewListItem("0x134", "4", "Size of the second NCCH [medias]", cxt.cci.SecondNCCHSize + " (=" + cxt.cci.SecondNCCHSize * 0x200 + " bytes)");
            makeNewListItem("0x158", "4", "Offset to the third NCCH [medias]", cxt.cci.ThirdNCCHOffset + " (=" + cxt.cci.ThirdNCCHOffset * 0x200 + " bytes)");
            makeNewListItem("0x15C", "4", "Size of the third NCCH [medias]", cxt.cci.ThirdNCCHSize + " (=" + cxt.cci.ThirdNCCHSize * 0x200 + " bytes)");
            makeNewListItem("0x188", "8", "NCCH Flags", byteArrayToString(cxt.cci.NCCHFlags));
            makeNewListItem("0x190", "8", "Partition ID of the first NCCH", toHexString(16, cxt.cci.FirstNCCHPartitionID));
            makeNewListItem("0x1A0", "8", "Partition ID of the second NCCH", toHexString(16, cxt.cci.SecondNCCHPartitionID));
            makeNewListItem("0x1C8", "8", "Partition ID of the third NCCH", toHexString(16, cxt.cci.ThirdNCCHPartitionID));
            makeNewListItem("0x200", "4", "Always 0xFFFFFFFF", byteArrayToString(cxt.cci.PaddingFF));
            makeNewListItem("0x300", "4", "Used ROM size [bytes]", cxt.cci.UsedRomSize.ToString());
            makeNewListItem("0x320", "16", "Unknown", byteArrayToString(cxt.cci.Unknown));
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
        }

        private void showNCCH(int i)
        {
            CCIContext cxt = (CCIContext)currentContext;
            lstInfo.Items.Clear();
            makeNewListItem("0x000", "0x100", "RSA-2048 signature of the NCCH header [SHA-256]", byteArrayToString(cxt.cxis[i].NCCHHeaderSignature));
            makeNewListItem("0x100", "4", "Magic ID, always 'NCCH'", charArrayToString(cxt.cxis[i].MagicID));
            makeNewListItem("0x104", "4", "Content size [medias]", cxt.cxis[i].CXISize + " (=" + cxt.cxis[i].CXISize * 0x200 + " bytes)");

            makeNewListItem("0x108", "8", "Partition ID", toHexString(16, cxt.cxis[i].PartitionID));
            string makerCode = charArrayToString(cxt.cxis[i].MakerCode);
            makeNewListItem("0x110", "2", "Maker Code", makerCode + " (=" + MakerResolver.Resolve(makerCode) + ")");
            makeNewListItem("0x112", "2", "Version", cxt.cxis[i].Version.ToString());
            makeNewListItem("0x118", "8", "Program ID", toHexString(16, cxt.cxis[i].ProgramID));
            makeNewListItem("0x120", "1", "Temp Flag", toHexString(2, cxt.cxis[i].TempFlag));
            string productCode = charArrayToString(cxt.cxis[i].ProductCode);
            makeNewListItem("0x150", "0x10", "Product Code", productCode + " (=" + GameTitleResolver.Resolve(productCode.Substring(7,2)) + ")");
            makeNewListItem("0x160", "0x20", "Extended Header Hash", byteArrayToString(cxt.cxis[i].ExtendedHeaderHash));
            makeNewListItem("0x180", "4", "Extended header size", cxt.cxis[i].ExtendedHeaderSize.ToString());
            makeNewListItem("0x188", "8", "Flags", byteArrayToString(cxt.cxis[i].Flags));
            makeNewListItem("0x190", "4", "Plain region offset [medias]", cxt.cxis[i].PlainRegionOffset + " (=" + cxt.cxis[i].PlainRegionOffset * 0x200 + " bytes)");
            makeNewListItem("0x194", "4", "Plain region size [medias]", cxt.cxis[i].PlainRegionSize + " (=" + cxt.cxis[i].PlainRegionSize * 0x200 + " bytes)");
            makeNewListItem("0x1A0", "4", "ExeFS offset [medias]", cxt.cxis[i].ExeFSOffset + " (=" + cxt.cxis[i].ExeFSOffset * 0x200 + " bytes)");
            makeNewListItem("0x1A4", "4", "ExeFS size [medias]", cxt.cxis[i].ExeFSSize + " (=" + cxt.cxis[i].ExeFSSize * 0x200 + " bytes)");
            makeNewListItem("0x1A8", "4", "ExeFS hash region size [medias]", cxt.cxis[i].ExeFSHashRegionSize + " (=" + cxt.cxis[i].ExeFSHashRegionSize * 0x200 + " bytes)");
            makeNewListItem("0x1B0", "4", "RomFS offset [medias]", cxt.cxis[i].RomFSOffset + " (=" + cxt.cxis[i].RomFSOffset * 0x200 + " bytes)");
            makeNewListItem("0x1B4", "4", "RomFS size [medias]", cxt.cxis[i].RomFSSize + " (=" + cxt.cxis[i].RomFSSize * 0x200 + " bytes)");
            makeNewListItem("0x1B8", "4", "RomFS hash region size [medias]", cxt.cxis[i].RomFSHashRegionSize + " (=" + cxt.cxis[i].RomFSHashRegionSize * 0x200 + " bytes)");
            makeNewListItem("0x1C0", "0x20", "ExeFS superblock hash", byteArrayToString(cxt.cxis[i].ExeFSSuperBlockhash));
            makeNewListItem("0x1E0", "0x20", "RomFS superblock hash", byteArrayToString(cxt.cxis[i].RomFSSuperBlockhash));
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
            ListViewItem lvItem;
            if (cxt.cxis[i].RomFSSize > 0)
            {
                lvItem = lvFileSystem.Items.Add("RomFS" + i + ".bin");
                lvItem.ImageIndex = 0;
                lvItem.Tag = cxt.cxis[i];
            }
            if (cxt.cxis[i].ExeFSSize > 0)
            {
                lvItem = lvFileSystem.Items.Add("ExeFS" + i + ".bin");
                lvItem.ImageIndex = 0;
                lvItem.Tag = cxt.cxis[i];
            }

        }

        private void showNCCHPlainRegion(int i)
        {
            CCIContext cxt = (CCIContext)currentContext;
            lstInfo.Items.Clear();
            for (int j = 0 ;j < cxt.cxiprs[i].PlainRegionStrings.Length ; j++)
                makeNewListItem("", cxt.cxiprs[i].PlainRegionStrings[j].Length.ToString(), "Text", cxt.cxiprs[i].PlainRegionStrings[j]);

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
        }

        private void OpenCCI(string path)
        {
            CCIContext cxt = new CCIContext();

            FileStream fs = File.OpenRead(path);

            cxt.cci = ReadStruct<CCI>(fs);

            //Build Tree
            treeView.Nodes.Clear();
            topNode = treeView.Nodes.Add("NCSD (" + path.Substring(path.LastIndexOf('\\') + 1) + ")");
            childNodes = new TreeNode[3]; // 3 Nodes (null nodes are missing NCCHs)

            // Read the NCCHs
            cxt.cxis = new CXI[3];
            cxt.cxiprs = new CXIPlaingRegion[3];
            byte[] plainRegionBuffer;
            if (cxt.cci.FirstNCCHSize > 0)
            {
                fs.Seek(cxt.cci.FirstNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[0] = ReadStruct<CXI>(fs);
                childNodes[0] = topNode.Nodes.Add("NCCH0 (" + (new String(cxt.cxis[0].ProductCode)).Substring(0, 10) + ")");
                // get Plaing Region
                fs.Seek((cxt.cxis[0].PlainRegionOffset + cxt.cci.FirstNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[0].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[0] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
                childNodes[0].Nodes.Add("PlainRegion");

                // byte[] exh = new byte[2048];
                // fs.Read(exh, 0, exh.Length);
                // Array.Reverse(exh);
                // File.OpenWrite(path.Substring(path.LastIndexOf('\\') + 1) + "-rev.exh").Write(exh, 0, exh.Length);

            }
            if (cxt.cci.SecondNCCHSize > 0)
            {
                fs.Seek(cxt.cci.SecondNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[1] = ReadStruct<CXI>(fs);
                childNodes[1] = topNode.Nodes.Add("NCCH1 (" + (new String(cxt.cxis[1].ProductCode)).Substring(0, 10) + ")");
                // get Plaing Region
                fs.Seek((cxt.cxis[1].PlainRegionOffset + cxt.cci.SecondNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[1].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[1] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
                childNodes[1].Nodes.Add("PlainRegion");
            }
            if (cxt.cci.ThirdNCCHSize > 0)
            {
                fs.Seek(cxt.cci.ThirdNCCHOffset * 0x200, SeekOrigin.Begin);
                cxt.cxis[2] = ReadStruct<CXI>(fs);
                childNodes[2] = topNode.Nodes.Add("NCCH2 (" + (new String(cxt.cxis[2].ProductCode)).Substring(0, 10) + ")");
                // get Plaing Region
                fs.Seek((cxt.cxis[2].PlainRegionOffset + cxt.cci.ThirdNCCHOffset) * 0x200, SeekOrigin.Begin);
                plainRegionBuffer = new byte[cxt.cxis[2].PlainRegionSize * 0x200];
                fs.Read(plainRegionBuffer, 0, plainRegionBuffer.Length);
                cxt.cxiprs[2] = CXI.getPlainRegionStringsFrom(plainRegionBuffer);
                childNodes[2].Nodes.Add("PlainRegion");
            }

            treeView.ExpandAll();

            fs.Close();

            currentContext = cxt;
            
            treeView.SelectedNode = topNode;
        }

        #endregion

        #region SFContext

        private void showSave()
        {
            SFContext cxt = (SFContext)currentContext;
            lstInfo.Items.Clear();
            makeNewListItem("0x000", "4", "Unknown 1", cxt.fileHeader.Unknown1.ToString());
            makeNewListItem("0x004", "4", "Unknown 2", cxt.fileHeader.Unknown2.ToString());
            makeNewListItem("", "", "Blockmap length", cxt.BlockmapLength.ToString());
            makeNewListItem("", "", "Journal size", cxt.JournalSize.ToString());
            makeNewListItem("DISA", "", "", "");
            makeNewListItem("0x000", "4", "DISA Magic", charArrayToString(cxt.imageHeader.DISA.Magic));
            makeNewListItem("0x004", "12", "Unknown", byteArrayToString(cxt.imageHeader.DISA.Unknown0));
            makeNewListItem("0x010", "8", "First Difi offset", cxt.imageHeader.DISA.FirstDifiOffset.ToString());
            makeNewListItem("0x018", "8", "Second Difi offset", cxt.imageHeader.DISA.SecondDifiOffset.ToString());
            makeNewListItem("0x020", "8", "First Difi size", cxt.imageHeader.DISA.FirstDifiSize.ToString());
            makeNewListItem("0x028", "8", "Padding?", cxt.imageHeader.DISA.Padding.ToString());
            makeNewListItem("0x030", "8", "Second Difi size", cxt.imageHeader.DISA.SecondDifiSize.ToString());
            makeNewListItem("0x038", "0x30", "Unknown", byteArrayToString(cxt.imageHeader.DISA.Unknown1));
            makeNewListItem("0x03C", "4", "Hash is for",cxt.imageHeader.DISA.FirstHashed == 1 ? "First Difi" : "Second Difi" );
            makeNewListItem("0x040", "0x20", "Hash", byteArrayToString(cxt.imageHeader.DISA.Hash));
            makeNewListItem("0x060", "0x74", "Unknown", byteArrayToString(cxt.imageHeader.DISA.Unknown2));
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
        }

        private void showDifi()
        {
            SFContext cxt = (SFContext)currentContext;
            lstInfo.Items.Clear();
            SFDIFIBlob difi = (SFDIFIBlob)cxt.difis[cxt.currentDifi];
            SFSave save = (SFSave)cxt.saves[cxt.currentDifi];

            makeNewListItem("0x000", "4", "Magic DIFI", charArrayToString(difi.MagicDIFI));
            makeNewListItem("0x004", "0x35", "Unknown", byteArrayToString(difi.Unknown0));
            makeNewListItem("0x039", "4", "Index", difi.Index.ToString());
            makeNewListItem("0x03D", "7", "Unknown", byteArrayToString(difi.Unknown1));
            makeNewListItem("0x044", "4", "Magic IVFC", charArrayToString(difi.MagicIVFC));
            makeNewListItem("0x048", "0x54", "Unknown", byteArrayToString(difi.Unknown2));
            makeNewListItem("0x050", "8", "HashTableLength", difi.HashTableLength.ToString());
            makeNewListItem("0x058", "8", "FileSystemLength", difi.FileSystemLength.ToString());

            makeNewListItem("0x060", "0x10", "Unknown", byteArrayToString(difi.Unknown3));
            makeNewListItem("0x070", "4", "Magic DPFS", charArrayToString(difi.MagicDPFS));
            makeNewListItem("0x074", "0x6C", "Unknown", byteArrayToString(difi.Unknown4));
            makeNewListItem("0x0E0", "4", "Garbage", byteArrayToString(difi.Garbage));
            
            makeNewListItem("[SAVE]", "", "", "");
            makeNewListItem("0x000", "4", "SAVE Magic", charArrayToString(save.MagicSAVE));
            makeNewListItem("0x000", "0x54", "Unknown", byteArrayToString(save.Unknown0));
            makeNewListItem("0x000", "4", "FileSystem Offset (form SAVE)", save.FSTOffset.ToString());
            makeNewListItem("0x000", "0x10", "Unknown", byteArrayToString(save.Unknown1));
            makeNewListItem("0x000", "4", "FileSystem Block Offset (block=0x200 bytes)", save.FSTBlockOffset.ToString());
            makeNewListItem("0x000", "0x08", "Unknown", byteArrayToString(save.Unknown2));
            makeNewListItem("0x000", "4", "FileSystem Exact Offset", save.FSTExactOffset.ToString());
            
            lvFileSystem.Clear();
            if (SaveTool.isSaveMagic(save.MagicSAVE))
            {
                makeNewListItem("[FILES]", "", "", "");

                lvFileSystem.Clear();
                ListViewItem lvItem;
                int i = 0;
                foreach (SFFileSystemEntry fse in (SFFileSystemEntry[])cxt.savesFiles[cxt.currentDifi])
                {
                    makeNewListItem(i++.ToString(), fse.FileSize.ToString(), charArrayToString(fse.Filename), "");
                    makeNewListItem("", "4", "NodeCount", fse.NodeCount.ToString());
                    makeNewListItem("", "4", "FileIndex", fse.Index.ToString());
                    makeNewListItem("", "4", "Magic? (Unknown1)", fse.Magic.ToString());
                    makeNewListItem("", "4", "FileBlockOffset", fse.BlockOffset.ToString() + "(=" + toHexString(4, (ulong)fse.BlockOffset) + ")");
                    makeNewListItem("", "4", "Unknown2", fse.Unknown2.ToString());
                    makeNewListItem("", "4", "Unknown3", fse.Unknown3.ToString() + "(=" +toHexString(4,(ulong)fse.Unknown3) +")");
                    makeNewListItem("", "4", "Unknown4", fse.Unknown4.ToString());
                    
                    lvItem = lvFileSystem.Items.Add(charArrayToString(fse.Filename) + "\n" + fse.FileSize + "b");
                    lvItem.ImageIndex = 0;
                    lvItem.Tag = fse;
                }
            }
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void OpenSave(string path, bool encrypted)
        {
            SFContext cxt = new SFContext();

            //get the file into buffer to find the key if needed
            byte[] fileBuffer = File.ReadAllBytes(path);
            MemoryStream ms = new MemoryStream(fileBuffer);

            //Build Tree
            treeView.Nodes.Clear();
            topNode = treeView.Nodes.Add("Save Flash");

            if (encrypted)
            {
                byte[] key = SaveTool.FindKey(fileBuffer);
                if (key == null)
                    MessageBox.Show("Can't find key to decrypt the binary file");
                else
                {
                    SaveTool.XorByteArray(fileBuffer, key, 0x1000);
                    cxt.Key = key;
                }
            }

            cxt.fileHeader = ReadStruct<SFHeader>(ms);
            ArrayList mapping = new ArrayList();

            //get the blockmap headers
            cxt.BlockmapLength = (int)(ms.Length >> 12) - 1;
            SFHeaderEntry hEntry = new SFHeaderEntry();
            for (int i=0;i<cxt.BlockmapLength;i++)
            {
                hEntry = ReadStruct<SFHeaderEntry>(ms);
                mapping.Add((int)(hEntry.PhysicalSector));
            }
            //Check crc16
            byte[] twoBytes = new byte[2], crcBytes = new byte[2];
            ms.Read(crcBytes, 0, 2);
            twoBytes = CRC16.GetCRC(fileBuffer,0,ms.Position - 2);
            if (crcBytes[0] != twoBytes[0] || crcBytes[1] != twoBytes[1])
            {
                MessageBox.Show("CRC Error");
                lstInfo.Clear();
                treeView.Nodes.Clear();
            }
            else
            {
                //get journal updates
                byte[] lastChk = new byte[8];
                cxt.JournalSize = 0;
                SFLongSectorEntry lsEntry = new SFLongSectorEntry();
                while (!SaveTool.isFF(lastChk) && ms.Position < 0x1000) //assure stopping
                {
                    lsEntry = ReadStruct<SFLongSectorEntry>(ms);
                    lastChk = lsEntry.Dupe.CheckSums;
                    if (!SaveTool.isFF(lastChk))
                    {
                        mapping[lsEntry.Sector.VirtualSector] = (int)(lsEntry.Sector.PhysicalSector);
                        cxt.JournalSize++;
                    }
                }
                //File.WriteAllBytes("image_notarranged.bin", fileBuffer);
                
                //rearragne by virtual
                cxt.image = new byte[fileBuffer.Length - 0x1000];
                for (int i = 0; i < mapping.Count; i++)
                    if ((int)mapping[i] != -1)
                        Buffer.BlockCopy(fileBuffer, ((int)mapping[i] & 0x7F) * 0x1000, cxt.image, i * 0x1000, 0x1000);

                //File.WriteAllBytes("image_arranged.bin", cxt.image);

                MemoryStream ims = new MemoryStream(cxt.image);

                cxt.imageHeader = ReadStruct<SFImageHeader>(ims);

                char[] lastDifiMagic = new char[] { 'D', 'I', 'F', 'I' };
                SFDIFIBlob difiBlob;
                cxt.difis = new ArrayList();
                while (SaveTool.isDifiMagic(lastDifiMagic))
                {
                    difiBlob = ReadStruct<SFDIFIBlob>(ims);
                    lastDifiMagic = difiBlob.MagicDIFI;
                    if (SaveTool.isDifiMagic(lastDifiMagic))
                    {
                        topNode.Nodes.Add("Partition " + cxt.difis.Count);
                        cxt.difis.Add(difiBlob);
                    }
                }

                //Collect save partitions
                cxt.saves = new ArrayList();
                cxt.savesFiles = new ArrayList();
                ims.Seek(0x2000, SeekOrigin.Begin);
                long lastPos = ims.Position, savePos;
                for (int i = 0; i < cxt.difis.Count; i++)
                {
                    lastPos = ims.Position;
                    ims.Seek((long)((SFDIFIBlob)cxt.difis[i]).HashTableLength, SeekOrigin.Current);
                    savePos = ims.Position;
                    SFSave save = ReadStruct<SFSave>(ims);
                    cxt.saves.Add(save);
                    if (SaveTool.isSaveMagic(((SFSave)cxt.saves[i]).MagicSAVE)) //read 
                    {
                        //go to FST
                        if ((save.FSTBlockOffset != 0) || (save.FSTOffset != 0))
                            ims.Seek(savePos + save.FSTBlockOffset * 0x200 + save.FSTOffset, SeekOrigin.Begin);
                        else //no block offset
                            ims.Seek(savePos + save.FSTExactOffset, SeekOrigin.Begin);

                        SFFileSystemEntry root = ReadStruct<SFFileSystemEntry>(ims);
                        if ((root.NodeCount > 1) && (root.Magic == 0)) //if has files
                        {
                            SFFileSystemEntry[] files = new SFFileSystemEntry[root.NodeCount - 1];
                            for (int j = 0; j < files.Length; j++)
                                files[j] = ReadStruct<SFFileSystemEntry>(ims);
                            cxt.savesFiles.Add(files);
                        }
                        else //empty
                        {
                            cxt.savesFiles.Add(new SFFileSystemEntry[0]);
                        }

                    }
                    else
                        cxt.savesFiles.Add(new SFFileSystemEntry[0]); //No legal SAVE filesystem
                    //go to next partition
                    ims.Seek(savePos + (long)((SFDIFIBlob)cxt.difis[i]).FileSystemLength, SeekOrigin.Begin);
                    
                    if (ims.Position % 0x1000 != 0) //go to the nearest block (0x1000)
                    {
                        ims.Seek(0x1000 - (ims.Position % 0x1000), SeekOrigin.Current);
                    }
                }

                ims.Close();

                lstInfo.Items.Clear();
            }
            ms.Close();

            currentContext = cxt;
            treeView.ExpandAll();
            treeView.SelectedNode = topNode;
        }

        #endregion

        #region TMDContext

        private void showTMDContentChunks()
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            TMDContentChunkRecord cr;
            for (int i = 0; i < cxt.chunks.Count; i++)
            {
                cr = (TMDContentChunkRecord)cxt.chunks[i];
                makeNewListItem(i.ToString(), "4", "Content ID", cr.ContentID.ToString());
                makeNewListItem("", "2", "Content Index", cr.ContentIndex.ToString());
                makeNewListItem("", "2", "Content Type", cr.ContentType.ToString() + " " + TMDTool.typeToString(cr.ContentType));
                makeNewListItem("", "8", "Content Size", cr.ContentSize.ToString());
                makeNewListItem("", "32", "Content Hash", byteArrayToString(cr.ContentHash));
            }
        }

        private void showTMDContentRecords()
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            for (int i = 0; i < 64; i++)
            {
                makeNewListItem(i.ToString(), "2", "Content Command Count", cxt.ContentInfoRecords[i].ContentCommandCount.ToString());
                makeNewListItem("", "2", "Content Index Offset", cxt.ContentInfoRecords[i].ContentIndexOffset.ToString());
                makeNewListItem("", "32", "Next Content Hash", byteArrayToString(cxt.ContentInfoRecords[i].NextContentHash));
            }
        }

        private void showTMD()
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            TMDHeader head = cxt.head;
            makeNewListItem("0x000", "4", "Signature Type", cxt.SignatureType.ToString());
            if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256 || cxt.SignatureType == TMDSignatureType.RSA_2048_SHA1)
                makeNewListItem("0x004", "0x100", "RSA-2048 signature of the TMD", byteArrayToString(cxt.tmdSHA));
            else
                makeNewListItem("0x004", "0x200", "RSA-4096 signature of the TMD", byteArrayToString(cxt.tmdSHA));
            makeNewListItem("", "60", "Reserved0", byteArrayToString(head.Reserved0));
            makeNewListItem("", "64", "Issuer", charArrayToString(head.Issuer));
            makeNewListItem("", "4", "Version", head.Version.ToString());
            makeNewListItem("", "", "Car Crl Version", head.CarCrlVersion.ToString());
            makeNewListItem("", "", "Signer Version", head.SignerVersion.ToString());
            makeNewListItem("", "", "Reserved1", head.Reserved1.ToString());
            makeNewListItem("", "", "System Version", byteArrayToString(head.SystemVersion));
            makeNewListItem("", "", "Title ID", byteArrayToString(head.TitleID));
            makeNewListItem("", "", "Title Type", head.TitleType.ToString());
            makeNewListItem("", "", "Group ID", charArrayToString(head.GroupID));
            makeNewListItem("", "", "Reserved2", byteArrayToString(head.Reserved2));
            makeNewListItem("", "", "Access Rights", head.AccessRights.ToString());
            makeNewListItem("", "", "Title Version", head.TitleVersion.ToString());
            makeNewListItem("", "", "Content Count", head.ContentCount.ToString());
            makeNewListItem("", "", "Boot Content", head.BootContent.ToString());
            makeNewListItem("", "", "Padding", head.Padding0.ToString());
            makeNewListItem("", "", "Content Info Records Hash", byteArrayToString(head.ContentInfoRecordsHash));

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
        }

        private void showTMDCertificate(int i)
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            TMDCertContext ccxt = (TMDCertContext)cxt.certs[i];
            TMDCertificate cert = ccxt.cert;
            makeNewListItem("0x000", "4", "Signature Type", ccxt.SignatureType.ToString());
            if (ccxt.SignatureType == TMDSignatureType.RSA_2048_SHA256 || ccxt.SignatureType == TMDSignatureType.RSA_2048_SHA1)
                makeNewListItem("0x004", "0x100", "RSA-2048 signature of the TMD", byteArrayToString(ccxt.tmdSHA));
            else
                makeNewListItem("0x004", "0x200", "RSA-4096 signature of the TMD", byteArrayToString(ccxt.tmdSHA));
            makeNewListItem("", "60", "Reserved0", byteArrayToString(cert.Reserved0));
            makeNewListItem("", "64", "Issuer", charArrayToString(cert.Issuer));
            makeNewListItem("", "4", "Tag", cert.Tag.ToString());
            makeNewListItem("", "64", "Name", charArrayToString(cert.Name));
            makeNewListItem("", "0x104", "Key", byteArrayToString(cert.Key));
            makeNewListItem("", "2", "Unknown0", cert.Unknown1.ToString());
            makeNewListItem("", "2", "Unknown1", cert.Unknown2.ToString());
            makeNewListItem("", "52", "Padding", byteArrayToString(cert.Padding));

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Clear();
        }

        private void OpenTMD(string path)
        {
            TMDContext cxt = new TMDContext();

            FileStream fs = File.OpenRead(path);
            bool supported = true;
            
            byte[] intBytes = new byte[4];
            fs.Read(intBytes, 0, 4);
            cxt.SignatureType = (TMDSignatureType)BitConverter.ToInt32(intBytes, 0);
            // Read the TMD RSA Type 
            if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256)
                cxt.tmdSHA = new byte[256];
            else if (cxt.SignatureType == TMDSignatureType.RSA_4096_SHA256)
                cxt.tmdSHA = new byte[512];
            else
            {
                MessageBox.Show("This kind of TMD is unsupported.");
                supported = false;
            }
            if (supported)
            {
                fs.Read(cxt.tmdSHA, 0, cxt.tmdSHA.Length);
                //Continue reading header
                cxt.head = ReadStructBE<TMDHeader>(fs); //read header
                cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                for (int i = 0; i < cxt.ContentInfoRecords.Length; i++)
                    cxt.ContentInfoRecords[i] = ReadStructBE<TMDContentInfoRecord>(fs);
                cxt.chunks = new ArrayList();
                for (int i = 0; i < cxt.head.ContentCount; i++)
                    cxt.chunks.Add(ReadStructBE<TMDContentChunkRecord>(fs));
                //start reading certificates
                cxt.certs = new ArrayList();
                while (fs.Position != fs.Length)
                {
                    TMDCertContext tcert = new TMDCertContext();
                    fs.Read(intBytes, 0, 4);
                    tcert.SignatureType = (TMDSignatureType)BitConverter.ToInt32(intBytes, 0);
                    // RSA Type
                    if (tcert.SignatureType == TMDSignatureType.RSA_2048_SHA256 || tcert.SignatureType == TMDSignatureType.RSA_2048_SHA1)
                        tcert.tmdSHA = new byte[256];
                    else if (tcert.SignatureType == TMDSignatureType.RSA_4096_SHA256 || tcert.SignatureType == TMDSignatureType.RSA_4096_SHA1)
                        tcert.tmdSHA = new byte[512];
                    fs.Read(tcert.tmdSHA, 0, tcert.tmdSHA.Length);
                    tcert.cert = ReadStructBE<TMDCertificate>(fs);
                    cxt.certs.Add(tcert);
                }
                //Build Tree
                treeView.Nodes.Clear();
                topNode = treeView.Nodes.Add("TMD");
                topNode.Nodes.Add("Content Info Records");
                topNode.Nodes.Add("Content Chunk Records");
                for (int i = 0; i < cxt.certs.Count; i++)
                {
                    TMDCertContext tmd = (TMDCertContext)cxt.certs[i];
                    topNode.Nodes.Add("TMD Certificate " + i);
                }
                treeView.ExpandAll();

                currentContext = cxt;

                treeView.SelectedNode = topNode;
            }
            fs.Close();
        }

        #endregion

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                FileStream fs = File.OpenRead(filePath);
                bool encrypted = false;
                byte[] magic = new byte[4];

                //Determin what kind of file it is
                int type = -1;

                if (filePath.EndsWith("3ds") || filePath.EndsWith("cci"))
                    type = 0;
                else if (filePath.EndsWith("sav") || filePath.EndsWith("bin"))
                    type = 1;
                else if (filePath.EndsWith("tmd") || filePath.EndsWith("tmd"))
                    type = 2;
                else //Autodetect by content
                {               
                    //TMD Check
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(magic, 0, 4);
                    if (magic[0] < 5 & magic[1] == 0 & magic[2] == 1 & magic[3] == 0)
                        type = 1;
                    else if (fs.Length >= 0x104) // > 256+4
                    {
                        //CCI CHECK
                        fs.Seek(0x100, SeekOrigin.Current);
                        fs.Read(magic, 0, 4);
                        if (magic[0] == 'N' && magic[1] == 'C' & magic[2] == 'S' & magic[3] == 'D')
                            type = 0;
                        else if (fs.Length >= 0x10000) // > 64kb
                        {
                            //SAVE Check
                            fs.Seek(0, SeekOrigin.Begin);
                            byte[] crcCheck = new byte[8 + 10 * (fs.Length / 0x1000 - 1)];
                            fs.Read(crcCheck, 0, crcCheck.Length);
                            fs.Read(magic, 0, 2);
                            byte[] calcCheck = CRC16.GetCRC(crcCheck);
                            if (magic[0] == calcCheck[0] && magic[1] == calcCheck[1]) //crc is ok then save
                                type = 1; //SAVE
                        }
                    }
                }
                if (type == 1)
                {
                    //check if encrypted
                    fs.Seek(0x1000, SeekOrigin.Begin); //Start of information
                    while ((fs.Length - fs.Position > 0x200) & !SaveTool.isSaveMagic(magic))
                    {
                        fs.Read(magic, 0, 4);
                        fs.Seek(0x200 - 4, SeekOrigin.Current);
                    }
                    encrypted = (fs.Length - fs.Position <= 0x200);

                }

                fs.Close();
                
                switch (type)
                {
                    case 0: OpenCCI(filePath); break;
                    case 1: OpenSave(filePath, encrypted); break;
                    case 2: OpenTMD(filePath); break;
                    default: MessageBox.Show("This file is unsupported!"); break;
                }
                btnSaveImage.Visible = (type == 1);
                btnSaveKey.Visible = (type == 1) && encrypted;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (currentContext is CCIContext)
            {
                CCIContext cxt = (CCIContext)currentContext;
                if (e.Node.Text.StartsWith("NCSD"))
                    showNCSD();
                else if (e.Node.Text.StartsWith("NCCH"))
                {
                    cxt.currentNcch = e.Node.Text[4] - '0';
                    showNCCH(cxt.currentNcch);
                }
                else if (e.Node.Text.StartsWith("Pla"))
                {
                    cxt.currentNcch = e.Node.Parent.Text[4] - '0';
                    showNCCHPlainRegion(cxt.currentNcch);
                }
            }
            else if (currentContext is SFContext)
            {
                SFContext cxt = (SFContext)currentContext;
                if (e.Node.Text.StartsWith("Save"))
                {
                    showSave();
                }
                else if (e.Node.Text.StartsWith("Part"))
                {
                    cxt.currentDifi = e.Node.Text[10] - '0';
                    showDifi();
                }
                else if (e.Node.Text.StartsWith("File"))
                {
                }
            }
            else if (currentContext is TMDContext)
            {
                TMDContext cxt = (TMDContext)currentContext;
                if (e.Node.Text.StartsWith("TMD C"))
                {
                    int i = e.Node.Text[16] - '0';
                    showTMDCertificate(i);
                }
                else if (e.Node.Text.StartsWith("TMD"))
                {
                    showTMD();
                }
                else if (e.Node.Text.StartsWith("Content I"))
                {
                    showTMDContentRecords();
                }
                else if (e.Node.Text.StartsWith("Content C"))
                {
                    showTMDContentChunks();
                }
            }
        }

        private byte[] parseKeyStringToByteArray(string str)
        {
            if (str.Equals("")) return new byte[0];
            if ((str.Length % 2 > 0) || (str.Length != 32)) return null; //must be a mutliple of 2
            byte[] retArray = new byte[str.Length / 2];
            try
            {
                for (int i = 0; i < str.Length; i += 2)
                {
                    retArray[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't parse key string!\n" + ex.Message);
                return null;
            }
            return retArray;
        }
        
        private void btnXOR_Click(object sender, EventArgs e)
        {
            (new frmKeyTool()).ShowDialog();
        }

        private void lblBrew_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(lblBrew.Text);
        }

        private void lvFileSystem_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem item = lvFileSystem.SelectedItems[0];
            if (item.Tag != null)
            {
                saveFileDialog.Filter = "All Files (*.*)|*.*";
                if (item.Tag is SFFileSystemEntry)
                {   SFFileSystemEntry entry = (SFFileSystemEntry)item.Tag;
                    SFContext cxt = (SFContext)currentContext;
                    saveFileDialog.FileName = charArrayToString(entry.Filename);
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        MemoryStream fs = new MemoryStream(cxt.image);

                        long offset = 0x2000; //Start of partitions
                        
                        //loop through difis to get to the SAVE	position
                        for (int i = 0; i < cxt.currentDifi; i++)	
                        {
                            offset += (long)((SFDIFIBlob)cxt.difis[i]).HashTableLength + (long)((SFDIFIBlob)cxt.difis[i]).FileSystemLength;
                        }		
                        offset += (long)(((SFDIFIBlob)cxt.difis[cxt.currentDifi]).HashTableLength);
                        
                        //calcualte the file's entry point
                        int fstBlockOffset = ((SFSave)cxt.saves[cxt.currentDifi]).FSTBlockOffset;
                        int fstExactOffset = ((SFSave)cxt.saves[cxt.currentDifi]).FSTExactOffset;
                        offset += (fstBlockOffset == 0 ? fstExactOffset : fstBlockOffset);

                        offset += entry.BlockOffset * 0x200;

                        fs.Seek(offset, SeekOrigin.Begin);

                        //read entry.filesize
                        byte[] fileBuffer = new byte[entry.FileSize];
                        fs.Read(fileBuffer, 0, fileBuffer.Length);
                        File.WriteAllBytes(saveFileDialog.FileName, fileBuffer);
                        fs.Close();
                    }
                }
                else if (item.Tag is CXI)
                {
                    CXI cxi = (CXI)item.Tag;
                    CCIContext cxt = (CCIContext)currentContext;
                    saveFileDialog.FileName = item.Text;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string strKey = InputBox.ShowDialog("Please Enter Key:\nPress OK with empty key to save encrypted");
                        if (strKey != null)
                        {
                            byte[] key = parseKeyStringToByteArray(strKey);

                            if (key == null)
                                MessageBox.Show("Error parsing key string, (must be a multiple of 2 and made of hex letters.");
                            else
                            {            
                                string inpath = openFileDialog.FileName;
                                FileStream infs = File.OpenRead(inpath);
                                bool isExeFS = item.Text.StartsWith("Exe");

                                long offset = isExeFS ? cxi.ExeFSOffset : cxi.RomFSOffset;
                                if (cxt.currentNcch == 0) offset += cxt.cci.FirstNCCHOffset;
                                else if (cxt.currentNcch == 1) offset += cxt.cci.SecondNCCHOffset;
                                else offset += cxt.cci.ThirdNCCHOffset;
                                offset *= 0x200; //media units

                                infs.Seek(offset, SeekOrigin.Begin);
                                long bufferSize = isExeFS ? cxi.ExeFSSize * 0x200 : cxi.RomFSSize * 0x200;
                                byte[] buffer = new byte[bufferSize];
                                infs.Read(buffer, 0, buffer.Length);
                                infs.Close();
                                if (key.Length > 0)
                                {
                                    AES128CTR aes = new AES128CTR(key);
                                    aes.Decrypt(buffer);
                                }
                                string outpath = saveFileDialog.FileName;
                                FileStream outfs = File.OpenWrite(outpath);
                                outfs.Write(buffer, 0, buffer.Length);
                                outfs.Close();
                            }
                        }
                    }
                }
            }
        }

        private void btnSaveKey_Click(object sender, EventArgs e)
        {
            SFContext cxt = (SFContext)currentContext;
            saveFileDialog.Filter = "Key file (*.key)|*.key|All Files|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, cxt.Key);
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
                saveFileDialog.Filter = "Image Files (*.bin)|*.bin";
                SFContext cxt = (SFContext)currentContext;
                saveFileDialog.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1).Replace('.','_') + ".bin";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(saveFileDialog.FileName, cxt.image);
        }

        private void frmExplorer_Load(object sender, EventArgs e)
        {
            Text = "3DS Explorer v." + Application.ProductVersion + " by elisherer";
        }

        private void lstInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lstInfo.SelectedIndices.Count > 0)
            {
                Clipboard.SetText(lstInfo.SelectedItems[0].SubItems[3].Text);
                MessageBox.Show("Value copied to clipboard!");
            }
        }

    }
}
