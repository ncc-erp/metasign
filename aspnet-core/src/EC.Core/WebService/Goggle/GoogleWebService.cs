using EC.WebService.Goggle.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService.Goggle
{
    public class GoogleWebService : BaseWebService
    {
        public GoogleWebService(HttpClient httpClient) : base(httpClient)
        {

        }

        public async Task<VerifyCapchaResponseDto> VerifyCapcha(string token)
        {
            var dto = new GoogleCapchaVerifyDto
            {
                secret = "6LfLFM4lAAAAAOpWAFUFTkfpozgZSs76CJAwtn65",
                response = token
            };

            return await PostAsyncGoogleCapcha<VerifyCapchaResponseDto>("recaptcha/api/siteverify", dto);
        }
    }
}
