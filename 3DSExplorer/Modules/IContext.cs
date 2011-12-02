using System.IO;

namespace _3DSExplorer
{
    public interface IContext
    {
        bool Open(FileStream fs);
        void Create(FileStream fs, FileStream src);
        void View(frmExplorer f, int view, int[] values);
    }
}
