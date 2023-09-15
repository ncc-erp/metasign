using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using EC.SignServer;
using EC.SignServer.Dto;
using EC.WebService.SignServer;
using Microsoft.AspNetCore.Mvc;
using EC.Manager.SignServerWorkers;
using EC.Manager.SignServerWorkers.Dto;

namespace EC.SignServer
{
    [AbpAuthorize]
    public class SignServerAppService: ECAppServiceBase
    {
        private readonly SignServerWorkerManager _signServerWorkerManager;

        public SignServerAppService(HttpClient httpClient, SignServerWorkerManager signServerWorkerManager)
        {
            _signServerWorkerManager = signServerWorkerManager;
        }

        [HttpGet]
        public async Task<SignServerDto<List<BaseWorkerDto>>> GetAllWorkers()
        {
            var result = await _signServerWorkerManager.GetAsyncJson<SignServerDto<List<BaseWorkerDto>>>("workers", null);

            return new SignServerDto<List<BaseWorkerDto>>
            {
                Message = result.Message,
                Payload = result.Payload,
                Success = result.Success
            };
        }

        [HttpPost]
        public async Task<SignServerDto<Object>> AddWorker(string implementationClass)
        {
            Dictionary<String, Object> paramList = new Dictionary<string, object>()
            {
                {"implementationClass", implementationClass}
            };
            var result = await _signServerWorkerManager.PostAsyncJson<SignServerDto<Object>>("workers", paramList, null);

            return new SignServerDto<Object>
            {
                Message = result.Message,
                Payload = result.Payload,
                Success = result.Success,
            };
        }


        [HttpGet]
        public async Task<Dictionary<string, string>> GetWorkerPropertiesById(int workerId)
        {
            Dictionary<String, Object> paramList = new Dictionary<string, object>()
            {
                {"workerId", workerId}
            };
            var result = await _signServerWorkerManager.GetAsyncJson<SignServerDto<Dictionary<String, String>>>("workers/properties", paramList);

            if (result.Success)
                return result.Payload;
            else
                throw new Exception(result.Message);
        }

        [HttpPost]
        public async Task<SignServerDto<Object>> ConfigWorker(ConfigWorkerDto input)
        {
            return await _signServerWorkerManager.PostAsyncJson<SignServerDto<Object>>("workers/properties", null, input);
        }

        [HttpGet]
        public async Task<SignServerDto<List<PropertiesPermissionDto>>> GetPropertiesPermissionList(String implementationClass)
        {
            Dictionary<String, Object> paramList = new Dictionary<String, object>()
            {
                {"type", "properties"}, {"implementationClass", implementationClass}
            };

            return await _signServerWorkerManager.GetAsyncJson<SignServerDto<List<PropertiesPermissionDto>>>("templates", paramList);
        }

        [HttpGet]
        public async Task<SignServerDto<X509CertificateInfoDto>> GetSignerCertificateInfo(int workerId)
        {
            Dictionary<String, Object> paramList = new Dictionary<string, object>()
            {
                {"workerId", workerId}
            };
            return await _signServerWorkerManager.GetAsyncJson<SignServerDto<X509CertificateInfoDto>>("workers/certificates", paramList);
        }
    }
}
