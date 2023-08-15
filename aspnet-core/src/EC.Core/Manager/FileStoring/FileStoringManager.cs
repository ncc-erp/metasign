using EC.Entities;
using EC.FileStoringServices;
using EC.Manager.Contracts;
using EC.Manager.Contracts.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.FileStoring
{
    public class FileStoringManager : BaseManager
    {
        private readonly FileStoringService fileStoringService;
        private readonly ContractManager contractManager;

        public FileStoringManager(IWorkScope workScope, 
            FileStoringService fileStorageServices, 
            ContractManager contractManager) : base(workScope)
        {
            this.fileStoringService = fileStorageServices;
            this.contractManager = contractManager;
        }

        public async void WaitForUploadingFile(FileCategory fileCategory, string guid, int index, string fileName)
        {
            int count = 0;
            do
            {
                count = (await fileStoringService.SearchForFiles(fileCategory, guid, index, fileName)).Count;
            } while (count == 0);
        }

        public async void WaitForDeletingFiles(FileCategory fileCategory, string guid, int? index, string? fileName)
        {
            int count;
            do
            {
                count = (await fileStoringService.SearchForFiles(fileCategory, guid, index, fileName)).Count;
            } while (count != 0);
        }

        public async Task<string> GetPresignedDownloadUrl(DownloadContractAndCertificateDto input)
        {
            var contract = WorkScope.GetAll<Contract>()
                .First(x => x.Id.Equals(input.ContractId));
            var fileNameOnly = Path.GetFileNameWithoutExtension(contract.File);
            string fileBase64 = await contractManager.DownloadContractAndCertificate(input);
            byte[] fileBytes = Convert.FromBase64String(fileBase64.Substring(fileBase64.IndexOf(',') + 1));

            var stream = new MemoryStream(fileBytes);
            // Index is used for multiple file
            // if only has 1 file, index will be 1
            int index = 1;
            List<string> searchResult = await fileStoringService.SearchForFiles(FileCategory.Download, contract.ContractGuid.ToString(), index, null);

            if (input.DownloadType.Equals(DownloadContractType.All))
            {
                if (!searchResult.Any(x => x.Contains(fileNameOnly + ".zip")))
                {
                    IFormFile file = new FormFile(stream, 0, stream.Length, fileNameOnly, fileNameOnly + ".zip");
                    await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                }
                WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + ".zip");
                return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + ".zip");
            }
            else if (input.DownloadType == DownloadContractType.Contract)
            {
                if (!searchResult.Any(x => x.Contains(fileNameOnly + ".pdf")))
                {
                    IFormFile file = new FormFile(stream, 0, stream.Length, fileNameOnly, fileNameOnly + ".pdf");
                    await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                }
                WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + ".pdf");
                return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + ".pdf");
            }
            else /*if (input.DownloadType == DownloadContractType.Certificate)*/
            {
                if (!searchResult.Any(x => x.Contains("Certificate.pdf")))
                {
                    IFormFile file = new FormFile(stream, 0, stream.Length, "Certificate", "Certificate.pdf");
                    await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                }
                WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, "Certificate.pdf");
                return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, "Certificate.pdf");
            }
        }

        public async Task ClearContractDownloadFiles(long contractId)
        {
            string contractGuid = WorkScope.GetAll<Contract>()
                .First(x => x.Id.Equals(contractId))
                .ContractGuid.ToString();
            await fileStoringService.DeleteMultipleFiles(FileCategory.Download, contractGuid);
            WaitForDeletingFiles(FileCategory.Download, contractGuid, null, null);
        }
    }
}
