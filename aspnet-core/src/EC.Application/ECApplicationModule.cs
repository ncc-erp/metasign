using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EC.Authorization;

namespace EC
{
    [DependsOn(
        typeof(ECCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ECApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ECAuthorizationProvider>();
            Configuration.MultiTenancy.IsEnabled = ECConsts.MultiTenancyEnabled;
            Configuration.MultiTenancy.TenantIdResolveKey = "Abp-TenantId";
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ECApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
