using Abp.UI;
using EC.Authorization.Users;
using EC.Entities;
using EC.Manager.Contracts;
using EC.Manager.SignerSignatureSettings.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.SignerSignatureSettings
{
    public class SignerSignatureSettingManager : BaseManager, ISignerSignatureSettingManager
    {
        protected readonly ContractManager _contractManager;

        public SignerSignatureSettingManager(IWorkScope workScope, ContractManager contractManager) : base(workScope)
        {
            _contractManager = contractManager;
        }

        public async Task<long> Create(CreateSignerSignatureSettingDto input)
        {
            var contractId = await WorkScope.GetAll<ContractSetting>()
            .Where(x => x.Id == input.ContractSettingId)
            .Select(x => x.ContractId)
            .FirstOrDefaultAsync();

            bool isInput = input.SignatureType != SignatureTypeSetting.Electronic
                && input.SignatureType != SignatureTypeSetting.Digital
                && input.SignatureType != SignatureTypeSetting.Acronym
                && input.SignatureType != SignatureTypeSetting.Stamp;

            bool isSignature = input.SignatureType == SignatureTypeSetting.Electronic
                    || input.SignatureType == SignatureTypeSetting.Digital
                    || input.SignatureType == SignatureTypeSetting.Acronym
                    || input.SignatureType == SignatureTypeSetting.Stamp;

            ValidCreate(contractId, input.ContractSettingId, isInput, isSignature, false, null);

            var entity = ObjectMapper.Map<SignerSignatureSetting>(input);

            var id = await WorkScope.InsertAndGetIdAsync(entity);

            await _contractManager.SaveDraft(contractId);

            return id;
        }

        public void ValidCreate(long contractId, long contractSettingId, bool isInput, bool isSignature, bool isEdit, long? oldId)
        {
            var settings = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .ToList();

            var existSettings = WorkScope.GetAll<SignerSignatureSetting>()
                        .Where(x => x.ContractSetting.ContractId == contractId)
                        .Where(x => x.ContractSettingId != contractSettingId)
                        .GroupBy(x => x.ContractSettingId)
                        .Select(x => new
                        {
                            x.Key,
                            Settings = x.Select(s => new
                            {
                                s.SignatureType,
                                signerEmail = s.ContractSetting.SignerEmail,
                                processOrder = s.ContractSetting.ProcesOrder
                            }).ToList()
                        }).ToDictionary(x => x.Key, x => x.Settings);

            bool anyWithInput = existSettings.Any(x => x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker));
            bool anyWithBoth = existSettings.Count(x => x.Value.Any(s => (s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker)
            && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp))) >= 1;

            bool isOrdered = settings.Any(x => x.ProcesOrder > 1);

            if (isOrdered)
            {
                var currentProcessOrder = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == contractSettingId)
                .Select(x => x.ProcesOrder)
                .FirstOrDefault();

                bool currentHasSignature = WorkScope.GetAll<SignerSignatureSetting>()
                           .Where(x => x.ContractSetting.ContractId == contractId)
                           .Where(x => x.ContractSettingId == contractSettingId)
                           .Any(x => x.SignatureType == SignatureTypeSetting.Electronic || x.SignatureType == SignatureTypeSetting.Digital || x.SignatureType == SignatureTypeSetting.Stamp);

                if (currentProcessOrder == 1 && anyWithInput && isSignature)
                {
                    throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                }

                if (currentProcessOrder == 1 && currentHasSignature && isInput && anyWithBoth)
                {
                    throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                }

                if (currentProcessOrder > 1)
                {
                    var prevSettinghasSignature = existSettings.Where(x => x.Value.All(s => s.processOrder < currentProcessOrder))
                     .Any(x => x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp));

                    var nextSettinghasInput = existSettings.Where(x => x.Value.All(s => s.processOrder > currentProcessOrder))
                        .Any(x => x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker));

                    if (prevSettinghasSignature && isInput)
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }

                    // next has input -> can't add signature
                    if (nextSettinghasInput && isSignature)
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }
                }
            }
            if (!isOrdered)
            {
                if (isInput)
                {
                    bool currentHasSignature = WorkScope.GetAll<SignerSignatureSetting>()
                            .Where(x => x.ContractSetting.ContractId == contractId)
                            .Where(x => x.ContractSettingId == contractSettingId)
                            .Any(x => x.SignatureType == SignatureTypeSetting.Electronic || x.SignatureType == SignatureTypeSetting.Digital || x.SignatureType == SignatureTypeSetting.Stamp);

                    var oldSettingId = WorkScope.GetAll<SignerSignatureSetting>()
                        .Where(x => x.Id == oldId)
                        .Select(x => x.ContractSettingId)
                        .FirstOrDefault();

                    var oldhaveMoreThan2InputAnd1Sig = existSettings.Where(x => x.Key == oldSettingId).Any(x => x.Value.Count(s => s.SignatureType == SignatureTypeSetting.Text
                    || s.SignatureType == SignatureTypeSetting.DatePicker) > 1
                    && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp));

                    var otherSettingHasBoth = existSettings
                     .Where(x => x.Key != oldSettingId && x.Key != contractSettingId)
                     .Any(x => x.Value.Any(s => (s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker)
                     && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp)));

                    if ((isEdit && otherSettingHasBoth && currentHasSignature) || (oldhaveMoreThan2InputAnd1Sig && currentHasSignature))
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }

                    if (!isEdit && anyWithBoth && currentHasSignature)
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }
                }

                if (isSignature)
                {
                    bool currentHasInput = WorkScope.GetAll<SignerSignatureSetting>()
                          .Where(x => x.ContractSetting.ContractId == contractId)
                          .Where(x => x.ContractSettingId == contractSettingId)
                          .Any(x => x.SignatureType == SignatureTypeSetting.Text || x.SignatureType == SignatureTypeSetting.DatePicker);

                    var oldSettingId = WorkScope.GetAll<SignerSignatureSetting>()
                      .Where(x => x.Id == oldId)
                      .Select(x => x.ContractSettingId)
                      .FirstOrDefault();

                    var oldhaveMoreThan2SignatureAnd1Input = existSettings.Where(x => x.Key == oldSettingId).Any(x => x.Value.Count(s => s.SignatureType == SignatureTypeSetting.Digital
                    || s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Stamp ) > 1
                    && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker));

                    var otherSettingHasBoth = existSettings
                        .Where(x => x.Key != oldSettingId && x.Key != contractSettingId)
                        .Any(x => x.Value.Any(s => (s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker)
                         && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp)));

                    if (isEdit && otherSettingHasBoth && currentHasInput || oldhaveMoreThan2SignatureAnd1Input && currentHasInput)
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }
                    if (!isEdit && anyWithBoth && currentHasInput)
                    {
                        throw new UserFriendlyException($"OnlyThePersonWithTheFirstOrderOfSigningCanAddContractInformation");
                    }
                }
            }
        }

        public async Task<UpdateSignerSignatureSetting> Update(UpdateSignerSignatureSetting input)
        {
            var contractId = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == input.ContractSettingId)
                .Select(x => x.ContractId)
                .FirstOrDefaultAsync();

            bool isInput = input.SignatureType != SignatureTypeSetting.Electronic
                && input.SignatureType != SignatureTypeSetting.Digital
                && input.SignatureType != SignatureTypeSetting.Acronym
                && input.SignatureType != SignatureTypeSetting.Stamp;

            bool isSignature = input.SignatureType == SignatureTypeSetting.Electronic
                    || input.SignatureType == SignatureTypeSetting.Digital
                    || input.SignatureType == SignatureTypeSetting.Acronym
                    || input.SignatureType == SignatureTypeSetting.Stamp;

            ValidCreate(contractId, input.ContractSettingId, isInput, isSignature, true, input.Id);

            var entity = ObjectMapper.Map<SignerSignatureSetting>(input);

            await WorkScope.UpdateAsync(entity);

            await _contractManager.SaveDraft(contractId);

            return input;
        }

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<SignerSignatureSetting>(id);

            return id;
        }

        public GetSignerSignatureSettingDto Get(long id)
        {
            return QueryAllSignerSignatureSetting()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public List<GetSignerSignatureSettingDto> GetAll()
        {
            return QueryAllSignerSignatureSetting().ToList();
        }

        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSetting(long contractSettingId, string email)
        {
            var contract = await WorkScope.GetAll<ContractSetting>()
              .Where(x => x.Id == contractSettingId)
              .Select(x => new
              {
                  contractId = x.ContractId,
                  contractName = x.Contract.Name,
                  contractBase64 = x.Contract.FileBase64,
                  contractStatus = x.Contract.Status,
                  contractRole = x.ContractRole,
                  signerEmail = x.SignerEmail,
                  iscomplete = x.IsComplete,
                  Color = x.Color
              })
              .FirstOrDefaultAsync();

            var isCreator = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contract.contractId)
                .Any(x => x.User.EmailAddress.ToLower().Trim() == email.ToLower().Trim());

            if (contract.signerEmail.ToLower().Trim() != email.ToLower().Trim())
            {
                if (isCreator && contract.contractStatus != ContractStatus.Inprogress) { }
                if (isCreator && contract.contractStatus == ContractStatus.Inprogress)
                {
                    throw new UserFriendlyException($"Email hiện tại không có quyền truy cập tài liệu, hãy đăng nhập lại");
                }
                if (!isCreator)
                {
                    throw new UserFriendlyException($"Email hiện tại không có quyền truy cập tài liệu, hãy đăng nhập lại");
                }
            }

            var currentContractResult = WorkScope.GetAll<ContractSigning>()
                .Where(x => x.ContractId == contract.contractId)
                .OrderByDescending(x => x.TimeAt)
                .Select(x => x.SigningResult)
                .FirstOrDefault();

            var signatureSettings = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contract.contractId)
                .OrderBy(x => x.Page)
                .ThenBy(x => x.PositionY)
                .Select(x => new GetSignerSignatureSettingDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    SignerName = x.ContractSetting.SignerName,
                    SignerEmail = x.ContractSetting.SignerEmail,
                    Color = x.ContractSetting.Color,
                    ContractSettingId = x.ContractSettingId,
                    SignatureTypeId = x.SignatureTypeId,
                    Height = x.Height,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    Width = x.Width,
                    Page = x.Page,
                    IsSigned = x.IsSigned,
                    IsAllowSigning = x.ContractSettingId == contractSettingId,
                    FontSize = x.FontSize,
                    FontFamily = x.FontFamily,
                    FontColor = x.FontColor,
                    ValueInput = x.ValueInput,
                })
                .ToListAsync();

            var isLoggedIn = WorkScope.GetAll<User>()
                .Any(x => x.EmailAddress.ToLower() == contract.signerEmail.ToLower());

            var defaultSignature = WorkScope.GetAll<SignatureUser>()
                .Where(x => x.User.EmailAddress.ToLower().Trim() == contract.signerEmail)
                .Where(x => x.IsDefault)
                .Select(x =>
                    new GetContractSignatureDefaultDto
                    {
                        ContractBase64 = x.FileBase64,
                        SignatureType = x.SignatureType
                    }

                )
                .FirstOrDefault();
            return new GetContractSignerSignatureSettingDto
            {
                ContractId = contract.contractId,
                ContractName = contract.contractName,
                Status = contract.contractStatus,
                Role = contract.contractRole,
                IsLoggedIn = isLoggedIn,
                IsComplete = contract.contractRole == ContractRole.Viewer ? contract.contractStatus == ContractStatus.Complete : contract.iscomplete,
                ContractBase64 = currentContractResult != default ? currentContractResult : contract.contractBase64,
                SignatureSettings = signatureSettings,
                SignatureDefault = defaultSignature != default ? defaultSignature : null,
                IsCreator = isCreator,
            };
        }

        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSettingForContractDesign(long contractId)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.FileBase64,
                    x.Status
                }).FirstOrDefaultAsync();

            if (contract == default)
            {
                throw new UserFriendlyException("Không tìm thấy hợp đồng");
            }

            var signatureSettings = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contract.Id)
                .Select(x => new GetSignerSignatureSettingDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    SignerName = x.ContractSetting.SignerName,
                    SignerEmail = x.ContractSetting.SignerEmail,
                    Color = x.ContractSetting.Color,
                    ContractSettingId = x.ContractSettingId,
                    SignatureTypeId = x.SignatureTypeId,
                    Height = x.Height,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    Width = x.Width,
                    Page = x.Page,
                    FontSize = x.FontSize,
                    FontFamily = x.FontFamily,
                    FontColor = x.FontColor,
                    ValueInput = x.ValueInput
                })
                .ToListAsync();

            return new GetContractSignerSignatureSettingDto
            {
                ContractId = contract.Id,
                ContractName = contract.Name,
                ContractBase64 = contract.FileBase64,
                SignatureSettings = signatureSettings,
            };
        }

        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSettingByContractId(long contractId)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .Select(x => new
                {
                    contractId = x.Id,
                    contractBase64 = x.FileBase64,
                })
                .FirstOrDefaultAsync();

            var dicSettingColor = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .ToDictionaryAsync(x => x.Id, x => x.Color);

            var signatureSettings = WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .ToList()
                .Select(x => new GetSignerSignatureSettingDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    ContractSettingId = x.ContractSettingId,
                    SignatureTypeId = x.SignatureTypeId,
                    Height = x.Height,
                    IsSigned = x.IsSigned,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    Width = x.Width,
                    SignerName = x.ContractSetting.SignerName,
                    Color = dicSettingColor.ContainsKey(x.ContractSettingId) ? dicSettingColor[x.ContractSettingId] : null,
                    ValueInput = x.ValueInput,
                }).ToList();

            return new GetContractSignerSignatureSettingDto
            {
                ContractId = contract.contractId,
                ContractBase64 = contract.contractBase64,
                SignatureSettings = signatureSettings
            };
        }

        public IQueryable<GetSignerSignatureSettingDto> QueryAllSignerSignatureSetting()
        {
            return WorkScope.GetAll<SignerSignatureSetting>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetSignerSignatureSettingDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    SignerName = x.ContractSetting.SignerName,
                    ContractSettingId = x.ContractSettingId,
                    SignatureTypeId = x.SignatureTypeId,
                    Height = x.Height,
                    IsSigned = x.IsSigned,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    Width = x.Width,
                    Page = x.Page
                });
        }
    }
}