using Abp.Application.Services.Dto;
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
    public class UpdateContactDto:EntityDto<long>
    {
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
