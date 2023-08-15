using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.MultiTenancy;
using EC.Editions;
using EC.MultiTenancy;

namespace EC.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly ECDbContext _context;

        public DefaultTenantBuilder(ECDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            // Default tenant

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == "NCC");
            if (defaultTenant == null)
            {
                defaultTenant = new Tenant("NCC", "NCCsoft");

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
        }
    }
}
