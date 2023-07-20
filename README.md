# Testing.Playwright

Quick project loosely based on the article [End-to-End test a Blazor App with Playwright](https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6) by Xavier Solau to allow testing an ASP.NET Core application with `Microsoft.Playwright`.

This also includes a Fixture logging system which was taken from one of my other projects, which used the same underlying `Microsoft.AspNetCore.Mvc.Testing` package.

The project uses xunit as the logging engine, although I'm sure it could be adapted to nunit easily enough.

This code 'hardcodes' the type in `PlaywrightWebApplicationFactory` to the web application `Program` class, although it could easily be made generic like the underlying `WebApplicationFactory`.

As xunit does not allow passing any parameters to a fixture, there are types which explicitly set the `Environment` (via `UseEnvironment`) to be `Development`, `Staging`, or `Production`.
Again you could inherit the fixture class and manually set a different environment if needed.

This implementation is 'opinionated' in that it always uses Chromium as the browser. Xavier Solau`s solution shows how it would be possible to allow the test to decide which browser to use.
This would also potentially allow the tests to be run for all browsers using `InlineData`.

This works from inside VS2022 and from the command line with
```shell
dotnet test --logger "console;verbosity=detailed"
```
or just
```shell
dotnet test
```

