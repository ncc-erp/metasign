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

        public async Task GenerateApiKey()
        {
            long loginUserId = AbpSession.UserId.Value;

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
            }
            else
            {
                existApiKey.Value = Guid.NewGuid().ToString();

                await WorkScope.UpdateAsync(existApiKey);
            }
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
