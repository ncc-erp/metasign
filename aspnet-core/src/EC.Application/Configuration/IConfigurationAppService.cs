using System.Threading.Tasks;
using EC.Configuration.Dto;

namespace EC.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
