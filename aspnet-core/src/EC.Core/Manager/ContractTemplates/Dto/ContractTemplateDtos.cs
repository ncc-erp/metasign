using Abp.AutoMapper;
using EC.Entities;
using EC.Manager.ContractTemplateSettings.Dto;
using EC.Manager.ContractTemplateSigners.Dto;
using NccCore.Anotations;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.ContractTemplates.Dto
{
    public class ContractTemplateDtos
    {
    }

    [AutoMapTo(typeof(ContractTemplate))]
    public class CreateContractTemplateDto
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public string HtmlContent { get; set; }
        public ContractTemplateType Type { get; set; }
        public long? UserId { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class GetContractTemplateDto
    {
        public long Id { get; set; }
        [ApplySearch]
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public string HtmlContent { get; set; }
        public ContractTemplateType Type { get; set; }
        public long? UserId { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreationUserName { get; set; }
        public DateTime? LastModifycationTime { get; set; }
        public string LastModifyCationUserName { get; set; }
    }

    public class GetContractTemplateByFilterDto
    {
        public ContractTemplateFilterType FilterType { get; set; }
        public GridParam GridParam { get; set; }
    }

    public class GetSignatureForContracttemplateDto
    {
        public GetContractTemplateDto ContractTemplate { get; set; }
        public List<GetContractTemplateSettingDto> SignatureSettings { get; set; }
        public List<GetContractTemplateSignerDto> SignerSettings { get; set; }
    }
}