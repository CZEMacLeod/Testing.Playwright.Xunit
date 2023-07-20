using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryProduction : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryProduction(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Production;
}
