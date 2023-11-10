using System.Diagnostics;
using Backend.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    readonly IFeatureManager _features;
    readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IFeatureManager features,
        ILogger<WeatherForecastController> logger)
    {
        _features = features;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        using var activity = Activity.Current?.Source.StartActivity("GetWeatherForecast");

        _logger.LogInformation("Get the weather");

        var clipped = await _features.IsEnabledAsync(FeatureFlags.ClippedForecasts);
        var numberOfDays = clipped ? 3 : 5;

        var weather = Enumerable.Range(1, numberOfDays).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        _logger.LogInformation("Weather generated");
        return weather;
    }
}
