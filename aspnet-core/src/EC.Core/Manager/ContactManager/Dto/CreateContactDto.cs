using Abp.AutoMapper;
using EC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContactManager.Dto
{
    [AutoMapTo(typeof(Contact))]

    public class CreateContactDto
    {
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
