using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSignings.Dto
{
    public class FillInputDto
    {
        public long SignerSignatureSettingId { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public string Content { get; set; }
        public int PageHeight { get; set; }
        public string Color { get; set; }
        public bool? IsCreateContract { get; set; }
        public SignatureTypeSetting SignatureType { get; set; }
    }

    public class SignInputsDto
    {
        public string Base64Pdf { get; set; }

        public List<FillInputDto> ListInput { get; set; }
    }
}
