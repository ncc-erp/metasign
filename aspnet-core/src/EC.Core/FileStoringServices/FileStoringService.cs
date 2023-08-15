using Abp.Application.Services;
using Abp.Dependency;
using Abp.Runtime.Session;
using EC.FileStorageServices;
using EC.MultiTenancy;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.FileStoringServices
{
    public class FileStoringService : ApplicationService
    {
        private readonly IFileStoringService fileStoringService;
        private readonly TenantManager tenantManager;
        private readonly IAbpSession session;

        public FileStoringService(IFileStoringService fileStoringService, TenantManager tenantManager, IAbpSession session)
        {
            this.fileStoringService = fileStoringService;
            this.tenantManager = tenantManager;
            this.session = session;
        }

        public async Task UploadFile(IFormFile file, FileCategory fileCategory, string guid, int? index)
        {
            var tenantName = await GetTenantName();
            await fileStoringService.UploadFile(file, tenantName, fileCategory, guid, index);
        }

        public async Task<byte[]> DownloadFile(FileCategory fileCategory, string guid, int? index, string fileName)
        {
            var tenantName = await GetTenantName();
            return await fileStoringService.DownloadFile(tenantName, fileCategory, guid, index, fileName);
        }

        public async Task<string> GetDownloadUrl(FileCategory fileCategory, string guid, int? index, string fileName)
        {
            var tenantName = await GetTenantName();
            return await fileStoringService.GetDirectDownloadUrl(tenantName, fileCategory, guid, index, fileName);
        }

        public async Task<List<byte[]>> DownloadMultiple(FileCategory fileCategory, string guid)
        {
            var tenantName = await GetTenantName();
            return await fileStoringService.DownloadMultipleFiles(tenantName, fileCategory, guid);
        }

        public async Task DeleteFile(FileCategory fileCategory, string guid, int? index, string fileName)
        {
            var tenantName = await GetTenantName();
            await fileStoringService.DeleteFile(tenantName, fileCategory, guid, index, fileName);
        }

        public async Task DeleteMultipleFiles(FileCategory fileCategory, string guid)
        {
            var tenantName = await GetTenantName();
            await fileStoringService.DeleteMultipleFiles(tenantName, fileCategory, guid);
        }

        public async Task<List<string>> SearchForFiles(FileCategory fileCategory, string guid, int? index, string fileName)
        {
            var tenantName = await GetTenantName();
            return await fileStoringService.SearchForFiles(tenantName, fileCategory, guid.ToString(), index, fileName);
        }

        private async Task<string> GetTenantName()
        {
            if (session.TenantId.HasValue)
            {
                var tenant = await tenantManager.GetByIdAsync(session.TenantId.Value);
                return tenant.TenancyName;
            }
            return "host";
        }
    }
}