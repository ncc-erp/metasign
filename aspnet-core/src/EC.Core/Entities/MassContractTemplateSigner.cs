using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace EC.Entities
{
    public class MassContractTemplateSigner : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractTemplateSignerId { get; set; }
        [ForeignKey(nameof(ContractTemplateSignerId))]
        public virtual ContractTemplateSigner ContractTemplateSigner { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
    }
}