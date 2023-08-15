using System;
using System.Collections.Generic;
using Abp.Configuration;

namespace EC.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.NotiExprireTime,"14", scopes: SettingScopes.Application| SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.GoogleClientId,"",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.IsEnableLoginByUsername, "true" ,scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.DefaultPDFSignerName, "PDFSigner01", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AWSAccessKeyId, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AWSSecretKey, "", scopes:SettingScopes.Application| SettingScopes.Tenant)
            };
        }
    }
}
