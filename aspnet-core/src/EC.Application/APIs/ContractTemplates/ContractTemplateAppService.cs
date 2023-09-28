using EC.Manager.Contracts.Dto;
using EC.Manager.ContractTemplates;
using EC.Manager.ContractTemplates.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.APIs.ContractTemplates
{
    public class ContractTemplateAppService : ECAppServiceBase
    {
        private readonly ContractTemplateManager _contractTemplateManager;

        public ContractTemplateAppService(ContractTemplateManager contractTemplateManager)
        {
            _contractTemplateManager = contractTemplateManager;
        }

        [HttpPost]
        public async Task<long> Create(CreateContractTemplateDto input)
        {
            return await _contractTemplateManager.Create(input);
        }

        [HttpGet]
        public async Task<GetSignatureForContracttemplateDto> Get(long id)
        {
            return await _contractTemplateManager.Get(id);
        }

        [HttpGet]
        public async Task<List<GetContractTemplateDto>> GetAll(ContractTemplateFilterType? input)
        {
            return await _contractTemplateManager.GetAll(input);
        }

        [HttpPost]
        public async Task<GridResult<GetContractTemplateDto>> GetAllPaging(GetContractTemplateByFilterDto input)
        {
            return await _contractTemplateManager.GetAllPaging(input);
        }

        [HttpPut]
        public async Task Update(GetContractTemplateDto input)
        {
            await _contractTemplateManager.Update(input);
        }

        [HttpDelete]
        public async Task Delete(long Id)
        {
            await _contractTemplateManager.Delete(Id);
        }

        [HttpPut]
        public async Task UpdateProcessOrder(long contractTemplateId)
        {
            await _contractTemplateManager.UpdateProcessOrder(contractTemplateId);
        }

        [HttpGet]
        public async Task<object> CheckHasInput(long contractTemplateId)
        {
            return await _contractTemplateManager.CheckHasInput(contractTemplateId);
        }

        [HttpDelete]
        public async Task RemoveAllSignature(long contractTemplateId)
        {
            await _contractTemplateManager.RemoveAllSignature(contractTemplateId);
        }

        [HttpPost]
        public async Task<FileBase64Dto> DownloadMassTemplate(long templateId)
        {
            return await _contractTemplateManager.DownloadMassTemplate(templateId);
        }

        [HttpPost]
        public async Task<ValidImportMassContractTemplateDto> ValidImportMassTemplate([FromForm]UploadMassTemplateFileDto input)
        {
            return await _contractTemplateManager.ValidImportMassTemplate(input);
        }
    }
}