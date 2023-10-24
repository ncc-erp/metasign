using Abp.Domain.Uow;
using Abp.UI;
using EC.Authorization.Users;
using EC.Entities;
using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using EC.Manager.ContractSignings.Dto;
using EC.Manager.FileStoring;
using EC.Manager.Public.Dto;
using EC.Utils;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Public
{
    public class PublicManager : BaseManager
    {
        private readonly FileStoringManager _fileStoringManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ContractHistoryManager _contractHistoryManager;

        public PublicManager(IWorkScope workScope,
            FileStoringManager fileStoringManager,
            IWebHostEnvironment webHostEnvironment,
            ContractHistoryManager contractHistoryManager) : base(workScope)
        {
            _fileStoringManager = fileStoringManager;
            _webHostEnvironment = webHostEnvironment;
            _contractHistoryManager = contractHistoryManager;
        }

        public async Task<CreatePublicContractDto> CreateContract(string apiKey, CreatePublicContractDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var userAccess = await WorkScope.GetAll<ApiKey>()
                .Where(x => x.Value.ToLower().Trim() == apiKey.ToLower().Trim())
                .FirstOrDefaultAsync();

                if (userAccess == default)
                {
                    throw new UserFriendlyException("Api key not valid!");
                }

                if (string.IsNullOrEmpty(input.FileBase64))
                {
                    throw new UserFriendlyException("File not valid");
                }

                if (!input.FileBase64.Contains(","))
                {
                    input.FileBase64 += "data:application/pdf;base64,";
                }

                if (!input.FileName.Contains("."))
                {
                    input.FileName += ".pdf";
                }

                var loginUserId = userAccess.UserId;

                var loginUserEmail = await WorkScope.GetAll<User>()
                    .Where(x => x.Id == loginUserId)
                    .Select(x => x.EmailAddress)
                    .FirstOrDefaultAsync();

                var guid = Guid.NewGuid();
                var entity = new Contract()
                {
                    Status = ContractStatus.Draft,
                    UserId = loginUserId,
                    ExpriredTime = input.ExpriedTime,
                    ContractGuid = guid,
                    Code = input.Code,
                    File = input.FileName,
                    Name = input.Name,
                };

                entity.Id = await WorkScope.InsertAndGetIdAsync(entity);

                var location = new SignPositionDto
                {
                    PositionX = 20,
                    PositionY = 10,
                    Page = 1
                };
                var fillInput = new FillInputDto
                {
                    Color = "black",
                    Content = $"MetaSign Document ID: {guid.ToString().ToUpper()}",
                    FontSize = 10,
                    PageHeight = 0,
                    SignerSignatureSettingId = 1,
                    IsCreateContract = true
                };
                var base64 = await SignUtils.FillPdfWithText(fillInput, location, input.FileBase64, _webHostEnvironment.WebRootPath);
                var file = CommonUtils.ConvertBase64PdfToFile(base64.Split(",")[1], entity.File);
                await _fileStoringManager.UploadUnsignedContract(entity.Id, file);

                var history = new CreaContractHistoryDto
                {
                    Action = HistoryAction.CreateContract,
                    AuthorEmail = loginUserEmail,
                    ContractId = entity.Id,
                    ContractStatus = ContractStatus.Draft,
                    TimeAt = DateTimeUtils.GetNow(),
                    Note = $"{loginUserEmail} createdTheDocument"
                };
                await _contractHistoryManager.Create(history);
            }

            return input;
        }
    }
}
