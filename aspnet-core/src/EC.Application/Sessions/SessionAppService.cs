using Abp.Auditing;
using EC.Configuration;
using EC.Sessions.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.Sessions
{
    public class SessionAppService : ECAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }

            output.GoogleClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientId);

            output.MicrosoftClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.MicrosoftClientId);

            output.IsEnableLoginByUsername = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.IsEnableLoginByUsername);

            return output;
        }
    }
}