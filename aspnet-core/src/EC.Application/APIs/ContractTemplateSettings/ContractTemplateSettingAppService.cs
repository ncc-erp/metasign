using EC.Manager.ContractTemplateSettings;
using EC.Manager.ContractTemplateSettings.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.APIs.ContractTemplateSettings
{
    public class ContractTemplateSettingAppService : ECAppServiceBase
    {
        private readonly ContractTemplateSettingManager _contractTemplateSettingManager;

        public ContractTemplateSettingAppService(ContractTemplateSettingManager contractTemplateSettingManager)
        {
            _contractTemplateSettingManager = contractTemplateSettingManager;
        }

        public async Task<long> Create(CreateContractTemplateSettingDto input)
        {
            return await _contractTemplateSettingManager.Create(input);
        }

        public async Task Update(UpdateContractTemplateSettingDto input)
        {
            await _contractTemplateSettingManager.Update(input);
        }

        public async Task<UpdateContractTemplateSettingDto> Get(long id)
        {
            return await _contractTemplateSettingManager.Get(id);
        }

        public async Task Delete(long id)
        {
            await _contractTemplateSettingManager.Delete(id);
        }

        public async Task<List<GetAllSignerLocationDto>> GetAllSignLocation(long contractTemplateId)
        {
            return await _contractTemplateSettingManager.GetAllSignLocation(contractTemplateId);
        }
    }
}