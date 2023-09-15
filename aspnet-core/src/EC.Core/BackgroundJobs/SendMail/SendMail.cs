using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using EC.Manager.ContractHistories;
using EC.Manager.Notifications.Email;
using EC.Manager.Notifications.Email.Dto;

namespace EC.BackgroundJobs.SendMail
{
    public class SendMail : BackgroundJob<MailPreviewInfoDto>, ITransientDependency
    {
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        private readonly EmailManager _emailManager;
        private readonly ContractHistoryManager _contractHistoryManager;

        public SendMail(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, EmailManager emailManager, ContractHistoryManager contractHistoryManager)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _emailManager = emailManager;
            _contractHistoryManager = contractHistoryManager;
        }

        [UnitOfWork]
        public override void Execute(MailPreviewInfoDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _emailManager.Send(args);
                if (args.MailHistory != null)
                {
                    _contractHistoryManager.CreateSync(args.MailHistory);
                }
                if (args.ContractSettingId.HasValue)
                {
                    _emailManager.SetSendStatus(args.ContractSettingId.Value);
                    uow.SaveChanges();
                }
            }
        }
    }
}