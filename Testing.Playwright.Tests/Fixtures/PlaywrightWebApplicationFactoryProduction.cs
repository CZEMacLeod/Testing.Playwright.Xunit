using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryProduction<TProgram> : PlaywrightWebApplicationFactory<TProgram>
    where TProgram : class
{
    public PlaywrightWebApplicationFactoryProduction(IMessageSink output) : base(output) { }

    public override string? Environment => Environments.Production;
}
