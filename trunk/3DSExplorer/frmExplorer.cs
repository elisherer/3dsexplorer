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
            for (i = 0; i < array.Length && i < 30; i++)
                arraystring += String.Format("{0:X2}", array[i]) + (i < array.Length - 1 ? " " : "");
            if (i == 30) return arraystring + "..."; //ellipsis
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

        private string toHexString(int digits, UInt64 number)
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
            makeNewListItem("DISA", "", "", "found @ 0x" + String.Format("{0:X4}", cxt.DisaOffset));
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
            makeNewListItem("0x000", "4", "FileSystem Offset (in block)", save.FSTOffset.ToString());
            makeNewListItem("0x000", "0x10", "Unknown", byteArrayToString(save.Unknown1));
            makeNewListItem("0x000", "4", "FileSystem Block Offset (block=0x200 bytes)", save.FSTBlockOffset.ToString());
            makeNewListItem("0x000", "0x08", "Unknown", byteArrayToString(save.Unknown2));
            makeNewListItem("0x000", "4", "FileSystem Exact Offset", save.FSTExactOffset.ToString());
            lvFileSystem.Clear();
            if (SaveTool.isSaveMagic(save.MagicSAVE))
            {
                lvFileSystem.Clear();
                ListViewItem lvItem;
                foreach (SFFileSystemEntry fse in (SFFileSystemEntry[])cxt.savesFiles[cxt.currentDifi])
                {
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
            twoBytes = CRC16.GetCRC(fileBuffer,0,ms.Position);
            if (crcBytes[0] != twoBytes[0] || crcBytes[1] != twoBytes[1])
            {
                MessageBox.Show("CRC Error");
                lstInfo.Clear();
                treeView.Nodes.Clear();
            }
            else
            {
                //get journal updates
                int lastMagic = cxt.JournalMagic;
                cxt.JournalSize = 0;
                SFLongSectorEntry lsEntry = new SFLongSectorEntry();
                while (lastMagic == cxt.JournalMagic)
                {
                    lsEntry = ReadStruct<SFLongSectorEntry>(ms);
                    lastMagic = lsEntry.Magic;
                    if (lastMagic == cxt.JournalMagic)
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
                cxt.DisaOffset = 0;
                while (!SaveTool.isDisaMagic(cxt.imageHeader.DISA.Magic)) //DISA not found, find it
                {
                    ims.Seek(0x1000 - Marshal.SizeOf(cxt.imageHeader), SeekOrigin.Current);
                    cxt.imageHeader = ReadStruct<SFImageHeader>(ims);
                    cxt.DisaOffset += 0x1000;
                }
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
                ims.Seek(cxt.DisaOffset + 0x2000, SeekOrigin.Begin);
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
                    /*
                    if (ims.Position % 0x1000 != 0) //go to the nearest block (0x1000)
                    {
                        ims.Seek(0x1000 - (ims.Position % 0x1000), SeekOrigin.Current);
                    }*/
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

        private void showTMD()
        {

            TMDContext cxt = (TMDContext)currentContext;
            if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256 || cxt.SignatureType == TMDSignatureType.RSA_4096_SHA256)
            {
                lstInfo.Items.Clear();
                TMDHeader head;
                if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256)
                    head = cxt.tmd2048.Header;
                else
                    head = cxt.tmd4096.Header;
                makeNewListItem("0x000", "4", "Signature Type", cxt.SignatureType.ToString());
                if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256)
                    makeNewListItem("0x004", "0x100", "RSA-2048 signature of the TMD", byteArrayToString(cxt.tmd2048.Signature));
                else
                    makeNewListItem("0x004", "0x200", "RSA-4096 signature of the TMD", byteArrayToString(cxt.tmd4096.Signature));
                
                makeNewListItem("", "", "Reserved0", byteArrayToString(head.Reserved0));
                makeNewListItem("", "", "Issuer", charArrayToString(head.Issuer));
                makeNewListItem("", "", "Version", head.Version.ToString());
                makeNewListItem("", "", "Car Crl Version", head.CarCrlVersion.ToString());
                makeNewListItem("", "", "Signer Version", head.SignerVersion.ToString());
                makeNewListItem("", "", "Reserved1", head.Reserved1.ToString());
                makeNewListItem("", "", "System Version", byteArrayToString(head.SystemVersion));
                makeNewListItem("", "", "Title ID", byteArrayToString(head.TitleID));
                makeNewListItem("", "", "Title Type", head.TitleType.ToString());
                makeNewListItem("", "", "Group ID", head.GroupID.ToString());
                makeNewListItem("", "", "Reserved2", byteArrayToString(head.Reserved2));
                makeNewListItem("", "", "Access Rights", head.AccessRights.ToString());
                makeNewListItem("", "", "Title Version", head.TitleVersion.ToString());
                makeNewListItem("", "", "Content Count", head.ContentCount.ToString());
                makeNewListItem("", "", "Boot Content", head.BootContent.ToString());
                makeNewListItem("", "", "Padding", head.Padding0.ToString());
                makeNewListItem("", "", "Content Info Records Hash", byteArrayToString(head.ContentInfoRecordsHash));
                /*
                for (int i = 0; i < 64; i++)
                {
                    makeNewListItem(i.ToString(), "", "Content Info Records Hash", byteArrayToString(head.ContentInfoRecordsHash));
                }
                //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
                //public TMDContentInfoRecord[] ContentInfoRecords;
                //TMDContentChunkRecord[ContentCount]
                */
                lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvFileSystem.Clear();
            }
        }

        private void OpenTMD(string path)
        {
            TMDContext cxt = new TMDContext();

            FileStream fs = File.OpenRead(path);

            byte[] intBytes = new byte[4];
            fs.Read(intBytes, 0, 4);
            cxt.SignatureType = (TMDSignatureType)BitConverter.ToInt32(intBytes, 0);

            //Build Tree
            treeView.Nodes.Clear();
            topNode = treeView.Nodes.Add("TMD");

            // Read the TMD
            switch (cxt.SignatureType)
            {
                case TMDSignatureType.RSA_2048_SHA1:
                    MessageBox.Show("This kind is unsupported!");
                    break;
                case TMDSignatureType.RSA_4096_SHA1:
                    MessageBox.Show("This kind is unsupported!");
                    break;
                case TMDSignatureType.RSA_2048_SHA256:
                    cxt.tmd2048 = ReadStruct<TMD2048>(fs);
                    cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                    for (int i = 0; i < cxt.ContentInfoRecords.Length; i++)
                        cxt.ContentInfoRecords[i] = ReadStruct<TMDContentInfoRecord>(fs);
                    cxt.chunks = new ArrayList();
                    for (int i = 0; i < cxt.tmd2048.Header.ContentCount; i++)
                        cxt.chunks.Add(ReadStruct<TMDContentChunkRecord>(fs));
                    break;
                case TMDSignatureType.RSA_4096_SHA256:
                    cxt.tmd4096 = ReadStruct<TMD4096>(fs);
                    cxt.ContentInfoRecords = new TMDContentInfoRecord[64];
                    for (int i = 0; i < cxt.ContentInfoRecords.Length; i++)
                        cxt.ContentInfoRecords[i] = ReadStruct<TMDContentInfoRecord>(fs);
                    cxt.chunks = new ArrayList();
                    for (int i = 0; i < cxt.tmd4096.Header.ContentCount; i++)
                        cxt.chunks.Add(ReadStruct<TMDContentChunkRecord>(fs));
                    break;
            }

            treeView.ExpandAll();

            fs.Close();

            currentContext = cxt;

            treeView.SelectedNode = topNode;
        }

        #endregion

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                //Determin what kind of file it is
                int type = -1;
                FileStream fs = File.OpenRead(filePath);
                byte[] magic = new byte[4];
                fs.Seek(0x100,SeekOrigin.Current);
                fs.Read(magic, 0, 4);
                if (magic[0] == 'N' && magic[1] == 'C' & magic[2] == 'S' & magic[3] == 'D')
                    type = 0; //CCI
                else
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(magic, 0, 4);
                    uint flag = BitConverter.ToUInt32(magic, 0);
                    if (flag < 0xff) //suppose to be small for save binary files
                        type = 1;
                    else if ((flag >= 0x00000100) && (flag <= 0x04000100))
                        type = 2;
                    else //can't decide (go with extension)
                    {
                        if (filePath.EndsWith("sav"))
                            type = 1;
                        else if (filePath.EndsWith("3ds"))
                            type = 0;
                        else
                            type = 2;
                    }
                }
                fs.Close();
                
                bool encrypted = false;
                switch (type)
                {
                    case 0: OpenCCI(filePath); break;
                    case 1: 
                        encrypted = (MessageBox.Show("Is this save file encrypted?","Question",MessageBoxButtons.YesNo)==DialogResult.Yes);
                        OpenSave(filePath, encrypted); 
                        break;
                    case 2: OpenTMD(filePath); break;
                    default: MessageBox.Show("This file is unsupported!"); break;
                }
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
                TMDContext tmd = (TMDContext)currentContext;
                if (e.Node.Text.StartsWith("TMD"))
                {
                    showTMD();
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
                        FileStream fs = File.OpenRead(filePath);
                        fs.Seek(0x2000 + cxt.DisaOffset, SeekOrigin.Begin);
                        //loop through difis to get to the SAVE
                        for (int i = 0; i < cxt.currentDifi; i++)
                        {
                            fs.Seek((long)(((SFDIFIBlob)cxt.difis[i]).HashTableLength + ((SFDIFIBlob)cxt.difis[i]).FileSystemLength),SeekOrigin.Current);
                        }
                        fs.Seek((long)(((SFDIFIBlob)cxt.difis[cxt.currentDifi]).HashTableLength), SeekOrigin.Current);
                        //then go FSTBlockOffset + entry.offset * 0x200
                        fs.Seek((long)(((SFSave)cxt.saves[cxt.currentDifi]).FSTBlockOffset + entry.BlockOffset * 0x200), SeekOrigin.Current);
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

    }
}
