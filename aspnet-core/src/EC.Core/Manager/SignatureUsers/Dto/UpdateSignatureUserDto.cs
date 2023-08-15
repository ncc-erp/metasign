using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.SignatureUsers.Dto
{
    [AutoMapTo(typeof(SignatureUser))]
    public class UpdateSignatureUserDto:EntityDto<long>
    {
        public long SignatureTypeId { get; set; }
        public long UserId { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public bool IsDefault { get; set; }

    }
}
