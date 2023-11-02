namespace Backend.Db;

public record DbSettings
{
    public const string Section = "Db";

    public string Host { get; set; } = "";
    public string Port { get; set; } = "";
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    string? _connectionString;
    public string ConnectionString =>
        _connectionString ??= ConcatenateToConnectionString(this);

    static string ConcatenateToConnectionString(DbSettings settings) =>
        $@"Host={settings.Host
        };Port={settings.Port
        };Database={settings.Database
        };Username={settings.Username
        };Password={settings.Password};";
}