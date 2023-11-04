using System.Diagnostics;
using Backend;
using Backend.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection(key: DbSettings.Section));
builder.Services.Configure<AdminDbSettings>(
    builder.Configuration.GetSection(key: AdminDbSettings.Section));

builder.EnableOpenTelemetry();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataContext>();
builder.Services.AddSingleton<InitDataContext>();

builder.Services
    .AddHealthChecks()
        .AddCheck<DatabaseAvailableHealthCheck>("Database");

var app = builder.Build();

app.MapGet("/", () => $"Hello World! OpenTelemetry trace: {Activity.Current?.TraceId}");

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthz");

app.MapControllers();

// initialize database, if needed
await app.Services.GetRequiredService<InitDataContext>().Init();

app.Run();

// make Program available as a type to reference from tests
public partial class Program {}