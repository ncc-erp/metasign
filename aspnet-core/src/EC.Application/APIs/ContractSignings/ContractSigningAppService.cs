using EC.Manager.ContractSignings;
using EC.Manager.ContractSignings.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EC.APIs.ContractSignings
{
    public class ContractSigningAppService : ECAppServiceBase
    {
        private readonly ContractSigningManager _contractSigningManager;

        public ContractSigningAppService(ContractSigningManager contractSigningManager)
        {
            _contractSigningManager = contractSigningManager;
        }

        [HttpPost]
        public async Task<string> SignMultiple(SignMultipleDto input)
        {
            return await _contractSigningManager.SignMultiple(input);
        }

        [HttpPost]
        public object ValidEmail(ValidEmailDto input)
        {
            return _contractSigningManager.ValidEmail(input);
        }

        [HttpGet]
        public object ValidContract(long contractId)
        {
            return _contractSigningManager.ValidContract(contractId);
        }

        [HttpGet]
        public string GetSignatureBase64()
        {
            return _contractSigningManager.GetSignatureBase64();
        }

        [HttpPost]
        public async Task<bool> InsertSigningResultAndComplete(InputSigningResultDto input)
        {
            return await _contractSigningManager.InsertSigningResultAndComplete(input);
        }

        [HttpPost]
        public async Task<bool> InsertSigningResult(InputSigningResultDto input)
        {
            return await _contractSigningManager.InsertSigningResultForInput(input);
        }

        [HttpPost]
        public async Task<string> SignInput(SignInputsDto input)
        {
            return await _contractSigningManager.SignInput(input);
        }

        [HttpGet]
        public async Task<string> GetSignerEmail(long settingId)
        {
            return await _contractSigningManager.GetSignerEmail(settingId);
        }
    }
}