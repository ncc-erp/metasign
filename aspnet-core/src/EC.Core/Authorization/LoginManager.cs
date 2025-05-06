using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using EC.Authorization.Roles;
using EC.Authorization.Users;
using EC.MultiTenancy;
using Abp.UI;
using EC.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Google.Apis.Auth;
using System.Linq;
using Abp.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using static EC.Constants.Enum;
using Microsoft.Extensions.Configuration;
using EC.WebService.Mezon.Dto;

namespace EC.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private readonly IConfiguration _configuration;
        public LogInManager(
            UserManager userManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager, 
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository, 
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher, 
            RoleManager roleManager,
            IConfiguration configuration,
            UserClaimsPrincipalFactory claimsPrincipalFactory) 
            : base(
                  userManager, 
                  multiTenancyConfig,
                  tenantRepository, 
                  unitOfWorkManager, 
                  settingManager, 
                  userLoginAttemptRepository, 
                  userManagementConfig, 
                  iocResolver, 
                  passwordHasher, 
                  roleManager, 
                  claimsPrincipalFactory)
        {
            _configuration = configuration;
        }



        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, LoginType type, string secretCode = "", string tenancyName = null, bool shouldLockout = true)
        {
            AbpLoginResult<Tenant, User> result;

            switch (type)
            {
                case LoginType.Google:
                    {
                        result = await LoginAsyncInternalNoPass(token, secretCode, tenancyName, shouldLockout);
                        break;
                    }
                case LoginType.Microsoft:
                    {
                        result = await LoginAsyncWithMicrosoft(token, secretCode, tenancyName, shouldLockout);
                        break;
                    }
                default: throw new UserFriendlyException("Invalid login type");

            }

            var user = result.User;

            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);

            return result;
        }

        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant,User>> LoginAsyncNoPassWithMezon(AuthOauth2Mezon input, string tenancyName = null , bool shouldLockout = true)
        {
          
            var result = await LoginAsyncWithMezon(tenancyName, shouldLockout, input);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        public async Task<AbpLoginResult<Tenant,User>> LoginAsyncWithMezon(string tenancyName, bool shouldLockout, AuthOauth2Mezon input)
        {
          
            try
            {
                var emailAddress = input.sub;
                var clientAppId = _configuration.GetValue<string>("Oauth2Mezon:CLient_Id");
                var corectAudience = input.aud.Any(s => s== clientAppId);
                var correctIssuer = input.iss == "https://oauth2.mezon.ai";
                var correctExpriryTime = input.auth_time != null || input.auth_time > 0 ;

                Tenant tenant = null;

                if(corectAudience && correctExpriryTime &&  correctIssuer)
                {
                    return await ValidateAndLoginUserAsync(tenant, emailAddress,tenancyName, shouldLockout);
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }catch(InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }

        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(string token, string secretCode, string tenancyName, bool shouldLockout)
        {
            if (token.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(token));
            }
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                var emailAddress = payload.Email;
                // checking
                var clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.GoogleClientId);//get clientAppId from setting
                var correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                var correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                var correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;

                Tenant tenant = null;

                if (correctAudience && correctIssuer && correctExpriryTime)
                {
                    //Get and check tenant
                   return await ValidateAndLoginUserAsync(tenant,emailAddress,tenancyName, shouldLockout);
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }
            catch (InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }

        private async Task<AbpLoginResult<Tenant, User>> ValidateAndLoginUserAsync(Tenant tenant,string emailAddress,string tenancyName,bool shouldLockout)
        {
            using (UnitOfWorkManager.Current.SetTenantId(null))
            {
                if (!MultiTenancyConfig.IsEnabled)
                {
                    tenant = await GetDefaultTenantAsync();
                }
                else if (!string.IsNullOrWhiteSpace(tenancyName))
                {
                    tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                    if (tenant == null)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                    }

                    if (!tenant.IsActive)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                    }
                }
            }
            var tenantId = tenant == null ? (int?)null : tenant.Id;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await UserManager.InitializeOptionsAsync(tenantId);

                var user = await UserManager.FindByNameOrEmailAsync(tenantId, emailAddress);
                if (user == null)
                {
                    throw new UserFriendlyException(string.Format("Email chưa được đăng ký"));
                }

                if (await UserManager.IsLockedOutAsync(user))
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                }
                if (shouldLockout)
                {
                    if (await TryLockOutAsync(tenantId, user.Id))
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                    }
                }

                await UserManager.ResetAccessFailedCountAsync(user);
                return await CreateLoginResultAsync(user, tenant);
            }
        }
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncWithMicrosoft(string token, string secretCode, string tenancyName = null, bool shouldLockout = true)
        {
            if (token.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                var emailAddress = GetEmailFromIdToken(token);

                Tenant tenant = null;
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    //Get and check tenant
                   return await ValidateAndLoginUserAsync(tenant,emailAddress,tenancyName,shouldLockout);
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }
            catch (SecurityTokenException)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }

        public string GetEmailFromIdToken(string idToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(idToken);

            foreach (var claim in jwtToken.Claims)
            {
                if (claim.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase)
                    || claim.Type.Equals("email", StringComparison.OrdinalIgnoreCase)
                    || claim.Type.Equals("upn", StringComparison.OrdinalIgnoreCase)
                    || claim.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }

            return null;
        }


    }
}