using EC.Manager.Contracts.Dto;
using EC.Manager.FileStoring;
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
    }
}
