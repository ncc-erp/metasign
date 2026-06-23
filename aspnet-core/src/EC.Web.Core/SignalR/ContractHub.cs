using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EC.Web.SignalR
{
    public class ContractHub : Hub
    {
        public async Task JoinContract(long contractId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Contract-{contractId}");
        }
    }
}
