using Abp.Timing;
using ChromiumHtmlToPdfLib;
using EC.Entities;
using EC.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static EC.Constants.Enum;

namespace EC.Utils
{
    public class CommonUtils
    {
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

        public static List<string> SupportFile = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".txt", ".json", ".html" };

        public static string ReplaceBodyMessage(string body, ContractMailTemplateDto content)
        {
            var expireTimeTag = content.ExpireTime.HasValue ? $"<h3>Thời hạn ký: {content.ExpireTime.Value.ToString("dd/MM/yyyy")}</h3>" : "";
            var newString = body.Replace("{{SendToEmail}}", $" {content.SendToName} ({content.SendToEmail}) " )
                .Replace("{{SignUrl}}", content.SignUrl)
                .Replace("{{AuthorEmail}}",$" {content.AuthorName} ({content.AuthorEmail}) ")
                .Replace("{{ContractCode}}</h3>",$"{content.ContractCode}</h3>{expireTimeTag}<h3>ID: {content.ContractGuid}</h3><h3>Nhấn vào <a href=\"{content.LookupUrl}\">đây</a> để tra cứu hợp đồng.</h3>");
            return newString;
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

        public static List<SignatureTypeSetting> InputSignature = new List<SignatureTypeSetting> { SignatureTypeSetting.Text, SignatureTypeSetting.DatePicker, SignatureTypeSetting.Acronym, SignatureTypeSetting.Dropdown };
        public static List<SignatureTypeSetting> SigningSignature = new List<SignatureTypeSetting> { SignatureTypeSetting.Electronic, SignatureTypeSetting.Digital ,SignatureTypeSetting.Stamp};
    }
}