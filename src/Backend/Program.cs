using System.Diagnostics;
using Backend.Common;
using Backend.Db;
using Backend.Telemetry;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.EnableOpenTelemetry(exportToConsole: false);
builder.ConfigureDatabaseConnection();

// Add services to the container.
builder.Services.AddFeatureManagement();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDatabase();

var app = builder.Build();

app.MapGet("/", () => $"Hello World! OpenTelemetry trace: {Activity.Current?.TraceId}");
app.MapGet("/version", () => Metadata.CurrentVersion());

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz");

app.MapControllers();

await app.InitializeDatabase();

app?.Run();

// make Program available as a type to reference from tests
public partial class Program {}