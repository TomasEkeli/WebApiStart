using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using static Backend.TelemetryConfig.Attributes;
using Microsoft.Extensions.Options;
using Backend.Common;

namespace Backend.Telemetry;

public static class OpenTelemetryExtensions
{
    static OpenTelemetryExportConfig? _config;

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
        builder.ConfigureTelemetry();
        _config = builder.GetTelemetryConfig();

        builder.AddOtelTraces(exportToConsole);
        builder.AddOtelLogging(exportToConsole);
        builder.AddOtelMetrics(exportToConsole);

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
    public static WebApplicationBuilder AddOtelLogging(
        this WebApplicationBuilder builder,
        bool exportToConsole = true)
    {
        // builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(builder.Environment);

            logging
                .SetResourceBuilder(resourceBuilder)
                .AddProcessor(new PutLogsOnActivity())
                .AddOtlpExporter(_ => _.ToConfiguredEndpoint());

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
    public static WebApplicationBuilder AddOtelTraces(
        this WebApplicationBuilder appBuilder,
        bool exportToConsole = true)
    {
        appBuilder.Services
            .AddOpenTelemetry()
            .ConfigureResource(otel =>
                otel
                    .AddService(appBuilder.Environment)
                )
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(opt =>
                        opt.Filter = ctx => ctx.Request.Path != "/healthz"
                    )
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddOtlpExporter(_ => _.ToConfiguredEndpoint());

                if (exportToConsole)
                {
                    tracing.AddConsoleExporter();
                }
            }
            );

        return appBuilder;
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
    public static WebApplicationBuilder AddOtelMetrics(
        this WebApplicationBuilder appBuilder,
        bool exportToConsole = true)
    {
        appBuilder.Services
            .AddOpenTelemetry()
            .ConfigureResource(otel =>
                    otel.AddService(appBuilder.Environment)
                )
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(MetricsConfig.Meter.Name)
                    .AddOtlpExporter(_ => ToConfiguredEndpoint(_));

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

        return appBuilder;
    }

    static ResourceBuilder AddService(
        this ResourceBuilder builder,
        IWebHostEnvironment environment) =>
        builder
            .AddService(TelemetryConfig.Name)
            .AddAttributes(
                new KeyValuePair<string, object>[]
                {
                    new(Service.Host, Environment.MachineName),
                    new(Service.Environment, environment.EnvironmentName),
                    new(Service.Version, Metadata.CurrentVersion())
                }
            );

    static WebApplicationBuilder ConfigureTelemetry(
        this WebApplicationBuilder builder)
    {
        builder
            .Services
            .AddOptions<OpenTelemetryExportConfig>()
            .Bind(
                builder.Configuration.GetSection(
                    OpenTelemetryExportConfig.Section
                )
            )
            .ValidateDataAnnotations();

        return builder;
    }

    static OpenTelemetryExportConfig GetTelemetryConfig(
        this WebApplicationBuilder builder) =>
        builder
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<OpenTelemetryExportConfig>>()
            .Value;

    static OtlpExporterOptions ToConfiguredEndpoint(
        this OtlpExporterOptions options)
    {
        if (_config?.Endpoint is not null)
        {
            options.Endpoint = new Uri(_config.Endpoint);
        }

        return options;
    }
}
