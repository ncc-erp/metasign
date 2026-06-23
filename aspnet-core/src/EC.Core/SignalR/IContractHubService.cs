using System.Threading.Tasks;

namespace EC.SignalR
{
    public interface IContractHubService
    {
        Task SendContractUpdatedEvent(long contractId);
    }
}
