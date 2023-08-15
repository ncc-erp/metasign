using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService.Goggle.Dto
{
    public class GoogleCapchaVerifyDto
    {
        public string secret { get; set; }
        public string response { get; set; }
    }
}
