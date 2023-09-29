using Abp.Domain.Entities;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractTemplate : AuditEntity, IMayHaveTenant
    {
        public string Content { get; set; }
        public string FileName { get; set; }
        public string HtmlContent { get; set; }
        public bool IsFavorite { get; set; }
        public string MassField { get; set; }
        public MassType MassType { get; set; }
        public string MassWordContent { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public ContractTemplateType Type { get; set; }
        public long? UserId { get; set; }
    }
}