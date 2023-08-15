using System.Collections.Generic;

namespace EC.Manager.Contracts.Dto
{
    public class GetSignersDto
    {
        public string Email { get; set; }
        //public string Name { get; set; }
    }

    public class OrderSignerDto
    {
        public List<long> ContractSettingIdHaveInput { get; set; }
        public List<long> ContractSettingIdHaveBoth { get; set; }
        public List<long> NormalSignerId { get; set; }
    }
}