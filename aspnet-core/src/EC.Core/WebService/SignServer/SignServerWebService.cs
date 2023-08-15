using EC.Configuration;
using EC.Manager.SignServerWorkers.Dto;
using EC.WebService.SignServer.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EC.Manager.SignServerWorkers;

namespace EC.WebService.SignServer
{
    public class SignServerWebService : BaseWebService
    {
        private readonly SignServerWorkerManager _signServerWorkerManager;

        public SignServerWebService( SignServerWorkerManager signServerWorkerManager, HttpClient httpClient) : base(httpClient)
        {
            _signServerWorkerManager = signServerWorkerManager;
        }

        public string SignProcess(SignProcessDto input)
        {
            return PostAsyncSignserverProcess<string>("process", input).Result;
        }

        public string GetWorkerId(string workerName)
        {
            var xmlBody = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                    "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:adm=\"http://adminws.signserver.org/\">\n" +
                    "  <soapenv:Header/>\n" +
                    "  <soapenv:Body>\n" +
                    "    <adm:getWorkerId>\n" +
                    $"      <workerName>{workerName}</workerName>\n" +
                    "    </adm:getWorkerId>\n" +
                    "  </soapenv:Body>\n" +
                    "</soapenv:Envelope>";

            return _signServerWorkerManager.PostAsyncXML<string>(xmlBody).Result;
        }

        public string GetWorkerConfig(string workerId)
        {
            var xmlBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:adm=\"http://adminws.signserver.org/\">\n" +
                            "  <soapenv:Header/>\n" +
                            "  <soapenv:Body>\n" +
                            "    <adm:getCurrentWorkerConfig>\n" +
                            $"      <workerId>{workerId}</workerId>\n" +
                            "    </adm:getCurrentWorkerConfig>\n" +
                            "  </soapenv:Body>\n" +
                            "</soapenv:Envelope>";

            return _signServerWorkerManager.PostAsyncXML<string>(xmlBody).Result;
        }

        public string SetWorkerProperty(SetWorkerPropertyDto input)
        {
            var xmlBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:adm=\"http://adminws.signserver.org/\">\n" +
                            "  <soapenv:Header/>\n" +
                            "  <soapenv:Body>\n" +
                            "    <adm:setWorkerProperty>\n" +
                            $"      <workerId>{input.WorkerId}</workerId>\n" +
                            $"      <key>{input.Key}</key>\n" +
                            $"      <value>{input.Value}</value>\n" +
                            "    </adm:setWorkerProperty>\n" +
                            "  </soapenv:Body>\n" +
                            "</soapenv:Envelope>";

            return _signServerWorkerManager.PostAsyncXML<string>(xmlBody).Result;
        }
        public string RemoveWokerProperty(string workerId, string property)
        {
            var xmlBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                            $"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:adm=\"http://adminws.signserver.org/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <adm:removeWorkerProperty>\r\n         <workerId>{workerId}</workerId>\r\n         <!--Optional:-->\r\n         <key>{property}</key>\r\n      </adm:removeWorkerProperty>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>" +
                            "</soapenv:Envelope>";

            return _signServerWorkerManager.PostAsyncXML<string>(xmlBody).Result;
        }

        public string ReloadWorker(string workerId)
        {
            var xmlBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                            $"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:adm=\"http://adminws.signserver.org/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <adm:reloadConfiguration>\r\n         <workerId>{workerId}</workerId>\r\n      </adm:reloadConfiguration>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>" +
                            "</soapenv:Envelope>";

            return _signServerWorkerManager.PostAsyncXML<string>(xmlBody).Result;
        }

     
    }
}
