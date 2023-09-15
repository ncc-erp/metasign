using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EC.EntityFrameworkCore;
using EC.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace EC.Web.Tests
{
    [DependsOn(
        typeof(ECWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ECWebTestModule : AbpModule
    {
        public ECWebTestModule(ECEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ECWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ECWebMvcModule).Assembly);
        }
    }
}