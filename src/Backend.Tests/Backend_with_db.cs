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
    const string default_host = "db";
    const string default_port = "5432";
    const string default_database = "postgres";
    const string default_username = "postgres";
    const string default_password = "postgres";

    readonly string _test_database_id = Guid.NewGuid().ToString("N");
    string Database_name => $"test_{_test_database_id}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());

        connection.Execute($"create database {Database_name};");
        connection.Close();

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
                $"drop database {Database_name} with (force);");
            connection.Close();
        }
        catch (PostgresException ex) when (ex.SqlState == "3D000")
        {
            // already dropped - that's OK
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        DropDatabase().Wait();
        base.Dispose(disposing);
    }

    static string GetConnectionString()
    {
        // Environment-variables take precedence

        var host = GetEnvironmentVariable("AdminDb__host") ?? default_host;
        var port = GetEnvironmentVariable("AdminDb__port") ?? default_port;
        var database = GetEnvironmentVariable("AdminDb__database")
            ?? default_database;
        var username = GetEnvironmentVariable("AdminDb__username")
            ?? default_username;
        var password = GetEnvironmentVariable("AdminDb__password")
            ?? default_password;

        return $"Host={host};" +
            $"Port={port};" +
            $"Username={username};" +
            $"Password={password};" +
            $"Database={database};";
    }
}
