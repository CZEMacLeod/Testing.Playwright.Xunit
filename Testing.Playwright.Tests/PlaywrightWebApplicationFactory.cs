﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Playwright;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactoryStaging : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryStaging(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Staging;
}

public class PlaywrightWebApplicationFactoryProduction : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryProduction(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Production;
}

public class PlaywrightWebApplicationFactoryDevelopment : PlaywrightWebApplicationFactory
{
    public PlaywrightWebApplicationFactoryDevelopment(IMessageSink output) : base(output) { }

    protected override string? Environment => Environments.Development;
}

public class PlaywrightWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IPlaywright? playwright;
    private IBrowser? browser;
    private string? uri;
    private readonly IMessageSink output;

    public string? Uri => uri;

    protected virtual string? Environment { get; }

    public PlaywrightWebApplicationFactory(IMessageSink output)
    {
        this.output = output;
    }

    [MemberNotNull(nameof(uri))]
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (Environment is not null)
        {
            builder.UseEnvironment(Environment);
        }
        builder.ConfigureLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddProvider(new MessageSinkProvider(output));
        });

        var port = 5000 + System.Random.Shared.Next(1000);
        uri = $"http://localhost:{port}";

        var testHost = base.CreateHost(builder);
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel(options =>
           {
               options.ListenLocalhost(port);
           }));
        var host = base.CreateHost(builder);
        var server = host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>() ?? throw new NullReferenceException("Could not get IServerAddressesFeature");
        var serverAddress = addresses.Addresses.FirstOrDefault();

        if (serverAddress is not null)
        {
            uri = serverAddress;
        }
        else
        {
            var message = new Xunit.Sdk.DiagnosticMessage("Could not get server address from IServerAddressesFeature");
            output.OnMessage(message);
        }

        return new CompositeHost(testHost, host);
    }

    public async Task<IPage> CreatePlaywrightPageAsync()
    {
        var server = Server;
        if (playwright is null || browser is null) { await InitializeAsync(); }    // Shouldn't be...

        return await browser.NewPageAsync(new BrowserNewPageOptions()
        {
            BaseURL = uri
        });
    }

    [MemberNotNull(nameof(playwright), nameof(browser))]
    public async Task InitializeAsync()
    {
        playwright = (await Microsoft.Playwright.Playwright.CreateAsync()) ?? throw new InvalidOperationException();
        browser = (await playwright.Chromium.LaunchAsync(new() { Headless = true })) ?? throw new InvalidOperationException();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (browser is not null)
        {
            await browser.DisposeAsync();
        }
        browser = null;
        playwright?.Dispose();
        playwright = null;
    }

    // Relay the call to both test host and kestrel host.
    public class CompositeHost : IHost
    {
        private readonly IHost testHost;
        private readonly IHost kestrelHost;
        public CompositeHost(IHost testHost, IHost kestrelHost)
        {
            this.testHost = testHost;
            this.kestrelHost = kestrelHost;
        }
        public IServiceProvider Services => testHost.Services;
        public void Dispose()
        {
            testHost.Dispose();
            kestrelHost.Dispose();
        }
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await testHost.StartAsync(cancellationToken);
            await kestrelHost.StartAsync(cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await testHost.StopAsync(cancellationToken);
            await kestrelHost.StopAsync(cancellationToken);
        }
    }

    private class MessageSinkProvider : ILoggerProvider
    {
        private IMessageSink? output;

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public MessageSinkProvider(IMessageSink output) => this.output = output;

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => output is null ? NullLogger.Instance : new MessageSinkLogger(name, output));

        protected virtual void Dispose(bool disposing) { output = null; }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private class MessageSinkLogger : ILogger
        {
            private string name;
            private IMessageSink output;

            public MessageSinkLogger(string name, IMessageSink output)
            {
                this.name = name;
                this.output = output;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                var message = new Xunit.Sdk.DiagnosticMessage(name + ":" + formatter(state, exception));
                output.OnMessage(message);
            }
        }
    }
}
