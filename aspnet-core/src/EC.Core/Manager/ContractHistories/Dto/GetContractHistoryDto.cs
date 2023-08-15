using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractHistories.Dto
{
    public class GetContractHistoryDto:EntityDto<long>
    {
        public ContractStatus ContractStatus { get; set; }
        public string AuthorEmail { get; set; }
        public HistoryAction Action { get; set; }
        public DateTime TimeAt { get; set; }
        public string Note { get; set; }
        public long ContractId { get; set; }
    }
}
