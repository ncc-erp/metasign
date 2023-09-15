using Abp.UI;
using AngleSharp.Text;
using EC.Constants.FileStoring;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace EC.Utils
{
    public class FileUtil
    {
        public static void CheckFileFormat(IFormFile file, string[] allowFileTypes)
        {
            if (file == null)
                return;
            var fileExt = Path.GetExtension(file.FileName).Substring(1).ToLower();
            if (!allowFileTypes.Contains(fileExt))
            {
                throw new UserFriendlyException($"Wrong format! Allow file type: {allowFileTypes}");
            }
        }
        public static string GetFileExtension(IFormFile file)
        {
            if (file == default || string.IsNullOrEmpty(file.FileName))
            {
                return "";
            }
            return Path.GetExtension(file.FileName).Substring(1).ToLower();
        }
        public static string GetFileName(string filePath)
        {
            if (filePath == null)
            {
                return "";
            }
            if (filePath.Contains("/"))
            {
                return filePath.Substring(filePath.LastIndexOf("/") + 1);   
            }
            return filePath;
        }
    }
}
