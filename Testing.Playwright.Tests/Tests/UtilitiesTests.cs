using Testing.Playwright.Tests.Utilities;

namespace Testing.Playwright.Tests.Playwright;

public class UtilitiesTests
{
    [Fact(Skip = "Interferes with other tests using Playwright")]
    public void UninstallPlaywright()
    {
        PlaywrightUtilities.Uninstall();
    }

    [Fact(Skip = "Can only be tested on a clean install")]
    public void InstallPlaywright()
    {
        PlaywrightUtilities.InstallPlaywright();
    }

    [Fact(Skip = "Is interactive")]
    public void ShowWebkitTrace()
    {
        PlaywrightUtilities.ShowTrace("Webkit_CanTrace.zip");
    }

    [Fact(Skip = "Is interactive")]
    public void ShowDefaultTrace()
    {
        PlaywrightUtilities.ShowTrace("CanTrace.zip");
    }

    [Fact(Skip = "Is interactive")]
    public void ShowFirefoxTrace()
    {
        _ = PlaywrightUtilities.ShowTraceAsync("Testing.Playwright.Tests.UnitTestFirefox_CanTrace.zip");
    }
}
