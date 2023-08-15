using System.Threading.Tasks;
using Abp.Application.Services;
using EC.Sessions.Dto;

namespace EC.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
