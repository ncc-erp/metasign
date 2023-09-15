using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSignings.Dto
{
    public class InputSigningResultDto
    {
        public long ContractSettingId { get; set; }
        public string SignResult { get; set; }
        public bool? HasDigital { get; set; }
    }
}
