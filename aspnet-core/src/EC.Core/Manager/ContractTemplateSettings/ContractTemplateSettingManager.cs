using Abp.UI;
using EC.Entities;
using EC.Manager.ContractTemplateSettings.Dto;
using EC.Manager.ContractTemplateSigners.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractTemplateSettings
{
    public class ContractTemplateSettingManager : BaseManager, IContractTemplateSettingManager
    {
        public ContractTemplateSettingManager(IWorkScope workScope) : base(workScope)
        {
        }

        public async Task<long> Create(CreateContractTemplateSettingDto input)
        {
            var contractTemplateId = await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.Id == input.ContractTemplateSignerId)
                .Select(x => x.ContractTemplateId)
                .FirstOrDefaultAsync();
            var entity = ObjectMapper.Map<ContractTemplateSetting>(input);
            bool isInput = input.SignatureType != SignatureTypeSetting.Electronic
                && input.SignatureType != SignatureTypeSetting.Digital
                && input.SignatureType != SignatureTypeSetting.Acronym
                && input.SignatureType != SignatureTypeSetting.Stamp;

            bool isSignature = input.SignatureType == SignatureTypeSetting.Electronic
                    || input.SignatureType == SignatureTypeSetting.Digital
            || input.SignatureType == SignatureTypeSetting.Acronym
            || input.SignatureType == SignatureTypeSetting.Stamp;

            ValidCreate(contractTemplateId, input.ContractTemplateSignerId, isInput, isSignature, false, null);
            var id = await WorkScope.InsertAndGetIdAsync(entity);

            return id;
        }

        public async Task Update(UpdateContractTemplateSettingDto input)
        {
            var contractTemplateId = await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.Id == input.ContractTemplateSignerId)
                .Select(x => x.ContractTemplateId)
                .FirstOrDefaultAsync();
            var item = await WorkScope.GetAsync<ContractTemplateSetting>(input.Id);
            item.Width = input.Width;
            item.IsSigned = input.IsSigned;
            item.Height = input.Height;
            item.PositionX = input.PositionX;
            item.PositionY = input.PositionY;
            item.ContractTemplateSignerId = input.ContractTemplateSignerId;
            item.FontSize = input.FontSize;
            item.FontFamily = input.FontFamily;
            item.FontColor = input.FontColor;
            item.Page = input.Page;
            item.SignatureType = input.SignatureType;
            item.ValueInput = input.ValueInput;
            bool isInput = input.SignatureType != SignatureTypeSetting.Electronic
                && input.SignatureType != SignatureTypeSetting.Digital
                && input.SignatureType != SignatureTypeSetting.Acronym
                && input.SignatureType != SignatureTypeSetting.Stamp;

            bool isSignature = input.SignatureType == SignatureTypeSetting.Electronic
                    || input.SignatureType == SignatureTypeSetting.Digital
            || input.SignatureType == SignatureTypeSetting.Acronym
            || input.SignatureType == SignatureTypeSetting.Stamp;

            ValidCreate(contractTemplateId, input.ContractTemplateSignerId, isInput, isSignature, true, input.Id);
            await WorkScope.UpdateAsync(item);
        }

        public void ValidCreate(long contractId, long contractSettingId, bool isInput, bool isSignature, bool isEdit, long? oldId)
        {
            var settings = WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == contractId)
                .ToList();

            var existSettings = WorkScope.GetAll<ContractTemplateSetting>()
                        .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractId)
                        .Where(x => x.ContractTemplateSignerId != contractSettingId)
                        .GroupBy(x => x.ContractTemplateSignerId)
                        .Select(x => new
                        {
                            x.Key,
                            Settings = x.Select(s => new
                            {
                                s.SignatureType,
                                signerEmail = s.ContractTemplateSigner.SignerEmail,
                                processOrder = s.ContractTemplateSigner.ProcesOrder
                            }).ToList()
                        }).ToDictionary(x => x.Key, x => x.Settings);

            bool anyWithInput = existSettings.Any(x => x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker));
            bool anyWithBoth = existSettings.Count(x => x.Value.Any(s => (s.SignatureType == SignatureTypeSetting.Text || s.SignatureType == SignatureTypeSetting.DatePicker)
            && x.Value.Any(s => s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Digital || s.SignatureType == SignatureTypeSetting.Stamp))) >= 1;

            bool isOrdered = settings.Any(x => x.ProcesOrder > 1);

            if (isOrdered)
            {
                var currentProcessOrder = WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.Id == contractSettingId)
                .Select(x => x.ProcesOrder)
                .FirstOrDefault();

                bool currentHasSignature = WorkScope.GetAll<ContractTemplateSetting>()
                           .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractId)
                           .Where(x => x.ContractTemplateSignerId == contractSettingId)
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
                    bool currentHasSignature = WorkScope.GetAll<ContractTemplateSetting>()
                            .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractId)
                            .Where(x => x.ContractTemplateSignerId == contractSettingId)
                            .Any(x => x.SignatureType == SignatureTypeSetting.Electronic || x.SignatureType == SignatureTypeSetting.Digital || x.SignatureType == SignatureTypeSetting.Stamp);

                    var oldSettingId = WorkScope.GetAll<ContractTemplateSetting>()
                        .Where(x => x.Id == oldId)
                        .Select(x => x.ContractTemplateSignerId)
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
                    bool currentHasInput = WorkScope.GetAll<ContractTemplateSetting>()
                          .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractId)
                          .Where(x => x.ContractTemplateSignerId == contractSettingId)
                          .Any(x => x.SignatureType == SignatureTypeSetting.Text || x.SignatureType == SignatureTypeSetting.DatePicker);

                    var oldSettingId = WorkScope.GetAll<ContractTemplateSetting>()
                      .Where(x => x.Id == oldId)
                      .Select(x => x.ContractTemplateSignerId)
                      .FirstOrDefault();

                    var oldhaveMoreThan2SignatureAnd1Input = existSettings.Where(x => x.Key == oldSettingId).Any(x => x.Value.Count(s => s.SignatureType == SignatureTypeSetting.Digital
                    || s.SignatureType == SignatureTypeSetting.Electronic || s.SignatureType == SignatureTypeSetting.Stamp) > 1
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

        public async Task<UpdateContractTemplateSettingDto> Get(long id)
        {
            return await WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.Id == id)
                .Select(x => new UpdateContractTemplateSettingDto
                {
                    Id = x.Id,
                    Width = x.Width,
                    Height = x.Height,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    IsSigned = x.IsSigned,
                    ContractTemplateSignerId = x.ContractTemplateSignerId,
                    FontSize = x.FontSize,
                    FontFamily = x.FontFamily,
                    FontColor = x.FontColor,
                    Page = x.Page,
                    SignatureType = x.SignatureType,
                    ValueInput = x.ValueInput,
                }).FirstOrDefaultAsync();
        }

        public async Task Delete(long id)
        {
            await WorkScope.DeleteAsync<ContractTemplateSetting>(id);
        }

        public async Task<List<GetAllSignerLocationDto>> GetAllSignLocation(long contractTemplateId)
        {
            return WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractTemplateId)
                .Select(x => new
                {
                    Signer = new GetContractTemplateSignerDto
                    {
                        Id = x.ContractTemplateSigner.Id,
                        Role = x.ContractTemplateSigner.SignerEmail,
                        SignerName = x.ContractTemplateSigner.SignerName,
                        SignerEmail = x.ContractTemplateSigner.SignerEmail,
                        ContractRole = x.ContractTemplateSigner.ContractRole,
                        ProcesOrder = x.ContractTemplateSigner.ProcesOrder,
                        Color = x.ContractTemplateSigner.Color,
                        ContractTemplateId = x.ContractTemplateSigner.ContractTemplateId
                    },
                    Setting = new GetContractTemplateSettingDto
                    {
                        Id = x.Id,
                        IsSigned = x.IsSigned,
                        Width = x.Width,
                        Height = x.Height,
                        PositionX = x.PositionX,
                        PositionY = x.PositionY,
                        ContractTemplateSignerId = x.ContractTemplateSignerId,
                        FontSize = x.FontSize,
                        FontFamily = x.FontFamily,
                        FontColor = x.FontColor,
                        Page = x.Page,
                        SignatureType = x.SignatureType,
                        SignerName = x.ContractTemplateSigner.SignerName,
                        SignerEmail = x.ContractTemplateSigner.SignerEmail
                    }
                }).GroupBy(x => x.Signer).AsEnumerable()
                .Select(x => new GetAllSignerLocationDto
                {
                    Signer = x.Key,
                    Settings = x.Select(y => y.Setting).ToList(),
                }).ToList();
        }
    }
}