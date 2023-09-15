using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContactManager.Dto
{
    public class ContactDto:EntityDto<long>
    {
        public string CompanyName { get; set; }
        [ApplySearch]
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        [ApplySearch]
        public string Email { get; set; }
    }
}
