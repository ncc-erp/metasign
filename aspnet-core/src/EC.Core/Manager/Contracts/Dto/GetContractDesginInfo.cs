using EC.Manager.SignerSignatureSettings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractDesginInfo
    {
        public int Page { get; set; }
        public string ContractBase64 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public List<GetSignerSignatureSettingDto> SignatureSettings { get; set; }
    }
}
