using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using EC.EntityFrameworkCore.Seed.Host;
using EC.EntityFrameworkCore.Seed.Tenants;
using System.Linq;

namespace EC.EntityFrameworkCore.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<ECDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(ECDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();
            var listTenantIds = context.Tenants.IgnoreQueryFilters()
                .Where(t => t.IsActive && !t.IsDeleted)
                .Select(t => t.Id)
                .ToList();
            foreach (var id in listTenantIds)
            {
                new TenantRoleAndUserBuilder(context, id).Create();
                new TenantEmailTemplateBuilder(context, id).Create();
            }
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
