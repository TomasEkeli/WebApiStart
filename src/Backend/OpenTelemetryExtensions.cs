using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Backend;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder UseOtelLogging(
        this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeScopes = true;

                var resourceBuilder = ResourceBuilder
                    .CreateDefault()
                    .AddService(DiagnosticsConfig.Name);

                logging
                    .SetResourceBuilder(resourceBuilder)
                    // TODO: only for demo purposes, remove in production and add
                    // "real world" exporters like OTLP, zipkin, jaeger, etc.
                    .AddConsoleExporter();
            }
        );

        return builder;
    }
}
