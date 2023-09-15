using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; }
    }

    public class UploadMassTemplateFileDto : UploadFileDto
    {
        public long TemplateId { get; set; }
    }

    public class FileBase64Dto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Base64 { get; set; }
    }

    public class ResponseFailDto
    {
        public int Row { get; set; }
        public string ReasonFail { get; set; }
    }

    public class ValidImportMassContractTemplateDto
    {
        public List<RowMassTemplateExportDto> SuccessList { get; set; }
        public List<ResponseFailDto> FailList { get; set; }
    }

    public class MassTemplateRowDataDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class RowMassTemplateExportDto
    {
        public ContractRole ContractRole { get; set; }
        public string Role { get; set; }
        public List<MassTemplateRowDataDto> RowData { get; set; }
    }

    public class CreateMassContractDto
    {
        public long TemplateId { get; set; }
        public List<RowMassTemplateExportDto> SignerList { get; set; }
    }

    public class GetMassSignerDto
    {
        public long ContractTemplateSignerId { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public string Color { get; set; }
    }

    public class GetGroupMassSignerDto
    {
        public long ContractTemplateSignerId { get; set; }
        public List<GetMassSignerDto> MassSigners { get; set; }
        public bool CanSignMultiple => this.MassSigners.DistinctBy(x => x.SignerEmail).DistinctBy(x => x.SignerName).Count() == 1;
        public Guid? SignerMassGuid { get; set; }
        public int? ProcessOrder { get; set; }
    }
}