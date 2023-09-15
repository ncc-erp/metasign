using EC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email.Dto
{
    public class GetMailPreviewInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MailFuncEnum Type { get; set; }
        public string Subject { get; set; }
        public string BodyMessage { get; set; }
        public string[] PropertiesSupport { get; set; }
        public List<string> CCs { get; set; } = new List<string>();
        public string SendToEmail { get; set; }
        public MailTemplateType TemplateType => CommonUtils.GetTemplateType(Type);
    }
}
