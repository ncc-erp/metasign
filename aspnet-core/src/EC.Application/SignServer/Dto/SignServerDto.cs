using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EC.SignServer.Dto
{
    public class SignServerDto<T>
    {
        public string Message { get; set; }
        public T Payload { get; set; }

        public bool Success { get; set; }
    }

    public class BaseWorkerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImplementationClass { get; set; }
    }

    public class X509CertificateInfoDto
    {
        public string SubjectDN { get; set; }
        public string SerialNumber { get; set; }
        public string IssuerDN { get; set; }
        public DateTime notBefore { get; set; }
        public DateTime notAfter { get; set; }
    }

    public class PropertiesDto
    {
        public string Lable { get; set; }
        public string Value { get; set; }
    }

    public class PropertiesPermissionDto
    {
        public string Key { get; set; }

        public Boolean editable { get; set; }
    }
}
