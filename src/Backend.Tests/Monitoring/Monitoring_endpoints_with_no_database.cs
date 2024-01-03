using System.Net;

namespace Backend.Tests.Monitoring;

public class Monitoring_endpoints_with_no_database : TestWithBackend
{
    [Fact]
    public async Task Health_check_returns_service_unavailable()
    {
        await _backend.DropDatabase();

        var response = await _api.GetAsync("/healthz");

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Health_check_indicates_unhealthy()
    {
        await _backend.DropDatabase();

        var response = await _api.GetAsync("/healthz");

        var content = await response.Content.ReadAsStringAsync();

        content.ShouldContain("Unhealthy");
    }

    [Fact]
    public async Task Ready_check_returns_service_unavailable()
    {
        await _backend.DropDatabase();

        var response = await _api.GetAsync("/readyz");

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Ready_check_indicates_unhealthy()
    {
        await _backend.DropDatabase();

        var response = await _api.GetAsync("/readyz");

        var content = await response.Content.ReadAsStringAsync();

        content.ShouldContain("Unhealthy");
    }
}
