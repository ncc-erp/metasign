using EC.Manager.ContractHistories.Dto;
using EC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static EC.Constants.Enum;

namespace EC.Manager.Notifications.Email.Dto
{
    public class EmailDto
    {
        public long Id { get; set; }
        public MailFuncEnum Type { get; set; }
        public string Name { get; set; }
        public string CCs { get; set; }
        public string[] ArrCCs { get => string.IsNullOrEmpty(CCs) ? new string[0] : CCs.Split(",").ToArray(); }
        public string Description { get; set; }
        public string BodyMessage { get; set; }
        public string SendToEmail { get; set; }
        public MailTemplateType TemplateType => CommonUtils.GetTemplateType(Type);
    }

    public class MailPreviewInfoDto : ICloneable
    {
        public long TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MailFuncEnum MailFuncType { get; set; }
        public string Subject { get; set; }
        public string BodyMessage { get; set; }
        public string SendToEmail { get; set; }
        public string[] PropertiesSupport { get; set; }
        public List<string> CCs { get; set; } = new List<string>();
        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
        public MailTemplateType TemplateType => CommonUtils.GetTemplateType(MailFuncType);
        public long? ContractSettingId { get; set; }
        public CreaContractHistoryDto MailHistory { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ResultTemplateEmail<T> where T : class
    {
        public T Result { get; set; }
        public string[] PropertiesSupport { get => typeof(T).GetProperties().Select(s => s.Name).ToArray(); }
    }

    public class InputSendMailToAllDto
    {
        public List<long> List { get; set; }
    }
}