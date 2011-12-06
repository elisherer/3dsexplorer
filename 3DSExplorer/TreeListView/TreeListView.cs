using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Design;

namespace TreeListView
{
    public partial class TreeListViewControl : UserControl, IColumnsProvider
    {
        public TreeListViewControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            treeView.ColumnProvider = this;
        }

        public const char ColumnSeperator = '©';

        [MergableProperty(false)]
        [Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        public ListView.ColumnHeaderCollection Columns
        {
            get
            {
                return listView.Columns;
            }
        }
        public TreeView TreeView { get { return treeView; } }
        [Category("Behavior")]
        public ImageList ImageList
        {
            set { treeView.ImageList = value; }
            get { return treeView.ImageList; }
        }

        public int TotalColWidth
        {
            get
            {
                var width = 0;
                foreach (var col in Columns.Cast<ColumnHeader>()) { width += col.Width; }
                return width;
            }
        }

        private bool _mScrollbarIsHidden;
        private Panel _mScrollbarBlanket;
        private Panel ScrollbarBlanket
        {
            get
            {
                if (_mScrollbarBlanket == null)
                {                    
                    _mScrollbarBlanket = new Panel();
                    _mScrollbarBlanket.BackColor = SystemColors.ControlLight;
                    const int scrollbarHeight = 20;
                    _mScrollbarBlanket.Parent = this;
                    _mScrollbarBlanket.Location = new Point(2, Height - scrollbarHeight);
                    _mScrollbarBlanket.Size = new Size(Width-4, scrollbarHeight);
                    _mScrollbarBlanket.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                }
                return _mScrollbarBlanket;
            }
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                return treeView.Nodes;
            }
        }

        public static string CreateMultiColumnNodeText(params string[] texts)
        {
            return string.Concat(texts.Select(t => t.Replace(ColumnSeperator.ToString(), "") + ColumnSeperator.ToString()).ToArray());//.PadRight(400);
        }

        public void AddSubItem(TreeNode tn, string text)
        {
            tn.Text += TreeListViewControl.ColumnSeperator + text;
        }

        public void ExpandAll()
        {
            treeView.ExpandAll();
        }

        public string GetMainText(TreeNode tn)
        {
            if (!tn.Text.Contains(ColumnSeperator))
                return tn.Text;
            else
                return tn.Text.Substring(0,tn.Text.IndexOf(ColumnSeperator));
        }

        private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            treeView.Refresh();
        }

        private void listView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (TotalColWidth > ClientSize.Width)
            {
                e.Cancel = true;
                e.NewWidth = ClientSize.Width - (TotalColWidth - Columns[e.ColumnIndex].Width);
            }
            treeView.Refresh();            
        }

        [Category("Action")]
        [Description("Fires when the a node in the treeview being double clicked.")]
        public event EventHandler TreeDoubleClicked;


        public List<TreeNode> TreeNodeListRecursive
        {
            get { return treeView.TreeNodeListRecursive; }
        }

        public List<TreeNode> ExposedTreeNodes
        {
            get { return treeView.ExposedNodes; }
        }

        private void treeView_HideOrShowScrollBar(object sender, MultiColumnTreeView.HideScrollBarEventArgs e)
        {
            if (e.Hide && !_mScrollbarIsHidden)
            {
                ScrollbarBlanket.BringToFront();
                ScrollbarBlanket.Visible = true;
                ScrollbarBlanket.Show();
                _mScrollbarIsHidden = true;
            }
            else if (!e.Hide && _mScrollbarIsHidden)
            {
                ScrollbarBlanket.Visible = false;
                ScrollbarBlanket.Hide();
                _mScrollbarIsHidden = false;
            }

        }

        private void TreeListViewControl_EnabledChanged(object sender, EventArgs e)
        {
            treeView.Enabled = Enabled;
            listView.Enabled = Enabled;
            listView.HeaderStyle = Enabled ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.Nonclickable;
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            TreeDoubleClicked(sender,e);
        }

    }
}
