using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSignings.Dto
{
    public class SignInputDto
    {
        public long ContractSettingId { get; set; }
        public string ContractBase64 { get; set; }
    }
}
