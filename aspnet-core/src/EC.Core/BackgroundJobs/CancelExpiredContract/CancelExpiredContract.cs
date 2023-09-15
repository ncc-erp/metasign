using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using EC.Manager.Contracts;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.ExpiredContract;
using System.Threading.Tasks;

namespace EC.BackgroundJobs.CancelExpiredContract
{
    public class CancelExpiredContract : AsyncBackgroundJob<CancelExpiredContractDto>, ITransientDependency
    {
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        private readonly ExpiredContractManager _expiredContractManager;

        public CancelExpiredContract(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, ContractManager contractManager, ExpiredContractManager expiredContractManager)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _expiredContractManager = expiredContractManager;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(CancelExpiredContractDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                await _expiredContractManager.ExpiredContract(args);
                uow.SaveChanges();
            }
        }
    }
}