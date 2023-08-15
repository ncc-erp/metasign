using Abp;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email.Dto
{
    public class ContractMailTemplateDto
    {
        public string Message { get; set; }
        public string ContractCode { get; set; }
        public string ContractName { get; set; }
        public string SignUrl { get; set; }
        public string LookupUrl { get; set; }
        public string Subject { get; set; }
        public string SendToEmail { get; set; }
        public string SendToName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorName { get; set; }
        public long? ContractSettingId { get; set; }
        public ContractRole ContractRole { get; set; }
        public Guid? ContractGuid{ get; set;}
        public DateTime? ExpireTime { get; set; }
    }
}
