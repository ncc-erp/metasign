using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Abp.Net.Mail.EmailSettingNames;

namespace EC.WebService.SignServer.Dto
{
    public class SignProcessDto
    {
        public string WorkerName { get; set; }
        public string Data { get; set; }
        public string Encoding { get; set; }
        public string ProcessType { get; set; }
        public Dictionary<string, string> RequestMetadata { get; set; }
        public string GetRequestMetadata()
        {
            string metadata = "";
            foreach (KeyValuePair<string, string> entry in  RequestMetadata)
                metadata += entry.Key + "=" + entry.Value + "\n";
            return metadata;
        }
    }
}
