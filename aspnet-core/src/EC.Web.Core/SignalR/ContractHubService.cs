using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using EC.SignalR;
using Abp.Dependency;

namespace EC.Web.SignalR
{
    public class ContractHubService : IContractHubService, ITransientDependency
    {
        private readonly IHubContext<ContractHub> _hubContext;

        public ContractHubService(IHubContext<ContractHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendContractUpdatedEvent(long contractId)
        {
            await _hubContext.Clients.Group($"Contract-{contractId}").SendAsync("ContractUpdated", contractId);
        }
    }
}
