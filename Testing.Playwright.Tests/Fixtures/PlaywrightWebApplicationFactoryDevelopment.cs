using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryDevelopment<TProgram> : PlaywrightWebApplicationFactory<TProgram>
    where TProgram : class
{
    public PlaywrightWebApplicationFactoryDevelopment(IMessageSink output) : base(output) { }

    public override string? Environment => Environments.Development;
}
