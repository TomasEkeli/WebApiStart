using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backend.Db;

public class DatabaseAvailableHealthCheck : IHealthCheck
{
    readonly DataContext _db;

    public DatabaseAvailableHealthCheck(DataContext db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var connection = _db.CreateDbConnection();
        const string sql = "SELECT 1;";

        try
        {
            var result = await connection.ExecuteScalarAsync<int>(sql);
            return result == 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy();
        }
        finally
        {
            connection.Close();
        }
    }
}