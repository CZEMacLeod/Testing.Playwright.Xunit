using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestProduction : IClassFixture<PlaywrightWebApplicationFactoryProduction>
{
    private readonly PlaywrightWebApplicationFactory webApplication;
    private readonly ITestOutputHelper outputHelper;

    public UnitTestProduction(PlaywrightWebApplicationFactoryProduction webApplication, ITestOutputHelper outputHelper)
    {
        this.webApplication = webApplication;
        this.outputHelper = outputHelper;
    }

    [Fact]
    public void CheckEnvironent()
    {
        var hostEnv = webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
        outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);
        Assert.Equal(Environments.Production, hostEnv.EnvironmentName);
    }
}
