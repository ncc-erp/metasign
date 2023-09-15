using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.SignerSignatureSettings.Dto
{
    public class GetSignerSignatureSettingDto : EntityDto<long>
    {
        public long ContractSettingId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public long SignatureTypeId { get; set; }
        public bool IsSigned { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int Page { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public string Color { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public bool IsAllowSigning { get; set; }
        public float FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontColor { get; set; }
        public string ValueInput { get; set; }
    }

    public class GetContractSignerSignatureSettingDto
    {
        public long ContractId { get; set; }
        public string ContractBase64 { get; set; }
        public string ContractName { get; set; }
        public ContractRole Role { get; set; }
        public ContractStatus Status { get; set; }
        public GetContractSignatureDefaultDto SignatureDefault { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsComplete { get; set; }
        public List<GetSignerSignatureSettingDto> SignatureSettings { get; set; }
        public bool IsCreator { get; set; }
        public Guid? MassGuid{ get; set; }
    }

    public class GetSigningSignatureSettingDto : GetSignerSignatureSettingDto
    {
        public bool IsAllowSigning { get; set; }
    }

    public class GetContractSignatureDefaultDto
    {
        public SignatureTypeSetting SignatureType { get; set; }
        public string ContractBase64 { get; set; }
    }
    public class GetMassContractNotSignDto
    {
        public long ContractSettingId { get; set; }
        public long ContractId { get; set; }
        public string Base64Pdf { get; set; }
        public List<GetMassContractNotSignSignatureDto> Signature { get; set; }
        public bool IsSigned { get; set; }
    }

    public class GetMassContractNotSignSignatureDto
    {
        public long SignerSignatureSettingid { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
    }
}