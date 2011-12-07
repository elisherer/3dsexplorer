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
        private bool _checkNow;

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
            var tempContext = ModuleHelper.CreateByType(type);
            if (tempContext == null)
            {
                MessageBox.Show(@"This file is unsupported!");
                fs.Close();
                return;
            }
            fs.Seek(0, SeekOrigin.Begin);
            if (!tempContext.Open(fs))
            {
                MessageBox.Show(@"Error: " + tempContext.GetErrorMessage());
                fs.Close();
                return;
            }
            fs.Close();

            //Start the open process
            LoadText(_filePath);
            treeView.Nodes.Clear();
            var nodes = tempContext.GetExplorerTopNode();
            treeView.Nodes.Add(nodes);
            treeView.ExpandAll();
            lvFileTree.Nodes.Clear();
            nodes = tempContext.GetFileSystemTopNode();
            if (nodes != null)
                lvFileTree.Nodes.Add(nodes);
            lvFileTree.ExpandAll();

            _currentContext = tempContext;
            treeView.SelectedNode = treeView.Nodes[0];

            menuFileSave.Enabled = _currentContext.CanCreate();
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
                    else if (tn.Tag is CWAVContext)
                    {
                        CWAVContext.Play((CWAVContext)tn.Tag);
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
            openFileDialog.Filter = @"All Supported (3ds,cci,bin,sav,tmd,cia,bnr,bcwav,cgfx)|*.3ds;*.cci;*.bin;*.sav;*.tmd;*.cia;*.bnr;*.bcwav;*.cwav;*.cgfx|3DS Rom Files (*.3ds,*.cci)|*.3ds;*.cci|Save Binary Files (*.bin,*.sav)|*.bin;*.sav|Title Metadata (*.tmd)|*.tmd|CTR Importable Archives (*.cia)|*.cia|CTR Banners (*.bnr)|*.bnr|CTR Waves (*.b/cwav)|*.bcwav;*.cwav|CTR Graphics (*.cgfx)|*.cgfx|All Files|*.*";
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
            (new frmAbout()).ShowDialog();
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
