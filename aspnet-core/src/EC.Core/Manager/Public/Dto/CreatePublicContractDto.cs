using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.Public.Dto
{
    [AutoMapTo(typeof(Contract))]

    public class CreatePublicContractDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
        public DateTime? ExpriedTime { get; set; }
    }
}
