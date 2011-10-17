using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace TreeListView
{
    public partial class TreeListViewControl : UserControl, TreeListView.IColumnsProvider
    {
        public TreeListViewControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            treeView.ColumnProvider = this;
        }

        public const char ColumnSeperator = '^';

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
                int width = 0;
                foreach (var col in Columns.Cast<ColumnHeader>()) { width += col.Width; }
                return width;
            }
        }

        private bool m_scrollbarIsHidden;
        private Panel m_scrollbarBlanket;
        private Panel ScrollbarBlanket
        {
            get
            {
                if (m_scrollbarBlanket == null)
                {                    
                    m_scrollbarBlanket = new Panel();
                    m_scrollbarBlanket.BackColor = SystemColors.ControlLight;
                    int scrollbarHeight = 20;
                    m_scrollbarBlanket.Parent = this;
                    m_scrollbarBlanket.Location = new Point(2, this.Height - scrollbarHeight);
                    m_scrollbarBlanket.Size = new Size(Width-4, scrollbarHeight);
                    m_scrollbarBlanket.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                }
                return m_scrollbarBlanket;
            }
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                return treeView.Nodes;
            }
        }

        public string CreateMultiColumnNodeText(params string[] texts)
        {
            return string.Concat(texts.Select(t => t.Replace(ColumnSeperator.ToString(), "") + ColumnSeperator.ToString()).ToArray());//.PadRight(400);
        }

        public void AddSubItem(TreeNode tn, string text)
        {
            tn.Text += TreeListViewControl.ColumnSeperator + text;
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
            if (e.Hide && !m_scrollbarIsHidden)
            {
                ScrollbarBlanket.BringToFront();
                ScrollbarBlanket.Visible = true;
                ScrollbarBlanket.Show();
                m_scrollbarIsHidden = true;
            }
            else if (!e.Hide && m_scrollbarIsHidden)
            {
                ScrollbarBlanket.Visible = false;
                ScrollbarBlanket.Hide();
                m_scrollbarIsHidden = false;
            }

        }

        private void TreeListViewControl_EnabledChanged(object sender, EventArgs e)
        {
            treeView.Enabled = this.Enabled;
            listView.Enabled = this.Enabled;
            if (Enabled)
                listView.HeaderStyle = ColumnHeaderStyle.Clickable;
            else
                listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            TreeDoubleClicked(sender,e);
        }

    }
}
