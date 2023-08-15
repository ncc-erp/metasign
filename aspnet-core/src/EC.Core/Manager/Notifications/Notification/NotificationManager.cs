using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using EC.BackgroundJobs.CancelExpiredContract;
using EC.BackgroundJobs.SendMail;
using EC.Entities;
using EC.Manager.Notifications.Email;
using EC.Manager.Notifications.Email.Dto;
using EC.MultiTenancy;
using EC.Utils;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Notification
{
    public class NotificationManager : BaseManager
    {
        private readonly EmailManager _emailManager;
        private readonly IConfiguration _appConfiguration;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<BackgroundJobInfo, long> _storeJob;

        public NotificationManager(IWorkScope workScope, EmailManager emailManager, IConfiguration appConfiguration, TenantManager tenantManager, IBackgroundJobManager backgroundJobManager, IRepository<BackgroundJobInfo, long> storeJob) : base(workScope)
        {
            _emailManager = emailManager;
            _appConfiguration = appConfiguration;
            _tenantManager = tenantManager;
            _backgroundJobManager = backgroundJobManager;
            _storeJob = storeJob;
        }

        public async Task NotifyCancelContract(long contractId, DateTime DateAt, string author)
        {
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Signing);

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            var contractSetting = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId && x.IsSendMail)
                .Select(x => new ContractMailTemplateDto
                {
                    ContractGuid = x.Contract.ContractGuid,
                    ExpireTime = x.Contract.ExpriredTime,
                    ContractName = x.Contract.Name,
                    SendToEmail = x.SignerEmail,
                    Subject = $"[Huỷ tài liệu] {x.Contract.Name}",
                    ContractCode = x.Contract.Code,
                    AuthorEmail = x.Contract.User.EmailAddress,
                    SendToName = x.SignerName,
                    AuthorName = x.Contract.User.FullName,
                    Message = $"Huỷ tài liệu lúc: {DateAt.ToString("HH:mm dd/MM/yyyy")} bởi {author}",
                    SignUrl = $"{baseUrl}app/signging/email-valid?settingId={x.Id}&contractId={x.ContractId}&tenantName={tenantName}",
                    LookupUrl= $"{baseUrl}app/email-login"
                }).ToListAsync();
            await NotifyToOwner(contractSetting.FirstOrDefault(), contractId, emailTemplate);
            var delaySendMail = 0;

            foreach (var item in contractSetting)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(item, emailTemplate);
                mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, item);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += ECConsts.DELAY_SEND_MAIL_SECOND;
            }
        }

        public async Task NotifyCompleteContract(long contractId)
        {
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Signing);
            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            var contractSetting = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .Select(x => new ContractMailTemplateDto
                {
                    ContractGuid = x.Contract.ContractGuid,
                    ExpireTime = x.Contract.ExpriredTime,
                    ContractName = x.Contract.Name,
                    SendToEmail = x.SignerEmail,
                    Subject = $"[Hoàn thành] {x.Contract.Name}",
                    Message = $"Hoàn thành tài liệu {x.Contract.Name}",
                    ContractCode = x.Contract.Code,
                    AuthorEmail = x.Contract.User.EmailAddress,
                    SendToName = x.SignerName,
                    AuthorName = x.Contract.User.FullName,
                    SignUrl = $"{baseUrl}app/signging/email-valid?settingId={x.Id}&contractId={x.ContractId}&tenantName={tenantName}",
                    LookupUrl= $"{baseUrl}app/email-login"
                }).ToListAsync();
            await NotifyToOwner(contractSetting.FirstOrDefault(), contractId, emailTemplate);
            var delaySendMail = 0;

            foreach (var item in contractSetting)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(item, emailTemplate);
                mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, item);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += ECConsts.DELAY_SEND_MAIL_SECOND;
            }
        }

        public async Task NotifyVoidOrDeclineToSign(ContractSetting settings, List<ContractSetting> allSignerHasSentMail, ContractHistory history)
        {
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Signing);

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            var contractSetting = allSignerHasSentMail
                .Select(x => new ContractMailTemplateDto
                {
                    ContractGuid = x.Contract.ContractGuid,
                    ExpireTime = x.Contract.ExpriredTime,
                    ContractName = x.Contract.Name,
                    SendToEmail = x.SignerEmail,
                    Subject = $"[Huỷ tài liệu] {x.Contract.Name}",
                    ContractCode = x.Contract.Code,
                    AuthorEmail = history.AuthorEmail,
                    SendToName = x.SignerName,
                    AuthorName = x.Contract.User.FullName,
                    Message = $"Huỷ tài liệu lúc: {history.TimeAt.ToString("HH:mm dd/MM/yyyy")} bởi {history.AuthorEmail}",
                    SignUrl = $"{baseUrl}app/signging/email-valid?settingId={x.Id}&contractId={x.ContractId}&tenantName={tenantName}",
                    LookupUrl= $"{baseUrl}app/email-login"
                }).ToList();
            await NotifyToOwner(contractSetting.FirstOrDefault(), settings.ContractId, emailTemplate);
            var delaySendMail = 0;

            foreach (var item in contractSetting)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(item, emailTemplate);
                mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, item);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += ECConsts.DELAY_SEND_MAIL_SECOND;
            }
        }

        private async Task NotifyToOwner(ContractMailTemplateDto input, long contractId, EmailTemplateDto emailTemplate)
        {
            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            var ownerMailInfo = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .Select(x => new ContractMailTemplateDto
                {
                    ContractGuid = x.ContractGuid,
                    ExpireTime = x.ExpriredTime,
                    ContractName = x.Name,
                    ContractCode = x.Code,
                    Subject = input.Subject,
                    Message = input.Message,
                    AuthorEmail = x.User.EmailAddress,
                    SendToName = x.User.FullName,
                    AuthorName = x.User.FullName,
                    SendToEmail = x.User.EmailAddress,
                    SignUrl = input.SignUrl,
                    LookupUrl=input.LookupUrl
                }).FirstOrDefault();
            var isJoin = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .AnyAsync(x => x.SignerEmail.ToLower().Trim() == ownerMailInfo.AuthorEmail.ToLower().Trim() && x.IsSendMail);
            if (isJoin)
            {
                return;
            }
            MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(ownerMailInfo, emailTemplate);
            mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, ownerMailInfo);
            _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(ECConsts.DELAY_SEND_MAIL_SECOND));
        }

        public async Task RemoveOldJob(long contractId)
        {
            var jobTypeName = typeof(CancelExpiredContract).FullName;
            _storeJob.GetAll()
               .Where(x => x.JobType.Contains(jobTypeName))
               .Where(x => x.JobArgs.Contains($"\"ContractId\":{contractId}"))
               .Select(x => x.Id)
               .ToList().ForEach(x =>
               {
                   _backgroundJobManager.Delete(x.ToString());
               });
        }
    }
}