using System.Collections.Generic;
using static EC.Authorization.PermissionNames;

namespace EC.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<SystemPermission> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}