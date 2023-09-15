using Abp.Domain.Uow;
using Abp.UI;
using Castle.MicroKernel.Registration;
using EC.Configuration;
using EC.Entities;
using EC.Manager.Contracts;
using EC.Manager.LookupPage.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static EC.Constants.Enum;

namespace EC.Manager.LookupPage
{
    public class LookupPageManager : BaseManager
    {
        private readonly IConfiguration _appConfiguration;
        private readonly ContractManager contractManager;

        public LookupPageManager(IWorkScope workScope,
            IConfiguration configuration,
            ContractManager contractManager) : base(workScope)
        {
            _appConfiguration = configuration;
            this.contractManager = contractManager;
        }

        public async Task<GridResult<LookupContractDto>> GetAllPaging(GetLookupContractDto input)
        {
            var dicContractSettingProgress =
                WorkScope.GetAll<ContractSetting>()
               .Where(x => x.ContractRole == ContractRole.Signer)
               .GroupBy(x => x.ContractId)
               .Select(x => new
               {
                   x.Key,
                   Progress = new
                   {
                       numberOfSetting = x.Count(),
                       CountComplete = x.Count(s => s.IsComplete)
                   }
               })
               .ToDictionary(x => x.Key, x => x.Progress);

            var contractIds = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.SignerEmail == input.Email)
                .Select(x => new
                {
                    x.ContractId,
                    x.Id,
                    x.ContractRole,
                    x.IsComplete,
                    x.IsSendMail
                })
                .Distinct()
            .ToListAsync();

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            var exprireNotiConfig = int.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NotiExprireTime));

            var userContractIds = contractIds.Select(x => x.ContractId).ToList();

            var query = WorkScope.GetAll<Contract>()
                .OrderByDescending(x => x.CreationTime)
                .Where(x => userContractIds.Contains(x.Id))
                .Select(x => new LookupContractDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    File = x.File,
                    Status = x.Status,
                    CreationTime = x.CreationTime,
                    ExpriedTime = x.ExpriredTime,
                    UpdatedTime = x.LastModificationTime,
                    NumberOfSetting = dicContractSettingProgress.ContainsKey(x.Id) ? dicContractSettingProgress[x.Id].numberOfSetting : 0,
                    CountCompleted = dicContractSettingProgress.ContainsKey(x.Id) ? dicContractSettingProgress[x.Id].CountComplete : 0,
                    ContractBase64 = x.FileBase64,
                    ContractGuid = x.ContractGuid
                });

            var result = await query.GetGridResult(query, input.GridParam);

            var ResultIds = result.Items.Select(x => x.Id).ToList();

            var dicContractBase64s = WorkScope.GetAll<ContractSigning>()
                                .Where(x => ResultIds.Contains(x.ContractId))
                                .GroupBy(x => x.ContractId)
                                .Select(x => new
                                {
                                    x.Key,
                                    base64 = x.OrderByDescending(s => s.TimeAt).Select(x => x.SigningResult).FirstOrDefault()
                                })
                                .ToDictionary(x => x.Key, x => x.base64);

            foreach (var item in result.Items)
            {
                item.ContractBase64 = dicContractBase64s.ContainsKey(item.Id) ? dicContractBase64s[item.Id] : item.ContractBase64;
                item.IsAllowSigning = contractIds.Any(s => s.ContractId == item.Id && s.ContractRole == ContractRole.Signer && !s.IsComplete && s.IsSendMail);
                item.SignUrl = contractIds.FirstOrDefault(s => s.ContractId == item.Id) != default ? $"{baseUrl}app/signging/email-valid?{HttpUtility.UrlEncode($"settingId={contractIds.FirstOrDefault(s => s.ContractId == item.Id).Id}&contractId={item.Id}")}" : "";
                item.IsHasSigned = (WorkScope.GetAll<ContractSigning>()
                    .Where(s => s.ContractId == item.Id && !string.IsNullOrEmpty(s.SignartureBase64))
                .ToList()).Any();
            }

            return result;
        }

        public async Task<ContractDetailByGuidDto> GetContractDetailByGuid(GetContractDetailByGuidDto input)
        {
            Guid guid;
            try
            {
                guid = Guid.Parse(input.Guid);
            }
            catch(Exception e)
            {
                return null;
            }
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var contractId = WorkScope.GetAll<ContractSetting>()
                    .Where(x => x.Contract.ContractGuid.Equals(guid))
                    .Where(x => x.SignerEmail.ToLower().Trim().Equals(input.Email.ToLower().Trim())
                    || x.Contract.User.EmailAddress.ToLower().Trim().Equals(input.Email.ToLower().Trim()))
                    .Select(x => x.ContractId)
                    .FirstOrDefault();
                if(contractId == default)
                {
                    return null;
                }

                var contract = await WorkScope.GetAll<Contract>()
                    .Include(x => x.User)
                    .Where(x => x.Id == contractId)
                    .FirstOrDefaultAsync();
                var createdEmail = contract.User.EmailAddress;
                var listRecipients = await WorkScope.GetAll<ContractSetting>()
                    .Where(x => x.ContractId == contractId)
                   .ToListAsync();
                var contractDetails = await contractManager.GetContractDetail(contractId);
                var contractSettings = new List<RecipientGetByGuidDto>();

                foreach (var recipient in listRecipients)
                {
                    var currentRecipient = contractDetails.Recipients.Where(x => x.Email == recipient.SignerEmail).FirstOrDefault();
                    var sendingTime = await WorkScope.GetAll<ContractHistory>()
                            .Where(x => x.ContractId == contractId &&
                                        x.Note.Contains($"sent theDocumentTo {recipient.SignerEmail}") &&
                                        x.Action == HistoryAction.SendMail)
                            .Select(x => x.CreationTime)
                            .FirstOrDefaultAsync();
                    var signingTime = await WorkScope.GetAll<ContractHistory>()
                          .Where(x => x.ContractId == contractId &&
                                      x.AuthorEmail == recipient.SignerEmail &&
                                      x.Action == HistoryAction.Sign)
                          .Select(x => x.CreationTime)
                          .FirstOrDefaultAsync();

                    var recipientItem = await WorkScope.GetAll<ContractSetting>()
                   .Where(x => x.ContractId == contractId && x.SignerEmail.ToLower().Trim().Equals(recipient.SignerEmail.ToLower().Trim()))
                   .Select(x => new RecipientGetByGuidDto
                   {
                       Email = x.SignerEmail,
                       Name = x.SignerName,
                       IsComplete = x.IsComplete,
                       Role = x.ContractRole,
                       ProcessOrder = x.ProcesOrder,
                       SendingTime = (contract.Status == ContractStatus.Draft || !recipient.IsSendMail) ? null : (currentRecipient.Role == ContractRole.Viewer ? currentRecipient.UpdateDate : sendingTime),
                       SigningTime = x.IsComplete ? x.UpdateDate : null
                   })
                  .FirstOrDefaultAsync();
                    contractSettings.Add(recipientItem);
                    var cancelHistory = new ContractHistory();

                    if (contract.Status == ContractStatus.Cancelled)
                    {
                        cancelHistory = WorkScope.GetAll<ContractHistory>()
                            .Where(x => x.ContractId == contract.Id &&
                            (x.Action == HistoryAction.CancelContract || x.Action == HistoryAction.VoidToSign)).FirstOrDefault();
                        contractSettings.Where(x => x.Email == cancelHistory.AuthorEmail && cancelHistory.Action == HistoryAction.VoidToSign).ToList().ForEach(x =>
                        {
                            x.IsCanceled = true;
                            x.CancelTime = cancelHistory.TimeAt;
                        });
                    }
                }

                if (contractId != default)
                {
                    return new ContractDetailByGuidDto
                    {
                        ContractName = contractDetails.ContractName,
                        CreatedEmail = createdEmail,
                        ContractId = contractId,
                        Status = contractDetails.Status,
                        CreationTime = contractDetails.CreationTime,
                        UpdatedTime = contractDetails.UpdatedTime,
                        ExpriedTime = contractDetails.ExpriedTime,
                        CreatedUser = contractDetails.CreatedUser,
                        Recipients = contractSettings,
                        ContractGuid = contractDetails.ContractGuid,
                    };
                }
                return null;
            }
        }
    }
}