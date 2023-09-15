using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class ContractSigning : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ContractId { get; set; }
        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }
        public string Email { get; set; }
        public string SignartureBase64 { get; set; }
        public string SigningResult { get; set; }
        public long? SignatureId { get; set; }
        public DateTime TimeAt { get; set; }
        public Guid? Guid { get; set; }
        public SignMethod? SignatureType { get; set; }
    }
}
