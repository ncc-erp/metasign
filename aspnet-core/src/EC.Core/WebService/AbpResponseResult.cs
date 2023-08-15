using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService
{
    public class AbpResponseResult<T> where T : class
    {
        [JsonProperty("result")]
        public T Result { get; set; }
        [JsonProperty("targetUrl")]
        public string TargetUrl { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error")]
        public AbpErrorResponse Error { get; set; }
        [JsonProperty("unAuthorizedRequest")]
        public bool UnAuthorizeRequest { get; set; }
        [JsonProperty("__abp")]
        public bool Abp { get; set; }
    }
    public class AbpErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
