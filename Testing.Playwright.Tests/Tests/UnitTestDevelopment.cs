using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestDevelopment : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactoryDevelopment<Program>>
{

    public UnitTestDevelopment(PlaywrightWebApplicationFactoryDevelopment<Program> webApplication, ITestOutputHelper outputHelper) :
            base(webApplication, outputHelper)
    {
    }

    [Fact]
    public void CheckEnvironent()
    {
        var hostEnv = webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
        outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);
        Assert.Equal(Environments.Development, hostEnv.EnvironmentName);
    }
}