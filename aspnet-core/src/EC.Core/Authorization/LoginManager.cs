﻿using Microsoft.AspNetCore.Identity;
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

namespace EC.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
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
        }



        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string secretCode = "", string tenancyName = null, bool shouldLockout = true)
        {
            var result = await LoginAsyncInternalNoPass(token, secretCode, tenancyName, shouldLockout);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
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
    }
}
