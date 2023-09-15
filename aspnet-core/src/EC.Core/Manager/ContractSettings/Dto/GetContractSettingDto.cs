using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSettings.Dto
{
    public class GetContractSettingDto : EntityDto<long>
    {
        public long ContractId { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Password { get; set; }
        public string Color { get; set; }
        public string ContractFileName { get; set; }
        public bool IsComplete { get; set; }
        public string Role { get; set; }
    }

    public class GetAllContractSettingDto
    {
        public bool IsOrder { get; set; }
        public List<GetContractSettingDto> Signers { get; set; }
    }

    public class CreateContractSettingFromTemplateDto
    {
        public long ContractId { get; set; }
        public long ContractSignerId { get; set; }
        public long ContractTemplateId { get; set; }
    }
}