using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NccCore.SymLinker
{
    public class WindowsSymLinkCreator : ISymLinkCreator
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode)]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymLinkFlag dwFlags);
        [HandleProcessCorruptedStateExceptions]
        public bool CreateSymLink(string linkPath, string targetPath, bool file)
        {
            //var linkName = Path.GetFileName(targetPath);
            //linkPath = Path.Combine(linkPath, linkName);
            bool success = false;
            try
            {
                var symbolicLinkType = file ? SymLinkFlag.File : SymLinkFlag.Directory;
                // allow unprivileged creation symbolic
                //symbolicLinkType = symbolicLinkType | SymLinkFlag.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE;
                success = CreateSymbolicLink(linkPath, targetPath, symbolicLinkType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public enum SymLinkFlag
        {
            File = 0x0,
            Directory = 0x1,
            SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE = 0x2
        }
    }
}
