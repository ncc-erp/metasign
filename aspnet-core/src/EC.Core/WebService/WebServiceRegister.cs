using EC.WebService.DesktopApp;
using EC.WebService.Goggle;
using EC.WebService.PDFConverter;
using EC.WebService.SignServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EC.WebService
{
    public static class WebServiceRegister
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfigurationRoot _appConfiguration)
        {
            services.AddHttpClient<SignServerWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("SignServerService:BaseAddress"));
            });

            services.AddHttpClient<GoogleWebService>(option =>
            {
                option.BaseAddress = new Uri("https://www.google.com/");
            });

            services.AddHttpClient<DesktopAppService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("DesktopApp:BaseAddress"));
            });
            services.AddHttpClient<PDFConverterWebService>(option =>
            {
                option.BaseAddress = new Uri(_appConfiguration.GetValue<string>("PDFConverterWebService:BaseAddress"));
            });
            return services;
        }
    }
}