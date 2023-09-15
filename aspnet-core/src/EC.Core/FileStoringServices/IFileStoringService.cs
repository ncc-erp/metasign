using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.FileStorageServices
{
    public interface IFileStoringService
    {
        public Task UploadFile(IFormFile file, string tenantName, FileCategory fileCategory, string guid, int? index);
        public Task<byte[]> DownloadFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName);
        public Task<List<byte[]>> DownloadMultipleFiles(string tenantName, FileCategory fileCategory, string guid);
        public Task DeleteFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName);
        public Task DeleteMultipleFiles(string tenantName, FileCategory fileCategory, string guid);
        public Task<string> GetDirectDownloadUrl(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName);
        public Task<List<string>> SearchForFiles(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName);
    }
}