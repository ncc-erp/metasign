using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.Helper
{
    public class FileUploadInfo
    {
        public string FileName { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public string MineType { get; set; }
        public long FileSize { get; set; }
        public string ServerPath { get; set; }
    }

    public enum ResourceType : byte
    {
        Video = 0,
        Document = 1,
        Image = 2
    }
}
