using EC.Entities;
using EC.Manager.ContractSettings;
using EC.Manager.SignerSignatureSettings;
using EC.Manager.SignerSignatureSettings.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.SignerSignatureSettings
{
    public class SignerSignatureSettingAppService : ECAppServiceBase
    {
        private readonly SignerSignatureSettingManager _signerSignatureSettingManager;

        public SignerSignatureSettingAppService(SignerSignatureSettingManager signerSignatureSettingManager)
        {
            _signerSignatureSettingManager = signerSignatureSettingManager;
        }

        [HttpPost]
        public async Task<long> Create(CreateSignerSignatureSettingDto input)
        {
            return await _signerSignatureSettingManager.Create(input);
        }

        [HttpGet]
        public GetSignerSignatureSettingDto Get(long id)
        {
            return _signerSignatureSettingManager.Get(id);
        }

        [HttpGet]
        public List<GetSignerSignatureSettingDto> GetAll()
        {
            return _signerSignatureSettingManager.GetAll();
        }

        [HttpGet]
        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSetting(long contractSettingId, string email)
        {
            return await _signerSignatureSettingManager.GetSignatureSetting(contractSettingId, email);
        }

        [HttpGet]
        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSettingForContractDesign(long contractId)
        {
            return await _signerSignatureSettingManager.GetSignatureSettingForContractDesign(contractId);
        }


        [HttpGet]
        public async Task<GetContractSignerSignatureSettingDto> GetSignatureSettingByContractId(long contractId)
        {
            return await _signerSignatureSettingManager.GetSignatureSettingByContractId(contractId);
        }

        [HttpPut]
        public async Task<UpdateSignerSignatureSetting> Update(UpdateSignerSignatureSetting input)
        {
            return await _signerSignatureSettingManager.Update(input);
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _signerSignatureSettingManager.Delete(id);
        }

        [HttpPost]
        public async Task<List<GetMassContractNotSignDto>> GetMassContractNotSign(Guid massGuid)
        {
            return await _signerSignatureSettingManager.GetMassContractNotSign(massGuid);
        }
    }
}
