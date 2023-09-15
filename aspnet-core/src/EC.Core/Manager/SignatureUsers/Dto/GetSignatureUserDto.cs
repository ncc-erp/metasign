using Abp.Application.Services.Dto;
using EC.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.SignatureUsers.Dto
{
    public class GetSignatureUserDto: EntityDto<long>
    {
        public SignatureTypeSetting SignatureType { get; set; }
        public long UserId { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public string LastModifierUser { get; set; }

    }
}
