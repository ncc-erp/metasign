using EC.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.Contracts.Dto
{
    public class SendMailDto
    {
        public MailPreviewInfoDto MailContent { get; set; }

        public long ContractId { get; set; }
    }
}
