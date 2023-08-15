using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractByFilterDto
    {
        public string SignerEmail { get; set; }
        public ContractFilterType FilterType { get; set; }
        public string Search { get; set; }
        public GridParam GridParam { get; set; }
    }
}
