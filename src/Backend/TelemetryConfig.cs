namespace Backend;
public record TelemetryConfig
{
    public const string Section = "OpenTelemetry";

    public string? ExporterEndpoint { get; init; }
}
