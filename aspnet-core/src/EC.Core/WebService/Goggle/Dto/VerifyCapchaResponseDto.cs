using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService.Goggle.Dto
{
    public class VerifyCapchaResponseDto
    {
        public string success { get; set; }
        public string challenge_ts { get; set; }
        public string apk_package_name { get; set; }
        [JsonProperty("error-codes")]
        public string[] errorCodes { get; set; }
}
}
