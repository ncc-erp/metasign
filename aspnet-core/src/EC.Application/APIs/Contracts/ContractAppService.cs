﻿using EC.Manager.Contracts;
using EC.Manager.Contracts.Dto;
using EC.Manager.Notifications.Email.Dto;
using EC.WebService.PDFConverter.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.APIs.Contracts
{
    public class ContractAppService : ECAppServiceBase
    {
        private readonly ContractManager _contractManager;

        public ContractAppService(ContractManager contractManager)
        {
            _contractManager = contractManager;
        }

        [HttpGet]
        public List<GetContractDto> GetAll()
        {
            return _contractManager.GetAll();
        }

        [HttpPost]
        public async Task<GridResult<GetContractDto>> GetAllPaging(GridParam input)
        {
            return await _contractManager.GetAllPaging(input);
        }

        [HttpPost]
        public async Task<GridResult<GetContractDto>> GetContractByFilterPaging(GetContractByFilterDto input)
        {
            return await _contractManager.GetContractByFilterPaging(input);
        }

        [HttpPost]
        public async Task<long> Create(CreatECDto input)
        {
            return await _contractManager.Create(input);
        }

        [HttpGet]
        public async Task<GetContractDto> Get(long id)
        {
            return await _contractManager.Get(id);
        }

        [HttpPut]
        public async Task<UpdatECDto> Update(UpdatECDto input)
        {
            return await _contractManager.Update(input);
        }

        [HttpGet]
        public async Task<GetContractMailSettingDto> GetSendMailInfo(long contractId)
        {
            return await _contractManager.GetSendMailInfo(contractId);
        }

        [HttpGet]
        public async Task<List<ContractBase64ImageDto>> GetContractFileImage(long contractId)
        {
            return await _contractManager.GetContractFileImage(contractId);
        }

        [HttpPost]
        public async Task<object> SendMail(SendMailDto input)
        {
            return await _contractManager.SendMail(input);
        }

        [HttpPost]
        public async Task SendMailToViewer(SendMailDto input)
        {
            await _contractManager.SendMailToViewer(input);
        }

        [HttpPost]
        public async Task ResendMailAll(long contractId)
        {
            await _contractManager.ResendMailAll(contractId);
        }

        [HttpPost]
        public async Task ResendMailOne(ReSendMailDto input)
        {
            await _contractManager.ResendMailOne(input);
        }

        [HttpGet]
        public MailPreviewInfoDto GetContractMailContent(long contractId)
        {
            return _contractManager.GetContractMailContent(contractId);
        }

        [HttpPut]
        public async Task<long> SaveDraft(long contractId)
        {
            return await _contractManager.SaveDraft(contractId);
        }

        [HttpGet]
        public string GetSignUrl(long settingId, long contractId)
        {
            return _contractManager.GetSignUrl(settingId, contractId);
        }

        [HttpPost]
        public async Task<long> CancelContract(CancelContractDto input)
        {
            return await _contractManager.CancelContract(input);
        }

        [HttpGet]
        public async Task<GetContractDetailDto> GetContractDetail(long contractId)
        {
            return await _contractManager.GetContractDetail(contractId);
        }

        [HttpGet]
        public async Task<List<GetContractDesginInfo>> GetContractDesginInfo(long contractId)
        {
            return await _contractManager.GetContractDesginInfo(contractId);
        }

        [HttpGet]
        public async Task<GetContractStatisticDto> GetContractStatistic()
        {
            return await _contractManager.GetContractStatistic();
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _contractManager.Delete(id);
        }

        [HttpGet]
        public async Task<List<GetSignersDto>> GetAllSigners()
        {
            return await _contractManager.GetAllSigners();
        }

        [HttpPost]
        public async Task<object> ConvertFile([FromForm] UploadFileDto input)
        {
            return await _contractManager.UploadFile(input.File);
        }

        [HttpPost]
        public async Task<string> ConvertHtmltoPdf([FromBody] string html)
        {
            return await _contractManager.ConvertHtmltoPdf(html);
        }

        [HttpPost]
        public async Task<long> CreateContractFromTemplate(CreateContractFromTemplateDto input)
        {
            return await _contractManager.CreateContractFromTemplate(input);
        }

        [HttpGet]
        public async Task<string> DownloadContractAndCertificate(DownloadContractAndCertificateDto input)
        {
            return await _contractManager.DownloadContractAndCertificate(input);
        }

        [HttpGet]
        public async Task<bool> CheckContractHasSigned(long contractId)
        {
            return await _contractManager.CheckContractHasSigned(contractId);
        }

        [HttpPut]
        public async Task UpdateProcessOrder(long contractId)
        {
            await _contractManager.UpdateProcessOrder(contractId);
        }

        [HttpGet]
        public async Task<object> CheckHasInput(long contractId)
        {
            return await _contractManager.CheckHasInput(contractId);
        }

        [HttpDelete]
        public async Task RemoveAllSignature(long contractId)
        {
            await _contractManager.RemoveAllSignature(contractId);
        }

        [HttpPost]
        public async Task SetNotiExpiredContract(long contractId)
        {
            await _contractManager.SetNotiExpiredContract(contractId);
        }

        [HttpPost]
        public async Task<string> UploadAndConvert([FromForm] UploadFileDto input)
        {
            return await _contractManager.UploadAndConvert(input.File);
        }
    }
}