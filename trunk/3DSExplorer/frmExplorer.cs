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
            LoadText(null);
        }

        public frmExplorer(string path)
        {
            InitializeComponent();
            openFile(path);
        }

        #region Stream Readers

        public byte[] ReadByteArray(Stream fs, int size)
        {
            byte[] buffer = new byte[size];
            fs.Read(buffer, 0, size);
            return buffer;
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

        #endregion

        private void makeNewListItem(string text, string sub1,string sub2, string sub3)
        {
            ListViewItem lvi = new ListViewItem(text);
            lvi.SubItems.Add(sub1);
            lvi.SubItems.Add(sub2);
            lvi.SubItems.Add(sub3);
            lstInfo.Items.Add(lvi);
        }

        private void makeEmptyListItem()
        {
            lstInfo.Items.Add(new ListViewItem(""));
        }

        #region ToString functions

        private string byteArrayToString(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length && i < 40; i++)
                arraystring += String.Format("{0:X2}", array[i]);
            if (i == 40) return arraystring + "..."; //ellipsis
            return arraystring;
        }

        private string byteArrayToStringSpaces(byte[] array)
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
            topNode = treeView.Nodes.Add("NCSD");
            LoadText(path);
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
            makeEmptyListItem();
            makeNewListItem("[Image]", "", "", "");
            makeNewListItem("", "0x10", "Image Hash", byteArrayToString(cxt.ImageHash));
            makeNewListItem("0x000", "4", "DISA Magic", charArrayToString(cxt.Disa.Magic));
            makeNewListItem("0x004", "4", "Unknown", cxt.Disa.Unknown0.ToString());
            makeNewListItem("0x008", "8", "Table Size", cxt.Disa.TableSize.ToString());
            makeNewListItem("0x010", "8", "Primary Table offset", cxt.Disa.PrimaryTableOffset.ToString());
            makeNewListItem("0x018", "8", "Secondary Table offset", cxt.Disa.SecondaryTableOffset.ToString());
            makeNewListItem("0x020", "8", "Table Length", cxt.Disa.TableLength.ToString());
            makeNewListItem("0x028", "8", "Padding?", cxt.Disa.Padding.ToString());
            makeNewListItem("0x030", "8", "Hashed size", cxt.Disa.HashSize.ToString());
            makeNewListItem("0x038", "0x30", "Unknown", byteArrayToString(cxt.Disa.Unknown1));
            makeNewListItem("0x03C", "4", "Active Table", ((cxt.Disa.ActiveTable & 1) == 1 ? "Primary" : "Secondary") + "  (=" + cxt.Disa.ActiveTable + ")");
            makeNewListItem("0x040", "0x20", "Hash", byteArrayToString(cxt.Disa.Hash));
            makeNewListItem("0x060", "0x74", "Unknown", byteArrayToString(cxt.Disa.Unknown2));
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileSystem.Items.Clear();
        }

        private void showDifi()
        {
            SFContext cxt = (SFContext)currentContext;
            lstInfo.Items.Clear();
            DIFI difi = cxt.Partitions[cxt.currentPartition].Difi;
            IVFC ivfc = cxt.Partitions[cxt.currentPartition].Ivfc;
            DPFS dpfs = cxt.Partitions[cxt.currentPartition].Dpfs;
            SAVE save = cxt.Save;

            makeNewListItem("0x000", "4", "Magic DIFI", charArrayToString(difi.Magic));
            makeNewListItem("0x004", "4", "Unknown 0", difi.Unknown0.ToString());

            makeNewListItem("0x008", "8", "IVFC Offset", difi.IVFCOffset.ToString());
            makeNewListItem("0x010", "8", "IVFC Size", difi.IVFCSize.ToString());
            makeNewListItem("0x018", "8", "DPFS Offset", difi.DPFSOffset.ToString());
            makeNewListItem("0x020", "8", "DPFS Size", difi.DPFSSize.ToString());
            makeNewListItem("0x028", "8", "Hash Offset", difi.HashOffset.ToString());
            makeNewListItem("0x030", "8", "Hash Size", difi.HashSize.ToString());
            makeNewListItem("0x038", "4", "Flags", toHexString(8,(ulong)difi.Flags));
            makeNewListItem("0x03C", "8", "File Base (for DATA partitions)", difi.FileBase.ToString());
            
            makeNewListItem("0x044", "4", "Magic IVFC", charArrayToString(ivfc.Magic));
            makeNewListItem("0x048", "0x20", "Unknown 2_0", byteArrayToString(ivfc.Unknown2_0));
            makeNewListItem("0x068", "0x1C", "Unknown 2_1", byteArrayToString(ivfc.Unknown2_1));

            makeNewListItem("0x084", "8", "HashTable Offset", ivfc.HashTableOffset.ToString());
            makeNewListItem("0x08C", "8", "HashTable Length", ivfc.HashTableLength.ToString());
            makeNewListItem("0x094", "8", "Unknown 2_2", ivfc.Unknown2_2.ToString());
            makeNewListItem("0x09C", "8", "FileSystem Offset", ivfc.FileSystemOffset.ToString());
            makeNewListItem("0x0A4", "8", "FileSystem Length", ivfc.FileSystemLength.ToString());

            makeNewListItem("0x0AC", "8", "HashedBlockLength", ivfc.HashedBlockLength + " (=" + (1 << (int)ivfc.HashedBlockLength) + ")");
            makeNewListItem("0x0B4", "8", "Unknown 3", ivfc.Unknown3.ToString());

            makeNewListItem("0x0BC", "4", "Magic DPFS", charArrayToString(dpfs.Magic));
            makeNewListItem("0x0C0", "0x20", "DPFS Data 0", byteArrayToString(dpfs.DPFSData_0));
            makeNewListItem("0x0E0", "0x20", "DPFS Data 1", byteArrayToString(dpfs.DPFSData_1));
            makeNewListItem("0x0FC", "8", "OffsetToNextPartition", dpfs.OffsetToNextPartition.ToString());
            makeNewListItem("0x104", "8", "DPFS Data Unknown", dpfs.DPFSUnknown.ToString());
            
            makeNewListItem("0x10C", "0x20", "Hash", byteArrayToString(cxt.Partitions[cxt.currentPartition].Hash));
            
            lvFileSystem.Items.Clear();
            
            if (cxt.currentPartition == 0)
            {
                makeEmptyListItem();
                makeNewListItem("[SAVE]", "", "", "");
                makeNewListItem("0x000", "4", "SAVE Magic", charArrayToString(save.Magic));
                makeNewListItem("0x004", "4", "Unknown 0", save.Unknown0.ToString());
                makeNewListItem("0x008", "4", "Unknown 1", save.Unknown1.ToString());
                makeNewListItem("0x00C", "4", "Unknown 2", save.Unknown2.ToString());
                makeNewListItem("0x010", "4", "Size of data partition [medias]", save.PartitionSize + " (=" + save.PartitionSize * 0x200 + ")");

                makeNewListItem("0x014", "4", "Unknown 3", save.Unknown3.ToString());
                makeNewListItem("0x018", "4", "Unknown 4", save.Unknown4.ToString());
                makeNewListItem("0x01C", "4", "Unknown 5", save.Unknown5.ToString());
                makeNewListItem("0x020", "4", "Unknown 6", save.Unknown6.ToString());
                makeNewListItem("0x024", "4", "Unknown 7", save.Unknown7.ToString());
                makeNewListItem("0x028", "4", "Unknown 8", save.Unknown8.ToString());

                makeNewListItem("0x02C", "0x2C", "Unknown 9", byteArrayToString(save.Unknown9));
                makeNewListItem("0x058", "4", "Local File Base Offset (form SAVE)", save.LocalFileBaseOffset.ToString());

                makeNewListItem("0x05C", "4", "Unknown 10", save.Unknown10.ToString());
                makeNewListItem("0x060", "4", "Size of data partition [medias] (used?)", save.Unknown11 + " (=" + save.Unknown11 * 0x200 + ")");
                makeNewListItem("0x064", "4", "Unknown 12", save.Unknown12.ToString());
                makeNewListItem("0x068", "4", "Unknown 13 Offset (form SAVE)", save.Unknown13.ToString());

                makeNewListItem("0x06C", "4", "FileSystem Table Block Offset (block=0x200 bytes)", save.FSTBlockOffset.ToString());
                makeNewListItem("0x070", "4", "Unknown 14", save.Unknown14.ToString());
                makeNewListItem("0x074", "4", "Unknown 15", save.Unknown15.ToString());
                makeNewListItem("0x078", "4", "FileSystem Table Exact Offset", save.FSTExactOffset.ToString());

                if (save.Magic != null & SaveTool.isSaveMagic(save.Magic))
                {
                    makeEmptyListItem();
                    makeNewListItem("[FILES]", "", "", "");

                    ListViewItem lvItem;
                    int i = 0;
                    foreach (FileSystemEntry fse in cxt.Files)
                    {
                        makeNewListItem(i++.ToString(), fse.FileSize.ToString(), charArrayToString(fse.Filename), "");
                        makeNewListItem("", "4", "NodeCount", fse.NodeCount.ToString());
                        makeNewListItem("", "4", "FileIndex", fse.Index.ToString());
                        makeNewListItem("", "4", "Magic? (Unknown 1)", fse.Magic.ToString() + "(=" + toHexString(4, (ulong)fse.Magic) + ")");
                        makeNewListItem("", "4", "FileBlockOffset", (fse.BlockOffset < 0 ? "Negative" : fse.BlockOffset.ToString()));
                        makeNewListItem("", "4", "Unknown 2", fse.Unknown2.ToString());
                        makeNewListItem("", "4", "Unknown 3", fse.Unknown3.ToString() + " (=" + toHexString(4, (ulong)fse.Unknown3) + ")");
                        makeNewListItem("", "4", "Unknown 4", fse.Unknown4.ToString());

                        lvItem = lvFileSystem.Items.Add(charArrayToString(fse.Filename) + "\n" + fse.FileSize + "b");
                        lvItem.ImageIndex = 0;
                        lvItem.Tag = fse;
                    }
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

            if (encrypted)
            {
                byte[] key = SaveTool.FindKey(fileBuffer);
                if (key == null)
                    MessageBox.Show("Can't find key to decrypt the binary file");
                else
                {
                    //SaveTool.XorByteArray(fileBuffer, key, 0x1000);
                    SaveTool.XorExperimental(fileBuffer, key, 0x1000);
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
                MessageBox.Show("CRC Error or Corrupt Save file");
                lvFileSystem.Items.Clear();
                lstInfo.Items.Clear();
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
               
                //rearragne by virtual
                cxt.image = new byte[fileBuffer.Length - 0x1000];
                for (int i = 0; i < mapping.Count; i++)
                    if ((int)mapping[i] != -1)
                        if ((((int)mapping[i] & 0x7F) * 0x1000 + 0x1000 <= fileBuffer.Length) && (i * 0x1000 + 0x1000 < cxt.image.Length)) //fix for partial images
                            Buffer.BlockCopy(fileBuffer, ((int)mapping[i] & 0x7F) * 0x1000, cxt.image, i * 0x1000, 0x1000);

                MemoryStream ims = new MemoryStream(cxt.image);

                cxt.ImageHash = ReadByteArray(ims, SFContext.IMAGE_HASH_LENGTH);
                //Go to start of image
                ims.Seek(0x100, SeekOrigin.Begin);
                cxt.Disa = ReadStruct<DISA>(ims);

                if (!SaveTool.isDisaMagic(cxt.Disa.Magic))
                {
                    MessageBox.Show("Corrupt Save File!");
                }
                else
                {
                    //Build Tree
                    treeView.Nodes.Clear();
                    topNode = treeView.Nodes.Add("Save Flash " + (encrypted ? "(Encrypted)" : ""));
                    LoadText(path);

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
                        cxt.Partitions[i].Difi = ReadStruct<DIFI>(ims);
                        ims.Seek(startOfDifi + cxt.Partitions[i].Difi.IVFCOffset, SeekOrigin.Begin);
                        cxt.Partitions[i].Ivfc = ReadStruct<IVFC>(ims);
                        ims.Seek(startOfDifi + cxt.Partitions[i].Difi.DPFSOffset, SeekOrigin.Begin);
                        cxt.Partitions[i].Dpfs = ReadStruct<DPFS>(ims);
                        ims.Seek(startOfDifi + cxt.Partitions[i].Difi.HashOffset, SeekOrigin.Begin);
                        cxt.Partitions[i].Hash = ReadByteArray(ims, Partition.HASH_LENGTH);
                        ims.Seek(4, SeekOrigin.Current); // skip garbage
                    }

                    //Get SAVE Partition
                    ims.Seek(0x2000, SeekOrigin.Begin);

                    //jump to backup if needed (SAVE partition is written twice)
                    //Always Needed???
                    //if ((cxt.imageHeader.DISA.ActiveTable & 1) == 1)
                    ims.Seek(cxt.Partitions[0].Dpfs.OffsetToNextPartition, SeekOrigin.Current);

                    long partStart = ims.Position;

                    //Get hashes table
                    ims.Seek(cxt.Partitions[0].Ivfc.HashTableOffset, SeekOrigin.Current);
                    cxt.Partitions[0].HashTable = new byte[cxt.Partitions[0].Ivfc.HashTableLength / 0x20][];
                    for (int i = 0; i < cxt.Partitions[0].HashTable.Length; i++)
                        cxt.Partitions[0].HashTable[i] = ReadByteArray(ims, 0x20);

                    topNode.Nodes.Add("SAVE Partition");
                    ims.Seek(partStart + cxt.Partitions[0].Ivfc.FileSystemOffset, SeekOrigin.Begin);
                    cxt.Save = ReadStruct<SAVE>(ims);
                    //add SAVE information (if exists) (suppose to...)
                    if (SaveTool.isSaveMagic(cxt.Save.Magic)) //read 
                    {
                        //go to FST
                        if ((cxt.Save.FSTBlockOffset != 0) || (cxt.Save.LocalFileBaseOffset != 0))
                        {
                            cxt.fileBase = partStart + cxt.Partitions[0].Ivfc.FileSystemOffset + cxt.Save.LocalFileBaseOffset;
                            ims.Seek(cxt.fileBase + cxt.Save.FSTBlockOffset * 0x200, SeekOrigin.Begin);
                        }
                        else //file base is remote
                        {
                            cxt.fileBase = cxt.Partitions[1].Difi.FileBase;
                            ims.Seek(partStart + cxt.Partitions[0].Ivfc.FileSystemOffset + cxt.Save.FSTExactOffset, SeekOrigin.Begin);
                        }

                        FileSystemEntry root = ReadStruct<FileSystemEntry>(ims);
                        if ((root.NodeCount > 1) && (root.Magic == 0)) //if has files
                        {
                            cxt.Files = new FileSystemEntry[root.NodeCount - 1];
                            for (int j = 0; j < cxt.Files.Length; j++)
                                cxt.Files[j] = ReadStruct<FileSystemEntry>(ims);
                        }
                        else //empty
                            cxt.Files = new FileSystemEntry[0];
                    }
                    else
                        cxt.Files = new FileSystemEntry[0]; //Not a legal SAVE filesystem
                    //go to next partition
                    ims.Seek(partStart + cxt.Partitions[0].Dpfs.OffsetToNextPartition, SeekOrigin.Begin);

                    /*/if we didn't use backup then we need to skip it...
                    if ((cxt.imageHeader.DISA.ActiveTable & 1) == 0)
                        ims.Seek(cxt.difis[0].OffsetToNextPartition, SeekOrigin.Current);
                    */

                    //There's always a 0x1000 padding (if DATA partitions exists)
                    ims.Seek(0x1000, SeekOrigin.Current);
                    partStart = ims.Position;

                    for (int i = 1; i < cxt.Partitions.Length; i++)
                    {
                        topNode.Nodes.Add("DATA Partition " + i);
                        
                        ims.Seek(cxt.Partitions[i].Ivfc.HashTableOffset, SeekOrigin.Current);
                        cxt.Partitions[i].HashTable = new byte[cxt.Partitions[i].Ivfc.HashTableLength / 0x20][];
                        for (int j = 0; j < cxt.Partitions[i].HashTable.Length; j++)
                            cxt.Partitions[i].HashTable[j] = ReadByteArray(ims, 0x20);
                        ims.Seek(partStart + cxt.Partitions[i].Dpfs.OffsetToNextPartition, SeekOrigin.Begin);
                        partStart = ims.Position;
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
                LoadText(path);
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

        private void openFile(string path)
        {
            filePath = path;
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
            menuFileSaveImageFile.Enabled = (type == 1);
            menuFileSaveKeyFile.Enabled = (type == 1) && encrypted;
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
                else if (e.Node.Text.StartsWith("SAVE"))
                {
                    cxt.currentPartition = 0;
                    showDifi();
                }
                else if (e.Node.Text.StartsWith("DATA"))
                {
                    cxt.currentPartition = e.Node.Text[15] - '0';
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

        private void lvFileSystem_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem item = lvFileSystem.SelectedItems[0];
            if (item.Tag != null)
            {
                saveFileDialog.Filter = "All Files (*.*)|*.*";
                if (item.Tag is FileSystemEntry)
                {   FileSystemEntry entry = (FileSystemEntry)item.Tag;
                    SFContext cxt = (SFContext)currentContext;
                    saveFileDialog.FileName = charArrayToString(entry.Filename);
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        MemoryStream fs = new MemoryStream(cxt.image);
                        fs.Seek(cxt.fileBase + entry.BlockOffset * 0x200, SeekOrigin.Begin);
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

        private void LoadText(string path)
        {
            Text = "3DS Explorer v." + Application.ProductVersion + " by elisherer" +  (path != null ? " (" + path.Substring(path.LastIndexOf('\\') + 1) + ")" : "");
        }

        private void lstInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lstInfo.SelectedIndices.Count > 0)
            {
                Clipboard.SetText(lstInfo.SelectedItems[0].SubItems[3].Text);
                MessageBox.Show("Value copied to clipboard!");
            }
        }

        #region Drag & Drop

        private void frmExplorer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        private void frmExplorer_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            openFile(files[0]);
        }

        #endregion

        #region MENU File

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                openFile(openFileDialog.FileName);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {

        }

        private void menuFileSaveImageFile_Click(object sender, EventArgs e)
        {
            SFContext cxt = (SFContext)currentContext;
            saveFileDialog.Filter = "Image Files (*.bin)|*.bin";
            saveFileDialog.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1).Replace('.', '_') + ".bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, cxt.image);
        }

        private void menuFileSaveKeyFile_Click(object sender, EventArgs e)
        {
            SFContext cxt = (SFContext)currentContext;
            saveFileDialog.Filter = "Key file (*.key)|*.key|All Files|*.*";
            saveFileDialog.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1).Replace('.', '_') + ".key";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, cxt.Key);
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region MENU Tools

        private void menuToolsXORTool_Click(object sender, EventArgs e)
        {
            (new frmXORTool()).ShowDialog();
        }

        private void menuToolsHashTool_Click(object sender, EventArgs e)
        {
            (new frmHashTool()).ShowDialog();
        }

        #endregion

        #region MENU Help

        private void menuHelpVisit3DBrew_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.3dbrew.org/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("This system doesn't support clicking a link...\n\n" + ex.Message);
            }
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
