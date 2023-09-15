using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.LookupPage.Dto
{
    public class GetLookupContractDto
    {
        public string Email { get; set; }
        public GridParam GridParam { get; set; }
    }
}
