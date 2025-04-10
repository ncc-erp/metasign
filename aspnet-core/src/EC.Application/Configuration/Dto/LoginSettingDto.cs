using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Configuration.Dto
{
    public class LoginSettingDto
    {
        public bool EnableNormalLogin { get; set; }
        public string MezonClientId { get; set; }
        public bool EnableLoginMezon { get; set; }
        public bool EnableLoginGoogle { get; set; }
        public bool EnableLoginMicrosoft { get; set; }
    }

}
