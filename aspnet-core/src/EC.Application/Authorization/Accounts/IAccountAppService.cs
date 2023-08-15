using System.Threading.Tasks;
using Abp.Application.Services;
using EC.Authorization.Accounts.Dto;

namespace EC.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
