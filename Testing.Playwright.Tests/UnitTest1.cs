using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests
{
    public class UnitTest1 : IClassFixture<PlaywrightWebApplicationFactory>
    {
        private readonly PlaywrightWebApplicationFactory webApplication;
        private readonly ITestOutputHelper outputHelper;

        public UnitTest1(PlaywrightWebApplicationFactory webApplication, ITestOutputHelper outputHelper)
        {
            this.webApplication = webApplication;
            this.outputHelper = outputHelper;
        }

        [Fact]
        public async Task TestRootTitle()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            await page.GotoAsync("/");
            Assert.Equal("Home page - Testing.Playwright", await page.TitleAsync());
        }

        [Fact]
        public async Task TestIndexTitle()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            await page.GotoAsync("/Index");
            Assert.Equal("Home page - Testing.Playwright", await page.TitleAsync());
        }

        [Fact]
        public async Task TestPrivacyTitle()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            await page.GotoAsync("/Privacy");
            Assert.Equal("Privacy Policy - Testing.Playwright", await page.TitleAsync());
        }

        [Fact]
        public async Task TestErrorTitle()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            await page.GotoAsync("/Error");
            Assert.Equal("Error - Testing.Playwright", await page.TitleAsync());
        }

        [Fact]
        public async Task Test404()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            var response = await page.GotoAsync("/Unknown");

            Assert.False(response.Ok);

        }

        [Fact]
        public async Task TestError()
        {
            var page = await webApplication.CreatePlaywrightPageAsync();

            var response = await page.GotoAsync("/BadRequest");

            var hostEnv = webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
            outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);
            if (!hostEnv.IsDevelopment())
            {
                Assert.Equal("Error - Testing.Playwright", await page.TitleAsync());
            }
            else
            {
                Assert.Equal("Bad Request", await page.TitleAsync());
            }
        }
    }
}