using EC.Entities;
using EC.Manager.ContractHistories.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Manager.ContractHistories
{
    public class ContractHistoryManager : BaseManager
    {
        public ContractHistoryManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<GetContractHistoryDto> IQContractHistory()
        {
            return WorkScope.GetAll<ContractHistory>()
                .Where(x => string.IsNullOrEmpty(x.MailContent))
                .OrderBy(x => x.CreationTime)
                .Select(x => new GetContractHistoryDto
                {
                    Id = x.Id,
                    Action = x.Action,
                    AuthorEmail = x.AuthorEmail,
                    ContractId = x.ContractId,
                    ContractStatus = x.ContractStatus,
                    Note = x.Note,
                    TimeAt = x.TimeAt
                });
        }

        public async Task<List<GetContractHistoryDto>> GetAll()
        {
            return IQContractHistory()
                .ToList();
        }

        public async Task<GetContractHistoryDto> Get(long id)
        {
            return await IQContractHistory()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GetContractHistoryDto>> GetHistoriesByContractId(long contractId)
        {
            return await IQContractHistory()
                .Where(x => x.ContractId == contractId)
                .OrderByDescending(x => x.TimeAt)
                .ToListAsync();
        }

        public async Task<GridResult<GetContractHistoryDto>> GetHistoriesByContractIdPaging(long contractId, GridParam input)
        {
            var query = IQContractHistory()
                .Where(X => X.ContractId == contractId);

            return await query.GetGridResult(query, input);
        }

        public async Task<CreaContractHistoryDto> Create(CreaContractHistoryDto input)
        {
            var entity = ObjectMapper.Map<ContractHistory>(input);

            await WorkScope.InsertAsync(entity);

            return input;
        }

        public CreaContractHistoryDto CreateSync(CreaContractHistoryDto input)
        {
            var entity = ObjectMapper.Map<ContractHistory>(input);
            WorkScope.Insert(entity);
            return input;
        }
    }
}