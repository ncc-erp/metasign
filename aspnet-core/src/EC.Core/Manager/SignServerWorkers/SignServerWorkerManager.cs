using EC.Configuration;
using EC.Manager.SignServerWorkers.Dto;
using EC.WebService.SignServer;
using HRMv2.NccCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Policy;
using EC.WebService.DesktopApp.Dto;

namespace EC.Manager.SignServerWorkers
{
    public class SignServerWorkerManager : BaseManager
    {
        private readonly IConfiguration _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SignServerWorkerManager(IWorkScope workScope, IWebHostEnvironment webHostEnvironment, IConfiguration configuration) : base(workScope)
        {
            _appConfiguration = configuration;
            _hostingEnvironment = webHostEnvironment;
        }

        public async Task<T> GetAsyncJson<T> (string url, Dictionary<String, Object> paramList)
        {
            string apiUrl = _appConfiguration.GetValue<string>("SignServerService:AdminWS");
            HttpClientHandler handler = new HttpClientHandler();
            handler = GetCertificate();

            var fullUrl = $"{apiUrl}{url}?";
            if (paramList != null && paramList.Count > 0)
                foreach (KeyValuePair<String, Object> kv in paramList)
                {
                    fullUrl += kv.Key + "=" + kv.Value.ToString() + "&";
                }

            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(fullUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(responseBody);
                    }
                    return default;
                }
            }
            catch (Exception ex)
            {

            }
            return default;
        }

        public async Task<T> PostAsyncJson<T>(string url, Dictionary<String, Object> paramList, Object body)
        {
            string apiUrl = _appConfiguration.GetValue<string>("SignServerService:AdminWS");
            HttpClientHandler handler = new HttpClientHandler();
            handler = GetCertificate();

            var fullUrl = $"{apiUrl}{url}?";
            if (paramList != null && paramList.Count > 0)
                foreach (KeyValuePair<String, Object> kv in paramList)
                {
                    fullUrl += kv.Key + "=" + kv.Value.ToString() + "&";
                }
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.PostAsync(fullUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(responseBody);
                    }
                    return default;
                }
            } 
            catch (Exception ex)
            {

            }
            return default;
        }

        public async Task<T> PostAsyncXML<T>(string xmlBody)
        {
            string apiUrl = _appConfiguration.GetValue<string>("SignServerService:AdminWS");
            HttpClientHandler handler = new HttpClientHandler();
            handler = GetCertificate();

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

        private dynamic GetCertificate()
        {
            string certificatePath = Path.Combine(_hostingEnvironment.WebRootPath, "certificate/NCC_Soft.p12");
            // string certificatePath = Path.Combine(_hostingEnvironment.WebRootPath, "certificate/MetaSign_Dev_Team.p12");
            string certificatePassword = "123";

            X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword);
            HttpClientHandler handler = new HttpClientHandler();
            X509Certificate2Collection certificates = new X509Certificate2Collection(certificate);

            handler.ClientCertificates.AddRange(certificates);
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            return handler;
        } 
    }
}
