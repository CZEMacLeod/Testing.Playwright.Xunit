using Microsoft.Playwright;

namespace Testing.Playwright.Tests.Utilities;

public class PlaywrightTrace : IAsyncDisposable
{
    internal PlaywrightTrace(IPage page)
    {
        this.page = page;
    }
    private readonly IPage page;

    private string? path;

    public string? TraceName => path;

    internal async Task InitializeAsync(TracingStartOptions options)
    {
        await page.Context.Tracing.StartAsync(options);
        path = options.Name;
    }

    public async ValueTask DisposeAsync()
    {
        await page.Context.Tracing.StopAsync(new()
        {
            Path = path
        });
    }
}
