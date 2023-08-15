using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using EC.FileStorageServices;
using EC.MultiTenancy;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace EC.BackgroundWorkers.AWSS3
{
    public class ClearDownloadFolder : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IFileStoringService fileStoringService;
        private readonly TenantManager tenantManager;

        public ClearDownloadFolder(AbpTimer timer, IFileStoringService fileStoringService, TenantManager tenantManager) : base(timer)
        {
            Timer.Period = 1 * 12 * 60 * 60 * 1000; // days * hours * minutes * seconds * milliseconds
            this.fileStoringService = fileStoringService;
            this.tenantManager = tenantManager;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            lock (fileStoringService)
            {
                var tenantNames = tenantManager.Tenants
                    .Select(x => x.TenancyName)
                    .ToList();
                tenantNames.Add("host");
                foreach (var tenantName in tenantNames)
                {
                    fileStoringService.DeleteMultipleFiles(tenantName, Constants.Enum.FileCategory.Download, "").Wait();
                }
            }
        }
    }
}
