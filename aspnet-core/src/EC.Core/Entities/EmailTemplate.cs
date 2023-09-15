using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class EmailTemplate : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public MailFuncEnum Type { get; set; }
        [MaxLength(200)]
        public string CCs { get; set; }
        public string SendToEmail { get; set; }
    }
}
