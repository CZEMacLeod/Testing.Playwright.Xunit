using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics.CodeAnalysis;
using Testing.Playwright.Tests.Utilities;
using Xunit.Abstractions;

namespace Testing.Playwright.Tests;

public class PlaywrightWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private IPlaywright? playwright;
    private IBrowser? browser;
    private string? uri;
    private readonly IMessageSink output;

    private static int nextPort = 0;

    public string? Uri => uri;

    #region "Overridable Properties"
    // Properties in this region can be overridden in a derived type and used as a fixture
    // If you create multiple derived fixtures, and derived tests injecting each one into a base test class
    // you can easily setup a test matrix for running a set of tests against multiple browsers and/or environments
    public virtual string? Environment { get; }
    public virtual PlaywrightBrowserType BrowserType => PlaywrightBrowserType.Chromium;

    protected virtual BrowserTypeLaunchOptions LaunchOptions { get; } = new BrowserTypeLaunchOptions()
    {
        Headless = true
    };

    public virtual bool AddMessageSinkProvider => true;
    public virtual LogLevel MinimumLogLevel => LogLevel.Trace;
    #endregion

    protected virtual IBrowserType GetBrowser() => BrowserType switch
    {
        PlaywrightBrowserType.Chromium=> playwright?.Chromium,
        PlaywrightBrowserType.Firefox => playwright?.Firefox,
        PlaywrightBrowserType.Webkit => playwright?.Webkit,
        _ => throw new ArgumentOutOfRangeException(nameof(BrowserType))
    } ?? throw new InvalidOperationException("Could not get browser type");

    public PlaywrightWebApplicationFactory(IMessageSink output) => this.output = output;

    [MemberNotNull(nameof(uri))]
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (Environment is not null)
        {
            builder.UseEnvironment(Environment);
        }
        builder.ConfigureLogging(logging =>
        {
            logging.SetMinimumLevel(MinimumLogLevel);
            if (AddMessageSinkProvider)
            {
                logging.AddXunit(output);
            }
        });

        // We randomize the server port so we ensure that any hard coded Uri's fail in the tests.
        // This also allows multiple servers to run during the tests.
        var port = 5000 + Interlocked.Add(ref nextPort, 10 + System.Random.Shared.Next(10));
        uri = $"http://localhost:{port}";

        // We the testHost, which can be used with HttpClient with a custom transport
        // It is assumed that the return of CreateHost is a host based on the TestHost Server.
        var testHost = base.CreateHost(builder);

        // Now we reconfigure the builder to use kestrel so we have an http listener that can be used by playwright
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel(options =>
        {
            options.ListenLocalhost(port);
        }));
        var host = base.CreateHost(builder);
        
        UpdateUriFromHost(host);    // For some reason, the kestrel server host does not seem to return the addresses.

        return new CompositeHost(testHost, host);
    }

    private void UpdateUriFromHost(IHost host)
    {
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
    }

    public async Task<IPage> CreatePlaywrightPageAsync()
    {
        var server = Server;        // Ensure Server is initialized
        await InitializeAsync();    // Ensure Playwright is initialized

        return await browser.NewPageAsync(new BrowserNewPageOptions()
        {
            BaseURL = uri
        });
    }

    [MemberNotNull(nameof(playwright), nameof(browser))]
    public async Task InitializeAsync()
    {
        PlaywrightUtilities.InstallPlaywright(BrowserType);
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        playwright ??= (await Microsoft.Playwright.Playwright.CreateAsync()) ?? throw new InvalidOperationException();
        browser ??= (await GetBrowser().LaunchAsync(LaunchOptions)) ?? throw new InvalidOperationException();
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
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

    // CompositeHost is based on https://github.com/xaviersolau/DevArticles/blob/e2e_test_blazor_with_playwright/MyBlazorApp/MyAppTests/WebTestingHostFactory.cs
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
}
