using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSignings.Dto
{
    public class CertificateDto
    {
        public long ContractId { get; set; }
        public Guid? ContractGuId { get; set; }
        public string ContractName { get; set; }
        public string Code { get; set; }
        public long UserId { get; set; }
        public string CreatorUser { get; set; }
        public string CreatorEmail { get; set; }
        public string FileName { get; set; }
        public ContractStatus Status { get; set; }
        public string StatusName => Enum.GetName(typeof(ContractStatus), Status);
        public DateTime CreationTime { get; set; }
        public DateTime? ExpriredTime { get; set; }
        public List<SignartureDto> Signatures { get; set;}
    }
    public class SignartureDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime SendingTime { get; set; }
        public DateTime SigningTime { get; set; }
        public string SignartureBase64 { get; set; }
        public Guid? GuId { get;set; }
        public SignMethod? SignatureType { get;set; }
    }
}