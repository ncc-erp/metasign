using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace EC.Controllers
{
    public abstract class ECControllerBase: AbpController
    {
        protected ECControllerBase()
        {
            LocalizationSourceName = ECConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
