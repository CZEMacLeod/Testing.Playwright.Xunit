using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryFirefox<TProgram> : PlaywrightWebApplicationFactory<TProgram>
    where TProgram : class
{
    public PlaywrightWebApplicationFactoryFirefox(IMessageSink output) : base(output) { }

    public override PlaywrightBrowserType BrowserType => PlaywrightBrowserType.Firefox;
}
