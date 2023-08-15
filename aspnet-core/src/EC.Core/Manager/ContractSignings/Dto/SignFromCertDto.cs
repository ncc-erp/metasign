using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSignings.Dto
{
    public class SignFromCertDto
    {
        public string CertSerial { get; set; }
        public string PdfBase64 { get; set; }
        public string SignatureBase64 { get; set; }
        public int Page { get; set; }
        public DigitalSignaturePositionDto Position { get; set; }
    }

    public class DigitalSignaturePositionDto
    {
        public int x { get; set; }
        public int y { get; set; }
        public int llx { get; set; }
        public int lly { get; set; }
    }
}
