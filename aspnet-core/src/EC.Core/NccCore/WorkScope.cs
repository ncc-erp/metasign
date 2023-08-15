using Abp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System.Linq.Expressions;
using System.Reflection;
using NccCore.Uitls;
using Abp.Runtime.Session;

namespace HRMv2.NccCore
{
    public class WorkScope : AbpServiceBase, IWorkScope
    {
        private readonly IIocManager _iocManager;

        public IAbpSession AbpSession { get; set; }
        public WorkScope(IAbpSession AbpSession, IIocManager iocManager)
        {
            this.AbpSession = AbpSession;
            _iocManager = iocManager;
        }

        async Task<long> IWorkScope.InsertAndGetIdAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).InsertAndGetIdAsync<TEntity, long>(entity);
        }

        long IWorkScope.InsertAndGetId<TEntity>(TEntity entity)
        {
            return (this as IWorkScope).InsertAndGetId<TEntity, long>(entity);
        }

        async Task<TPrimaryKey> IWorkScope.InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateLastModifiedTime<TEntity, TPrimaryKey>(entity);
            UpdateTenantId<TEntity, TPrimaryKey>(entity);            
            return await repo.InsertAndGetIdAsync(entity);
        }

        TPrimaryKey IWorkScope.InsertAndGetId<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateLastModifiedTime<TEntity, TPrimaryKey>(entity);
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertAndGetId(entity);
        }

        async Task<TEntity> IWorkScope.InsertAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).InsertAsync<TEntity, long>(entity);
        }

        async Task<TEntity> IWorkScope.InsertAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateLastModifiedTime<TEntity, TPrimaryKey>(entity);
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.InsertAsync(entity);
        }

        IQueryable<TEntity> IWorkScope.GetAll<TEntity, TPrimaryKey>()
        {
            return (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>().GetAll();
        }

        IQueryable<TEntity> IWorkScope.GetAll<TEntity>()
        {
            return (this as IWorkScope).GetAll<TEntity, long>();
        }

        IRepository<TEntity, TPrimaryKey> IWorkScope.GetRepo<TEntity, TPrimaryKey>()
        {
            var repoType = typeof(IRepository<,>);
            Type[] typeArgs = { typeof(TEntity), typeof(TPrimaryKey) };
            var repoGenericType = repoType.MakeGenericType(typeArgs);
            var resolveMethod = _iocManager.GetType().GetMethods()
                .Where(s => s.Name == "Resolve" && !s.IsGenericMethod && s.GetParameters().Length == 1 && s.GetParameters()[0].ParameterType == typeof(Type))
                .FirstOrDefault();
            var repo = resolveMethod.Invoke(_iocManager, new Type[] { repoGenericType });
            return repo as IRepository<TEntity, TPrimaryKey>;
        }

        IRepository<TEntity, long> IWorkScope.GetRepo<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>();
        }

        private void UpdateTenantId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            var tenantEntity = entity as IMayHaveTenant;
            if (tenantEntity != null)
                tenantEntity.TenantId = CurrentUnitOfWork.GetTenantId();
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            await (this as IWorkScope).GetRepo<TEntity, long>().DeleteAsync(entity);
        }

        public async Task DeleteAsync<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            await (this as IWorkScope).GetRepo<TEntity, long>().DeleteAsync(id);
        }

        public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<long>
        {
            await (this as IWorkScope).GetRepo<TEntity, long>().DeleteAsync(predicate);
        }
        private void UpdateLastModifiedTime<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity == null) return;
            Type entityType = typeof(TEntity);
            PropertyInfo lastModifiedTime = entityType.GetProperty("LastModificationTime");
            PropertyInfo LastModifierUserId = entityType.GetProperty("LastModifierUserId");
            lastModifiedTime.SetValue(entity, DateTimeUtils.GetNow());
            LastModifierUserId.SetValue(entity, AbpSession.UserId);
        }
        public async Task<List<TEntityDto>> InsertUpdateAndDelete<TEntity, TEntityDto, TPrimaryKey>(List<TEntityDto> entitiesDto, IQueryable<TPrimaryKey> existIds, bool includeUpdate = true)
            where TEntity : class, IEntity<TPrimaryKey>
            where TEntityDto : class, IEntityDto<TPrimaryKey>
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();

            var insertItemDtos = entitiesDto.Where(s => s.Id.Equals(default(TPrimaryKey)));

            // update all tenant here
            foreach (var itemDto in insertItemDtos)
            {
                var item = ObjectMapper.Map<TEntity>(itemDto);
                UpdateTenantId<TEntity, TPrimaryKey>(item);
                UpdateLastModifiedTime<TEntity, TPrimaryKey>(item);

                item.Id = await repo.InsertAndGetIdAsync(item);
                itemDto.Id = item.Id;
            }

            var entityIds = entitiesDto.Select(s => s.Id);

            // update item
            if (includeUpdate)
            {
                var updateItemDtos = entitiesDto.Where(s => existIds.Contains(s.Id)).ToLookup(s => s.Id, s => s);
                var updateIds = updateItemDtos.Select(s => s.Key);
                var updateItems = repo.GetAll().Where(s => updateIds.Contains(s.Id));
                foreach (var item in updateItems)
                {
                    var itemDto = updateItemDtos[item.Id].FirstOrDefault();
                    ObjectMapper.Map(itemDto, item);
                    UpdateLastModifiedTime<TEntity, TPrimaryKey>(item);
                    UpdateTenantId<TEntity, TPrimaryKey>(item);
                    await repo.UpdateAsync(item);
                }
            }

            // delete item
            var deletedIds = existIds.Where(s => !entityIds.Contains(s)).ToList();
            await repo.DeleteAsync(s => deletedIds.Contains(s.Id));
            return entitiesDto;
        }

        async Task<TEntity> IWorkScope.InsertOrUpdateAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).InsertOrUpdateAsync<TEntity, long>(entity);
        }

        async Task<TEntity> IWorkScope.InsertOrUpdateAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.InsertOrUpdateAsync(entity);
        }

        async Task<TEntity> IWorkScope.UpdateAsync<TEntity>(TEntity entity)
        {
            return await (this as IWorkScope).UpdateAsync<TEntity, long>(entity);
        }

        async Task<TEntity> IWorkScope.UpdateAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.UpdateAsync(entity);
        }

        async Task<IEnumerable<TEntity>> IWorkScope.UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add(await (this as IWorkScope).UpdateAsync<TEntity, long>(entity));
            }

            return updatedEntities;
        }

        public async Task<long> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).InsertOrUpdateAndGetIdAsync<TEntity, long>(entity);
        }

        async Task<TPrimaryKey> IWorkScope.InsertOrUpdateAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return await repo.InsertOrUpdateAndGetIdAsync(entity);
        }

        public async Task<TEntity> GetAsync<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).GetRepo<TEntity, long>().GetAsync(id);
        }

        IRepository<TEntity, long> IWorkScope.Repository<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>();
        }

        async Task<IEnumerable<TEntity>> IWorkScope.InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add(await (this as IWorkScope).InsertAsync(entity));
            }

            return updatedEntities;
        }

        IEnumerable<TEntity> IWorkScope.InsertRange<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add((this as IWorkScope).Insert(entity));
            }

            return updatedEntities;
        }


        TEntity IWorkScope.Insert<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateLastModifiedTime<TEntity, TPrimaryKey>(entity);
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.Insert(entity);
        }

        TEntity IWorkScope.Insert<TEntity>(TEntity entity)
        {
            return (this as IWorkScope).Insert<TEntity, long>(entity);
        }
    }
}
