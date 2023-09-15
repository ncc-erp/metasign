using System;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractDetailDto
    {
        public string ContractName { get; set; }
        public string ContractCode { get; set; }
        public string ContractFile { get; set; }
        public string ContractBase64 { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? ExpriedTime { get; set; }
        public string CreatedUser { get; set; }
        public bool IsAssigned { get; set; }
        public string SignUrl { get; set; }
        public bool IsMyContract { get; set; }
        public long SettingId { get; set; }
        public long ContractId { get; set; }
        public bool IsHasSigned { get; set; }
        public List<RecipientDto> Recipients { get; set; }
        public string Note { get; set; }
        public Guid? ContractGuid { get; set; }
    }

    public class RecipientDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ContractRole Role { get; set; }
        public bool IsComplete { get; set; }
        public int? ProcessOrder { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsSendMail { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime CancelTime { get; set; }
    }

    public class CancelContractDto
    {
        public long ContractId { get; set; }
        public string Reason { get; set; }
    }
}