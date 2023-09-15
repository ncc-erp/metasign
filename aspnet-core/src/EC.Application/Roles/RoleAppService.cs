using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using EC.Authorization;
using EC.Authorization.Roles;
using EC.Authorization.Users;
using EC.Roles.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EC.Authorization.PermissionNames;

namespace EC.Roles
{
    [AbpAuthorize(PermissionNames.Admin_Role)]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<User, long> _userRepository;
        public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager,
             IRepository<UserRole, long> userRoleRepository, IRepository<User, long> userRepository
            )
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        [AbpAuthorize(PermissionNames.Admin_Role_Create)]
        public override async Task<RoleDto> CreateAsync(CreateRoleDto input)
        {
            CheckCreatePermission();

            var role = ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            CheckErrors(await _roleManager.CreateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
        {
            var roles = await _roleManager
                .Roles
                .WhereIf(
                    !input.Permission.IsNullOrWhiteSpace(),
                    r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
                )
                .ToListAsync();

            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }
        [AbpAuthorize(PermissionNames.Admin_Role_Edit)]
        public override async Task<RoleDto> UpdateAsync(RoleDto input)
        {
            CheckUpdatePermission();

            //var role = await _roleManager.GetRoleByIdAsync(input.Id);

            //ObjectMapper.Map(input, role);

            //CheckErrors(await _roleManager.UpdateAsync(role));

            //var grantedPermissions = PermissionManager
            //    .GetAllPermissions()
            //    .Where(p => input.GrantedPermissions.Contains(p.Name))
            //    .ToList();

            //await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            ObjectMapper.Map(input, role);

            CheckErrors(await _roleManager.UpdateAsync(role));

            return input;

            //+return MapToEntityDto(role);
        }
        [AbpAuthorize(PermissionNames.Admin_Role_Delete)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList()
            ));
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Permissions)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                || x.DisplayName.Contains(input.Keyword)
                || x.Description.Contains(input.Keyword));
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        //public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
        //{
        //    var permissions = PermissionManager.GetAllPermissions();
        //    var role = await _roleManager.GetRoleByIdAsync(input.Id);
        //    var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
        //    var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

        //    return new GetRoleForEditOutput
        //    {
        //        Role = roleEditDto,
        //        Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
        //        GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
        //    };
        //}

        public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
        {
            var permissions = SystemPermission.TreePermissions;
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
            var users = (await _userManager.GetUsersInRoleAsync(role.NormalizedName)).ToArray();
            var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions = permissions,
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList(),
            };
        }

        [AbpAuthorize(PermissionNames.Admin_Role_Edit)]
        public async Task<RolePermissionDto> ChangeRolePermission(RolePermissionDto input)
        {
            CheckUpdatePermission();

            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            var p = PermissionManager.GetAllPermissions().ToList();
            var grantedPermissions = PermissionManager
               .GetAllPermissions()
               .Where(p => input.Permissions.Contains(p.Name))
               .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return input;
        }

        [HttpGet]
        public List<GetUserInRoleDto> GetAllUsersInRole(long roleId)
        {
            var listUsersRole = (from ur in _userRoleRepository.GetAll()
                                 join u in _userRepository.GetAll() on ur.UserId equals u.Id
                                 where ur.RoleId == roleId
                                 select new GetUserInRoleDto
                                 {
                                     UserName = u.UserName,
                                     EmailAddress = u.EmailAddress,
                                     UserId = u.Id,
                                     Id = ur.Id
                                 }).Distinct().ToList();

            return listUsersRole;
        }
        [HttpGet]
        public async Task<List<GetUserNotInRoleDto>> GetAllUsersNotInRole(long roleId)
        {


            var listUserIdsInRole = _userRoleRepository.GetAll()
                .Where(x => x.RoleId == roleId)
                .Select(x => x.UserId).ToList();
            var listUsersNotInRole = _userRepository.GetAll()
                .Where(x => !listUserIdsInRole.Contains(x.Id))
                .Select(x => new GetUserNotInRoleDto
                {
                    UserId = x.Id,
                    EmailAddress = x.EmailAddress,
                    UserName = x.UserName
                }).Distinct().ToList();
            return listUsersNotInRole;
        }
        [AbpAuthorize(PermissionNames.Admin_Role_Edit)]
        [HttpPost]
        public async Task<AddUserIntoRole> AddUserIntoRole(AddUserIntoRole input)
        {
            await _userRoleRepository.InsertAsync(new UserRole
            {
                RoleId = input.RoleId,
                UserId = input.UserId,
                TenantId = AbpSession.TenantId
            });
            return input;
        }
        [AbpAuthorize(PermissionNames.Admin_Role_Edit)]
        [HttpDelete]
        public async Task<string> RemoveUserFromRole(long Id)
        {
            await _userRoleRepository.DeleteAsync(Id);
            return "Deleted successfully";
        }
    }
}

