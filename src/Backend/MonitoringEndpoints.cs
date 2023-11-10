using Backend.Common;
using HealthChecks.UI.Client;

namespace Backend;

public static class MonitoringEndpoints
{
  public static void MapMonitoringEndpoints(this WebApplication app)
  {
    app
      .MapGet("/version", () => Metadata.CurrentVersion())
      // this endpoint is not part of the API, it is an infrastructure endpoint
      .ExcludeFromDescription();

    app.MapHealthChecks(
      "/healthz",
      new()
      {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });
    app.MapHealthChecks("/readyz");
  }
}