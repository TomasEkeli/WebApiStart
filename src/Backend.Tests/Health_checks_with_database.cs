using System.Net;

namespace Backend.Tests;

public class Health_checks_with_database : TestWithBackend
{
    [Fact]
    public async Task Health_check_returns_ok()
    {
        var response = await _api.GetAsync("/healthz");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
