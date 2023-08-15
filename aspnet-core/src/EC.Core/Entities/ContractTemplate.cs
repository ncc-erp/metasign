using Abp.Domain.Entities;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractTemplate : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public string HtmlContent { get; set; }
        public ContractTemplateType Type { get; set; }
        public long? UserId { get; set; }
        public bool IsFavorite { get; set; }
    }
}