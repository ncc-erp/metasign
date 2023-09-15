using Abp.UI;
using EC.Authorization.Users;
using EC.Entities;
using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Notification;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Uitls;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.ExpiredContract
{
    public class ExpiredContractManager : BaseManager
    {
        private readonly ContractHistoryManager _contractHistoryManager;
        private readonly NotificationManager _notificationManager;

        public ExpiredContractManager(IWorkScope workScope, ContractHistoryManager contractHistoryManager, NotificationManager notificationManager) : base(workScope)
        {
            _contractHistoryManager = contractHistoryManager;
            _notificationManager = notificationManager;
        }

        public async Task ExpiredContract(CancelExpiredContractDto input)
        {
            var contract = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == input.ContractId)
                .FirstOrDefault();
            if (contract.Status == ContractStatus.Cancelled)
            {
                throw new UserFriendlyException("This contract had been Canceled!");
            }

            contract.Status = ContractStatus.Cancelled;

            string author = WorkScope.GetAll<User>()
            .Where(x => x.Id == contract.UserId)
            .Select(x => x.EmailAddress)
            .FirstOrDefault();
            var history = new CreaContractHistoryDto
            {
                Action = HistoryAction.CancelContract,
                AuthorEmail= author,
                ContractId = input.ContractId,
                ContractStatus = ContractStatus.Cancelled,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"ExpiredContractMailContent"
            };
            await _notificationManager.NotifyCancelContract(input.ContractId, history.TimeAt, history.AuthorEmail);
            await _contractHistoryManager.Create(history);
        }
    }
}