using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractHistory : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string AuthorEmail { get; set; }
        public HistoryAction Action { get; set; }
        public DateTime TimeAt { get; set; }
        public string Note { get; set; }
        public long ContractId { get; set; }
        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }
        public string MailContent { get; set; }
    }
}
