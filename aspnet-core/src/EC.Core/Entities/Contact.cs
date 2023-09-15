using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Entities
{
    public class Contact : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
