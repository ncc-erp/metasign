using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.WebService.DesktopApp.Dto
{
    public class GetCertificateInfo
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
        public string SignatureBase64 { get; set; }
        public CertDetailtDto CertDetailInfo { get; set; }
    }

    public class CertDetailtDto
    {
        public string CertSerial { get; set; }
        public string OrganizationCA { get; set; }
        public string OwnCA { get; set; }
        public string Uid { get; set; }
        public DateTime BeginDateCA { get; set; }
        public DateTime EndDateCA { get; set; }
    }
}
