using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using static EC.Authorization.PermissionNames;

namespace EC.Authorization
{
    public class ECAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //context.CreatePermission(PermissionNames.Admin_User, L("Users"));
            ////context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            //context.CreatePermission(PermissionNames.Admin_Role, L("Roles"));
            //context.CreatePermission(PermissionNames.Admin_Tenant, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Name, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ECConsts.LocalizationSourceName);
        }
    }
}
