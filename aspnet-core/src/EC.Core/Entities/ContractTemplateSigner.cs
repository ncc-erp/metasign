using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractTemplateSigner : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Role { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
        public long ContractTemplateId { get; set; }
        [ForeignKey(nameof(ContractTemplateId))]
        public virtual ContractTemplate ContractTemplate { get; set; }
    }
}