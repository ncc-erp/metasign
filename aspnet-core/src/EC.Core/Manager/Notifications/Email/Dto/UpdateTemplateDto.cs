using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email.Dto
{
    [AutoMapTo(typeof(EmailTemplate))]
    public class UpdateTemplateDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public MailFuncEnum Type { get; set; }
        public List<string> ListCC { get; set; }
        public string SendToEmail { get; set; }
    }
}
