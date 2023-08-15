using EC.WebService.PDFConverter.Dto;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace EC.WebService.PDFConverter
{
    public class PDFConverterWebService : BaseWebService
    {
        public PDFConverterWebService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<string> UploadAndConvert(IFormFile file)
        {
            return await UploadAndConvert(file,"");
        }
    }
}