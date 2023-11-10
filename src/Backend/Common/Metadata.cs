namespace Backend.Common;

public static class Metadata
{
    public static string CurrentVersion() =>
       typeof(Program)
       .Assembly
       .GetName()
       .Version
       ?.ToString()
       ?? "unknown";
}