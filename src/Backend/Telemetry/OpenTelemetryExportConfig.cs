namespace Backend.Telemetry;

public record OpenTelemetryExportConfig
{
    public const string Section = "OpenTelemetryExport";

    public string? Endpoint { get; init; }
}
