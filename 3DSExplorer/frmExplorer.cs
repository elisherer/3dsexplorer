using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Net;

namespace _3DSExplorer
{
    public partial class frmExplorer : Form
    {
        private IContext _currentContext;
        private string _filePath, _remoteVer;
        private bool _checkNow = false;

        private class TreeViewContextTag
        {
            public IContext Context;
            public int View;
            public int[] Values;
        }

        public frmExplorer()
        {
            InitializeComponent();
            InitializeForm();
            if (Properties.Settings.Default.CheckForUpdatesOnStartup)
                bwCheckForUpdates.RunWorkerAsync();
        }

        public frmExplorer(string path) : this()
        {
            OpenFile(path);
        }

        private void InitializeForm()
        {
            Text = @"3DS Explorer v." + Application.ProductVersion;
            menuHelpCheckUpdates.Checked = Properties.Settings.Default.CheckForUpdatesOnStartup;
        }

        #region ListView Functions

        public void SetGroupHeaders(params string[] groupHeader)
        {
            for (var i = 0; i < groupHeader.Length && i < lstInfo.Groups.Count; i++)
            {
                lstInfo.Groups[i].Header = groupHeader[i];
            }
        }

        public void ClearInformation()
        {
            lstInfo.Items.Clear();
        }

        public void AutoAlignColumns()
        {
            lstInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        
        public void AddListItem(int offset, int size, string description, ulong value, int group)
        {
            var lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add(value.ToString());
            lvi.SubItems.Add(StringUtil.ToHexString(size * 2,value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        public void AddListItem(int offset, int size, string description, byte[] value, int group)
        {
            var lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add("");
            lvi.SubItems.Add(StringUtil.ByteArrayToString(value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        public void AddListItem(int offset, int size, string description, char[] value, int group)
        {
            var lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add("");
            lvi.SubItems.Add(StringUtil.CharArrayToString(value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }

        public void AddListItem(string offset, string size, string description, string value, string hexvalue, int group)
        {
            var lvi = new ListViewItem(offset);
            lvi.SubItems.Add(size);
            lvi.SubItems.Add(description);
            lvi.SubItems.Add(value);
            lvi.SubItems.Add(hexvalue);
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        #endregion

        private void OpenFile(string path)
        {
            _filePath = path;
            var fs = File.OpenRead(_filePath);

            var type = ModuleHelper.GetModuleType(_filePath, fs);

            treeView.Nodes.Clear();
            lvFileTree.Nodes.Clear();
            fs.Seek(0, SeekOrigin.Begin);
            TreeNode tNode;
            switch (type)
            {
                case ModuleType.Rom:
                    var rcxt = new RomContext();
                    if (!rcxt.Open(fs))
                    {
                        MessageBox.Show(@"Error reading file.");
                        break;
                    }
                    LoadText(_filePath);
                    //Build Tree
                    tNode = treeView.Nodes.Add("Rom");
                    tNode.Tag = new TreeViewContextTag { Context = rcxt, View = (int)RomContext.RomView.NCSD};
                    for (int i = 0; i < rcxt.cxis.Length; i++) //ADD CXIs
                        if (rcxt.cxis[i].CXISize > 0)
                        {
                            tNode.Nodes.Add("NCCH " + i + " (" +
                                                (new String(rcxt.cxis[i].ProductCode)).Substring(0, 10) + ")").Tag = new TreeViewContextTag { Context = rcxt, View = (int)RomContext.RomView.NCCH , Values = new[] {i}};
                            if (rcxt.cxis[i].PlainRegionSize > 0) //Add PlainRegions
                                tNode.Nodes[tNode.Nodes.Count - 1].Nodes.Add("PlainRegion").Tag = new TreeViewContextTag { Context = rcxt, View = (int)RomContext.RomView.NCCHPlainRegion, Values = new[]{i}};
                        }
                    for (var i = 0; i < 2; i++)
                    {
                        if (rcxt.cxis[i].ExeFSSize > 0)
                        {
                            tNode = lvFileTree.Nodes.Add("ExeFS" + i + ".bin");
                            lvFileTree.AddSubItem(tNode, (rcxt.cxis[i].ExeFSSize * 0x200).ToString());
                            lvFileTree.AddSubItem(tNode, StringUtil.ToHexString(6, rcxt.cxis[i].ExeFSOffset * 0x200));
                            tNode.ImageIndex = 0;
                            tNode.Tag = rcxt.cxis[i];
                        }
                        if (rcxt.cxis[i].RomFSSize <= 0) continue;
                        tNode = lvFileTree.Nodes.Add("RomFS" + i + ".bin");
                        lvFileTree.AddSubItem(tNode, (rcxt.cxis[i].RomFSSize * 0x200).ToString());
                        lvFileTree.AddSubItem(tNode, StringUtil.ToHexString(6, rcxt.cxis[i].RomFSOffset * 0x200));
                        tNode.ImageIndex = 0;
                        tNode.Tag = rcxt.cxis[i];
                    }
                    treeView.ExpandAll();
                    _currentContext = rcxt;
                    treeView.SelectedNode = treeView.Nodes[0];
                    break;
                case ModuleType.SRAM_Decrypted: //Save
                case ModuleType.SRAM: //Save
                    var scxt = new SRAMContext();
                    if (!scxt.Open(fs))
                    {
                        MessageBox.Show(@"Error: " + scxt.errorMessage);
                        break;
                    }    
                    LoadText(_filePath);
                    //Build Tree
                    tNode = treeView.Nodes.Add("Save Flash " + (scxt.Encrypted ? "(Encrypted)" : ""));
                    tNode.Tag = new TreeViewContextTag { Context = scxt, View = (int)SRAMContext.SRAMView.Image };
                    var sNode = tNode.Nodes.Add("SAVE Partition");
                    sNode.Tag = new TreeViewContextTag { Context = scxt, View = (int)SRAMContext.SRAMView.Partition, Values = new[] {0}};
                    sNode.Nodes.Add("Maps").Tag = new TreeViewContextTag { Context = scxt, View = (int)SRAMContext.SRAMView.Tables};
                    if (scxt.IsData)
                        tNode.Nodes.Add("DATA Partition").Tag = new TreeViewContextTag { Context = scxt, View = (int)SRAMContext.SRAMView.Partition, Values = new[] { 1 } };
                    lvFileTree.Nodes.Clear();
                    var folders = new TreeNode[scxt.Folders.Length];
                    //add root folder
                    folders[0] = lvFileTree.Nodes.Add("ROOT");
                    folders[0].ImageIndex = 1;
                    folders[0].SelectedImageIndex = 1;
                    //add folders
                    if (scxt.Folders.Length > 1)
                        for (int i = 1; i < scxt.Folders.Length; i++)
                        {
                            folders[i] =
                                folders[scxt.Folders[i].ParentFolderIndex - 1].Nodes.Add(
                                    StringUtil.CharArrayToString(scxt.Folders[i].FolderName));
                            folders[i].ImageIndex = 1;
                            folders[i].SelectedImageIndex = 1;
                        }
                    //add files
                    if (scxt.Files.Length > 0)
                    {
                        for (var i = 0; i < scxt.Files.Length; i++)
                        {
                            var lvItem =
                                folders[scxt.Files[i].ParentFolderIndex - 1].Nodes.Add(
                                    StringUtil.CharArrayToString(scxt.Files[i].Filename));
                            lvFileTree.AddSubItem(lvItem, scxt.Files[i].FileSize.ToString());
                            lvFileTree.AddSubItem(lvItem,
                                                    StringUtil.ToHexString(6,
                                                                            (ulong)
                                                                            (scxt.FileBase +
                                                                            0x200*scxt.Files[i].BlockOffset)));
                            lvItem.ImageIndex = 0;
                            lvItem.Tag = scxt.Files[i];
                        }
                    }
                    folders[0].ExpandAll();
                    treeView.ExpandAll();
                    _currentContext = scxt;
                    treeView.SelectedNode = treeView.Nodes[0];
                    break;
                case ModuleType.TMD:
                    var tcxt = new TMDContext();
                    if (!tcxt.Open(fs))
                    {
                        MessageBox.Show(@"This kind of TMD is unsupported.");
                        break;
                    }                    
                    LoadText(_filePath);
                    //Build Tree
                    tNode = treeView.Nodes.Add("TMD");
                    tNode.Tag = new TreeViewContextTag { Context = tcxt, View = (int)TMDContext.TMDView.TMD };
                    tNode.Nodes.Add("Content Info Records").Tag = new TreeViewContextTag { Context = tcxt, View = (int)TMDContext.TMDView.ContentInfoRecord };
                    tNode.Nodes.Add("Content Chunk Records").Tag = new TreeViewContextTag { Context = tcxt, View = (int)TMDContext.TMDView.ContentChunkRecord };
                    if (tcxt.CertificatesContext != null && tcxt.CertificatesContext.List.Count > 0)
                    {
                        tNode = treeView.TopNode.Nodes.Add("Certificates");
                        tNode.Tag = new TreeViewContextTag { Context = tcxt.CertificatesContext, Values = new[] { -1 } };
                        for (var i = 0; i < tcxt.CertificatesContext.List.Count; i++)
                            tNode.Nodes.Add("Certificate " + i).Tag = new TreeViewContextTag { Context = tcxt.CertificatesContext, Values = new[] { i } };
                    }
                    treeView.ExpandAll();
                    _currentContext = tcxt;
                    treeView.SelectedNode = treeView.Nodes[0];
                    break;
                case ModuleType.CIA:
                    var ccxt = new CIAContext();
                    if (!ccxt.Open(fs))
                    {
                        MessageBox.Show(@"Error reading file.");
                        break;
                    }
                    LoadText(_filePath);
                    //Build Tree
                    treeView.Nodes.Add("CIA").Tag = new TreeViewContextTag {Context = ccxt, View = (int)CIAContext.CIAView.CIA};
                    if (ccxt.CertificatesContext.List.Count > 0)
                    {
                        tNode = treeView.TopNode.Nodes.Add("Certificates");
                        tNode.Tag = new TreeViewContextTag { Context = ccxt.CertificatesContext, Values = new[] { -1 } };
                        for (var i = 0; i < ccxt.CertificatesContext.List.Count; i++)
                            tNode.Nodes.Add("Certificate " + i).Tag = new TreeViewContextTag { Context = ccxt.CertificatesContext, Values = new[] {i}};
                    }
                    if ((uint) ccxt.TicketContext.Ticket.SignatureType != 0)
                        treeView.TopNode.Nodes.Add("Ticket").Tag = new TreeViewContextTag { Context = ccxt.TicketContext };;
                    if (ccxt.TMDContext != null)
                    {
                        tNode = treeView.TopNode.Nodes.Add("TMD");
                        tNode.Tag = new TreeViewContextTag { Context = ccxt.TMDContext, View = (int)TMDContext.TMDView.TMD };
                        tNode.Nodes.Add("Content Info Records").Tag = new TreeViewContextTag { Context = ccxt.TMDContext, View = (int)TMDContext.TMDView.ContentInfoRecord };
                        tNode.Nodes.Add("Content Chunk Records").Tag = new TreeViewContextTag { Context = ccxt.TMDContext, View = (int)TMDContext.TMDView.ContentChunkRecord };
                    }
                    if (ccxt.Header.BannerLength > 0)
                    {
                        tNode = treeView.TopNode.Nodes.Add("Banner");
                        tNode.Tag = new TreeViewContextTag { Context = ccxt, View = (int)CIAContext.CIAView.Banner };
                        tNode.Nodes.Add("Meta-Data").Tag = new TreeViewContextTag { Context = ccxt, View = (int)CIAContext.CIAView.BannerMetaData }; ;
                        if (imlFS.Images.ContainsKey("small"))
                            imlFS.Images.RemoveByKey("small");
                        if (imlFS.Images.ContainsKey("large"))
                            imlFS.Images.RemoveByKey("large");
                        imlFS.Images.Add("small", ccxt.SmallIcon);
                        imlFS.Images.Add("large", ccxt.LargeIcon);
                        tNode = lvFileTree.Nodes.Add("small", "Small (24x24)", "small");
                        tNode.SelectedImageKey = @"small";
                        tNode.Tag = ccxt.SmallIcon;
                        tNode = lvFileTree.Nodes.Add("large", "Large (48x48)", "large");
                        tNode.SelectedImageKey = @"large";
                        tNode.Tag = ccxt.LargeIcon;
                    }
                    treeView.ExpandAll();
                    _currentContext = ccxt;
                    treeView.SelectedNode = treeView.Nodes[0];
                    break;
                case ModuleType.Banner:
                    var bcxt = new BannerContext();
                    if (!bcxt.Open(fs))
                    {
                        MessageBox.Show(@"Error reading file.");
                        break;
                    }
                    LoadText(_filePath);
                    //Build Tree
                    tNode = treeView.Nodes.Add("CBMD");
                    tNode.Tag = new TreeViewContextTag { Context = bcxt, View = (int)BannerContext.BannerView.Banner };
                    tNode.Nodes.Add("CGFX").Tag = new TreeViewContextTag { Context = bcxt, View = (int)BannerContext.BannerView.CGFX };
                    tNode.Nodes.Add("CWAV").Tag = new TreeViewContextTag { Context = bcxt, View = (int)BannerContext.BannerView.CWAV };

                    if (imlFS.Images.ContainsKey("banner"))
                        imlFS.Images.RemoveByKey("banner");
                    imlFS.Images.Add("banner", bcxt.BannerImage);
                    tNode = lvFileTree.Nodes.Add("banner", "Banner (256x128)", "banner");
                    tNode.SelectedImageKey = @"banner";
                    tNode.Tag = bcxt.BannerImage;
                        
                    treeView.ExpandAll();
                    _currentContext = bcxt;
                    treeView.SelectedNode = treeView.Nodes[0];
                    
                    break;
                default: MessageBox.Show(@"This file is unsupported!"); break;
            }
            fs.Close();
            if (_currentContext == null) return;
            menuFileSave.Enabled = (type == ModuleType.SRAM) || (type == ModuleType.SRAM_Decrypted) || ((type == ModuleType.CIA) && ((CIAContext)_currentContext).Header.BannerLength > 0);
            menuFileSaveImageFile.Enabled = (type == ModuleType.SRAM) || (type == ModuleType.SRAM_Decrypted);
            menuFileSaveKeyFile.Enabled = type == ModuleType.SRAM;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = (TreeViewContextTag) e.Node.Tag;
            tag.Context.View(this, tag.View, tag.Values);
        }

        private void lvFileTree_DoubleClick(object sender, EventArgs e)
        {
            var tn = lvFileTree.TreeView.SelectedNode;
            if (tn != null)
            {
                if (tn.Tag != null)
                {
                    saveFileDialog.Filter = @"All Files (*.*)|*.*";
                    if (tn.Tag is FileSystemFileEntry)
                    {
                        var entry = (FileSystemFileEntry)tn.Tag;
                        var cxt = (SRAMContext)_currentContext;
                        saveFileDialog.FileName = StringUtil.CharArrayToString(entry.Filename);
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var fs = new MemoryStream(cxt.Image);
                            fs.Seek(cxt.FileBase + entry.BlockOffset * 0x200, SeekOrigin.Begin);
                            //read entry.filesize
                            var fileBuffer = new byte[entry.FileSize];
                            fs.Read(fileBuffer, 0, fileBuffer.Length);
                            File.WriteAllBytes(saveFileDialog.FileName, fileBuffer);
                            fs.Close();
                        }
                    }
                    else if (tn.Tag is CXI)
                    {
                        var cxi = (CXI)tn.Tag;
                        var cxt = (RomContext)_currentContext;
                        saveFileDialog.FileName = lvFileTree.GetMainText(tn);
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string strKey = InputBox.ShowDialog("Please Enter Key:\nPress OK with empty key to save encrypted");
                            if (strKey != null)
                            {
                                byte[] key = StringUtil.ParseKeyStringToByteArray(strKey);

                                if (key == null)
                                    MessageBox.Show(@"Error parsing key string, (must be a multiple of 2 and made of hex letters.");
                                else
                                {
                                    //string inpath = saveFileDialog.FileName;
                                    var infs = File.OpenRead(_filePath);
                                    var isExeFS = tn.Text.StartsWith("Exe");

                                    long offset = isExeFS ? cxi.ExeFSOffset : cxi.RomFSOffset;
                                    switch (tn.Text[5])
                                    {
                                        case '0':
                                            offset += cxt.cci.FirstNCCHOffset;
                                            break;
                                        case '1':
                                            offset += cxt.cci.SecondNCCHOffset;
                                            break;
                                        default:
                                            offset += cxt.cci.ThirdNCCHOffset;
                                            break;
                                    }
                                    offset *= 0x200; //media units

                                    infs.Seek(offset, SeekOrigin.Begin);
                                    long bufferSize = isExeFS ? cxi.ExeFSSize * 0x200 : cxi.RomFSSize * 0x200;
                                    var buffer = new byte[bufferSize];
                                    infs.Read(buffer, 0, buffer.Length);
                                    infs.Close();
                                    if (key.Length > 0)
                                    {
                                        var iv = new byte[0x10];
                                        for (var i = 0; i < 8; i++)
                                            iv[i] = 0;
                                        Buffer.BlockCopy(cxt.cxis[0].ProgramID, 0, iv, 8, 8);

                                        var aes = new Aes128Ctr(key,iv);
                                        aes.TransformBlock(buffer);
                                    }
                                    var outpath = saveFileDialog.FileName;
                                    var outfs = File.OpenWrite(outpath);
                                    outfs.Write(buffer, 0, buffer.Length);
                                    outfs.Close();
                                }
                            }
                        }
                    }
                    else if (tn.Tag is System.Drawing.Image)
                    {
                        ImageBox.ShowDialog((System.Drawing.Image)tn.Tag);
                    }
                }
            }
        }

        private void LoadText(string path)
        {
            lblCaptionTree.Text = path.Substring(path.LastIndexOf('\\') + 1);
        }

        private void lstInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lstInfo.SelectedIndices.Count <= 0) return;
            var toClip = lstInfo.SelectedItems[0].SubItems[3].Text == "" ? lstInfo.SelectedItems[0].SubItems[4].Text : lstInfo.SelectedItems[0].SubItems[3].Text;
            Clipboard.SetText(toClip);
            MessageBox.Show(@"Value copied to clipboard!");
        }

        #region Drag & Drop

        private void frmExplorer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        private void frmExplorer_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenFile(files[0]);
        }

        #endregion

        #region MENU File

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            //Todo: get strings from the modules
            openFileDialog.Filter = @"All Supported (3ds,cci,bin,sav,tmd,cia,bnr)|*.3ds;*.cci;*.bin;*.sav;*.tmd;*.cia;*.bnr|3DS Rom Files (*.3ds,*.cci)|*.3ds;*.cci|Save Binary Files (*.bin,*.sav)|*.bin;*.sav|Title Metadata (*.tmd)|*.tmd|CTR Importable Archives (*.cia)|*.cia|CTR Banners (*.bnr)|*.bnr|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                OpenFile(openFileDialog.FileName);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            
            //TODO: add these strings to the modules
            if (_currentContext is SRAMContext)
                saveFileDialog.Filter = @"SRAM Files (*.sav)|*.sav;*.bin|All Files|*.*";
            else if (_currentContext is CIAContext)
                saveFileDialog.Filter = @"CTR Importable Archives (*.cia)|*.cia|All Files|*.*";

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            var outStream = File.OpenWrite(saveFileDialog.FileName);
            var inStream = File.OpenRead(_filePath);
            _currentContext.Create(outStream, inStream);
            inStream.Close();
            outStream.Close();
        }

        private void menuFileSaveImageFile_Click(object sender, EventArgs e)
        {
            var cxt = (SRAMContext)_currentContext;
            saveFileDialog.Filter = @"Image Files (*.bin)|*.bin";
            saveFileDialog.FileName = _filePath.Substring(_filePath.LastIndexOf('\\') + 1).Replace('.', '_') + ".bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, cxt.Image);
        }

        private void menuFileSaveKeyFile_Click(object sender, EventArgs e)
        {
            var cxt = (SRAMContext)_currentContext;
            saveFileDialog.Filter = @"Key file (*.key)|*.key|All Files|*.*";
            saveFileDialog.FileName = _filePath.Substring(_filePath.LastIndexOf('\\') + 1).Replace('.', '_') + ".key";
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
            var form = (from Form f in Application.OpenForms where f.GetType().IsAssignableFrom(typeof (T)) select (T) f).FirstOrDefault() ??
                     new T();
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

        private void menuHelpCheckNow_Click(object sender, EventArgs e)
        {
            _checkNow = true;
            bwCheckForUpdates.RunWorkerAsync();
        }

        private void menuHelpCheckUpdates_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CheckForUpdatesOnStartup = menuHelpCheckUpdates.Checked;
            Properties.Settings.Default.Save();
        }

        private void menuHelpVisitGoogleCode_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://3dsexplorer.googlecode.com/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"This system doesn't support clicking a link...\n\n" + ex.Message);
            }
        }

        private void menuHelpVisit3DBrew_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.3dbrew.org/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"This system doesn't support clicking a link...\n\n" + ex.Message);
            }
        }

        private void menuHelpVisitNDev_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.n-dev.net");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"This system doesn't support clicking a link...\n\n" + ex.Message);
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

        private void cxtFileOpen_Click(object sender, EventArgs e)
        {
            lvFileTree_DoubleClick(null, null);
        }

        private void cxtFileReplaceWith_Click(object sender, EventArgs e)
        {
            if (_currentContext is SRAMContext)
            {
                var cxt = (SRAMContext)_currentContext;
                openFileDialog.Filter = @"All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var originalFile = (FileSystemFileEntry)lvFileTree.TreeView.SelectedNode.Tag;
                    var newFile = File.OpenRead(openFileDialog.FileName);
                    var newFileSize = (ulong)newFile.Length;
                    newFile.Close();
                    if (originalFile.FileSize != newFileSize)
                    {
                        MessageBox.Show(@"File's size doesn't match the target file. \nIt must be the same size as the one to replace.");
                        return;
                    }
                    long offSetInImage = cxt.FileBase + originalFile.BlockOffset * 0x200;
                    Buffer.BlockCopy(File.ReadAllBytes(openFileDialog.FileName), 0, cxt.Image, (int)offSetInImage, (int)newFileSize);
                    MessageBox.Show(@"File replaced.");
                }
            }
            else if (_currentContext is CIAContext)
            {
                var cxt = (CIAContext)_currentContext;
                openFileDialog.Filter = @"All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var iconImage = (System.Drawing.Image)lvFileTree.TreeView.SelectedNode.Tag;
                    var graphics = System.Drawing.Graphics.FromImage(iconImage);
                    var newImage = System.Drawing.Image.FromFile(openFileDialog.FileName);
                    if (newImage == null)
                    {
                        MessageBox.Show(@"The file selected is not a valid image!");
                        return;
                    }
                    graphics.DrawImage(newImage, 0, 0, iconImage.Width, iconImage.Height);

                    if (imlFS.Images.ContainsKey("small"))
                        imlFS.Images.RemoveByKey("small");
                    if (imlFS.Images.ContainsKey("large"))
                        imlFS.Images.RemoveByKey("large");
                    imlFS.Images.Add("small", cxt.SmallIcon);
                    imlFS.Images.Add("large", cxt.LargeIcon);

                    MessageBox.Show(@"File replaced.");
                    newImage.Dispose();
                }
            }
            else
                MessageBox.Show(@"This action can't be done!");
        }
        #endregion

        #region Check for updates
        private void bwCheckForUpdates_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _remoteVer = @"<Error: Couldn't parse the version number>";
                var checkUrl = @"http://3dsexplorer.googlecode.com/svn/trunk/3DSExplorer/Properties/AssemblyInfo.cs";
                var request = (HttpWebRequest) WebRequest.Create(checkUrl);
                if (request.GetResponse().GetResponseStream() == null) return;
                var reader = new StreamReader(request.GetResponse().GetResponseStream());
                string line;
                while ((line = reader.ReadLine()) != null)
                    if (line.Contains("AssemblyFileVersion")) //Get the version between the quotation marks
                    {
                        var start = line.IndexOf('"') + 1;
                        var len = line.LastIndexOf('"') - start;
                        _remoteVer = line.Substring(start, len);
                        break;
                    }
            }
            catch
            {
                //No harm done...possibly no internet connection
            }
        }

        private void bwCheckForUpdates_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Application.ProductVersion.Equals(_remoteVer))
                MessageBox.Show("This version is v" + Application.ProductVersion + Environment.NewLine +
                                "The version on the server is v" + _remoteVer + Environment.NewLine +
                                "You might want to download a newer version.");
            else if (_checkNow)
                MessageBox.Show("v" + Application.ProductVersion + " is the latest version.");
        }
        #endregion
    }

}
