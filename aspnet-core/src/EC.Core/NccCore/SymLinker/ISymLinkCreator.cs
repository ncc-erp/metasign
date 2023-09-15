
using System.Runtime.InteropServices;

namespace NccCore.SymLinker
{
    public interface ISymLinkCreator
    {
        bool CreateSymLink(string linkPath, string targetPath, bool file);
    }

    public class SymLinkCreator
    {
        private static ISymLinkCreator linker;
        public static ISymLinkCreator GetSymLinkCreator()
        {
            if (linker == null)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    linker = new WindowsSymLinkCreator();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    linker = new LinuxSymLinkCreator();
                }
                else
                {
                    linker = new OSXSymLinkCreator();
                }
            }
            return linker;
        }
    }
}
