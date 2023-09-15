using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Roles.Dto
{
    public class CreateUserRoleDto
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
    }
}
