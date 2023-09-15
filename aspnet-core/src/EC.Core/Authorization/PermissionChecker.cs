using Abp.Authorization;
using EC.Authorization.Roles;
using EC.Authorization.Users;

namespace EC.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
