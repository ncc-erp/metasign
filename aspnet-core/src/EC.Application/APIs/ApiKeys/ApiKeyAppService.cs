using EC.Manager.ApiKeys;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.ApiKeys
{
    public class ApiKeyAppService : ECAppServiceBase
    {
        private readonly ApiKeyManager _apiKeyManager;

        public ApiKeyAppService(ApiKeyManager apiKeyManager)
        {
            _apiKeyManager = apiKeyManager;
        }

        [HttpPost]
        public async Task GenerateApiKey()
        {
            await _apiKeyManager.GenerateApiKey();
        }

        [HttpGet]
        public async Task<string> GetApiKey()
        {
            return await _apiKeyManager.GetApiKey();
        }
    }
}
