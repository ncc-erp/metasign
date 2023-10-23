using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ApiKeys.Dto
{
    public class ApiKeyDto
    {
        public long UserId { get; set; }
        public string Value { get; set; }
    }

    [AutoMapTo(typeof(ApiKey))]
    public class CreateApiKeyDto
    {
        public long UserId { get; set; }
        public string Value { get; set; }
    }
}
