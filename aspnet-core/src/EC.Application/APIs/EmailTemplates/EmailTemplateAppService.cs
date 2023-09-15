using Abp.Authorization;
using EC.Authorization;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Email;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.EmailTemplates
{
    public class EmailTemplateAppService : ECAppServiceBase
    {
        private readonly EmailManager _emailManager;

        public EmailTemplateAppService(EmailManager emailManager)
        {
            _emailManager = emailManager;
        }

        [HttpGet]
        public async Task<List<EmailDto>> GetAll()
        {
            return await _emailManager.GetAllMailTemplate();
        }

        [HttpGet]
        public GetMailPreviewInfoDto PreviewTemplate(long id)
        {
            return _emailManager.PreviewTemplate(id);
        }

        [HttpGet]
        public GetMailPreviewInfoDto GetTemplateById(long id)
        {
            return _emailManager.GetTemplateById(id);
        }

        [HttpPut]
        public async Task<UpdateTemplateDto> UpdateTemplate(UpdateTemplateDto input)
        {
            return await _emailManager.UpdateTemplate(input);
        }

        [HttpPost]
        public void SendMail(MailPreviewInfoDto input)
        {
            _emailManager.SendMail(input);
        }
    }
}
