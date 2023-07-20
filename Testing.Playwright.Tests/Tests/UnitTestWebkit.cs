using Microsoft.Playwright;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class UnitTestWebkit : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactoryWebkit<Program>>
{
    public UnitTestWebkit(PlaywrightWebApplicationFactoryWebkit<Program> webApplication, ITestOutputHelper outputHelper) :
            base(webApplication, outputHelper)
    {
    }

    [Fact]
    public async Task CanTrace()
    {
        var page = await webApplication.CreatePlaywrightPageAsync();
        await using var trace = await page.TraceAsync($"Testing Tracing on {webApplication.BrowserType}", this, true, true, true);
        outputHelper.WriteLine($"Tracing to {trace.TraceName}");
        Assert.Equal("Webkit_CanTrace.zip", trace.TraceName);

        await page.GotoAsync("/");

        Assert.True(System.IO.File.Exists(trace.TraceName));
    }

    public override string ToString() => webApplication.Environment is null ?
        webApplication.BrowserType.ToString() :
        $"{webApplication.Environment}_{webApplication.BrowserType}";
}