using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.SymLinker
{
    public class LinuxSymLinkCreator : ISymLinkCreator
    {
        public bool CreateSymLink(string linkPath, string targetPath, bool file)
        {
            throw new NotImplementedException();
        }
    }
}
