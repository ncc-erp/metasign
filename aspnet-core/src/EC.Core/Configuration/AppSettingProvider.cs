using Abp.Configuration;
using System.Collections.Generic;

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
                new SettingDefinition(AppSettingNames.AWSSecretKey, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AWSRegion, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AWSBucketName, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AWSPrefix, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.MicrosoftClientId,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SignServerBaseAddress, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SignServerAdminAPI, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableLoginGoogle, "false", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableLoginMezon, "true", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableNormalLogin, "false", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableLoginMicrosoft, "false", scopes:SettingScopes.Application| SettingScopes.Tenant)
            };
        }
    }
}