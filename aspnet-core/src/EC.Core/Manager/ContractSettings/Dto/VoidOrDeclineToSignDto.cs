using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSettings.Dto
{
    public class VoidOrDeclineToSignDto
    {
        public long ContractSettingId { get; set; }
        public string Reason { get; set; }
    }
}
