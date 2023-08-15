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
    public class SignerSignatureSetting : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractSettingId { get; set; }
        [ForeignKey(nameof(ContractSettingId))]
        public virtual ContractSetting ContractSetting { get; set; }
        public long SignatureTypeId { get; set; }
        [ForeignKey(nameof(SignatureTypeId))]
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
