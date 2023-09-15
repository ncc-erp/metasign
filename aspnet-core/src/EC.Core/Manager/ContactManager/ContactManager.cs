using Abp.BackgroundJobs;
using EC.BackgroundJobs.SendMail;
using EC.Configuration;
using EC.Entities;
using EC.Manager.ContactManager.Dto;
using EC.Manager.Contracts.Dto;
using EC.Manager.Notifications.Email.Dto;
using EC.Manager.Notifications.Email;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EC.Constants.Enum;
using Abp.Net.Mail;

namespace EC.Manager.ContactManager
{
    public class ContactManager : BaseManager
    {
        private readonly EmailManager _emailManager;
        private readonly IEmailSender _emailSender;
        private readonly BackgroundJobManager _backgroundJobManager;
        public ContactManager(IWorkScope workScope,
            IEmailSender emailSender,
            EmailManager emailManager, BackgroundJobManager backgroundJobManager) : base(workScope)
        {

            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
            _emailSender = emailSender;
        }

        public async Task<List<ContactDto>> GetAll()
        {
            return await WorkScope.GetAll<Contact>()
                .Select(x => new ContactDto
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    CustomerName = x.CustomerName,
                    Email = x.Email,
                    Phone = x.Phone
                }).ToListAsync();
        }

        public async Task<object> CreateContact(CreateContactDto input)
        {
            var entity = ObjectMapper.Map<Contact>(input);

            await WorkScope.InsertAsync(entity);

            await NotifyContact(input.CompanyName, input.CustomerName, input.Phone, input.Email);

            return new
            {
                success = true,
            };
        }

        public async Task<UpdateContactDto> Update(UpdateContactDto input)
        {
            var entity = ObjectMapper.Map<Contact>(input);

            await WorkScope.UpdateAsync(entity);

            return input;
        }

        public async Task<long> Delete(long id)
        {
            var contact = WorkScope.GetAll<Contact>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (contact != default)
            {
                await WorkScope.DeleteAsync(contact);
            }

            return id;
        }

        public async Task<GridResult<ContactDto>> GetAllPaging(GridParam input)
        {
            var query = WorkScope.GetAll<Contact>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new ContactDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    CompanyName = x.CompanyName,
                    CustomerName = x.CustomerName
                });

            return await query.GetGridResult(query, input);
        }

        public async Task NotifyContact(string companyName, string customerName, string phone, string email)
        {
            var message = $"<br/><div>\r\n  <strong>Name:</strong> {customerName}<br/>\r\n  <strong>Email:</strong> {email}<br/>\r\n  <strong>Phone:</strong> {phone}<br/>\r\n  <strong>Company:</strong> {companyName}<br/>\r\n</div>";

            await _emailSender.SendAsync(
                   to: "info@metasign.com.vn",
                   subject: "Metasign Contact info",
                   body: message,
                   isBodyHtml: true
           );
        }
    }
}
