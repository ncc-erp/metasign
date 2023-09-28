using EC.Manager.ContractTemplateSettings.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class CheckMultiple
    {
        public Guid? MassGuid { get; set; }
        public List<NameEmail> NameEmails { get; set; }
    }

    public class FileBase64Dto
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }

    public class GetGroupMassSignerDto
    {
        public bool CanSignMultiple => this.MassSigners.GroupBy(x => new { x.SignerName, x.SignerEmail }).Count() == 1;
        public long ContractTemplateSignerId { get; set; }
        public List<GetMassSignerDto> MassSigners { get; set; }
        public int? ProcessOrder { get; set; }
        public Guid? SignerMassGuid { get; set; }
    }

    public class GetMassSignerDto
    {
        public string Color { get; set; }
        public ContractRole ContractRole { get; set; }
        public long ContractTemplateSignerId { get; set; }
        public string SignerEmail { get; set; }
        public string SignerName { get; set; }
    }

    public class NameEmail
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class ResponseFailDto
    {
        public string ReasonFail { get; set; }
        public string Address { get; set; }
    }

    public class RowMassTemplateExportDto
    {
        public CreatECDto Contract { get; set; }
        public List<string> ListFieldDto { get; set; }
        public List<SignerMassDto> Signers { get; set; }
    }

    public class SignerMassDto : SignerDto
    {
        public List<GetContractTemplateSettingDto> SignatureSettings { get; set; }
        public Guid? MassGuid { get; set; }
    }

    public class UploadFileDto
    {
        public IFormFile File { get; set; }
    }

    public class UploadMassTemplateFileDto : UploadFileDto
    {
        public long TemplateId { get; set; }
    }

    public class ValidImportMassContractTemplateDto
    {
        public List<ResponseFailDto> FailList { get; set; }
        public List<RowMassTemplateExportDto> SuccessList { get; set; }
    }
}