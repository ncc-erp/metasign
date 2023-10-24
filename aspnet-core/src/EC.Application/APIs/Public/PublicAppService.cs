using Abp.Authorization;
using Abp.UI;
using EC.Entities;
using EC.Manager.ContactManager;
using EC.Manager.ContactManager.Dto;
using EC.Manager.Public;
using EC.Manager.Public.Dto;
using EC.WebService.DesktopApp;
using EC.WebService.DesktopApp.Dto;
using EC.WebService.Goggle;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EC.APIs.Public
{
    public class PublicAppService : ECAppServiceBase
    {
        private readonly PublicManager _publicManager;
        private readonly ContactManager _contactManager;
        private readonly DesktopAppService _desktopAppService;
        private readonly GoogleWebService _googleWebService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PublicAppService(ContactManager contactManager,
            DesktopAppService desktopAppService,
            GoogleWebService googleWebService,
            PublicManager publicManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _contactManager = contactManager;
            _desktopAppService = desktopAppService;
            _googleWebService = googleWebService;
            _hostingEnvironment = webHostEnvironment;
            _publicManager = publicManager;
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

        [HttpGet]
        public async Task<List<ContactDto>> GetAll()
        {
            return await _contactManager.GetAll();
        }
        [HttpGet]
        [AbpAllowAnonymous]
        public string GetAppVersion()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(_hostingEnvironment.WebRootPath, "exe", "metasign.exe"));
            return versionInfo.ProductVersion.Trim();
        }

        [HttpGet]
        public async Task<GetCertificateInfo> GetCertInfo()
        {
            return await _desktopAppService.GetCertInfo();
        }

        [HttpPost]
        public async Task<CreatePublicContractDto> CreateContract(string apiKey, CreatePublicContractDto input)
        {
            return await _publicManager.CreateContract(apiKey, input);
        }
    }
}