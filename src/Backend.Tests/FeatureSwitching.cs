using System.Net.Http.Json;

namespace Backend.Tests;

public class FeatureSwitching : TestWithBackend
{
    [Fact]
    public async Task Get_weatherforecast_returns_clipped_forecast_sometimes()
    {
        // call the endpoint 100 times - we should get a mix of 3 and 5 day forecasts
        var tasks = Enumerable.Range(0, 25).Select(_ => _api.GetFromJsonAsync<IEnumerable<Forecast>>("/weatherforecast"));
        var responses = await Task.WhenAll(tasks);

        var clipped = responses.Count(r => r!.Count() < 5);
        var full = responses.Count(r => r!.Count() >= 5);

        clipped.ShouldBeGreaterThan(5);
        full.ShouldBeGreaterThan(5);
    }

    // do not re-use the class from the backend project to fail if the type changes
    record Forecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF  { get; set; }

        public string? Summary { get; set; }
    }
}
