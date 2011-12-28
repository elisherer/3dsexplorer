using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Net;
using _3DSExplorer.Modules;

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
            Text = string.Format("{0} v.{1}",Application.ProductName,Application.ProductVersion);
            menuHelpCheckUpdates.Checked = Properties.Settings.Default.CheckForUpdatesOnStartup;
        }

        #region Info ListView Functions

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
                
        private void lstInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lstInfo.SelectedIndices.Count <= 0) return;
            var toClip = lstInfo.SelectedItems[0].SubItems[3].Text == "" ? lstInfo.SelectedItems[0].SubItems[4].Text : lstInfo.SelectedItems[0].SubItems[3].Text;
            Clipboard.SetText(toClip);
            MessageBox.Show("Value copied to clipboard!");
        }
        
        #endregion

        private void OpenFile(string path)
        {
            menuToolsQuickCRC.Enabled = false;
            _filePath = path;
            var fs = File.OpenRead(_filePath);
            var type = ModuleHelper.GetModuleType(_filePath, fs);
            var tempContext = ModuleHelper.CreateByType(type);
            if (tempContext == null)
            {
                MessageBox.Show("This file is unsupported!");
                fs.Close();
                return;
            }
            fs.Seek(0, SeekOrigin.Begin);
            if (!tempContext.Open(fs))
            {
                MessageBox.Show(string.Format("Error: {0}",tempContext.GetErrorMessage()));
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
            menuToolsQuickCRC.Enabled = true;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = (TreeViewContextTag) e.Node.Tag;
            tag.Context.View(this, tag.Type, tag.Values);
        }

        private void LoadText(string path)
        {
            lblTreeViewTitle.Text = Path.GetFileName(path);
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
            openFileDialog.Filter = ModuleHelper.OpenString;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                OpenFile(openFileDialog.FileName);
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = _currentContext.GetFileFilter() + "|All Files|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            var outStream = File.OpenWrite(saveFileDialog.FileName);
            var inStream = File.OpenRead(_filePath);
            _currentContext.Create(outStream, inStream);
            inStream.Close();
            outStream.Close();
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

        private void menuTools3DVideo_Click(object sender, EventArgs e)
        {
            openForm<frm3DVideo>();
        }

        private void menuToolsQuickCRC_Click(object sender, EventArgs e)
        {
            frmCheckSum.ShowDialog(_filePath);
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

        private void GoToUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("This system doesn't support clicking a link...\n\n{0}",ex.Message));
            }
        }

        private void menuHelpVisitGoogleCode_Click(object sender, EventArgs e)
        {
            GoToUrl("http://3dsexplorer.googlecode.com/");
        }

        private void menuHelpVisit3DBrew_Click(object sender, EventArgs e)
        {
            GoToUrl("http://www.3dbrew.org/");
        }

        private void menuHelpVisitNDev_Click(object sender, EventArgs e)
        {
            GoToUrl("http://www.n-dev.net");
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            (new frmAbout()).ShowDialog();
        }

        #endregion

        #region CXTMENU FileContext

        private void cxtFileItemClick(object sender, EventArgs e)
        {
            var contextTag = (TreeViewContextTag)((ToolStripItem)sender).Tag;
            contextTag.Context.Activate(_filePath, contextTag.Type, contextTag.Values);
        }

        private void lvFileTree_TreeDoubleClicked(object sender, MouseEventArgs e)
        {
            var node = lvFileTree.NodeAt(e.Location);
            if (node == null) return;
            lvFileTree.TreeView.SelectedNode = node;
            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (node.Tag != null)
                    {
                        var contextTag = ((TreeViewContextTag[])node.Tag)[0];
                        contextTag.Context.Activate(_filePath, contextTag.Type, contextTag.Values);
                    }
                    break;
            }
        }

        private void lvFileTree_TreeMouseClicked(object sender, MouseEventArgs e)
        {
            var node = lvFileTree.NodeAt(e.Location);
            if (node == null) return;
            lvFileTree.TreeView.SelectedNode = node;
            switch (e.Button)
            {
                case MouseButtons.Right:
                        
                    if (node.Tag != null)
                    {
                        var tags = (TreeViewContextTag[])node.Tag;
                        cxtFile.Items.Clear();
                        for (var i = 0; i < tags.Length; i++)
                        {
                            var toolItem = cxtFile.Items.Add(tags[i].ActivationString, null, cxtFileItemClick);
                            toolItem.Tag = tags[i];
                        }
                        cxtFile.Items[0].Font = new Font(cxtFile.Items[0].Font, FontStyle.Bold);
                        cxtFile.Show(lvFileTree.TreeView, e.Location);
                    }
                    break;
            }
        }
        #endregion

        #region Check for updates
        private void bwCheckForUpdates_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _remoteVer = "<Error: Couldn't parse the version number>";
                var request = (HttpWebRequest) WebRequest.Create("http://3dsexplorer.googlecode.com/svn/trunk/3DSExplorer/Properties/AssemblyInfo.cs");
                var responseStream = request.GetResponse().GetResponseStream();
                if (responseStream == null) return;
                var reader = new StreamReader(responseStream);
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

        private bool IsNewerAvailable(string newerVersion)
        {
            var thisVersion = Version.Parse(Application.ProductVersion);
            var remoteVersion = Version.Parse(newerVersion);
            return remoteVersion.CompareTo(thisVersion) > 0;
        }

        private void bwCheckForUpdates_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsNewerAvailable(_remoteVer))
                MessageBox.Show(string.Format("This version is v{0}\nThe version on the server is v{1}\nYou might want to download a newer version.",Application.ProductVersion,_remoteVer));
            else if (_checkNow)
                MessageBox.Show(string.Format("v{0} is the latest version.", Application.ProductVersion));
        }
        #endregion
    }

}
