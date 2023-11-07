using System.Diagnostics;
using Backend.Db;
using Backend.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.EnableOpenTelemetry(exportToConsole: true);
builder.ConfigureDatabaseConnection();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDatabase();

var app = builder.Build();

app.MapGet("/", () => $"Hello World! OpenTelemetry trace: {Activity.Current?.TraceId}");

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthz");

app.MapControllers();

await app.InitializeDatabase();

app?.Run();

// make Program available as a type to reference from tests
public partial class Program {}