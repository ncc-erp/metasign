using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.ContractHistories
{
    public class ContractHistoryAppService : ECAppServiceBase
    {
        private readonly ContractHistoryManager _contractHistoryManager;
        public ContractHistoryAppService(ContractHistoryManager contractHistoryManager)
        {
            _contractHistoryManager = contractHistoryManager;
        }

        [HttpGet]
        public async Task<List<GetContractHistoryDto>> GetAll()
        {
            return await _contractHistoryManager.GetAll();
        }

        [HttpGet]
        public async Task<GetContractHistoryDto> Get(long id)
        {
            return await _contractHistoryManager.Get(id);
        }

        [HttpGet]
        public async Task<List<GetContractHistoryDto>> GetHistoriesByContractId(long contractId)
        {
            return await _contractHistoryManager.GetHistoriesByContractId(contractId);
        }

        [HttpPost]
        public async Task<GridResult<GetContractHistoryDto>> GetHistoriesByContractIdPaging(long contractId, GridParam input)
        {
            return await _contractHistoryManager.GetHistoriesByContractIdPaging(contractId, input);
        }

        [HttpPost]
        public async Task<CreaContractHistoryDto> Create(CreaContractHistoryDto input)
        {
            return await _contractHistoryManager.Create(input);
        }
    }
}
