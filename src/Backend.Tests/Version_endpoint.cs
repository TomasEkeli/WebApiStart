namespace Backend.Tests;

public class Version_endpoint : TestWithBackend
{
    [Fact]
    public async Task Returns_version()
    {
        var response = await _api.GetAsync("/version");

        var content = await response.Content.ReadAsStringAsync();

        var expectedVersion = typeof(Program).Assembly.GetName().Version?.ToString();

        content.ShouldContain(expectedVersion!);
    }
}
