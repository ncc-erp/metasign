using System.Collections.Generic;
using static EC.Constants.Enum;

namespace EC.Manager.Contracts.Dto
{
    public class GetContractMailSettingDto
    {
        public string MailContent { get; set; }
        public string File { get; set; }
        public List<SignerDto> Signers { get; set; }
    }

    public class SignerDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public ContractRole ContractRole { get; set; }
        public int? ProcesOrder { get; set; }
        public string Color { get; set; }
    }

    public class GetSignerProcessOrderMailStatusDto
    {
        public long ContractId { get; set; }
        public int? ProcessOrder { get; set; }
        public bool IsSendmail { get; set; }
    }
}