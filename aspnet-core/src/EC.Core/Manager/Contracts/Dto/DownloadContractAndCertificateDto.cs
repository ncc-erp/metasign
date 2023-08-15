using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class DownloadContractAndCertificateDto
    {
        public long ContractId { get; set; }
        public DownloadContractType DownloadType { get; set; }
    }
}
