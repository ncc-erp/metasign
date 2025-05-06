using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using EC.WebService.Mezon.Dto;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;

namespace EC.WebService.Mezon
{
    public class MezonWebService : BaseWebService
    {
        private readonly IConfiguration _configuration;
        public MezonWebService(HttpClient httpClient, IConfiguration configuration) : base(httpClient)
        {
            _configuration = configuration;
        }

        public async Task<AuthOauth2Mezon> GetTokenForOauth2Mezon(string code)
        {
            var url = _configuration.GetValue<string>("Oauth2Mezon:Url_Oauth2Mezon");
            var urlInfo = _configuration.GetValue<string>("Oauth2Mezon:Url_UserInfo");
            var client_id = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
            var client_secret = _configuration.GetValue<string>("Oauth2Mezon:Client_Secret");
            var grant_type = _configuration.GetValue<string>("Oauth2Mezon:Grant_Type");
            var redirect_uri = _configuration.GetValue<string>("Oauth2Mezon:Redirect_URI");

            var formData = new Dictionary<string, string>
            {
               { "client_id", client_id },
                       { "client_secret", client_secret },
                       { "grant_type", grant_type },
                       { "redirect_uri", redirect_uri },
                       { "code", code }
             };

            var response = await PostFormUrlEncodedAsync<TokenResponse>(url, formData);
            SetAuthorizationToken(response.AccessToken);

            var infoAuth = await PostAsync<AuthOauth2Mezon>(urlInfo, null);

            return infoAuth;
        }


    }
}

