using EC.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Templates
{
    public static class TemplateHelper
    {
        public static string ContentEmailTemplate(MailFuncEnum type) =>
    type switch
    {
        MailFuncEnum.Signing => @"<div style=""max-width:100%;padding:20px;font-family:arial;color:#242323;width:915px""><header><img style=""width:200px;margin-left:-5px"" src=""blob:https://metasign.nccsoft.vn/2fa297dd-0d8f-471a-bdee-e0d4eed9475d""></header><br><div style=""color:#fbf7ec;height:200px;background-color:#004037;padding:0 20px;border-radius:2px""><table style=""width:100%""><tbody><tr><td style=""width:75%;color:#fff""><h1>Dear {{SendToEmail}}</h1><h3>{{Message}}</h3><p>&nbsp;</p></td><td style=""width:25%""><img style=""margin-top:20px"" src=""https://img.freepik.com/free-vector/signing-contract-concept-illustration_114360-4879.jpg?w=2000"" width=""200""></td></tr></tbody></table></div><h3>T&ecirc;n hợp đồng: {{ContractName}}</h3><h3>M&atilde; hợp đồng: {{ContractCode}}</h3><h3>Người gửi: {{AuthorEmail}}</h3><div style=""width:100%;height:100px;border:2px dashed #dfdede""><table style=""width:100%""><tbody><tr><td style=""width:20%"">&nbsp;</td><td style=""width:20%"">&nbsp;</td><td style=""width:20%""><a style=""background-color:#ffc107;padding:12px;line-height:100px;border-radius:2px;font-size:18px;width:183px"" href=""{{SignUrl}}""><strong>Xem hợp đồng</strong></a></td><td style=""width:20%"">&nbsp;</td><td style=""width:20%"">&nbsp;</td></tr></tbody></table></div></div>",
        _ => string.Empty
    };

        public static ContractMailTemplateDto GetFakeContractMailTemplate()
        {
            return new ContractMailTemplateDto
            {
                ContractName = "Hợp đồng test",
                Message = "Anh/chị vui lòng truy cập vào đường link {{SignUrl}} và kí vào hợp đồng",
                SignUrl = "http://localhost:4200",
                SendToEmail = "xuan.vuduy@ncc.asia",
                AuthorEmail = "test@gmail.com",
                ContractCode = "123"
            };
        }
    }
}
