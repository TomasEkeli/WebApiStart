using System.Net;

namespace Backend.Tests;

public class Monitoring_endpoints_with_database : TestWithBackend
{
    [Fact]
    public async Task Health_check_returns_ok()
    {
        var response = await _api.GetAsync("/healthz");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_check_returns_ok()
    {
        var response = await _api.GetAsync("/readyz");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
