using Xunit.Abstractions;

namespace Testing.Playwright.Tests
{
    public class UnitTestDefault : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactory>
    {
        public UnitTestDefault(PlaywrightWebApplicationFactory webApplication, ITestOutputHelper outputHelper) : 
            base(webApplication, outputHelper)
        {
        }
    }
}
