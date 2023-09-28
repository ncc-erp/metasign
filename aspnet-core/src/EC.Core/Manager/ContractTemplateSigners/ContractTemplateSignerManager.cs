using EC.Entities;
using EC.Manager.ContractTemplateSigners.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace EC.Manager.ContractTemplateSigners
{
    public class ContractTemplateSignerManager : BaseManager, IContractTemplateSignerManager
    {
        public ContractTemplateSignerManager(IWorkScope workScope) : base(workScope)
        {
        }

        public async Task<UpdateListContractTemplateSignerDto> Create(UpdateListContractTemplateSignerDto input)
        {
            var listEntity = new List<ContractTemplateSigner>();
            var templateMassType = WorkScope.GetAll<ContractTemplate>().Where(x => x.Id == input.ContractTemplateId).FirstOrDefault().MassType;
            var existSetting = await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == input.ContractTemplateId)
                .Select(x => new
                {
                    x.SignerName,
                    x.SignerEmail,
                    x.ContractRole,
                    x.Color,
                    x.ProcesOrder,
                    x.Role,
                })
                .ToListAsync();
            var listNewId = new List<long>();
            for (int i = 0; i < input.ContractTemplateSigners.Count; i++)
            {
                var entity = ObjectMapper.Map<ContractTemplateSigner>(input.ContractTemplateSigners[i]);
                entity.ContractTemplateId = input.ContractTemplateId;
                bool alreadyExist = existSetting.Any(x => x.SignerEmail == input.ContractTemplateSigners[i].SignerEmail
                && x.SignerName == input.ContractTemplateSigners[i].SignerName
                && x.ContractRole == input.ContractTemplateSigners[i].ContractRole
                && x.Color == input.ContractTemplateSigners[i].Color
                && x.ProcesOrder == input.ContractTemplateSigners[i].ProcesOrder
                && x.Role == input.ContractTemplateSigners[i].Role);
                if (alreadyExist)
                {
                    continue;
                }

                if (input.ContractTemplateSigners[i].Id != null)
                {
                    await WorkScope.UpdateAsync(entity);
                }
                else
                {
                    await WorkScope.InsertAsync(entity);
                }
            }
            return input;
        }

        public async Task Update(UpdateListContractTemplateSignerDto input)
        {
            var items = await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == input.ContractTemplateId).ToListAsync();
            items.ForEach(x =>
            {
                var updateitem = input.ContractTemplateSigners.Where(y => x.Id == y.Id).FirstOrDefault();

                x.SignerEmail = updateitem.SignerEmail;
                x.SignerName = updateitem.SignerEmail;
                x.Role = updateitem.Role;
                x.ContractRole = updateitem.ContractRole;
                x.ProcesOrder = updateitem.ProcesOrder;
                x.Color = updateitem.Color;
                x.ContractTemplateId = input.ContractTemplateId;
            });
            await WorkScope.UpdateRangeAsync(items);
        }

        public async Task<GetContractTemplateSignerDto> Get(long id)
        {
            return await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.Id == id)
                .Select(x => new GetContractTemplateSignerDto
                {
                    Id = x.Id,
                    Color = x.Color,
                    ContractRole = x.ContractRole,
                    ContractTemplateId = x.ContractTemplateId,
                    ProcesOrder = x.ProcesOrder,
                    Role = x.Role,
                    SignerEmail = x.SignerEmail,
                    SignerName = x.SignerName,
                }).FirstOrDefaultAsync();
        }

        public async Task<GetAllContractTemplateSignerDto> GetAllByContractTemplateId(long id)
        {
            var listSigner = WorkScope.GetAll<ContractTemplateSigner>().AsNoTracking()
                .Where(x => x.ContractTemplateId == id).ToList();
            var isOrder = false;
            if (listSigner.Count > 0)
            {
                isOrder = listSigner.Any(x => x.ProcesOrder != 1);
            }
            var signers = await WorkScope.GetAll<ContractTemplateSigner>()
                .Where(x => x.ContractTemplateId == id).OrderBy(x => x.ContractRole)
                .Select(x => new GetContractTemplateSignerDto
                {
                    Id = x.Id,
                    Color = x.Color,
                    ContractRole = x.ContractRole,
                    ContractTemplateId = x.ContractTemplateId,
                    ProcesOrder = x.ProcesOrder,
                    Role = x.Role,
                    SignerEmail = x.SignerEmail,
                    SignerName = x.SignerName,
                }).ToListAsync();
            return new GetAllContractTemplateSignerDto
            {
                IsOrder = isOrder,
                Signers = signers
            };
        }

        public async Task Delete(long id)
        {
            var settings = WorkScope.GetAll<ContractTemplateSetting>()
                .Where(x => x.ContractTemplateSignerId == id).ToList();
            var item = await WorkScope.GetAsync<ContractTemplateSigner>(id);
            settings.ForEach(x =>
            {
                x.IsDeleted = true;
            });
            await CurrentUnitOfWork.SaveChangesAsync();
            await WorkScope.DeleteAsync(item);
        }
    }
}