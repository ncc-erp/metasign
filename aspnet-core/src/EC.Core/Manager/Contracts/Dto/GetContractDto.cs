using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public long UserId { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime? ExpriedTime { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public int NumberOfSetting { get; set; }
        public int CountCompleted { get; set; }
        public bool IsMyContract { get; set; }
        public bool IsAllowSigning { get; set; }
        public string SignUrl { get; set; }
        public string ContractBase64 { get; set; }
        public bool IsHasSigned { get; set; }
        public bool IsExprireSoon { get; set; }
        public string StatusName => Enum.GetName(typeof(ContractStatus), Status);
        public long? ContractTemplateId { get; set; }
        public Guid? ContractGuid { get; set; }
    }
}
