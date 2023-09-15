using Abp.Collections.Extensions;
using EC.Entities;
using EC.Manager.Contracts.Dto;
using EC.Manager.ContractTemplates.Dto;
using EC.Manager.ContractTemplateSettings.Dto;
using EC.Manager.ContractTemplateSigners.Dto;
using EC.Utils;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractTemplates
{
    public class ContractTemplateManager : BaseManager, IContractTemplateManager
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string tempConvertFolder = Path.Combine("tempConvert");

        public ContractTemplateManager(IWorkScope workScope, IWebHostEnvironment webHostEnvironment) : base(workScope)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<long> Create(CreateContractTemplateDto input)
        {
            var entity = ObjectMapper.Map<ContractTemplate>(input);
            if (!string.IsNullOrEmpty(input.HtmlContent))
            {
                entity.Content = "data:application/pdf;base64," + CommonUtils.ConvertHtmlToPdf(input.HtmlContent);
            }

            var templateId = await WorkScope.InsertAndGetIdAsync(entity);

            return templateId;
        }

        public async Task<GetSignatureForContracttemplateDto> Get(long id)
        {
            var item = WorkScope.GetAll<ContractTemplate>()
                .Where(x => x.Id == id).FirstOrDefault();
            var contractTemplate = new GetContractTemplateDto
            {
                Id = item.Id,
                Name = item.Name,
                FileName = item.FileName,
                Content = item.Content,
                HtmlContent = item.HtmlContent,
                IsFavorite = item.IsFavorite,
                Type = item.Type,
                UserId = item.UserId,
                CreationTime = item.CreationTime,
                LastModifycationTime = item.LastModificationTime,
                MassType = item.MassType
            };
            var settings = WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == id)
                .Select(x => new GetContractTemplateSettingDto
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
                    SignerEmail = x.ContractTemplateSigner.SignerEmail,
                    SignerName = x.ContractTemplateSigner.SignerName,
                    Color = x.ContractTemplateSigner.Color,
                    ValueInput = x.ValueInput
                }).ToList();
            var signers = WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == id)
                .Select(x => new GetContractTemplateSignerDto
                {
                    Id = x.Id,
                    Role = x.Role,
                    SignerName = x.SignerName,
                    SignerEmail = x.SignerEmail,
                    ContractRole = x.ContractRole,
                    ProcesOrder = x.ProcesOrder,
                    Color = x.Color,
                    ContractTemplateId = x.ContractTemplateId
                }).ToList();
            return new GetSignatureForContracttemplateDto
            {
                ContractTemplate = contractTemplate,
                SignatureSettings = settings,
                SignerSettings = signers
            };
        }

        public async Task<List<GetContractTemplateDto>> GetAll(ContractTemplateFilterType? input)
        {
            return WorkScope.GetAll<ContractTemplate>()
                .OrderBy(x => x.CreationTime)
                .Select(x => new GetContractTemplateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    FileName = x.FileName,
                    Content = x.Content,
                    HtmlContent = x.HtmlContent,
                    UserId = x.UserId,
                    Type = x.Type,
                    IsFavorite = x.IsFavorite,
                    CreationTime = x.CreationTime,
                    CreationUserName = x.CreatorUser.FullName,
                    LastModifycationTime = x.LastModificationTime,
                    LastModifyCationUserName = x.LastModifierUser.FullName,
                    MassType = x.MassType
                })
                .WhereIf(input.HasValue && input.Value == ContractTemplateFilterType.Me, x => x.UserId == AbpSession.UserId)
                .WhereIf(input.HasValue && input.Value == ContractTemplateFilterType.System, x => x.UserId == null)
                .ToList();
        }

        public async Task<GridResult<GetContractTemplateDto>> GetAllPaging(GetContractTemplateByFilterDto input)
        {
            var loginUserId = AbpSession.UserId;
            var query = WorkScope.GetAll<ContractTemplate>()
                .OrderByDescending(x => x.CreationTime)
                .Where(x => x.UserId == loginUserId || x.UserId == null)
                .Select(x => new GetContractTemplateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    FileName = x.FileName,
                    Content = x.Content,
                    HtmlContent = x.HtmlContent,
                    IsFavorite = x.IsFavorite,
                    Type = x.Type,
                    UserId = x.UserId,
                    CreationTime = x.CreationTime,
                    CreationUserName = x.CreatorUser.FullName,
                    LastModifycationTime = x.LastModificationTime,
                    LastModifyCationUserName = x.LastModifierUser.FullName,
                    MassType = x.MassType
                });
            switch (input.FilterType)
            {
                case ContractTemplateFilterType.Me:
                    {
                        query = query.Where(x => x.UserId == loginUserId);
                        break;
                    }
                case ContractTemplateFilterType.System:
                    {
                        query = query.Where(x => x.UserId == null);
                        break;
                    }
            }
            return await query.GetGridResult(query, input.GridParam);
        }

        public async Task Update(GetContractTemplateDto input)
        {
            var item = await WorkScope.GetAsync<ContractTemplate>(input.Id);

            if (input.Content != item.Content)
            {
                if (!string.IsNullOrEmpty(input.HtmlContent) && input.HtmlContent != item.HtmlContent)
                {
                    input.Content = "data:application/pdf;base64," + CommonUtils.ConvertHtmlToPdf(input.HtmlContent);
                }
                else
                {
                    item.Content = input.Content;
                }

                var settings = WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == item.Id).ToList();
                settings.ForEach(x =>
                {
                    x.IsDeleted = true;
                });
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            item.Name = input.Name;
            item.FileName = !string.IsNullOrEmpty(input.HtmlContent) ? input.Name + ".pdf" : input.FileName;
            item.HtmlContent = input.HtmlContent;
            item.Type = input.Type;
            item.IsFavorite = input.IsFavorite;
            item.UserId = input.UserId;
            await WorkScope.UpdateAsync(item);
        }

        public async Task Delete(long id)
        {
            var signer = WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == id).ToList();
            var settings = WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == id).ToList();
            settings.ForEach(x =>
            {
                x.IsDeleted = true;
            });
            signer.ForEach(x =>
            {
                x.IsDeleted = true;
            });
            await CurrentUnitOfWork.SaveChangesAsync();
            await WorkScope.DeleteAsync<ContractTemplate>(id);
        }

        private OrderSignerDto OrderSigner(List<ContractTemplateSetting> input)
        {
            var grSignerSignatureSetting = input.GroupBy(x => x.ContractTemplateSignerId)
                .Select(x => new
                {
                    ContractTemplateSignerId = x.Key,
                    Signature = x.Select(y => y.SignatureType).ToList()
                });
            var contractSettingIdHaveInput = grSignerSignatureSetting.Where(x => x.Signature.Intersect(CommonUtils.InputSignature).Any() && !x.Signature.Intersect(CommonUtils.SigningSignature).Any()).OrderByDescending(x => x.ContractTemplateSignerId).Select(x => x.ContractTemplateSignerId).ToList();
            var normalSignerId = grSignerSignatureSetting.Where(x => !x.Signature.Intersect(CommonUtils.InputSignature).Any() && x.Signature.Intersect(CommonUtils.SigningSignature).Any()).OrderByDescending(x => x.ContractTemplateSignerId).Select(x => x.ContractTemplateSignerId).ToList();
            var contractSettingIdHaveBoth = grSignerSignatureSetting.Where(x => !contractSettingIdHaveInput.Contains(x.ContractTemplateSignerId) && !normalSignerId.Contains(x.ContractTemplateSignerId)).OrderByDescending(x => x.ContractTemplateSignerId).Select(x => x.ContractTemplateSignerId).ToList();

            return new OrderSignerDto
            {
                ContractSettingIdHaveInput = contractSettingIdHaveInput,
                ContractSettingIdHaveBoth = contractSettingIdHaveBoth,
                NormalSignerId = normalSignerId,
            };
        }

        public async Task UpdateProcessOrder(long contractTemplateId)
        {
            var qSignatureSettings = await WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractTemplateId)
                .ToListAsync();
            var hasInput = qSignatureSettings.Any(x => CommonUtils.InputSignature.Contains(x.SignatureType));
            if (hasInput)
            {
                var Signers = OrderSigner(qSignatureSettings);
                var contractSettingIds = new List<long>();
                contractSettingIds.AddRange(Signers.ContractSettingIdHaveInput);
                contractSettingIds.AddRange(Signers.ContractSettingIdHaveBoth);
                contractSettingIds.AddRange(Signers.NormalSignerId);
                var updateSettings = await WorkScope.GetAll<ContractTemplateSigner>()
                    .Where(x => contractSettingIds.Contains(x.Id)).ToListAsync();
                var hasOrder = updateSettings.Any(x => x.ProcesOrder != 1);
                if (!hasOrder)
                {
                    for (int i = 0; i < contractSettingIds.Count; i++)
                    {
                        updateSettings.Where(x => x.Id == contractSettingIds[i]).FirstOrDefault().ProcesOrder += i;
                    }
                    await WorkScope.UpdateRangeAsync(updateSettings);
                }
            }
        }

        public async Task<object> CheckHasInput(long contractTemplateId)
        {
            var hasInput = await WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractTemplateId)
                .AnyAsync(x => CommonUtils.InputSignature.Contains(x.SignatureType));
            var hasSign = await WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractTemplateId)
                .AnyAsync(x => CommonUtils.InputSignature.Contains(x.SignatureType));
            return new
            {
                HasInput = hasInput,
                hasSign = hasSign
            };
        }

        public async Task RemoveAllSignature(long contractTemplateId)
        {
            var allSignature = await WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSigner.ContractTemplateId == contractTemplateId)
                .ToListAsync();
            allSignature.ForEach(x =>
            {
                x.IsDeleted = true;
            });
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}