using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using PasswordSharing.Web;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using PasswordSharing.Web.Models;

namespace PasswordSharing.ApiTests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public UnitTest1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("testsettings.json", optional: false, reloadOnChange: true);
                }));
        }

        [Fact]
        public void Test1()
        {
            using (var httpClient = _factory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/password"))
            {
                var passwordInModel = new PasswordInModel { Password = "123", ExpiresIn = 100 };
                request.Content = new ObjectContent<PasswordInModel>(passwordInModel,
                    new JsonMediaTypeFormatter(),
                    MediaTypeNames.Application.Json);

                using (var response = httpClient.SendAsync(request).Result)
                {
                    Assert.False(true);
                }
            }
        }
    }
}
