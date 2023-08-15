using EC.Manager.SignatureUsers;
using EC.Manager.SignatureUsers.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.SignatureUsers
{
    public class SignatureUserAppService : ECAppServiceBase
    {
        private readonly SignatureUserManager _signatureUserManager;
        public SignatureUserAppService(SignatureUserManager signatureUserManager)
        {
            _signatureUserManager = signatureUserManager;
        }

        [HttpPost]
        public async Task<CreateSignatureUserDto> Create(CreateSignatureUserDto input)
        {
            return await _signatureUserManager.Create(input);
        }

        [HttpGet]
        public GetSignatureUserDto Get(long id)
        {
            return _signatureUserManager.Get(id);
        }

        [HttpGet]
        public async Task<List<GetSignatureUserDto>> GetAll()
        {
            return await _signatureUserManager.GetAll();
        }

        [HttpGet]
        public async Task<List<GetSignatureUserDto>> GetAllByEmail(long settingId)
        {
            return await _signatureUserManager.GetAllByEmail(settingId);
        }

        [HttpPut]
        public async Task<UpdateSignatureUserDto> Update(UpdateSignatureUserDto input)
        {
            return await _signatureUserManager.Update(input);
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _signatureUserManager.Delete(id);
        }

        [HttpPost]
        public async Task SetDefaultSignature(SetDefaultSignatureDto input)
        {
            await _signatureUserManager.SetDefaultSignature(input);
        }
    }
}
