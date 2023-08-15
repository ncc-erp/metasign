using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using Abp.UI;
using EC.Authorization.Users;
using EC.Entities;
using EC.Manager.Contracts;
using EC.Manager.ContractSettings.Dto;
using EC.Manager.Notifications.Email;
using EC.Manager.Notifications.Notification;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Uitls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSettings
{
    public class ContractSettingManager : BaseManager, IContractSettingManager
    {
        protected readonly ContractManager _contractManager;
        private readonly IConfiguration _appConfiguration;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly NotificationManager _notificationManager;

        public ContractSettingManager(IWorkScope workScope, ContractManager contractManager, IConfiguration appConfiguration = null, EmailManager emailManager = null, BackgroundJobManager backgroundJobManager = null, NotificationManager notificationManager = null) : base(workScope)
        {
            _contractManager = contractManager;
            _appConfiguration = appConfiguration;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
            _notificationManager = notificationManager;
        }

        public async Task<CreatECSettingDto> Create(CreatECSettingDto input)
        {
            var existSetting = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == input.ContractId)
                .Select(x => new
                {
                    x.SignerName,
                    x.SignerEmail,
                    x.ContractRole,
                    x.Color,
                    x.ProcesOrder,
                    x.Role
                })
                .ToListAsync();

            foreach (var item in input.ContractSettings)
            {
                var entity = ObjectMapper.Map<ContractSetting>(item);

                entity.ContractId = input.ContractId;

                bool alreadyExist = existSetting.Any(x => x.SignerEmail == item.SignerEmail
                && x.SignerName == item.SignerName
                && x.ContractRole == item.ContractRole
                && x.Color == item.Color
                && x.ProcesOrder == item.ProcesOrder
                && x.Role == item.Role);

                if (alreadyExist)
                {
                    continue;
                }

                if (item.Id != null)
                {
                    await WorkScope.UpdateAsync(entity);
                }
                else
                {
                    if (entity.ContractRole == ContractRole.Signer)
                        entity.Status = ContractSettingStatus.NotConfirmed;
                    var id = await WorkScope.InsertAndGetIdAsync(entity);
                    item.Id = id;
                    if (item.ContractTemplateSignerId.HasValue)
                    {
                        var signPosition = WorkScope.GetAll<ContractTemplateSetting>()
                            .Where(x => x.ContractTemplateSignerId == item.ContractTemplateSignerId.Value).ToList();
                        var listEntity = new List<SignerSignatureSetting>();
                        signPosition.ForEach(x =>
                        {
                            var item = new SignerSignatureSetting
                            {
                                ContractSettingId = id,
                                FontColor = x.FontColor,
                                FontFamily = x.FontFamily,
                                FontSize = x.FontSize,
                                Height = x.Height,
                                IsSigned = x.IsSigned,
                                Page = x.Page,
                                PositionX = x.PositionX,
                                PositionY = x.PositionY,
                                Width = x.Width,
                                SignatureType = x.SignatureType,
                                ValueInput = x.ValueInput,
                            };
                            listEntity.Add(item);
                        });
                        await WorkScope.InsertRangeAsync(listEntity);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }
            }

            await _contractManager.SaveDraft(input.ContractId);

            return input;
        }

        public async Task<GetContractSettingDto> Get(long id)
        {
            return await QueryAllContractSetting()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<GetAllContractSettingDto> GetSettingByContractId(long contractId)
        {
            var listSigner = WorkScope.GetAll<ContractSetting>().AsNoTracking()
                .Where(x => x.ContractId == contractId).ToList();
            var contract = await WorkScope.GetAsync<Entities.Contract>(contractId);
            var dicIdRole = new Dictionary<long, string>();
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                dicIdRole = WorkScope.GetAll<ContractTemplateSigner>()
                   .Where(x => x.ContractTemplateId == contract.ContractTemplateId).ToDictionary(x => x.Id, y => y.Role);
            }
            var isOrder = false;
            if (listSigner.Count > 0)
            {
                isOrder = listSigner.Any(x => x.ProcesOrder != 1);
            }
            var signers = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .OrderBy(x => x.LastModificationTime)
                .Select(x => new GetContractSettingDto
                {
                    Id = x.Id,
                    Color = x.Color,
                    ContractFileName = x.Contract.File,
                    ContractRole = x.ContractRole,
                    ContractId = x.ContractId,
                    ProcesOrder = x.ProcesOrder,
                    SignerEmail = x.SignerEmail,
                    SignerName = x.SignerName,
                    Role = x.Role,
                }).ToListAsync();
            return new GetAllContractSettingDto
            {
                IsOrder = isOrder,
                Signers = signers
            };
        }

        public List<GetContractSettingDto> GetAll()
        {
            return QueryAllContractSetting().ToList();
        }

        public async Task<UpdatECSettingDto> Update(UpdatECSettingDto input)
        {
            var entity = ObjectMapper.Map<ContractSetting>(input);

            await WorkScope.UpdateAsync(entity);

            return input;
        }

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<ContractSetting>(id);

            return id;
        }

        public IQueryable<GetContractSettingDto> QueryAllContractSetting()
        {
            return WorkScope.GetAll<ContractSetting>()
                .Select(x => new GetContractSettingDto
                {
                    Id = x.Id,
                    ContractId = x.ContractId,
                    SignerName = x.SignerName,
                    SignerEmail = x.SignerEmail,
                    ContractRole = x.ContractRole,
                    Password = x.Password,
                    ProcesOrder = x.ProcesOrder,
                    Color = x.Color,
                    ContractFileName = x.Contract.File,
                    IsComplete = x.IsComplete
                });
        }

        public async Task VoidOrDeclineToSign(VoidOrDeclineToSignDto input)
        {
            var setting = await WorkScope.GetAsync<ContractSetting>(input.ContractSettingId);
            if (!setting.IsSendMail)
            {
                throw new UserFriendlyException("This signer has not been sent mail!");
            }
            var contract = await WorkScope.GetAsync<Entities.Contract>(setting.ContractId);
            var creator = await WorkScope.GetAsync<User>(contract.UserId);
            var isCreator = creator.EmailAddress == setting.SignerEmail;
            var note = isCreator ? $"{setting.SignerEmail} cancelledtheDocument {input.Reason}]" : $"{setting.SignerEmail} declinedtheDocument {input.Reason}]";
            if (contract.Status == ContractStatus.Cancelled)
            {
                throw new UserFriendlyException("This contract had been Canceled!");
            }
            var allSignerHasSentMail = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract).ThenInclude(x => x.User)
                .Where(x => x.ContractId == setting.ContractId && x.IsSendMail)
                .AsNoTracking()
                .ToListAsync();
            setting.Status = Constants.Enum.ContractSettingStatus.Rejected;
            contract.Status = Constants.Enum.ContractStatus.Cancelled;
            var history = new ContractHistory
            {
                Action = isCreator ? HistoryAction.CancelContract : HistoryAction.VoidToSign,
                AuthorEmail = setting.SignerEmail,
                ContractId = setting.ContractId,
                ContractStatus = ContractStatus.Cancelled,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"{note}"
            };
            await WorkScope.InsertAsync(history);
            await _notificationManager.RemoveOldJob(contract.Id);
            await _notificationManager.NotifyVoidOrDeclineToSign(setting, allSignerHasSentMail, history);
        }
    }
}