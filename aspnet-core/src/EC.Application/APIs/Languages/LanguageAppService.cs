using Abp.Localization;
using EC.Manager.ContractHistories;
using EC.Manager.ContractHistories.Dto;
using EC.Manager.Languages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC.APIs.Languages
{
    public class LanguageAppService : ECAppServiceBase
    {
        private readonly LanguagesManager _languageManager;

        public LanguageAppService(LanguagesManager languageManager)
        {
            _languageManager = languageManager;
        }
        [HttpGet]
        public Task<object> GetCurrentUserLanguage(string currentUserLanguage)
        {
            return _languageManager.GetCurrentUserLanguage(currentUserLanguage);
        }
    }
}
