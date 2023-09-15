using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Configuration.Dto
{
    public class EmailConfigDto
    {
        public string DisplayName { get; set; }
        public string DefaultAddress { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EnableSsl { get; set; }
        public string UseDefaultCredentials { get; set; }
    }
}
