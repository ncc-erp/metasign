using Abp.Domain.Entities;
using EC.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Entities
{
    public class SignatureUser : AuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long SignatureTypeId { get; set; }
        [ForeignKey(nameof(SignatureTypeId))]
        public SignatureTypeSetting SignatureType { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public bool IsDefault { get; set; }
    }
}
