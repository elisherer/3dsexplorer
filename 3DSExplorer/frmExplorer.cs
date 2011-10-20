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
using TreeListView;

namespace _3DSExplorer
{

    public partial class frmExplorer : Form
    {

        TreeNode topNode;
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

        #region AddListItem

        private void AddListItem(int offset, int size, string description, ulong value, string group)
        {
            ListViewItem lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add(value.ToString());
            lvi.SubItems.Add(toHexString(size * 2,value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        private void AddListItem(int offset, int size, string description, byte[] value, string group)
        {
            ListViewItem lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add("");
            lvi.SubItems.Add(byteArrayToString(value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        private void AddListItem(int offset, int size, string description, char[] value, string group)
        {
            ListViewItem lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add(charArrayToString(value));
            lvi.SubItems.Add("");
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }

        private void AddListItem(string offset, string size, string description, string value, string hexvalue, string group)
        {
            ListViewItem lvi = new ListViewItem(offset);
            lvi.SubItems.Add(size);
            lvi.SubItems.Add(description);
            lvi.SubItems.Add(value);
            lvi.SubItems.Add(hexvalue);
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        #endregion

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

        #region RomContext

        private void showNCSD()
        {
            RomContext cxt = (RomContext)currentContext;
            lstInfo.Items.Clear();
            AddListItem(0x000, 0x100, "RSA-2048 signature of the NCSD header [SHA-256]", cxt.cci.NCSDHeaderSignature,"lvgHash");
            AddListItem(0x100, 4, "Magic ID, always 'NCSD'", cxt.cci.MagicID, "lvgNCSD");
            AddListItem(0x104, 4, "Content size [medias]", cxt.cci.CCISize, "lvgNCSD");
            AddListItem(0x108, 8, "Title/Program ID", cxt.cci.TitleID, "lvgNCSD");
            AddListItem(0x120, 4, "Offset to the first NCCH [medias]", cxt.cci.FirstNCCHOffset, "lvgNCSD");
            AddListItem(0x124, 4, "Size of the first NCCH [medias]", cxt.cci.FirstNCCHSize, "lvgNCSD");
            AddListItem(0x130, 4, "Offset to the second NCCH [medias]", cxt.cci.SecondNCCHOffset, "lvgNCSD");
            AddListItem(0x134, 4, "Size of the second NCCH [medias]", cxt.cci.SecondNCCHSize , "lvgNCSD");
            AddListItem(0x158, 4, "Offset to the third NCCH [medias]", cxt.cci.ThirdNCCHOffset , "lvgNCSD");
            AddListItem(0x15C, 4, "Size of the third NCCH [medias]", cxt.cci.ThirdNCCHSize, "lvgNCSD");
            AddListItem(0x188, 8, "NCCH Flags", cxt.cci.NCCHFlags, "lvgNCSD");
            AddListItem(0x190, 8, "Partition ID of the first NCCH", cxt.cci.FirstNCCHPartitionID, "lvgNCSD");
            AddListItem(0x1A0, 8, "Partition ID of the second NCCH", cxt.cci.SecondNCCHPartitionID, "lvgNCSD");
            AddListItem(0x1C8, 8, "Partition ID of the third NCCH", cxt.cci.ThirdNCCHPartitionID, "lvgNCSD");
            AddListItem(0x200, 4, "Always 0xFFFFFFFF", cxt.cci.PaddingFF, "lvgNCSD");
            AddListItem(0x300, 4, "Used ROM size [bytes]", cxt.cci.UsedRomSize, "lvgNCSD");
            AddListItem(0x320, 16, "Unknown", cxt.cci.Unknown, "lvgNCSD");
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileTree.Nodes.Clear();;
        }

        private void showNCCH(int i)
        {
            RomContext cxt = (RomContext)currentContext;
            lstInfo.Items.Clear();
            AddListItem(0x000, 0x100, "RSA-2048 signature of the NCCH header [SHA-256]", cxt.cxis[i].NCCHHeaderSignature, "lvgHash");
            AddListItem(0x100, 4, "Magic ID, always 'NCCH'", cxt.cxis[i].MagicID, "lvgNCCH");
            AddListItem(0x104, 4, "Content size [medias]", cxt.cxis[i].CXISize, "lvgNCCH");

            AddListItem(0x108, 8, "Partition ID", cxt.cxis[i].PartitionID, "lvgNCCH");
            AddListItem(0x110, 2, "Maker Code" + " (=" + MakerResolver.Resolve(cxt.cxis[i].MakerCode) + ")", cxt.cxis[i].MakerCode, "lvgNCCH");
            AddListItem(0x112, 2, "Version", cxt.cxis[i].Version, "lvgNCCH");
            AddListItem(0x118, 8, "Program ID", cxt.cxis[i].ProgramID, "lvgNCCH");
            AddListItem(0x120, 1, "Temp Flag", cxt.cxis[i].TempFlag, "lvgNCCH");
            AddListItem(0x150, 0x10, "Product Code " + " (=" + GameTitleResolver.Resolve(cxt.cxis[i].ProductCode) + ")", cxt.cxis[i].ProductCode, "lvgNCCH");
            AddListItem(0x160, 0x20, "Extended Header Hash", cxt.cxis[i].ExtendedHeaderHash, "lvgNCCH");
            AddListItem(0x180, 4, "Extended header size", cxt.cxis[i].ExtendedHeaderSize, "lvgNCCH");
            AddListItem(0x188, 8, "Flags", cxt.cxis[i].Flags, "lvgNCCH");
            AddListItem(0x190, 4, "Plain region offset [medias]", cxt.cxis[i].PlainRegionOffset, "lvgNCCH");
            AddListItem(0x194, 4, "Plain region size [medias]", cxt.cxis[i].PlainRegionSize, "lvgNCCH");
            AddListItem(0x1A0, 4, "ExeFS offset [medias]", cxt.cxis[i].ExeFSOffset, "lvgNCCH");
            AddListItem(0x1A4, 4, "ExeFS size [medias]", cxt.cxis[i].ExeFSSize, "lvgNCCH");
            AddListItem(0x1A8, 4, "ExeFS hash region size [medias]", cxt.cxis[i].ExeFSHashRegionSize, "lvgNCCH");
            AddListItem(0x1B0, 4, "RomFS offset [medias]", cxt.cxis[i].RomFSOffset, "lvgNCCH");
            AddListItem(0x1B4, 4, "RomFS size [medias]", cxt.cxis[i].RomFSSize, "lvgNCCH");
            AddListItem(0x1B8, 4, "RomFS hash region size [medias]", cxt.cxis[i].RomFSHashRegionSize, "lvgNCCH");
            AddListItem(0x1C0, 0x20, "ExeFS superblock hash", cxt.cxis[i].ExeFSSuperBlockhash, "lvgNCCH");
            AddListItem(0x1E0, 0x20, "RomFS superblock hash", cxt.cxis[i].RomFSSuperBlockhash, "lvgNCCH");
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileTree.Nodes.Clear();;
            TreeNode lvItem;
            if (cxt.cxis[i].ExeFSSize > 0)
            {
                lvItem = lvFileTree.Nodes.Add("ExeFS" + i + ".bin");
                lvFileTree.AddSubItem(lvItem,(cxt.cxis[i].ExeFSSize * 0x200).ToString());
                lvFileTree.AddSubItem(lvItem, toHexString(6, (ulong)(cxt.cxis[i].ExeFSOffset * 0x200)));
                lvItem.ImageIndex = 0;
                lvItem.Tag = cxt.cxis[i];
            }
            if (cxt.cxis[i].RomFSSize > 0)
            {
                lvItem = lvFileTree.Nodes.Add("RomFS" + i + ".bin");
                lvFileTree.AddSubItem(lvItem, (cxt.cxis[i].RomFSSize * 0x200).ToString());
                lvFileTree.AddSubItem(lvItem, toHexString(6, (ulong)(cxt.cxis[i].RomFSOffset * 0x200)));
                lvItem.ImageIndex = 0;
                lvItem.Tag = cxt.cxis[i];
            }
        }

        private void showNCCHPlainRegion(int i)
        {
            RomContext cxt = (RomContext)currentContext;
            lstInfo.Items.Clear();
            for (int j = 0 ;j < cxt.cxiprs[i].PlainRegionStrings.Length ; j++)
                AddListItem(0, 4, cxt.cxiprs[i].PlainRegionStrings[j], (ulong)cxt.cxiprs[i].PlainRegionStrings[j].Length, "lvgPlainRegions");

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileTree.Nodes.Clear();;
        }
        #endregion

        #region SFContext

        private void showImage()
        {
            SFContext cxt = (SFContext)currentContext;
            DISA disa = cxt.Disa;
            lstInfo.Items.Clear();
            AddListItem(0x000, 4, "Unknown 1", cxt.fileHeader.Unknown1, "lvgSaveFlash");
            AddListItem(0x004, 4, "Unknown 2", cxt.fileHeader.Unknown2, "lvgSaveFlash");
            AddListItem(0, 4, "** Blockmap length", (ulong)cxt.Blockmap.Length, "lvgSaveFlash");
            AddListItem(0, 4, "** Journal size", cxt.JournalSize, "lvgSaveFlash");

            AddListItem(0, 0x10, "** Image Hash", cxt.ImageHash,"lvgImage");

            AddListItem(0x000, 4, "DISA Magic", disa.Magic, "lvgImage");
            AddListItem(0x004, 4, "Unknown", disa.Unknown0, "lvgImage");
            AddListItem(0x008, 8, "Table Size", disa.TableSize, "lvgImage");
            AddListItem(0x010, 8, "Primary Table offset", disa.PrimaryTableOffset, "lvgImage");
            AddListItem(0x018, 8, "Secondary Table offset", disa.SecondaryTableOffset, "lvgImage");
            AddListItem(0x020, 8, "Table Length", disa.TableLength, "lvgImage");
            AddListItem(0x028, 8, "SAVE Entry Table offset", disa.SAVEEntryOffset, "lvgImage");
            AddListItem(0x030, 8, "SAVE Entry Table length", disa.SAVEEntryLength, "lvgImage");
            AddListItem(0x038, 8, "DATA Entry Table offset", disa.DATAEntryOffset,"lvgImage");
            AddListItem(0x040, 8, "DATA Entry Table length", disa.DATAEntryLength, "lvgImage");
            AddListItem(0x048, 8, "SAVE Partition Offset", disa.SAVEPartitionOffset, "lvgImage");
            AddListItem(0x050, 8, "SAVE Partition Length", disa.SAVEPartitionLength, "lvgImage");
            AddListItem(0x058, 8, "DATA Partition Offset", disa.DATAPartitionOffset, "lvgImage");
            AddListItem(0x060, 8, "DATA Partition Length", disa.DATAPartitionLength, "lvgImage");
            AddListItem(0x068, 4, "Active Table is " + ((disa.ActiveTable & 1) == 1 ? "Primary" : "Secondary"), disa.ActiveTable, "lvgImage");
            AddListItem(0x06C, 0x20, "Hash", disa.Hash, "lvgImage");
            AddListItem(0x08C, 4, "Zero Padding 0(to 8 bytes)", disa.ZeroPad0, "lvgImage");
            AddListItem(0x090, 4, "Flag 0 ?", disa.Flag0, "lvgImage");
            AddListItem(0x094, 4, "Zero Padding 1(to 8 bytes)", disa.ZeroPad1, "lvgImage");
            AddListItem(0x098, 4, "Unknown 1", disa.Unknown1, "lvgImage");
            AddListItem(0x09C, 4, "Unknown 2 (Magic?)", disa.Unknown2, "lvgImage");
            AddListItem(0x0A0, 8, "Data FS Length", disa.DataFsLength, "lvgImage");
            AddListItem(0x0A8, 8, "Unknown 3", disa.Unknown3, "lvgImage");
            AddListItem(0x0B0, 4, "Unknown 4", disa.Unknown4, "lvgImage");
            AddListItem(0x0B4, 4, "Unknown 5", disa.Unknown5, "lvgImage");
            AddListItem(0x0B8, 4, "Unknown 6", disa.Unknown6, "lvgImage");
            AddListItem(0x0BC, 4, "Unknown 7", disa.Unknown7, "lvgImage");
            AddListItem(0x0C0, 4, "Unknown 8", disa.Unknown8, "lvgImage");
            AddListItem(0x0C4, 4, "Flag 1 ?", disa.Flag1, "lvgImage");
            AddListItem(0x0C8, 4, "Flag 2 ?", disa.Flag2, "lvgImage");
            AddListItem(0x0CC, 4, "Flag 3 ?", disa.Flag3, "lvgImage");
            AddListItem(0x0D0, 4, "Flag 4 ?", disa.Flag4, "lvgImage");
            AddListItem(0x0D4, 4, "Unknown 14", disa.Unknown14, "lvgImage");
            AddListItem(0x0D8, 4, "Flag 5 ?", disa.Flag5, "lvgImage");
            AddListItem(0x0DC, 4, "Unknown 16", disa.Unknown16, "lvgImage");
            AddListItem(0x0E0, 4, "Magic 17", disa.Magic17, "lvgImage");
            AddListItem(0x0E4, 4, "Unknown 18", disa.Unknown18, "lvgImage");
            AddListItem(0x0E8, 4, "Flag 6 ?", disa.Flag6, "lvgImage");
            AddListItem(0x0EC, 4, "Flag 7 ?", disa.Flag7, "lvgImage");
            AddListItem(0x0F0, 4, "Flag 8 ?", disa.Flag8, "lvgImage");
            AddListItem(0x0F4, 4, "Unknown 21", disa.Unknown21, "lvgImage");
            AddListItem(0x0F8, 4, "Unknown 22", disa.Unknown22, "lvgImage");
            AddListItem(0x0FC, 4, "Unknown 23", disa.Unknown23, "lvgImage");
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void showTables()
        {
            SFContext cxt = (SFContext)currentContext;
            lstInfo.Items.Clear();

            if (SaveTool.isSaveMagic(cxt.Save.Magic))
            {
                for (int i = 0; i < cxt.FilesMap.Length; i++)
                    AddListItem(i, 4, "UInt32", cxt.FilesMap[i], "lvgFiles");
                for (int i = 0; i < cxt.FoldersMap.Length; i++)
                    AddListItem(i, 4, "UInt32", cxt.FoldersMap[i], "lvgFolders");

                AddListItem("", "", "Start", "Start:" + (cxt.BlockMap[0].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[0].EndBlock & 0xff), "Start:" + cxt.BlockMap[0].StartBlock.ToString("X8") + ", End: " + cxt.BlockMap[0].EndBlock.ToString("X8"), "lvgUnknown");
                for (int i = 1; i < cxt.BlockMap.Length - 1; i++)
                    AddListItem("", (i - 1).ToString(), "Block " + i + (cxt.BlockMap[i].EndBlock == 0x80000000 && cxt.BlockMap[i].StartBlock == 0x80000000 ? " (Start of data)" : ""), "Start:" + (cxt.BlockMap[i].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[i].EndBlock & 0xff), "Start:" + cxt.BlockMap[i].StartBlock.ToString("X8") + ", End: " + cxt.BlockMap[i].EndBlock.ToString("X8"), "lvgUnknown");
                AddListItem("", "", "End", "", "Start:" + (cxt.BlockMap[cxt.BlockMap.Length - 1].StartBlock & 0xff) + ", End: " + (cxt.BlockMap[cxt.BlockMap.Length - 1].EndBlock & 0xff), "lvgUnknown");
            }
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void showPartition()
        {
            SFContext cxt = (SFContext)currentContext;
            lstInfo.Items.Clear();
            DIFI difi = cxt.Partitions[cxt.currentPartition].Difi;
            IVFC ivfc = cxt.Partitions[cxt.currentPartition].Ivfc;
            DPFS dpfs = cxt.Partitions[cxt.currentPartition].Dpfs;
            SAVE save = cxt.Save;

            AddListItem(0x000, 4, "Magic DIFI", difi.Magic, "lvgDifi");
            AddListItem(0x004, 4, "Magic Padding", difi.MagicPadding, "lvgDifi");
            AddListItem(0x008, 8, "IVFC Offset", difi.IVFCOffset, "lvgDifi");
            AddListItem(0x010, 8, "IVFC Size", difi.IVFCSize, "lvgDifi");
            AddListItem(0x018, 8, "DPFS Offset", difi.DPFSOffset, "lvgDifi");
            AddListItem(0x020, 8, "DPFS Size", difi.DPFSSize, "lvgDifi");
            AddListItem(0x028, 8, "Hash Offset", difi.HashOffset, "lvgDifi");
            AddListItem(0x030, 8, "Hash Size", difi.HashSize, "lvgDifi");
            AddListItem(0x038, 4, "Flags", difi.Flags, "lvgDifi");
            AddListItem(0x03C, 8, "File Base (for DATA partitions)", difi.FileBase, "lvgDifi");

            AddListItem(0x000, 4, "Magic IVFC", ivfc.Magic, "lvgIvfc");
            AddListItem(0x004, 4, "Magic Padding", ivfc.MagicPadding, "lvgIvfc");
            AddListItem(0x008, 8, "Unknown 1", ivfc.Unknown1, "lvgIvfc");
            AddListItem(0x010, 8, "FirstHash Offset", ivfc.FirstHashOffset, "lvgIvfc");
            AddListItem(0x018, 8, "FirstHash Length", ivfc.FirstHashLength, "lvgIvfc");
            AddListItem(0x020, 8, "FirstHash Block"  + " (=" + (1 << (int)ivfc.FirstHashBlock) + ")", ivfc.FirstHashBlock, "lvgIvfc");
            AddListItem(0x028, 8, "SecondHash Offset", ivfc.SecondHashOffset, "lvgIvfc");
            AddListItem(0x030, 8, "SecondHash Length", ivfc.SecondHashLength, "lvgIvfc");
            AddListItem(0x038, 8, "SecondHash Block" + " (=" + (1 << (int)ivfc.SecondHashBlock) + ")", ivfc.SecondHashBlock, "lvgIvfc");
            AddListItem(0x040, 8, "HashTable Offset", ivfc.HashTableOffset, "lvgIvfc");
            AddListItem(0x048, 8, "HashTable Length", ivfc.HashTableLength, "lvgIvfc");
            AddListItem(0x050, 8, "HashTable Block" + " (=" + (1 << (int)ivfc.HashTableBlock) + ")", ivfc.HashTableBlock, "lvgIvfc");
            AddListItem(0x058, 8, "FileSystem Offset", ivfc.FileSystemOffset, "lvgIvfc");
            AddListItem(0x060, 8, "FileSystem Length", ivfc.FileSystemLength, "lvgIvfc");
            AddListItem(0x068, 8, "FileSystem Block" + " (=" + (1 << (int)ivfc.FileSystemBlock) + ")", ivfc.FileSystemBlock, "lvgIvfc");
            AddListItem(0x070, 8, "Unknown 3 (?=0x78)", ivfc.Unknown3, "lvgIvfc");

            AddListItem(0x000, 4, "Magic DPFS", dpfs.Magic, "lvgDpfs");
            AddListItem(0x004, 4, "Magic Padding", dpfs.MagicPadding, "lvgDpfs");
            AddListItem(0x008, 8, "First Table Offset", dpfs.FirstTableOffset, "lvgDpfs");
            AddListItem(0x010, 8, "First Table Length", dpfs.FirstTableLength, "lvgDpfs");
            AddListItem(0x018, 8, "First Table Block", dpfs.FirstTableBlock, "lvgDpfs");
            AddListItem(0x020, 8, "Second Table Offset", dpfs.SecondTableOffset, "lvgDpfs");
            AddListItem(0x028, 8, "Second Table Length", dpfs.SecondTableLength, "lvgDpfs");
            AddListItem(0x030, 8, "Second Table Block", dpfs.SecondTableBlock, "lvgDpfs");
            AddListItem(0x038, 8, "Offset to Data", dpfs.OffsetToData, "lvgDpfs");
            AddListItem(0x040, 8, "Data Length", dpfs.DataLength, "lvgDpfs");
            AddListItem(0x048, 8, "Data Block", dpfs.DataBlock, "lvgDpfs");

#if DEBUG
            AddListItem(0x000, 4, "* First Flag", cxt.Partitions[cxt.currentPartition].FirstFlag, "lvgDpfs");
            AddListItem(0x000, 4, "* First Flag Dupe", cxt.Partitions[cxt.currentPartition].FirstFlagDupe, "lvgDpfs");
            AddListItem(0x000, 4, "* Second Flag", cxt.Partitions[cxt.currentPartition].SecondFlag, "lvgDpfs");
            AddListItem(0x000, 4, "* Second Flag Dupe", cxt.Partitions[cxt.currentPartition].SecondFlagDupe, "lvgDpfs");
#endif
            
            AddListItem(0x000, 0x20, "Hash", cxt.Partitions[cxt.currentPartition].Hash, "lvgHash");
            
            if (cxt.currentPartition == 0)
            {
                AddListItem(0x000, 4, "SAVE Magic", save.Magic, "lvgSave");
                AddListItem(0x004, 4, "Magic Padding", save.MagicPadding, "lvgSave");
                AddListItem(0x008, 8, "Unknown 1 (?=0x020)", save.Unknown1, "lvgSave");
                AddListItem(0x010, 8, "Size of data Partition [medias]", save.PartitionSize, "lvgSave");
                AddListItem(0x018, 4, "Partition Media Size", save.PartitionMediaSize, "lvgSave");
                AddListItem(0x01C, 8, "Unknown 3 (?=0x000)", save.Unknown3, "lvgSave");
                AddListItem(0x024, 4, "Unknown 4 (?=0x200)", save.Unknown4, "lvgSave");
                AddListItem(0x028, 8, "File Map Offset", save.FileMapOffset, "lvgSave");
                AddListItem(0x030, 4, "File Map Size", save.FileMapSize, "lvgSave");
                AddListItem(0x034, 4, "File Map MediaSize", save.FileMapMediaSize, "lvgSave");
                AddListItem(0x038, 8, "Folder Map Offset", save.FolderMapOffset, "lvgSave");
                AddListItem(0x040, 4, "Folder Map Size", save.FolderMapSize, "lvgSave");
                AddListItem(0x044, 4, "Folder Map Media Size", save.FolderMapMediaSize, "lvgSave");
                AddListItem(0x048, 8, "Block Map Offset", save.BlockMapOffset, "lvgSave");
                AddListItem(0x050, 4, "Block Map Size", save.BlockMapSize, "lvgSave");
                AddListItem(0x054, 4, "Block Map Media Size", save.BlockMapMediaSize, "lvgSave");
                AddListItem(0x058, 8, "Filestore Offset (from SAVE)", save.FileStoreOffset, "lvgSave");
                AddListItem(0x060, 4, "Filestore Length (medias)", save.FileStoreLength, "lvgSave");
                AddListItem(0x064, 4, "Filestore Media", save.FileStoreMedia, "lvgSave");
                AddListItem(0x068, 4, "Folders Table offset (medias/exact)", save.FolderTableOffset, "lvgSave");
                AddListItem(0x06C, 4, "Folders Table Length (medias)", save.FolderTableLength, "lvgSave");
                AddListItem(0x070, 4, "Folders Table Unknown", save.FolderTableUnknown, "lvgSave");
                AddListItem(0x074, 4, "Folders Table Media Size", save.FolderTableMedia, "lvgSave");
                AddListItem(0x078, 4, "Files Table Offset (medias/exact)", save.FSTOffset, "lvgSave");
                AddListItem(0x07C, 4, "Files Table Length", save.FSTLength, "lvgSave");
                AddListItem(0x080, 4, "Files Table Unknown", save.FSTUnknown, "lvgSave");
                AddListItem(0x084, 4, "Files Table Media Size", save.FSTMedia, "lvgSave");

                if (SaveTool.isSaveMagic(save.Magic))
                {
                    int i = 1;
                    foreach (FileSystemFolderEntry fse in cxt.Folders)
                        AddListItem(i++.ToString(),
                                    fse.Index.ToString(),
                                    charArrayToString(fse.FolderName),
                                    fse.ParentFolderIndex.ToString(),
                                    toHexString(8, fse.LastFileIndex),
                                    "lvgFolders");
                    i = 1;
                    foreach (FileSystemFileEntry fse in cxt.Files)
                        AddListItem(i++.ToString(),
                                    fse.BlockOffset.ToString(),
                                    "[" + fse.Index + "] " + charArrayToString(fse.Filename) + ", (" + fse.FileSize + "b)" ,
                                    fse.ParentFolderIndex.ToString(),
                                    toHexString(8, fse.Unknown2) + " " + toHexString(8, fse.Magic),
                                    "lvgFiles");
                }
            }
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
                AddListItem(i, 4, "Content ID", cr.ContentID, "lvgTmd");
                AddListItem(0, 2, "Content Index", cr.ContentIndex, "lvgTmd");
                AddListItem(0, 2, "Content Type (=" + TMDTool.typeToString(cr.ContentType) + ")", cr.ContentType, "lvgTmd");
                AddListItem(0, 8, "Content Size", cr.ContentSize, "lvgTmd");
                AddListItem(0, 32, "Content Hash", cr.ContentHash, "lvgTmd");
            }
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void showTMDContentRecords()
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            for (int i = 0; i < 64; i++)
            {
                AddListItem(i * 36, 2, "Content Command Count", cxt.ContentInfoRecords[i].ContentCommandCount, "lvgTmd");
                AddListItem(i * 36 + 2, 2, "Content Index Offset", cxt.ContentInfoRecords[i].ContentIndexOffset, "lvgTmd");
                AddListItem(i * 36 + 4, 32, "Next Content Hash", cxt.ContentInfoRecords[i].NextContentHash, "lvgTmd");
            }
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void showTMD()
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            TMDHeader head = cxt.head;
            AddListItem(0, 4, "Signature Type", (ulong)cxt.SignatureType, "lvgTmd");
            int off = 4;
            if (cxt.SignatureType == TMDSignatureType.RSA_2048_SHA256 || cxt.SignatureType == TMDSignatureType.RSA_2048_SHA1)
            {
                AddListItem(off, 0x100, "RSA-2048 signature of the TMD", cxt.tmdSHA, "lvgTmd");
                off += 0x100;
            }
            else
            {
                AddListItem(off, 0x200, "RSA-4096 signature of the TMD", cxt.tmdSHA, "lvgTmd");
                off += 0x200;
            }
            AddListItem(off,    60, "Reserved0", head.Reserved0, "lvgTmd");
            AddListItem(off+60, 64, "Issuer", head.Issuer, "lvgTmd");
            AddListItem(off+124, 4, "Version", head.Version, "lvgTmd");
            AddListItem(off+128, 1, "Car Crl Version", head.CarCrlVersion, "lvgTmd");
            AddListItem(off+129, 1, "Signer Version", head.SignerVersion, "lvgTmd");
            AddListItem(off+130, 1, "Reserved1", head.Reserved1, "lvgTmd");
            AddListItem(off+131, 8, "System Version", head.SystemVersion, "lvgTmd");
            AddListItem(off+139, 8, "Title ID", head.TitleID, "lvgTmd");
            AddListItem(off+147, 4, "Title Type", head.TitleType, "lvgTmd");
            AddListItem(off+151, 2, "Group ID", head.GroupID, "lvgTmd");
            AddListItem(off+153, 62, "Reserved2", head.Reserved2, "lvgTmd");
            AddListItem(off+215, 4, "Access Rights", head.AccessRights, "lvgTmd");
            AddListItem(off+219, 2, "Title Version", head.TitleVersion, "lvgTmd");
            AddListItem(off+221, 2, "Content Count", head.ContentCount, "lvgTmd");
            AddListItem(off+223, 2, "Boot Content", head.BootContent, "lvgTmd");
            AddListItem(off+225, 2, "Padding", head.Padding0, "lvgTmd");
            AddListItem(off+227, 32, "Content Info Records Hash", head.ContentInfoRecordsHash, "lvgTmd");

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileTree.Nodes.Clear();;
        }

        private void showTMDCertificate(int i)
        {
            TMDContext cxt = (TMDContext)currentContext;
            lstInfo.Items.Clear();
            TMDCertContext ccxt = (TMDCertContext)cxt.certs[i];
            TMDCertificate cert = ccxt.cert;
            AddListItem(0, 4, "Signature Type", (ulong)ccxt.SignatureType, "lvgTmd");
            int off = 4;
            if (ccxt.SignatureType == TMDSignatureType.RSA_2048_SHA256 || ccxt.SignatureType == TMDSignatureType.RSA_2048_SHA1)
            {
                AddListItem(off, 0x100, "RSA-2048 signature of the TMD", ccxt.tmdSHA, "lvgTmd");
                off += 0x100;
            }
            else
            {
                AddListItem(off, 0x200, "RSA-4096 signature of the TMD", ccxt.tmdSHA, "lvgTmd");
                off += 0x200;
            }
            AddListItem(off, 60, "Reserved0", cert.Reserved0, "lvgTmd");
            AddListItem(off+60, 64, "Issuer", cert.Issuer, "lvgTmd");
            AddListItem(off+124, 4, "Tag", cert.Tag, "lvgTmd");
            AddListItem(off+128, 64, "Name", cert.Name, "lvgTmd");
            AddListItem(off+292, 0x104, "Key", cert.Key, "lvgTmd");
            AddListItem(off+552, 2, "Unknown0", cert.Unknown1, "lvgTmd");
            AddListItem(off+554, 2, "Unknown1", cert.Unknown2, "lvgTmd");
            AddListItem(off+556, 52, "Padding", cert.Padding, "lvgTmd");

            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvFileTree.Nodes.Clear();;
        }
        #endregion

        private void openFile(string path)
        {
            filePath = path;
            FileStream fs = File.OpenRead(filePath);
            byte[] magic = new byte[4];
            bool encrypted = false;

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
                case 0: //Rom
                    RomContext rcxt = RomTool.Open(filePath);
                    LoadText(filePath);
                    //Build Tree
                    treeView.Nodes.Clear();
                    topNode = treeView.Nodes.Add("Rom");
                    for (int i=0; i < rcxt.cxis.Length ; i++) //ADD CXIs
                        if (rcxt.cxis[i].CXISize > 0)
                        {
                            topNode.Nodes.Add("NCCH " + i + " (" + (new String(rcxt.cxis[i].ProductCode)).Substring(0, 10) + ")");
                            if (rcxt.cxis[i].PlainRegionSize > 0) //Add PlainRegions
                                topNode.Nodes[topNode.Nodes.Count - 1].Nodes.Add("PlainRegion");
                        }
                    treeView.ExpandAll();
                    currentContext = rcxt;    
                    treeView.SelectedNode = topNode;
                    break;
                case 1:
                    string errMsg = null;
                    SFContext scxt = SaveTool.Open(filePath, ref errMsg);
                    if (scxt == null)
                        MessageBox.Show("Error: " + errMsg);
                    else
                    {
                        LoadText(filePath);
                        //Build Tree
                        treeView.Nodes.Clear();
                        topNode = treeView.Nodes.Add("Save Flash " + (scxt.Encrypted ? "(Encrypted)" : ""));
                        topNode.Nodes.Add("SAVE Partition").Nodes.Add("Maps");
                        if (scxt.isData)
                            topNode.Nodes.Add("DATA Partition");
                        lvFileTree.Nodes.Clear();;
                        TreeNode[] folders = new TreeNode[scxt.Folders.Length];
                        //add root folder
                        folders[0] = lvFileTree.Nodes.Add("ROOT");
                        folders[0].ImageIndex = 1;
                        folders[0].SelectedImageIndex = 1;
                        //add folders
                        if (scxt.Folders.Length > 1)
                            for (int i = 1; i < scxt.Folders.Length; i++)
                            {
                                folders[i] = folders[scxt.Folders[i].ParentFolderIndex-1].Nodes.Add(charArrayToString(scxt.Folders[i].FolderName));
                                folders[i].ImageIndex = 1;
                                folders[i].SelectedImageIndex = 1;
                            }
                        //add files
                        if (scxt.Files.Length > 0)
                        {
                            TreeNode lvItem;
                            for (int i = 0; i < scxt.Files.Length; i++)
                            {
                                lvItem = folders[scxt.Files[i].ParentFolderIndex-1].Nodes.Add(charArrayToString(scxt.Files[i].Filename));
                                lvFileTree.AddSubItem(lvItem, scxt.Files[i].FileSize.ToString());
                                lvFileTree.AddSubItem(lvItem, toHexString(6, (ulong)(scxt.fileBase + 0x200 * scxt.Files[i].BlockOffset)));
                                lvItem.ImageIndex = 0;
                                lvItem.Tag = scxt.Files[i];
                            }
                        }
                        folders[0].ExpandAll();
                        treeView.ExpandAll();
                        currentContext = scxt;
                        treeView.SelectedNode = topNode;
                    }
                    break;
                case 2: //TMD
                    TMDContext tcxt = TMDTool.Open(filePath);
                    if (tcxt == null)
                        MessageBox.Show("This kind of TMD is unsupported.");
                    else
                    {
                        LoadText(filePath);
                        //Build Tree
                        treeView.Nodes.Clear();
                        topNode = treeView.Nodes.Add("TMD");
                        topNode.Nodes.Add("Content Info Records");
                        topNode.Nodes.Add("Content Chunk Records");
                        for (int i = 0; i < tcxt.certs.Count; i++)
                            topNode.Nodes.Add("TMD Certificate " + i);
                        treeView.ExpandAll();
                        currentContext = tcxt;
                        treeView.SelectedNode = topNode;
                    }
                    break;
                default: MessageBox.Show("This file is unsupported!"); break;
            }
            menuFileSave.Enabled = (type == 1);
            menuFileSaveImageFile.Enabled = (type == 1);
            menuFileSaveKeyFile.Enabled = (type == 1) && encrypted;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (currentContext is RomContext)
            {
                RomContext cxt = (RomContext)currentContext;
                if (e.Node.Text.StartsWith("Rom"))
                    showNCSD();
                else if (e.Node.Text.StartsWith("NCCH"))
                {
                    cxt.currentNcch = e.Node.Text[5] - '0';
                    showNCCH(cxt.currentNcch);
                }
                else if (e.Node.Text.StartsWith("Pla"))
                {
                    cxt.currentNcch = e.Node.Parent.Text[5] - '0';
                    showNCCHPlainRegion(cxt.currentNcch);
                }
            }
            else if (currentContext is SFContext)
            {
                SFContext cxt = (SFContext)currentContext;
                switch (e.Node.Text[2])
                {
                    case 'v': //Save
                        showImage();
                        break;
                    case 'V': //SAVE/DATA Partition
                        cxt.currentPartition = e.Node.Text[2]=='V' ? 0 : 1;
                        showPartition();
                        break;
                    case 'p': //Maps
                        showTables();
                        break;
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

        private void lvFileTree_DoubleClick(object sender, EventArgs e)
        {
            TreeNode tn = lvFileTree.TreeView.SelectedNode;
            if (tn != null)
            {
                if (tn.Tag != null)
                {
                    saveFileDialog.Filter = "All Files (*.*)|*.*";
                    if (tn.Tag is FileSystemFileEntry)
                    {
                        FileSystemFileEntry entry = (FileSystemFileEntry)tn.Tag;
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
                    else if (tn.Tag is CXI)
                    {
                        CXI cxi = (CXI)tn.Tag;
                        RomContext cxt = (RomContext)currentContext;
                        saveFileDialog.FileName = lvFileTree.GetMainText(tn);
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
                                    //string inpath = saveFileDialog.FileName;
                                    FileStream infs = File.OpenRead(filePath);
                                    bool isExeFS = tn.Text.StartsWith("Exe");

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
        }

        private void LoadText(string path)
        {
            Text = "3DS Explorer v." + Application.ProductVersion + " " +  (path != null ? " (" + path.Substring(path.LastIndexOf('\\') + 1) + ")" : "");
        }

        private void lstInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lstInfo.SelectedIndices.Count > 0)
            {
                string toClip = "";
                if (lstInfo.SelectedItems[0].SubItems[3].Text == "")
                    toClip = lstInfo.SelectedItems[0].SubItems[4].Text;
                else
                    toClip = lstInfo.SelectedItems[0].SubItems[3].Text;
                Clipboard.SetText(toClip);
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
            openFileDialog.Filter = "All Supported (3ds,cci,bin,sav,tmd)|*.3ds;*.cci;*.bin;*.sav;*.tmd|3DS Dump Files (*.3ds,*.cci)|*.3ds;*.cci|Save Binary Files (*.bin,*.sav)|*.bin;*.sav|Title Metadata (*.tmd)|*.tmd|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                openFile(openFileDialog.FileName);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Save Sav Files (*.sav)|*.sav;*.bin|All Files|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, SaveTool.createSAV((SFContext)currentContext));
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

        private void openForm<T>() where T : Form, new()
        {
            T form = null;

            foreach (Form f in Application.OpenForms)
                if (f.GetType().IsAssignableFrom(typeof(T)))
                {
                    form = (T)f;
                    break;
                }

            if (form == null)
                form = new T();
            form.Show();
            form.BringToFront();
        }

        private void menuToolsXORTool_Click(object sender, EventArgs e)
        {
            openForm<frmXORTool>();
        }

        private void menuToolsHashTool_Click(object sender, EventArgs e)
        {
            openForm<frmHashTool>();
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

        #region CXTMENU FileContext
        private void cxtFile_MouseEnter(object sender, EventArgs e)
        {
            if (lvFileTree.TreeView.SelectedNode == null)
                cxtFile.Close();
            else
                if (lvFileTree.TreeView.SelectedNode.Tag == null)
                    cxtFile.Close();
        }

        private void cxtFileSaveAs_Click(object sender, EventArgs e)
        {
            lvFileTree_DoubleClick(null, null);
        }

        private void cxtFileReplaceWith_Click(object sender, EventArgs e)
        {
            if (currentContext is SFContext)
            {
                SFContext cxt = (SFContext)currentContext;
                openFileDialog.Filter = "All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileSystemFileEntry originalFile = (FileSystemFileEntry)lvFileTree.TreeView.SelectedNode.Tag;
                    FileStream newFile = File.OpenRead(openFileDialog.FileName);
                    ulong newFileSize = (ulong)newFile.Length;
                    newFile.Close();
                    if (originalFile.FileSize != newFileSize)
                    {
                        MessageBox.Show("File's size doesn't match the target file. \nIt must be the same size as the one to replace.");
                        return;
                    }
                    long offSetInImage = cxt.fileBase + originalFile.BlockOffset * 0x200;
                    Buffer.BlockCopy(File.ReadAllBytes(openFileDialog.FileName), 0, cxt.image, (int)offSetInImage, (int)newFileSize);
                    MessageBox.Show("File replaced.");

                    //TODO: Fix hashes
                }
            }
            else
                MessageBox.Show("This action can't be done!");
        }
        #endregion
    }
}
