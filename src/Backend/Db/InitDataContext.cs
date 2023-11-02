using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Backend.Db;

public partial class InitDataContext
{
    const int EventIds = 100;
    readonly AdminDbSettings _admin;
    readonly DbSettings _db;
    readonly ILogger<InitDataContext> _logger;

    public InitDataContext(
        IOptions<AdminDbSettings> initSettings,
        IOptions<DbSettings> dbSettings,
        ILogger<InitDataContext> logger)
    {
        _admin = initSettings.Value;
        _db = dbSettings.Value;
        _logger = logger;
    }

    public async Task Init()
    {
        await InitDatabase();
        await InitTables();
    }

    /**
     * <summary>
     * Create the database if it doesn't exist
     * </summary>
     */
    async Task InitDatabase()
    {
        using NpgsqlConnection connection = new(_admin.ConnectionString);
        var sql = $"select 1 from pg_database where datname='{_db.Database}';";

        LogCheckingDatabase(
            _logger,
            _db.Database,
            _admin.Host,
            _admin.Port);
        var exists = await connection.ExecuteScalarAsync<int>(sql);

        if (exists != 1)
        {
            LogCreatingDatabase(
                _logger,
                _db.Database,
                _admin.Host,
                _admin.Port);
            await connection.ExecuteAsync(
                $"create database {_db.Database};");
        }

        LogDatabaseExists(
            _logger,
            _db.Database,
            _admin.Host,
            _admin.Port);
    }

    /**
     * <summary>
     * Create tables if they do not exist
     * </summary>
     */
    async Task InitTables()
    {
        using NpgsqlConnection connection = new(_db.ConnectionString);
        var sql = @"create table if not exists users (
            id serial primary key,
            username varchar(255) not null unique,
            password_hash varchar(255) not null,
            email varchar(255) not null unique,
            created_at timestamp not null default current_timestamp,
            updated_at timestamp not null default current_timestamp
        );";

        await connection.ExecuteAsync(sql);
    }

    [LoggerMessage(
        EventId = EventIds,
        Level = LogLevel.Debug,
        Message = "Checking if database {Database} exists in {Host}:{Port}")]
    static partial void LogCheckingDatabase(
        ILogger logger,
        string Database,
        string Host,
        string Port);

    [LoggerMessage(
        EventId = EventIds + 1,
        Level = LogLevel.Information,
        Message = "Creating database {Database} in {Host}:{Port}")]
    static partial void LogCreatingDatabase(
        ILogger logger,
        string Database,
        string Host,
        string Port);

    [LoggerMessage(
        EventId = EventIds + 2,
        Level = LogLevel.Debug,
        Message = "Database {Database} exists in {Host}:{Port}")]
    static partial void LogDatabaseExists(
        ILogger logger,
        string Database,
        string Host,
        string Port);
}