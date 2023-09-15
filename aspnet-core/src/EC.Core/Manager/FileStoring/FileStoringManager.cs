using EC.Authorization.Users;
using EC.Entities;
using EC.FileStoringServices;
using EC.Manager.Contracts;
using EC.Manager.Contracts.Dto;
using EC.Manager.ContractSignings.Dto;
using HRMv2.NccCore;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.FileStoring
{
    public class FileStoringManager : BaseManager
    {
        private readonly FileStoringService fileStoringService;
        private readonly IWebHostEnvironment webHostEnvironment;

        private const string BASE64_PREFIX = "data:application/pdf;base64,";
        private const string PDF_EXTENSION = "pdf";
        private const string ZIP_EXTENSION = "zip";

        public FileStoringManager(IWorkScope workScope,
            FileStoringService fileStorageServices,
            IWebHostEnvironment webHostEnvironment) : base(workScope)
        {
            this.fileStoringService = fileStorageServices;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task WaitForUploadingFile(FileCategory fileCategory, string guid, int index, string fileName)
        {
            int count = 0;
            do
            {
                count = (await fileStoringService.SearchForFiles(fileCategory, guid, index, fileName)).Count;
            } while (count == 0);
        }

        public async Task WaitForDeletingFiles(FileCategory fileCategory, string guid, int? index, string? fileName)
        {
            int count;
            do
            {
                count = (await fileStoringService.SearchForFiles(fileCategory, guid, index, fileName)).Count;
            } while (count != 0);
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
                            ZipArchiveEntry entry = zipArchive.CreateEntry(file.FileName, CompressionLevel.Optimal);
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

        public async Task<string> RenderCertificatePdf(CertificateDto certificate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

            var outputFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "certificatePdf", $"Certificate_{certificate.ContractId}.pdf");

            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 10, 10, 20, 20);

            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputFilePath, FileMode.Create));

            document.Open();

            PdfPTable table = new PdfPTable(3);
            float[] columnWidths = { 45f, 25f, 30f };
            table.SetWidths(columnWidths);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            string fontPath = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "font", "times.ttf");
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
                if (!String.IsNullOrEmpty(lastContractSignature.SigningResult))
                {
                    base64Contract = lastContractSignature.SigningResult.Split(',')[1];
                }
            }

            if (base64Contract == null || base64Contract.Length == 0)
            {
                byte[] fileBytes = await DownloadLatestContract(contract.Id);
                base64Contract = Convert.ToBase64String(fileBytes);
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

        public async Task<string> GetPresignedDownloadUrl(DownloadContractAndCertificateDto input)
        {
            var contract = WorkScope.GetAll<Contract>()
                .First(x => x.Id.Equals(input.ContractId));
            var fileNameOnly = Path.GetFileNameWithoutExtension(contract.File);
            string fileBase64 = await DownloadContractAndCertificate(input);
            byte[] fileBytes = Convert.FromBase64String(fileBase64.Substring(fileBase64.IndexOf(',') + 1));

            var stream = new MemoryStream(fileBytes);
            // Index is used for multiple files
            // if only has 1 file, index will be 1
            int index = 1;
            List<string> searchResult = await fileStoringService.SearchForFiles(FileCategory.Download, contract.ContractGuid.ToString(), index, null);

            switch (input.DownloadType)
            {
                case DownloadContractType.Contract:
                    if (!searchResult.Any(x => x.Contains(fileNameOnly + '.' + PDF_EXTENSION)))
                    {
                        IFormFile file = new FormFile(stream, 0, stream.Length, fileNameOnly, fileNameOnly + '.' + PDF_EXTENSION);
                        await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                    }
                    await WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + '.' + PDF_EXTENSION);
                    return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + '.' + PDF_EXTENSION);

                case DownloadContractType.Certificate:
                    if (!searchResult.Any(x => x.Contains("Certificate.pdf")))
                    {
                        IFormFile file = new FormFile(stream, 0, stream.Length, "Certificate", "Certificate.pdf");
                        await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                    }
                    await WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, "Certificate.pdf");
                    return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, "Certificate.pdf");

                default:
                    if (!searchResult.Any(x => x.Contains(fileNameOnly + '.' + ZIP_EXTENSION)))
                    {
                        IFormFile file = new FormFile(stream, 0, stream.Length, fileNameOnly, fileNameOnly + '.' + ZIP_EXTENSION);
                        await fileStoringService.UploadFile(file, FileCategory.Download, contract.ContractGuid.ToString(), index);
                    }
                    await WaitForUploadingFile(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + '.' + ZIP_EXTENSION);
                    return await fileStoringService.GetDownloadUrl(FileCategory.Download, contract.ContractGuid.ToString(), index, fileNameOnly + '.' + ZIP_EXTENSION);
            }
        }

        private async Task<string> GetContractGuid(long contractId)
        {
            return WorkScope.GetAll<Contract>()
                .First(x => x.Id.Equals(contractId))
                .ContractGuid.ToString();
        }

        public async Task ClearContractDownloadFiles(long contractId)
        {
            string contractGuid = await GetContractGuid(contractId);
            if ((await fileStoringService.SearchForFiles(FileCategory.Download, contractGuid, null, null)).Count > 0)
            {
                await fileStoringService.DeleteMultipleFiles(FileCategory.Download, contractGuid);
                await WaitForDeletingFiles(FileCategory.Download, contractGuid, null, null);
            }
        }

        public async Task UploadUnsignedContract(long contractId, IFormFile file)
        {
            string contractGuid = await GetContractGuid(contractId);
            await fileStoringService.UploadFile(file, FileCategory.UnsignedContract, contractGuid.ToString(), 1);
        }

        public async Task<string> DownloadUnsignedContractBase64(long contractId)
        {
            string contractGuid = await GetContractGuid(contractId);
            string fileName = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .FirstOrDefault()
                .File;

            byte[] fileBytes = await fileStoringService.DownloadFile(FileCategory.UnsignedContract, contractGuid, 1, fileName);
            return (BASE64_PREFIX + Convert.ToBase64String(fileBytes));
        }

        public async Task DeleteUnsignedContract(long contractId)
        {
            string contractGuid = await GetContractGuid(contractId);
            await fileStoringService.DeleteMultipleFiles(FileCategory.UnsignedContract, contractGuid);
        }

        public async Task UploadContract(long contractId, IFormFile file)
        {
            string contractGuid = await GetContractGuid(contractId);
            int index = (await fileStoringService.SearchForFiles(FileCategory.SignedContract, contractGuid.ToString(), null, null)).Count + 1;
            await fileStoringService.UploadFile(file, FileCategory.SignedContract, contractGuid.ToString(), index);
        }

        public async Task<byte[]> DownloadLatestContract(long contractId)
        {
            string contractGuid = await GetContractGuid(contractId);
            string fileName = WorkScope.GetAll<Contract>()
                .Where(x => x.Id == contractId)
                .FirstOrDefault()
                .File;

            List<string> contractList = await fileStoringService.SearchForFiles(FileCategory.SignedContract, contractGuid, null, null);
            if (contractList.Count == 0)
            {
                return await fileStoringService.DownloadFile(FileCategory.UnsignedContract, contractGuid.ToString(), 1, fileName);
            } 
            else
            {
                int index = contractList.Count;
                return await fileStoringService.DownloadFile(FileCategory.SignedContract, contractGuid.ToString(), index, fileName);
            }
        }

        public async Task<string> DownloadLatestContractBase64(long contractId)
        {
            byte[] fileBytes = await DownloadLatestContract(contractId);
            return (BASE64_PREFIX + Convert.ToBase64String(fileBytes));
        }
    }
}
