using System;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class MassTemplateExportDto
    {
        public string SignerRole => Enum.GetName(typeof(ContractRole), this.ContractRole);
        public ContractRole ContractRole { get; set; }
        public string Role { get; set; }
        public int? ProcessOrder { get; set; }
    }
}