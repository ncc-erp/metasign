using EC.Manager.ContactManager;
using EC.Manager.ContactManager.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.APIs.Contacts
{
    public class ContactAppService: ECAppServiceBase
    {
        private readonly ContactManager _contactManager;
        public ContactAppService(ContactManager contactManager)
        {
            _contactManager = contactManager;
        }

        [HttpPost]
        public async Task<GridResult<ContactDto>> GetAllPaging(GridParam input)
        {
            return await _contactManager.GetAllPaging(input);
        }

        [HttpGet]
        public async Task<List<ContactDto>> GetAll()
        {
            return await _contactManager.GetAll();
        }

        [HttpPost]
        public async Task<object> CreateContact(CreateContactDto input)
        {
            return await _contactManager.CreateContact(input);
        }

        [HttpPost]
        public async Task<UpdateContactDto> Update(UpdateContactDto input)
        {
            return await _contactManager.Update(input);
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _contactManager.Delete(id);
        }
    }
}
