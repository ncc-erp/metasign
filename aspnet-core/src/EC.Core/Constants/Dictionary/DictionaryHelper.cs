using EC.Manager.Notifications.Templates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Constants.Dictionary
{
    public class DictionaryHelper
    {
        public static readonly Dictionary<string, string[]> FileTypeDic =
    new Dictionary<string, string[]>()
    {
                {"IMAGE", new string[] { "jpeg", "png", "svg", "jpg"} }
    };

        public static readonly Dictionary<MailFuncEnum, MailInfoDto> SeedMailDic = new Dictionary<MailFuncEnum, MailInfoDto>()
        {
            {
                MailFuncEnum.Signing,
                new MailInfoDto
                {
                    Name = "Signing mail",
                    Description = "",
                    Subject = "[NCC]_Contract_mail",
                }
            },
        };
    }
}
