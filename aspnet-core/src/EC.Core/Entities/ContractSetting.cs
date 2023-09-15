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
    public class ContractSetting : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractId { get; set; }
        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Password { get; set; }
        public string Color { get; set; }
        public bool IsComplete { get; set; }
        public bool IsSendMail { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Role { get; set; }
        public ContractSettingStatus Status { get; set; }
        public Guid? SignerMassGuid { get; set; }
    }
}
