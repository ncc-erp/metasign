using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email.Dto
{
    public class EmailTemplateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        public MailFuncEnum Type { get; set; }
        public string CCs { get; set; }
        public string SendToEmail { get; set; }
    }
}
