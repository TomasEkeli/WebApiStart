using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

    public static WebApplicationBuilder UseOtelTracing(
        this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder
                    .AddService(DiagnosticsConfig.Name)
                )
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    // TODO: only for demo purposes, remove in production and add
                    // "real world" exporters like OTLP, zipkin, jaeger, etc.
                    .AddConsoleExporter()
            );

        return builder;
    }

    public static WebApplicationBuilder UseOtelMetrics(
        this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder
                    .AddService(DiagnosticsConfig.Name)
                )
            .WithMetrics(metrics =>
                metrics.AddAspNetCoreInstrumentation()
                // TODO: only for demo purposes, remove in production and add
                // "real world" exporters like OTLP, zipkin, jaeger, etc.
                .AddConsoleExporter((_, metric_reader_options) =>
                {
                    metric_reader_options
                        .PeriodicExportingMetricReaderOptions
                        .ExportIntervalMilliseconds = 10000;
                }
                )
            );

        return builder;
    }
}
