using HRMv2.NccCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace EC.Manager.Languages
{
    public class LanguagesManager : BaseManager
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public LanguagesManager(
            IWorkScope workScope,
            IHostingEnvironment hostingEnvironment
        ) : base(workScope)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<object> GetCurrentUserLanguage(string currentUserLanguage)
        {
            if (currentUserLanguage == null) currentUserLanguage = "en";
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "languageSource", $"eContract-{currentUserLanguage}.xml");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            var localizationItems = new Dictionary<string, string>();

            var textNodes = xmlDoc.SelectNodes("//text");

            foreach (XmlNode textNode in textNodes)
            {
                var key = textNode.Attributes["name"].Value;
                var valueAttribute = textNode.Attributes["value"];
                if (valueAttribute != null && key != null)
                {
                    var value = valueAttribute.Value;
                    localizationItems[key] = value;
                }
            }
            return new
            {
                currentUserLanguage = currentUserLanguage,
                LocalizationItems = localizationItems
            };
        }
    }
}