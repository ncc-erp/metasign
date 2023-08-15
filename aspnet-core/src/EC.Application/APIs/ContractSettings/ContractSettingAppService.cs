using EC.Manager.ContractSettings;
using EC.Manager.ContractSettings.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.APIs.ContractSettings
{
    public class ContractSettingAppService : ECAppServiceBase
    {
        private readonly ContractSettingManager _contractSettingManager;

        public ContractSettingAppService(ContractSettingManager contractSettingManager)
        {
            _contractSettingManager = contractSettingManager;
        }

        [HttpGet]
        public List<GetContractSettingDto> GetAll()
        {
            return _contractSettingManager.GetAll();
        }

        [HttpPost]
        public async Task<CreatECSettingDto> Create(CreatECSettingDto input)
        {
            return await _contractSettingManager.Create(input);
        }

        [HttpGet]
        public async Task<GetContractSettingDto> Get(long id)
        {
            return await _contractSettingManager.Get(id);
        }

        [HttpGet]
        public async Task<GetAllContractSettingDto> GetSettingByContractId(long contractId)
        {
            return await _contractSettingManager.GetSettingByContractId(contractId);
        }

        [HttpPut]
        public async Task<UpdatECSettingDto> Update(UpdatECSettingDto input)
        {
            return await _contractSettingManager.Update(input);
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _contractSettingManager.Delete(id);
        }

        [HttpPost]
        public async Task VoidOrDeclineToSignDto(VoidOrDeclineToSignDto input)
        {
            await _contractSettingManager.VoidOrDeclineToSign(input);
        }
    }
}