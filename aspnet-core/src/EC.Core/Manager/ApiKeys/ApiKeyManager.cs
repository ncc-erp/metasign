using EC.Entities;
using EC.Manager.ApiKeys.Dto;
using EC.Manager.ContactManager.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ApiKeys
{
    public class ApiKeyManager : BaseManager
    {
        public ApiKeyManager(IWorkScope workScope) : base(workScope)
        {

        }

        public async Task<string> GenerateApiKey()
        {
            long loginUserId = AbpSession.UserId.Value;

            string result = "";

            var existApiKey = await WorkScope.GetAll<ApiKey>()
                .Where(x => x.UserId == loginUserId)
                .FirstOrDefaultAsync();

            if (existApiKey == default)
            {
                var dto = new ApiKey
                {
                    UserId = loginUserId,
                    Value = Guid.NewGuid().ToString(),
                };

                await WorkScope.InsertAsync(dto);
                result = dto.Value;
            }
            else
            {
                existApiKey.Value = Guid.NewGuid().ToString();

                await WorkScope.UpdateAsync(existApiKey);
                result = existApiKey.Value;
            }

            return result;
        }

        public async Task<string> GetApiKey()
        {
            var userId = AbpSession.UserId;

            return await WorkScope.GetAll<ApiKey>()
                .Where(x => x.UserId == userId)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();
        }
    }
}
