using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Roles.Dto
{
    public class UserRoleDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AvatarPath { get; set; }
    }
}
