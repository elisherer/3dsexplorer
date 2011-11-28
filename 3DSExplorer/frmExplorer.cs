using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public partial class frmExplorer : Form
    {
        IContext _currentContext;
        string _filePath;

        public frmExplorer()
        {
            InitializeComponent();
            InitializeForm();
        }

        public frmExplorer(string path)
        {
            InitializeComponent();
            InitializeForm();
            OpenFile(path);
        }

        private void InitializeForm()
        {
            Text = @"3DS Explorer v." + Application.ProductVersion;
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
            lvi.SubItems.Add(Util.ToHexString(size * 2,value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        public void AddListItem(int offset, int size, string description, byte[] value, int group)
        {
            var lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add("");
            lvi.SubItems.Add(Util.ByteArrayToString(value));
            lvi.Group = lstInfo.Groups[group];
            lstInfo.Items.Add(lvi);
        }
        public void AddListItem(int offset, int size, string description, char[] value, int group)
        {
            var lvi = new ListViewItem("0x" + offset.ToString("X3"));
            lvi.SubItems.Add(size.ToString());
            lvi.SubItems.Add(description);
            lvi.SubItems.Add("");
            lvi.SubItems.Add(Util.CharArrayToString(value));
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
            var magic = new byte[4];
            var encrypted = false;

            //Determin what kind of file it is
            var type = -1;

            var extension = Path.GetExtension(_filePath);
            if (extension != null) 
                extension = extension.ToLower();

            switch (extension)
            {
                case "cci":
                case "3ds":
                    type = 0;
                    break;
                case "bin":
                case "sav":
                    type = 1;
                    break;
                case "tmd":
                    type = 2;
                    break;
                case "cia":
                    type = 3;
                    break;
                default:
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(magic, 0, 4);
                    if (magic[0] < 5 && magic[1] == 0 && magic[2] == 1 && magic[3] == 0)
                        type = 1;
                    else if (magic[0] == 0x20 && magic[1] == 0x20 && magic[2] == 0 && magic[3] == 0)
                        type = 3;
                    else if (fs.Length >= 0x104) // > 256+4
                    {
                        //CCI CHECK
                        fs.Seek(0x100, SeekOrigin.Current);
                        fs.Read(magic, 0, 4);
                        if (magic[0] == 'N' && magic[1] == 'C' && magic[2] == 'S' && magic[3] == 'D')
                            type = 0;
                        else if (fs.Length >= 0x10000) // > 64kb
                        {
                            //SAVE Check
                            fs.Seek(0, SeekOrigin.Begin);
                            var crcCheck = new byte[8 + 10 * (fs.Length / 0x1000 - 1)];
                            fs.Read(crcCheck, 0, crcCheck.Length);
                            fs.Read(magic, 0, 2);
                            var calcCheck = CRC16.GetCRC(crcCheck);
                            if (magic[0] == calcCheck[0] && magic[1] == calcCheck[1]) //crc is ok then save
                                type = 1; //SAVE
                        }
                    }
                    break;
            }
            if (type == 1)
            {
                //check if encrypted
                fs.Seek(0x1000, SeekOrigin.Begin); //Start of information
                while ((fs.Length - fs.Position > 0x200) & !SRAMTool.IsSaveMagic(magic))
                {
                    fs.Read(magic, 0, 4);
                    fs.Seek(0x200 - 4, SeekOrigin.Current);
                }
                encrypted = (fs.Length - fs.Position <= 0x200);

            }

            fs.Close();
            treeView.Nodes.Clear();
            lvFileTree.Nodes.Clear();
            switch (type)
            {
                case 0: //Rom
                    RomContext rcxt = RomTool.Open(_filePath);
                    if (rcxt != null)
                    {
                        LoadText(_filePath);
                        //Build Tree
                        var topNode = treeView.Nodes.Add("Rom");
                        for (int i = 0; i < rcxt.cxis.Length; i++) //ADD CXIs
                            if (rcxt.cxis[i].CXISize > 0)
                            {
                                topNode.Nodes.Add("NCCH " + i + " (" + (new String(rcxt.cxis[i].ProductCode)).Substring(0, 10) + ")");
                                if (rcxt.cxis[i].PlainRegionSize > 0) //Add PlainRegions
                                    topNode.Nodes[topNode.Nodes.Count - 1].Nodes.Add("PlainRegion");
                            }
                        for (var i = 0; i < 2; i++)
                        {
                            TreeNode lvItem;
                            if (rcxt.cxis[i].ExeFSSize > 0)
                            {
                                lvItem = lvFileTree.Nodes.Add("ExeFS" + i + ".bin");
                                lvFileTree.AddSubItem(lvItem, (rcxt.cxis[i].ExeFSSize * 0x200).ToString());
                                lvFileTree.AddSubItem(lvItem, Util.ToHexString(6, rcxt.cxis[i].ExeFSOffset * 0x200));
                                lvItem.ImageIndex = 0;
                                lvItem.Tag = rcxt.cxis[i];
                            }
                            if (rcxt.cxis[i].RomFSSize <= 0) continue;
                            lvItem = lvFileTree.Nodes.Add("RomFS" + i + ".bin");
                            lvFileTree.AddSubItem(lvItem, (rcxt.cxis[i].RomFSSize * 0x200).ToString());
                            lvFileTree.AddSubItem(lvItem, Util.ToHexString(6, rcxt.cxis[i].RomFSOffset * 0x200));
                            lvItem.ImageIndex = 0;
                            lvItem.Tag = rcxt.cxis[i];
                        }
                        treeView.ExpandAll();
                        _currentContext = rcxt;
                        treeView.SelectedNode = treeView.TopNode;
                    }
                    break;
                case 1: //Save
                    string errMsg = null;
                    var scxt = SRAMTool.Open(_filePath, ref errMsg);
                    if (scxt == null)
                        MessageBox.Show(@"Error: " + errMsg);
                    else
                    {
                        LoadText(_filePath);
                        //Build Tree
                        var topNode = treeView.Nodes.Add("Save Flash " + (scxt.Encrypted ? "(Encrypted)" : ""));
                        topNode.Nodes.Add("SAVE Partition").Nodes.Add("Maps");
                        if (scxt.IsData)
                            topNode.Nodes.Add("DATA Partition");
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
                                folders[i] = folders[scxt.Folders[i].ParentFolderIndex - 1].Nodes.Add(Util.CharArrayToString(scxt.Folders[i].FolderName));
                                folders[i].ImageIndex = 1;
                                folders[i].SelectedImageIndex = 1;
                            }
                        //add files
                        if (scxt.Files.Length > 0)
                        {
                            for (var i = 0; i < scxt.Files.Length; i++)
                            {
                                var lvItem = folders[scxt.Files[i].ParentFolderIndex - 1].Nodes.Add(Util.CharArrayToString(scxt.Files[i].Filename));
                                lvFileTree.AddSubItem(lvItem, scxt.Files[i].FileSize.ToString());
                                lvFileTree.AddSubItem(lvItem, Util.ToHexString(6, (ulong)(scxt.FileBase + 0x200 * scxt.Files[i].BlockOffset)));
                                lvItem.ImageIndex = 0;
                                lvItem.Tag = scxt.Files[i];
                            }
                        }
                        folders[0].ExpandAll();
                        treeView.ExpandAll();
                        _currentContext = scxt;
                        treeView.SelectedNode = treeView.TopNode;
                    }
                    break;
                case 2: //TMD
                    var tcxt = TMDTool.Open(_filePath);
                    if (tcxt == null)
                        MessageBox.Show(@"This kind of TMD is unsupported.");
                    else
                    {
                        LoadText(_filePath);
                        //Build Tree
                        var topNode = treeView.Nodes.Add("TMD");
                        topNode.Tag = TMDTool.TMDView.TMD;
                        topNode.Nodes.Add("Content Info Records").Tag = TMDTool.TMDView.ContentInfoRecord;
                        topNode.Nodes.Add("Content Chunk Records").Tag = TMDTool.TMDView.ContentChunkRecord;
                        if (tcxt.Certificates.Count > 0)
                        {
                            topNode = treeView.TopNode.Nodes.Add("Certificates");
                            for (var i = 0; i < tcxt.Certificates.Count; i++)
                                topNode.Nodes.Add("Certificate " + i);
                        }
                        treeView.ExpandAll();
                        _currentContext = tcxt;
                        treeView.SelectedNode = treeView.TopNode;
                    }
                    break;
                case 3: //CIA
                    var ccxt = CIATool.Open(_filePath);
                    LoadText(_filePath);
                    //Build Tree
                    treeView.Nodes.Add("CIA");
                    if (ccxt.Certificates.Count > 0)
                    {
                        var topNode = treeView.TopNode.Nodes.Add("Certificates");
                        for (var i = 0; i < ccxt.Certificates.Count; i++)
                            topNode.Nodes.Add("Certificate " + i);
                    }
                    if ((uint)ccxt.Ticket.SignatureType != 0)
                        treeView.TopNode.Nodes.Add("Ticket");
                    if (ccxt.TMD != null)
                    {
                        var topNode = treeView.TopNode.Nodes.Add("TMD");
                        topNode.Tag = TMDTool.TMDView.TMD;
                        topNode.Nodes.Add("Content Info Records").Tag = TMDTool.TMDView.ContentInfoRecord;
                        topNode.Nodes.Add("Content Chunk Records").Tag = TMDTool.TMDView.ContentChunkRecord;
                    }
                    if (ccxt.header.BannerLength > 0)
                    {
                        var topNode = treeView.TopNode.Nodes.Add("Banner");
                        topNode.Nodes.Add("Meta-Data");
                        if (imlFS.Images.ContainsKey("small"))
                            imlFS.Images.RemoveByKey("small");
                        if (imlFS.Images.ContainsKey("large"))
                            imlFS.Images.RemoveByKey("large");
                        imlFS.Images.Add("small", ccxt.SmallIcon);
                        imlFS.Images.Add("large", ccxt.LargeIcon);
                        lvFileTree.Nodes.Add("small","Small (24x24)","small").SelectedImageKey = @"small";
                        lvFileTree.Nodes.Add("large","Large (48x48)","large").SelectedImageKey = @"large";
                    }
                    treeView.ExpandAll();
                    _currentContext = ccxt;
                    treeView.SelectedNode = treeView.TopNode;
                    break;
                default: MessageBox.Show(@"This file is unsupported!"); break;
            }
            if (_currentContext != null)
            {
                menuFileSave.Enabled = (type == 1);
                menuFileSaveImageFile.Enabled = (type == 1);
                menuFileSaveKeyFile.Enabled = (type == 1) && encrypted;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_currentContext is RomContext)
            {
                var cxt = (RomContext)_currentContext;
                if (e.Node.Text.StartsWith("Rom"))
                    RomTool.View(this, cxt, RomTool.RomView.NCSD);
                else if (e.Node.Text.StartsWith("NCCH"))
                {
                    cxt.currentNcch = e.Node.Text[5] - '0';
                    RomTool.View(this, cxt, RomTool.RomView.NCCH);
                }
                else if (e.Node.Text.StartsWith("Pla"))
                {
                    cxt.currentNcch = e.Node.Parent.Text[5] - '0';
                    RomTool.View(this, cxt, RomTool.RomView.NCCHPlainRegion);
                }
            }
            else if (_currentContext is SRAMContext)
            {
                var cxt = (SRAMContext)_currentContext;
                switch (e.Node.Text[2])
                {
                    case 'v': //Save
                        SRAMTool.View(this, cxt, SRAMTool.SRAMView.Image);
                        break;
                    case 'V': //SAVE/DATA Partition
                        cxt.CurrentPartition = e.Node.Text[2]=='V' ? 0 : 1;
                        SRAMTool.View(this, cxt, SRAMTool.SRAMView.Partition);
                        break;
                    case 'p': //Tables (maps)
                        SRAMTool.View(this, cxt, SRAMTool.SRAMView.Tables);
                        break;
                }
            }
            else if (_currentContext is TMDContext)
            {
                var cxt = (TMDContext)_currentContext;
                if (e.Node.Text.StartsWith("Certificate "))
                {
                    int i = e.Node.Text[12] - '0';
                    CertTool.View(this, cxt.Certificates, i);
                }
                else if (e.Node.Text.StartsWith("Certificates"))
                {
                    CertTool.View(this, cxt.Certificates, -1);
                }
                else if (e.Node.Text.StartsWith("TMD"))
                {
                    TMDTool.View(this, cxt, TMDTool.TMDView.TMD);
                }
                else if (e.Node.Text.StartsWith("Content I"))
                {
                    TMDTool.View(this, cxt, TMDTool.TMDView.ContentInfoRecord);
                }
                else if (e.Node.Text.StartsWith("Content C"))
                {
                    TMDTool.View(this, cxt, TMDTool.TMDView.ContentChunkRecord);
                }
            }
            else if (_currentContext is CIAContext)
            {
                var cxt = (CIAContext)_currentContext;
                if (e.Node.Text.StartsWith("CIA"))
                {
                    CIATool.View(this, cxt, CIATool.CIAView.CIA);
                }
                else if (e.Node.Text.StartsWith("Certificate "))
                {
                    var i = e.Node.Text[12] - '0';
                    CertTool.View(this, cxt.Certificates, i);
                }
                else if (e.Node.Text.StartsWith("Certificates"))
                {
                    CertTool.View(this, cxt.Certificates, -1);
                }
                else if (e.Node.Text.StartsWith("TMD"))
                {
                    TMDTool.View(this, cxt.TMD, TMDTool.TMDView.TMD);
                }
                else if (e.Node.Text.StartsWith("Content I"))
                {
                    TMDTool.View(this, cxt.TMD, TMDTool.TMDView.ContentInfoRecord);
                }
                else if (e.Node.Text.StartsWith("Content C"))
                {
                    TMDTool.View(this, cxt.TMD, TMDTool.TMDView.ContentChunkRecord);
                }
                else if (e.Node.Text.StartsWith("Ticket"))
                {
                    TicketTool.View(this,cxt.Ticket);
                }
                else if (e.Node.Text.StartsWith("Banner"))
                {
                    CIATool.View(this, cxt, CIATool.CIAView.Banner);
                }
                else if (e.Node.Text.StartsWith("Meta-Data"))
                {
                    CIATool.View(this, cxt, CIATool.CIAView.BannerMetaData);
                }
            }
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
                        saveFileDialog.FileName = Util.CharArrayToString(entry.Filename);
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
                                byte[] key = Util.ParseKeyStringToByteArray(strKey);

                                if (key == null)
                                    MessageBox.Show(@"Error parsing key string, (must be a multiple of 2 and made of hex letters.");
                                else
                                {
                                    //string inpath = saveFileDialog.FileName;
                                    var infs = File.OpenRead(_filePath);
                                    var isExeFS = tn.Text.StartsWith("Exe");

                                    long offset = isExeFS ? cxi.ExeFSOffset : cxi.RomFSOffset;
                                    switch (cxt.currentNcch)
                                    {
                                        case 0:
                                            offset += cxt.cci.FirstNCCHOffset;
                                            break;
                                        case 1:
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
                                        for (int i = 0; i < 8; i++)
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
                }
                else if (tn.ImageKey.Equals("small"))
                {
                    ImageBox.ShowDialog(((CIAContext)_currentContext).SmallIcon);
                }
                else if (tn.ImageKey.Equals("large"))
                {
                    ImageBox.ShowDialog(((CIAContext)_currentContext).LargeIcon);
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
            openFileDialog.Filter = @"All Supported (3ds,cci,bin,sav,tmd,cia)|*.3ds;*.cci;*.bin;*.sav;*.tmd;*.cia|3DS Dump Files (*.3ds,*.cci)|*.3ds;*.cci|Save Binary Files (*.bin,*.sav)|*.bin;*.sav|Title Metadata (*.tmd)|*.tmd|CTR Importable Archives (*.cia)|*.cia|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                OpenFile(openFileDialog.FileName);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = @"Save Sav Files (*.sav)|*.sav;*.bin|All Files|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, SRAMTool.CreateSAV((SRAMContext)_currentContext));
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

        private void cxtFileSaveAs_Click(object sender, EventArgs e)
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

                    //TODO: Fix hashes
                }
            }
            else
                MessageBox.Show(@"This action can't be done!");
        }
        #endregion
    }

    interface IContext
    {
    }

}
