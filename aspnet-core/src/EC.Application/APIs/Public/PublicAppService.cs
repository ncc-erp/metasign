using Abp.UI;
using EC.Manager.ContactManager;
using EC.Manager.ContactManager.Dto;
using EC.Utils;
using EC.WebService.DesktopApp;
using EC.WebService.DesktopApp.Dto;
using EC.WebService.Goggle;
using EC.WebService.Goggle.Dto;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using System.Threading.Tasks;

namespace EC.APIs.Public
{
    public class PublicAppService : ECAppServiceBase
    {
        private readonly ContactManager _contactManager;
        private readonly GoogleWebService _googleWebService;
        private readonly DesktopAppService _desktopAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PublicAppService(ContactManager contactManager,
            DesktopAppService desktopAppService,
            GoogleWebService googleWebService,
            IWebHostEnvironment webHostEnvironment)
        {
            _contactManager = contactManager;
            _desktopAppService = desktopAppService;
            _googleWebService = googleWebService;
            _hostingEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<List<ContactDto>> GetAll()
        {
            return await _contactManager.GetAll();
        }

        [HttpPost]
        public async Task<object> CreateContact(CreateContactDto input)
        {
            var verify = await _googleWebService.VerifyCapcha(input.Token);

            if (verify.success == "false")
            {
                return new
                {
                    success = false,
                    errorCodes = verify.errorCodes,
                    errorMessage = "Unable to submit form! Please try again!"
                };
            }

            return await _contactManager.CreateContact(input);
        }

        [HttpGet]
        public async Task<GetCertificateInfo> GetCertInfo()
        {
            return await _desktopAppService.GetCertInfo();
        }

        [HttpGet]
        public dynamic DownloadApp()
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "exe/metasign.exe");

            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException("Setup file not found!");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return fileBytes;
        }
    }
}