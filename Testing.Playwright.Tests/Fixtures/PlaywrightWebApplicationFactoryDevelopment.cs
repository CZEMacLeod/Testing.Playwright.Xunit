using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryDevelopment : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryDevelopment(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Development;
}
