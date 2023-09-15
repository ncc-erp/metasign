using Abp.Domain.Entities;
using EC.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class Contract : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public string Name { get; set; }
        public string EmailContent { get; set; }
        public string Code { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime? ExpriredTime { get; set; }
        public long? ContractTemplateId { get; set; }
        public Guid? ContractGuid { get; set; }
        public Guid? MassGuid { get; set; }
    }
}
