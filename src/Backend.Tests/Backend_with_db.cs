using Backend.Db;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using static System.Environment;

namespace Backend.Tests;

/**
 * <summary>
 * A backend connected to a real test-database on db:5432. The database will
 * be dropped when this class is disposed.
 * </summary>
 */
public class Backend_with_db : WebApplicationFactory<Program>, IAsyncLifetime
{
    const string Default_host = "db";
    const string Default_port = "5432";
    const string Default_database = "postgres";
    const string Default_username = "postgres";
    const string Default_password = "postgres";
    const int Default_timeout = 500;

    readonly string _test_database_id = Guid.NewGuid().ToString("N");
    string Database_name => $"test_{_test_database_id}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Execute(
            sql: $"create database {Database_name};",
            commandTimeout: Default_timeout
        );

        builder.ConfigureTestServices(
            service_collection =>
            {
                service_collection
                    .Configure<DbSettings>(
                        configured => configured.Database = $"{Database_name}"
                    );
            }
        );
    }

    public async Task DropDatabase()
    {
        try
        {
            using var connection = new NpgsqlConnection(GetConnectionString());

            await connection.ExecuteAsync(
                sql: $@"drop database if exists {Database_name} with (force);",
                commandTimeout: Default_timeout
            );
        }
        catch (PostgresException ex) when (ex.SqlState == "3D000")
        {
            // already dropped - that's OK
        }
    }

    /**
     * <summary>
     * use this to clean up after tests that create databases if needed
     * </summary>
     */
    public async static Task DropAllTestDatabases()
    {
        using var connection = new NpgsqlConnection(GetConnectionString());

        var drop_statements = await connection.QueryAsync<string>(
            sql: "select 'drop database if exists ' || datname || ' with (force);' " +
                "from pg_database " +
                "where datname like 'test_%';",
            commandTimeout: Default_timeout
        );

        foreach (var drop_statement in drop_statements)
        {
            await connection.ExecuteAsync(drop_statement);
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await DropDatabase();
    }

    static string GetConnectionString()
    {
        // Environment-variables take precedence

        var host = GetEnvironmentVariable("AdminDb__host") ?? Default_host;
        var port = GetEnvironmentVariable("AdminDb__port") ?? Default_port;
        var database = GetEnvironmentVariable("AdminDb__database")
            ?? Default_database;
        var username = GetEnvironmentVariable("AdminDb__username")
            ?? Default_username;
        var password = GetEnvironmentVariable("AdminDb__password")
            ?? Default_password;

        return $"Host={host};" +
            $"Port={port};" +
            $"Username={username};" +
            $"Password={password};" +
            $"Database={database};";
    }
}
