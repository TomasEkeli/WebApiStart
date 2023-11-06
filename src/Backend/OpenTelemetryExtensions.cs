using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Backend;

public static class OpenTelemetryExtensions
{
    const string OtlpEndpoint = "http://otel-collector:4317";

    /**
     * <summary>
     * Enables tracing, logging and metrics with OpenTelemetry.
     * </summary>
     * <param name="exportToConsole">will fill the console with text</param>
     */
    public static WebApplicationBuilder EnableOpenTelemetry(
        this WebApplicationBuilder builder,
        bool exportToConsole = true)
    {
        builder.TraceWithOtel(exportToConsole);
        builder.LogWithOtel(exportToConsole);
        builder.ExportMetricsWithOthel(exportToConsole);

        return builder;
    }

    /**
     * <summary>
     * <para>
     * Adds OpenTelemetry logging to the application.
     * </para><para>
     * After calling this method, the default logging provider will remain, but
     * logging is aslo done with OpenTelemetry. This means that the logs with
     * the application-name and other properties in a standard format are produced
     * and exported to the configured exporters (otlp and possibly console).
     * </para>
     * </summary>
     */
    public static WebApplicationBuilder LogWithOtel(
        this WebApplicationBuilder builder,
        bool exportToConsole = true)
    {
        // builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeScopes = true;

                var resourceBuilder = ResourceBuilder
                    .CreateDefault()
                    .AddService(DiagnosticsConfig.Name);

                logging
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(opt =>
                        opt.Endpoint = new Uri(OtlpEndpoint)
                    );

                if (exportToConsole)
                {
                    logging.AddConsoleExporter();
                }
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
        this WebApplicationBuilder builder,
        bool exportToConsole = true)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder
                    .AddService(DiagnosticsConfig.Name)
                )
            .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(opt =>
                            opt.Filter = ctx => ctx.Request.Path != "/healthz"
                        )
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter(opt =>
                            opt.Endpoint = new Uri(OtlpEndpoint)
                        );

                    if (exportToConsole)
                    {
                        tracing.AddConsoleExporter();
                    }
                }
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
        this WebApplicationBuilder builder,
        bool exportToConsole = true)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder
                    .AddService(DiagnosticsConfig.Name)
                )
            .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddMeter(MetricsConfig.Meter.Name)
                        .AddOtlpExporter(opt =>
                            opt.Endpoint = new Uri(OtlpEndpoint)
                        );

                    if (exportToConsole)
                    {
                        metrics
                            .AddConsoleExporter((_, metric_reader_options) =>
                            {
                                metric_reader_options
                                    .PeriodicExportingMetricReaderOptions
                                    .ExportIntervalMilliseconds = 10000;
                            }
                        );
                    }
                }
            );

        return builder;
    }
}
