using EC.Manager.Contracts.Dto;
using EC.Manager.LookupPage;
using EC.Manager.LookupPage.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.LookupPage
{
    public class LookupPageAppService : ECAppServiceBase
    {
        private readonly LookupPageManager _lookupManager;

        public LookupPageAppService(LookupPageManager lookupManager)
        {
            _lookupManager = lookupManager;
        }

        [HttpPost]
        public async Task<GridResult<LookupContractDto>> GetAllPaging(GetLookupContractDto input)
        {
            return await _lookupManager.GetAllPaging(input);
        }

        [HttpPost]
        public async Task<ContractDetailByGuidDto> GetContractDetailByGuid(GetContractDetailByGuidDto input)
        {
            return await _lookupManager.GetContractDetailByGuid(input);
        }
    }
}
