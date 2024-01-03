using Backend;
using Backend.Db;
using Backend.Monitoring;
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

app.UseSwagger();
app.UseSwaggerUI();

app.MapMonitoringEndpoints();

app.MapControllers();

app.MapGet("/", () => "Don't look at my back-end!");

await app.InitializeDatabase();

app?.Run();

// make Program available as a type to reference from tests
public partial class Program {}