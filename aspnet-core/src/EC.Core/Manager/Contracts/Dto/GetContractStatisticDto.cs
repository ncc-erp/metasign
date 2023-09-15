using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractStatisticDto
    {
        public int WaitForMe { get; set; }
        public int WaitForOther { get; set; }
        public int ExprireSoon { get; set; }
        public int Complete { get; set; }
    }
}
