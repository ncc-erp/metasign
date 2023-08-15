using Abp.UI;
using Castle.Core.Logging;
using EC.Constants.Wokers;
using EC.WebService.Goggle.Dto;
using EC.WebService.PDFConverter.Dto;
using EC.WebService.SignServer.Dto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EC.WebService
{
    public abstract class BaseWebService
    {
        private readonly HttpClient _httpClient;
        protected readonly ILogger _logger;

        protected BaseWebService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = NullLogger.Instance;
        }

        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var fullUrl = $"{_httpClient.BaseAddress}{url}";
            try
            {
                //_logger.Info($"Get: {fullUrl}");
                var response = await _httpClient.GetAsync(url);

                var responseContent = await response.Content.ReadAsStringAsync();
                //_logger.Info($"Get: {fullUrl} response: {responseContent}");
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                //_logger.Error($"Get: {fullUrl} error: {ex.Message}");
            }
            return default;
        }

        protected virtual async Task<T> PostAsyncGoogleCapcha<T>(string url, GoogleCapchaVerifyDto input)
        {
            var fullUrl = $"{_httpClient.BaseAddress}{url}";
            var strInput = $"secret={input.secret}&response={input.response}";
            var contentString = new StringContent(strInput, Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                var response = await _httpClient.PostAsync(url, contentString);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Post: {fullUrl} error: {ex.Message}");
            }
            return default;
        }

        protected virtual async Task<string> PostAsyncSignserverProcess<T>(string url, SignProcessDto input)
        {
            var payload = new Dictionary<string, string>();
            payload.Add("data", input.Data.Split(",")[1]);
            payload.Add("encoding", input.Encoding);
            payload.Add("workerName", input.WorkerName);
            payload.Add("processType", input.ProcessType);
            payload.Add(PDFSigner.REQUEST_METADATA, input.GetRequestMetadata());

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(payload));

            if (response.IsSuccessStatusCode)
            {
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType == "application/pdf")
                {
                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    return "data:application/pdf;base64," + Convert.ToBase64String(pdfBytes);
                }
            }
            else
            {
                throw new UserFriendlyException($"Failed to call signServer with status: {response.StatusCode}");
            }
            return default;
        }

        protected virtual async Task<T> PostAsyncXML<T>(string xmlBody)
        {
            string apiUrl = "https://172.16.100.158:8443/signserver/AdminWSService/AdminWS";
            string certificatePath = @"C:\Users\xuan.vuduy.ncc\Desktop\NCC_Soft.p12";
            string certificatePassword = "123";

            X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword);
            HttpClientHandler handler = new HttpClientHandler();
            X509Certificate2Collection certificates = new X509Certificate2Collection(certificate);

            handler.ClientCertificates.AddRange(certificates);
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using (var httpClient = new HttpClient(handler))
            {
                var contentString = new StringContent(xmlBody, Encoding.UTF8, "text/xml");
                var response = await httpClient.PostAsync(apiUrl, contentString);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = ConvertXMLToObject(responseBody);

                    return JsonConvert.DeserializeObject<T>(result);
                }

                return default;
            }
        }

        public void Post(string url, object input)
        {
            var fullUrl = $"{_httpClient.BaseAddress}/{url}";
            string strInput = JsonConvert.SerializeObject(input);
            try
            {
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");

                _logger.Info($"Post: {fullUrl} input: {strInput}");

                _httpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
                _logger.Error($"Post: {fullUrl} input: {strInput} Error: {e.Message}");
            }
        }

        public dynamic ConvertXMLToObject(string responseXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseXml);

            XmlNode returnNode = xmlDoc.SelectSingleNode("//return");

            if (returnNode != null)
            {
                return returnNode.InnerText;
            }
            return "";
        }

        public async Task<string> UploadAndConvert(IFormFile file, string url)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("2PDF"), "convtype");
            formData.Add(new StringContent("image/bmp,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,image/gif,text/html,text/html,image/jpeg,image/jpeg,text/plain,message/rfc822,message/rfc822,application/vnd.oasis.opendocument.formula,application/vnd.oasis.opendocument.presentation,application/vnd.oasis.opendocument.spreadsheet,application/vnd.oasis.opendocument.text,image/png,application/mspowerpoint,application/rtf,image/tif,image/tif,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation,application/x-mspublisher,text/plain"), "extesion");
            formData.Add(new StreamContent(file.OpenReadStream()), "file[0]", file.FileName);

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{_httpClient.BaseAddress}/conversion/upload_wp.php", formData);

                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.Debug("Response:");
                _logger.Debug(responseBody);
                var convertRes = JsonConvert.DeserializeObject<PDFConvertResult>(responseBody);
                await GetConvertJobStatus(convertRes.JobId);
                return await GetConvertFile(convertRes.JobId);
            }
        }

        public async Task<PDFConvertJobResult> GetConvertJobStatus(string jobId)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{_httpClient.BaseAddress}/conversion/getIsConverted.php?jobId={jobId}");
                var responseBody = await response.Content.ReadAsStringAsync();
                var jobRes = JsonConvert.DeserializeObject<PDFConvertJobResult>(responseBody);
                while (string.IsNullOrEmpty(jobRes.download_url))
                {
                    jobRes = await GetConvertJobStatus(jobId);
                }
                //await GetConvertFile(jobId);
                return jobRes;
            }
        }

        public async Task<string> GetConvertFile(string jobId)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{_httpClient.BaseAddress}/conversion/fetch.php?job_id={jobId}");
                var responseBody = await response.Content.ReadAsStreamAsync();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    responseBody.CopyTo(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }
}