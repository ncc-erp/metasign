using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractHistories.Dto
{
    [AutoMapTo(typeof(ContractHistory))]
    public class CreaContractHistoryDto
    {
        public ContractStatus ContractStatus { get; set; }
        public string AuthorEmail { get; set; }
        public HistoryAction Action { get; set; }
        public DateTime TimeAt { get; set; }
        public string Note { get; set; }
        public long ContractId { get; set; }
        public string  MailContent { get; set; }
    }
}
