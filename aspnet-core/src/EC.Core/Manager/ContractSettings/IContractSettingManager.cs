using Abp.Dependency;
using EC.Manager.Contracts.Dto;
using EC.Manager.ContractSettings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSettings
{
    public interface IContractSettingManager : ITransientDependency
    {
        List<GetContractSettingDto> GetAll();
        Task<CreatECSettingDto> Create(CreatECSettingDto input);
        Task<GetContractSettingDto> Get(long id);
        Task<UpdatECSettingDto> Update(UpdatECSettingDto input);
        Task<GetAllContractSettingDto> GetSettingByContractId(long contractId);
    }
}
