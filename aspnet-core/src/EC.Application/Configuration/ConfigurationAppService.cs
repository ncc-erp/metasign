using Abp.Authorization;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using EC.Configuration.Dto;
using EC.GoogleClientId.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EC.Configuration
{
    
    public class ConfigurationAppService : ECAppServiceBase, IConfigurationAppService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [AbpAuthorize]
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<EmailConfigDto> GetEmailSetting()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new EmailConfigDto
            {
                DefaultAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                DisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                UseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials)
            } : new EmailConfigDto

            {
                DefaultAddress = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromAddress, tenantId.Value),
                DisplayName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromDisplayName, tenantId.Value),
                Host = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Host, tenantId.Value),
                Port = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Port, tenantId.Value),
                UserName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UserName, tenantId.Value),
                Password = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Password, tenantId.Value),
                EnableSsl = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.EnableSsl, tenantId.Value),
                UseDefaultCredentials = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UseDefaultCredentials, tenantId.Value)
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetEmailSetting(EmailConfigDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
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
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.DefaultFromAddress, input.DefaultAddress);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.DefaultFromDisplayName, input.DisplayName);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.Host, input.Host);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.Port, input.Port);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.UserName, input.UserName);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.Password, input.Password);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.EnableSsl, input.EnableSsl);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, EmailSettingNames.Smtp.UseDefaultCredentials, input.UseDefaultCredentials);
            }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<GetConfigurationDto> GetNotiExprireTime()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new GetConfigurationDto
            {
                NotiExprireTime = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NotiExprireTime)
            } : new GetConfigurationDto
            {
                NotiExprireTime = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.NotiExprireTime, tenantId.Value)
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetNotiExprireTime(GetConfigurationDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            { await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.NotiExprireTime, input.NotiExprireTime); }
            else
            { await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.NotiExprireTime, input.NotiExprireTime); }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<GoogleClientIdDto> GetGoogleClientId()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new GoogleClientIdDto
            {
                GoogleClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientId)
            } : new GoogleClientIdDto
            {
                GoogleClientId = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.GoogleClientId, tenantId.Value)
            };
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<CurrentPdfSignerDto> GetCurrentPdfSignerName()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new CurrentPdfSignerDto
            {
                CurrentPdfSigner = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.DefaultPDFSignerName)
            } : new CurrentPdfSignerDto
            {
                CurrentPdfSigner = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.DefaultPDFSignerName, tenantId.Value)
            };
        }
        [HttpGet]
        public async Task<LoginSettingDto> GetLoginSetting()
        {
            return new LoginSettingDto
            {
                EnableNormalLogin = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableNormalLogin)),
                MezonClientId = _configuration.GetValue<string>("Oauth2Mezon:Client_Id"),
                EnableLoginGoogle = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableLoginGoogle)),
                EnableLoginMezon = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableLoginMezon)),
                EnableLoginMicrosoft = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableLoginMicrosoft))

            };
        }

        [HttpPut]
        [AbpAuthorize]
        public async Task ChangeLoginSetting(LoginSettingDto loginSetting)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableNormalLogin, loginSetting.EnableNormalLogin.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableLoginMezon, loginSetting.EnableLoginMezon.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableLoginGoogle, loginSetting.EnableLoginGoogle.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableLoginMicrosoft, loginSetting.EnableLoginMicrosoft.ToString());
        }
        [HttpPost]
        [AbpAuthorize]
        public async Task SetGoogleClientId(GoogleClientIdDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.GoogleClientId, input.GoogleClientId);
            }
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.GoogleClientId, input.GoogleClientId);
            }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<IsEnableLoginByUsernameDto> GetIsEnableLoginByUsername()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new IsEnableLoginByUsernameDto
            {
                IsEnableLoginByUsername = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.IsEnableLoginByUsername)
            } : new IsEnableLoginByUsernameDto
            {
                IsEnableLoginByUsername = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.IsEnableLoginByUsername, tenantId.Value)
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetIsEnableLoginByUsername(IsEnableLoginByUsernameDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            { await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.IsEnableLoginByUsername, input.IsEnableLoginByUsername); }
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.IsEnableLoginByUsername, input.IsEnableLoginByUsername);
            }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<AWSCredentialDto> GetAWSCredential()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new AWSCredentialDto
            {
                AccessKeyId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSAccessKeyId),
                SecretKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSSecretKey),
                Region = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSRegion),
                BucketName = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSBucketName),
                Prefix = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSPrefix),
            } : new AWSCredentialDto
            {
                AccessKeyId = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSAccessKeyId, tenantId.Value),
                SecretKey = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSSecretKey, tenantId.Value),
                Region = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSRegion, tenantId.Value),
                BucketName = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSBucketName, tenantId.Value),
                Prefix = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSPrefix, tenantId.Value),
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetAWSCredential(AWSCredentialDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSAccessKeyId, input.AccessKeyId);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSSecretKey, input.SecretKey);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSRegion, input.Region);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSBucketName, input.BucketName);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AWSPrefix, input.Prefix);
            }
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.AWSAccessKeyId, input.AccessKeyId);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.AWSSecretKey, input.SecretKey);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.AWSRegion, input.Region);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.AWSBucketName, input.BucketName);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.AWSPrefix, input.Prefix);
            }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<MicrosoftClientIdDto> GetMicrosoftClientId()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new MicrosoftClientIdDto
            {
                MicrosoftClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.MicrosoftClientId)
            } : new MicrosoftClientIdDto
            {
                MicrosoftClientId = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.MicrosoftClientId, tenantId.Value)
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetMicrosoftClientId(MicrosoftClientIdDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.MicrosoftClientId, input.MicrosoftClientId);
            }
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.MicrosoftClientId, input.MicrosoftClientId);
            }
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task<SignServerUrlDto> GetSignServerUrlDto()
        {
            var tenantId = AbpSession.TenantId;
            return !tenantId.HasValue ? new SignServerUrlDto
            {
                BaseAddress = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.SignServerBaseAddress),
                AdminAPI = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.SignServerAdminAPI)
            } : new SignServerUrlDto
            {
                BaseAddress = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.SignServerBaseAddress, tenantId.Value),
                AdminAPI = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.SignServerAdminAPI, tenantId.Value)
            };
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task SetSignServerUrlDto(SignServerUrlDto input)
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SignServerBaseAddress, input.BaseAddress);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SignServerAdminAPI, input.AdminAPI);
            }
            else
            {
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.SignServerBaseAddress, input.BaseAddress);
                await SettingManager.ChangeSettingForTenantAsync(tenantId.Value, AppSettingNames.SignServerAdminAPI, input.AdminAPI);
            }
        }
    }
}