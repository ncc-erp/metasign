using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSignings.Dto
{
    public class ValidEmailDto
    {
        public string Email { get; set; }
        public long ContractSettingId { get; set; }
    }
}
