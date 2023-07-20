using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryStaging<TProgram> : PlaywrightWebApplicationFactory<TProgram>
    where TProgram : class
{
    public PlaywrightWebApplicationFactoryStaging(IMessageSink output) : base(output) { }

    public override string? Environment => Environments.Staging;
}
