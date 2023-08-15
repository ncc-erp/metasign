using Abp.AutoMapper;
using EC.Entities;
using EC.Manager.ContractTemplateSigners.Dto;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.ContractTemplateSettings.Dto
{
    public class ContractTemplateSettingDto
    {
    }

    [AutoMapTo(typeof(ContractTemplateSetting))]
    public class CreateContractTemplateSettingDto
    {
        public long ContractTemplateSignerId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public bool IsSigned { get; set; }
        public int Page { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string ValueInput { get; set; }
    }

    public class UpdateContractTemplateSettingDto
    {
        public long Id { get; set; }
        public long ContractTemplateSignerId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public bool IsSigned { get; set; }
        public int Page { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string ValueInput { get; set; }

    }

    public class GetContractTemplateSettingDto
    {
        public long Id { get; set; }
        public long ContractTemplateSignerId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public bool IsSigned { get; set; }
        public int Page { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public string Color { get; set; }
        public string ValueInput { get; set; }

    }

    public class GetAllSignerLocationDto
    {
        public GetContractTemplateSignerDto Signer { get; set; }
        public List<GetContractTemplateSettingDto> Settings { get; set; }
    }
}