using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSignings.Dto
{
    public class SigningDto
    {
        public long SignerSignatureSettingId { get; set; }
        public string SignartureBase64 { get; set; }
        public long? SignatureUserId { get; set; }
        public bool? IsNewSignature { get; set; }
        public long SignatureTypeId { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
        public bool SetDefault { get; set; }
        public float PageHeight { get; set; }
        public string CertSerial { get; set; }
    }
    public class SignMultipleDto
    {
        public long ContractId { get; set; }
        public string ContractBase64 { get; set; }
        public List<SigningDto> SignSignatures { get; set; }
    }
}
