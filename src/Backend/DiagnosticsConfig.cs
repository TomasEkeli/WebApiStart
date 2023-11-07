namespace Backend;

public static class DiagnosticsConfig
{
    public const string Name = "Backend"; // TODO: replace with your app name

    public static class Attributes
    {
        public static class Service
        {
            public const string Host = "service.host-machine";
            public const string Environment = "service.environment";
            public const string Version = "service.version";
        }

        public static class Log
        {
            public const string Level = "log.level";
            public const string Message = "log.message";
            public const string Exception = "log.exception";
            public const string EventId = "log.event-id";
            public const string EventName = "log.event-name";
            public const string CategoryName = "log.category-name";
        }
    }
}
