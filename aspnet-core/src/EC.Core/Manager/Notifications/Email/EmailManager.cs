using Abp.Net.Mail;
using Abp.Timing;
using Abp.UI;
using EC.Constants.Dictionary;
using EC.Entities;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Templates;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email
{
    public class EmailManager : BaseManager
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _appConfiguration;

        public EmailManager(IWorkScope workScope,
            IConfiguration configuration,
            IEmailSender emailSender) : base(workScope)
        {
            _emailSender = emailSender;
            _appConfiguration = configuration;
        }

        public IQueryable<EmailDto> IQGetEmailTemplate()
        {
            return WorkScope.GetAll<EmailTemplate>()
                    .Select(s => new EmailDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        BodyMessage = s.BodyMessage.Replace("\"", "'"),
                        Type = s.Type,
                        CCs = s.CCs,
                        SendToEmail = s.SendToEmail
                    });
        }

        public void SendMail(MailPreviewInfoDto input)
        {
            Send(input);
        }

        public void Send(MailPreviewInfoDto message)
        {
            if (message.CCs.Any())
            {
                SendToCC(message);
            }
            else
            {
                SendDefault(message);
            }
        }

        private void SendDefault(MailPreviewInfoDto message)
        {
            _emailSender.SendAsync(
                          to: message.SendToEmail,
                          subject: message.Subject,
                          body: message.BodyMessage,
                          isBodyHtml: true
                      );
        }

        private void SendToCC(MailPreviewInfoDto message)
        {
            var mailMessage = new MailMessage()
            {
                Body = message.BodyMessage,
                Subject = message.Subject
            };
            mailMessage.To.Add(message.SendToEmail);
            message.CCs.ForEach(cc => mailMessage.CC.Add(cc));
            mailMessage.IsBodyHtml = true;
            _emailSender.SendAsync(mailMessage);
        }

        public MailPreviewInfoDto GenerateEmailContent<TDto, TEntity>(TDto data, TEntity mailEntity) where TDto : class where TEntity : class
        {
            Type typeOfEntity = typeof(TEntity);
            Type typeOfDto = typeof(TDto);
            var templateId = typeOfEntity.GetProperty("Id").GetValue(mailEntity) as long?;

            var bodyMessage = typeOfEntity.GetProperty("BodyMessage").GetValue(mailEntity) as string;
            var subject = typeOfDto.GetProperty("Subject").GetValue(data) != null ? typeOfDto.GetProperty("Subject").GetValue(data) as string : "";

            var properties = typeOfDto.GetProperties().Where(s => s.Name != "SendToEmail" 
            && s.Name != "SignUrl" 
            && s.Name != "AuthorEmail"
            && s.Name != "ContractCode").Select(s => s.Name).ToArray();
            foreach (var property in properties)
            {
                bodyMessage = bodyMessage.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
                subject = subject.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
            }
            var sendTo = typeOfDto.GetProperty("SendToEmail").GetValue(data) != null ?
                typeOfDto.GetProperty("SendToEmail").GetValue(data) as string :
                typeOfEntity.GetProperty("SendToEmail").GetValue(mailEntity) != null ?
                typeOfEntity.GetProperty("SendToEmail").GetValue(mailEntity) as string : "";

            var ccs = typeOfEntity.GetProperty("CCs").GetValue(mailEntity) as string;
            var listCCs = string.IsNullOrEmpty(ccs) ? new List<string>() : ccs.Split(",").ToList();

            var type = typeOfEntity.GetProperty("Type").GetValue(mailEntity) as MailFuncEnum?;

            return new MailPreviewInfoDto
            {
                MailFuncType = type.HasValue ? type.Value : default,
                BodyMessage = bodyMessage.Replace("\"", "'"),
                Subject = subject,
                TemplateId = templateId.HasValue ? templateId.Value : 0,
                SendToEmail = sendTo,
                CCs = listCCs
            };
        }

        private dynamic EmailDispatchData(MailFuncEnum EmailType, long? id)
        {
            switch (EmailType)
            {
                case MailFuncEnum.Signing:
                    return GetDataContractMail(id);

                default:
                    return null;
            }
        }

        public async Task<List<EmailDto>> GetAllMailTemplate()
        {
            return await IQGetEmailTemplate()
                .OrderBy(s => s.Type)
                .ToListAsync();
        }

        public GetMailPreviewInfoDto PreviewTemplate(long templateId)
        {
            var template = WorkScope.GetAll<EmailTemplate>()
                .Where(x => x.Id == templateId)
                .FirstOrDefault();

            if (template == default)
            {
                throw new UserFriendlyException($"Can't find template with id {templateId}");
            }
            /*     ResultTemplateEmail<InputPayslipMailTemplate>*/

            var data = EmailDispatchData(template.Type, null);

            var result = GenerateEmailContent(data.Result, template);
            return new GetMailPreviewInfoDto
            {
                Id = result.TemplateId,
                Name = template.Name,
                Description = template.Description,
                BodyMessage = result.BodyMessage,
                Subject = result.Subject,
                CCs = result.CCs,
                PropertiesSupport = data.PropertiesSupport,
                Type = template.Type,
                SendToEmail = result.SendToEmail
            };
        }

        public GetMailPreviewInfoDto GetTemplateById(long templateId)
        {
            var template = WorkScope.GetAll<EmailTemplate>()
                .Where(x => x.Id == templateId)
                .FirstOrDefault();

            if (template == default)
            {
                throw new UserFriendlyException($"Can't find template with id {templateId}");
            }

            var data = EmailDispatchData(template.Type, null);
            var ccs = string.IsNullOrEmpty(template.CCs) ? new List<string>() : template.CCs.Split(",").ToList();

            return new GetMailPreviewInfoDto
            {
                Id = template.Id,
                Type = template.Type,
                BodyMessage = template.BodyMessage.Replace("\"", "'"),
                Subject = template.Subject,
                CCs = ccs,
                Name = template.Name,
                Description = template.Description,
                PropertiesSupport = data.PropertiesSupport,
                SendToEmail = template.SendToEmail,
            };
        }

        public async Task<UpdateTemplateDto> UpdateTemplate(UpdateTemplateDto input)
        {
            var entity = ObjectMapper.Map<EmailTemplate>(input);
            entity.CCs = string.Join(",", input.ListCC);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public MailPreviewInfoDto GetEmailContentById(MailFuncEnum mailType, long id)
        {
            var template = WorkScope.GetAll<EmailTemplate>().Where(x => x.Type == mailType).FirstOrDefault();

            var data = EmailDispatchData(mailType, id);

            return GenerateEmailContent(data.Result, template);
        }

        public void CreateDefaultMailTemplate(int tenantId)
        {
            var mailTemplates = new List<EmailTemplate>();
            var mails = WorkScope.GetAll<EmailTemplate>()
                .Where(q => q.TenantId == tenantId).Select(x => x.Type).ToList();

            Enum.GetValues(typeof(MailFuncEnum))
           .Cast<MailFuncEnum>()
           .ToList()
           .ForEach(e =>
           {
               if (!mails.Contains(e))
               {
                   var isSeedMailExist = DictionaryHelper.SeedMailDic.ContainsKey(e);
                   mailTemplates.Add(
                       new EmailTemplate
                       {
                           Subject = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Subject : string.Empty,
                           Name = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Name : string.Empty,
                           BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                           Description = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Description : string.Empty,
                           Type = e,
                           TenantId = tenantId
                       }
                   );
               }
           });
            WorkScope.InsertRange(mailTemplates);
        }

        public ResultTemplateEmail<ContractMailTemplateDto> GetDataContractMail(long? contractId)
        {
            if (contractId == null)
            {
                return new ResultTemplateEmail<ContractMailTemplateDto>
                {
                    Result = TemplateHelper.GetFakeContractMailTemplate()
                };
            }

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            var sendTo = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .Select(x => x.SignerEmail)
                .ToList();

            var contract = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .Select(x => new ContractMailTemplateDto
                {
                    ContractName = x.Name,
                    Message = x.EmailContent,
                    AuthorEmail = x.User.EmailAddress,
                    ContractCode = x.Code,
                    SendToEmail = x.User.EmailAddress,
                    SignUrl = $"{baseUrl}/app/signging/unAuthen-signing?settingId={x.Id}&contractId={x.Id}"
                })
               .FirstOrDefault();

            return new ResultTemplateEmail<ContractMailTemplateDto>()
            {
                Result = contract
            };
        }

        public EmailTemplateDto GetEmailTemplateDto(MailFuncEnum type)
        {
            var emailTemplateDto = WorkScope.GetAll<EmailTemplate>()
                .Where(s => s.Type == type)
                .Select(s => new EmailTemplateDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    BodyMessage = s.BodyMessage,
                    CCs = s.CCs,
                    Name = s.Name,
                    Subject = s.Subject,
                    SendToEmail = s.SendToEmail
                }).FirstOrDefault();

            return emailTemplateDto;
        }

        public void SetSendStatus(long id)
        {
            var item = WorkScope.GetAll<ContractSetting>().Where(x => x.Id == id).FirstOrDefault();
            item.IsSendMail = true;
            item.UpdateDate = Clock.Provider.Now;
        }
    }
}