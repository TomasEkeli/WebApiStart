using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Backend;

public static class OpenTelemetryExtensions
{
    /**
     * <summary>
     * Enables tracing, logging and metrics with OpenTelemetry.
     * </summary>
     */
    public static WebApplicationBuilder EnableOpenTelemetry(
        this WebApplicationBuilder builder)
    {
        builder.TraceWithOtel();
        builder.LogWithOtel();
        builder.ExportMetricsWithOthel();

        return builder;
    }

    /**
     * <summary>
     * <para>
     * Replace the default logging provider with OpenTelemetry-logging.
     * </para><para>
     * After calling this method, the default logging provider is removed and
     * logging is done with OpenTelemetry. This means that the logs will contain
     * the application-name and other properties in a standard format, and that
     * the logs will be exported to the configured log-exporter.
     * </para>
     * </summary>
     */
    public static WebApplicationBuilder LogWithOtel(
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

    /**
     * <summary>
     * <para>
     * Add OpenTelemetry tracing to the application.
     * </para><para>
     * After calling this method, the application will emit spans in the
     * OpenTelemetry format, with the application-name and any configured
     * attributes. These spans will be exported to the configured span-exporter.
     * </para>
     * </summary>
     */
    public static WebApplicationBuilder TraceWithOtel(
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
                    .AddAspNetCoreInstrumentation(
                        options => options.Filter = (httpContext) => httpContext.Request.Path != "/healthz"
                    )
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    // TODO: only for demo purposes, remove in production and add
                    // "real world" exporters like OTLP, zipkin, jaeger, etc.
                    .AddConsoleExporter()
            );

        return builder;
    }

    /**
     * <summary>
     * <para>
     * Add OpenTelemetry metrics to the application.
     * </para><para>
     * After calling this method, the application will emit metrics in the
     * OpenTelemetry format, with the application-name and any configured
     * attributes. These metrics will be exported to the configured
     * metric-exporter.
     * </para><para>
     * Metrics are set up in the MetricsConfig class, and can be added to from
     * anywhere in the application by using the static metrics defined there.
     * example:
     * <code>
     * MetricsConfig.Users.Add(1);
     * </code>
     * </para>
     * </summary>
     */
    public static WebApplicationBuilder ExportMetricsWithOthel(
        this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder
                    .AddService(DiagnosticsConfig.Name)
                )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(MetricsConfig.Meter.Name)
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
