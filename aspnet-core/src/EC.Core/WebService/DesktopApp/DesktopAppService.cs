using Abp.UI;
using EC.WebService.DesktopApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService.DesktopApp
{
    public class DesktopAppService: BaseWebService
    {
        public DesktopAppService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<GetCertificateInfo> GetCertInfo()
        {
          var result = await GetAsync<GetCertificateInfo>("api/Sign/GetCertInfor");

            if (result == null)
            {
                result = new GetCertificateInfo();
                result.IsSuccess = false;
            }
            else
            {
                result.IsSuccess = true;
            }

            return result;
        }
    }
}
