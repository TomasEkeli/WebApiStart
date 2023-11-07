using System.Diagnostics;

namespace Backend.Db;

public static class DbSetupExtensions
{
    public static WebApplicationBuilder ConfigureDatabaseConnection(
        this WebApplicationBuilder builder)
    {
        builder
            .Services
            .AddOptions<DbSettings>()
            .Bind(builder.Configuration.GetSection(key: DbSettings.Section))
            .ValidateDataAnnotations();

        builder
            .Services
            .AddOptions<AdminDbSettings>()
            .Bind(builder.Configuration.GetSection(key: AdminDbSettings.Section))
            .ValidateDataAnnotations();

        return builder;
    }

    public static IServiceCollection RegisterDatabase(
        this IServiceCollection services)
    {
        services.AddSingleton<DataContext>();
        services.AddSingleton<InitDataContext>();

        services
            .AddHealthChecks()
            .AddCheck<DatabaseAvailableHealthCheck>("Database");

        return services;
    }

    public async static Task<WebApplication> InitializeDatabase(
        this WebApplication app)
    {
        using var activity = Activity.Current?.Source.StartActivity("InitializeDatabse");
        await app.Services.GetRequiredService<InitDataContext>().Init();

        return app;
    }
}