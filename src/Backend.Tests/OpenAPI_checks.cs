using System.Net;

namespace Backend.Tests;

public class OpenAPI_checks : TestWithBackend
{
    [Fact]
    public async Task Swagger_returns_ok()
    {
        var response = await _api.GetAsync("/swagger");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
