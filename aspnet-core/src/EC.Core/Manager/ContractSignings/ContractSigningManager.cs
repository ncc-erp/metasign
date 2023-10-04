using Abp.BackgroundJobs;
using Abp.Timing;
using Abp.UI;
using EC.Authorization.Users;
using EC.Configuration;
using EC.Constants.Wokers;
using EC.Entities;
using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using EC.Manager.Contracts;
using EC.Manager.ContractSignings.Dto;
using EC.Manager.FileStoring;
using EC.Manager.Notifications.Email;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Notification;
using EC.Manager.SignatureUsers;
using EC.Manager.SignServerWorkers;
using EC.Utils;
using EC.WebService.DesktopApp;
using EC.WebService.SignServer;
using EC.WebService.SignServer.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSignings
{
    public class ContractSigningManager : BaseManager
    {
        private readonly IConfiguration _appConfiguration;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly ContractHistoryManager _contractHistoryManager;
        private readonly ContractManager _contractManager;
        private readonly DesktopAppService _desktopAppService;
        private readonly EmailManager _emailManager;
        private readonly FileStoringManager _fileStoringManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly NotificationManager _notificationManager;
        private readonly SignatureUserManager _signatureUserManager;
        private readonly SignServerWebService _signServerWebService;
        private readonly SignServerWorkerManager _signServerWorkerManager;
        public ContractSigningManager(IWorkScope workScope,
            SignServerWebService signServerWebService,
            SignServerWorkerManager signServerWorkerManager,
            ContractHistoryManager contractHistoryManager,
            BackgroundJobManager backgroundJobManager,
            IConfiguration configuration,
            SignatureUserManager signatureUserManager,
            DesktopAppService desktopAppService,
            ContractManager contractManager,
            IWebHostEnvironment webHostEnvironment,
            EmailManager emailManager,
            NotificationManager notificationManager,
            FileStoringManager fileStoringManager) : base(workScope)
        {
            _signServerWebService = signServerWebService;
            _signServerWorkerManager = signServerWorkerManager;
            _contractHistoryManager = contractHistoryManager;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
            _appConfiguration = configuration;
            _signatureUserManager = signatureUserManager;
            _contractManager = contractManager;
            _desktopAppService = desktopAppService;
            _hostingEnvironment = webHostEnvironment;
            _notificationManager = notificationManager;
            _fileStoringManager = fileStoringManager;
        }

        public async Task CompleteContract(long contractId)
        {
            var allComplete = true;
            var settings = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.ContractId == contractId)
                .Where(x => x.ContractRole == ContractRole.Signer)
                .Select(x => x.IsComplete)
                .ToListAsync();

            foreach (var item in settings)
            {
                if (item == false)
                {
                    allComplete = false; break;
                }
            }

            if (allComplete)
            {
                var contract = await WorkScope.GetAll<Entities.Contract>()
                    .Where(x => x.Id == contractId)
                    .FirstOrDefaultAsync();

                if (contract.Status != ContractStatus.Complete)
                {
                    contract.Status = ContractStatus.Complete;

                    await WorkScope.UpdateAsync(contract);

                    var history = new CreaContractHistoryDto
                    {
                        Action = HistoryAction.Complete,
                        AuthorEmail = "",
                        ContractId = contractId,
                        ContractStatus = ContractStatus.Complete,
                        TimeAt = DateTimeUtils.GetNow(),
                        Note = $"TheDocumentHasBeenCompleted"
                    };
                    await _contractHistoryManager.Create(history);
                }
                await _notificationManager.RemoveOldJob(contractId);
                await _notificationManager.NotifyCompleteContract(contractId);
            }
        }

        public string ConvertImageToBase64(string imagePath)
        {
            try
            {
                string wwwRootPath = _hostingEnvironment.WebRootPath;
                string imagePathInWebRoot = Path.Combine(wwwRootPath, imagePath);

                byte[] imageBytes = File.ReadAllBytes(imagePathInWebRoot);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting image to Base64: " + ex.Message);
                return null;
            }
        }

        public async Task CreateUserSignature(string base64Signature, SignatureTypeSetting sigatureType, string email, bool setDefault)
        {
            var userId = await WorkScope.GetAll<User>()
                .Where(x => x.EmailAddress.ToLower().Trim() == email.ToLower().Trim())
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (userId != default)
            {
                var newSignatureUser = new SignatureUser
                {
                    SignatureType = sigatureType,
                    UserId = userId,
                    FileBase64 = base64Signature,
                    IsDefault = setDefault,
                };
                long id = await WorkScope.InsertAndGetIdAsync(newSignatureUser);

                if (newSignatureUser.IsDefault)
                {
                    await _signatureUserManager.UnDefaultSignatures(id);
                }
            }
        }

        public string GetSignatureBase64()
        {
            string signatureBase64 = ConvertImageToBase64("digitalSignatureImg/signature.png");

            return signatureBase64;
        }

        public async Task<string> GetSignerEmail(long settingId)
        {
            return WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == settingId).FirstOrDefault().SignerEmail.Trim();
        }

        public SignMethod GetSignMethod(SignatureTypeSetting type)
        {
            var guid = Guid.NewGuid();
            switch (type)
            {
                case SignatureTypeSetting.Electronic:
                    {
                        return SignMethod.Image;
                    }
                case SignatureTypeSetting.Digital:
                    {
                        return SignMethod.UsbToken;
                    }
                case SignatureTypeSetting.Text:
                    {
                        return SignMethod.Input;
                    }
                case SignatureTypeSetting.DatePicker:
                    {
                        return SignMethod.Input;
                    }
                case SignatureTypeSetting.Stamp:
                    {
                        return SignMethod.Image;
                    }
                default: return SignMethod.Image;
            }
        }

        public string GetSignPosition(float x, float y, float width, float height)
        {
            if (width == default)
            {
                width = 120;
            }

            if (height == default)
            {
                height = 90;
            }

            var rectangleCoordinates = CommonUtils.GetRectangleCoordinates(x, y, width, height);

            return rectangleCoordinates;
        }

        public async Task InsertSigningResult(long contractId, Guid guid, string signingResult, SignMethod signatureType, string signatureBase64 = "", string email = "")
        {
            var entity = new ContractSigning
            {
                ContractId = contractId,
                SignartureBase64 = signatureBase64,
                Email = email,
                TimeAt = DateTimeUtils.GetNow(),
                Guid = guid,
                SignatureType = signatureType
            };
            var fileName = WorkScope.GetAll<Contract>().Where(x => x.Id == contractId).FirstOrDefault().File;
            var file = CommonUtils.ConvertBase64PdfToFile(signingResult.Split(",")[1], fileName);
            await _fileStoringManager.UploadContract(contractId, file);
            await WorkScope.InsertAsync(entity);
        }

        public async Task<bool> InsertSigningResultAndComplete(InputSigningResultDto input)
        {
            var contractSetting = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == input.ContractSettingId)
                .FirstOrDefaultAsync();
            var signMethod = SignMethod.Input;
            var guid = Guid.NewGuid();
            if (input.HasDigital.HasValue && input.HasDigital.Value)
            {
                signMethod = SignMethod.UsbToken;
            }

            var signatureBase64 = GetSignatureBase64();
            contractSetting.IsComplete = true;
            contractSetting.UpdateDate = Clock.Provider.Now;
            contractSetting.LastModificationTime = Clock.Provider.Now;

            await InsertSigningResult(contractSetting.ContractId, guid, input.SignResult, signMethod, signatureBase64, contractSetting.SignerEmail);

            await UpdateSignedSignature(input.ContractSettingId);

            var history = new CreaContractHistoryDto
            {
                Action = HistoryAction.Sign,
                AuthorEmail = contractSetting.SignerEmail,
                ContractId = contractSetting.ContractId,
                ContractStatus = ContractStatus.Inprogress,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"{contractSetting.SignerEmail} signedTheDocument"
            };
            await _contractHistoryManager.Create(history);

            CurrentUnitOfWork.SaveChanges();

            var signers = await WorkScope.GetAll<ContractSetting>()
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.User)
                            .Where(x => x.ContractId == contractSetting.ContractId && x.ContractRole == ContractRole.Signer && !x.IsComplete)
                            .ToListAsync();

            var isOrder = signers.Any(x => x.ProcesOrder != 1);
            if (isOrder)
            {
                var firstContent = WorkScope.GetAll<ContractHistory>()
                .Where(x => x.ContractId == contractSetting.ContractId && x.Action == HistoryAction.SendMail)
                .Where(x => !string.IsNullOrEmpty(x.MailContent)).FirstOrDefault();
                var mailContent = JsonSerializer.Deserialize<MailPreviewInfoDto>(firstContent.MailContent);
                await _contractManager.SendMail(new Contracts.Dto.SendMailDto { ContractId = contractSetting.ContractId, MailContent = mailContent });
            }

            await CompleteContract(contractSetting.ContractId);

            return true;
        }

        public async Task<bool> InsertSigningResultForInput(InputSigningResultDto input)
        {
            var contractSetting = await WorkScope.GetAll<ContractSetting>()
            .Where(x => x.Id == input.ContractSettingId)
            .FirstOrDefaultAsync();
            //var signatureType = await WorkScope.GetAll<SignerSignatureSetting>()
            //    .Where(x => x.ContractSetting.ContractId == contractSetting.ContractId)
            //    .Where(x => x.ContractSettingId == input.ContractSettingId)
            //    .Select(x => x.SignatureType)
            //    .FirstOrDefaultAsync();

            var signMethod = SignMethod.UsbToken;
            var signatureBase64 = GetSignatureBase64();

            var guid = Guid.NewGuid();

            await InsertSigningResult(contractSetting.ContractId, guid, input.SignResult, signMethod, signatureBase64, contractSetting.SignerEmail);
            return true;
        }

        public async Task<string> SignDigitalSignature(SignFromCertDto input)
        {
            var cert = CertUtils.getX509Certificate(input.CertSerial);

            var result = SignUtils.SignPdfFromBase64(input.PdfBase64, input.SignatureBase64, cert, input.Page, input.Position);

            return result;
        }

        public async Task<string> SignInput(SignInputsDto input)
        {
            string result = input.Base64Pdf;

            var filledSetting = input.ListInput.Select(x => x.SignerSignatureSettingId).ToList();

            var settings = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => filledSetting.Contains(x.Id))
                .ToListAsync();

            foreach (var item in input.ListInput)
            {
                var signLocation = await WorkScope.GetAll<SignerSignatureSetting>()
                    .Where(x => x.Id == item.SignerSignatureSettingId)
                    .Select(x => new SignPositionDto
                    {
                        PositionX = x.PositionX,
                        PositionY = x.PositionY,
                        Page = x.Page,
                    })
                    .FirstOrDefaultAsync();

                result = await SignUtils.FillPdfWithText(item, signLocation, result, _hostingEnvironment.WebRootPath);
            }

            foreach (var item in settings)
            {
                item.IsSigned = true;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<string> SignMultiple(SignMultipleDto input)
        {
            string contractBase64 = "";

            if (!String.IsNullOrEmpty(input.ContractBase64))
            {
                contractBase64 = input.ContractBase64;
            }
            else
            {
                contractBase64 = WorkScope.GetAll<ContractSigning>()
                .Where(x => x.ContractId == input.ContractId)
                .OrderByDescending(x => x.TimeAt)
                .Select(x => x.SigningResult)
                .FirstOrDefault();

                if (contractBase64 == default)
                {
                    contractBase64 = await WorkScope.GetAll<Entities.Contract>()
                           .Where(x => x.Id == input.ContractId)
                           .Select(x => x.FileBase64)
                           .FirstOrDefaultAsync();
                    if (string.IsNullOrEmpty(contractBase64))
                    {
                        contractBase64 = await _fileStoringManager.DownloadLatestContractBase64(input.ContractId);
                    }
                }
                else if (string.IsNullOrEmpty(contractBase64))
                {
                    contractBase64 = await _fileStoringManager.DownloadLatestContractBase64(input.ContractId);
                }
            }

            var contractSettingId = await WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.Id == input.SignSignatures.FirstOrDefault().SignerSignatureSettingId)
                .Select(x => x.ContractSettingId)
                .FirstOrDefaultAsync();

            var contractSetting = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == contractSettingId)
                .FirstOrDefaultAsync();

            var contract = await WorkScope.GetAsync<Entities.Contract>(contractSetting.ContractId);

            foreach (var item in input.SignSignatures)
            {
                var guid = Guid.NewGuid();

                var signMethod = GetSignMethod(item.SignatureType);

                contractBase64 = await SignProcess(item, contractBase64, guid);

                await InsertSigningResult(contractSetting.ContractId, guid, contractBase64, signMethod, item.SignartureBase64, contractSetting.SignerEmail);
            }

            SigningDto newSignature = input.SignSignatures.FirstOrDefault(x => x.IsNewSignature.Value);

            if (newSignature != default)
            {
                await CreateUserSignature(newSignature.SignartureBase64, newSignature.SignatureType, contractSetting.SignerEmail, newSignature.SetDefault);
            }

            contractSetting.Status = ContractSettingStatus.Confirmed;
            contractSetting.IsComplete = true;
            contractSetting.UpdateDate = Clock.Provider.Now;
            contract.LastModificationTime = Clock.Provider.Now;

            await CurrentUnitOfWork.SaveChangesAsync();

            await UpdateSignedSignature(contractSettingId);

            var history = new CreaContractHistoryDto
            {
                Action = HistoryAction.Sign,
                AuthorEmail = contractSetting.SignerEmail,
                ContractId = contractSetting.ContractId,
                ContractStatus = ContractStatus.Inprogress,
                TimeAt = DateTimeUtils.GetNow(),
                Note = $"{contractSetting.SignerEmail} signedTheDocument"
            };
            await _contractHistoryManager.Create(history);
            var signers = await WorkScope.GetAll<ContractSetting>()
                .Include(x => x.Contract)
                .Include(x => x.Contract.User)
                .Where(x => x.ContractId == contractSetting.ContractId && x.ContractRole == ContractRole.Signer && !x.IsComplete && x.IsSendMail == false)
                .ToListAsync();

            var isOrder = signers.Any(x => x.ProcesOrder != 1);

            if (isOrder && signers.Count > 0)
            {
                var firstContent = WorkScope.GetAll<ContractHistory>()
                .Where(x => x.ContractId == contractSetting.ContractId && x.Action == HistoryAction.SendMail)
                .Where(x => !string.IsNullOrEmpty(x.MailContent)).FirstOrDefault();
                var mailContent = JsonSerializer.Deserialize<MailPreviewInfoDto>(firstContent.MailContent);
                await _contractManager.SendMail(new Contracts.Dto.SendMailDto { ContractId = contractSetting.ContractId, MailContent = mailContent });
            }
            await CompleteContract(contractSetting.ContractId);

            return contractBase64;
        }
        public async Task<string> SignProcess(SigningDto input, string contractBase64, Guid guid)
        {
            if (input.SignartureBase64 == default && input.SignerSignatureSettingId == default)
            {
                throw new UserFriendlyException("Bạn chưa chọn chữ kí");
            }

            var setting = WorkScope.GetAll<SignerSignatureSetting>()
                .Include(x => x.ContractSetting)
                .Include(x => x.ContractSetting.Contract)
                .Where(x => x.Id == input.SignerSignatureSettingId)
                .ToList()
                .Select(x => new
                {
                    ContractId = x.ContractSetting.ContractId,
                    PositionX = x.PositionX / 2,
                    PositionY = x.PositionY / 2,
                    Width = x.Width / 2,
                    Height = x.Height / 2,
                    Page = x.Page,
                    ContractGuid = x.ContractSetting.Contract.ContractGuid.HasValue ? x.ContractSetting.Contract.ContractGuid : null
                })
                .FirstOrDefault();

            var signPositionY = (input.PageHeight / 2) - setting.PositionY;

            string signPostion = GetSignPosition(setting.PositionX, signPositionY, setting.Width.Value, setting.Height.Value);

            string signatureBase64 = "";

            if (input.SignatureUserId != null)
            {
                signatureBase64 = WorkScope.GetAll<SignatureUser>()
                   .Where(x => x.Id == input.SignerSignatureSettingId)
                   .Select(x => x.FileBase64)
                   .FirstOrDefault();
            }
            else
            {
                signatureBase64 = input.SignartureBase64;
            }

            var result = "";

            switch (input.SignatureType)
            {
                case SignatureTypeSetting.Stamp:
                case SignatureTypeSetting.Electronic:
                    {
                        string signatureId = $"signature {guid}";

                        var metadata = new Dictionary<string, string>();
                        metadata.Add(PDFSigner.VISIBLE_SIGNATURE_RECTANGLE, signPostion);
                        metadata.Add(PDFSigner.VISIBLE_SIGNATURE_FIELD_NAME, signatureId);
                        metadata.Add(PDFSigner.VISIBLE_SIGNATURE_PAGE, setting.Page.ToString());
                        metadata.Add(PDFSigner.VISIBLE_SIGNATURE_CUSTOM_IMAGE_BASE64, signatureBase64.Split(",")[1]);

                        var dto = new SignProcessDto
                        {
                            Data = contractBase64,
                            Encoding = "base64",
                            WorkerName = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.DefaultPDFSignerName),
                            ProcessType = "signDocument",
                            RequestMetadata = metadata,
                        };
                        result = _signServerWebService.SignProcess(dto);
                        break;
                    }
                case SignatureTypeSetting.Digital:
                    break;

                case SignatureTypeSetting.Acronym:
                    break;

                case SignatureTypeSetting.Text:
                    break;

                case SignatureTypeSetting.DatePicker:
                    break;

                case SignatureTypeSetting.Dropdown:
                    break;

                default:
                    break;
            }

            return result;
        }

        public async Task UpdateSignedSignature(long contractSettingId)
        {
            var SignedSettingIds = WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => x.ContractSettingId == contractSettingId)
                .Select(x => x.Id)
                .ToList();

            var signatureSetting = WorkScope.GetAll<SignerSignatureSetting>()
                .Where(x => SignedSettingIds.Contains(x.Id));

            foreach (var item in signatureSetting)
            {
                item.IsSigned = true;
                await WorkScope.UpdateAsync(item);
            }
        }
        public object ValidContract(long contractId)
        {
            string message = "";
            bool isValid = true;

            var contract = WorkScope.GetAll<Entities.Contract>()
                        .Where(x => x.Id == contractId)
                        .FirstOrDefault();

            if (contract == default)
            {
                message = "Không tìm thấy hợp đồng";
                isValid = false;
            }

            if (contract.Status == Constants.Enum.ContractStatus.Cancelled)
            {
                message = "Hợp đồng đã bị hủy";
                isValid = false;
            }

            return new
            {
                message = message,
                isValid = isValid
            };
        }

        public bool ValidEmail(ValidEmailDto input)
        {
            string userEmail = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == input.ContractSettingId)
                .Select(x => x.SignerEmail)
                .FirstOrDefault();
            var isOwner = WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == input.ContractSettingId).Any(x => x.Contract.User.EmailAddress.ToLower().Trim() == input.Email.ToLower().Trim());
            if (userEmail.ToLower().Trim() == input.Email.ToLower().Trim() || isOwner)
            {
                return true;
            }

            return false;
        }
    }
}