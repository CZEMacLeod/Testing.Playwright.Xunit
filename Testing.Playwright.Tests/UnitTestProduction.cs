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

public class UnitTestStaging : IClassFixture<PlaywrightWebApplicationFactoryStaging>
{
    private readonly PlaywrightWebApplicationFactory webApplication;
    private readonly ITestOutputHelper outputHelper;

    public UnitTestStaging(PlaywrightWebApplicationFactoryStaging webApplication, ITestOutputHelper outputHelper)
    {
        this.webApplication = webApplication;
        this.outputHelper = outputHelper;
    }

    [Fact]
    public void CheckEnvironent()
    {
        var hostEnv = webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
        outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);
        Assert.Equal(Environments.Staging, hostEnv.EnvironmentName);
    }
}

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