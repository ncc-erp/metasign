using Abp.Domain.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NccCore.Helper
{
    public interface IUploadHelper : IDomainService
    {
        Task<IEnumerable<FileUploadInfo>> UploadFiles(IEnumerable<IFormFile> files, string subFolder, string prefixName = "");
        Task<FileUploadInfo> UploadFile(IFormFile file, string subFolder, string prefixName = "", bool isSCORM = false);
        string GetMediaFolderPath(string settingMedia, bool shouldDeleteOldFolder = false, string subFolder = "", bool isSCORM = false);
        string GetTenantFolder();
        void DeleteFile(string path,string filename);
    }
}
