using Abp.Dependency;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using EC.Authorization.Roles;
using EC.Authorization.Users;
using EC.BackgroundWorkers.AWSS3;
using EC.Configuration;
using EC.Localization;
using EC.MultiTenancy;
using EC.Timing;

namespace EC
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class ECCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            ECLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = ECConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();

            Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));

            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = ECConsts.DefaultPassPhrase;
            SimpleStringCipher.DefaultPassPhrase = ECConsts.DefaultPassPhrase;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ECCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}