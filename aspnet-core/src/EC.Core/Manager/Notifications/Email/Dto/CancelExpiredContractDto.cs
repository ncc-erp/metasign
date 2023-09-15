using System;

namespace EC.Manager.Notifications.Email.Dto
{
    public class CancelExpiredContractDto
    {
        public long ContractId { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
    }
}