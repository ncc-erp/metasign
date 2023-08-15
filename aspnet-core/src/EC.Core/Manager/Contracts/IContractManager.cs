using Abp.Dependency;
using EC.Manager.Contracts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.Contracts
{
    public interface IContractManager : ITransientDependency
    {
        List<GetContractDto> GetAll();
        Task<long> Create(CreatECDto input);
        Task<GetContractDto> Get(long id);
        Task<UpdatECDto> Update(UpdatECDto input);
        Task<GetContractMailSettingDto> GetSendMailInfo(long contractId);

    }
}
