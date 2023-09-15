using Abp.Dependency;
using EC.Manager.ContractSettings.Dto;
using EC.Manager.SignerSignatureSettings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.SignerSignatureSettings
{
    public interface ISignerSignatureSettingManager : ITransientDependency
    {
        List<GetSignerSignatureSettingDto> GetAll();
        Task<long> Create(CreateSignerSignatureSettingDto input);
        GetSignerSignatureSettingDto Get(long id);
    }
}
