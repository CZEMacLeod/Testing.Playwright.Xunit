using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryStaging : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryStaging(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Staging;
}
