using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.Manager.ContractSettings.Dto
{
    [AutoMapTo(typeof(ContractSetting))]
    public class UpdatECSettingDto : EntityDto<long>
    {
        public long ContractId { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Password { get; set; }
        public string Color { get; set; }
        public string Role { get; set; }
    }
}
