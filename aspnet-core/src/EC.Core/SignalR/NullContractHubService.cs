using System.Threading.Tasks;

namespace EC.SignalR
{
    public class NullContractHubService : IContractHubService
    {
        public static NullContractHubService Instance { get; } = new NullContractHubService();

        public Task SendContractUpdatedEvent(long contractId)
        {
            return Task.CompletedTask;
        }
    }
}
