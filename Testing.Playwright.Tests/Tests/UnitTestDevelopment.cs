using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestDevelopment : IClassFixture<PlaywrightWebApplicationFactoryDevelopment>
{
    private readonly PlaywrightWebApplicationFactory webApplication;
    private readonly ITestOutputHelper outputHelper;

    public UnitTestDevelopment(PlaywrightWebApplicationFactoryDevelopment webApplication, ITestOutputHelper outputHelper)
    {
        this.webApplication = webApplication;
        this.outputHelper = outputHelper;
    }

    [Fact]
    public void CheckEnvironent()
    {
        var hostEnv = webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
        outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);
        Assert.Equal(Environments.Development, hostEnv.EnvironmentName);
    }
}