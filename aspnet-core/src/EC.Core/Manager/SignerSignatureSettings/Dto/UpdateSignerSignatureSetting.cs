using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.SignerSignatureSettings.Dto
{
    [AutoMapTo(typeof(SignerSignatureSetting))]
    public class UpdateSignerSignatureSetting : EntityDto<long>
    {
        public long ContractSettingId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public long SignatureTypeId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public int Page { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string ValueInput { get; set; }
    }
}
