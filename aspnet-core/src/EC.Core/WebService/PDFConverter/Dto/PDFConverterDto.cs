using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EC.WebService.PDFConverter.Dto
{
    public class PDFConverterDto
    {
        [JsonProperty("convtype")]
        public string convtype { get; set; }
        [JsonProperty("extesion")]
        public string extesion { get; set; }
        [JsonProperty("file[0]")]
        public IFormFile file { get; set; }
    }

    public class PDFConvertResult
    {
        public int Status { get; set; }
        public string JobId { get; set; }
    }

    public class PDFConvertJobResult
    {
        public string Status { get; set; }
        public string download_url { get; set; }
    }
}