using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Roles.Dto
{
    [AutoMap(typeof(Role))]

    public class RolePermissionDto : EntityDto<int>
    {
        public List<string> Permissions { get; set; }
    }
}
