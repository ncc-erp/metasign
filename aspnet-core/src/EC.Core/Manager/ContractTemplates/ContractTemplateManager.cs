using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.UI;
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
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;
using static iTextSharp.text.pdf.AcroFields;

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

        public static DateTime? TryParse(string text) => DateTime.TryParse(text, out var date) ? date : (DateTime?)null;

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

        public async Task<FileBase64Dto> DownloadMassTemplate(long templateId)
        {
            var data = await Get(templateId);
            if (data.SignerSettings.Count(x => x.ContractRole == ContractRole.Signer) == 0)
            {
                throw new UserFriendlyException("Not have signer yet!");
            }
            using (var excelPackage = new ExcelPackage())
            {
                var sheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                sheet.Cells["A1:A2"].Merge = true;
                sheet.Cells["A1:A2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheet.Cells["A1:A2"].Value = "No.";
                sheet.Cells["B1:B2"].Merge = true;
                sheet.Cells["B1:B2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheet.Cells["B1:B2"].Value = "Contract Name (*)";
                sheet.Cells["C1:C2"].Merge = true;
                sheet.Cells["C1:C2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheet.Cells["C1:C2"].Value = "Contract Code";
                sheet.Cells["D1:D2"].Merge = true;
                sheet.Cells["D1:D2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheet.Cells["D1:D2"].Value = "Expire Time";
                var startCol = 5;
                var startRow = 1;
                foreach (var item in data.SignerSettings)
                {
                    sheet.Cells[startRow, startCol, startRow, startCol + 1].Merge = true;
                    if (data.SignerSettings.IndexOf(item) == 0)
                    {
                        sheet.Names.Add("StartSigner", sheet.Cells[startRow, startCol]);
                    }
                    if (data.SignerSettings.IndexOf(item) == data.SignerSettings.Count() - 1)
                    {
                        sheet.Names.Add("EndSigner", sheet.Cells[startRow, startCol]);
                    }
                    var role = string.IsNullOrEmpty(item.Role)
                        ? Enum.GetName(typeof(ContractRole), item.ContractRole) : item.Role;
                    sheet.Cells[startRow, startCol].Value = role;
                    sheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[startRow + 1, startCol].Value = $"Name ( {role} ) (*)";
                    sheet.Cells[startRow + 1, startCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[startRow + 1, startCol + 1].Value = $"Email ( {role} ) (*)";
                    sheet.Cells[startRow + 1, startCol + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    startCol += 2;
                }
                for (var index = 0; index < data.ContractTemplate.ListField.Count; index++)
                {
                    sheet.Cells[startRow, startCol, startRow + 1, startCol].Merge = true;
                    sheet.Cells[startRow, startCol, startRow + 1, startCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[startRow, startCol].Value = data.ContractTemplate.ListField[index];
                    if (index == 0)
                    {
                        sheet.Names.Add("StartListField", sheet.Cells[startRow, startCol]);
                    }
                    if (index == data.ContractTemplate.ListField.Count() - 1)
                    {
                        sheet.Names.Add("EndListField", sheet.Cells[startRow, startCol]);
                    }
                    startCol++;
                }
                sheet.Cells[1, 1, 2, startCol - 1].Style.Font.Bold = true;
                sheet.Cells[1, 1, 2, startCol - 1].Style.Locked = true;
                excelPackage.Workbook.CreateVBAProject();
                excelPackage.Workbook.CodeModule.Name = "Module1";
                excelPackage.Workbook.CodeModule.Code = new StringBuilder().AppendLine("Private Sub Workbook_Open()")
                                              .AppendLine("\tDim ws As Worksheet")
                                              .AppendLine("\tOn Error Resume Next ' In case there are no worksheets")
                                              .AppendLine("\tFor Each ws In ThisWorkbook.Worksheets")
                                              .AppendLine("\t\tws.Cells.EntireColumn.Autofit")
                                              .AppendLine("\t\tws.Cells.EntireRow.Autofit")
                                              .AppendLine("\t\tws.Cells.HorizontalAlignment = xlCenter")
                                              .AppendLine("\t\tws.Cells.VerticalAlignment = xlCenter")
                                              .AppendLine("\tNext ws")
                                              .AppendLine("\tOn Error GoTo 0 ' Reset error handling")
                                              .AppendLine("End Sub")
                                              .AppendLine("Private Sub Workbook_SheetSelectionChange(ByVal Sh As Object, ByVal Target As Range)")
                                              .AppendLine("\tCells.EntireColumn.Autofit")
                                              .AppendLine("\tCells.EntireRow.Autofit")
                                              .AppendLine("\tCells.HorizontalAlignment = xlCenter")
                                              .AppendLine("\tCells.VerticalAlignment = xlCenter")
                                              .AppendLine("End Sub").ToString();

                using (var memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    return new FileBase64Dto
                    {
                        Base64 = Convert.ToBase64String(memoryStream.ToArray()),
                        FileName = $"{data.ContractTemplate.Name}_template.xlsm",
                        FileType = "application/vnd.ms-excel.sheet.macroEnabled.12"
                    };
                }
            }
        }

        public async Task<GetSignatureForContracttemplateDto> Get(long id)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
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
                    MassType = item.MassType,
                    MassWordContent = item.MassWordContent,
                    MassField = item.MassField
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
                    .OrderBy(x => x.ContractRole).ThenBy(x => x.ProcesOrder)
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

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
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
            item.MassWordContent = input.MassWordContent;
            item.MassField = input.MassField;
            await WorkScope.UpdateAsync(item);
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

        public async Task<ValidImportMassContractTemplateDto> ValidImportMassTemplate(UploadMassTemplateFileDto input)
        {
            try
            {
                var templateData = await Get(input.TemplateId);
                using (var fileStream = new MemoryStream())
                {
                    input.File.CopyTo(fileStream);
                    using (var package = new ExcelPackage(fileStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var sheet = package.Workbook.Worksheets[0];
                        var rowCount = sheet.Dimension.End.Row;
                        var colCount = sheet.Dimension.End.Column;
                        if (rowCount <= 2)
                        {
                            throw new UserFriendlyException("Data column is missed");
                        }
                        var result = new ValidImportMassContractTemplateDto()
                        {
                            FailList = new List<ResponseFailDto>(),
                            SuccessList = new List<RowMassTemplateExportDto>()
                        };
                        for (int row = 3; row <= rowCount; row++)
                        {
                            var contractName = sheet.Cells[row, 2].GetCellValue<string>();
                            if (contractName == null)
                            { result.FailList.Add(new ResponseFailDto { Address = sheet.Cells[row, 2].Address, ReasonFail = "ContractNameIsEmpty" }); }
                            var contractCode = sheet.Cells[row, 3].GetCellValue<string>();
                            DateTime? expireTime = null;
                            if (string.IsNullOrEmpty(sheet.Cells[row, 4].GetCellValue<string>()))
                            {
                            }
                            if (!string.IsNullOrEmpty(sheet.Cells[row, 4].GetCellValue<string>()))
                            {
                                expireTime = DateTime.TryParse(sheet.Cells[row, 4].GetCellValue<string>(), out var dt) ? dt : (DateTime?)null;
                            }
                            result.SuccessList.Add(new RowMassTemplateExportDto { Contract = new CreatECDto { Name = contractName, Code = contractCode, ExpriedTime = expireTime, } });
                        }
                        var startColSigner = sheet.Names["StartSigner"].EntireColumn.StartColumn;
                        var endColSigner = sheet.Names["EndSigner"].EntireColumn.StartColumn;
                        if ((endColSigner - startColSigner) / 2 + 1 != templateData.SignerSettings.Count)
                        {
                            throw new UserFriendlyException("Wrong Template!");
                        }
                        var checkMultiple = new List<CheckMultiple>();
                        for (int col = 0; col <= endColSigner - startColSigner; col += 2)
                        {
                            var colCheck = new CheckMultiple() { NameEmails = new List<NameEmail>() };
                            for (int row = 3; row <= rowCount; row++)
                            {
                                var name = sheet.Cells[row, col + startColSigner].GetCellValue<string>();
                                var email = sheet.Cells[row, col + startColSigner + 1].GetCellValue<string>();
                                colCheck.NameEmails.Add(new NameEmail
                                {
                                    Name = string.IsNullOrEmpty(name) ? "" : name.Trim(),
                                    Email = string.IsNullOrEmpty(email) ? "" : email.Trim(),
                                });
                            }
                            colCheck.MassGuid = colCheck.NameEmails.GroupBy(x => new { x.Name, x.Email }).Count() == 1 ? Guid.NewGuid() : null;
                            checkMultiple.Add(colCheck);
                        }
                        for (int row = 3; row <= rowCount; row++)
                        {
                            var startSigner = startColSigner;
                            var signers = new List<SignerMassDto>();
                            result.SuccessList[row - 3].Signers = new List<SignerMassDto>();
                            while (startSigner <= endColSigner)
                            {
                                var signerName = sheet.Cells[row, startSigner].GetCellValue<string>();
                                if (string.IsNullOrEmpty(signerName))
                                {
                                    result.FailList.Add(new ResponseFailDto { Address = sheet.Cells[row, startSigner].Address, ReasonFail = "NameIsEmpty" });
                                }
                                var signerEmail = sheet.Cells[row, startSigner + 1].GetCellValue<string>();
                                if (!CommonUtils.IsValidEmail(signerEmail))
                                {
                                    result.FailList.Add(new ResponseFailDto { Address = sheet.Cells[row, startSigner + 1].Address, ReasonFail = "EmailIsNotValid" });
                                }
                                signers.Add(new SignerMassDto
                                {
                                    Name = signerName,
                                    Email = signerEmail,
                                    Color = templateData.SignerSettings[(startSigner - startColSigner) / 2].Color,
                                    ContractRole = templateData.SignerSettings[(startSigner - startColSigner) / 2].ContractRole,
                                    ProcesOrder = templateData.SignerSettings[(startSigner - startColSigner) / 2].ProcesOrder,
                                    SignatureSettings = templateData.SignatureSettings.Where(x => x.ContractTemplateSignerId == templateData.SignerSettings[(startSigner - startColSigner) / 2].Id).ToList(),
                                    MassGuid = checkMultiple[(startSigner - startColSigner) / 2].MassGuid
                                });
                                startSigner += 2;
                            }
                            result.SuccessList[row - 3].Signers.AddRange(signers);
                        }
                        var startColListField = sheet.Names["StartListField"].EntireColumn.StartColumn;
                        var endColListField = sheet.Names["EndListField"].EntireColumn.StartColumn;
                        if ((endColListField - startColListField) + 1 != templateData.ContractTemplate.ListField.Count)
                        {
                            throw new UserFriendlyException("Wrong Template!");
                        }
                        for (int row = 3; row <= rowCount; row++)
                        {
                            var startListField = startColListField;
                            result.SuccessList[row - 3].ListFieldDto = new List<string>();
                            var listField = new List<string>();
                            while (startListField <= endColListField)
                            {
                                var fieldValue = sheet.Cells[row, startListField].GetCellValue<string>();
                                if (string.IsNullOrEmpty(fieldValue))
                                {
                                    result.FailList.Add(new ResponseFailDto { Address = sheet.Cells[row, startListField].Address, ReasonFail = "AutoFillFieldValueIsEmpty" });
                                }
                                startListField++;
                                listField.Add(fieldValue);
                            }
                            result.SuccessList[row - 3].ListFieldDto.AddRange(listField);
                        }
                        return result;
                    }
                }
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
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
    }
}