using System.IO;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public class TreeViewContextTag
    {
        public IContext Context;
        public int View;
        public int[] Values;

        public static TreeViewContextTag Create(IContext context)
        {
            return new TreeViewContextTag { Context = context};
        }

        public static TreeViewContextTag Create(IContext context, int view)
        {
            return new TreeViewContextTag { Context = context, View = view};
        }

        public static TreeViewContextTag Create(IContext context, int view, int[] values)
        {
            return new TreeViewContextTag {Context = context, View = view, Values= values};
        }
    }

    public interface IContext
    {
        bool Open(Stream fs);
        string GetErrorMessage();
        void Create(FileStream fs, FileStream src);
        void View(frmExplorer f, int view, int[] values);
        bool CanCreate();

        TreeNode GetExplorerTopNode();
        TreeNode GetFileSystemTopNode();
    }
}
