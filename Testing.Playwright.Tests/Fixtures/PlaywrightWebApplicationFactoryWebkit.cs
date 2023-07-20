using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryWebkit<TProgram> : PlaywrightWebApplicationFactory<TProgram>
    where TProgram : class
{
    public PlaywrightWebApplicationFactoryWebkit(IMessageSink output) : base(output) { }

    public override PlaywrightBrowserType BrowserType => PlaywrightBrowserType.Webkit;
}
