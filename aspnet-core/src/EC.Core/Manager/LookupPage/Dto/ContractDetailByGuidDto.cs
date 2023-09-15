using EC.Manager.Contracts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.LookupPage.Dto
{
    public class ContractDetailByGuidDto
    {
        public string ContractName { get; set; }
        public long ContractId { get; set; }

        public ContractStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? ExpriedTime { get; set; }
        public string CreatedUser { get; set; }
        public string CreatedEmail { get; set; }
        public List<RecipientGetByGuidDto> Recipients { get; set; }
        public Guid? ContractGuid { get; set; }
    }

    public class RecipientGetByGuidDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ContractRole Role { get; set; }
        public bool IsComplete { get; set; }
        public int? ProcessOrder { get; set; }
        public DateTime? CancelTime { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime? SendingTime { get; set; }
        public DateTime? SigningTime { get; set; }
    }
}
