using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Entities
{
    public class ApiKey : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        public string Value { get; set; }
    }
}
