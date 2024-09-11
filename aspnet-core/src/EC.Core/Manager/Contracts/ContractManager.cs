using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Json;
using Abp.Logging;
using Abp.Timing;
using Abp.UI;
using EC.Authorization.Users;
using EC.BackgroundJobs.CancelExpiredContract;
using EC.BackgroundJobs.SendMail;
using EC.Configuration;
using EC.Entities;
using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using EC.Manager.Contracts.Dto;
using EC.Manager.ContractSignings.Dto;
using EC.Manager.FileStoring;
using EC.Manager.Notifications.Email;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Notification;
using EC.Manager.SignerSignatureSettings.Dto;
using EC.Manager.SignServerWorkers;
using EC.MultiTenancy;
using EC.Utils;
using EC.WebService.PDFConverter;
using HRMv2.NccCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Extension;
using NccCore.Extensions;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static EC.Constants.Enum;
using Contract = EC.Entities.Contract;
using Document = Spire.Doc.Document;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace EC.Manager.Contracts
{
    public class ContractManager : BaseManager, IContractManager
    {
        private readonly IConfiguration _appConfiguration;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly ContractHistoryManager _contractHistoryManager;
        private readonly EmailManager _emailManager;
        private readonly FileStoringManager _fileStoringManager;
        private readonly NotificationManager _notificationManager;
        private readonly PDFConverterWebService _pDFConverterWebService;
        private readonly TenantManager _tenantManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignServerWorkerManager _workerManager;
        private readonly string tempConvertFolder = Path.Combine("tempConvert");

        public ContractManager(IWorkScope workScope,
            SignServerWorkerManager signServerWorkerManager,
            BackgroundJobManager backgroundJobManager,
            EmailManager emailManager,
            ContractHistoryManager contractHistoryManager,
             IConfiguration configuration,
             IWebHostEnvironment webHostEnvironment,
             TenantManager tenantManager,
             NotificationManager notificationManager,
             PDFConverterWebService pDFConverterWebService,
             FileStoringManager fileStoringManager) : base(workScope)
        {
            _workerManager = signServerWorkerManager;
            _appConfiguration = configuration;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
            _contractHistoryManager = contractHistoryManager;
            _webHostEnvironment = webHostEnvironment;
            _tenantManager = tenantManager;
            _notificationManager = notificationManager;
            _pDFConverterWebService = pDFConverterWebService;
            _fileStoringManager = fileStoringManager;
        }

        public async Task<long> CancelContract(CancelContractDto input)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == input.ContractId)
                .FirstOrDefaultAsync();
            if (contract.Status == ContractStatus.Cancelled)
            {
                throw new UserFriendlyException("This contract had been Canceled!");
            }

            contract.Status = ContractStatus.Cancelled;

            await WorkScope.UpdateAsync(contract);
            string loginUserEmail = await WorkScope.GetAll<User>()
            .Where(x => x.Id == AbpSession.UserId)
            .Select(x => x.EmailAddress)
            .FirstOrDefaultAsync();

            var history = new CreaContractHistoryDto
            {
                Action = HistoryAction.CancelContract,
                AuthorEmail = loginUserEmail,
                ContractId = input.ContractId,
                ContractStatus = ContractStatus.Cancelled,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"{loginUserEmail} cancelledtheDocument {input.Reason}]"
            };
            await _contractHistoryManager.Create(history);
            await _notificationManager.RemoveOldJob(contract.Id);
            await _notificationManager.NotifyCancelContract(input.ContractId, history.TimeAt, history.AuthorEmail);

            return input.ContractId;
        }

        public async Task<bool> CheckContractHasSigned(long contractId)
        {
            return (WorkScope.GetAll<ContractSigning>()
                    .Where(s => s.ContractId == contractId && !string.IsNullOrEmpty(s.SignartureBase64))
                    .ToList()).Any();
        }

        public async Task<object> CheckHasInput(long contractId)
        {
            var hasInput = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .AnyAsync(x => CommonUtils.InputSignature.Contains(x.SignatureType));
            var hasSign = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .AnyAsync(x => CommonUtils.SigningSignature.Contains(x.SignatureType));
            return new
            {
                HasInput = hasInput,
                HasSign = hasSign
            };
        }

        public async Task<string> CompressPdfFilesToZip(List<FileDto> listFile)
        {
            using (MemoryStream zipMemoryStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in listFile)
                    {
                        byte[] pdfBytes = Convert.FromBase64String(file.FileBase64);

                        using (MemoryStream pdfMemoryStream = new MemoryStream(pdfBytes))
                        {
                            ZipArchiveEntry entry = zipArchive.CreateEntry(file.FileName, System.IO.Compression.CompressionLevel.Optimal);
                            using (Stream entryStream = entry.Open())
                            {
                                await pdfMemoryStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }

                byte[] zipBytes = zipMemoryStream.ToArray();

                string zipBase64 = Convert.ToBase64String(zipBytes);

                return "data:application/zip;base64," + zipBase64;
            }
        }

        public async Task<string> ConvertHtmltoPdf(string html)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder, "output.html");
            File.WriteAllText(filePath, html);
            var outputPdfPath = Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder);
            await ConvertToPdf(filePath, Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder));
            string pdfFilePath = Path.Combine(outputPdfPath, Path.GetFileNameWithoutExtension(filePath) + ".pdf");
            Logger.Log(LogSeverity.Debug, $"PDF Location: {pdfFilePath}");
            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            File.Delete(filePath);
            File.Delete(pdfFilePath);
            return "data:application/pdf;base64," + pdfBase64;
        }

        public async Task<long> Create(CreatECDto input)
        {
            var loginUserId = AbpSession.UserId.Value;

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
                File = input.File,
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

            return entity.Id;
        }

        public async Task<long> CreateContractFromTemplate(CreateContractFromTemplateDto input)
        {
            var loginUserId = AbpSession.UserId.Value;
            var template = await WorkScope.GetAsync<ContractTemplate>(input.ContractTemplateId);

            var loginUserEmail = await WorkScope.GetAll<User>()
                .Where(x => x.Id == loginUserId)
                .Select(x => x.EmailAddress)
                .FirstOrDefaultAsync();

            var guid = Guid.NewGuid();

            var entity = new Contract
            {
                Name = input.Name,
                Code = input.Code,
                Status = ContractStatus.Draft,
                UserId = loginUserId,
                File = template.Name + ".pdf",
                ContractTemplateId = input.ContractTemplateId,
                ExpriredTime = input.ExpriedTime,
                ContractGuid = guid,
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
            var base64 = await SignUtils.FillPdfWithText(fillInput, location, template.Content, _webHostEnvironment.WebRootPath);
            var file = CommonUtils.ConvertBase64PdfToFile(base64.Split(",")[1], entity.File);
            await _fileStoringManager.UploadUnsignedContract(entity.Id, file);

            var history = new CreaContractHistoryDto
            {
                Action = HistoryAction.CreateContract,
                AuthorEmail = loginUserEmail,
                ContractId = entity.Id,
                ContractStatus = ContractStatus.Draft,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"{loginUserEmail} đã tạo tài liệu"
            };
            await _contractHistoryManager.Create(history);

            return entity.Id;
        }

        public async Task CreateMassContract(CreateMassContractDto input)
        {
            var loginUserId = AbpSession.UserId.Value;
            var template = WorkScope.GetAll<ContractTemplate>().Where(x => x.Id == input.Id).FirstOrDefault();

            var loginUserEmail = WorkScope.GetAll<User>()
                .Where(x => x.Id == loginUserId)
                .Select(x => x.EmailAddress)
                .FirstOrDefault();
            var massGuid = Guid.NewGuid();
            var numOfContracts = input.RowData.Count;
            for (int i = 0; i < numOfContracts; i++)
            {
                #region Create Contract

                var entity = new Contract
                {
                    Name = input.RowData[i].Contract.Name,
                    Status = ContractStatus.Draft,
                    UserId = loginUserId,
                    File = input.RowData[i].Contract.Name + ".pdf",
                    ContractTemplateId = template.Id,
                    ContractGuid = Guid.NewGuid(),
                    MassGuid = massGuid,
                    ExpriredTime = input.RowData[i].Contract.ExpriedTime
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
                    Content = $"MetaSign Document ID: {entity.ContractGuid.ToString().ToUpper()}",
                    FontSize = 10,
                    PageHeight = 0,
                    SignerSignatureSettingId = 1,
                    IsCreateContract = true
                };
                var base64 = "";
                if (!string.IsNullOrEmpty(template.MassWordContent))
                {
                    var base64Convert = await FillAndRepaceContent(template.MassWordContent, template.MassField, input.RowData[i].ListFieldDto);
                    base64 = await SignUtils.FillPdfWithText(fillInput, location, base64Convert, _webHostEnvironment.WebRootPath);
                }
                else
                {
                    base64 = await SignUtils.FillPdfWithText(fillInput, location, template.Content, _webHostEnvironment.WebRootPath);
                }
                var file = CommonUtils.ConvertBase64PdfToFile(base64.Split(",")[1], entity.File);
                await _fileStoringManager.UploadUnsignedContract(entity.Id, file);

                #endregion Create Contract

                #region Create Contract History

                var history = new CreaContractHistoryDto
                {
                    Action = HistoryAction.CreateContract,
                    AuthorEmail = loginUserEmail,
                    ContractId = entity.Id,
                    ContractStatus = ContractStatus.Draft,
                    TimeAt = DateTimeUtils.GetNow(),
                    Note = $"{loginUserEmail} đã tạo tài liệu"
                };
                await _contractHistoryManager.Create(history);

                #endregion Create Contract History

                #region Create Signer and SignatureSetting

                foreach (var item1 in input.RowData[i].Signers)
                {
                    var signer = new ContractSetting
                    {
                        ContractId = entity.Id,
                        ContractRole = item1.ContractRole,
                        SignerName = item1.Name,
                        SignerEmail = item1.Email,
                        Status = ContractSettingStatus.NotConfirmed,
                        ProcesOrder = item1.ProcesOrder,
                        Color = item1.Color,
                        SignerMassGuid = item1.MassGuid
                    };
                    signer.Id = WorkScope.InsertAndGetId(signer);
                    var signatureSetting = item1.SignatureSettings.Select(x => new SignerSignatureSetting
                    {
                        ContractSettingId = signer.Id,
                        SignatureType = x.SignatureType,
                        Page = x.Page,
                        PositionX = x.PositionX,
                        PositionY = x.PositionY,
                        Width = x.Width,
                        Height = x.Height,
                        FontSize = x.FontSize,
                        FontFamily = x.FontFamily,
                        FontColor = x.FontColor,
                        ValueInput = x.ValueInput,
                    }).ToList();
                    WorkScope.InsertRange(signatureSetting);
                }

                #endregion Create Signer and SignatureSetting

                #region Send Mail

                var sendMailDto = new SendMailDto
                {
                    ContractId = entity.Id,
                    MailContent = GetContractMailContent(entity.Id)
                };
                await SendMailToViewer(sendMailDto);
                await SendMail(sendMailDto);

                #endregion Send Mail
            }
        }

        public async Task<long> Delete(long id)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            await _fileStoringManager.DeleteUnsignedContract(contract.Id);
            await WorkScope.DeleteAsync(contract);

            return id;
        }

        public async Task<string> DownloadContractAndCertificate(DownloadContractAndCertificateDto input)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == input.ContractId)
                .Select(x => new { x.Id, x.Name, x.File, x.Code, x.UserId, x.Status, x.CreationTime, x.ExpriredTime, x.CreatorUserId, x.FileBase64, x.ContractGuid })
                .FirstOrDefaultAsync();

            var createdUser = await WorkScope.GetAll<User>()
                .Where(x => x.Id == contract.UserId)
                .Select(x => new { x.FullName, x.EmailAddress })
                .FirstOrDefaultAsync();

            var listContractSignarture = await WorkScope.GetAll<ContractSigning>()
                .Where(x => x.ContractId == input.ContractId)
                .OrderBy(x => x.TimeAt)
                .ToListAsync();

            var lastContractSignature = listContractSignarture.LastOrDefault();

            string base64Contract = contract.FileBase64;

            if (listContractSignarture.Count != 0)
            {
                base64Contract = lastContractSignature.SigningResult;
            }

            if (input.DownloadType == DownloadContractType.Contract)
            {
                return base64Contract;
            };

            var signatures = new List<SignartureDto>();

            foreach (var contractSigning in listContractSignarture)
            {
                if (!string.IsNullOrEmpty(contractSigning.SignartureBase64))
                {
                    var signerName = await WorkScope.GetAll<ContractSetting>()
                    .Where(x => x.ContractId == input.ContractId &&
                                x.SignerEmail == contractSigning.Email)
                    .Select(x => x.SignerName)
                    .FirstOrDefaultAsync();

                    var signingTime = await WorkScope.GetAll<ContractHistory>()
                        .Where(x => x.ContractId == input.ContractId &&
                                    x.AuthorEmail == contractSigning.Email &&
                                    x.Action == HistoryAction.Sign)
                        .Select(x => x.CreationTime)
                        .FirstOrDefaultAsync();

                    var sendingTime = await WorkScope.GetAll<ContractHistory>()
                        .Where(x => x.ContractId == input.ContractId &&
                                    x.Note.Contains($"sent theDocumentTo {contractSigning.Email}") &&
                                    x.Action == HistoryAction.SendMail)
                        .Select(x => x.CreationTime)
                        .FirstOrDefaultAsync();

                    var signature = new SignartureDto
                    {
                        Email = contractSigning.Email,
                        SignartureBase64 = contractSigning.SignartureBase64,
                        Name = signerName,
                        SigningTime = signingTime,
                        SendingTime = sendingTime,
                        GuId = contractSigning.Guid,
                        SignatureType = contractSigning.SignatureType
                    };

                    signatures.Add(signature);
                }
            }

            var certificate = new CertificateDto
            {
                ContractId = contract.Id,
                ContractGuId = contract.ContractGuid,
                ContractName = contract.Name,
                FileName = contract.File,
                Code = contract.Code,
                UserId = contract.UserId,
                Status = contract.Status,
                CreationTime = contract.CreationTime,
                CreatorUser = createdUser.FullName,
                CreatorEmail = createdUser.EmailAddress,
                Signatures = signatures,
                ExpriredTime = contract.ExpriredTime,
            };

            string base64Certificate = await RenderCertificatePdf(certificate);

            if (input.DownloadType == DownloadContractType.Certificate)
            {
                return base64Certificate;
            };

            List<FileDto> listFile = new List<FileDto>();

            listFile.Add(new FileDto
            {
                FileName = "Certificate.pdf",
                FileBase64 = base64Certificate.Replace("data:application/pdf;base64,", "")
            });

            listFile.Add(new FileDto
            {
                FileName = contract.File,
                FileBase64 = base64Contract.Replace("data:application/pdf;base64,", "")
            });

            return await CompressPdfFilesToZip(listFile);
        }

        public async Task<FileBase64Dto> DownLoadMassTemplate()
        {
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, "massTemplate", "Mass_Template_Contract.xlsx"))))
            {
                return new FileBase64Dto
                {
                    Base64 = Convert.ToBase64String(package.GetAsByteArray()),
                    FileName = "Mass_Contract_Template.xlsx",
                    FileType = "application/vnd.ms-excel"
                };
            }
        }

        public async Task<GetContractDto> Get(long id)
        {
            var res = await QueryAllContract()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(res.FileBase64))
            {
                res.FileBase64 = await _fileStoringManager.DownloadLatestContractBase64(res.Id);
            }
            return res;
        }

        public List<GetContractDto> GetAll()
        {
            return QueryAllContract().ToList();
        }

        public async Task<GridResult<GetContractDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<Contract>()
                .Where(x => x.UserId == AbpSession.UserId)
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetContractDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    File = x.File,
                    Status = x.Status,
                    CreationTime = x.CreationTime,
                    UserId = x.UserId,
                    ExpriedTime = x.ExpriredTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    UpdatedTime = x.LastModificationTime
                });

            return await query.GetGridResult(query, input);
        }

        public async Task<List<GetSignersDto>> GetAllSigners()
        {
            var userId = AbpSession.UserId;
            var userEmail = WorkScope.GetAsync<User>(userId.Value).Result.EmailAddress;

            var query = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Contract.UserId == userId && x.ContractRole != ContractRole.Viewer)
                .Select(x => new
                {
                    Signer = new
                    {
                        Email = x.SignerEmail,

                        //Name = x.SignerName
                    }
                })
                .ToList()
                .GroupBy(x => x.Signer)
                .Select(x => new GetSignersDto
                {
                    Email = x.Key.Email,

                    //Name = x.Key.Name
                }).ToList();
            query = query.Where(x => x.Email != userEmail).ToList();
            return query;
        }

        public async Task<GridResult<GetContractDto>> GetContractByFilterPaging(GetContractByFilterDto input)
        {
            var userId = AbpSession.UserId;
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var userEmail = WorkScope.GetAll<User>()
                .Where(x => x.Id == userId)
                .Select(x => x.EmailAddress)
                .FirstOrDefault();

            var contractSettings = WorkScope.GetAll<ContractSetting>()
                .ToList();

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

            var loginEmail = await WorkScope.GetAll<User>()
              .Where(x => x.Id == AbpSession.UserId)
              .Select(x => x.EmailAddress)
              .FirstOrDefaultAsync();

            var contractIds = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.SignerEmail == loginEmail)

                //.Where(x => x.ContractId == contractId)
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

            var query = WorkScope.GetAll<Contract>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetContractDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    File = x.File,
                    Status = x.Status,
                    CreationTime = x.CreationTime,
                    UserId = x.UserId,
                    ExpriedTime = x.ExpriredTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    UpdatedTime = x.LastModificationTime,
                    IsMyContract = x.UserId == userId,
                    NumberOfSetting = dicContractSettingProgress.ContainsKey(x.Id) ? dicContractSettingProgress[x.Id].numberOfSetting : 0,
                    CountCompleted = dicContractSettingProgress.ContainsKey(x.Id) ? dicContractSettingProgress[x.Id].CountComplete : 0,
                    ContractGuid = x.ContractGuid,
                    ContractBase64 = x.FileBase64
                });

            if (input.FilterType > 0)
            {
                switch (input.FilterType)
                {
                    case ContractFilterType.AssignToMe:
                        var contractIdsNeedToSign = contractSettings
                            .Where(x => !x.IsComplete && x.SignerEmail == userEmail && x.ContractRole == ContractRole.Signer && x.IsSendMail)
                            .Select(x => x.ContractId)
                            .ToList();

                        query = query
                            .Where(x => x.Status == ContractStatus.Inprogress)
                            .Where(x => contractIdsNeedToSign.Contains(x.Id));
                        break;

                    case ContractFilterType.WatingForOther:
                        var settingWaitForOther = contractSettings
                            .Where(x => x.IsComplete && x.SignerEmail == userEmail)
                            .Select(x => x.ContractId)
                            .ToList();

                        query = query
                            .Where(x => x.Status == ContractStatus.Inprogress)
                            .Where(x => settingWaitForOther.Contains(x.Id));
                        break;

                    case ContractFilterType.ExpirgingSoon:
                        var myContracts = contractSettings.Where(x => x.SignerEmail == userEmail).Select(x => x.ContractId);
                        query = query
                            .Where(x => myContracts.Contains(x.Id))
                            .Where(x => x.Status == ContractStatus.Inprogress)
                            .Where(x => x.ExpriedTime.HasValue && x.ExpriedTime != default)
                            .Where(x => ((x.ExpriedTime.Value.Date - DateTimeUtils.GetNow().Date).Days < exprireNotiConfig && (x.ExpriedTime.Value.Date - DateTimeUtils.GetNow().Date).Days >= 0));
                        break;

                    case ContractFilterType.Completed:
                        var settingComplete = contractSettings
                            .Where(x => x.IsComplete && x.SignerEmail == userEmail)
                            .Select(x => x.ContractId)
                            .ToList();

                        query = query.Where(x => x.Status == ContractStatus.Complete)
                            .Where(x => settingComplete.Contains(x.Id));
                        break;
                }
            }
            else
            {
                var listContractsFilter = new List<long>();
                if (!string.IsNullOrEmpty(input.SignerEmail))
                {
                    listContractsFilter = WorkScope.GetAll<ContractSetting>()
                        .Where(x => x.SignerEmail == input.SignerEmail)
                        .WhereIf(input.SignerEmail == loginEmail, x => x.ContractRole == ContractRole.Signer)
                        .Select(x => x.ContractId).ToList();
                }

                query = query.Where(x => x.UserId == userId);
                if (!string.IsNullOrEmpty(input.SignerEmail) && listContractsFilter.Count > 0)
                {
                    query = query.Where(x => listContractsFilter.Contains(x.Id));
                }
            }

            if (!string.IsNullOrEmpty(input.Search))
            {
                query = query.Where(x => x.Name.ToLower().Trim().Contains(input.Search.ToLower().Trim())
                || x.ContractGuid.ToString().ToLower() == input.Search.ToLower().Trim() || x.Code.ToLower().Trim().Contains(input.Search.ToLower().Trim()));
            }

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
                item.SignUrl = contractIds.FirstOrDefault(s => s.ContractId == item.Id) != default ? $"{baseUrl}app/signging/email-valid?{HttpUtility.UrlEncode($"settingId={contractIds.FirstOrDefault(s => s.ContractId == item.Id).Id}&contractId={item.Id}&tenantName={tenantName}")}" : "";
                item.IsHasSigned = (WorkScope.GetAll<ContractSigning>()
                    .Where(s => s.ContractId == item.Id && !string.IsNullOrEmpty(s.SignartureBase64))
                .ToList()).Any();
                if (item.ExpriedTime != null)
                {
                    item.IsExprireSoon = item.Status == ContractStatus.Inprogress && (item.ExpriedTime.Value.Date - DateTimeUtils.GetNow().Date).Days <= (exprireNotiConfig - 1) && (item.ExpriedTime.Value.Date - DateTimeUtils.GetNow().Date).Days >= 0;
                }
            }
            return result;
        }

        public async Task<List<GetContractDesginInfo>> GetContractDesginInfo(long contractId)
        {
            var contractImages = WorkScope.GetAll<ContractBase64Image>()
                     .Where(x => x.ContractId == contractId)
                     .OrderBy(x => x.ContractPage)
                     .Select(x => new ContractBase64ImageDto
                     {
                         ContractPage = x.ContractPage,
                         FileBase64 = x.FileBase64,
                         Width = x.Width,
                         Height = x.Height
                     })
                     .ToList();

            var dicSettingColor = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .ToDictionaryAsync(x => x.Id, x => x.Color);

            var signatureSettings = WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .ToList()
                .Select(x => new GetSignerSignatureSettingDto
                {
                    Id = x.Id,
                    ContractSettingId = x.ContractSettingId,
                    SignatureType = x.SignatureType,
                    Height = x.Height,
                    IsSigned = x.IsSigned,
                    PositionX = x.PositionX,
                    PositionY = x.PositionY,
                    Width = x.Width,
                    SignerName = x.ContractSetting.SignerName,
                    Page = x.Page,
                    Color = dicSettingColor.ContainsKey(x.ContractSettingId) ? dicSettingColor[x.ContractSettingId] : null,
                    FontSize = x.FontSize,
                    FontColor = x.FontColor,
                    FontFamily = x.FontFamily
                }).ToList();

            var result = new List<GetContractDesginInfo>();

            foreach (var img in contractImages)
            {
                var designInfo = new GetContractDesginInfo
                {
                    ContractBase64 = img.FileBase64,
                    Page = img.ContractPage,
                    Width = img.Width,
                    Height = img.Height,
                    SignatureSettings = signatureSettings.Where(x => x.Page == img.ContractPage).ToList()
                };
                result.Add(designInfo);
            }

            return result;
        }

        public async Task<GetContractDetailDto> GetContractDetail(long contractId)
        {
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .FirstOrDefaultAsync();

            var contractSettings = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .Select(x => new RecipientDto
                {
                    Email = x.SignerEmail,
                    Name = x.SignerName,
                    IsComplete = x.IsComplete,
                    Role = x.ContractRole,
                    ProcessOrder = x.ProcesOrder,
                    IsSendMail = x.IsSendMail,
                    UpdateDate = x.UpdateDate
                })
                .ToListAsync();

            var createdUserName = await WorkScope.GetAll<User>()
                .Where(x => x.Id == contract.UserId)
                .Select(x => x.FullName)
                .FirstOrDefaultAsync();

            var loginEmail = await WorkScope.GetAll<User>()
                .Where(x => x.Id == AbpSession.UserId)
                .Select(x => x.EmailAddress)
                .FirstOrDefaultAsync();

            var settingId = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.SignerEmail == loginEmail)
                .Where(x => x.ContractId == contractId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            var signUrl = "";

            if (settingId != default)
            {
                signUrl = $"{baseUrl}app/signging/email-valid?{HttpUtility.UrlEncode($"settingId={settingId}&contractId={contractId}&tenantName={tenantName}")}";
            }
            var contractBase64 = await WorkScope.GetAll<ContractSigning>()
                .Where(x => x.ContractId == contractId)
                .OrderByDescending(x => x.TimeAt)
                .Select(x => x.SigningResult)
                .FirstOrDefaultAsync();

            if (contractBase64 == default)
            {
                contractBase64 = contract.FileBase64;
            }
            bool IsHasSigned = (WorkScope.GetAll<ContractSigning>()
                    .Where(s => s.ContractId == contractId && !string.IsNullOrEmpty(s.SignartureBase64))
                    .ToList()).Any();
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

            return new GetContractDetailDto
            {
                SettingId = settingId,
                ContractId = contractId,
                ContractName = contract.Name,
                ContractCode = contract.Code,
                ContractBase64 = contractBase64,
                CreationTime = contract.CreationTime,
                ContractFile = contract.File,
                CreatedUser = createdUserName,
                Status = contract.Status,
                UpdatedTime = contract.LastModificationTime,
                ExpriedTime = contract.ExpriredTime,
                Recipients = contractSettings,
                SignUrl = signUrl,
                IsAssigned = settingId != default,
                IsMyContract = contract.UserId == AbpSession.UserId,
                IsHasSigned = IsHasSigned,
                Note = cancelHistory.Note,
                ContractGuid = contract.ContractGuid
            };
        }

        public async Task<List<ContractBase64ImageDto>> GetContractFileImage(long contractId)
        {
            return await WorkScope.GetAll<ContractBase64Image>()
                .Where(x => x.ContractId == contractId)
                .OrderBy(x => x.ContractPage)
                .Select(x => new ContractBase64ImageDto
                {
                    ContractPage = x.ContractPage,
                    FileBase64 = x.FileBase64,
                    Width = x.Width,
                    Height = x.Height
                })
                .ToListAsync();
        }

        public MailPreviewInfoDto GetContractMailContent(long contractId)
        {
            return _emailManager.GetEmailContentById(MailFuncEnum.Signing, contractId);
        }

        public async Task<GetContractStatisticDto> GetContractStatistic()
        {
            int totalWaitForMe = 0;
            int totalWaitForOther = 0;
            int totalExprireSoon = 0;
            int totalComplete = 0;

            var userId = AbpSession.UserId;

            var userEmail = WorkScope.GetAll<User>()
                .Where(x => x.Id == userId)
                .Select(x => x.EmailAddress)
                .FirstOrDefault();

            var contractSettings = WorkScope.GetAll<ContractSetting>()
                .ToList();

            var query = WorkScope.GetAll<Contract>();

            var contractIdsNeedToSign = contractSettings
                           .Where(x => !x.IsComplete && x.SignerEmail == userEmail && x.ContractRole == ContractRole.Signer && x.IsSendMail)
                           .Select(x => x.ContractId)
                           .ToList();

            totalWaitForMe = query
                .Where(x => x.Status == ContractStatus.Inprogress)
                .Where(x => contractIdsNeedToSign.Contains(x.Id))
                .Count();

            var settingWaitForOther = contractSettings
                           .Where(x => x.IsComplete && x.SignerEmail == userEmail)
                           .Select(x => x.ContractId)
                           .ToList();
            totalWaitForOther = query
                .Where(x => x.Status == ContractStatus.Inprogress)
                .Where(x => settingWaitForOther.Contains(x.Id))
                .Count();

            var settingComplete = contractSettings
                         .Where(x => x.IsComplete && x.SignerEmail == userEmail)
                         .Select(x => x.ContractId)
                         .ToList();
            totalComplete = query.Where(x => x.Status == ContractStatus.Complete)
                .Where(x => settingComplete.Contains(x.Id))
            .Count();

            var myContracts = contractSettings.Where(x => x.SignerEmail == userEmail).Select(x => x.ContractId);
            var exprireNotiConfig = int.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.NotiExprireTime));
            totalExprireSoon = query
                .Where(x => myContracts.Contains(x.Id))
                .Where(x => x.Status == ContractStatus.Inprogress)
                .Where(x => (x.ExpriredTime.Value.Date - DateTimeUtils.GetNow().Date).Days <= (exprireNotiConfig - 1) && (x.ExpriredTime.Value.Date - DateTimeUtils.GetNow().Date).Days >= 0)
                .Count();

            return new GetContractStatisticDto
            {
                WaitForMe = totalWaitForMe,
                WaitForOther = totalWaitForOther,
                ExprireSoon = totalExprireSoon,
                Complete = totalComplete
            };
        }

        public async Task<GetContractMailSettingDto> GetSendMailInfo(long contractId)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .FirstOrDefaultAsync();

            var signers = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .Select(x => new SignerDto
                {
                    Name = x.SignerName,
                    Email = x.SignerEmail,
                    ContractRole = x.ContractRole,
                    ProcesOrder = x.ProcesOrder,
                    Color = x.Color
                })
                .ToListAsync();

            return new GetContractMailSettingDto
            {
                File = contract.File,
                MailContent = contract.EmailContent,
                Signers = signers
            };
        }

        public string GetSignUrl(long settingId, long contractId)
        {
            var tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            return $"{baseUrl}app/signging/email-valid?{HttpUtility.UrlEncode($"settingId={settingId}&contractId={contractId}&tenantName={tenantName}")}";
        }

        public IQueryable<GetContractDto> QueryAllContract()
        {
            return WorkScope.GetAll<Contract>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetContractDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Code = x.Code,
                    ExpriedTime = x.ExpriredTime.Value,
                    Status = x.Status,
                    File = x.File,
                    FileBase64 = x.FileBase64,
                    UpdatedUser = (x.LastModificationTime == default || x.LastModifierUser == null) ? string.Empty : x.LastModifierUser.FullName,
                    UpdatedTime = x.LastModificationTime != default ? x.LastModificationTime : null,
                    CreatorUser = x.CreatorUser == null ? string.Empty : x.CreatorUser.FullName,
                    CreationTime = x.CreationTime,
                    ContractTemplateId = x.ContractTemplateId
                });
        }

        public async Task RemoveAllSignature(long contractId)
        {
            var allSignature = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .ToListAsync();
            allSignature.ForEach(x =>
            {
                x.IsDeleted = true;
            });
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<string> RenderCertificatePdf(CertificateDto certificate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

            var outputFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "certificatePdf", $"Certificate_{certificate.ContractId}.pdf");

            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 10, 10, 20, 20);

            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputFilePath, FileMode.Create));

            document.Open();

            PdfPTable table = new PdfPTable(3);
            float[] columnWidths = { 45f, 25f, 30f };
            table.SetWidths(columnWidths);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            string fontPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "font", "times.ttf");
            BaseFont customFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            Font fontNormal = new Font(customFont, 10, Font.NORMAL);
            Font fontBold = new Font(customFont, 12, Font.BOLD);

            PdfPCell signatureCertificate = new PdfPCell(new Phrase("LỊCH SỬ KÝ TÀI LIỆU", new Font(customFont, 16, Font.BOLD)));
            signatureCertificate.Colspan = 3;
            signatureCertificate.HorizontalAlignment = Element.ALIGN_CENTER;
            signatureCertificate.BorderWidth = 0f;
            signatureCertificate.MinimumHeight = 30f;
            table.AddCell(signatureCertificate);

            PdfPCell contractGuid = new PdfPCell(new Phrase($"ID: {certificate.ContractGuId}", fontNormal));
            contractGuid.Colspan = 3;
            contractGuid.BorderWidth = 0f;
            contractGuid.PaddingBottom = 3f;
            table.AddCell(contractGuid);

            PdfPCell contractName = new PdfPCell(new Phrase($"Tên hợp đồng: {certificate.ContractName}", fontNormal));
            contractName.Colspan = 3;
            contractName.BorderWidth = 0f;
            contractName.PaddingBottom = 3f;
            table.AddCell(contractName);

            //PdfPCell fileName = new PdfPCell(new Phrase($"Tên file hợp đồng: {certificate.FileName}", fontNormal));
            //fileName.Colspan = 3;
            //fileName.BorderWidth = 0f;
            //table.AddCell(fileName);

            string statusName = "";
            switch (certificate.Status)
            {
                case ContractStatus.Draft: statusName = "Bản nháp"; break;
                case ContractStatus.Inprogress: statusName = "Đang chờ ký"; break;
                case ContractStatus.Complete: statusName = "Hoàn thành"; break;
                case ContractStatus.Cancelled: statusName = "Đã hủy"; break;
                default: statusName = ""; break;
            }

            PdfPCell status = new PdfPCell(new Phrase($"Trạng thái: {statusName}", fontNormal));
            status.Colspan = 3;
            status.BorderWidth = 0f;
            status.PaddingBottom = 3f;
            table.AddCell(status);

            PdfPCell creationUser = new PdfPCell(new Phrase($"Người tạo: {certificate.CreatorEmail}", fontNormal));
            creationUser.Colspan = 3;
            creationUser.BorderWidth = 0f;
            creationUser.PaddingBottom = 3f;
            table.AddCell(creationUser);

            PdfPCell creationTime = new PdfPCell(new Phrase($"Thời gian tạo: {certificate.CreationTime.ToString("dd/MM/yyyy HH:mm:ss")}", fontNormal));
            creationTime.Colspan = 3;
            creationTime.BorderWidth = 0f;
            creationTime.PaddingBottom = 3f;
            table.AddCell(creationTime);

            if (certificate.ExpriredTime.HasValue)
            {
                PdfPCell expireTime = new PdfPCell(new Phrase($"Thời hạn ký: {certificate.ExpriredTime.Value.ToString("dd/MM/yyyy HH:mm:ss")}", fontNormal));
                expireTime.Colspan = 3;
                expireTime.BorderWidth = 0f;
                expireTime.PaddingBottom = 3f;
                table.AddCell(expireTime);
            }

            //PdfPCell expriedTime = new PdfPCell(new Phrase($"Thời hạn hợp đồng: {certificate.ExpriredTime}" + (certificate.ExpriredTime.HasValue ? $" Múi giờ: {localTimeZone.DisplayName}" : ""), fontNormal));
            //expriedTime.Colspan = 3;
            //expriedTime.BorderWidth = 0f;
            //table.AddCell(expriedTime);

            PdfPCell localTimeName = new PdfPCell(new Phrase($"Múi giờ: {localTimeZone.DisplayName}", fontNormal));
            localTimeName.Colspan = 3;
            localTimeName.BorderWidth = 0f;
            localTimeName.PaddingBottom = 10f;
            table.AddCell(localTimeName);

            certificate.Signatures = certificate.Signatures.Where(x => !x.SignatureType.HasValue || (x.SignatureType.Value == SignMethod.Image || x.SignatureType.Value == SignMethod.UsbToken)).ToList();

            if (certificate.Signatures.Count > 0)
            {
                PdfPCell signerTitle = new PdfPCell(new Phrase("Người ký", fontBold));
                PdfPCell timestampTitle = new PdfPCell(new Phrase("Thời gian", fontBold));
                PdfPCell signatureTitle = new PdfPCell(new Phrase("Chữ ký", fontBold));

                signerTitle.BorderWidth = 0f;
                timestampTitle.BorderWidth = 0f;
                signatureTitle.BorderWidth = 0f;

                signerTitle.PaddingBottom = 6f;
                timestampTitle.PaddingBottom = 6f;
                signatureTitle.PaddingBottom = 6f;
                signatureTitle.HorizontalAlignment = Element.ALIGN_CENTER;

                if (certificate.Signatures.Count != 0)
                {
                    table.AddCell(signerTitle);
                    table.AddCell(timestampTitle);
                    table.AddCell(signatureTitle);
                }

                foreach (var signatureItem in certificate.Signatures)
                {
                    PdfPCell signerName = new PdfPCell(new Phrase(signatureItem.Name, new Font(customFont, 14, Font.BOLD)));
                    signerName.Colspan = 2;
                    signerName.BorderWidth = 0f;
                    signerName.BorderWidthTop = 0.2f;
                    table.AddCell(signerName);

                    string base64Image = signatureItem.SignartureBase64.Replace("data:image/png;base64,", "");
                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                    Image image = Image.GetInstance(imageBytes);
                    image.ScaleAbsoluteWidth(130f);
                    image.Alignment = Image.ALIGN_CENTER;

                    PdfPCell signature = new PdfPCell();
                    signature.AddElement(image);
                    signature.BorderWidth = 0f;
                    signature.BorderWidthTop = 0.2f;
                    if (signatureItem.SignatureType.HasValue && signatureItem.SignatureType.Value != SignMethod.UsbToken)
                    {
                        signature.Rowspan = 5;
                    }
                    else signature.Rowspan = 4;
                    signature.PaddingBottom = 0f;
                    signature.PaddingLeft = 28f;
                    signature.HorizontalAlignment = Element.ALIGN_LEFT;
                    signature.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(signature);

                    PdfPCell email = new PdfPCell(new Phrase($"Email: {signatureItem.Email}", fontNormal));
                    email.Colspan = 2;
                    email.BorderWidth = 0f;
                    table.AddCell(email);
                    if (signatureItem.SignatureType.HasValue && signatureItem.SignatureType.Value != SignMethod.UsbToken)
                    {
                        PdfPCell id = new PdfPCell(new Phrase($"ID: {signatureItem.GuId}", fontNormal));
                        id.Colspan = 2;
                        id.BorderWidth = 0f;
                        table.AddCell(id);
                    }

                    PdfPCell sent = new PdfPCell(new Phrase("Thời gian gửi:", fontNormal));
                    sent.BorderWidth = 0f;
                    table.AddCell(sent);

                    PdfPCell sendingTime = new PdfPCell(new Phrase(signatureItem.SendingTime.ToString("dd/MM/yyyy HH:mm:ss") + $" {localTimeZone.DisplayName.Split(')')[0]})", fontNormal));
                    sendingTime.BorderWidth = 0f;
                    table.AddCell(sendingTime);

                    PdfPCell signed = new PdfPCell(new Phrase("Thời gian ký:", fontNormal));
                    signed.BorderWidth = 0f;
                    signed.PaddingBottom = 10f;
                    table.AddCell(signed);

                    PdfPCell signedTime = new PdfPCell(new Phrase(signatureItem.SigningTime.ToString("dd/MM/yyyy HH:mm:ss") + $" {localTimeZone.DisplayName.Split(')')[0]})", fontNormal));
                    signedTime.BorderWidth = 0f;
                    signedTime.PaddingBottom = 12f;
                    table.AddCell(signedTime);

                    //PdfPCell methodSign = new PdfPCell(new Phrase($"Phương thức ký: {signatureItem.SignatureType}", fontNormal));
                    //methodSign.Colspan = 2;
                    //methodSign.BorderWidth = 0f;
                    //methodSign.PaddingBottom = 12f;
                    //table.AddCell(methodSign);
                }
            }

            document.Add(table);

            document.Close();

            byte[] fileBytes = File.ReadAllBytes(outputFilePath);
            string base64Pdf = Convert.ToBase64String(fileBytes);

            File.Delete(outputFilePath);

            return "data:application/pdf;base64," + base64Pdf;
        }

        public async Task ResendMailAll(long contractId)
        {
            var contract = WorkScope.GetAll<Contract>()
                .Include(x => x.User)
                .Where(x => x.Id == contractId).FirstOrDefault();

            string contractCreator = contract.User.EmailAddress;

            var contractHistory = WorkScope.GetAll<ContractHistory>()
                .Where(x => x.ContractId == contractId)
                .Where(x => !string.IsNullOrEmpty(x.MailContent))
                .FirstOrDefault();

            MailPreviewInfoDto mailContent = contractHistory.MailContent.FromJsonString<MailPreviewInfoDto>();

            var signers = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract)
                .Include(x => x.Contract.User)
                .Where(x => x.ContractId == contractId && x.IsSendMail == true)
                .OrderBy(x => x.ProcesOrder)
                .ToListAsync();

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            List<ResultTemplateEmail<ContractMailTemplateDto>> mailContents = signers.Where(x => x.ContractRole == ContractRole.Viewer || x.IsComplete == false).Select(x => new ResultTemplateEmail<ContractMailTemplateDto>
            {
                Result = SetContractMailTemplate(x, baseUrl)
            }).ToList();

            bool IsReSent = true;

            foreach (var content in mailContents)
            {
                MailPreviewInfoDto clonedMailContent = (MailPreviewInfoDto)mailContent.Clone();
                clonedMailContent.SendToEmail = content.Result.SendToEmail;
                clonedMailContent.CurrentUserLoginId = AbpSession.UserId;
                clonedMailContent.TenantId = AbpSession.TenantId;
                var action = content.Result.ContractRole == ContractRole.Signer ? "[Ký tài liệu]" : "[Nhận một bản sao]";
                clonedMailContent.Subject = string.IsNullOrEmpty(mailContent.Subject) ? $"{action} {content.Result.ContractName}" : mailContent.Subject;
                clonedMailContent.BodyMessage = CommonUtils.ReplaceBodyMessage(clonedMailContent.BodyMessage, content.Result);
                clonedMailContent.MailHistory = CreateContractHistory(contractCreator, contractId, content.Result.SendToEmail, content.Result.ContractRole, IsReSent);

                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(clonedMailContent, BackgroundJobPriority.High, TimeSpan.FromSeconds(0));
            };
        }

        public async Task ResendMailOne(ReSendMailDto input)
        {
            var contract = WorkScope.GetAll<Contract>()
                .Include(x => x.User)
                .Where(x => x.Id == input.ContractId).FirstOrDefault();

            string contractCreator = contract.User.EmailAddress;

            var contractHistory = WorkScope.GetAll<ContractHistory>()
                .Where(x => x.ContractId == input.ContractId)
                .Where(x => !string.IsNullOrEmpty(x.MailContent))
                .FirstOrDefault();

            MailPreviewInfoDto mailContent = contractHistory.MailContent.FromJsonString<MailPreviewInfoDto>();

            var signers = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract)
                .Include(x => x.Contract.User)
                .Where(x => x.ContractId == input.ContractId && x.SignerEmail == input.ResentToMail)
                .OrderBy(x => x.ProcesOrder)
                .FirstOrDefaultAsync();

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");
            var content = new ResultTemplateEmail<ContractMailTemplateDto>
            {
                Result = SetContractMailTemplate(signers, baseUrl)
            };

            bool IsReSent = true;

            var action = content.Result.ContractRole == ContractRole.Signer ? "[Ký tài liệu]" : "[Nhận một bản sao]";
            mailContent.SendToEmail = signers.SignerEmail;
            mailContent.CurrentUserLoginId = AbpSession.UserId;
            mailContent.TenantId = AbpSession.TenantId;
            mailContent.Subject = string.IsNullOrEmpty(mailContent.Subject) ? $"{action} {content.Result.ContractName}" : mailContent.Subject;
            mailContent.BodyMessage = CommonUtils.ReplaceBodyMessage(mailContent.BodyMessage, content.Result);
            mailContent.MailHistory = CreateContractHistory(contractCreator, input.ContractId, content.Result.SendToEmail, content.Result.ContractRole, IsReSent);
            _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailContent, BackgroundJobPriority.High, TimeSpan.FromSeconds(0));
        }

        public async Task<long> SaveDraft(long contractId)
        {
            var contract = await WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .FirstOrDefaultAsync();

            contract.Status = ContractStatus.Draft;

            await WorkScope.UpdateAsync(contract);

            return contractId;
        }

        public async Task<object> SendMail(SendMailDto input)
        {
            var contract = WorkScope.GetAll<Contract>()
                .Include(x => x.User)
                .Where(x => x.Id == input.ContractId).FirstOrDefault();

            string contractCreator = contract.User.EmailAddress;

            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            var signers = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract)
                .Include(x => x.Contract.User)
                .Where(x => x.ContractId == input.ContractId && x.ContractRole == ContractRole.Signer)
                .OrderBy(x => x.ProcesOrder)
                .ToListAsync();

            var isOrder = signers.Any(x => x.ProcesOrder != 1);
            List<ResultTemplateEmail<ContractMailTemplateDto>> maiContents = signers.Where(x => !x.IsComplete)
            .Select(x => new ResultTemplateEmail<ContractMailTemplateDto>
            {
                Result = SetContractMailTemplate(x, baseUrl)
            }).ToList();

            var lastcontent = new ResultTemplateEmail<ContractMailTemplateDto>();
            if (maiContents.Count == 0 && isOrder /*&& !isSendAll*/)
            {
                var lastSigner = signers.Last();
                lastcontent = new ResultTemplateEmail<ContractMailTemplateDto>
                {
                    Result = SetContractMailTemplate(lastSigner, baseUrl)
                };
                MailPreviewInfoDto mailInput = (MailPreviewInfoDto)input.MailContent.Clone();
                mailInput.CurrentUserLoginId = AbpSession.UserId;
                mailInput.TenantId = AbpSession.TenantId;
                mailInput.Subject = string.IsNullOrEmpty(mailInput.Subject) ? $"[Ký tài liệu] {lastcontent.Result.ContractName}" : mailInput.Subject;
                mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, lastcontent.Result);
                mailInput.ContractSettingId = lastcontent.Result.ContractSettingId;
                mailInput.MailHistory = CreateContractHistory(contractCreator, input.ContractId, lastcontent.Result.SendToEmail, ContractRole.Signer);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(1));
            }

            var delaySendMail = 0;

            if (maiContents.Count > 0 /*|| isSendAll*/)
            {
                foreach (var content in maiContents)
                {
                    MailPreviewInfoDto mailInput = (MailPreviewInfoDto)input.MailContent.Clone();
                    mailInput.CurrentUserLoginId = AbpSession.UserId;
                    mailInput.TenantId = AbpSession.TenantId;
                    mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, content.Result);
                    mailInput.ContractSettingId = content.Result.ContractSettingId;
                    mailInput.SendToEmail = content.Result.SendToEmail;
                    mailInput.Subject = string.IsNullOrEmpty(mailInput.Subject) ? $"[Ký tài liệu] {content.Result.ContractName}" : mailInput.Subject;
                    mailInput.MailHistory = CreateContractHistory(contractCreator, input.ContractId, content.Result.SendToEmail, ContractRole.Signer);
                    _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));

                    delaySendMail += ECConsts.DELAY_SEND_MAIL_SECOND;

                    var firstContent = WorkScope.GetAll<ContractHistory>()
                        .Where(x => x.ContractId == input.ContractId && x.Action == HistoryAction.SendMail)
                        .Where(x => string.IsNullOrEmpty(x.MailContent)).FirstOrDefault();

                    if (firstContent == default)
                    {
                        var contentHistory = new CreaContractHistoryDto
                        {
                            MailContent = input.MailContent.ToJsonString(),
                            Action = HistoryAction.SendMail,
                            ContractId = input.ContractId,
                            ContractStatus = ContractStatus.Inprogress,
                            Note = "CreateEmailTemplate",
                            TimeAt = Clock.Provider.Now,
                            AuthorEmail = contractCreator
                        };
                        await _contractHistoryManager.Create(contentHistory);
                    }
                    if (isOrder /*&& !isSendAll*/)
                    {
                        break;
                    }
                };
            }

            var contractSettingId = signers.Where(x => x.SignerEmail == contractCreator)
                .Select(x => x.Id)
                .FirstOrDefault();

            var isfirstSigner = signers.First().SignerEmail == contractCreator;

            contract.Status = ContractStatus.Inprogress;
            await WorkScope.UpdateAsync(contract);
            return new
            {
                contractId = input.ContractId,
                settingId = contractSettingId,
                isAssigned = contractSettingId != default,
                isOrder = isOrder,
                isfirstSigner = isfirstSigner,
                EmailContent = maiContents.Count > 0 ? maiContents[0].Result.SignUrl : lastcontent.Result.SignUrl,
                Receivers = maiContents
            };
        }

        public async Task SendMailToViewer(SendMailDto input)
        {
            //var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Signing);
            //if (emailTemplate == default)
            //{
            //    throw new UserFriendlyException($"Not found email template");
            //}
            var contract = WorkScope.GetAll<Contract>()
                .Include(x => x.User)
                .Where(x => x.Id == input.ContractId).FirstOrDefault();
            string contractCreator = contract.User.EmailAddress;
            var baseUrl = _appConfiguration.GetValue<string>("App:ClientRootAddress");

            var viewers = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract)
                .Include(x => x.Contract.User)
                .Where(x => x.ContractId == input.ContractId && x.ContractRole == ContractRole.Viewer)
                .ToListAsync();

            if (viewers.Count > 0)
            {
                string notiReceivers = viewers.Select(x => x.SignerEmail).JoinAsString(", ");
                List<ResultTemplateEmail<ContractMailTemplateDto>> maiContents = viewers.Select(x => new ResultTemplateEmail<ContractMailTemplateDto>
                {
                    Result = SetContractMailTemplate(x, baseUrl)
                }).ToList();

                var delaySendMail = 0;
                foreach (var content in maiContents)
                {
                    MailPreviewInfoDto mailInput = (MailPreviewInfoDto)input.MailContent.Clone();
                    mailInput.SendToEmail = content.Result.SendToEmail;
                    mailInput.CurrentUserLoginId = AbpSession.UserId;
                    mailInput.TenantId = AbpSession.TenantId;
                    mailInput.Subject = string.IsNullOrEmpty(mailInput.Subject) ? $"[Nhận một bản sao] {content.Result.ContractName}" : mailInput.Subject;
                    mailInput.BodyMessage = CommonUtils.ReplaceBodyMessage(mailInput.BodyMessage, content.Result);
                    mailInput.ContractSettingId = content.Result.ContractSettingId;
                    mailInput.MailHistory = CreateContractHistory(contractCreator, input.ContractId, notiReceivers, ContractRole.Viewer);
                    _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                    delaySendMail += ECConsts.DELAY_SEND_MAIL_SECOND;
                    var item = await WorkScope.GetAsync<ContractSetting>(mailInput.ContractSettingId.Value);
                    item.IsComplete = true;
                    CurrentUnitOfWork.SaveChanges();
                };
                contract.Status = ContractStatus.Inprogress;
                await WorkScope.UpdateAsync(contract);
            }
        }

        public async Task SetNotiExpiredContract(long contractId)
        {
            var contract = await WorkScope.GetAsync<Contract>(contractId);
            var input = new CancelExpiredContractDto
            {
                ContractId = contract.Id,
                CurrentUserLoginId = AbpSession.UserId,
                ExpiredTime = contract.ExpriredTime,
                TenantId = AbpSession.TenantId
            };
            if (input.ExpiredTime.HasValue)
            {
                var delay = (input.ExpiredTime.Value - CommonUtils.GetNow()).TotalMilliseconds;
                _backgroundJobManager.Enqueue<CancelExpiredContract, CancelExpiredContractDto>(input, BackgroundJobPriority.High, TimeSpan.FromMilliseconds(delay));
            }
        }

        public async Task<UpdatECDto> Update(UpdatECDto input)
        {
            var oldContract = WorkScope.GetAll<Contract>().Where(x => x.Id == input.Id).FirstOrDefault();

            //var isChangeFile = oldContract.FileBase64 != input.FileBase64;

            //var entity = ObjectMapper.Map<Contract>(input);

            //entity.ContractGuid = oldContract.ContractGuid;

            if (oldContract.UserId == default)
            {
                oldContract.UserId = AbpSession.UserId.Value;
            }

            oldContract.ExpriredTime = input.ExpriedTime;
            oldContract.Code = input.Code;
            oldContract.File = input.File;
            oldContract.Status = input.Status;
            oldContract.ContractTemplateId = input.ContractTemplateId;
            oldContract.Name = input.Name;

            //if (isChangeFile)
            //{
            var location = new SignPositionDto
            {
                PositionX = 20,
                PositionY = 10,
                Page = 1
            };
            var fillInput = new FillInputDto
            {
                Color = "black",
                Content = $"MetaSign Document ID: {oldContract.ContractGuid.ToString().ToUpper()}",
                FontSize = 10,
                PageHeight = 0,
                SignerSignatureSettingId = 1,
                IsCreateContract = true
            };

            var base64 = await SignUtils.FillPdfWithText(fillInput, location, input.FileBase64, _webHostEnvironment.WebRootPath);
            var file = CommonUtils.ConvertBase64PdfToFile(base64.Split(",")[1], oldContract.File);
            await _fileStoringManager.DeleteUnsignedContract(oldContract.Id);
            await _fileStoringManager.UploadUnsignedContract(oldContract.Id, file);

            //}

            await WorkScope.UpdateAsync(oldContract);

            //if (isChangeFile)
            //{
            var signatureSettings = WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == input.Id)
                .ToList();
            foreach (var item in signatureSettings)
            {
                await WorkScope.DeleteAsync(item);
            }
            await SaveDraft(input.Id);

            //}

            return input;
        }

        public async Task UpdateProcessOrder(long contractId)
        {
            var qSignatureSettings = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSetting.ContractId == contractId)
                .ToListAsync();
            var hasInput = qSignatureSettings.Any(x => CommonUtils.InputSignature.Contains(x.SignatureType));
            if (hasInput)
            {
                var Signers = OrderSigner(qSignatureSettings);
                var contractSettingIds = new List<long>();
                contractSettingIds.AddRange(Signers.ContractSettingIdHaveInput);
                contractSettingIds.AddRange(Signers.ContractSettingIdHaveBoth);
                contractSettingIds.AddRange(Signers.NormalSignerId);
                var updateSettings = await WorkScope.GetAll<ContractSetting>()
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

        public async Task<string> UploadAndConvert(IFormFile file)
        {
            return await _pDFConverterWebService.UploadAndConvert(file);
        }

        public async Task<object> UploadFile(IFormFile file)
        {
            var fullFilePath = Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder, file.FileName);
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var wordBase64 = "";
            var matchMassField = "";
            var fileType = Path.GetExtension(fullFilePath);
            if (fileType == ".doc" || fileType == ".docx")
            {
                matchMassField = CommonUtils.GetMatchField(file);
                wordBase64 = Convert.ToBase64String(File.ReadAllBytes(fullFilePath));
            }
            if (!CommonUtils.SupportFile.Contains(fileType))
            {
                File.Delete(fullFilePath);
                throw new UserFriendlyException("ThisDocumentFormatIsNotSupported");
            }
            var outputPdfPath = Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder);
            await ConvertToPdf(fullFilePath, Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder));
            string pdfFilePath = Path.Combine(outputPdfPath, Path.GetFileNameWithoutExtension(fullFilePath) + ".pdf");
            Logger.Log(LogSeverity.Debug, $"PDF Location: {pdfFilePath}");
            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            var pdfFileName = Path.GetFileName(pdfFilePath);
            File.Delete(fullFilePath);
            File.Delete(pdfFilePath);
            return new
            {
                Base64String = "data:application/pdf;base64," + pdfBase64,
                WordBase64 = "data:application/msword;base64," + wordBase64,
                MatchMassField = matchMassField,
                PageNumber = 0,
                FileName = pdfFileName
            };
        }

        private async Task ConvertToPdf(string filePath, string outputPdfPath)
        {
            string libreOfficePath = _appConfiguration.GetValue<string>("LibreOfficeDir");
            Logger.Log(LogSeverity.Debug, $"outputPdfPath Location: {outputPdfPath}");

            string command = $" --headless --convert-to pdf " + $"\"{filePath}\"" + " --outdir " + $"\"{outputPdfPath}\"";

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = libreOfficePath,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            Logger.Log(LogSeverity.Debug, $"Command: {command}");
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Logger.Log(LogSeverity.Debug, $"Error process: {stderr}");
            Logger.Log(LogSeverity.Debug, $"Output process: {stdout}");
        }

        private CreaContractHistoryDto CreateContractHistory(string loginUserEmail, long contractId, string receiver, ContractRole contractRole, bool IsReSent = false)
        {
            string sentedTranslate = "sent";
            if (IsReSent)
            {
                sentedTranslate = "reSent";
            };
            return new CreaContractHistoryDto
            {
                Action = HistoryAction.SendMail,
                AuthorEmail = loginUserEmail,
                ContractId = contractId,
                ContractStatus = ContractStatus.Inprogress,
                TimeAt = DateTimeUtils.GetNow(),
                Note = contractRole == ContractRole.Signer ? $"{loginUserEmail} {sentedTranslate} theDocumentTo {receiver}" : $"{loginUserEmail} {sentedTranslate} aCopyOfTheDocumentTo {receiver}"
            };
        }

        private async Task<string> FillAndRepaceContent(string massWordContent, string massField, List<string> listFieldDto)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "tempReplace", Guid.NewGuid().ToString() + ".docx");
            var bytes = Convert.FromBase64String(massWordContent.Split(',')[1].Trim());
            var fieldName = JsonConvert.DeserializeObject<List<string>>(massField);
            File.WriteAllBytes(filePath, bytes);
            using (Document doc = new Document())
            {
                doc.LoadFromFile(filePath);
                for (int i = 0; i < fieldName.Count; i++)
                {
                    doc.Replace(fieldName[i], listFieldDto[i], false, true);
                }
                doc.SaveToFile(filePath, FileFormat.Auto);
            }
            await ConvertToPdf(filePath, Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder));
            var outputFile = Path.Combine(_webHostEnvironment.WebRootPath, tempConvertFolder, Path.GetFileNameWithoutExtension(filePath) + ".pdf");
            var result = Convert.ToBase64String(File.ReadAllBytes(outputFile));
            File.Delete(filePath);
            File.Delete(outputFile);
            return "data:application/pdf;base64," + result;
        }

        private OrderSignerDto OrderSigner(List<SignerSignatureSetting> input)
        {
            var grSignerSignatureSetting = input.GroupBy(x => x.ContractSettingId)
                .Select(x => new
                {
                    ContractSettingId = x.Key,
                    Signature = x.Select(y => y.SignatureType).ToList()
                });
            var contractSettingIdHaveInput = grSignerSignatureSetting.Where(x => x.Signature.Intersect(CommonUtils.InputSignature).Any() && !x.Signature.Intersect(CommonUtils.SigningSignature).Any()).OrderByDescending(x => x.ContractSettingId).Select(x => x.ContractSettingId).ToList();
            var normalSignerId = grSignerSignatureSetting.Where(x => !x.Signature.Intersect(CommonUtils.InputSignature).Any() && x.Signature.Intersect(CommonUtils.SigningSignature).Any()).OrderByDescending(x => x.ContractSettingId).Select(x => x.ContractSettingId).ToList();
            var contractSettingIdHaveBoth = grSignerSignatureSetting.Where(x => !contractSettingIdHaveInput.Contains(x.ContractSettingId) && !normalSignerId.Contains(x.ContractSettingId)).OrderByDescending(x => x.ContractSettingId).Select(x => x.ContractSettingId).ToList();

            return new OrderSignerDto
            {
                ContractSettingIdHaveInput = contractSettingIdHaveInput,
                ContractSettingIdHaveBoth = contractSettingIdHaveBoth,
                NormalSignerId = normalSignerId,
            };
        }

        private ContractMailTemplateDto SetContractMailTemplate(ContractSetting contractSetting, string baseUrl)
        {
            string tenantName = "";
            if (AbpSession.TenantId.HasValue)
            {
                tenantName = _tenantManager.GetById(AbpSession.TenantId.Value).TenancyName;
            }
            return new ContractMailTemplateDto
            {
                AuthorName = contractSetting.Contract.User.FullName,
                SendToName = contractSetting.SignerName,
                ContractSettingId = contractSetting.Id,
                ContractName = contractSetting.Contract.Name,
                SendToEmail = contractSetting.SignerEmail,
                ContractCode = contractSetting.Contract.Code,
                ContractRole = contractSetting.ContractRole,
                AuthorEmail = contractSetting.Contract.User.EmailAddress,
                Subject = contractSetting.ContractRole == ContractRole.Signer ? $"[SignDocument] {contractSetting.Contract.Name}" : $"[ACopyDocument] {contractSetting.Contract.Name}",
                SignUrl = $"{baseUrl}app/signging/email-valid?{HttpUtility.UrlEncode($"settingId={contractSetting.Id}&contractId={contractSetting.ContractId}&tenantName={tenantName}")}",
                Message = contractSetting.ContractRole == ContractRole.Signer ? "YouHadDocumentNeedSign" : "YouHaveReceivedACopyOfTheDocument",
                ContractGuid = contractSetting.Contract.ContractGuid,
                ExpireTime = contractSetting.Contract.ExpriredTime,
                LookupUrl = $"{baseUrl}app/email-login"
            };
        }
    }
}