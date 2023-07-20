using Microsoft.Playwright;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestFirefox : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactoryFirefox<Program>>
{
    public UnitTestFirefox(PlaywrightWebApplicationFactoryFirefox<Program> webApplication, ITestOutputHelper outputHelper) :
            base(webApplication, outputHelper)
    {
    }

    [Fact]
    public async Task CanTrace()
    {
        var page = await webApplication.CreatePlaywrightPageAsync();
        await using var trace = await page.TraceAsync<UnitTestFirefox>($"Testing Tracing on {webApplication.BrowserType}", true, true, true);
        outputHelper.WriteLine($"Tracing to {trace.TraceName}");
        
        Assert.Equal("Testing.Playwright.Tests.UnitTestFirefox_CanTrace.zip", trace.TraceName);

        await page.GotoAsync("/");
        
        Assert.True(System.IO.File.Exists(trace.TraceName));
    }
}