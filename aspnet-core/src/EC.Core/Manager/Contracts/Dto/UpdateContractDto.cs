using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    [AutoMapTo(typeof(Contract))]
    public class UpdatECDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public DateTime? ExpriedTime { get; set; }
        public ContractStatus Status { get; set; }
        public List<ContractBase64ImageDto> Base64Images { get; set; }
        public long? ContractTemplateId { get; set; }
    }
}