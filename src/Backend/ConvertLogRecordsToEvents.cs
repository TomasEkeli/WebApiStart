using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Logs;
using static Backend.DiagnosticsConfig.Attributes;

namespace Backend;
public class ConvertLogRecordsToEvents : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        if (data is null)
        {
            return;
        }

        Activity.Current?.AddEvent(data.ToActivityEvent());
    }
}

public static class LogRecordExtensions
{
    public static ActivityEvent ToActivityEvent(this LogRecord logRecord) =>
        new(
            name: $"[{logRecord.LogLevel}] {logRecord.FormattedMessage}",
            timestamp: logRecord.Timestamp,
            tags: logRecord.ToTags()
        );

    public static ActivityTagsCollection ToTags(this LogRecord logRecord)
    {
        var tags = new ActivityTagsCollection
        {
            [Log.Level] = logRecord.LogLevel,
            [Log.EventId] = logRecord.EventId,
            [Log.EventName] = logRecord.EventId.Name,
            [Log.CategoryName] = logRecord.CategoryName,
            [Log.Message] = logRecord.FormattedMessage,
            [Log.Exception] = logRecord.Exception
        };

        if (logRecord.Attributes is not null)
        {
            foreach (var attribute in logRecord.Attributes)
            {
                tags[attribute.Key] = attribute.Value;
            }
        }
        return tags;
    }
}