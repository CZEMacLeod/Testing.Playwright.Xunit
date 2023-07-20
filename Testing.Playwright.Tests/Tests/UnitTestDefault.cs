using Microsoft.Playwright;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestDefault : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactory<Program>>
{
    public UnitTestDefault(PlaywrightWebApplicationFactory<Program> webApplication, ITestOutputHelper outputHelper) : 
        base(webApplication, outputHelper)
    {
    }

    [Fact]
    public async Task CanTrace()
    {
        var page = await webApplication.CreatePlaywrightPageAsync();
        await using var trace = await page.TraceAsync($"Testing Tracing on {webApplication.BrowserType}", true, true, true);

        outputHelper.WriteLine($"Tracing to {trace.TraceName}");

        Assert.Equal("CanTrace.zip", trace.TraceName);

        await page.GotoAsync("/");
        Assert.Equal("Home page - Testing.Playwright", await page.TitleAsync());

        Assert.True(System.IO.File.Exists(trace.TraceName));

    }
}
