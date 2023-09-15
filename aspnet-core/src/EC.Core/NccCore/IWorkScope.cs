using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.NccCore
{
    public interface IWorkScope : ITransientDependency
    {
        IRepository<TEntity, TPrimaryKey> GetRepo<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IQueryable<TEntity> GetAll<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IRepository<TEntity, long> GetRepo<TEntity>() where TEntity : class, IEntity<long>;
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity<long>;
        #region Async
        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<long> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        long InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;

        Task<TEntity> InsertAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TEntity Insert<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<IEnumerable<TEntity>> InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;
        IEnumerable<TEntity> InsertRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;
        Task<TPrimaryKey> InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;

        TPrimaryKey InsertAndGetId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> UpdateAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;

        Task<List<TEntityDto>> InsertUpdateAndDelete<TEntity, TEntityDto, TPrimaryKey>(List<TEntityDto> entities, IQueryable<TPrimaryKey> existIds, bool includeUpdate = true)
            where TEntity : class, IEntity<TPrimaryKey>
            where TEntityDto : class, IEntityDto<TPrimaryKey>;

        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task DeleteAsync<TEntity>(long id) where TEntity : class, IEntity<long>;
        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<long>;
        Task<TEntity> GetAsync<TEntity>(long id) where TEntity : class, IEntity<long>;
        Task<long> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        IRepository<TEntity, long> Repository<TEntity>() where TEntity : class, IEntity<long>;
        Task<TEntity> InsertOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<TEntity> InsertOrUpdateAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        #endregion
    }
}
