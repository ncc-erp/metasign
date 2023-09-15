using Abp.Domain.Entities.Auditing;
using EC.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Entities
{
    public abstract class AuditEntity : FullAuditedEntity<long>
    {
        [ForeignKey(nameof(CreatorUserId))]
        public User CreatorUser { get; set; }
        [ForeignKey(nameof(LastModifierUserId))]
        public User LastModifierUser { get; set; }
    }
}
