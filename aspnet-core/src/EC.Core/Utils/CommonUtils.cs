using Abp.Json;
using Abp.Timing;
using ChromiumHtmlToPdfLib;
using EC.Manager.Notifications.Email.Dto;
using Microsoft.AspNetCore.Http;
using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static EC.Constants.Enum;

namespace EC.Utils
{
    public class CommonUtils
    {
        public static List<SignatureTypeSetting> InputSignature = new List<SignatureTypeSetting> { SignatureTypeSetting.Text, SignatureTypeSetting.DatePicker, SignatureTypeSetting.Acronym, SignatureTypeSetting.Dropdown };

        public static List<SignatureTypeSetting> SigningSignature = new List<SignatureTypeSetting> { SignatureTypeSetting.Electronic, SignatureTypeSetting.Digital, SignatureTypeSetting.Stamp };

        public static List<string> SupportFile = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".txt", ".json", ".html" };

        public static IFormFile ConvertBase64PdfToFile(string base64String, string fileName, string contentType = "application/pdf")
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(bytes);

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }

        public static string ConvertHtmlToPdf(string htmlContent)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var converter = new Converter();
                converter.ConvertToPdf(htmlContent, stream, new ChromiumHtmlToPdfLib.Settings.PageSettings
                {
                    PrintBackground = true,
                    Scale = 1
                });
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static string GetMatchField(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                Document doc = new Document();
                doc.LoadFromStream(ms, FileFormat.Auto);

                var matchList = new List<string>();
                foreach (Section section in doc.Sections)
                {
                    foreach (Paragraph paragraph in section.Paragraphs)
                    {
                        var matches = Regex.Matches(paragraph.Text, @"{{(.*?)}}");
                        foreach (Match match in matches)
                        {
                            string matchValue = match.Groups[0].Value;
                            matchList.Add(matchValue);
                        }
                    }
                }
                return matchList.ToJsonString();
            }
        }

        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }

        //public static string GetRectangleCoordinates(float x, float y, float width, float height)
        //{
        //    var llx = Math.Round(x);
        //    var lly = Math.Round(y) ;
        //    var urx = Math.Round(x + width);
        //    var ury = Math.Round(y + height);

        //    return string.Format("{0},{1},{2},{3}", llx, lly, urx, ury);
        //}

        public static string GetRectangleCoordinates(float x, float y, float width, float height)
        {
            var llx = Math.Round(x);
            var lly = Math.Round(y - (height));
            var urx = Math.Round(llx + width);
            var ury = Math.Round(lly + height);

            return string.Format("{0},{1},{2},{3}", llx, lly, urx, ury);
        }

        public static MailTemplateType GetTemplateType(MailFuncEnum type)
        {
            switch (type)
            {
                case MailFuncEnum.Signing:
                    return MailTemplateType.Mail;
            }
            return MailTemplateType.Mail;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) { return false; }
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email.Trim());
        }

        public static string ReplaceBodyMessage(string body, ContractMailTemplateDto content)
        {
            var expireTimeTag = content.ExpireTime.HasValue ? $"<h3>Thời hạn ký: {content.ExpireTime.Value.ToString("dd/MM/yyyy")}</h3>" : "";
            var newString = body.Replace("{{SendToEmail}}", $" {content.SendToName} ({content.SendToEmail}) ")
                .Replace("{{SignUrl}}", content.SignUrl)
                .Replace("{{AuthorEmail}}", $" {content.AuthorName} ({content.AuthorEmail}) ")
                .Replace("{{ContractCode}}</h3>", $"{content.ContractCode}</h3>{expireTimeTag}<h3>ID: {content.ContractGuid}</h3><h3>Nhấn vào <a href=\"{content.LookupUrl}\">đây</a> để tra cứu hợp đồng.</h3>");
            return newString;
        }

    }
}