using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Controllers.Dto
{
    public class TokenDto
    {
        public string googleToken { get; set; }
        public string secretCode { get; set; }
        public LoginType type { get; set; }
    }
}
