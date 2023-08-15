using EC.Manager.ContractTemplateSigners;
using EC.Manager.ContractTemplateSigners.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.APIs.ContractTemplateSigners
{
    public class ContractTemplateSignerAppService : ECAppServiceBase
    {
        private readonly ContractTemplateSignerManager _contractTemplateSigners;

        public ContractTemplateSignerAppService(ContractTemplateSignerManager contractTemplateSigners)
        {
            _contractTemplateSigners = contractTemplateSigners;
        }

        [HttpPost]
        public async Task<UpdateListContractTemplateSignerDto> Create(UpdateListContractTemplateSignerDto input)
        {
           return await _contractTemplateSigners.Create(input);
        }

        [HttpPut]
        public async Task Update(UpdateListContractTemplateSignerDto input)
        {
            await _contractTemplateSigners.Update(input);
        }

        [HttpGet]
        public async Task<GetContractTemplateSignerDto> Get(long id)
        {
            return await _contractTemplateSigners.Get(id);
        }

        [HttpGet]
        public async Task<GetAllContractTemplateSignerDto> GetAllByContractTemplateId(long id)
        {
            return await _contractTemplateSigners.GetAllByContractTemplateId(id);
        }

        [HttpDelete]
        public async Task Delete(long id)
        {
            await _contractTemplateSigners.Delete(id);
        }
    }
}