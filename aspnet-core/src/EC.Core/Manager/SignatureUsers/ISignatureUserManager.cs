using Abp.Dependency;
using EC.Manager.SignatureUsers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.SignatureUsers
{
    public interface ISignatureUserManager :ITransientDependency
    {
        Task<List<GetSignatureUserDto>> GetAll();
        GetSignatureUserDto Get(long id);
        Task<CreateSignatureUserDto> Create(CreateSignatureUserDto input);
        Task<UpdateSignatureUserDto> Update(UpdateSignatureUserDto input);
        Task<long> Delete(long id);
    }
}
