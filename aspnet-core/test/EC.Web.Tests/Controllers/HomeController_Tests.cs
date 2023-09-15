using System.Threading.Tasks;
using EC.Models.TokenAuth;
using EC.Web.Controllers;
using Shouldly;
using Xunit;

namespace EC.Web.Tests.Controllers
{
    public class HomeController_Tests: ECWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}