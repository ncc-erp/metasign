using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using EC.Authorization;
using EC.Authorization.Roles;
using EC.Authorization.Users;
using static EC.Authorization.PermissionNames;
using System.Collections.Generic;

namespace EC.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly ECDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(ECDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new ECAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    })
                );
                _context.SaveChanges();
            }

            // Admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
            CreateRoleAndAddPermissions(_tenantId);

        }

        private void CreateRoleAndAddPermissions(int tenantId)
        {
            var roleSeeds = new List<string>()
            {
                StaticRoleNames.Tenants.Admin
            };
            foreach (var roleSeed in roleSeeds)
            {
                var role = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == tenantId && r.Name == roleSeed);
                if (role == null)
                {
                    role = _context.Roles.Add(new Role(tenantId, roleSeed, roleSeed) { IsStatic = false }).Entity;
                    _context.SaveChanges();

                    var grantedPermissionsByRole = GrantPermissionRoles.PermissionRoles
                                            .Where(x => x.Key == roleSeed)
                                            .FirstOrDefault()
                                            .Value;
                    if (grantedPermissionsByRole != null)
                    {
                        var permissions = PermissionFinder.GetAllPermissions(new ECAuthorizationProvider())
                                            .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                                             grantedPermissionsByRole.Contains(p.Name))
                                            .ToList();
                        if (permissions.Any())
                        {
                            _context.Permissions.AddRange(
                                permissions.Select(permission => new RolePermissionSetting
                                {
                                    TenantId = tenantId,
                                    Name = permission.Name,
                                    IsGranted = true,
                                    RoleId = role.Id
                                }));
                            _context.SaveChanges();
                        }
                    }
                }

            }

        }

    }
}
