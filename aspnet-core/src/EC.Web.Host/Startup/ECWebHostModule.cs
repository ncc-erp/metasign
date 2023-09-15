using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EC.Configuration;
using Abp.Threading.BackgroundWorkers;
using EC.BackgroundWorkers.AWSS3;

namespace EC.Web.Host.Startup
{
    [DependsOn(
       typeof(ECWebCoreModule))]
    public class ECWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ECWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ECWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<ClearDownloadFolder>());
        }
    }
}
