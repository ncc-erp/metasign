using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using EC.Configuration.Dto;
using EC.Constants.FileStoring;
using EC.FileStorageServices;
using EC.FileStoringServices;
using EC.GoogleClientId.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EC.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ECAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        [HttpGet]
        public async Task<EmailConfigDto> GetEmailSetting()
        {
            return new EmailConfigDto
            {
                DefaultAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                DisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                UseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        [HttpPost]
        public async Task SetEmailSetting(EmailConfigDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress, input.DefaultAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName, input.DisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.Host);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port, input.Port);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName, input.UserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password, input.Password);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl, input.EnableSsl);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials, input.UseDefaultCredentials);
        }


        [HttpGet]
        public async Task<GetConfigurationDto> GetNotiExprireTime()
        {
            return new GetConfigurationDto
            {
                NotiExprireTime = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NotiExprireTime)
            };
        }

        [HttpPost]
        public async Task SetNotiExprireTime(GetConfigurationDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NotiExprireTime, input.NotiExprireTime);
        }

        [HttpGet]
        public async Task<GoogleClientIdDto> GetGoogleClientId()
        {
            return new GoogleClientIdDto
            {
                GoogleClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientId)
            };
        }

        [HttpGet]
        public async Task<CurrentPdfSignerDto> GetCurrentPdfSignerName()
        {
            return new CurrentPdfSignerDto
            {
                CurrentPdfSigner = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.DefaultPDFSignerName)
            };
        }

        [HttpPost]
        public async Task SetGoogleClientId(GoogleClientIdDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.GoogleClientId, input.GoogleClientId);
        }

        [HttpGet]
        public async Task<IsEnableLoginByUsernameDto> GetIsEnableLoginByUsername()
        {
            return new IsEnableLoginByUsernameDto
            {
                IsEnableLoginByUsername = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.IsEnableLoginByUsername)
            };
        }

        [HttpPost]
        public async Task SetIsEnableLoginByUsername(IsEnableLoginByUsernameDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.IsEnableLoginByUsername, input.IsEnableLoginByUsername);
        }

        [HttpGet]
        public async Task<AWSCredentialDto> GetAWSCredential()
        {
            return new AWSCredentialDto
            {
                AccessKeyId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSAccessKeyId),
                SecretKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSSecretKey)
            };
        }

        [HttpPost]
        public async Task SetAWSCredential(AWSCredentialDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSAccessKeyId, input.AccessKeyId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSSecretKey, input.SecretKey);
        }
    }
}
