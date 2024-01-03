namespace Backend.Tests.Root;

public class BasicApiTests : TestWithBackend
{
    [Fact]
    public async Task Get_weatherforecast_returns_ok()
    {
        var response = await _api.GetAsync("/");

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }
}
