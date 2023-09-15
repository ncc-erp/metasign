using EC.Manager.Contracts.Dto;
using EC.Manager.FileStoring;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.FileStoring
{
    public class FileStoringAppService : ECAppServiceBase
    {
        private readonly FileStoringManager fileStoringManager;

        public FileStoringAppService(FileStoringManager fileStoringManager)
        {
            this.fileStoringManager = fileStoringManager;
        }

        [HttpGet]
        public async Task<string> GetPresignedDownloadUrl(DownloadContractAndCertificateDto input)
        {
            return await fileStoringManager.GetPresignedDownloadUrl(input);
        }

        [HttpDelete]
        public async Task ClearContractDownloadFiles(long contractId)
        {
            await fileStoringManager.ClearContractDownloadFiles(contractId);
        }

        [HttpPost]
        public async Task UploadUnsignedContract(long contractId, IFormFile file)
        {
            await fileStoringManager.UploadUnsignedContract(contractId, file);
        }

        [HttpGet]
        public async Task<string> DownloadUnsignedContractBase64(long contractId)
        {
            return await fileStoringManager.DownloadUnsignedContractBase64(contractId);
        }

        [HttpDelete]
        public async Task DeleteUnsignedContract(long contractId)
        {
            await fileStoringManager.DeleteUnsignedContract(contractId);
        }

        [HttpPost]
        public async Task UploadContract(long contractId, IFormFile file)
        {
            await fileStoringManager.UploadContract(contractId, file);
        }

        [HttpGet]
        public async Task<string> DownloadLatestContractBase64(long contractId)
        {
            return await fileStoringManager.DownloadLatestContractBase64(contractId);
        }
    }
}
