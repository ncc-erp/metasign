using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractTemplateSetting : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractTemplateSignerId { get; set; }
        [ForeignKey(nameof(ContractTemplateSignerId))]
        public virtual ContractTemplateSigner ContractTemplateSigner { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public int Page { get; set; }
        public bool IsSigned { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string ValueInput { get; set; }
    }
}