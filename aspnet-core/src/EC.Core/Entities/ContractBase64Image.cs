using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Entities
{
    public class ContractBase64Image : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractId { get; set; }
        public int ContractPage { get; set; }
        public string FileBase64 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
