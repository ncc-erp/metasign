using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Configuration.Dto
{
    public class AWSCredentialDto
    {
        public string AccessKeyId { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
        public string Prefix { get; set; }
    }
}
