using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Backend.Db;

public partial class DataContext
{
    readonly DbSettings _dbSettings;
    readonly ILogger<DataContext> _logger;

    public DataContext(
        IOptions<DbSettings> dbSettings,
        ILogger<DataContext> logger)
    {
        _dbSettings = dbSettings.Value;
        _logger = logger;
    }

    public IDbConnection CreateDbConnection()
    {
        LogMakingConnection(
            _logger,
            _dbSettings.Database,
            _dbSettings.Host,
            _dbSettings.Port);

        return new NpgsqlConnection(_dbSettings.ConnectionString);
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Debug,
        Message = "Connecting to database {Database} on {Host}:{Port}")]
    public static partial void LogMakingConnection(
        ILogger logger,
        string Database,
        string Host,
        string Port);
}
