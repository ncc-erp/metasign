using Abp.AutoMapper;
using EC.Entities;
using EC.Manager.ContractTemplateSettings.Dto;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.ContractTemplateSigners.Dto
{
    public class ContractTemplateSignerDto
    {
        public string Role { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
    }

    [AutoMapTo(typeof(ContractTemplateSigner))]
    public class CreateContractTemplateSignerDto
    {
        public string Role { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
        public long ContractTemplateId { get; set; }
    }

    public class CreateListContractTemplateSignerDto
    {
        public long ContractTemplateId { get; set; }
        public List<ContractTemplateSignerDto> ContractTemplateSigners { get; set; }
    }

    public class UpdateListContractTemplateSignerDto
    {
        public long ContractTemplateId { get; set; }
        public List<UpdateContractTemplateSignerDto> ContractTemplateSigners { get; set; }
    }

    [AutoMapTo(typeof(ContractTemplateSigner))]
    public class UpdateContractTemplateSignerDto
    {
        public long? Id { get; set; }
        public string Role { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
    }

    public class GetContractTemplateSignerDto
    {
        public long Id { get; set; }
        public string Role { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
        public long ContractTemplateId { get; set; }
        public List<string> MassField { get; internal set; }
    }

    public class GetMassContractTemplateSignerDto
    {
        public long ContractTemplateSignerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ContractRole ContractRole { get; set; }
    }

    public class GetAllContractTemplateSignerDto
    {
        public bool IsOrder { get; set; }
        public List<GetContractTemplateSignerDto> Signers { get; set; }
    }

    public class GetContractTemplateSignerSettingsDto
    {
        public GetContractTemplateSignerDto Singer { get; set; }
        public List<GetAllSignerLocationDto> Settings { get; set; }
    }
}